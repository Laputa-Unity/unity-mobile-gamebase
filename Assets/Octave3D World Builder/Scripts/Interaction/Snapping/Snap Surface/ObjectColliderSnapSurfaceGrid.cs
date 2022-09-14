#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectColliderSnapSurfaceGrid
    {
        #region Private Variables
        [SerializeField]
        private XZGrid _grid;
        #endregion

        #region Private Properties
        private XZGrid Grid
        {
            get
            {
                if(_grid == null)
                {
                    _grid = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<XZGrid>();
                    _grid.DimensionSettings.DimensionType = XZGridDimensionType.Finite;
                    _grid.RenderSettings.CellLineThickness = 0.01f;
                    _grid.RenderSettings.PlaneColor = new Color(0.3254f, 1.0f, 0.2f, 0.3019f);
                    _grid.RenderableCoordinateSystem.RenderSettings.IsVisible = false;
                }

                return _grid;
            }
        }
        #endregion

        #region Public Properties
        public XZGridRenderSettings RenderSettings { get { return Grid.RenderSettings; } }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            Vector3 oldOriginPosition = Grid.GetOriginPosition();

            // Note: This should not be necessary, but on a couple of ocasions this was reset to 'Infinite' probably due to a serialization issue.
            //       Can not reproduce this anymore. Moreover, the 'XZGrid' class has been made into a ScripatbleObject which most likely fixed the
            //       issue. However, it's better to be safe :)
            Grid.DimensionSettings.DimensionType = XZGridDimensionType.Finite; 
            Grid.Translate(Grid.Plane.normal * 0.002f);
            Grid.RenderGizmos();
            Grid.SetOriginPosition(oldOriginPosition);
        }

        public XZGridCell GetCellFromPoint(Vector3 point)
        {
            return Grid.GetCellFromPoint(point);
        }

        public void FromXZOrientedQuad(XZOrientedQuad3D orientedQuad)
        {
            float desiredCellSize = ObjectSnapSettings.Get().ObjectColliderSnapSurfaceGridSettings.DesiredCellSize;

            // As a first step, ensure that the grid sits in the middle of the quad and has the same orientation
            Grid.SetOriginPosition(orientedQuad.GetOriginPosition());
            Grid.SetRotation(orientedQuad.GetRotation());

            Vector2 quadSize = orientedQuad.ScaledXZSize;
            float cellSizeX = desiredCellSize;
            float cellSizeZ = desiredCellSize;

            int cellCountX, cellCountZ;
            if(cellSizeX > quadSize.x)
            {
                cellSizeX = quadSize.x;
                cellCountX = 1;
            }
            else
            {
                // Adjust the cell size in such a way that we get an integer number of cells along each dimension
                float divisionResult = quadSize.x / cellSizeX;
                cellCountX = (int)divisionResult;
                float fractionalValue = divisionResult - cellCountX;
                if (fractionalValue != 0.0f) cellSizeX += (fractionalValue / cellCountX) * desiredCellSize;     // If the fractional value is not 0, it means the cell size needs to be adjusted
                                                                                                                // in such a way that the calculated number of cells ('cellCountX') covers the entire
                                                                                                                // quad area along the corresponding dimension.
            }

            if(cellSizeZ > quadSize.y)
            {
                cellSizeZ = quadSize.y;
                cellCountZ = 1;
            }
            else
            {
                float divisionResult = quadSize.y / cellSizeZ;
                cellCountZ = (int)divisionResult;
                float fractionalValue = divisionResult - cellCountZ;
                if (fractionalValue != 0.0f) cellSizeZ += (fractionalValue / cellCountZ) * desiredCellSize;
            }

            // Make sure the cell size is not larger than the quad
            if (cellSizeX > quadSize.x) cellSizeX = quadSize.x;
            if (cellSizeZ > quadSize.y) cellSizeZ = quadSize.y;

            Grid.CellSizeSettings.CellSizeX = cellSizeX;
            Grid.CellSizeSettings.CellSizeZ = cellSizeZ;

            // Store the cell count without taking into consideration the cell which sits at the origin of the grid
            int cellCountXNoMiddle = cellCountX - 1;
            int cellCountZNoMiddle = cellCountZ - 1;

            if(cellCountXNoMiddle % 2 == 0)
            {
                int halfCount = cellCountXNoMiddle / 2;
                Grid.DimensionSettings.FiniteDimensionSettings.XAxisCellIndexRange.Min = -halfCount;
                Grid.DimensionSettings.FiniteDimensionSettings.XAxisCellIndexRange.Max = halfCount;
            }
            else
            {
                int halfCount = cellCountXNoMiddle / 2;
                Grid.DimensionSettings.FiniteDimensionSettings.XAxisCellIndexRange.Min = -halfCount;
                Grid.DimensionSettings.FiniteDimensionSettings.XAxisCellIndexRange.Max = halfCount + 1;
            }

            if (cellCountZNoMiddle % 2 == 0)
            {
                int halfCount = cellCountZNoMiddle / 2;
                Grid.DimensionSettings.FiniteDimensionSettings.ZAxisCellIndexRange.Min = -halfCount;
                Grid.DimensionSettings.FiniteDimensionSettings.ZAxisCellIndexRange.Max = halfCount;
            }
            else
            {
                int halfCount = cellCountZNoMiddle / 2;
                Grid.DimensionSettings.FiniteDimensionSettings.ZAxisCellIndexRange.Min = -halfCount;
                Grid.DimensionSettings.FiniteDimensionSettings.ZAxisCellIndexRange.Max = halfCount + 1;
            }

            // We need to make sure that the grid nicely sits within the boundaries of the quad. In order to do this, we will
            // align the top left corners of the quad and the grid's top left cell. 
            Vector3 quadTopLeftCornerPoint = orientedQuad.GetCornerPoints()[(int)XZOrientedQuad3DCornerPoint.TopLeft];
            XZOrientedQuad3D topLeftCellQuad = Grid.CalculateCellQuad(Grid.DimensionSettings.FiniteDimensionSettings.XAxisCellIndexRange.Min, Grid.DimensionSettings.FiniteDimensionSettings.ZAxisCellIndexRange.Max);
            Vector3 cellQuadTopLeftCornerPoint = topLeftCellQuad.GetCornerPoints()[(int)XZOrientedQuad3DCornerPoint.TopLeft];
            Vector3 gridOriginMoveVector = quadTopLeftCornerPoint - cellQuadTopLeftCornerPoint;
            Grid.Translate(gridOriginMoveVector);
        }
        #endregion
    }
}
#endif