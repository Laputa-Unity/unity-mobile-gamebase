#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class SquarePivotPointRenderer : IPivotPointRenderer
    {
        #region Public Methods
        public void Render(Vector3 pivotPoint, Color fillColor, Color borderLineColor, float sizeInPixels)
        {
            Camera camera = SceneViewCamera.Camera;
            Vector2 screenPoint = Vector3Extensions.WorldToScreenPoint(pivotPoint);

            Square2D square = new Square2D(screenPoint, sizeInPixels);
            GizmosEx.Render2DFilledSquare(square, fillColor);
            GizmosEx.Render2DSquareBorderLines(square, borderLineColor);
        }
        #endregion
    }
}
#endif