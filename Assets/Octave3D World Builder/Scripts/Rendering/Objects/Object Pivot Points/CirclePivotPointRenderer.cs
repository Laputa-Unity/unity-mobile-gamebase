#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class CirclePivotPointRenderer : IPivotPointRenderer
    {
        #region Public Methods
        public void Render(Vector3 pivotPoint, Color fillColor, Color borderLineColor, float sizeInPixels)
        {
            Camera camera = SceneViewCamera.Camera;
            Vector2 screenPoint = Vector3Extensions.WorldToScreenPoint(pivotPoint);

            Circle2D circle = new Circle2D(screenPoint, sizeInPixels * 0.5f);
            GizmosEx.Render2DFilledCircle(circle, fillColor);
            GizmosEx.Render2DCircleBorderLines(circle, borderLineColor);
        }
        #endregion
    }
}
#endif