#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace O3DWB
{
    [ExecuteInEditMode]
    public class ObjectPlacementGuide : MonoBehaviour, IMessageListener
    {
        #region Private Static Variables
        private static string _nameSuffix = "Active Prefab";
        private static ObjectPlacementGuide _instance;
        #endregion

        #region Private Variables
        [SerializeField]
        private Prefab _sourcePrefab;
        [SerializeField]
        private Transform _transform;

        private ObjectMouseUniformScaleSession _mouseUniformScaleSession = new ObjectMouseUniformScaleSession();
        private ObjectMouseRotationSession _mouseRotationSession = new ObjectMouseRotationSession();
        private ObjectMouseMoveAlongDirectionSession _mouseMoveAlongDirectionSession = new ObjectMouseMoveAlongDirectionSession();
        #endregion

        #region Public Static Properties
        public static ObjectPlacementGuide Instance
        {
            get
            {
                if (_instance == null) InstantiateFromActivePrefabIfPossible();
                return _instance;
            }
        }

        public static GameObject SceneObject { get { return ExistsInScene ? Instance.gameObject : null; } }
        public static bool ExistsInScene { get { return _instance != null; } }
        public static bool ExistsInSceneAndIsActive { get { return ExistsInScene && Instance.gameObject.activeSelf; } }
        public static bool Active { set { if (ExistsInScene) Instance.gameObject.SetActive(value); } }
        #endregion

        #region Public Properties
        public Prefab SourcePrefab { get { return _sourcePrefab; } }
        public bool IsSourcePrefabAvailable { get { return SourcePrefab != null && SourcePrefab.UnityPrefab != null; } }
        public Vector3 WorldPosition { get { return _transform.position; } }
        public Quaternion WorldRotation { get { return _transform.rotation; } }
        public Vector3 WorldScale { get { return _transform.lossyScale; } set { gameObject.SetWorldScale(value); } }
        public bool IsMouseMoveAlongDirectionSessionActive { get { return _mouseMoveAlongDirectionSession.IsActive; } }
        public bool IsMouseUniformScaleSessionActive { get { return _mouseUniformScaleSession.IsActive; } }
        public bool IsMouseRotationSessionActive { get { return _mouseRotationSession.IsActive; } }
        public bool IsMouseRotationSessionForCustomAxis { get { return _mouseRotationSession.RotatingAroundCustomAxis; } }
        public TransformAxis MouseRotationSessionAxis { get { return _mouseRotationSession.RotationAxis; } }
        public bool IsAnyMouseSessionActive { get { return IsMouseRotationSessionActive || IsMouseUniformScaleSessionActive || IsMouseMoveAlongDirectionSessionActive; } }
        #endregion

        #region Public Static Functions
        public static bool ContainsChild(Transform potentialChild)
        {
            if (ExistsInScene) return potentialChild != null && potentialChild.IsChildOf(Instance._transform);
            return false;
        }

        public static bool Equals(GameObject gameObject)
        {
            if (ExistsInScene) return gameObject == Instance.gameObject;
            return false;
        }

        public static void CreateFromActivePrefabIfNotExists()
        {
            if (!ExistsInScene) InstantiateFromActivePrefabIfPossible();
        }

        public static void DestroyIfExists()
        {
            if (ExistsInScene) Octave3DWorldBuilder.DestroyImmediate(ObjectPlacementGuide.Instance.gameObject);
        }
        #endregion

        #region Public Methods
        public void BeginMouseMoveAlongDirectionSession(Vector3 moveDirection)
        {
            if (!IsAnyMouseSessionActive) _mouseMoveAlongDirectionSession.Begin(gameObject, moveDirection, ObjectPlacementGuideSettings.Get().MouseOffsetFromPlacementSurfaceSettings);
        }

        public void EndMouseMoveAlongDirectionSession()
        {
            _mouseMoveAlongDirectionSession.End();
        }

        public void BeginMouseScaleSession(Vector3 scalePivotPoint)
        {
            if (!IsAnyMouseSessionActive)
            {
                _mouseUniformScaleSession.PivotPoint = scalePivotPoint;
                _mouseUniformScaleSession.Begin(gameObject, ObjectPlacementSettings.Get().ObjectPlacementGuideSettings.MouseUniformScaleSettings);
            }
        }

        public void EndMouseScaleSession()
        {
            _mouseUniformScaleSession.End();
        }

        public void BeginMouseRotationSession(TransformAxis rotationAxis)
        {
            if (!IsAnyMouseSessionActive)
                _mouseRotationSession.BeginRotationAroundAxis(gameObject, ObjectPlacementSettings.Get().ObjectPlacementGuideSettings.MouseRotationSettings, rotationAxis);
        }

        public void BeginMouseRotationSession(Vector3 customRotationAxis)
        {
            if (!IsAnyMouseSessionActive)
                _mouseRotationSession.BeginRotationAroundCustomAxis(gameObject, ObjectPlacementSettings.Get().ObjectPlacementGuideSettings.MouseRotationSettings, customRotationAxis);
        }

        public void EndMouseRotationSession()
        {
            _mouseRotationSession.End();
        }

        public void UpdateActiveMouseSessionsForMouseMovement(Event e)
        {
            if(IsMouseMoveAlongDirectionSessionActive) _mouseMoveAlongDirectionSession.UpdateForMouseMovement(e);
            else
            if(IsMouseUniformScaleSessionActive)
            {
                _mouseUniformScaleSession.UpdateForMouseMovement(e);
                UndoEx.RecordForToolAction(_sourcePrefab.ActivationSettings);
                _sourcePrefab.ActivationSettings.WorldScale = _transform.lossyScale;
            }
            else
            if(IsMouseRotationSessionActive)
            {
                _mouseRotationSession.UpdateForMouseMovement(e);
                UndoEx.RecordForToolAction(_sourcePrefab.ActivationSettings);
                _sourcePrefab.ActivationSettings.WorldRotation = _transform.rotation;
            }
        }

        public void SetHierarchyWorldRotationAndPreserveHierarchyCenter(Quaternion rotation)
        {
            if (IsAnyMouseSessionActive) return;

            UndoEx.RecordForToolAction(_transform);
            gameObject.SetHierarchyWorldRotationAndPreserveHierarchyCenter(rotation);

            UndoEx.RecordForToolAction(_sourcePrefab.ActivationSettings);
            _sourcePrefab.ActivationSettings.WorldRotation = _transform.rotation;
        }

        public void SetHierarchyWorldScaleByPivotPoint(float scale, Vector3 pivotPoint)
        {
            if (IsAnyMouseSessionActive) return;

            UndoEx.RecordForToolAction(_transform);
            gameObject.SetHierarchyWorldScaleByPivotPoint(scale, pivotPoint);

            UndoEx.RecordForToolAction(_sourcePrefab.ActivationSettings);
            _sourcePrefab.ActivationSettings.WorldScale = _transform.lossyScale;
        }

        public void SetHierarchyWorldScaleByPivotPoint(Vector3 scale, Vector3 pivotPoint)
        {
            if (IsAnyMouseSessionActive) return;

            UndoEx.RecordForToolAction(_transform);
            gameObject.SetHierarchyWorldScaleByPivotPoint(scale, pivotPoint);

            UndoEx.RecordForToolAction(_sourcePrefab.ActivationSettings);
            _sourcePrefab.ActivationSettings.WorldScale = _transform.lossyScale;
        }

        public void Snap()
        {
            float offsetFromSurface = SourcePrefab.OffsetFromGridSurface;
            if (ObjectSnapping.Get().SnapSurfaceType != SnapSurfaceType.GridCell) offsetFromSurface = SourcePrefab.OffsetFromObjectSurface;

            if (AllShortcutCombos.Instance.PlaceGuideBehindSurfacePlane.IsActive()) offsetFromSurface *= -1.0f;

            if (AllShortcutCombos.Instance.SnapCenterToCenter.IsActive()) ObjectSnapping.Get().SnapObjectHierarchyToCenterOfSnapSurface(gameObject, ObjectPlacement.Get().CenterProjectedGuidePivotPoint, ObjectPlacement.Get().ProjectedGuidePivotPoints, offsetFromSurface);
            else ObjectSnapping.Get().SnapObjectHierarchy(gameObject, ObjectPlacement.Get().ProjectedGuidePivotPoints, offsetFromSurface);

            PersistentObjectPlacementGuideData.Get().LastUsedWorldPosition = _transform.position;
        }

        public void SetWorldPosition(Vector3 position)
        {
            _transform.position = position;
            PersistentObjectPlacementGuideData.Get().LastUsedWorldPosition = _transform.position;
        }

        public void SetWorldScale(Vector3 worldScale)
        {
            UndoEx.RecordForToolAction(_transform);
            _transform.SetWorldScale(worldScale);
            UndoEx.RecordForToolAction(_sourcePrefab.ActivationSettings);
            _sourcePrefab.ActivationSettings.WorldScale = worldScale;
        }

        public void RegisterCurrentPosition()
        {
            PersistentObjectPlacementGuideData.Get().LastUsedWorldPosition = _transform.position;
        }

        public void SetWorldRotation(Quaternion rotation)
        {
            _transform.rotation = rotation;
            //_sourcePrefab.ActivationSettings.WorldRotation = _transform.rotation;
        }

        public void RotateUsingKeyboardSettings(Vector3 customRotationAxis)
        {
            if (IsAnyMouseSessionActive) return;
            UndoEx.RecordForToolAction(_transform);

            ObjectKeyboardRotationSettings keyboardRotationSettings = ObjectPlacementGuideSettings.Get().KeyboardRotationSettings;
            gameObject.RotateHierarchyBoxAroundPoint(keyboardRotationSettings.CustomAxisRotationSettings.RotationAmountInDegrees,
                                                     customRotationAxis, gameObject.GetHierarchyWorldOrientedBox().Center);

            UndoEx.RecordForToolAction(_sourcePrefab.ActivationSettings);
            _sourcePrefab.ActivationSettings.WorldRotation = _transform.rotation;
        }

        public void RotateUsingKeyboardSettings(TransformAxis rotationAxis)
        {
            if (IsAnyMouseSessionActive) return;

            ObjectKeyboardRotationSettings keyboardRotationSettings = ObjectPlacementGuideSettings.Get().KeyboardRotationSettings;
            UndoEx.RecordForToolAction(_transform);

            AxisKeyboardRotationSettings axisKeyboardRotationSettings = keyboardRotationSettings.XAxisRotationSettings;
            if (rotationAxis == TransformAxis.Y) axisKeyboardRotationSettings = keyboardRotationSettings.YAxisRotationSettings;
            else if (rotationAxis == TransformAxis.Z) axisKeyboardRotationSettings = keyboardRotationSettings.XAxisRotationSettings;

            gameObject.RotateHierarchyBoxAroundPoint(axisKeyboardRotationSettings.RotationAmountInDegrees,
                                                     TransformAxes.GetVector(rotationAxis, TransformSpace.Global, _transform),
                                                     gameObject.GetHierarchyWorldOrientedBox().Center);

            UndoEx.RecordForToolAction(_sourcePrefab.ActivationSettings);
            _sourcePrefab.ActivationSettings.WorldRotation = _transform.rotation;
        }
        #endregion

        #region Private Methods
        private void Awake()
        {
            if (!IsGuideInstanceAllowedToExist()) { DestroyImmediate(this); return; }
            Initialize();
        }

        private void OnEnable()
        {
            if (!IsGuideInstanceAllowedToExist()) { DestroyImmediate(this); return; }
            Initialize();
        }

        private void Initialize()
        {
            MessageListenerRegistration.PerformRegistrationForObjectPlacementGuide(this);
            _instance = this;
            _transform = transform;

            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        private bool IsGuideInstanceAllowedToExist()
        {
            if (_instance != null && _instance != this) return false;
            return true;
        }

        private void Start()
        {
            // Note: We will always ensure that the guide is placed in the last position which it had before
            //       it was destroyed. Although not strictly necessary, it is more pleasent than to see it
            //       suddenly snap to position <0, 0, 0> every time an Undo/Redo operation is performed or
            //       when the scripts are recompiled. 
            // Note: This has to be done from the 'Start' function because here we can be sure that the object
            //       which holds the persistent guide data has been created.
            gameObject.transform.position = PersistentObjectPlacementGuideData.Get().LastUsedWorldPosition;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                MessageListenerDatabase.Instance.UnregisterListener(this);
                _instance = null;

                EditorApplication.update -= OnEditorUpdate;
            }
        }

        private void OnEditorUpdate()
        {
            if (!EditorApplication.isPlaying)
            {
                if (Octave3DWorldBuilder.ActiveInstance == null) UndoEx.DestroyObjectImmediate(gameObject);
                else
                if (!Selection.Contains(Octave3DWorldBuilder.ActiveInstance.gameObject)) ObjectPlacement.Get().DestroyPlacementGuide();
            }
        }
        #endregion

        #region Message Handlers
        public void RespondToMessage(Message message)
        {
            switch(message.Type)
            {
                case MessageType.NewPrefabWasActivated:

                    RespondToMessage(message as NewPrefabWasActivatedMessage);
                    break;

                case MessageType.NewPrefabCategoryWasActivated:

                    RespondToMessage(message as NewPrefabCategoryWasActivatedMessage);
                    break;

                case MessageType.PrefabWasRemovedFromCategory:

                    RespondToMessage(message as PrefabWasRemovedFromCategoryMessage);
                    break;

                case MessageType.PrefabCategoryWasRemovedFromDatabase:

                    RespondToMessage(message as PrefabCategoryWasRemovedFromDatabaseMessage);
                    break;

                case MessageType.UndoRedoWasPerformed:

                    RespondToMessage(message as UndoRedoWasPerformedMessage);
                    break;
            }
        }

        private void RespondToMessage(NewPrefabWasActivatedMessage message)
        {
            if (Inspector.Get().ActiveInspectorGUIIdentifier == InspectorGUIIdentifier.ObjectSnapping ||
                Inspector.Get().ActiveInspectorGUIIdentifier == InspectorGUIIdentifier.ObjectPlacement)
            {
                DestroyIfExists();
                CreateFromActivePrefabIfNotExists();
            }
        }

        private void RespondToMessage(NewPrefabCategoryWasActivatedMessage message)
        {
            if(Inspector.Get().ActiveInspectorGUIIdentifier == InspectorGUIIdentifier.ObjectSnapping || 
               Inspector.Get().ActiveInspectorGUIIdentifier == InspectorGUIIdentifier.ObjectPlacement)
            {
                DestroyIfExists();
                CreateFromActivePrefabIfNotExists();
            }
        }

        private void RespondToMessage(PrefabWasRemovedFromCategoryMessage message)
        {
            DestroyIfExists();
            CreateFromActivePrefabIfNotExists();
        }

        private void RespondToMessage(PrefabCategoryWasRemovedFromDatabaseMessage message)
        {
            DestroyIfExists();
            CreateFromActivePrefabIfNotExists();
        }

        private void RespondToMessage(UndoRedoWasPerformedMessage message)
        {
            if(_sourcePrefab != PrefabCategoryDatabase.Get().ActivePrefabCategory.ActivePrefab)
            {
                DestroyIfExists();
                CreateFromActivePrefabIfNotExists();
            }
        }
        #endregion

        #region Private Static Functions
        private static void InstantiateFromActivePrefabIfPossible()
        {
            Prefab activePrefab = PrefabQueries.GetActivePrefab();
            if (activePrefab != null) InstantiateFromPrefab(activePrefab);
        }

        private static void InstantiateFromPrefab(Prefab prefab)
        {
            _instance = ObjectInstantiation.InstantiateObjectPlacementGuide(prefab, prefab.Name + "(" + _nameSuffix + ")");
            _instance._sourcePrefab = prefab;
            _instance.transform.rotation = prefab.ActivationSettings.WorldRotation;
            _instance.gameObject.SetWorldScale(prefab.ActivationSettings.WorldScale);

            ObjectPlacementGuideWasInstantiatedMessage.SendToInterestedListeners();
        }
        #endregion
    }
}
#endif