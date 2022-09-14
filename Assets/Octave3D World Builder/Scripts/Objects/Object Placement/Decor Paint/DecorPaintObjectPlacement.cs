#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacement : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private SingleModeDecorPaintStroke _singleModeDecorPaintStroke = new SingleModeDecorPaintStroke();
        [SerializeField]
        private BrushModeDecorPaintStroke _brushModeDecorPaintStroke = new BrushModeDecorPaintStroke();

        [SerializeField]
        private ObjectMask _decorPaintMask = new ObjectMask();

        [SerializeField]
        private bool _wasInitialized = false;
        #endregion

        #region Public Properties
        public XZOrientedEllipseShapeRenderSettings BrushCircleRenderSettings { get { return _brushModeDecorPaintStroke.BrushCircleRenderSettings; } }
        public Plane DecorPaintSurfacePlane { get { return GetPaintStroke().StrokeSurfacePlane; } }
        public GameObject DecorPaintSurfaceObject { get { return GetPaintStroke().StrokeSurfaceObject; } }
        public ObjectMask DecorPaintMask { get { return _decorPaintMask; } }
        public DecorPaintMode DecorPaintMode { get { return DecorPaintObjectPlacementSettings.Get().DecorPaintMode; } }
        public bool IsStroking { get { return GetPaintStroke().IsBeingPerformed; } }
        #endregion

        #region Public Static Functions
        public static DecorPaintObjectPlacement Get()
        {
            return ObjectPlacement.Get().DecorPaintObjectPlacement;
        }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            GetPaintStroke().RenderGizmos();
        }

        public void HandleMouseMoveEvent(Event e)
        {
            GetPaintStroke().HandleMouseMoveEvent(e);
        }

        public void HandleMouseButtonDownEvent(Event e)
        {
            GetPaintStroke().HandleMousButtonDownEvent(e);
        }

        public void HandleMouseButtonUpEvent(Event e)
        {
            GetPaintStroke().HandleMouseButtonUpEvent(e);
        }

        public void HandleMouseDragEvent(Event e)
        {
            GetPaintStroke().HandleMouseDragEvent(e);
        }

        public void HandleMouseScrollWheelEvent(Event e)
        {
            if (DecorPaintMode == DecorPaintMode.Brush) HandleMouseScrollWheelEventForBrushMode(e);
        }

        public void UpdateGuidePivotPoints()
        {
            if (DecorPaintMode == DecorPaintMode.Single) _singleModeDecorPaintStroke.UpdateGuidePivotPoints();
        }

        public Vector3 CalculatePlacementGuidePosition()
        {
            if (DecorPaintMode == DecorPaintMode.Single) return _singleModeDecorPaintStroke.CalculatePlacementGuidePosition();
            return Vector3.zero;
        }
        #endregion

        #region Private Methods
        private DecorPaintStroke GetPaintStroke()
        {
            if (DecorPaintMode == DecorPaintMode.Brush) return _brushModeDecorPaintStroke;
            else if (DecorPaintMode == DecorPaintMode.Single) return _singleModeDecorPaintStroke;

            return null;
        }

        private void OnEnable()
        {
            if (!_wasInitialized)
            {
                InitializeBrushDecorPaintStroke();
                ObjectPlacementSettings.Get().DecorPaintObjectPlacementSettings.SingleDecorPaintModeSettings.PlacementGuideSurfaceAlignmentSettings.IsEnabled = true;

                _wasInitialized = true;
            }
        }

        private void InitializeBrushDecorPaintStroke()
        {
            _brushModeDecorPaintStroke.BrushCircleRenderSettings.BorderLineColor = Color.green;
        }

        private void HandleMouseScrollWheelEventForBrushMode(Event e)
        {
            if(AllShortcutCombos.Instance.EnableScrollWheelSizeAdjustmentForDecorPaintBrush.IsActive())
            {
                e.DisableInSceneView();
                AdjustBrushShapeSizeForMouseWheelScroll(e);
            }
        }

        private void AdjustBrushShapeSizeForMouseWheelScroll(Event e)
        {
            DecorPaintObjectPlacementBrush activeBrush = DecorPaintObjectPlacementBrushDatabase.Get().ActiveBrush;
            if (activeBrush != null)
            {
                BrushDecorPaintModeObjectPlacementSettings brushDecorPaintSettings = ObjectPlacementSettings.Get().DecorPaintObjectPlacementSettings.BrushDecorPaintModeSettings;
                int sizeAdjustAmount = (int)(-e.delta.y * brushDecorPaintSettings.ScrollWheelCircleRadiusAdjustmentSpeed);

                UndoEx.RecordForToolAction(activeBrush);
                activeBrush.Radius += sizeAdjustAmount;

                Octave3DWorldBuilder.ActiveInstance.Inspector.EditorWindow.Repaint();
                SceneView.RepaintAll();
            }
        }
        #endregion
    }
}
#endif