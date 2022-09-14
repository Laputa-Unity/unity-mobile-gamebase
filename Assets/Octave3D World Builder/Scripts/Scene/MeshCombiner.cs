#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace O3DWB
{
    public class MeshCombiner
    {
        private class MeshCombineMaterial
        {
            public Material Material;
            public List<MeshInstance> MeshInstances = new List<MeshInstance>();
        }

        private class MeshInstance
        {
            public Mesh SourceMesh;
            public int SubmeshIndex;
            public Transform MeshTransform;
        }

        private class CombinedMeshData
        {
            public List<Vector4> VertTangents;
            public List<Vector3> VertPositions;
            public List<Vector3> VertNormals;
            public List<Vector2> VertUV1;
            public List<Vector2> VertUV2;
            public List<Vector2> VertUV3;
            public List<Vector2> VertUV4;
            public List<Color> VertColors;
            public List<int> VertIndices;
            public int CurrentVertIndex;

            public CombinedMeshData(int combinedNumVertsGuess)
            {
                VertTangents = new List<Vector4>(combinedNumVertsGuess);
                VertPositions = new List<Vector3>(combinedNumVertsGuess);
                VertNormals = new List<Vector3>(combinedNumVertsGuess);
                VertUV1 = new List<Vector2>(combinedNumVertsGuess);
                VertUV2 = new List<Vector2>(combinedNumVertsGuess);
                VertUV3 = new List<Vector2>(combinedNumVertsGuess);
                VertUV4 = new List<Vector2>(combinedNumVertsGuess);
                VertColors = new List<Color>(combinedNumVertsGuess);

                // Assume each group of 3 verts forms a triangle
                VertIndices = new List<int>(combinedNumVertsGuess / 3);
            }

            public void Reset()
            {
                VertTangents.Clear();
                VertPositions.Clear();
                VertNormals.Clear();
                VertUV1.Clear();
                VertUV2.Clear();
                VertUV3.Clear();
                VertUV4.Clear();
                VertColors.Clear();
                VertIndices.Clear();
                CurrentVertIndex = 0;
            }

            public void AddCurrentVertIndex()
            {
                // Note: Assumes that vertices are stored in the combined mesh buffers in the same
                //       way as they are encountered when reading the vertex data using indices
                //       from the source mesh.
                VertIndices.Add(CurrentVertIndex++);
            }

            public void ReverseWindingOrderForLastTriangle()
            {
                int lastIndexPtr = VertIndices.Count - 1;
                int tempIndex = VertIndices[lastIndexPtr];
                VertIndices[lastIndexPtr] = VertIndices[lastIndexPtr - 2];
                VertIndices[lastIndexPtr - 2] = tempIndex;
            }
        }

        private MeshCombineSettings _combineSettings;
        public MeshCombineSettings CombineSettings { set { if (value != null) _combineSettings = value; } }

        public void CombineChildren(List<GameObject> ignoreObjects)
        {
            if (_combineSettings == null) return;
            if (_combineSettings.ChildrenCombineSourceParent == null)
            {
                EditorUtility.DisplayDialog("Missing Parent", "Please specify the parent of the objects which must be combined.", "Ok");
                return;
            }

            List<GameObject> allMeshObjects = _combineSettings.ChildrenCombineSourceParent.GetAllMeshObjectsInHierarchy();
            if(ignoreObjects != null && ignoreObjects.Count != 0) allMeshObjects.RemoveAll(item => ignoreObjects.Contains(item));

            List<MeshCombineMaterial> meshCombineMaterials = GetMeshCombineMaterials(allMeshObjects, _combineSettings.ChildrenCombineSourceParent);
            if (meshCombineMaterials.Count == 0)
            {
                EditorUtility.DisplayDialog("No Combinable Objects", "The specified parent does not contain any objects which can be combined. Please check the following: \n\r" +
                                            "-the parent must contain mesh child objects; \n\r " + 
                                            "-check the \'Combine static/dynamic meshes\' toggles to ensure that the objects can actually be combined; \n\r" + 
                                            "-if the objects belong to a hierarchy, make sure to unhceck the \'Ignore objects in hierarchies\' toggle.", "Ok");
                return;
            }

            Combine(meshCombineMaterials, null);

            if (_combineSettings.DisableSourceParent) _combineSettings.ChildrenCombineSourceParent.SetActive(false);

            EditorUtility.DisplayDialog("Done!", "Mesh objects combined successfully!", "Ok");
        }

        public void CombineSelectedObjects(List<GameObject> selectedObjects, List<GameObject> ignoreObjects)
        {
            if (_combineSettings == null || selectedObjects == null) return;
            if(selectedObjects.Count == 0)
            {
                EditorUtility.DisplayDialog("No Selected Objects", "The mesh combine process can not start because there are no objects currently selected.", "Ok");
                return;
            }

            List<GameObject> allMeshObjects = new List<GameObject>(selectedObjects.Count);
            foreach(var selectedObject in selectedObjects)
            {
                if (selectedObject.HasMeshFilterWithValidMesh()) allMeshObjects.Add(selectedObject);
            }
            if (ignoreObjects != null && ignoreObjects.Count != 0) allMeshObjects.RemoveAll(item => ignoreObjects.Contains(item));
         
            List<MeshCombineMaterial> meshCombineMaterials = GetMeshCombineMaterials(allMeshObjects, null);
            if (meshCombineMaterials.Count == 0)
            {
                EditorUtility.DisplayDialog("No Combinable Objects", "The current selection does not contain any objects which can be combined. Please check the following: \n\r" +
                                            "-the selection must contain mesh objects; \n\r " +
                                            "-check the \'Combine static/dynamic meshes\' toggles to ensure that the objects can actually be combined; \n\r" +
                                            "-if the objects belong to a hierarchy, make sure to unhceck the \'Ignore objects in hierarchies\' toggle.", "Ok");
                return;
            }

            Combine(meshCombineMaterials, _combineSettings.SelectionCombineDestinationParent);
          
            if(_combineSettings.SelCombineMode == MeshCombineSettings.CombineMode.Replace)
            {
                List<GameObject> parents = GameObjectExtensions.GetParents(selectedObjects);
                foreach(var parent in parents)
                {
                    UndoEx.DestroyObjectImmediate(parent);
                }
            }

            EditorUtility.DisplayDialog("Done!", "Mesh objects combined successfully!", "Ok");
        }

        private List<MeshCombineMaterial> GetMeshCombineMaterials(List<GameObject> meshObjects, GameObject sourceParent)
        {
            var meshCombineMaterialsMap = new Dictionary<Material, MeshCombineMaterial>();
            var meshCombineMaterials = new List<MeshCombineMaterial>();

            Transform sourceParentTransform = sourceParent != null ? sourceParent.transform : null;
            foreach (var meshObject in meshObjects)
            {
                Transform meshTransform = meshObject.transform;
                if (_combineSettings.IgnoreObjectsInHierarchies)
                {
                    if (sourceParentTransform != null)
                    {
                        if (meshTransform.parent != sourceParentTransform || meshTransform.childCount != 0) continue;
                    }
                    else
                    {
                        if (meshTransform.parent != null || meshTransform.childCount != 0) continue;
                    }
                }

                if (!_combineSettings.CombineStaticMeshes && meshTransform.gameObject.isStatic) continue;
                if (!_combineSettings.CombineDynamicMeshes && !meshTransform.gameObject.isStatic) continue;

                MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.sharedMesh == null) continue;

                MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();
                if (meshRenderer == null) continue;

                Mesh sharedMesh = meshFilter.sharedMesh;
                int numSharedMaterials = meshRenderer.sharedMaterials.Length;
                for (int submeshIndex = 0; submeshIndex < sharedMesh.subMeshCount; ++submeshIndex)
                {
                    // Note: How can this happen?
                    if (submeshIndex >= numSharedMaterials) break;

                    Material subMeshMaterial = meshRenderer.sharedMaterials[submeshIndex];
                    MeshCombineMaterial meshCombineMaterial = null;

                    if (!meshCombineMaterialsMap.ContainsKey(subMeshMaterial))
                    {
                        meshCombineMaterial = new MeshCombineMaterial();
                        meshCombineMaterial.Material = subMeshMaterial;

                        meshCombineMaterialsMap.Add(subMeshMaterial, meshCombineMaterial);
                        meshCombineMaterials.Add(meshCombineMaterial);
                    }
                    else meshCombineMaterial = meshCombineMaterialsMap[subMeshMaterial];

                    var meshInstance = new MeshInstance();
                    meshInstance.SourceMesh = sharedMesh;
                    meshInstance.SubmeshIndex = submeshIndex;
                    meshInstance.MeshTransform = meshTransform;
                    meshCombineMaterial.MeshInstances.Add(meshInstance);
                }
            }

            return meshCombineMaterials;
        }

        private void Combine(List<MeshCombineMaterial> meshCombineMaterials, GameObject combinedMeshesParent)
        {
            List<GameObject> allCombinedMeshObjects = new List<GameObject>(100);
            if (combinedMeshesParent == null)
            {
                combinedMeshesParent = new GameObject(GetNameForCombinedMeshesParent());
                UndoEx.RegisterCreatedGameObject(combinedMeshesParent);
            }
            for (int combMaterialIndex = 0; combMaterialIndex < meshCombineMaterials.Count; ++combMaterialIndex)
            {
                MeshCombineMaterial meshCombMaterial = meshCombineMaterials[combMaterialIndex];

                List<GameObject> combineMeshdObjects = Combine(meshCombMaterial, combinedMeshesParent);
                if (_combineSettings.WeldVertexPositions)
                {
                    if (_combineSettings.WeldVertexPositionsOnlyForCommonMaterial) WeldVertexPositionsForMeshObjects(combineMeshdObjects);
                    else allCombinedMeshObjects.AddRange(combineMeshdObjects);
                }
            }

            if (_combineSettings.WeldVertexPositions && !_combineSettings.WeldVertexPositionsOnlyForCommonMaterial) WeldVertexPositionsForMeshObjects(allCombinedMeshObjects);
            if(_combineSettings.CollHandling == MeshCombineSettings.ColliderHandling.Preserve) PreserveColliders(meshCombineMaterials, combinedMeshesParent);
        }

        private List<GameObject> Combine(MeshCombineMaterial meshCombineMaterial, GameObject combinedMeshesParent)
        {
            List<MeshInstance> meshInstances = meshCombineMaterial.MeshInstances;
            if (meshInstances.Count == 0) return new List<GameObject>();

            int maxNumMeshVerts = GetMaxNumberOfMeshVerts();
            const int numVertsPerMeshGuess = 500;
            int totalNumVertsGuess = meshCombineMaterial.MeshInstances.Count * numVertsPerMeshGuess;
            var combinedMeshData = new CombinedMeshData(totalNumVertsGuess);

            List<GameObject> combinedMeshObjects = new List<GameObject>(100);
            for (int meshInstanceIndex = 0; meshInstanceIndex < meshInstances.Count; ++meshInstanceIndex)
            {
                MeshInstance meshInstance = meshInstances[meshInstanceIndex];
                Mesh mesh = meshInstance.SourceMesh;
                if (mesh.vertexCount == 0) continue;

                EditorUtility.DisplayProgressBar("Combining Meshes", "Processing material: " + meshCombineMaterial.Material.name, (float)meshInstanceIndex / meshInstances.Count);

                Matrix4x4 worldMatrix = meshInstance.MeshTransform.localToWorldMatrix;
                Matrix4x4 worldInverseTranspose = worldMatrix.inverse.transpose;

                int numNegativeScaleComps = 0;
                Vector3 worldScale = meshInstance.MeshTransform.lossyScale;
                if (worldScale[0] < 0.0f) ++numNegativeScaleComps;
                if (worldScale[1] < 0.0f) ++numNegativeScaleComps;
                if (worldScale[2] < 0.0f) ++numNegativeScaleComps;
                bool reverseVertexWindingOrder = (numNegativeScaleComps % 2 != 0);

                int[] submeshVertIndices = mesh.GetTriangles(meshInstance.SubmeshIndex);
                if (submeshVertIndices.Length == 0) continue;

                Vector4[] vertTangents = mesh.tangents;
                Vector3[] vertPositions = mesh.vertices;
                Vector3[] vertNormals = mesh.normals;
                Vector2[] vertUV1 = mesh.uv;
                Vector2[] vertUV2 = mesh.uv2;
                Vector2[] vertUV3 = mesh.uv3;
                Vector2[] vertUV4 = mesh.uv4;
                Color[] vertColors = mesh.colors;

                foreach (var vertIndex in submeshVertIndices)
                {
                    if (vertTangents.Length != 0)
                    {
                        Vector3 transformedTangent = new Vector3(vertTangents[vertIndex].x, vertTangents[vertIndex].y, vertTangents[vertIndex].z);
                        transformedTangent = worldInverseTranspose.MultiplyVector(transformedTangent);
                        transformedTangent.Normalize();

                        combinedMeshData.VertTangents.Add(new Vector4(transformedTangent.x, transformedTangent.y, transformedTangent.z, vertTangents[vertIndex].w));
                    }

                    if (vertNormals.Length != 0)
                    {
                        Vector3 transformedNormal = worldInverseTranspose.MultiplyVector(vertNormals[vertIndex]);
                        transformedNormal.Normalize();

                        combinedMeshData.VertNormals.Add(transformedNormal);
                    }

                    if (vertPositions.Length != 0) combinedMeshData.VertPositions.Add(worldMatrix.MultiplyPoint(vertPositions[vertIndex]));
                    if (vertColors.Length != 0) combinedMeshData.VertColors.Add(vertColors[vertIndex]);
                    if (vertUV1.Length != 0) combinedMeshData.VertUV1.Add(vertUV1[vertIndex]);
                    if (vertUV3.Length != 0) combinedMeshData.VertUV2.Add(vertUV3[vertIndex]);
                    if (vertUV4.Length != 0) combinedMeshData.VertUV3.Add(vertUV4[vertIndex]);
                    if (vertUV2.Length != 0 && !_combineSettings.GenerateLightmapUVs) combinedMeshData.VertUV2.Add(vertUV2[vertIndex]);

                    combinedMeshData.AddCurrentVertIndex();

                    int numIndices = combinedMeshData.VertIndices.Count;
                    if (reverseVertexWindingOrder && numIndices % 3 == 0) combinedMeshData.ReverseWindingOrderForLastTriangle();

                    int numMeshVerts = combinedMeshData.VertPositions.Count;
                    if (combinedMeshData.VertIndices.Count % 3 == 0 && (maxNumMeshVerts - numMeshVerts) < 3)
                    {
                        combinedMeshObjects.Add(CreateCombinedMeshObject(combinedMeshData, meshCombineMaterial.Material, combinedMeshesParent));
                        combinedMeshData.Reset();
                    }
                }
            }

            combinedMeshObjects.Add(CreateCombinedMeshObject(combinedMeshData, meshCombineMaterial.Material, combinedMeshesParent));
            EditorUtility.ClearProgressBar();

            return combinedMeshObjects;
        }

        private GameObject CreateCombinedMeshObject(CombinedMeshData combinedMeshData, Material meshMaterial, GameObject combinedMeshesParent)
        {
            Mesh combinedMesh = CreateCombinedMesh(combinedMeshData);

            GameObject combinedMeshObject = new GameObject(_combineSettings.CombinedObjectName);
            UndoEx.RegisterCreatedGameObject(combinedMeshObject);
            combinedMeshObject.transform.parent = combinedMeshesParent.transform;
            combinedMeshObject.isStatic = _combineSettings.MakeCombinedMeshesStatic ? true : false;

            MeshFilter meshFilter = combinedMeshObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = combinedMesh;

            MeshRenderer meshRenderer = combinedMeshObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = meshMaterial;

            Vector3 meshPivotPt = MeshCombineSettings.MeshPivotToWorldPoint(combinedMeshObject, _combineSettings.CombinedMeshesPivot);
            combinedMeshObject.SetMeshPivotPoint(meshPivotPt);

            if (_combineSettings.CollHandling == MeshCombineSettings.ColliderHandling.CreateNew)
            {
                if(_combineSettings.NewCollidersType == MeshCombineSettings.ColliderType.Mesh)
                {
                    MeshCollider meshCollider = combinedMeshObject.AddComponent<MeshCollider>();
                    if (_combineSettings.UseConvexMeshColliders) meshCollider.convex = true;
                    else meshCollider.convex = false;

                    if (_combineSettings.NewCollidersAreTriggers) meshCollider.isTrigger = true;
                    else meshCollider.isTrigger = false;
                }
                else
                if(_combineSettings.NewCollidersType == MeshCombineSettings.ColliderType.Box)
                {
                    BoxCollider boxCollider = combinedMeshObject.AddComponent<BoxCollider>();
                    if (_combineSettings.NewCollidersAreTriggers) boxCollider.isTrigger = true;
                    else boxCollider.isTrigger = false;
                }
            }

            return combinedMeshObject;
        }

        private Mesh CreateCombinedMesh(CombinedMeshData combinedMeshData)
        {
            Mesh combinedMesh = new Mesh();
            combinedMesh.name = _combineSettings.CombinedMeshName;

            combinedMesh.vertices = combinedMeshData.VertPositions.ToArray();
            if (combinedMeshData.VertTangents.Count != 0) combinedMesh.tangents = combinedMeshData.VertTangents.ToArray();
            if (combinedMeshData.VertNormals.Count != 0) combinedMesh.normals = combinedMeshData.VertNormals.ToArray();
            if (combinedMeshData.VertUV1.Count != 0) combinedMesh.uv = combinedMeshData.VertUV1.ToArray();
            if (combinedMeshData.VertUV3.Count != 0) combinedMesh.uv3 = combinedMeshData.VertUV3.ToArray();
            if (combinedMeshData.VertUV4.Count != 0) combinedMesh.uv4 = combinedMeshData.VertUV4.ToArray();
            if (combinedMeshData.VertColors.Count != 0) combinedMesh.colors = combinedMeshData.VertColors.ToArray();
            combinedMesh.SetIndices(combinedMeshData.VertIndices.ToArray(), MeshTopology.Triangles, 0);

            if (_combineSettings.GenerateLightmapUVs) Unwrapping.GenerateSecondaryUVSet(combinedMesh);
            else if (combinedMeshData.VertUV2.Count != 0) combinedMesh.uv2 = combinedMeshData.VertUV2.ToArray();

            combinedMesh.UploadMeshData(_combineSettings.MarkNoLongerReadable);
            SaveCombinedMeshAsAsset(combinedMesh);

            return combinedMesh;
        }

        private static void SaveCombinedMeshAsAsset(Mesh combinedMesh)
        {
            string absoluteFolderPath = FileSystem.GetToolFolderName() + "/Octave3D Combined Meshes";
            if (!Directory.Exists(absoluteFolderPath)) Directory.CreateDirectory(absoluteFolderPath);

            AssetDatabase.CreateAsset(combinedMesh, absoluteFolderPath + "/" + combinedMesh.name + "_" + combinedMesh.GetHashCode() + ".asset");
            AssetDatabase.SaveAssets();
        }

        private int GetMaxNumberOfMeshVerts()
        {
            if (_combineSettings.GenerateLightmapUVs) return 32000;
            return 65000;
        }

        private string GetNameForCombinedMeshesParent()
        {
            return "cmbParent_Octave3D";
        }

        private void WeldVertexPositionsForMeshObjects(List<GameObject> meshObjects)
        {
            if (meshObjects.Count == 0) return;

            var allMeshes = new List<Mesh>(meshObjects.Count);
            foreach (var gameObject in meshObjects)
            {
                Mesh mesh = gameObject.GetMeshFromMeshFilter();
                if (mesh != null) allMeshes.Add(mesh);
            }

            WeldVertexPositionsForMeshes(allMeshes);
        }

        private void WeldVertexPositionsForMeshes(List<Mesh> meshes)
        {
            var meshVertices = new List<Vector3>(meshes.Count * 100);
            for (int meshIndex = 0; meshIndex < meshes.Count; ++meshIndex)
            {
                Mesh mesh = meshes[meshIndex];
                meshVertices.AddRange(mesh.vertices);
            }
            WeldVertexPositions(meshVertices);

            int vertexOffset = 0;
            for (int meshIndex = 0; meshIndex < meshes.Count; ++meshIndex)
            {
                Mesh mesh = meshes[meshIndex];
                mesh.vertices = (meshVertices.GetRange(vertexOffset, mesh.vertexCount)).ToArray();

                vertexOffset += mesh.vertexCount;
            }
        }

        private void WeldVertexPositions(List<Vector3> vertexPositions)
        {
            VertexWeldOctree vertexWeldOctree = new VertexWeldOctree(10.0f);
            vertexWeldOctree.Build(vertexPositions);

            vertexPositions.Clear();
            vertexPositions.AddRange(vertexWeldOctree.WeldVertices(_combineSettings.VertexPositionWeldEpsilon));
        }

        private void PreserveColliders(List<MeshCombineMaterial> meshCombineMaterials, GameObject combinedMeshesParent)
        {
            if(meshCombineMaterials.Count == 0 || _combineSettings.CollHandling != MeshCombineSettings.ColliderHandling.Preserve) return;

            GameObject colliderParent = combinedMeshesParent;
            Transform colliderParentTransform = colliderParent.transform;
            if(_combineSettings.CollPreserveParent == MeshCombineSettings.ColliderPreserveParent.OneForAll)
            {
                colliderParent = new GameObject(_combineSettings.OneForAllColliderParentName);
                UndoEx.RegisterCreatedGameObject(colliderParent);
                colliderParentTransform = colliderParent.transform;
                colliderParentTransform.parent = combinedMeshesParent.transform;
            }

            HashSet<GameObject> processedObjects = new HashSet<GameObject>();
            Vector3 positionSum = Vector3.zero;
            for (int cmbMaterialIndex = 0; cmbMaterialIndex < meshCombineMaterials.Count; ++cmbMaterialIndex )
            {
                MeshCombineMaterial meshCmbMaterial = meshCombineMaterials[cmbMaterialIndex];
                EditorUtility.DisplayProgressBar("Collider Handling", "Preserving colliders...", (float)cmbMaterialIndex / meshCombineMaterials.Count);

                List<MeshInstance> meshInstances = meshCmbMaterial.MeshInstances;
                foreach (var meshInstance in meshInstances)
                {
                    GameObject meshObject = meshInstance.MeshTransform.gameObject;

                    if (processedObjects.Contains(meshObject)) continue;
                    processedObjects.Add(meshObject);

                    positionSum += meshObject.transform.position;

                    BoxCollider[] boxColliders = meshObject.GetComponents<BoxCollider>();
                    foreach (var boxCollider in boxColliders)
                    {
                        string colliderName = _combineSettings.CollPreserveName == MeshCombineSettings.ColliderPreserveName.SameAsSource ? meshObject.name : GetDefaultBoxColliderObjectName();
                        GameObject colliderObject = boxCollider.CloneAsNewObject(colliderName).gameObject;
                        UndoEx.RegisterCreatedGameObject(colliderObject);
                        colliderObject.transform.parent = colliderParentTransform;
                    }

                    SphereCollider[] sphereColliders = meshObject.GetComponents<SphereCollider>();
                    foreach (var sphereCollider in sphereColliders)
                    {
                        string colliderName = _combineSettings.CollPreserveName == MeshCombineSettings.ColliderPreserveName.SameAsSource ? meshObject.name : GetDefaultSphereColliderObjectName();
                        GameObject colliderObject = sphereCollider.CloneAsNewObject(colliderName).gameObject;
                        UndoEx.RegisterCreatedGameObject(colliderObject);
                        colliderObject.transform.parent = colliderParentTransform;
                    }

                    CapsuleCollider[] capsuleColliders = meshObject.GetComponents<CapsuleCollider>();
                    foreach (var capsuleCollider in capsuleColliders)
                    {
                        string colliderName = _combineSettings.CollPreserveName == MeshCombineSettings.ColliderPreserveName.SameAsSource ? meshObject.name : GetDefaultCapsuleColliderObjectName();
                        GameObject colliderObject = capsuleCollider.CloneAsNewObject(colliderName).gameObject;
                        UndoEx.RegisterCreatedGameObject(colliderObject);
                        colliderObject.transform.parent = colliderParentTransform;
                    }

                    MeshCollider[] meshColliders = meshObject.GetComponents<MeshCollider>();
                    foreach (var meshCollider in meshColliders)
                    {
                        string colliderName = _combineSettings.CollPreserveName == MeshCombineSettings.ColliderPreserveName.SameAsSource ? meshObject.name : GetDefaultMeshColliderObjectName();
                        GameObject colliderObject = meshCollider.CloneAsNewObject(colliderName).gameObject;
                        UndoEx.RegisterCreatedGameObject(colliderObject);
                        colliderObject.transform.parent = colliderParentTransform;
                    }
                }
            }

            if (_combineSettings.CollPreserveParent == MeshCombineSettings.ColliderPreserveParent.OneForAll)
                colliderParent.SetWorldPosDontAffectChildren(positionSum *= (1.0f / processedObjects.Count));
            if (colliderParent.transform.childCount == 0) GameObject.DestroyImmediate(colliderParent);

            EditorUtility.ClearProgressBar();
        }

        private string GetDefaultBoxColliderObjectName()
        {
            return "boxCollider";
        }

        private string GetDefaultSphereColliderObjectName()
        {
            return "sphereCollider";
        }

        private string GetDefaultCapsuleColliderObjectName()
        {
            return "capsuleCollider";
        }

        private string GetDefaultMeshColliderObjectName()
        {
            return "meshCollider";
        }
    }
}
#endif