#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class XZVisibleGridCellRangeCalculator
    {
        #region Public Methods
        public XZVisibleGridCellRange Calculate(XZGrid grid, CameraViewVolume cameraViewVolume)
        {
            List<Vector3> gridPlaneIntersectionPoints = GetGridPlaneIntersectionPointsWithVolumeRays(grid, cameraViewVolume);
            return CalculateVisibleCellRangeFromGridPlaneIntersectionPoints(grid, gridPlaneIntersectionPoints);
        }
        #endregion

        #region Private Methods
        private List<Vector3> GetGridPlaneIntersectionPointsWithVolumeRays(XZGrid grid, CameraViewVolume cameraViewVolume)
        {
            Plane gridPlane = grid.Plane;
            Ray3D[] volumeRays = cameraViewVolume.WorldSpaceVolumeEdgeRays;

            float t;
            var intersectionPoints = new List<Vector3>();
            foreach(Ray3D ray in volumeRays)
            {
                if (ray.IntersectsPlane(gridPlane, out t)) intersectionPoints.Add(ray.Origin + ray.Direction * t);
            }

            return intersectionPoints;
        }

        private XZVisibleGridCellRange CalculateVisibleCellRangeFromGridPlaneIntersectionPoints(XZGrid grid, List<Vector3> gridPlaneIntersectionPoints)
        {
            if (gridPlaneIntersectionPoints.Count == 0) new XZVisibleGridCellRange();

            List<XZGridCell> gridCellsFromIntersectionPoints = grid.GetCellsFromPoints(gridPlaneIntersectionPoints);
            return CalculateVisibleCellRangeFromGridCells(grid, gridCellsFromIntersectionPoints);
        }

        private XZVisibleGridCellRange CalculateVisibleCellRangeFromGridCells(XZGrid grid, List<XZGridCell> gridCells)
        {
            int minCellIndexX = int.MaxValue;
            int maxCellIndexX = int.MinValue;
            int minCellIndexZ = int.MaxValue;
            int maxCellIndexZ = int.MinValue;

            foreach(XZGridCell cell in gridCells)
            {
                if (cell.XIndex < minCellIndexX) minCellIndexX = cell.XIndex;
                if (cell.XIndex > maxCellIndexX) maxCellIndexX = cell.XIndex;

                if (cell.ZIndex < minCellIndexZ) minCellIndexZ = cell.ZIndex;
                if (cell.ZIndex > maxCellIndexZ) maxCellIndexZ = cell.ZIndex;
            }

            XZGridDimensionSettings gridDimensionSettings = grid.DimensionSettings;
            if(gridDimensionSettings.DimensionType == XZGridDimensionType.Finite)
            {
                XZGridFiniteDimensionSettings gridFiniteDimensionSettings = grid.DimensionSettings.FiniteDimensionSettings;

                if (minCellIndexX < gridFiniteDimensionSettings.XAxisCellIndexRange.Min) minCellIndexX = gridFiniteDimensionSettings.XAxisCellIndexRange.Min;
                if (maxCellIndexX > gridFiniteDimensionSettings.XAxisCellIndexRange.Max) maxCellIndexX = gridFiniteDimensionSettings.XAxisCellIndexRange.Max;

                if (minCellIndexZ < gridFiniteDimensionSettings.ZAxisCellIndexRange.Min) minCellIndexZ = gridFiniteDimensionSettings.ZAxisCellIndexRange.Min;
                if (maxCellIndexZ > gridFiniteDimensionSettings.ZAxisCellIndexRange.Max) maxCellIndexZ = gridFiniteDimensionSettings.ZAxisCellIndexRange.Max;

                if (minCellIndexX > maxCellIndexX) minCellIndexX = maxCellIndexX;
                if (minCellIndexZ > maxCellIndexZ) minCellIndexZ = maxCellIndexZ;
            }

            var visibleCellRange = new XZVisibleGridCellRange();
            visibleCellRange.XAxisVisibleCellRange.Min = minCellIndexX;
            visibleCellRange.XAxisVisibleCellRange.Max = maxCellIndexX;
            visibleCellRange.ZAxisVisibleCellRange.Min = minCellIndexZ;
            visibleCellRange.ZAxisVisibleCellRange.Max = maxCellIndexZ;

            return visibleCellRange;
        }
        #endregion
    }
}
#endif