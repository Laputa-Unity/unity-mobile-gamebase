#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class XZGrid : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private XZGridRenderSettings _renderSettings;
        [SerializeField]
        private XZGridCellSizeSettings _cellSizeSettings;
        [SerializeField]
        private XZGridDimensionSettings _dimensionSettings = new XZGridDimensionSettings();

        [SerializeField]
        private RenderableCoordinateSystem _renderableCoordinateSystem = new RenderableCoordinateSystem();

        private XZGridRenderer _renderer = new XZGridRenderer();

        private bool _alignCellEdgeWithOrigin = true;
        #endregion

        #region Public Static Properties
        public static Vector3 ModelSpaceRightAxis { get { return Vector3.right; } }
        public static Vector3 ModeSpacePlaneNormal { get { return Vector3.up; } }
        public static Vector3 ModelSpaceLookAxis { get { return Vector3.forward; } }

        public static string ModelSpaceRightAxisName { get { return "X"; } }
        public static string ModelSpaceUpAxisName { get { return "Y"; } }
        public static string ModelSpaceLookAxisName { get { return "Z"; } }
        #endregion

        #region Public Properties
        public bool AlignCellEdgeWithOrigin { get { return _alignCellEdgeWithOrigin; } set { _alignCellEdgeWithOrigin = value; } }
        public XZGridRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<XZGridRenderSettings>();
                return _renderSettings;
            }
        }
        public XZGridCellSizeSettings CellSizeSettings
        {
            get
            {
                if (_cellSizeSettings == null) _cellSizeSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<XZGridCellSizeSettings>();
                return _cellSizeSettings;
            }
        }
        public XZGridDimensionSettings DimensionSettings { get { return _dimensionSettings; } }

        public TransformMatrix TransformMatrix { get { return _renderableCoordinateSystem.TransformMatrix; } }
        public RenderableCoordinateSystem RenderableCoordinateSystem { get { return _renderableCoordinateSystem; } }
        public Plane Plane { get { return new Plane(TransformMatrix.GetNormalizedUpAxis(), TransformMatrix.Translation); } }
        public Quaternion Rotation { get { return _renderableCoordinateSystem.GetRotation(); } }
        #endregion

        #region Public Methods
        public void SnapToPoint(Vector3 point)
        {
            Plane gridPlane = Plane;
            float planeDistanceToPoint = gridPlane.GetDistanceToPoint(point);
            SetOriginPosition(GetOriginPosition() + gridPlane.normal * planeDistanceToPoint);
        }

        public void SetTransformMatrix(TransformMatrix transformMatrix)
        {
            _renderableCoordinateSystem.SetTransformMatrix(transformMatrix);
        }

        public void Translate(Vector3 translationAmount)
        {
            _renderableCoordinateSystem.Translate(translationAmount);
        }

        public void SetYOriginPosition(float yOriginPosition)
        {
            Vector3 originPosition = GetOriginPosition();
            originPosition.y = yOriginPosition;
            SetOriginPosition(originPosition);
        }

        public void SetXOriginPosition(float xPos)
        {
            Vector3 originPosition = GetOriginPosition();
            originPosition.x = xPos;
            SetOriginPosition(originPosition);
        }

        public void SetZOriginPosition(float zPos)
        {
            Vector3 originPosition = GetOriginPosition();
            originPosition.z = zPos;
            SetOriginPosition(originPosition);
        }

        public void SetOriginPosition(Vector3 originPosition)
        {
            _renderableCoordinateSystem.SetOriginPosition(originPosition);
        }

        public Vector3 GetOriginPosition()
        {
            return _renderableCoordinateSystem.GetOriginPosition();
        }

        public void SetRotation(Quaternion rotation)
        {
            _renderableCoordinateSystem.SetRotation(rotation);
        }

        public void RenderGizmos()
        {
            _renderer.RenderGizmos(this, SceneViewCamera.Instance.GetViewVolume());
        }

        public XZGridCell GetCellFromPoint(Vector3 point)
        {
            int cellIndexX, cellIndexZ;
            CalculateCellIndicesFromPoint(point, out cellIndexX, out cellIndexZ);
            XZOrientedQuad3D cellQuad = CalculateCellQuad(cellIndexX, cellIndexZ);

            return new XZGridCell(cellIndexX, cellIndexZ, this, cellQuad);
        }

        public bool Raycast(Ray ray, out Vector3 intersectPoint)
        {
            intersectPoint = Vector3.zero;

            float t = 0.0f;
            if(Plane.Raycast(ray, out t))
            {
                intersectPoint = ray.GetPoint(t);
                return true;
            }

            return false;
        }

        public XZGridCell GetCellFromIndices(int xIndex, int zIndex)
        {
            return new XZGridCell(xIndex, zIndex, this, CalculateCellQuad(xIndex, zIndex));
        }

        public List<XZGridCell> GetCellsFromPoints(List<Vector3> points)
        {
            var gridCells = new List<XZGridCell>(points.Count);
            foreach (Vector3 point in points)
            {
                gridCells.Add(GetCellFromPoint(point));
            }

            return gridCells;
        }

        public void CalculateCellIndicesFromPoint(Vector3 point, out int cellIndexX, out int cellIndexZ)
        {
            Vector3 gridSpacePoint = TransformMatrix.MultiplyPointInverse(point);

            float offsetX = _alignCellEdgeWithOrigin ? 0.0f : CellSizeSettings.CellSizeX;
            float offsetZ = _alignCellEdgeWithOrigin ? 0.0f : CellSizeSettings.CellSizeZ;
            cellIndexX = Mathf.FloorToInt((gridSpacePoint.x + offsetX) / CellSizeSettings.CellSizeX);
            cellIndexZ = Mathf.FloorToInt((gridSpacePoint.z + offsetZ) / CellSizeSettings.CellSizeZ);
        }

        public XZOrientedQuad3D CalculateCellQuad(int cellIndexX, int cellIndexZ)
        {
            Vector3 quadGridSpaceCenter = GetCellHrzStart(cellIndexX) + XZGrid.ModelSpaceRightAxis * CellSizeSettings.HalfCellSizeX +
                                          GetCellDepthStart(cellIndexZ) + XZGrid.ModelSpaceLookAxis * CellSizeSettings.HalfCellSizeZ;
            Vector2 quadXZSize = new Vector2(CellSizeSettings.CellSizeX, CellSizeSettings.CellSizeZ);

            XZOrientedQuad3D orientedQuad = new XZOrientedQuad3D(quadGridSpaceCenter, quadXZSize);
            orientedQuad.Transform(TransformMatrix);

            return orientedQuad;
        }

        public Vector3 GetCellHrzStart(int hrzCellIndex)
        {
            float offset = _alignCellEdgeWithOrigin ? 0.0f : -CellSizeSettings.CellSizeX;
            return XZGrid.ModelSpaceRightAxis * (hrzCellIndex * CellSizeSettings.CellSizeX + offset);
        }

        public Vector3 GetCellDepthStart(int depthCellIndex)
        {
            float offset = _alignCellEdgeWithOrigin ? 0.0f : -CellSizeSettings.CellSizeZ;
            return XZGrid.ModelSpaceLookAxis * (depthCellIndex * CellSizeSettings.CellSizeZ + offset);
        }
        #endregion
    }
}
#endif