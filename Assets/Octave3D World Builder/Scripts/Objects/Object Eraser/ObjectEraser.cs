#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectEraser : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectEraserSettings _settings;

        [SerializeField]
        private ObjectMask _eraseMask = new ObjectMask();

        [SerializeField]
        private Object2DMassEraseShape _mass2DEraseShape = new Object2DMassEraseShape();
        [SerializeField]
        private Object3DMassEraseShape _mass3DEraseShape = new Object3DMassEraseShape();
        private DateTime _lastEraseOperationTime = System.DateTime.Now;

        [SerializeField]
        private bool _wasInitialized = false;
        #endregion

        #region Public Properties
        public ObjectMask EraseMask { get { return _eraseMask; } }
        public ObjectEraserSettings Settings
        {
            get
            {
                if (_settings == null) _settings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectEraserSettings>();
                return _settings;
            }
        }
        public EllipseShapeRenderSettings Circle2DMassEraseShapeRenderSettings { get { return _mass2DEraseShape.CircleShapeRenderSettings; } }
        public XZOrientedEllipseShapeRenderSettings Circle3DMassEraseShapeRenderSettings { get { return _mass3DEraseShape.CircleShapeRenderSettings; } }
        #endregion

        #region Public Static Functions
        public static ObjectEraser Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ObjectEraser;
        }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            if (Settings.EraseMode == ObjectEraseMode.ObjectMass2D) _mass2DEraseShape.RenderGizmos();
            else if (Settings.EraseMode == ObjectEraseMode.ObjectMass3D) _mass3DEraseShape.RenderGizmos();
        }

        public void HandleMouseMoveEvent(Event e)
        {
            if (Settings.EraseMode == ObjectEraseMode.ObjectMass2D)
            {
                e.DisableInSceneView();
                _mass2DEraseShape.HandleMouseMoveEvent(e);
            }
            else if (Settings.EraseMode == ObjectEraseMode.ObjectMass3D)
            {
                e.DisableInSceneView();
                _mass3DEraseShape.HandleMouseMoveEvent(e);
            }
        }

        public void HandleMouseDragEvent(Event e)
        {
            if (!e.InvolvesLeftMouseButton()) return;

            if (Settings.EraseMode == ObjectEraseMode.ObjectMass2D) _mass2DEraseShape.HandleMouseDragEvent(e);
            else if (Settings.EraseMode == ObjectEraseMode.ObjectMass3D) _mass3DEraseShape.HandleMouseDragEvent(e);

            AnalyzeEventAndPerformEraseOperationIfNecessary(e);
            e.DisableInSceneView();
        }

        public void HandleMouseButtonDownEvent(Event e)
        {
            AnalyzeEventAndPerformEraseOperationIfNecessary(e);
        }

        public void HandleMouseButtonUpEvent(Event e)
        {
        }

        public void HandleMouseScrollWheelEvent(Event e)
        {
            if (CanAdjustMassEraseShapeSizeForMouseScrollWheel())
            {
                e.DisableInSceneView();
                AdjustMassEraseShapeSizeForMouseWheelScroll(e);
            }
        }

        public List<GameObject> GetGameObjectsForMassEraseOperation()
        {
            if (Settings.EraseMode == ObjectEraseMode.ObjectMass2D) return FilterObjectsWhichCanBeErased(_mass2DEraseShape.GetOverlappedGameObjectsForEraseOperation());
            else if (Settings.EraseMode == ObjectEraseMode.ObjectMass3D) return FilterObjectsWhichCanBeErased(_mass3DEraseShape.GetOverlappedGameObjectsForEraseOperation());
            else return new List<GameObject>();
        }

        public List<GameObject> FilterObjectsWhichCanBeErased(List<GameObject> gameObjectsToErase)
        {
            if (gameObjectsToErase.Count == 0) return new List<GameObject>();

            // If we don't need to erase only mesh objects, no filtering is required, so we will just return the original list
            if (!Settings.EraseOnlyMeshObjects) return gameObjectsToErase;

            // Ensure we only erase mesh objects
            var gameObjectsWhichCanBeErased = new List<GameObject>(gameObjectsToErase.Count);
            foreach(GameObject gameObject in gameObjectsToErase)
            {
                if (!gameObject.HasMesh() && (gameObject.HasLight() || gameObject.HasParticleSystem())) continue;
                gameObjectsWhichCanBeErased.Add(gameObject);
            }

            return gameObjectsWhichCanBeErased;
        }

        public MouseCursorRayHit GetMouseCursorRayHit()
        {
            MouseCursor.Instance.PushObjectPickMaskFlags(MouseCursorObjectPickFlags.ObjectTerrain);
            MouseCursorRayHit cursorRayHit = MouseCursor.Instance.GetRayHit();
            MouseCursor.Instance.PopObjectPickMaskFlags();

            return cursorRayHit;
        }
        #endregion

        #region Private Methods
        private bool EnoughTimeHasPassedForNewEraseOperation(DateTime currentTime)
        {
            return ((currentTime - _lastEraseOperationTime).TotalSeconds >= _settings.EraseDelayInSeconds);
        }

        private void AdjustMassEraseShapeSizeForMouseWheelScroll(Event e)
        {
            if (Settings.EraseMode == ObjectEraseMode.ObjectMass2D) AdjustMass2DEraseShapeSizeForMouseWheelScroll(e);
            else
            if (Settings.EraseMode == ObjectEraseMode.ObjectMass3D) AdjustMass3DEraseShapeSizeForMouseWheelScroll(e);
        }

        private void AdjustMass2DEraseShapeSizeForMouseWheelScroll(Event e)
        {
            Object2DMassEraseSettings mass2DEraseSettings = _settings.Mass2DEraseSettings;
            int sizeAdjustAmount = (int)(-e.delta.y * mass2DEraseSettings.ScrollWheelCircleRadiusAdjustmentSpeed);

            UndoEx.RecordForToolAction(mass2DEraseSettings);
            mass2DEraseSettings.CircleShapeRadius += sizeAdjustAmount;

            SceneView.RepaintAll();
            Octave3DWorldBuilder.ActiveInstance.Inspector.EditorWindow.Repaint();
        }

        private void AdjustMass3DEraseShapeSizeForMouseWheelScroll(Event e)
        {
            Object3DMassEraseSettings mass3DEraseSettings = _settings.Mass3DEraseSettings;
            int sizeAdjustAmount = (int)(-e.delta.y * mass3DEraseSettings.ScrollWheelCircleRadiusAdjustmentSpeed);

            UndoEx.RecordForToolAction(mass3DEraseSettings);
            mass3DEraseSettings.CircleShapeRadius += sizeAdjustAmount;

            SceneView.RepaintAll();
            Octave3DWorldBuilder.ActiveInstance.Inspector.EditorWindow.Repaint();
        }

        private void AnalyzeEventAndPerformEraseOperationIfNecessary(Event e)
        {
            DateTime currentTime = DateTime.Now;
            if (e.InvolvesLeftMouseButton() && EnoughTimeHasPassedForNewEraseOperation(currentTime))
            {
                e.DisableInSceneView();
                PerformEraseOperation(currentTime);
            }
        }

        private void PerformEraseOperation(DateTime currentTime)
        {
            PerformNecessaryUndoRecordingsBeforeEraseOperation();
            ObjectEraseOperationFactory.Create(_settings.EraseMode).Perform();
            _lastEraseOperationTime = currentTime;
        }

        private void PerformNecessaryUndoRecordingsBeforeEraseOperation()
        {
            // Note: We have to increment the current group, because otherwise the previous records will be overridden
            //       by the new one and we would not be able to properly restore the states on Undo. For example, this
            //       can result in loosing the object selection information.
            UndoEx.IncrementCurrentGroup();
            UndoEx.RecordForToolAction(ObjectSelection.Get());
        }

        private bool CanAdjustMassEraseShapeSizeForMouseScrollWheel()
        {
            return IsAnyMassEraseShapeVisible() && AllShortcutCombos.Instance.EnableScrollWheelSizeAdjustmentForMassEraseShape.IsActive();
        }

        private bool IsAnyMassEraseShapeVisible()
        {
            return _mass2DEraseShape.IsVisible() || _mass3DEraseShape.IsVisible();
        }

        private void OnEnable()
        {
            if(!_wasInitialized)
            {
                InitializeMassEraseShapes();
                _wasInitialized = true;
            }
        }

        private void InitializeMassEraseShapes()
        {
            InitializeMass2DEraseShape();
            InitializeMass3DEraseShape();
        }

        private void InitializeMass2DEraseShape()
        {
            _mass2DEraseShape.CircleShapeRenderSettings.BorderLineColor = Color.red;
            _mass2DEraseShape.CircleShapeRenderSettings.FillColor = new Color(1.0f, 0.0f, 0.0f, 0.4f);
        }

        private void InitializeMass3DEraseShape()
        {
            _mass3DEraseShape.CircleShapeRenderSettings.BorderLineColor = Color.red;
        }
        #endregion
    }
}
#endif