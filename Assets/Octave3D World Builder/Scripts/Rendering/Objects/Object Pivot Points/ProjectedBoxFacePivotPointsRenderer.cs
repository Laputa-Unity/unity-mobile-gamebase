#if UNITY_EDITOR
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    public class ProjectedBoxFacePivotPointsRenderer
    {
        #region Public Methods
        public void RenderGizmos(ProjectedBoxFacePivotPoints projectedBoxFacePivotPoints, ObjectPivotPointsRenderSettings pivotPointsRenderSettings)
        {
            if (ObjectPlacementGuide.ExistsInSceneAndIsActive && 
                SceneViewCamera.Instance.IsGameObjectHierarchyVisible(ObjectPlacementGuide.SceneObject))
            {
                RenderPivotPointConnectionLines(projectedBoxFacePivotPoints, pivotPointsRenderSettings);
                RenderPivotPointProjectionLines(projectedBoxFacePivotPoints, pivotPointsRenderSettings);
                RenderAllPivotPoints(projectedBoxFacePivotPoints, pivotPointsRenderSettings);
            }
        }
        #endregion

        #region Private Methods
        private void RenderAllPivotPoints(ProjectedBoxFacePivotPoints projectedBoxFacePivotPoints, ObjectPivotPointsRenderSettings pivotPointsRenderSettings)
        {
            List<Vector3> allPivotPoints = projectedBoxFacePivotPoints.AllPoints;
            if (allPivotPoints.Count != 0)
            {
                ProjectedBoxFacePivotPointsRenderSettings renderSettings = pivotPointsRenderSettings.ProjectedBoxFacePivotPointsRenderSettings;
                IPivotPointRenderer objectPivotPointRenderer = PivotPointRendererFactory.Create(pivotPointsRenderSettings.ShapeType);

                Color activePivotPointFillColor = renderSettings.ActivePivotPointRenderSettings.FillColor;
                Color inactivePivotPointFillColor = renderSettings.InactivePivotPointRenderSettings.FillColor;
                Color activePivotPointBorderLineColor = renderSettings.ActivePivotPointRenderSettings.BorderLineColor;
                Color inactivePivotPointBorderLineColor = renderSettings.InactivePivotPointRenderSettings.BorderLineColor;

                float pivotPointSizeInPixels = pivotPointsRenderSettings.PivotPointSizeInPixels;
                float activePivotPointScale = renderSettings.ActivePivotPointRenderSettings.Scale;
                float inactivePivotPointScale = renderSettings.InactivePivotPointRenderSettings.Scale;

                if(renderSettings.InactivePivotPointRenderSettings.IsVisible)
                {
                    for (int pivotPointIndex = 0; pivotPointIndex < allPivotPoints.Count; ++pivotPointIndex)
                    {
                        if (pivotPointIndex != projectedBoxFacePivotPoints.IndexOfActivePoint)
                            objectPivotPointRenderer.Render(allPivotPoints[pivotPointIndex], inactivePivotPointFillColor, inactivePivotPointBorderLineColor, pivotPointSizeInPixels * inactivePivotPointScale);
                    }
                }

                if(renderSettings.ActivePivotPointRenderSettings.IsVisible)
                    objectPivotPointRenderer.Render(allPivotPoints[projectedBoxFacePivotPoints.IndexOfActivePoint], activePivotPointFillColor, activePivotPointBorderLineColor, pivotPointSizeInPixels * activePivotPointScale);
            }
        }

        private void RenderPivotPointConnectionLines(ProjectedBoxFacePivotPoints projectedBoxFacePivotPoints, ObjectPivotPointsRenderSettings pivotPointsRenderSettings)
        {
            ProjectedBoxFacePivotPointsRenderSettings renderSettings = pivotPointsRenderSettings.ProjectedBoxFacePivotPointsRenderSettings;

            if(renderSettings.RenderPivotPointConnectionLines)
            {
                List<Vector3> pivotPointsNoCenter = projectedBoxFacePivotPoints.GetAllPointsExcludingCenter();
                if (pivotPointsNoCenter.Count != 0) GizmosEx.RenderLinesBetweenPoints(pivotPointsNoCenter, renderSettings.PivotPointConnectionLineColor);
            }
        }

        private void RenderPivotPointProjectionLines(ProjectedBoxFacePivotPoints projectedBoxFacePivotPoints, ObjectPivotPointsRenderSettings pivotPointsRenderSettings)
        {
            ProjectedBoxFacePivotPointsRenderSettings renderSettings = pivotPointsRenderSettings.ProjectedBoxFacePivotPointsRenderSettings;

            if(renderSettings.RenderProjectionLines)
            {
                List<Vector3> allPivotPoints = projectedBoxFacePivotPoints.AllPoints;
                if(allPivotPoints.Count != 0)
                {
                    Color projectionLineColor = renderSettings.ProjectionLineColor;

                    List<Vector3> unprojectedPoints = projectedBoxFacePivotPoints.GetUnprojectedPivotPoints();
                    for(int pointIndex = 0; pointIndex < unprojectedPoints.Count; ++pointIndex)
                    {
                        GizmosEx.RenderLine(allPivotPoints[pointIndex], unprojectedPoints[pointIndex], projectionLineColor);
                    }
                }
            }
        }
        #endregion
    }
}
#endif