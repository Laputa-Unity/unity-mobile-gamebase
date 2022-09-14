#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlock : IMessageListener
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementBlockSettings _settings;

        [SerializeField]
        private ObjectPlacementBlockRenderSettings _renderSettings;
        private ObjectPlacementBlockRenderer _renderer = new ObjectPlacementBlockRenderer();

        [SerializeField]
        private ObjectPlacementExtensionPlane _extensionPlane = new ObjectPlacementExtensionPlane();
        [SerializeField]
        private BoxFace _extensionPlaneBoxFace = BoxFace.Bottom;

        private GameObject _startObject;
        private ObjectPlacementBlockManualConstructionSession _manualConstructionSession = new ObjectPlacementBlockManualConstructionSession();
        private List<ObjectPlacementBoxStackSegment> _segments = new List<ObjectPlacementBoxStackSegment>();
        #endregion

        #region Public Properties
        public int NumberOfSegments { get { return _segments.Count; } }
        public ObjectPlacementBlockSettings Settings
        {
            get
            {
                if (_settings == null) _settings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementBlockSettings>();
                return _settings;
            }
        }
        public bool IsUnderManualConstruction { get { return _manualConstructionSession.IsActive; } }
        public ObjectPlacementBlockRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementBlockRenderSettings>();
                return _renderSettings;
            }
        }
        public ObjectPlacementExtensionPlaneRenderSettings ExtensionPlaneRenderSettings { get { return _extensionPlane.RenderSettings; } }
        public Plane ExtensionPlane { get { return _extensionPlane.Plane; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlock()
        {
            MessageListenerRegistration.PerformRegistrationForObjectPlacementBlock(this);
        }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            if (_startObject == null) CancelManualConstruction();

            if (CanRenderExtensionPlane())
            {
                _extensionPlane.FromOrientedBoxFace(_startObject.GetHierarchyWorldOrientedBox(), _extensionPlaneBoxFace);
                _extensionPlane.RenderGizmos();
            }

            if (CanRenderBlock()) _renderer.RenderGizmos(this);
        }

        public void RenderHandles()
        {
            if (IsUnderManualConstruction && NumberOfSegments > 0 && _segments[0].NumberOfStacks > 0 && _segments[0].GetStackByIndex(0).NumberOfBoxes > 0)
            {
                int numberOfObjectsAlongRight = _segments[0].NumberOfStacks;
                int numberOfObjectsAlongLook = NumberOfSegments;
                int numberOfObjectsAlongUp = _segments[0].GetStackByIndex(0).NumberOfBoxes;

                Vector3 boxSize = _segments[0].GetStackByIndex(0).BoxSize;
                Vector3 labelPosition = _segments[0].GetStackByIndex(0).GetBoxByIndex(0).Center;

                int numberOfStacks = _segments[0].NumberOfStacks;
                if (numberOfStacks == 1)
                {
                    labelPosition -= _extensionPlane.RightAxis * boxSize.x;
                }
                else
                {
                    Vector3 fromSecondToFirstStack= _segments[0].GetStackByIndex(0).BasePosition - _segments[0].GetStackByIndex(1).BasePosition;
                    fromSecondToFirstStack.Normalize();
                    labelPosition += fromSecondToFirstStack * boxSize.x;
                }

                if(NumberOfSegments == 1)
                {
                    labelPosition -= _extensionPlane.LookAxis * boxSize.z;
                }
                else
                {
                    Vector3 fromSecondToFirstSegment = _segments[0].GetStackByIndex(0).BasePosition - _segments[1].GetStackByIndex(0).BasePosition;
                    fromSecondToFirstSegment.Normalize();
                    labelPosition += fromSecondToFirstSegment * boxSize.z;
                }

                LabelRenderSettings labelRenderSettings = RenderSettings.ManualConstructionRenderSettings.DimensionsLabelRenderSettings;

                var labelStyle = new GUIStyle("label");
                labelStyle.normal.textColor = labelRenderSettings.TextColor;
                if (labelRenderSettings.Bold) labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.fontSize = labelRenderSettings.FontSize;
                string labelText = "Block dimensions: " + "(" + numberOfObjectsAlongRight + ", ";
                if (Settings.ManualConstructionSettings.HeightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementBlockHeightAdjustmentMode.AutomaticRandom) labelText += "#, ";
                else labelText += numberOfObjectsAlongUp + ", ";
                labelText += numberOfObjectsAlongLook + ")";

                Handles.BeginGUI();
                GUI.Label(new Rect(0.0f, SceneViewCamera.Camera.pixelHeight - 20.0f, 300.0f, 20.0f), labelText, labelStyle);
                Handles.EndGUI();
            }
        }

        public void NextExtensionPlane()
        {
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
            _manualConstructionSession.ManualRaiseBlock();
        }

        public void ManualLower()
        {
            _manualConstructionSession.ManualLowerBlock();
        }

        public void UpdateForMouseMoveEvent()
        {
            if (_startObject == null) CancelManualConstruction();
            _manualConstructionSession.UpdateForMouseMoveEvent();
        }
        #endregion

        #region Private Methods
        private ObjectPlacementBlockManualConstructionSessionData GetManualConstructionSessionData()
        {
            var manualConstructionSessionData = new ObjectPlacementBlockManualConstructionSessionData();
            manualConstructionSessionData.Block = this;
            manualConstructionSessionData.BlockSegments = _segments;
            manualConstructionSessionData.BlockExtensionPlane = _extensionPlane;
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

        private bool CanRenderBlock()
        {
            return IsUnderManualConstruction && _startObject != null;
        }

        private bool CanActivateNextExtensionPlane()
        {
            return !IsUnderManualConstruction;
        }

        private void ClearData()
        {
            _segments.Clear();
        }
        #endregion

        #region Message Handlers
        public void RespondToMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.ObjectPlacementBlockExcludeCornersWasChanged:

                    RespondToMessage(message as ObjectPlacementBlockExcludeCornersWasChangedMessage);
                    break;

                case MessageType.ObjectPlacementBlockPaddingSettingsWereChanged:

                    RespondToMessage(message as ObjectPlacementBlockPaddingSettingsWereChangedMessage);
                    break;

                case MessageType.ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChanged:

                    RespondToMessage(message as ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChangedMessage);
                    break;

                case MessageType.ObjectPlacementBlockHeightAdjustmentModeWasChanged:

                    RespondToMessage(message as ObjectPlacementBlockHeightAdjustmentModeWasChangedMessage);
                    break;

                case MessageType.ObjectPlacementBlockSubdivisionSettingsWereChanged:

                    RespondToMessage(message as ObjectPlacementBlockSubdivisionSettingsWereChangedMessage);
                    break;

                case MessageType.UndoRedoWasPerformed:

                    RespondToMessage(message as UndoRedoWasPerformedMessage);
                    break;
            }
        }

        private void RespondToMessage(ObjectPlacementBlockExcludeCornersWasChangedMessage message)
        {
            if (message.BlockManualConstructionSettings == Settings.ManualConstructionSettings)
                _manualConstructionSession.OnExcludeCornersSettingsChanged();
        }

        private void RespondToMessage(ObjectPlacementBlockPaddingSettingsWereChangedMessage message)
        {
            if (message.PaddingSettings == Settings.ManualConstructionSettings.PaddingSettings)
                _manualConstructionSession.OnPaddingSettingsChanged();
        }

        private void RespondToMessage(ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChangedMessage message)
        {
            if (message.AutomaticRandomHeightAdjustmentSettings == Settings.ManualConstructionSettings.HeightAdjustmentSettings.AutomaticRandomHeightAdjustmentSettings)
                _manualConstructionSession.OnAutomaticRandomHeightAdjustmentSettingsChanged();
        }

        private void RespondToMessage(ObjectPlacementBlockHeightAdjustmentModeWasChangedMessage message)
        {
            if (message.HeightAdjustmentSettings == Settings.ManualConstructionSettings.HeightAdjustmentSettings)
                _manualConstructionSession.OnHeightAdjustmentModeChanged();
        }

        private void RespondToMessage(ObjectPlacementBlockSubdivisionSettingsWereChangedMessage message)
        {
            if (message.SubdivisionSettings == Settings.ManualConstructionSettings.SubdivisionSettings)
                _manualConstructionSession.OnSubdivisionSettingsChanged();
        }

        private void RespondToMessage(UndoRedoWasPerformedMessage message)
        {
            _manualConstructionSession.OnExcludeCornersSettingsChanged();
            _manualConstructionSession.OnPaddingSettingsChanged();
            _manualConstructionSession.OnAutomaticRandomHeightAdjustmentSettingsChanged();
            _manualConstructionSession.OnHeightAdjustmentModeChanged();
            _manualConstructionSession.OnSubdivisionSettingsChanged();
        }
        #endregion
    }
}
#endif