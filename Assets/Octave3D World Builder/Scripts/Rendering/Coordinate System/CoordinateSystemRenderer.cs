#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public class CoordinateSystemRenderer
    {
        #region Public Methods
        public void RenderGizmos(RenderableCoordinateSystem coordinateSystem)
        {
            CoordinateSystemRenderSettings coordinateSystemRenderSettings = coordinateSystem.RenderSettings;
            if (coordinateSystemRenderSettings.IsVisible)
            {
                GizmosMatrix.Push(coordinateSystem.TransformMatrix.ToMatrix4x4x);
                RenderAllCooridnateSystemAxes(coordinateSystemRenderSettings);
                GizmosMatrix.Pop();
            }
        }
        #endregion

        #region Private Methods
        private void RenderAllCooridnateSystemAxes(CoordinateSystemRenderSettings coordinateSystemRenderSettings)
        {
            List<CoordinateSystemAxis> coordinateSystemAxes = CoordinateSystemAxes.GetAll();
            foreach (CoordinateSystemAxis axis in coordinateSystemAxes)
            {
                RenderCoordinateSystemAxis(axis, coordinateSystemRenderSettings);
            }
        }

        private void RenderCoordinateSystemAxis(CoordinateSystemAxis axis, CoordinateSystemRenderSettings coordinateSystemRenderSettings)
        {
            if(coordinateSystemRenderSettings.IsAxisVisible(axis))
            {
                GizmosColor.Push(coordinateSystemRenderSettings.GetAxisColor(axis));
                Vector3 axisVector = CoordinateSystemAxes.GetGlobalVector(axis);
                Gizmos.DrawLine(Vector3.zero, axisVector * coordinateSystemRenderSettings.GetAxisSize(axis));
                GizmosColor.Pop();
            }
        }
        #endregion
    }
}
#endif