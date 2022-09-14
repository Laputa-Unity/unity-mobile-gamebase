#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPath : IMessageListener
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementPathSettings _settings;
        [SerializeField]
        private ObjectPlacementPathRenderSettings _renderSettings;

        [SerializeField]
        private BoxFace _extensionPlaneBoxFace = BoxFace.Bottom;

        [SerializeField]
        private ObjectPlacementExtensionPlane _extensionPlane = new ObjectPlacementExtensionPlane();

        private List<ObjectPlacementBoxStackSegment> _segments = new List<ObjectPlacementBoxStackSegment>();
        private ObjectPlacementPathManualConstructionSession _manualConstructionSession = new ObjectPlacementPathManualConstructionSession();
        private GameObject _startObject;

        private List<ObjectPlacementPathTileConnectionGridCell> _tileConnectionGridCells = new List<ObjectPlacementPathTileConnectionGridCell>();
        private ObjectPlacementPathRenderer _renderer = new ObjectPlacementPathRenderer();

        private bool _usingTileConnections;
        #endregion

        #region Public Properties
        public ObjectPlacementPathSettings Settings
        {
            get
            {
                if (_settings == null) _settings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathSettings>();
                return _settings;
            }
        }
        public ObjectPlacementPathRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathRenderSettings>();
                return _renderSettings;
            }
        }
        public ObjectPlacementExtensionPlaneRenderSettings ExtensionPlaneRenderSettings { get { return _extensionPlane.RenderSettings; } }
        public bool IsEmpty { get { return NumberOfSegments == 0; } }
        public int NumberOfSegments { get { return _segments.Count; } }
        public bool IsUnderManualConstruction { get { return _manualConstructionSession.IsActive; } }
        public Vector3 ExtensionPlaneRightAxis { get { return _extensionPlane.RightAxis; } }
        public Vector3 ExtensionPlaneLookAxis { get { return _extensionPlane.LookAxis; } }
        public Plane ExtensionPlane { get { return _extensionPlane.Plane; } }
        public List<ObjectPlacementPathTileConnectionGridCell> TileConnectionGridCells { get { return new List<ObjectPlacementPathTileConnectionGridCell>(_tileConnectionGridCells); } }
        public GameObject StartObject { get { return _startObject; } }
        #endregion

        #region Constructors
        public ObjectPlacementPath()
        {
            MessageListenerRegistration.PerformRegistrationForObjectPlacementPath(this);
        }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            if (_startObject == null) CancelManualConstruction();
        
            if (CanRenderExtensionPlane())
            {
                RestrictExtensionPlane();
                _extensionPlane.FromOrientedBoxFace(_startObject.GetHierarchyWorldOrientedBox(), _extensionPlaneBoxFace);
                _extensionPlane.RenderGizmos();
            }

            if (CanRenderPath()) _renderer.RenderGizmos(this);
        }

        public void NextExtensionPlane()
        {
            if(Settings.TileConnectionSettings.UseTileConnections)
            {
                Debug.LogWarning("The extension plane can not be changed when using tile connections. For 3D tiles it will always reside at the bottom of the tiles (in local space) " + 
                                 "and for 2D sprites it will reside behind the sprite plane (in local space).");
            }
            if (CanActivateNextExtensionPlane())
            {
                _extensionPlaneBoxFace = BoxFaces.GetNext(_extensionPlaneBoxFace);
                SceneView.RepaintAll();
            }
        }

        public List<OrientedBox> GetAllOrientedBoxes()
        {
            var allOrientedBoxes = new List<OrientedBox>();
            foreach (var segment in _segments) allOrientedBoxes.AddRange(segment.GetAllOrientedBoxes());

            return allOrientedBoxes;
        }

        public List<ObjectPlacementBoxStackSegment> GetAllSegments()
        {
            return new List<ObjectPlacementBoxStackSegment>(_segments);
        }

        public ObjectPlacementBoxStackSegment GetSegmentByIndex(int segmentIndex)
        {
            return _segments[segmentIndex];
        }

        public void SetStartObject(GameObject startObject)
        {
            if (_manualConstructionSession.IsActive) return;
            _startObject = startObject;
        }

        public void BeginManualConstruction()
        {
            if (CanBeginManualConstruction())
            {
                _manualConstructionSession.SetData(GetManualConstructionSessionData());
                _manualConstructionSession.Begin();

                _usingTileConnections = Settings.TileConnectionSettings.UseTileConnections;
                RestrictExtensionPlane();

                SceneView.RepaintAll();
            }
        }

        public List<ObjectPlacementData> EndManualConstruction()
        {
            if (IsUnderManualConstruction)
            {
                List<ObjectPlacementData> objectPlacementDataInstances = _manualConstructionSession.End();
                ClearData();
                return objectPlacementDataInstances;
            }
            else
            {
                ClearData();
                return new List<ObjectPlacementData>();
            }
        }

        public void CancelManualConstruction()
        {
            _manualConstructionSession.Cancel();
            ClearData();
        }

        public void ManualRaise()
        {
            _manualConstructionSession.ManualRaisePath();
        }

        public void ManualLower()
        {
            _manualConstructionSession.ManualLowerPath();
        }

        public void UpdateForMouseMoveEvent()
        {
            if (_startObject == null) CancelManualConstruction();
            _manualConstructionSession.UpdateForMouseMoveEvent();
        }

        public void Attach2NewSegmentsIfUnderManualConstruction()
        {
            _manualConstructionSession.Attach2NewSegments();
        }

        public void RemoveLast2SegmentsIfUnderManualConstruction()
        {
            _manualConstructionSession.RemoveLast2Segments();
        }
        #endregion

        #region Private Methods
        private void RestrictExtensionPlane()
        {
            if(Settings.TileConnectionSettings.UseTileConnections)
            {
                if (Settings.TileConnectionSettings.UsesSprites()) _extensionPlaneBoxFace = BoxFace.Back;
                else _extensionPlaneBoxFace = BoxFace.Bottom;
            }
        }

        private ObjectPlacementPathManualConstructionSessionData GetManualConstructionSessionData()
        {
            var manualConstructionSessionData = new ObjectPlacementPathManualConstructionSessionData();
            manualConstructionSessionData.Path = this;
            manualConstructionSessionData.PathSegments = _segments;
            manualConstructionSessionData.TileConnectionGridCells = _tileConnectionGridCells;
            manualConstructionSessionData.PathExtensionPlane = _extensionPlane;
            manualConstructionSessionData.StartObject = _startObject;

            return manualConstructionSessionData;
        }

        private bool CanBeginManualConstruction()
        {
            return _startObject != null && !_manualConstructionSession.IsActive;
        }

        private bool CanRenderExtensionPlane()
        {
            return !IsUnderManualConstruction && _startObject != null;
        }

        private bool CanRenderPath()
        {
            return IsUnderManualConstruction && _startObject != null;
        }

        private bool CanActivateNextExtensionPlane()
        {
            return !IsUnderManualConstruction && !Settings.TileConnectionSettings.UseTileConnections;
        }

        private void ClearData()
        {
            _segments.Clear();
            _tileConnectionGridCells.Clear();
        }
        #endregion

        #region Message Handlers
        public void RespondToMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.ObjectPlacementPathExcludeCornersWasChanged:

                    RespondToMessage(message as ObjectPlacementPathExcludeCornersWasChangedMessage);
                    break;

                case MessageType.ObjectPlacementPathRotateObjectsToFollowPathWasChanged:

                    RespondToMessage(message as ObjectPlacementPathRotateObjectsToFollowPathWasChangedMessage);
                    break;

                case MessageType.ObjectPlacementPathPaddingSettingsWereChanged:

                    RespondToMessage(message as ObjectPlacementPathPaddingSettingsWereChangedMessage);
                    break;

                case MessageType.ObjectPlacementPathBorderSettingsWereChanged:

                    RespondToMessage(message as ObjectPlacementPathBorderSettingsWereChangedMessage);
                    break;

                case MessageType.ObjectPlacementPathHeightAdjustmentModeWasChanged:

                    RespondToMessage(message as ObjectPlacementPathHeightAdjustmentModeWasChangedMessage);
                    break;

                case MessageType.ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsWereChanged:

                    RespondToMessage(message as ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsWereChangedMessage);
                    break;

                case MessageType.ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChanged:

                    RespondToMessage(message as ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChangedMessage);
                    break;

                case MessageType.ObjectPlacementPathHeightPatternWasRemovedFromDatabase:

                    RespondToMessage(message as ObjectPlacementPathHeightPatternWasRemovedFromDatabaseMessage);
                    break;

                case MessageType.NewObjectPlacementPathHeightPatternWasActivated:

                    RespondToMessage(message as NewObjectPlacementPathHeightPatternWasActivatedMessage);
                    break;

                case MessageType.UndoRedoWasPerformed:

                    RespondToMessage(message as UndoRedoWasPerformedMessage);
                    break;
            }
        }

        private void RespondToMessage(ObjectPlacementPathExcludeCornersWasChangedMessage message)
        {
            if (message.PathManualConstructionSettings == Settings.ManualConstructionSettings)
                _manualConstructionSession.OnExcludeCornersSettingsChanged();
        }

        private void RespondToMessage(ObjectPlacementPathRotateObjectsToFollowPathWasChangedMessage message)
        {
            if (message.PathManualConstructionSettings == Settings.ManualConstructionSettings)
                _manualConstructionSession.OnRotateObjectsToFollowPathSettingsChanged();
        }

        private void RespondToMessage(ObjectPlacementPathPaddingSettingsWereChangedMessage message)
        {
            if (message.PaddingSettings == Settings.ManualConstructionSettings.PaddingSettings)
                _manualConstructionSession.OnPaddingSettingsChanged();
        }

        private void RespondToMessage(ObjectPlacementPathBorderSettingsWereChangedMessage message)
        {
            if (message.BorderSettings == Settings.ManualConstructionSettings.BorderSettings)
                _manualConstructionSession.OnBorderSettingsChanged();
        }

        private void RespondToMessage(ObjectPlacementPathHeightAdjustmentModeWasChangedMessage message)
        {
            if (message.HeightAdjustmentSettings == Settings.ManualConstructionSettings.HeightAdjustmentSettings)
                _manualConstructionSession.OnHeightAdjustmentModeChanged();
        }

        private void RespondToMessage(ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsWereChangedMessage message)
        {
            if (message.AutomaticRandomHeightAdjustmentSettings == Settings.ManualConstructionSettings.HeightAdjustmentSettings.AutomaticRandomHeightAdjustmentSettings)
                _manualConstructionSession.OnAutomaticRandomHeightAdjustmentSettingsChanged();
        }

        private void RespondToMessage(ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChangedMessage message)
        {
            if (message.AutomaticPatternHeightAdjustmentSettings == Settings.ManualConstructionSettings.HeightAdjustmentSettings.AutomaticPatternHeightAdjustmentSettings)
                _manualConstructionSession.OnAutomaticPatternHeightAdjustmentSettingsChanged();
        }

        private void RespondToMessage(ObjectPlacementPathHeightPatternWasRemovedFromDatabaseMessage message)
        {
            _manualConstructionSession.OnHeightPatternRemoved();
        }

        private void RespondToMessage(NewObjectPlacementPathHeightPatternWasActivatedMessage message)
        {
            _manualConstructionSession.OnNewHeightPatternWasActivated();
        }

        private void RespondToMessage(UndoRedoWasPerformedMessage message)
        {
            if (IsUnderManualConstruction &&
                _usingTileConnections != Settings.TileConnectionSettings.UseTileConnections)
            {
                Debug.LogWarning("The \'Use tile connections\' property was changed via Undo/Redo while the path was being constructed. This is not allowed. Construction was cancelled.");
                CancelManualConstruction();
                return;
            }

            _manualConstructionSession.OnExcludeCornersSettingsChanged();
            _manualConstructionSession.OnRotateObjectsToFollowPathSettingsChanged();
            _manualConstructionSession.OnPaddingSettingsChanged();
            _manualConstructionSession.OnBorderSettingsChanged();
            _manualConstructionSession.OnHeightAdjustmentModeChanged();
            _manualConstructionSession.OnAutomaticRandomHeightAdjustmentSettingsChanged();
            _manualConstructionSession.OnAutomaticPatternHeightAdjustmentSettingsChanged();
            _manualConstructionSession.OnHeightPatternRemoved();
            _manualConstructionSession.OnNewHeightPatternWasActivated();
        }
        #endregion
    }
}
#endif