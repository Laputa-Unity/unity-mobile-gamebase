#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class MeshCombineSettings : ScriptableObject
    {
        public enum MeshPivot
        {
            Center = 0,
            TopCenter,
            BottomCenter,
            LeftCenter,
            RightCenter,
            FrontCenter,
            BackCenter
        }

        public enum ColliderHandling
        {
            NoColliders = 0,
            Preserve,
            CreateNew
        }

        public enum ColliderPreserveParent
        {
            SameAsCombinedObjects = 0,
            OneForAll
        }

        public enum ColliderPreserveName
        {
            Default = 0,
            SameAsSource
        }

        public enum ColliderType
        {
            Mesh = 0,
            Box
        }

        public enum CombineMode
        {
            Duplicate = 0,
            Replace
        }

        [SerializeField]
        private bool _combineStaticMeshes = true;
        [SerializeField]
        private bool _combineDynamicMeshes = true;
        [SerializeField]
        private bool _makeCombinedMeshesStatic = true;
        [SerializeField]
        private MeshPivot _combinedMeshesPivot = MeshPivot.Center;

        [SerializeField]
        private bool _ignoreObjectsInHierarchies = true;

        [SerializeField]
        private bool _generateLightmapUVs = false;
        [SerializeField]
        private bool _markNoLongerReadable = true;

        [SerializeField]
        private bool _weldVertexPositions = false;
        [SerializeField]
        private float _vertexPositionWeldEpsilon = MinVertexPositionWeldEpsilon;
        [SerializeField]
        private bool _weldVertexPositionsOnlyForCommonMaterial = false;

        [SerializeField]
        private ColliderHandling _colliderHandling = ColliderHandling.Preserve;
        [SerializeField]
        private ColliderPreserveParent _colliderPreserveParent = ColliderPreserveParent.OneForAll;
        [SerializeField]
        private ColliderPreserveName _colliderPreserveName = ColliderPreserveName.SameAsSource;
        [SerializeField]
        private string _oneForAllColliderParentName = "colliderParent";

        [SerializeField]
        private ColliderType _newCollidersType = ColliderType.Mesh;
        [SerializeField]
        private bool _useConvexMeshColliders = false;
        [SerializeField]
        private bool _newCollidersAreTriggers = false;

        [SerializeField]
        private string _combinedObjectName = "cmbMeshObject";
        [SerializeField]
        private string _combinedMeshName = "cmbMesh";

        [SerializeField]
        private bool _disableSourceParent = true;
        [SerializeField]
        private GameObject _childrenCombineSourceParent;

        [SerializeField]
        private CombineMode _selectionCombineMode = CombineMode.Duplicate;
        [SerializeField]
        private GameObject _selectionCombineDestinationParent;

        [SerializeField]
        private MeshCombineWindow _window;

        public static float MinVertexPositionWeldEpsilon { get { return 1e-5f; } }
        public static float MaxVertexPositionWeldEpsilon { get { return 1e-1f; } }

        public bool CombineStaticMeshes { get { return _combineStaticMeshes; } set { _combineStaticMeshes = value; } }
        public bool CombineDynamicMeshes { get { return _combineDynamicMeshes; } set { _combineDynamicMeshes = value; } }
        public bool MakeCombinedMeshesStatic { get { return _makeCombinedMeshesStatic; } set { _makeCombinedMeshesStatic = value; } }
        public MeshPivot CombinedMeshesPivot { get { return _combinedMeshesPivot; } set { _combinedMeshesPivot = value; } }

        public bool IgnoreObjectsInHierarchies { get { return _ignoreObjectsInHierarchies; } set { _ignoreObjectsInHierarchies = value; } }

        public bool GenerateLightmapUVs { get { return _generateLightmapUVs; } set { _generateLightmapUVs = value; } }
        public bool MarkNoLongerReadable { get { return _markNoLongerReadable; } set { _markNoLongerReadable = value; } }

        public bool WeldVertexPositions { get { return _weldVertexPositions; } set { _weldVertexPositions = value; } }
        public float VertexPositionWeldEpsilon { get { return _vertexPositionWeldEpsilon; } set { _vertexPositionWeldEpsilon = Mathf.Clamp(value, MinVertexPositionWeldEpsilon, MaxVertexPositionWeldEpsilon); } }
        public bool WeldVertexPositionsOnlyForCommonMaterial { get { return _weldVertexPositionsOnlyForCommonMaterial; } set { _weldVertexPositionsOnlyForCommonMaterial = value; } }

        public ColliderHandling CollHandling { get { return _colliderHandling; } set { _colliderHandling = value; } }
        public ColliderPreserveParent CollPreserveParent { get { return _colliderPreserveParent; } set { _colliderPreserveParent = value; } }
        public ColliderPreserveName CollPreserveName { get { return _colliderPreserveName; } set { _colliderPreserveName = value; } }
        public string OneForAllColliderParentName { get { return _oneForAllColliderParentName; } set { if (value != null) _oneForAllColliderParentName = value; } }

        public ColliderType NewCollidersType { get { return _newCollidersType; } set { _newCollidersType = value; } }
        public bool UseConvexMeshColliders { get { return _useConvexMeshColliders; } set { _useConvexMeshColliders = value; } }
        public bool NewCollidersAreTriggers { get { return _newCollidersAreTriggers; } set { _newCollidersAreTriggers = value; } }

        public string CombinedObjectName { get { return _combinedObjectName; } set { if (value != null) _combinedObjectName = value; } }
        public string CombinedMeshName { get { return _combinedMeshName; } set { if (value != null) _combinedMeshName = value; } }

        public bool DisableSourceParent { get { return _disableSourceParent; } set { _disableSourceParent = value; } }
        public GameObject ChildrenCombineSourceParent { get { return _childrenCombineSourceParent; } set { _childrenCombineSourceParent = value; } }

        public CombineMode SelCombineMode { get { return _selectionCombineMode; } set { _selectionCombineMode = value; } }
        public GameObject SelectionCombineDestinationParent { get { return _selectionCombineDestinationParent; } set { _selectionCombineDestinationParent = value; } }

        public static Vector3 MeshPivotToWorldPoint(GameObject meshObject, MeshPivot meshPivot)
        {
            Box meshWorldOOBB = meshObject.GetMeshWorldBox();

            if (meshPivot == MeshPivot.Center) return meshWorldOOBB.Center;
            else if (meshPivot == MeshPivot.BackCenter) return meshWorldOOBB.GetBoxFaceCenter(BoxFace.Back);
            else if (meshPivot == MeshPivot.FrontCenter) return meshWorldOOBB.GetBoxFaceCenter(BoxFace.Front);
            else if (meshPivot == MeshPivot.BottomCenter) return meshWorldOOBB.GetBoxFaceCenter(BoxFace.Bottom);
            else if (meshPivot == MeshPivot.TopCenter) return meshWorldOOBB.GetBoxFaceCenter(BoxFace.Top);
            else if (meshPivot == MeshPivot.LeftCenter) return meshWorldOOBB.GetBoxFaceCenter(BoxFace.Left);
            else return meshWorldOOBB.GetBoxFaceCenter(BoxFace.Right);
        }

        public void ShowEditorWindow()
        {
            if (_window == null) _window = Octave3DWorldBuilder.ActiveInstance.EditorWindowPool.CreateWindow<MeshCombineWindow>();
            if (_window != null)
            {
                _window.MeshCombineSettings = this;
                _window.ShowOctave3DWindow();
            }
        }

        public void RenderView()
        {
            bool newBool; float newFloat; string newString;
            MeshPivot newMeshPivot;
            ColliderHandling newColliderHandling;
            ColliderPreserveParent newColliderPreserveParent;
            ColliderPreserveName newColliderPreserveName;
            ColliderType newColliderType;
            GameObject newParentObject;
            CombineMode newSelectionCombineMode;

            if(!_combineStaticMeshes && !_combineDynamicMeshes)
            {
                EditorGUILayout.HelpBox("Neither static nor dynamic meshes are allowed to be combined. At least one type must be involved in the combine " + 
                                        "process. Otherise, pressing the 'Combine###' buttons will have no effect.", UnityEditor.MessageType.Warning);
            }

            var content = new GUIContent();
            content.text = "Combine static meshes";
            content.tooltip = "Allows you to specify if static meshes should be combined. Uncheck this if you don't want to combine static meshes.";
            newBool = EditorGUILayout.ToggleLeft(content, CombineStaticMeshes);
            if(newBool != CombineStaticMeshes)
            {
                UndoEx.RecordForToolAction(this);
                CombineStaticMeshes = newBool;
            }

            content.text = "Combine dynamic meshes";
            content.tooltip = "Allows you to specify if dynamic meshes should be combined. Uncheck this if you don't want to combine dynamic meshes.";
            newBool = EditorGUILayout.ToggleLeft(content, CombineDynamicMeshes);
            if (newBool != CombineDynamicMeshes)
            {
                UndoEx.RecordForToolAction(this);
                CombineDynamicMeshes = newBool;
            }

            content.text = "Make combined static";
            content.tooltip = "If this is checked, the combined meshes will be marked as static. If not checked, they will be marked as dynamic.";
            newBool = EditorGUILayout.ToggleLeft(content, MakeCombinedMeshesStatic);
            if (newBool != MakeCombinedMeshesStatic)
            {
                UndoEx.RecordForToolAction(this);
                MakeCombinedMeshesStatic = newBool;
            }

            content.text = "Mesh pivot";
            content.tooltip = "Allows you to specify a pivot point for the combined meshes. All pivot point choices are relative to the bounding box of a combined mesh.";
            newMeshPivot = (MeshPivot)EditorGUILayout.EnumPopup(content, CombinedMeshesPivot);
            if (newMeshPivot != CombinedMeshesPivot)
            {
                UndoEx.RecordForToolAction(this);
                CombinedMeshesPivot = newMeshPivot;
            }

            EditorGUILayout.Separator();
            if (GenerateLightmapUVs) EditorGUILayout.HelpBox("Note: Lightmap UV generation can increase waiting times.", UnityEditor.MessageType.Info);
            content.text = "Generate lightmap UVs";
            content.tooltip = "You must check this if you wish to use lightmapping with the combined meshes.";
            newBool = EditorGUILayout.ToggleLeft(content, GenerateLightmapUVs);
            if (newBool != GenerateLightmapUVs)
            {
                UndoEx.RecordForToolAction(this);
                GenerateLightmapUVs = newBool;
            }

            content.text = "Mark no longer readable";
            content.tooltip = "If this is checked, the combined meshes will have their system memory data freed and reading the mesh data will no longer be possible at runtime.";
            newBool = EditorGUILayout.ToggleLeft(content, MarkNoLongerReadable);
            if (newBool != MarkNoLongerReadable)
            {
                UndoEx.RecordForToolAction(this);
                MarkNoLongerReadable = newBool;
            }

            content.text = "Ignore objects in hierarchies";
            content.tooltip = "If this is checked, objects which are part of a multi-level hierarchy will not be combined.";
            newBool = EditorGUILayout.ToggleLeft(content, IgnoreObjectsInHierarchies);
            if (newBool != IgnoreObjectsInHierarchies)
            {
                UndoEx.RecordForToolAction(this);
                IgnoreObjectsInHierarchies = newBool;
            }

            EditorGUILayout.Separator();
            if (WeldVertexPositions) EditorGUILayout.HelpBox("Note: Vertex welding can increase waiting times especially for a large number of vertices.", UnityEditor.MessageType.Info);
            content.text = "Weld vertex positions";
            content.tooltip = "Allows you to specify if the vertex positions must be welded based on a specified weld epsilon.";
            newBool = EditorGUILayout.ToggleLeft(content, WeldVertexPositions);
            if (newBool != WeldVertexPositions)
            {
                UndoEx.RecordForToolAction(this);
                WeldVertexPositions = newBool;
            }

            if (WeldVertexPositions)
            {
                content.text = "Weld only for common material";
                content.tooltip = "If this is checked, vertex welding will be performed only for meshes that share a common material. Otherwise, " + 
                                  "the welding process will be applied to all meshes which were generated by the mesh combine process.";
                newBool = EditorGUILayout.ToggleLeft(content, WeldVertexPositionsOnlyForCommonMaterial);
                if (newBool != WeldVertexPositionsOnlyForCommonMaterial)
                {
                    UndoEx.RecordForToolAction(this);
                    WeldVertexPositionsOnlyForCommonMaterial = newBool;
                }

                content.text = "Weld epsilon";
                content.tooltip = "This is the vertex position weld epsilon value. Two or more vertices will be welded if the distance between them is < than the weld epsilon.";
                newFloat = EditorGUILayout.FloatField(content, VertexPositionWeldEpsilon);
                if (newFloat != VertexPositionWeldEpsilon)
                {
                    UndoEx.RecordForToolAction(this);
                    VertexPositionWeldEpsilon = newFloat;
                }
            }

            EditorGUILayout.Separator();
            content.text = "Collider handling";
            content.tooltip = "Allows you to specify how the tool will handle colliders (don't do anything, preserve existing colliders, create new etc).";
            newColliderHandling = (ColliderHandling)EditorGUILayout.EnumPopup(content, CollHandling);
            if(newColliderHandling != CollHandling)
            {
                UndoEx.RecordForToolAction(this);
                CollHandling = newColliderHandling;
            }

            if(CollHandling == ColliderHandling.Preserve)
            {
                content.text = "Preserve parent";
                content.tooltip = "When collider handling is set to \'Preserve\', the tool will have to create new collider objects and it has to know what is the parent of those objects. " + 
                                  "This field allows you to choose the parent for the new collider objects.";
                newColliderPreserveParent = (ColliderPreserveParent)EditorGUILayout.EnumPopup(content, CollPreserveParent);
                if(newColliderPreserveParent != CollPreserveParent)
                {
                    UndoEx.RecordForToolAction(this);
                    CollPreserveParent = newColliderPreserveParent;
                }

                content.text = "Preserved collider name";
                content.tooltip = "Allows you to control the names of the collider objects that are created for collider preservation.";
                newColliderPreserveName = (ColliderPreserveName)EditorGUILayout.EnumPopup(content, CollPreserveName);
                if(newColliderPreserveName != CollPreserveName)
                {
                    UndoEx.RecordForToolAction(this);
                    CollPreserveName = newColliderPreserveName;
                }

                if(CollPreserveParent == ColliderPreserveParent.OneForAll)
                {
                    content.text = "Collider parent name";
                    content.tooltip = "The name of the parent object which will hold all collider objects. Used when the preserve mode is set to \'NewForAll\'.";
                    newString = EditorGUILayout.TextField(content, OneForAllColliderParentName);
                    if(newString != OneForAllColliderParentName)
                    {
                        UndoEx.RecordForToolAction(this);
                        OneForAllColliderParentName = newString;
                    }
                }
            }

            if(CollHandling == ColliderHandling.CreateNew)
            {
                content.text = "Collider type";
                content.tooltip = "The type of colliders to create.";
                newColliderType = (ColliderType)EditorGUILayout.EnumPopup(content, NewCollidersType);
                if(newColliderType != NewCollidersType)
                {
                    UndoEx.RecordForToolAction(this);
                    NewCollidersType = newColliderType;
                }

                if (NewCollidersType == ColliderType.Mesh && NewCollidersAreTriggers && !UseConvexMeshColliders)
                    EditorGUILayout.HelpBox("Mesh colliders can only be triggers when marked as convex. Unity will not allow concave meshes to be triggers.", UnityEditor.MessageType.Warning);

                content.text = "Colliders are triggers";
                content.tooltip = "If this is checked, all colliders which will be created will be marked as triggers.";
                newBool = EditorGUILayout.ToggleLeft(content, NewCollidersAreTriggers);
                if (newBool != NewCollidersAreTriggers)
                {
                    UndoEx.RecordForToolAction(this);
                    NewCollidersAreTriggers = newBool;
                }

                if(NewCollidersType == ColliderType.Mesh)
                {
                    content.text = "Convex mesh colliders";
                    content.tooltip = "If this is checked, the created mesh colliders will be made convex.";
                    newBool = EditorGUILayout.ToggleLeft(content, UseConvexMeshColliders);
                    if(newBool != UseConvexMeshColliders)
                    {
                        UndoEx.RecordForToolAction(this);
                        UseConvexMeshColliders = newBool;
                    }
                }
            }

            EditorGUILayout.Separator();
            content.text = "Combined object name";
            content.tooltip = "The name which will be assigned to all objects created during the mesh combine. These are the objects that hold the generated combined meshes.";
            newString = EditorGUILayout.TextField(content, CombinedObjectName);
            if (newString != CombinedObjectName)
            {
                UndoEx.RecordForToolAction(this);
                CombinedObjectName = newString;
            }

            content.text = "Combined mesh name";
            content.tooltip = "The name which will be assigned to all meshes created during the mesh combine.";
            newString = EditorGUILayout.TextField(content, CombinedMeshName);
            if (newString != CombinedMeshName)
            {
                UndoEx.RecordForToolAction(this);
                CombinedMeshName = newString;
            }

            EditorGUILayout.Separator();
            content.text = "Disable source parent";
            content.tooltip = "If this is checked, the tool will disable the parent of the children which participate in the mesh combine process.";
            newBool = EditorGUILayout.ToggleLeft(content, DisableSourceParent);
            if (newBool != DisableSourceParent)
            {
                UndoEx.RecordForToolAction(this);
                DisableSourceParent = newBool;
            }

            EditorGUILayout.BeginHorizontal();
            content.text = "Combine children";
            content.tooltip = "Combines all the children of the specified object (the field to the right of the button).";
            if(GUILayout.Button(content, GUILayout.Width(120.0f)))
            {
                var meshCombiner = new MeshCombiner();
                meshCombiner.CombineSettings = this;
                meshCombiner.CombineChildren(ObjectPlacementGuide.ExistsInScene ? new List<GameObject> { ObjectPlacementGuide.Instance.gameObject } : null);
            }
            content.text = "";
            content.tooltip = "";
            newParentObject = EditorGUILayout.ObjectField(content, ChildrenCombineSourceParent, typeof(GameObject), true) as GameObject;
            if(newParentObject != ChildrenCombineSourceParent)
            {
                if (newParentObject == null || newParentObject.IsSceneObject())
                {
                    UndoEx.RecordForToolAction(this);
                    ChildrenCombineSourceParent = newParentObject;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            content.text = "Combine selection";
            content.tooltip = "Combines the currently selected objects using the specified selection combine mode  (the field to the right of the button).";
            if (GUILayout.Button(content, GUILayout.Width(120.0f)))
            {
                var meshCombiner = new MeshCombiner();
                meshCombiner.CombineSettings = this;
                meshCombiner.CombineSelectedObjects(ObjectSelection.Get().GetAllSelectedGameObjects(),
                                                    ObjectPlacementGuide.ExistsInScene ? new List<GameObject> { ObjectPlacementGuide.Instance.gameObject } : null);
            }
            content.text = "";
            content.tooltip = "";
            newSelectionCombineMode = (CombineMode)EditorGUILayout.EnumPopup(content, SelCombineMode);
            if(newSelectionCombineMode != SelCombineMode)
            {
                UndoEx.RecordForToolAction(this);
                SelCombineMode = newSelectionCombineMode;
            }
            EditorGUILayout.EndHorizontal();

            content.text = "Destination parent";
            content.tooltip = "All combined selected objects will be attached to this parent.";
            newParentObject = EditorGUILayout.ObjectField(content, SelectionCombineDestinationParent, typeof(GameObject), true) as GameObject;
            if (newParentObject != SelectionCombineDestinationParent)
            {
                if (newParentObject == null || newParentObject.IsSceneObject())
                {
                    UndoEx.RecordForToolAction(this);
                    SelectionCombineDestinationParent = newParentObject;
                }
            }
        }
    }
}
#endif