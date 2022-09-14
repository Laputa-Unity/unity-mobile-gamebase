#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class XZGridRenderer
    {
        public void RenderGizmos(XZGrid grid, CameraViewVolume cameraViewVolume)
        {
            if (!grid.RenderSettings.IsVisible) return;

            // Note: Can not figure out how to render a finite grid inside a shader yet... :D
            if(grid.DimensionSettings.DimensionType == XZGridDimensionType.Finite)
            {
                RenderGizmos_Obsolete(grid, cameraViewVolume);
                return;
            }

            Plane gridPlane = grid.Plane;
            Vector3 gridPlaneCenter = gridPlane.ProjectPoint(SceneViewCamera.Camera.transform.position);

            Box camVolumeAABB = cameraViewVolume.WorldSpaceAABB;     
            List<Vector3> projectedVolumeAABBPts = gridPlane.ProjectAllPoints(camVolumeAABB.GetCornerPoints());
            List<Vector3> modelSpacePrjPts = Vector3Extensions.GetTransformedPoints(projectedVolumeAABBPts, grid.TransformMatrix.ToMatrix4x4x.inverse);
            Box modelSpacePtsBox = Box.FromPoints(modelSpacePrjPts);
            Vector3 gridPlaneSize = modelSpacePtsBox.Size;

            Matrix4x4 planeTransformMatrix = Matrix4x4.TRS(gridPlaneCenter, grid.Rotation, gridPlaneSize);
            Material xzGridMaterial = MaterialPool.Get().XZGridMaterial;
            xzGridMaterial.SetFloat("_CellSizeX", grid.CellSizeSettings.CellSizeX);
            xzGridMaterial.SetFloat("_CellSizeZ", grid.CellSizeSettings.CellSizeZ);
            xzGridMaterial.SetVector("_CellOffset", grid.GetOriginPosition());
            xzGridMaterial.SetColor("_LineColor", grid.RenderSettings.CellLineColor);
            xzGridMaterial.SetColor("_PlaneColor", grid.RenderSettings.PlaneColor);
            xzGridMaterial.SetFloat("_CamFarPlaneDist", SceneViewCamera.Camera.farClipPlane);
            xzGridMaterial.SetVector("_CamWorldPos", SceneViewCamera.Camera.transform.position);
            xzGridMaterial.SetMatrix("_InvRotMatrix", Matrix4x4.TRS(Vector3.zero, grid.Rotation, Vector3.one).inverse);
            xzGridMaterial.SetMatrix("_PlaneTransformMtx", planeTransformMatrix);

            int numPasses = xzGridMaterial.passCount;
            for (int passIndex = 0; passIndex < numPasses; ++passIndex)
            {
                xzGridMaterial.SetPass(passIndex);
                Graphics.DrawMeshNow(GizmosEx.XZRectangleMesh, planeTransformMatrix);
            }

            GizmosMatrix.Push(grid.TransformMatrix.ToMatrix4x4x);
            grid.RenderableCoordinateSystem.RenderGizmos();
            GizmosMatrix.Pop();
        }

        private void RenderGizmos_Obsolete(XZGrid grid, CameraViewVolume cameraViewVolume)
        {
            if (!grid.RenderSettings.IsVisible) return;

            var visibleCellRangeCalculator = new XZVisibleGridCellRangeCalculator();
            XZVisibleGridCellRange visibleCellRange = visibleCellRangeCalculator.Calculate(grid, cameraViewVolume);

            GizmosMatrix.Push(grid.TransformMatrix.ToMatrix4x4x);
            RenderGridPlane_Obsolete(grid, visibleCellRange);
            RenderGridCellLines_Obsolete(grid, visibleCellRange);
            grid.RenderableCoordinateSystem.RenderGizmos();
            GizmosMatrix.Pop();
        }

        private void RenderGridPlane_Obsolete(XZGrid grid, XZVisibleGridCellRange visibleCellRange)
        {
            int minCellIndexX = visibleCellRange.XAxisVisibleCellRange.Min;
            int maxCellIndexX = visibleCellRange.XAxisVisibleCellRange.Max;

            int minCellIndexZ = visibleCellRange.ZAxisVisibleCellRange.Min;
            int maxCellIndexZ = visibleCellRange.ZAxisVisibleCellRange.Max;

            float numberOfCellsOnX = maxCellIndexX - minCellIndexX + 1.0f;
            float numberOfCellsOnZ = maxCellIndexZ - minCellIndexZ + 1.0f;

            XZGridCellSizeSettings gridCellSizeSettings = grid.CellSizeSettings;
            Vector3 planeCenter = (grid.GetCellHrzStart(minCellIndexX) + grid.GetCellDepthStart(minCellIndexZ) +
                                   grid.GetCellHrzStart(maxCellIndexX + 1) + grid.GetCellDepthStart(maxCellIndexZ + 1)) * 0.5f;
            Vector3 planeSize = new Vector3(numberOfCellsOnX * gridCellSizeSettings.CellSizeX, 0.0f, numberOfCellsOnZ * gridCellSizeSettings.CellSizeZ);

            GizmosColor.Push(grid.RenderSettings.PlaneColor);
            Gizmos.DrawCube(planeCenter, planeSize);
            GizmosColor.Pop();
        }

        private void RenderGridCellLines_Obsolete(XZGrid grid, XZVisibleGridCellRange visibleCellRange)
        {
            int minCellIndexX = visibleCellRange.XAxisVisibleCellRange.Min;
            int maxCellIndexX = visibleCellRange.XAxisVisibleCellRange.Max;
            float numberOfCellsOnX = maxCellIndexX - minCellIndexX + 1.0f;

            int minCellIndexZ = visibleCellRange.ZAxisVisibleCellRange.Min;
            int maxCellIndexZ = visibleCellRange.ZAxisVisibleCellRange.Max;
            float numberOfCellsOnZ = maxCellIndexZ - minCellIndexZ + 1.0f;

            XZGridCellSizeSettings gridCellSizeSettings = grid.CellSizeSettings;      
            XZGridRenderSettings gridRenderSettings = grid.RenderSettings;

            Vector3 startPointOnX, startPointOnZ;
            Vector3 lineCubeSize = Vector3.zero;
  
            // Render the grid lines which extend along the grid's X axis
            GizmosColor.Push(grid.RenderSettings.CellLineColor);
            startPointOnX = grid.GetCellHrzStart(minCellIndexX);
            lineCubeSize.z = gridRenderSettings.CellLineThickness;
            int maxLineIndex = maxCellIndexZ + 1;
            for (int lineIndex = minCellIndexZ; lineIndex <= maxLineIndex; ++lineIndex)
            {
                startPointOnZ = grid.GetCellDepthStart(lineIndex);
                Vector3 firstPoint = startPointOnX + startPointOnZ;
                Vector3 secondPoint = firstPoint + XZGrid.ModelSpaceRightAxis * (numberOfCellsOnX * gridCellSizeSettings.CellSizeX);

                lineCubeSize.x = (firstPoint - secondPoint).magnitude + gridRenderSettings.CellLineThickness;
                Gizmos.DrawCube((firstPoint + secondPoint) * 0.5f, lineCubeSize);
            }

            // Render the grid lines which extend along the grid's Z axis
            startPointOnZ = grid.GetCellDepthStart(minCellIndexZ);
            lineCubeSize.x = gridRenderSettings.CellLineThickness;
            maxLineIndex = maxCellIndexX + 1;
            for (int lineIndex = minCellIndexX; lineIndex <= maxLineIndex; ++lineIndex)
            {
                startPointOnX = grid.GetCellHrzStart(lineIndex);
                Vector3 firstPoint = startPointOnX + startPointOnZ;
                Vector3 secondPoint = firstPoint + XZGrid.ModelSpaceLookAxis * (numberOfCellsOnZ * gridCellSizeSettings.CellSizeZ);

                lineCubeSize.z = (firstPoint - secondPoint).magnitude;
                Gizmos.DrawCube((firstPoint + secondPoint) * 0.5f, lineCubeSize);
            }
            GizmosColor.Pop();
        }
    }
}
#endif