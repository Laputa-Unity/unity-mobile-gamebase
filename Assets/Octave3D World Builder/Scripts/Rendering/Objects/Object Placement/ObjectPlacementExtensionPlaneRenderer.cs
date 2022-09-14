#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementExtensionPlaneRenderer
    {
        #region Public Methods
        public void RenderGizmos(ObjectPlacementExtensionPlane extensionPlane)
        {
            ObjectPlacementExtensionPlaneRenderSettings renderSettings = extensionPlane.RenderSettings;
            XZOrientedQuad3D planeQuad = extensionPlane.PlaneQuad;
            planeQuad.SetScale(renderSettings.PlaneScale);

            // Note: Add a small offset to avoid Z wars when the extension plane sits on top of other objects.
            const float quadOffset = 0.005f;
            GizmosEx.RenderXZOrientedQuad(planeQuad, renderSettings.PlaneColor, quadOffset);     
            GizmosEx.RenderXZOrientedQuadBorderLines(planeQuad, renderSettings.PlaneBorderLineColor, quadOffset);

            // Render the plane normals
            List<Vector3> quadCornerPoints = planeQuad.GetCornerPoints();
            Vector3 offsetToEndOfLine = extensionPlane.Plane.normal * renderSettings.PlaneNormalLineLength;
            foreach(Vector3 quadCornerPoint in quadCornerPoints)
            {
                GizmosEx.RenderLine(quadCornerPoint, quadCornerPoint + offsetToEndOfLine, renderSettings.PlaneNormalLineColor);
            }
        }
        #endregion
    }
}
#endif