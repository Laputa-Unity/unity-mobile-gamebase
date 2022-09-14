#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionShape
    {
        #region Private Variables
        private bool _isVisibleForStandardMode = false;

        [SerializeField]
        private RectangleObjectInteractionShape _rectangleShape = new RectangleObjectInteractionShape();
        [SerializeField]
        private EllipseObjectInteractionShape _ellipseShape = new EllipseObjectInteractionShape();
        #endregion

        #region Public Properties
        public RectangleShapeRenderSettings RectangleShapeRenderSettings { get { return _rectangleShape.RenderSettings; } }
        public EllipseShapeRenderSettings EllipseShapeRenderSettings { get { return _ellipseShape.RenderSettings; } }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            if (IsVisible())
            {
                OnBeforeRender();
                GetShape().RenderGizmos();
            }
        }

        public List<GameObject> GetOverlappedGameObjects()
        {
            if(ObjectSelectionSettings.Get().SelectionMode == ObjectSelectionMode.Standard)
            {
                // Note: We will only select objects if at least one of the enclosing rectangle dimensions
                //       is >= 'minRectSize'. This helps avoid situations in which the user wants to click on
                //       a game object, but they accodentally drag the mouse a little bit which causes unwanted
                //       objects to be selected.
                const int minRectSize = 15;
                ObjectInteraction2DShape shape = GetShape();
                Rect enclosingRectangle = shape.EnclosingRect;
                if (Mathf.Abs(enclosingRectangle.size.x) >= minRectSize || 
                    Mathf.Abs(enclosingRectangle.size.y) >= minRectSize) return shape.GetOverlappedGameObjects(ObjectSelectionSettings.Get().AllowPartialOverlap);
                else return new List<GameObject>();
            }
            else return GetShape().GetOverlappedGameObjects(ObjectSelectionSettings.Get().AllowPartialOverlap);
        }

        public bool IsVisible()
        {
            if (!MouseCursor.Instance.IsInsideSceneView()) return false;
            if (ObjectSelectionSettings.Get().SelectionMode == ObjectSelectionMode.Standard) return _isVisibleForStandardMode;

            return true;
        }

        public void HandleMouseButtonDownEvent(Event e)
        {
            if(e.InvolvesLeftMouseButton())
            {
                e.DisableInSceneView();

                _isVisibleForStandardMode = true;
                if (ObjectSelectionSettings.Get().SelectionMode == ObjectSelectionMode.Standard) GetShape().SetEnclosingRectMinMaxPoints(e.InvMousePos(SceneViewCamera.Camera));
            }
        }

        public void HandleMouseButtonUpEvent(Event e)
        {
            if(e.InvolvesLeftMouseButton())
            {
                _isVisibleForStandardMode = false;
                if (ObjectSelectionSettings.Get().SelectionMode == ObjectSelectionMode.Standard) SceneView.RepaintAll();
            }
        }

        public void HandleMouseDragEvent(Event e)
        {
            if(e.InvolvesLeftMouseButton())
            {
                if (ObjectSelectionSettings.Get().SelectionMode == ObjectSelectionMode.Standard) AdjustStandardShapeSizeForMouseDragEvent(e);
                if (!MouseCursor.Instance.IsInsideSceneView()) _isVisibleForStandardMode = false;
            }
        }

        public void HandleMouseMoveEvent(Event e)
        {
            SceneView.RepaintAll();
        }
        #endregion

        #region Private Methods
        private ObjectInteraction2DShape GetShape()
        {
            ObjectSelectionSettings selectionSettings = ObjectSelectionSettings.Get();
            if (selectionSettings.SelectionShapeType == ObjectSelectionShapeType.Ellipse) return _ellipseShape;
            else if (selectionSettings.SelectionShapeType == ObjectSelectionShapeType.Rectangle) return _rectangleShape;

            return null;
        }

        private void OnBeforeRender()
        {
            ObjectSelectionSettings selectionSettings = ObjectSelectionSettings.Get();

            if(selectionSettings.SelectionMode == ObjectSelectionMode.Paint)
            {
                ObjectSelectionPaintModeSettings paintModeSettings = selectionSettings.PaintModeSettings;
                SetWidthHeight(paintModeSettings.SelectionShapeWidthInPixels, paintModeSettings.SelectionShapeHeightInPixels);

                GetShape().EnclosingRectCenter = Event.current.InvMousePos(SceneViewCamera.Camera);
                SceneView.RepaintAll();
            }
        }

        private void AdjustStandardShapeSizeForMouseDragEvent(Event e)
        {
            ObjectInteraction2DShape shape = GetShape();
            shape.SetEnclosingRectMaxPoint(e.InvMousePos(SceneViewCamera.Camera));

            SceneView.RepaintAll();
        }

        private void SetWidthHeight(float width, float height)
        {
            _rectangleShape.EnclosingRectWidth = width;
            _rectangleShape.EnclosingRectHeight = height;

            _ellipseShape.EnclosingRectWidth = width;
            _ellipseShape.EnclosingRectHeight = height;
        }
        #endregion
    }
}
#endif