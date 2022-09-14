#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class GridCellRayHit
    {
        #region Private Variables
        private Ray _ray;
        private float _hitEnter;
        private XZGridCell _hitCell;
        private Vector3 _hitPoint;
        private Vector3 _hitNormal;
        #endregion

        #region Public Properties
        public Ray Ray { get { return _ray; } }
        public float HitEnter { get { return _hitEnter; } }
        public XZGridCell HitCell { get { return _hitCell; } }
        public Vector3 HitPoint { get { return _hitPoint; } }
        public Vector3 HitNormal { get { return _hitNormal; } }
        #endregion

        #region Constructors
        public GridCellRayHit(Ray ray, float hitEnter, XZGridCell hitCell)
        {
            _ray = ray;
            _hitEnter = hitEnter;

            _hitCell = hitCell;
            _hitPoint = ray.GetPoint(hitEnter);   
            _hitNormal = _hitCell.ParentGrid.TransformMatrix.GetNormalizedUpAxis();
        }
        #endregion
    }
}
#endif
