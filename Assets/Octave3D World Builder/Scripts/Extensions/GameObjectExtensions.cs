#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class GameObjectExtensions
    {
        #region Extension Methods
        public static bool IsSceneObject(this GameObject gameObject)
        {
            var prefabAssetType = PrefabUtility.GetPrefabAssetType(gameObject);
            return prefabAssetType == PrefabAssetType.NotAPrefab;
        }

        public static Octave3DMesh GetOctave3DMesh(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromFilterOrSkinnedMeshRenderer();
            if (mesh == null) return null;

            return Octave3DMeshDatabase.Get().GetOctave3DMesh(mesh);
        }

        public static void SetMeshPivotPoint(this GameObject gameObject, Vector3 pivotPoint)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh == null) return;

            Vector3[] vertexPositions = mesh.vertices;
            for(int vIndex = 0; vIndex < vertexPositions.Length; ++vIndex)
            {
                vertexPositions[vIndex] = vertexPositions[vIndex] - pivotPoint;
            }
            mesh.vertices = vertexPositions;
            mesh.RecalculateBounds();

            gameObject.transform.position = pivotPoint;
        }

        public static void SetWorldPosDontAffectChildren(this GameObject gameObject, Vector3 worldPos)
        {
            Transform objectTransform = gameObject.transform;
            Dictionary<Transform, Vector3> childToPos = new Dictionary<Transform, Vector3>();
            for (int childIndex = 0; childIndex < objectTransform.childCount; ++childIndex )
            {
                Transform childTransform = objectTransform.GetChild(childIndex);
                childToPos.Add(childTransform, childTransform.position);
            }

            objectTransform.position = worldPos;
            for (int childIndex = 0; childIndex < objectTransform.childCount; ++childIndex)
            {
                Transform childTransform = objectTransform.GetChild(childIndex);
                childTransform.position = childToPos[childTransform];
            }
        }

        public static List<GameObject> GetAllMeshObjectsInHierarchy(this GameObject root)
        {
            var meshFilters = root.GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length == 0) return new List<GameObject>();

            var meshObjects = new List<GameObject>(meshFilters.Length);
            foreach(var meshFilter in meshFilters)
            {
                if(meshFilter.sharedMesh != null) meshObjects.Add(meshFilter.gameObject);
            }

            return meshObjects;
        }

        public static List<Vector3> GetOverlappedVertsInHierarchy(this GameObject root, OrientedBox overlapBox)
        {
            List<GameObject> allObjects = root.GetAllChildrenIncludingSelf();
            if (allObjects.Count == 0) return new List<Vector3>();

            var overlappedVerts = new List<Vector3>(allObjects.Count * 50);
            foreach (var gameObject in allObjects)
            {
                Mesh mesh = gameObject.GetMeshFromFilterOrSkinnedMeshRenderer();
                if (mesh != null)
                {
                    Octave3DMesh octave3DMesh = Octave3DMeshDatabase.Get().GetOctave3DMesh(mesh);
                    if (octave3DMesh != null) overlappedVerts.AddRange(octave3DMesh.GetOverlappedWorldVerts(overlapBox, new TransformMatrix(gameObject.transform.localToWorldMatrix)));
                }
            }

            return overlappedVerts;
        }

        public static List<Octave3DMesh> GetAllOctave3DMeshesInHierarchy(this GameObject root)
        {
            List<GameObject> allObjects = root.GetAllChildrenIncludingSelf();
            if (allObjects.Count == 0) return new List<Octave3DMesh>();

            var allOctave3DMeshes = new List<Octave3DMesh>(allObjects.Count);
            foreach(var gameObject in allObjects)
            {
                Mesh mesh = gameObject.GetMeshFromFilterOrSkinnedMeshRenderer();
                if(mesh != null)
                {
                    Octave3DMesh octave3DMesh = Octave3DMeshDatabase.Get().GetOctave3DMesh(mesh);
                    if (octave3DMesh != null) allOctave3DMeshes.Add(octave3DMesh);
                }
            }

            return allOctave3DMeshes;
        }

        public static void EmbedInSurfaceByVertex(this GameObject root, Vector3 embedDirection, GameObject embedSurface)
        {
            if (!root.DoesHierarchyContainMesh()) return;
            OrientedBox worldOOBB = root.GetHierarchyWorldOrientedBox();
            if (worldOOBB.IsValid())
            {
                Vector3 worldOOBBScaledSize = worldOOBB.ScaledSize;

                embedDirection.Normalize();
                BoxFace boxFace = worldOOBB.GetBoxFaceMostAlignedWithNormal(embedDirection);
                Vector3 faceCenter = worldOOBB.GetBoxFaceCenter(boxFace);
                Plane facePlane = worldOOBB.GetBoxFacePlane(boxFace);

                int sizeComponentIndex = -1;
                if (boxFace == BoxFace.Back || boxFace == BoxFace.Front) sizeComponentIndex = 2;
                else if (boxFace == BoxFace.Left || boxFace == BoxFace.Right) sizeComponentIndex = 0;
                else if (boxFace == BoxFace.Top || boxFace == BoxFace.Bottom) sizeComponentIndex = 1;

                const float vCollectSizeScale = 0.01f;
                const float vCollectEps = 1e-2f;
                Vector3 vCollectBoxSize = worldOOBB.ScaledSize;
                vCollectBoxSize[sizeComponentIndex] *= vCollectSizeScale;
                vCollectBoxSize[sizeComponentIndex] += vCollectEps;
                vCollectBoxSize[(sizeComponentIndex + 1) % 3] += vCollectEps;
                vCollectBoxSize[(sizeComponentIndex + 2) % 3] += vCollectEps;

                OrientedBox vCollectBox = new OrientedBox(worldOOBB);
                vCollectBox.Scale = Vector3.one;
                vCollectBox.ModelSpaceSize = vCollectBoxSize;
                vCollectBox.Center = (faceCenter + facePlane.normal * vCollectEps) - facePlane.normal * vCollectBoxSize[sizeComponentIndex] * 0.5f;

                List<Vector3> overlappedVerts = root.GetOverlappedVertsInHierarchy(vCollectBox);
                if (overlappedVerts.Count == 0) return;

                GameObjectRayHit surfaceRayHit;
                float maxDistSq = float.MinValue;
                bool needToMove = false;
                Vector3 cameraPos = SceneViewCamera.Camera.transform.position;
                foreach (var vertex in overlappedVerts)
                {
                    // Ignore if already below the surface
                    Ray ray = new Ray(vertex, -embedDirection);
                    if (embedSurface.RaycastTerrainOrMesh(ray, out surfaceRayHit)) continue;

                    // Ignore if already on the surface
                    Vector3 fromCamToPt = vertex - cameraPos;
                    ray = new Ray(cameraPos, Vector3.Normalize(fromCamToPt));
                    if (embedSurface.RaycastTerrainOrMesh(ray, out surfaceRayHit))
                    {
                        if (Mathf.Abs(fromCamToPt.magnitude - (surfaceRayHit.HitPoint - cameraPos).magnitude) < 1e-3f) continue;
                    }

                    ray = new Ray(vertex, embedDirection);
                    if (embedSurface.RaycastTerrainOrMesh(ray, out surfaceRayHit))
                    {
                        float distSq = (vertex - surfaceRayHit.HitPoint).sqrMagnitude;
                        if (distSq > maxDistSq)
                        {
                            maxDistSq = distSq;
                            needToMove = true;
                        }
                    }
                }

                if (needToMove) root.transform.position += embedDirection * Mathf.Sqrt(maxDistSq);
            }
        }

        public static void EmbedAllObjectsInSurface(List<GameObject> gameObjects, Vector3 embedDirection, GameObject embedSurface)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject == embedSurface) continue;
                gameObject.EmbedInSurface(embedDirection, embedSurface);
            }
        }

        public static void EmbedObjectBoxInSurface(OrientedBox objectBox, Vector3 embedDirection, GameObject embedSurface)
        {
            GameObjectRayHit rayHit;
            if (objectBox.IsValid())
            {
                BoxFace boxFace = objectBox.GetBoxFaceMostAlignedWithNormal(embedDirection);
                //Vector3 faceCenter = objectBox.GetBoxFaceCenter(boxFace);

                /*Ray ray = new Ray(faceCenter, embedDirection);
                if (embedSurface.RaycastTerrainOrMeshReverseIfFail(ray, out rayHit))
                {
                    Vector3 moveVector = rayHit.HitPoint - faceCenter;
                    objectBox.Center += moveVector;
                }*/

                float maxDistSq = float.MinValue;
                float minDistSq = float.MaxValue;
                bool hitAlongPrjDir = false;
                bool hitAlongReversePrjDir = false;
                List<Vector3> boxFaceCorners = objectBox.GetBoxFaceCornerPoints(boxFace);
                foreach (var corner in boxFaceCorners)
                {
                    Ray ray = new Ray(corner, -embedDirection);
                    if (embedSurface.RaycastTerrainOrMesh(ray, out rayHit))
                    {
                        float distSq = (corner - rayHit.HitPoint).sqrMagnitude;
                        if (distSq < minDistSq)
                        {
                            minDistSq = distSq;
                            hitAlongReversePrjDir = true;
                        }

                        continue;
                    }

                    ray = new Ray(corner, embedDirection);
                    if (embedSurface.RaycastTerrainOrMesh(ray, out rayHit))
                    {
                        float distSq = (corner - rayHit.HitPoint).sqrMagnitude;
                        if (distSq > maxDistSq)
                        {
                            maxDistSq = distSq;
                            hitAlongPrjDir = true;
                        }
                    }
                }

                if (hitAlongPrjDir) objectBox.Center += embedDirection * Mathf.Sqrt(maxDistSq);   
                else
                if (hitAlongReversePrjDir) objectBox.Center -= embedDirection * Mathf.Sqrt(minDistSq);   
            }
        }

        public static void EmbedInSurface(this GameObject gameObject, Vector3 embedDirection, GameObject embedSurface)
        {
            GameObjectRayHit rayHit;
            OrientedBox worldOOBB = gameObject.GetHierarchyWorldOrientedBox();
            if (worldOOBB.IsValid())
            {
                BoxFace boxFace = worldOOBB.GetBoxFaceMostAlignedWithNormal(embedDirection);
                //Vector3 faceCenter = worldOOBB.GetBoxFaceCenter(boxFace);

                /*Ray ray = new Ray(faceCenter, embedDirection);
                if (embedSurface.RaycastTerrainOrMeshReverseIfFail(ray, out rayHit))
                {
                    Vector3 moveVector = rayHit.HitPoint - faceCenter;
                    GameObjectExtensions.RecordObjectTransformsForUndo(new List<GameObject> { gameObject });
                    gameObject.transform.position += moveVector;
                    worldOOBB.Center += moveVector;
                }*/

                float maxDistSq = float.MinValue;
                float minDistSq = float.MaxValue;
                bool hitAlongPrjDir = false;
                bool hitAlongReversePrjDir = false;
                List<Vector3> boxFaceCorners = worldOOBB.GetBoxFaceCornerPoints(boxFace);
                foreach (var corner in boxFaceCorners)
                {
                    Ray ray = new Ray(corner, -embedDirection);
                    if (embedSurface.RaycastTerrainOrMesh(ray, out rayHit))
                    {
                        float distSq = (corner - rayHit.HitPoint).sqrMagnitude;
                        if (distSq < minDistSq)
                        {
                            minDistSq = distSq;
                            hitAlongReversePrjDir = true;
                        }

                        continue;
                    }

                    ray = new Ray(corner, embedDirection);
                    if (embedSurface.RaycastTerrainOrMesh(ray, out rayHit))
                    {
                        float distSq = (corner - rayHit.HitPoint).sqrMagnitude;
                        if (distSq > maxDistSq)
                        {
                            maxDistSq = distSq;
                            hitAlongPrjDir = true;
                        }
                    }
                }

                if (hitAlongPrjDir)
                {
                    GameObjectExtensions.RecordObjectTransformsForUndo(new List<GameObject> { gameObject });
                    gameObject.transform.position += embedDirection * Mathf.Sqrt(maxDistSq);
                }
                else
                if (hitAlongReversePrjDir)
                {
                    GameObjectExtensions.RecordObjectTransformsForUndo(new List<GameObject> { gameObject });
                    gameObject.transform.position -= embedDirection * Mathf.Sqrt(minDistSq);
                }
            }
        }

        public static void ProjectAllObjectsOnPlane(List<GameObject> gameObjects, Vector3 projectionDir, Plane plane)
        {
            List<GameObject> parents = GameObjectExtensions.GetParents(gameObjects);
            foreach (GameObject gameObject in parents)
            {
                gameObject.ProjectOnPlane(projectionDir, plane);
            }
        }

        public static void ProjectOnPlane(this GameObject gameObject, Vector3 projectionDir, Plane plane)
        {
            OrientedBox worldOOBB = gameObject.GetHierarchyWorldOrientedBox();
            if (worldOOBB.IsValid())
            {
                BoxFace boxFace = worldOOBB.GetBoxFaceMostAlignedWithNormal(projectionDir);
                //Vector3 faceCenter = worldOOBB.GetBoxFaceCenter(boxFace);

                /*Ray ray = new Ray(faceCenter, embedDirection);
                if (embedSurface.RaycastTerrainOrMeshReverseIfFail(ray, out rayHit))
                {
                    Vector3 moveVector = rayHit.HitPoint - faceCenter;
                    GameObjectExtensions.RecordObjectTransformsForUndo(new List<GameObject> { gameObject });
                    gameObject.transform.position += moveVector;
                    worldOOBB.Center += moveVector;
                }*/

                float maxDistSq = float.MinValue;
                float minDistSq = float.MaxValue;
                bool hitAlongPrjDir = false;
                bool hitAlongReversePrjDir = false;
                List<Vector3> boxFaceCorners = worldOOBB.GetBoxFaceCornerPoints(boxFace);
                foreach (var corner in boxFaceCorners)
                {
                    Ray ray = new Ray(corner, -projectionDir);
                    float t;
                    if (plane.Raycast(ray, out t))
                    {
                        float distSq = (corner - ray.GetPoint(t)).sqrMagnitude;
                        if (distSq < minDistSq)
                        {
                            minDistSq = distSq;
                            hitAlongReversePrjDir = true;
                        }

                        continue;
                    }

                    ray = new Ray(corner, projectionDir);
                    if (plane.Raycast(ray, out t))
                    {
                        float distSq = (corner - ray.GetPoint(t)).sqrMagnitude;
                        if (distSq > maxDistSq)
                        {
                            maxDistSq = distSq;
                            hitAlongPrjDir = true;
                        }
                    }
                }

                if (hitAlongPrjDir)
                {
                    GameObjectExtensions.RecordObjectTransformsForUndo(new List<GameObject> { gameObject });
                    gameObject.transform.position += projectionDir * Mathf.Sqrt(maxDistSq);
                }
                else
                    if (hitAlongReversePrjDir)
                    {
                        GameObjectExtensions.RecordObjectTransformsForUndo(new List<GameObject> { gameObject });
                        gameObject.transform.position -= projectionDir * Mathf.Sqrt(minDistSq);
                    }
            }
        }

        public static List<Vector3> GetLocalAxes(this GameObject gameObject)
        {
            Transform gameObjectTransform = gameObject.transform;
            return new List<Vector3> { gameObjectTransform.right, gameObjectTransform.up, gameObjectTransform.forward };
        }

        public static void ApplyTransformDataToRootChildren(this GameObject root, GameObject sourceRoot)
        {
            List<GameObject> destChildren = root.GetAllChildren();
            List<GameObject> sourceChildren = sourceRoot.GetAllChildren();

            if (destChildren.Count != sourceChildren.Count) return;

            // Note: Assumes there will always be a 1 to 1 mapping between the children in the hierarchy. 
            for(int childIndex = 0; childIndex < destChildren.Count; ++childIndex)
            {
                destChildren[childIndex].transform.InheritWorldTransformFrom(sourceChildren[childIndex].transform);
            }
        }

        public static GameObject CloneAsWorkingObject(this GameObject gameObject, Transform parentTransform, bool allowUndoRedo = true)
        {
            Transform gameObjectTransform = gameObject.transform;
            GameObject clone = GameObject.Instantiate(gameObject, gameObjectTransform.position, gameObjectTransform.rotation) as GameObject;
            if (allowUndoRedo) UndoEx.RegisterCreatedGameObject(clone);

            clone.transform.localScale = gameObjectTransform.transform.lossyScale;
            clone.name = gameObject.name;
            clone.isStatic = gameObject.isStatic;
            clone.layer = gameObject.layer;
            clone.transform.parent = parentTransform;

            Octave3DScene.Get().RegisterObjectHierarchy(clone);
            SceneViewCamera.Instance.SetObjectVisibilityDirty();

            return clone;
        }

        public static GameObject GetSourcePrefab(this GameObject gameObject)
        {
            return PrefabUtility.GetCorrespondingObjectFromSource(gameObject) as GameObject;
        }

        public static GameObject GetSourcePrefabRoot(this GameObject gameObject)
        {
            GameObject sourcePrefab = gameObject.GetSourcePrefab();
            if (sourcePrefab == null) return null;

            Transform sourcePrefabTransform = sourcePrefab.transform;
            if (sourcePrefabTransform.root != null) sourcePrefab = sourcePrefabTransform.root.gameObject;

            return sourcePrefab;
        }

        public static bool RaycastBox(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;
            OrientedBox objectWorldOrientedBox = gameObject.GetWorldOrientedBox();
        
            OrientedBoxRayHit objectBoxRayHit;
            if (objectWorldOrientedBox.Raycast(ray, out objectBoxRayHit))
                objectRayHit = new GameObjectRayHit(ray, gameObject, objectBoxRayHit, null, null, null);

            return objectRayHit != null;
        }

        public static bool RaycastSprite(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return false;

            OrientedBox objectWorldOrientedBox = gameObject.GetWorldOrientedBox();

            OrientedBoxRayHit objectBoxRayHit;
            if(objectWorldOrientedBox.Raycast(ray, out objectBoxRayHit))
            {
                SpriteRayHit spriteHit = new SpriteRayHit(ray, objectBoxRayHit.HitEnter, spriteRenderer, objectBoxRayHit.HitPoint, objectBoxRayHit.HitNormal);
                objectRayHit = new GameObjectRayHit(ray, gameObject, null, null, null, spriteHit);
            }

            return objectRayHit != null;
        }

        public static bool RaycastMesh(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;
            Mesh objectMesh = gameObject.GetMeshFromFilterOrSkinnedMeshRenderer();
            if (objectMesh == null) return false;

            Octave3DMesh octaveMesh = Octave3DMeshDatabase.Get().GetOctave3DMesh(objectMesh);
            if (octaveMesh == null) return false;

            MeshRayHit meshRayHit = octaveMesh.Raycast(ray, gameObject.transform.GetWorldMatrix());
            if (meshRayHit == null) return false;

            objectRayHit = new GameObjectRayHit(ray, gameObject, null, meshRayHit, null, null);
            return true;
        }

        public static bool RaycastMeshReverseIfFail(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            if (gameObject.RaycastMesh(ray, out objectRayHit)) return true;
            return gameObject.RaycastMesh(new Ray(ray.origin, -ray.direction), out objectRayHit);
        }

        public static bool RaycastTerrain(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;
            if (!gameObject.HasTerrain()) return false;

            TerrainCollider terrainCollider = gameObject.GetComponent<TerrainCollider>();
            if (terrainCollider == null) return false;

            RaycastHit raycastHit;
            if (terrainCollider.Raycast(ray, out raycastHit, float.MaxValue))
            {
                TerrainRayHit terrainRayHit = new TerrainRayHit(ray, raycastHit);
                objectRayHit = new GameObjectRayHit(ray, gameObject, null, null, terrainRayHit, null);
            }

            return objectRayHit != null;
        }

        public static bool RaycastTerrainReverseIfFail(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            if (gameObject.RaycastTerrain(ray, out objectRayHit)) return true;
            return gameObject.RaycastTerrain(new Ray(ray.origin, -ray.direction), out objectRayHit);
        }

        public static bool RaycastTerrainOrMeshReverseIfFail(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;
            if(gameObject.HasTerrain() && gameObject.GetComponent<TerrainCollider>() != null) return gameObject.RaycastTerrainReverseIfFail(ray, out objectRayHit);
            else if (gameObject.HasMesh()) return gameObject.RaycastMeshReverseIfFail(ray, out objectRayHit);

            return false;
        }

        public static bool RaycastTerrainOrMesh(this GameObject gameObject, Ray ray, out GameObjectRayHit objectRayHit)
        {
            objectRayHit = null;
            if (gameObject.HasTerrain() && gameObject.GetComponent<TerrainCollider>() != null) return gameObject.RaycastTerrain(ray, out objectRayHit);
            else if (gameObject.HasMesh()) return gameObject.RaycastMesh(ray, out objectRayHit);

            return false;
        }

        public static void SetHierarchyStatic(this GameObject hierarchyRoot, bool isStatic)
        {
            List<GameObject> allChildren = hierarchyRoot.GetAllChildrenIncludingSelf();
            foreach(GameObject child in allChildren)
            {
                child.isStatic = isStatic;
            }
        }

        public static void RotateHierarchyBoxAroundPoint(this GameObject root, float rotationInDegrees, Vector3 rotationAxis, Vector3 pivotPoint)
        {
            // OLD CODE: Was not taking pivot point into account.
            /*OrientedBox hierarchyWorldOrientedBox = root.GetHierarchyWorldOrientedBox();
            Transform rootTransform = root.transform;

            Vector3 fromCenterToPosition = rootTransform.position - hierarchyWorldOrientedBox.Center;
            Quaternion oldRotation = rootTransform.rotation;

            rotationAxis.Normalize();
            rootTransform.Rotate(rotationAxis, rotationInDegrees, Space.World);

            fromCenterToPosition = oldRotation.GetRelativeRotation(rootTransform.rotation) * fromCenterToPosition;
            rootTransform.position = hierarchyWorldOrientedBox.Center + fromCenterToPosition;*/

            OrientedBox hierarchyWorldOrientedBox = root.GetHierarchyWorldOrientedBox();
            Transform rootTransform = root.transform;

            rotationAxis.Normalize();
            Quaternion rotation = Quaternion.AngleAxis(rotationInDegrees, rotationAxis);

            Vector3 fromPivotToCenter = rotation * (hierarchyWorldOrientedBox.Center - pivotPoint);
            Vector3 newCenter = pivotPoint + fromPivotToCenter;
            Vector3 fromCenterToPosition = rotation * (rootTransform.position - hierarchyWorldOrientedBox.Center);

            rootTransform.rotation = rotation * rootTransform.rotation;
            rootTransform.position = newCenter + fromCenterToPosition;
        }

        public static void SetHierarchyWorldRotationAndPreserveHierarchyCenter(this GameObject root, Quaternion rotation)
        {
            OrientedBox hierarchyWorldOrientedBox = root.GetHierarchyWorldOrientedBox();
            Transform rootTransform = root.transform;

            Vector3 fromCenterToPosition = rootTransform.position - hierarchyWorldOrientedBox.Center;
            Quaternion oldRotation = rootTransform.rotation;
            rootTransform.rotation = rotation;

            fromCenterToPosition = oldRotation.GetRelativeRotation(rootTransform.rotation) * fromCenterToPosition;
            rootTransform.position = hierarchyWorldOrientedBox.Center + fromCenterToPosition;
        }

        public static void SetHierarchyWorldScaleByPivotPoint(this GameObject root, float scale, Vector3 pivotPoint)
        {
            root.SetHierarchyWorldScaleByPivotPoint(new Vector3(scale, scale, scale), pivotPoint);
        }

        public static void SetHierarchyWorldScaleByPivotPoint(this GameObject root, Vector3 scale, Vector3 pivotPoint)
        {
            Transform rootTransform = root.transform;
            Vector3 fromPivotToPosition = rootTransform.position - pivotPoint;
            Vector3 oldScale = rootTransform.lossyScale;
            root.SetWorldScale(scale);

            Vector3 invOldScaleVector = new Vector3(1.0f / oldScale.x, 1.0f / oldScale.y, 1.0f / oldScale.z);
            Vector3 relativeScale = Vector3.Scale(scale, invOldScaleVector);
            fromPivotToPosition = Vector3.Scale(relativeScale, fromPivotToPosition);
            rootTransform.position = pivotPoint + fromPivotToPosition;
        }

        public static void PlaceHierarchyOnPlane(this GameObject root, Plane placementPlane)
        {
            OrientedBox hierarchyWorldOrientedBox = root.GetHierarchyWorldOrientedBox();
            List<Vector3> boxPts = hierarchyWorldOrientedBox.GetCornerPoints();

            int ptIndex = placementPlane.GetIndexOfFurthestPointBehind(boxPts);
            if (ptIndex < 0) ptIndex = placementPlane.GetIndexOfClosestPointInFrontOrOnPlane(boxPts);

            if(ptIndex >= 0)
            {
                float distToPt = placementPlane.GetDistanceToPoint(boxPts[ptIndex]);
                root.transform.position -= placementPlane.normal * distToPt;
            }
        }

        public static void PlaceObjectBoxOnPlane(OrientedBox oobb, Plane plane)
        {
            List<Vector3> boxPts = oobb.GetCornerPoints();

            int ptIndex = plane.GetIndexOfFurthestPointBehind(boxPts);
            if (ptIndex < 0) ptIndex = plane.GetIndexOfClosestPointInFrontOrOnPlane(boxPts);

            if (ptIndex >= 0)
            {
                float distToPt = plane.GetDistanceToPoint(boxPts[ptIndex]);
                oobb.Center -= plane.normal * distToPt;
            }
        }

        public static void SetSelectedHierarchyWireframeHidden(this GameObject hierarchyRoot, bool isWireframeHidden)
        {
            Renderer renderer = hierarchyRoot.GetComponent<Renderer>();
            if (renderer != null)
            {
                #if UNITY_2017_1_OR_NEWER
                EditorUtility.SetSelectedRenderState(renderer, isWireframeHidden ? EditorSelectedRenderState.Hidden : EditorSelectedRenderState.Highlight);
                #else
                EditorUtility.SetSelectedWireframeHidden(renderer, isWireframeHidden);
                #endif
            }

            Transform gameObjectTransform = hierarchyRoot.transform;
            for (int childIndex = 0; childIndex < gameObjectTransform.childCount; ++childIndex)
            {
                SetSelectedHierarchyWireframeHidden(gameObjectTransform.GetChild(childIndex).gameObject, isWireframeHidden);
            }
        }

        public static bool DoesHierarchyContainMesh(this GameObject root)
        {
            List<GameObject> allChildrenIncludingSelf = root.GetAllChildrenIncludingSelf();
            foreach(GameObject gameObject in allChildrenIncludingSelf)
            {
                if (gameObject.HasMesh()) return true;
            }

            return false;
        }

        public static List<GameObject> GetHierarchyObjectsWithMesh(this GameObject hierarchyRoot)
        {
            List<GameObject> allChildrenIncludingSelf = hierarchyRoot.GetAllChildrenIncludingSelf();
            allChildrenIncludingSelf.RemoveAll(item => !item.HasMesh());

            return allChildrenIncludingSelf;
        }

        public static List<GameObject> GetHierarchyObjectsWithSprites(this GameObject hierarchyRoot)
        {
            List<GameObject> allChildrenIncludingSelf = hierarchyRoot.GetAllChildrenIncludingSelf();
            allChildrenIncludingSelf.RemoveAll(item => !item.HasSpriteRendererWithSprite());

            return allChildrenIncludingSelf;
        }

        public static bool HasMesh(this GameObject gameObject)
        {
            return gameObject.HasMeshFilterWithValidMesh() || gameObject.HasSkinnedMeshRendererWithValidMesh();
        }

        public static bool HasMeshFilterWithValidMesh(this GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            return meshFilter != null && meshFilter.sharedMesh != null;
        }

        public static bool HasSkinnedMeshRendererWithValidMesh(this GameObject gameObject)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            return skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null;
        }

        public static bool HasTerrain(this GameObject gameObject)
        {
            return gameObject.GetComponent<Terrain>() != null;
        }

        public static bool HasLight(this GameObject gameObject)
        {
            return gameObject.GetComponent<Light>() != null;
        }

        public static bool HasParticleSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<ParticleSystem>() != null;
        }

        public static bool HasSpriteRenderer(this GameObject gameObject)
        {
            return gameObject.GetComponent<SpriteRenderer>() != null;
        }

        public static bool HasSpriteRendererWithSprite(this GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return false;

            return spriteRenderer.sprite != null;
        }

        public static void AttachChildren(this GameObject gameObject, List<GameObject> children, bool allowUndoRedo)
        {
            if(allowUndoRedo)
            {
                Transform objectTransform = gameObject.transform;
                UndoEx.RecordForToolAction(objectTransform);

                foreach (GameObject child in children)
                {
                    Transform childTransform = child.transform;
                    UndoEx.RecordForToolAction(childTransform);
                    childTransform.parent = objectTransform;
                }
            }
            else
            {
                Transform objectTransform = gameObject.transform;
                foreach (GameObject child in children)
                {
                    child.transform.parent = objectTransform;
                }
            }
        }

        public static List<GameObject> GetAllChildrenIncludingSelf(this GameObject gameObject)
        {
            var finalObjectList = new List<GameObject> { gameObject };

            List<GameObject> allChildren = gameObject.GetAllChildren();
            if (allChildren.Count != 0) finalObjectList.AddRange(allChildren);

            return finalObjectList;
        }

        public static List<GameObject> GetAllChildren(this GameObject gameObject)
        {
            Transform objectTransform = gameObject.transform;
            Transform[] allChildTransforms = gameObject.GetComponentsInChildren<Transform>(true);

            var allChildren = new List<GameObject>();
            foreach(Transform childTransform in allChildTransforms)
            {
                if (objectTransform != childTransform) allChildren.Add(childTransform.gameObject);
            }

            return allChildren;
        }

        public static List<GameObject> GetImmediateChildren(this GameObject gameObject)
        {
            Transform objectTransform = gameObject.transform;
            List<Transform> immediateChildTransforms = objectTransform.GetImmediateChildTransforms();

            if (immediateChildTransforms.Count != 0)
            {
                List<GameObject> immediateChildren = new List<GameObject>(immediateChildTransforms.Count);
                foreach (Transform childTransform in immediateChildTransforms)
                {
                    immediateChildren.Add(childTransform.gameObject);
                }

                return immediateChildren;
            }
            else return new List<GameObject>();
        }

        public static void MoveImmediateChildrenUpOneLevel(this GameObject gameObject, bool allowUndoRedo)
        {
            Transform objectTransform = gameObject.transform;
            Transform objectParentTransform = objectTransform.parent;

            List<Transform> immediateChildTransforms = objectTransform.GetImmediateChildTransforms();

            if(allowUndoRedo)
            {
                foreach (Transform childTransform in immediateChildTransforms)
                {
                    UndoEx.SetTransformParent(childTransform, objectParentTransform);
                }
            }
            else
            {
                foreach (Transform childTransform in immediateChildTransforms)
                {
                    childTransform.parent = objectParentTransform;
                }
            }
        }

        public static GameObject GetParentWhichIsChildOf(this GameObject gameObject, GameObject targetParent)
        {
            // Store needed data for easy access
            Transform targetParentTransform = targetParent.transform;
            Transform currentTransform = gameObject.transform;

            // Keep moving up the hierarchy until we encounter the parent whose parent is 'targetParent'
            while (currentTransform != null && currentTransform.parent != targetParentTransform)
            {
                // Move up
                currentTransform = currentTransform.parent;
            }

            // If the current transform is not null, it means we found the parent which is a child of 'targetParent'.
            // Otherwise, it means that either 'gameObject' doesn't have any parents or its top parent is not a child
            // of 'targetParent'.
            return currentTransform != null ? currentTransform.gameObject : null;
        }

        public static GameObject GetParentWhichIsChildOf(this GameObject gameObject, List<Type> possibleParentTypes)
        {
            Transform currentTransform = gameObject.transform;

            while (currentTransform != null && currentTransform.parent != null)
            {
                GameObject currentParentObject = currentTransform.parent.gameObject;
                if (currentParentObject.HasComponentsOfAnyType(possibleParentTypes)) return currentTransform.gameObject;

                currentTransform = currentTransform.parent;
            }

            return null;
        }

        public static bool HasComponentsOfAnyType(this GameObject gameObject, List<Type> possibleTypes)
        {
            foreach (var type in possibleTypes)
            {
                if (gameObject.GetComponents(type).Length != 0) return true;
            }

            return false;
        }

        public static void SetWorldScale(this GameObject gameObject, float worldScale)
        {
            gameObject.SetWorldScale(new Vector3(worldScale, worldScale, worldScale));
        }

        public static void SetWorldScale(this GameObject gameObject, Vector3 worldScale)
        {
            Transform objectTransform = gameObject.transform;
            Transform objectParent = objectTransform.parent;

            objectTransform.parent = null;
            objectTransform.localScale = worldScale;

            float minScale = 1e-4f;
            if (Mathf.Abs(objectTransform.localScale.x) < minScale) objectTransform.localScale = new Vector3(minScale, objectTransform.localScale.y, objectTransform.localScale.z);
            if (Mathf.Abs(objectTransform.localScale.y) < minScale) objectTransform.localScale = new Vector3(objectTransform.localScale.x, minScale, objectTransform.localScale.z);
            if (Mathf.Abs(objectTransform.localScale.z) < minScale) objectTransform.localScale = new Vector3(objectTransform.localScale.x, objectTransform.localScale.y, minScale);

            objectTransform.parent = objectParent;
        }

        public static Rect GetScreenRectangle(this GameObject gameObject, Camera camera)
        {
            Box worldBox = gameObject.GetWorldBox();
            if (worldBox.IsValid()) return worldBox.GetScreenRectangle(camera);

            return new Rect();
        }

        public static OrientedBox GetHierarchyWorldOrientedBox(this GameObject hierarchyRoot)
        {
            OrientedBox hierarchyWorldOrientedBox = hierarchyRoot.GetHierarchyModelSpaceOrientedBox();           
            hierarchyWorldOrientedBox.Transform(hierarchyRoot.transform);

            return hierarchyWorldOrientedBox;
        }

        public static OrientedBox GetHierarchyModelSpaceOrientedBox(this GameObject hierarchyRoot)
        {
            Box hierarchyModelSpaceBox = hierarchyRoot.GetHierarchyModelSpaceBox();
            return new OrientedBox(hierarchyModelSpaceBox);
        }

        public static Box GetHierarchyWorldBox(this GameObject hierarchyRoot)
        {
            Box hierarchyWorldBox = hierarchyRoot.GetHierarchyModelSpaceBox();
            hierarchyWorldBox = hierarchyWorldBox.Transform(hierarchyRoot.transform.GetWorldMatrix());
            return hierarchyWorldBox;
        }

        public static Box GetHierarchyModelSpaceBox(this GameObject hierarchyRoot)
        {
            Transform rootTransform = hierarchyRoot.transform;
            Transform[] allChildTransforms = hierarchyRoot.GetComponentsInChildren<Transform>();

            bool hierarchyContainsMesh = hierarchyRoot.DoesHierarchyContainMesh();
            Box hierarchyModelSpaceBox = hierarchyRoot.GetModelSpaceBox();
            foreach (Transform childTransform in allChildTransforms)
            {
                GameObject childObject = childTransform.gameObject;
                bool objectHasMesh = childObject.HasMesh();

                if(objectHasMesh)
                {
                    Renderer renderer = childObject.GetRenderer();
                    if (renderer == null || !renderer.enabled) continue;
                }

                // If the hierarchy contains mesh objects, we will only take mesh objects into account
                if (!objectHasMesh && hierarchyContainsMesh) continue;
                if (childObject != hierarchyRoot)
                {
                    /*Octave3DMesh octave3DMesh = Octave3DMeshDatabase.Get().GetOctave3DMesh(childObject.GetMeshFromFilterOrSkinnedMeshRenderer());
                    if(octave3DMesh != null)
                    {
                        TransformMatrix rootRelativeTransformMatrix = childTransform.GetRelativeTransformMatrix(rootTransform);
                        rootRelativeTransformMatrix.Scale = rootRelativeTransformMatrix.Scale.GetVectorWithPositiveComponents();

                        List<Vector3> vertexPositions = new List<Vector3>(octave3DMesh.VertexPositions);
                        vertexPositions = Vector3Extensions.GetTransformedPoints(vertexPositions, rootRelativeTransformMatrix.ToMatrix4x4x);

                        Box vertexBox = Vector3Extensions.GetPointCloudBox(vertexPositions);
                        if (hierarchyModelSpaceBox.IsValid()) hierarchyModelSpaceBox.Encapsulate(vertexBox);
                        else hierarchyModelSpaceBox = new Box(vertexBox);
                    }
                    else*/
                    {
                        Box childModelSpaceBox = childObject.GetModelSpaceBox();
                        if (childModelSpaceBox.IsValid())
                        {
                            // Note: Negative scale values are a pain to work with, so we will set the scale to a positive value.
                            //       Negative scale causes problems inside 'Box.Transform' because it modifies the translation
                            //       in an undesirable manner. However, is it a good idea to ignore negative scale ???!!!
                            TransformMatrix rootRelativeTransformMatrix = childTransform.GetRelativeTransformMatrix(rootTransform);
                            rootRelativeTransformMatrix.Scale = rootRelativeTransformMatrix.Scale.GetVectorWithPositiveComponents();

                            childModelSpaceBox = childModelSpaceBox.Transform(rootRelativeTransformMatrix);

                            if (hierarchyModelSpaceBox.IsValid()) hierarchyModelSpaceBox.Encapsulate(childModelSpaceBox);
                            else hierarchyModelSpaceBox = new Box(childModelSpaceBox);
                        }
                    }
                }
            }

            return hierarchyModelSpaceBox;
        }

        /*public static OrientedBox GetHierarchyModelSpaceOrientedBox(this GameObject hierarchyRoot)
        {
            Transform rootTransform = hierarchyRoot.transform;
            Transform[] allChildTransforms = hierarchyRoot.GetComponentsInChildren<Transform>();

            bool hierarchyContainsMesh = hierarchyRoot.DoesHierarchyContainMesh();
            OrientedBox hierarchyModelSpaceBox = hierarchyRoot.GetModelSpaceOrientedBox();
            foreach (Transform childTransform in allChildTransforms)
            {
                GameObject childObject = childTransform.gameObject;

                // If the hierarchy contains mesh objects, we will only take mesh objects into account
                if (!childObject.HasMesh() && hierarchyContainsMesh) continue;
                if (childObject != hierarchyRoot)
                {
                    OrientedBox childModelSpaceBox = childObject.GetModelSpaceOrientedBox();
                    if (childModelSpaceBox.IsValid())
                    {
                        // Note: Negative scale values are a pain to work with, so we will set the scale to a positive value.
                        //       Negative scale causes problems inside 'Box.Transform' because it modifies the translation
                        //       in an undesirable manner. However, is it a good idea to ignore negative scale ???!!!
                        TransformMatrix rootRelativeTransformMatrix = childTransform.GetRelativeTransformMatrix(rootTransform);
                        rootRelativeTransformMatrix.Scale = rootRelativeTransformMatrix.Scale.GetVectorWithPositiveComponents();

                        childModelSpaceBox.Transform(rootRelativeTransformMatrix);

                        if (hierarchyModelSpaceBox.IsValid()) hierarchyModelSpaceBox.Encapsulate(childModelSpaceBox);
                        else hierarchyModelSpaceBox = new OrientedBox(childModelSpaceBox);
                    }
                }
            }

            return hierarchyModelSpaceBox;
        }*/

        public static OrientedBox GetWorldOrientedBox(this GameObject gameObject)
        {
            OrientedBox worldOrientedBox = gameObject.GetMeshWorldOrientedBox();
            if (worldOrientedBox.IsValid()) return worldOrientedBox;

            return gameObject.GetNonMeshWorldOrientedBox();
        }

        public static Box GetWorldBox(this GameObject gameObject)
        {
            Box worldBox = gameObject.GetMeshWorldBox();
            if (worldBox.IsValid()) return worldBox;

            return gameObject.GetNonMeshWorldBox();
        }

        public static OrientedBox GetModelSpaceOrientedBox(this GameObject gameObject)
        {
            OrientedBox modelSpaceOrientedBox = gameObject.GetMeshModelSpaceOrientedBox();
            if (modelSpaceOrientedBox.IsValid()) return modelSpaceOrientedBox;

            return gameObject.GetNonMeshModelSpaceOrientedBox();
        }

        public static Box GetModelSpaceBox(this GameObject gameObject)
        {
            Box modelSpaceBox = gameObject.GetMeshModelSpaceBox();
            if (modelSpaceBox.IsValid()) return modelSpaceBox;

            return gameObject.GetNonMeshModelSpaceBox();
        }

        public static OrientedBox GetMeshWorldOrientedBox(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh != null) return new OrientedBox(new Box(mesh.bounds), gameObject.transform);

            mesh = gameObject.GetMeshFromSkinnedMeshRenderer();
            if (mesh != null) return new OrientedBox(new Box(gameObject.GetComponent<SkinnedMeshRenderer>().localBounds), gameObject.transform);

            return OrientedBox.GetInvalid();
        }

        public static Box GetMeshWorldBox(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh != null) return new Box(mesh.bounds).Transform(gameObject.transform.GetWorldMatrix());

            mesh = gameObject.GetMeshFromSkinnedMeshRenderer();
            if (mesh != null) return new Box(gameObject.GetComponent<SkinnedMeshRenderer>().localBounds).Transform(gameObject.transform.GetWorldMatrix());

            return Box.GetInvalid();
        }

        public static OrientedBox GetNonMeshWorldOrientedBox(this GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if(spriteRenderer != null)
            {
                return new OrientedBox(spriteRenderer.GetModelSpaceBox(), gameObject.transform);
            }
            else
            {
                OrientedBox modelSpaceOrientedBox = gameObject.GetNonMeshModelSpaceOrientedBox();
                if (!modelSpaceOrientedBox.IsValid()) return modelSpaceOrientedBox;

                OrientedBox worldOrientedBox = new OrientedBox(modelSpaceOrientedBox);
                Transform objectTransform = gameObject.transform;
                worldOrientedBox.Center = objectTransform.position;
                worldOrientedBox.Rotation = objectTransform.rotation;
                worldOrientedBox.Scale = objectTransform.lossyScale;

                return worldOrientedBox;
            }
        }

        public static Box GetNonMeshWorldBox(this GameObject gameObject)
        {
            Box modelSpaceBox = gameObject.GetNonMeshModelSpaceBox();
            if (!modelSpaceBox.IsValid()) return modelSpaceBox;

            Box worldBox = modelSpaceBox.Transform(gameObject.transform.GetWorldMatrix());
            return worldBox;
        }

        public static OrientedBox GetMeshModelSpaceOrientedBox(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh != null) return new OrientedBox(new Box(mesh.bounds), Quaternion.identity);

            mesh = gameObject.GetMeshFromSkinnedMeshRenderer();
            if (mesh != null) return new OrientedBox(new Box(gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh.bounds), Quaternion.identity);
          
            return OrientedBox.GetInvalid();
        }

        public static Box GetMeshModelSpaceBox(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh != null) return new Box(mesh.bounds);

            mesh = gameObject.GetMeshFromSkinnedMeshRenderer();
            if (mesh != null) return new Box(gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh.bounds);

            return Box.GetInvalid();
        }

        public static OrientedBox GetNonMeshModelSpaceOrientedBox(this GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) return new OrientedBox(spriteRenderer.GetModelSpaceBox());

            if (!gameObject.HasLight() && !gameObject.HasParticleSystem()) return OrientedBox.GetInvalid();
            return new OrientedBox(gameObject.GetNonMeshModelSpaceBox());
        }

        public static Box GetNonMeshModelSpaceBox(this GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) return spriteRenderer.GetModelSpaceBox();

            if (!gameObject.HasLight() && !gameObject.HasParticleSystem()) return Box.GetInvalid();
            return new Box(Vector3.zero, Octave3DScene.VolumeSizeForNonMeshObjects);
        }

        public static Renderer GetRenderer(this GameObject gameObject)
        {
            return gameObject.GetComponent<Renderer>();
        }

        public static bool IsSprite(this GameObject gameObject)
        {
            if (gameObject.HasMesh()) return false;

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            return spriteRenderer != null && spriteRenderer.sprite != null;
        }

        public static Mesh GetMeshFromFilterOrSkinnedMeshRenderer(this GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMeshFromMeshFilter();
            if (mesh == null) mesh = gameObject.GetMeshFromSkinnedMeshRenderer();

            return mesh;
        }

        public static Mesh GetMeshFromMeshFilter(this GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null) return meshFilter.sharedMesh;

            return null;
        }

        public static Mesh GetMeshFromSkinnedMeshRenderer(this GameObject gameObject)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null) return skinnedMeshRenderer.sharedMesh;

            return null;
        }
        #endregion

        #region Utilities
        public static List<GameObject> GetParents(IEnumerable<GameObject> gameObjects)
        {
            List<GameObject> topParents = new List<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                bool foundParentForThisObject = false;
                Transform gameObjectTransform = gameObject.transform;

                foreach (GameObject potentialParent in gameObjects)
                {
                    if (gameObject != potentialParent && 
                        gameObjectTransform.IsChildOf(potentialParent.transform))
                    {
                        foundParentForThisObject = true;
                        break;
                    }
                }

                if (!foundParentForThisObject) topParents.Add(gameObject);
            }

            return topParents;
        }

        public static List<GameObject> GetAllObjectsInHierarchyCollection(List<GameObject> hierarchyRoots)
        {
            var allGameObjects = new List<GameObject>(hierarchyRoots.Count);
            foreach(GameObject root in hierarchyRoots)
            {
                allGameObjects.AddRange(root.GetAllChildrenIncludingSelf());
            }

            return allGameObjects;
        }

        public static void RecordObjectTransformsForUndo(IEnumerable<GameObject> gameObjects)
        {
            List<Transform> objectTransforms = GetObjectTransforms(gameObjects);
            UndoEx.RecordForToolAction(objectTransforms);
        }

        public static List<Transform> GetObjectTransforms(IEnumerable<GameObject> gameObjects)
        {
            var objectTransforms = new List<Transform>();
            foreach(GameObject gameObject in gameObjects)
            {
                objectTransforms.Add(gameObject.transform);
            }

            return objectTransforms;
        }

        public static List<OrientedBox> GetHierarchyWorldOrientedBoxes(List<GameObject> hierarchyRoots)
        {
            if (hierarchyRoots.Count == 0) return new List<OrientedBox>();

            var orientedBoxes = new List<OrientedBox>(hierarchyRoots.Count);
            foreach (GameObject selectedObject in hierarchyRoots)
            {
                OrientedBox orientedBox = selectedObject.GetHierarchyWorldOrientedBox();
                if (orientedBox.IsValid()) orientedBoxes.Add(orientedBox);
            }

            return orientedBoxes;
        }

        public static void SetSelectedHierarchyWireframeHidden(List<GameObject> hierarchyRoots, bool isWireframeHidden)
        {
            foreach(GameObject root in hierarchyRoots)
            {
                root.SetSelectedHierarchyWireframeHidden(isWireframeHidden);
            }
        }

        public static void SetHierarchyLayer(this GameObject root, int layer, bool allowUndoRedo)
        {
            var allObjects = root.GetAllChildrenIncludingSelf();
            AssignGameObjectsToLayer(allObjects, layer, allowUndoRedo);
        }

        public static void AssignGameObjectsToLayer(List<GameObject> gameObjects, int objectLayer, bool allowUndoRedo)
        {
            if(allowUndoRedo)
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    UndoEx.RecordForToolAction(gameObject);
                    gameObject.layer = objectLayer;
                }
            }
            else
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    gameObject.layer = objectLayer;
                }
            }
        }

        public static List<GameObject> GetObjectsWithMesh(List<GameObject> gameObjects)
        {
            if (gameObjects.Count == 0) return new List<GameObject>();

            var objectsWithMesh = new List<GameObject>(gameObjects.Count);
            foreach(var gameObject in gameObjects)
            {
                if (gameObject.HasMesh()) objectsWithMesh.Add(gameObject);
            }

            return objectsWithMesh;
        }
        #endregion
    }
}
#endif