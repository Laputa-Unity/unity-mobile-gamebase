#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [ExecuteInEditMode]
    public class Octave3DWorldBuilder : MonoBehaviour
    {
        #region Private Variables
        private static Octave3DWorldBuilder _lastActiveInstance;

        private ObjectPlacementSceneViewEventHandler _objectPlacementSceneViewHandler = new ObjectPlacementSceneViewEventHandler();
        private ObjectSelectionSceneViewEventHandler _objectSelectionSceneViewHandler = new ObjectSelectionSceneViewEventHandler();
        private ObjectEraserSceneViewEventHandler _objectEraseSceneViewHandler = new ObjectEraserSceneViewEventHandler();

        [SerializeField]
        private Transform _transform;
        
        [SerializeField]
        private bool _showGUIHints = true;

        [SerializeField]
        private Octave3DScene _octave3DScene = new Octave3DScene();
        private SceneRenderer _sceneRenderer = new SceneRenderer();
        private ToolSupervisor _toolSupervisor = new ToolSupervisor();
        private ToolResources _toolResources = new ToolResources();
        [SerializeField]
        private PrefabPreviewGenerator _prefabPreviewGenerator = new PrefabPreviewGenerator();
     
        private Octave3DConfigSaveLoadSettings _configSaveSettings;
        private Octave3DConfigSaveLoadSettings _configLoadSettings;

        [SerializeField]
        private EditorWindowPool _editorWindowPool;

        [SerializeField]
        private PrefabCategoryDatabase _prefabCategoryDatabase;
        [SerializeField]
        private ObjectLayerDatabase _objectLayerDatabase;
        [SerializeField]
        private PrefabTagDatabase _prefabTagDatabase;
        [SerializeField]
        private ObjectPlacementPathHeightPatternDatabase _objectPlacementPathHeightPatternDatabase;
        [SerializeField]
        private ObjectPlacementPathTileConnectionConfigurationDatabase _objectPlacementPathTileConnectionConfigurationDatabase;
        [SerializeField]
        private DecorPaintObjectPlacementBrushDatabase _decorPaintObjectPlacementBrushDatabase;
        [SerializeField]
        private ObjectGroupDatabase _placementObjectGroupDatabase;

        [SerializeField]
        private ObjectPlacement _objectPlacement;
        [SerializeField]
        private ObjectSnapping _objectSnapping;
        [SerializeField]
        private ObjectEraser _objectEraser;
        [SerializeField]
        private ObjectSelection _objectSelection;

        [SerializeField]
        private PrefabsToCategoryDropEventHandler _prefabsToCategoryDropEventHandler = new PrefabsToCategoryDropEventHandler();
        [SerializeField]
        private PrefabsToPathTileConectionDropEventHandler _prefabsToPathTileConnectionDropEventHandler = new PrefabsToPathTileConectionDropEventHandler();
        [SerializeField]
        private PrefabsToDecorPaintBrushEventHandler _prefabsToDecorPaintBrushEventHandler = new PrefabsToDecorPaintBrushEventHandler();
        [SerializeField]
        private FolderToPrefabCreationFolderField _folderToPrefabCreationFolderField = new FolderToPrefabCreationFolderField();

        [SerializeField]
        private Inspector _inspector;
        [SerializeField]
        private MeshCombineSettings _meshCombineSettings;

        [SerializeField]
        private ShaderPool _shaderPool = new ShaderPool();
        [SerializeField]
        private MaterialPool _materialPool = new MaterialPool();
        #endregion

        #region Public Properties
        public PrefabPreviewGenerator PrefabPreviewGenerator { get { return _prefabPreviewGenerator; } }
        public ShaderPool ShaderPool { get { return _shaderPool; } }
        public MaterialPool MaterialPool { get { return _materialPool; } }

        public EditorWindowPool EditorWindowPool
        {
            get
            {
                if (_editorWindowPool == null) CreateEditorWindowPool();
                return _editorWindowPool;
            }
        }
        public bool ShowGUIHints { get { return _showGUIHints; } set { _showGUIHints = value; } }
        public Octave3DScene Octave3DScene { get { return _octave3DScene; } }
        public Octave3DConfigSaveLoadSettings ConfigSaveSettings
        {
            get
            {
                if (_configSaveSettings == null) _configSaveSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<Octave3DConfigSaveLoadSettings>();
                return _configSaveSettings;
            }
        }
        public Octave3DConfigSaveLoadSettings ConfigLoadSettings
        {
            get
            {
                if (_configLoadSettings == null) _configLoadSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<Octave3DConfigSaveLoadSettings>();
                return _configLoadSettings;
            }
        }
        public Octave3DConfigSaveWindow ConfigSaveWindow { get { return EditorWindowPool.Octave3DConfigSaveWindow; } }
        public Octave3DConfigLoadWindow ConfigLoadWindow { get { return EditorWindowPool.Octave3DConfigLoadWindow; } }
        public SceneRenderer SceneRenderer { get { return _sceneRenderer; } }
        public ToolSupervisor ToolSupervisor { get { return _toolSupervisor; } }
        public ToolResources ToolResources { get { return _toolResources; } }
        public PrefabCategoryDatabase PrefabCategoryDatabase
        {
            get
            {
                if (_prefabCategoryDatabase == null) _prefabCategoryDatabase = CreateScriptableObject<PrefabCategoryDatabase>();
                return _prefabCategoryDatabase;
            }
        }
        public ObjectLayerDatabase ObjectLayerDatabase
        {
            get
            {
                if (_objectLayerDatabase == null) _objectLayerDatabase = CreateScriptableObject<ObjectLayerDatabase>();
                return _objectLayerDatabase;
            }
        }
        public PrefabTagDatabase PrefabTagDatabase
        {
            get
            {
                if (_prefabTagDatabase == null) _prefabTagDatabase = CreateScriptableObject<PrefabTagDatabase>();
                return _prefabTagDatabase;
            }
        }
        public ObjectPlacementPathHeightPatternDatabase ObjectPlacementPathHeightPatternDatabase
        {
            get
            {
                if (_objectPlacementPathHeightPatternDatabase == null) _objectPlacementPathHeightPatternDatabase = CreateScriptableObject<ObjectPlacementPathHeightPatternDatabase>();
                return _objectPlacementPathHeightPatternDatabase;
            }
        }
        public ObjectPlacementPathTileConnectionConfigurationDatabase ObjectPlacementPathTileConnectionConfigurationDatabase
        {
            get
            {
                if (_objectPlacementPathTileConnectionConfigurationDatabase == null) _objectPlacementPathTileConnectionConfigurationDatabase = CreateScriptableObject<ObjectPlacementPathTileConnectionConfigurationDatabase>();
                return _objectPlacementPathTileConnectionConfigurationDatabase;
            }
        }
        public DecorPaintObjectPlacementBrushDatabase DecorPaintObjectPlacementBrushDatabase
        {
            get
            {
                if (_decorPaintObjectPlacementBrushDatabase == null) _decorPaintObjectPlacementBrushDatabase = CreateScriptableObject<DecorPaintObjectPlacementBrushDatabase>();
                return _decorPaintObjectPlacementBrushDatabase;
            }
        }
        public ObjectGroupDatabase PlacementObjectGroupDatabase
        {
            get
            {
                if (_placementObjectGroupDatabase == null) _placementObjectGroupDatabase = CreateScriptableObject<ObjectGroupDatabase>();
                return _placementObjectGroupDatabase;
            }
        }

        public ObjectPlacement ObjectPlacement
        {
            get
            {
                if (_objectPlacement == null) _objectPlacement = CreateScriptableObject<ObjectPlacement>();
                return _objectPlacement;
            }
        }
        public ObjectSnapping ObjectSnapping
        {
            get
            {
                if (_objectSnapping == null) _objectSnapping = CreateScriptableObject<ObjectSnapping>();
                return _objectSnapping;
            }
        }
        public ObjectEraser ObjectEraser
        {
            get
            {
                if (_objectEraser == null) _objectEraser = CreateScriptableObject<ObjectEraser>();
                return _objectEraser;
            }
        }
        public ObjectSelection ObjectSelection
        {
            get
            {
                if (_objectSelection == null) _objectSelection = CreateScriptableObject<ObjectSelection>();
                return _objectSelection;
            }
        }
        public MeshCombineSettings MeshCombineSettings
        {
            get
            {
                if (_meshCombineSettings == null) _meshCombineSettings = CreateScriptableObject<MeshCombineSettings>();
                return _meshCombineSettings;
            }
        }

        public PrefabsToCategoryDropEventHandler PrefabsToCategoryDropEventHandler { get { return _prefabsToCategoryDropEventHandler; } }
        public PrefabsToPathTileConectionDropEventHandler PrefabsToPathTileConectionDropEventHandler { get { return _prefabsToPathTileConnectionDropEventHandler; } }
        public PrefabsToDecorPaintBrushEventHandler PrefabsToDecorPaintBrushEventHandler { get { return _prefabsToDecorPaintBrushEventHandler; } }
        public FolderToPrefabCreationFolderField FolderToPrefabCreationFolderField { get { return _folderToPrefabCreationFolderField; } }

        public PrefabsToCategoryDropSettingsWindow PrefabsToCategoryDropSettingsWindow { get { return EditorWindowPool.PrefabsToCategoryDropSettingsWindow; } }
        public ObjectPlacementSettingsWindow ObjectPlacementSettingsWindow { get { return EditorWindowPool.ObjectPlacementSettingsWindow; } }
        public PrefabManagementWindow PrefabManagementWindow { get { return EditorWindowPool.PrefabManagementWindow; } }
        public ActivePrefabCategoryView ActivePrefabCategoryView { get { return EditorWindowPool.ActivePrefabCategoryView; } }
        public ActivePrefabView ActivePrefabView { get { return EditorWindowPool.ActivePrefabView; } }

        public PrefabTagsWindow PrefabTagsWindow { get { return EditorWindowPool.PrefabTagsWindow; } }
        public ObjectLayersWindow ObjectLayersWindow { get { return EditorWindowPool.ObjectLayersWindow; } }
        public Inspector Inspector
        {
            get
            {
                if (_inspector == null)
                {
                    _inspector = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<Inspector>();
                    _inspector.Initialize();
                }
                return _inspector;
            }
        }
        #endregion

        #region Public Static Properties
        public static Octave3DWorldBuilder ActiveInstance 
        { 
            get 
            {
                GameObject activeGameObject = Selection.activeGameObject;
                if (activeGameObject == null) return _lastActiveInstance;
                if (_lastActiveInstance != null && activeGameObject == _lastActiveInstance.gameObject) return _lastActiveInstance;
                
                Octave3DWorldBuilder octave3D = activeGameObject.GetComponent<Octave3DWorldBuilder>();
                if (octave3D != null)
                {
                    _lastActiveInstance = octave3D;
                    return _lastActiveInstance;
                }
                else return _lastActiveInstance;
            } 
        }

        public static int NumInstances
        {
            get
            {
                return FindObjectsOfType<Octave3DWorldBuilder>().Length;
            }
        }
        #endregion

        #region Public Methods
        public void RepaintAllEditorWindows()
        {
            EditorWindowPool.RepaintAll();
        }

        public T CreateScriptableObject<T>() where T : ScriptableObject
        {
            return ScriptableObject.CreateInstance<T>();
        }

        public void ShowGUIHint(string hint)
        {
            if(_showGUIHints) EditorGUILayout.HelpBox(hint, UnityEditor.MessageType.Info);
        }

        public List<GameObject> GetSceneObjects()
        {
            return new List<GameObject>(FindObjectsOfType<GameObject>());
        }

        public List<GameObject> GetSceneObjectsExceptPlacementGuide()
        {
            List<GameObject> sceneObjects = GetSceneObjects();
            if (ObjectPlacementGuide.ExistsInScene) sceneObjects.RemoveAll(item => ObjectPlacementGuide.Equals(item) || ObjectPlacementGuide.ContainsChild(item.transform));

            return sceneObjects;
        }

        public List<GameObject> GetImmediateChildrenExcludingPlacementGuide()
        {
            List<GameObject> immediateChildren = gameObject.GetImmediateChildren();
            if (ObjectPlacementGuide.ExistsInScene) immediateChildren.RemoveAll(item => ObjectPlacementGuide.Equals(item) || ObjectPlacementGuide.ContainsChild(item.transform));

            return immediateChildren;
        }

        public List<GameObject> GetAllWorkingObjects()
        {
            List<GameObject> allWorkingObjects = GetSceneObjectsExceptPlacementGuide();
            allWorkingObjects.RemoveAll(item => IsPivotWorkingObject(item));

            return allWorkingObjects;
        }

        public bool IsWorkingObject(GameObject gameObj)
        {
            if (gameObj == null) return false;
            if (ObjectQueries.IsGameObjectPartOfPlacementGuideHierarchy(gameObj)) return false;
            return true;
        }

        public GameObject GetRoot(GameObject workingObject)
        {
            if (!IsWorkingObject(workingObject)) return null;

            Transform workingObjectTransform = workingObject.transform;
            if (workingObjectTransform.parent == null) return workingObject;

            Transform currentTransform = workingObjectTransform;
            while(currentTransform != null)
            {
                if (currentTransform.parent != null && IsPivotWorkingObject(currentTransform.parent.gameObject)) return currentTransform.gameObject;
                currentTransform = currentTransform.parent;
            }

            return workingObjectTransform.root.gameObject;
        }

        public List<GameObject> GetRoots(IEnumerable<GameObject> sceneObjects)
        {
            var roots = new List<GameObject>(50);
            var rootHash = new HashSet<GameObject>();
            foreach (GameObject gameObject in sceneObjects)
            {
                var root = Octave3DWorldBuilder.ActiveInstance.GetRoot(gameObject);
                if (root == null || rootHash.Contains(root)) continue;

                roots.Add(root);
                rootHash.Add(root);
            }
            rootHash.Clear();

            return roots;
        }

        public bool IsPivotWorkingObject(GameObject workingObject)
        {
            List<Type> pivotWorkingObjectTypes = GetPivotWorkingObjectTypes();
            return workingObject.HasComponentsOfAnyType(pivotWorkingObjectTypes) || PlacementObjectGroupDatabase.ContainsObjectGroup(workingObject);
        }

        public List<Type> GetPivotWorkingObjectTypes()
        {
            return new List<Type> { typeof(Octave3DWorldBuilder), typeof(Terrain) };
        }

        public void SelectInSceneView()
        {
            List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>();
            selectedObjects.Add(this.gameObject);
            Selection.objects = selectedObjects.ToArray();
        }

        public void OnInspectorEnabled()
        {
            _lastActiveInstance = this;
        }
        #endregion

        #region Private Static Functions
        [MenuItem("Tools/Octave3D/Deselect &D")]
        private static void DeselectObjectGrid3D()
        {
            if (Octave3DWorldBuilder.ActiveInstance != null)
            {
                Octave3DWorldBuilder.ActiveInstance.DeselectInSceneView();

                // Clear states. Otherwise, the 'D' button state will remain set to true.
                KeyboardButtonStates.Instance.ClearStates();
            }
        }

        [MenuItem("Tools/Octave3D/Select &R")]
        private static void SelectObjectGrid3D()
        {
            Octave3DWorldBuilder[] instances = FindObjectsOfType<Octave3DWorldBuilder>();
            if (instances.Length != 0) instances[0].SelectInSceneView();
        }

        [MenuItem("Tools/Octave3D/Focus #&F")]
        private static void FocusGameObject()
        {
            if (Octave3DWorldBuilder.ActiveInstance != null)
            {
                var currentActive = Selection.activeGameObject;
                Selection.activeGameObject = Octave3DWorldBuilder.ActiveInstance.ObjectSelection.GetLastSelectedGameObject();
                if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.FrameSelected();
                Selection.activeGameObject = currentActive;

                Octave3DWorldBuilder.ActiveInstance.SelectInSceneView();
            }
        }

        [MenuItem("GameObject/Octave3D/Make group", false, 0)]
        private static void MakeGroup()
        {
            Octave3DWorldBuilder octave3D = Octave3DWorldBuilder.ActiveInstance;
            if (octave3D == null)
            {
                Debug.LogWarning("There is no Octave3D object in the scene. Please create an empty game object and attach the Octave3D World Builder script to it.");
                return;
            }

            octave3D.PlacementObjectGroupDatabase.CreateObjectGroup(Selection.activeGameObject);
        }

        [MenuItem("GameObject/Octave3D/Is not group", false, 0)]
        private static void SetNoLoLongerGroup()
        {
            Octave3DWorldBuilder octave3D = Octave3DWorldBuilder.ActiveInstance;
            if (octave3D == null)
            {
                Debug.LogWarning("There is no Octave3D object in the scene. Please create an empty game object and attach the Octave3D World Builder script to it.");
                return;
            }

            octave3D.PlacementObjectGroupDatabase.MakeNoLongerGroup(Selection.activeGameObject);
        }
        #endregion

        #region Private methods
        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            ToolWasStartedMessage.SendToInterestedListeners();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (Selection.activeTransform == null) return;
            if (Selection.activeTransform.GetComponent<Octave3DWorldBuilder>() == null) return;

            switch(Inspector.ActiveInspectorGUIIdentifier)
            {
                case InspectorGUIIdentifier.ObjectPlacement:
                case InspectorGUIIdentifier.ObjectSnapping:
        
                    _objectPlacementSceneViewHandler.HandleSceneViewEvent(Event.current);
                    break;

                case InspectorGUIIdentifier.ObjectSelection:
                
                    _objectSelectionSceneViewHandler.HandleSceneViewEvent(Event.current);
                    break;

                case InspectorGUIIdentifier.ObjectErase:
                    _objectEraseSceneViewHandler.HandleSceneViewEvent(Event.current);
                    break;
            }
        }

        private void OnEnable()
        {
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

            #if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
            #else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            #endif
            EditorApplication.update += EditorUpdate;

            EditorWindowPool.RepaintAll();
        }

        private void Reset()
        {
            _transform = transform;
            DestroyPools();
            _prefabPreviewGenerator.DestroyData();
        }

        private void OnDisable()
        {
            #if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
            #else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            #endif
            EditorApplication.update -= EditorUpdate;
        }

        private void OnDestroy()
        {
            #if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
            #else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            #endif

            EditorApplication.update -= EditorUpdate;
            int numInstances = NumInstances;
            if (numInstances == 1)
            {
                MessageListenerDatabase.Instance.Clear();
                ToolResources.DisposeResources();
            }

            if (numInstances != 0) DestroyPools();
            _prefabPreviewGenerator.DestroyData();
        }

        private string GetScriptableObjectAssetFolderName()
        {
            return FileSystem.GetToolFolderName() + "/Scriptable Objects/Octave3D_" + this.GetHashCode();
        }

        private void DestroyPools()
        {
            EditorWindowPool[] editorWindowPools = gameObject.GetComponents<EditorWindowPool>();
            foreach (var comp in editorWindowPools)
            {
                if (comp != null)
                    DestroyImmediate(comp);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if(ActiveInstance != null)
            {
                _sceneRenderer.RenderGizmos();
               // _octave3DScene.RenderGizmosDebug();
            }
        }

        private void Update()
        {
            if(!Application.isPlaying)
            {
                if (ActiveInstance != null)
                {
                    EnsureTransformDataIsCorrect();
                    _octave3DScene.Update();
                }
            }
        }

        private void EnsureTransformDataIsCorrect()
        {
            EnsurePositionIsZero();
            EnsureRotationIsIdentity();
            EnsureScaleIsOne();
        }

        private void EnsurePositionIsZero()
        {
            if (_transform.position != Vector3.zero) _transform.position = Vector3.zero;
        }

        private void EnsureRotationIsIdentity()
        {
            if (_transform.rotation.eulerAngles != Vector3.zero) _transform.rotation = Quaternion.identity;
        }

        private void EnsureScaleIsOne()
        {
            if (_transform.localScale != Vector3.one) _transform.localScale = Vector3.one;
        }

        private void CreateEditorWindowPool()
        {
            _editorWindowPool = gameObject.AddComponent<EditorWindowPool>();
            _editorWindowPool.hideFlags = HideFlags.HideInInspector;
        }

        private void DeselectInSceneView()
        {
            List<UnityEngine.Object> selectedObjects = new List<UnityEngine.Object>(Selection.objects);
            selectedObjects.Remove(Octave3DWorldBuilder.ActiveInstance.gameObject);
            Selection.objects = selectedObjects.ToArray();
        }

        private double _lastCamFocusTime;
        private bool _camWasFocused;

        public void OnCamFocused()
        {
            _camWasFocused = true;
            _lastCamFocusTime = EditorApplication.timeSinceStartup;
        }

        private void EditorUpdate()
        {
            if (ObjectSelection.Get() != null) ObjectSelection.Get().RemoveNullObjects();
            if (_camWasFocused && (EditorApplication.timeSinceStartup - _lastCamFocusTime) > 0.6f)
            {
                _camWasFocused = false;
                _lastCamFocusTime = 0.0f;
                SelectInSceneView();
            }
        }
#endregion
    } 
}
#endif