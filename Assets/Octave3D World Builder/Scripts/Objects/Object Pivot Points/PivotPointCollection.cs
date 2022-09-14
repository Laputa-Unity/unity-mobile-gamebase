#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PivotPointCollection
    {
        #region Protected Variables
        [SerializeField]
        private int _indexOfActivePoint = 0;
        [SerializeField]
        private List<Vector3> _pivotPoints = new List<Vector3> { Vector3.zero };
        #endregion

        #region Public Properties
        public int IndexOfActivePoint { get { return _indexOfActivePoint; } }
        public Vector3 ActivePoint { get { return _pivotPoints[_indexOfActivePoint]; } }
        public List<Vector3> AllPoints { get { return new List<Vector3>(_pivotPoints); } }
        public int NumberOfPoints { get { return _pivotPoints.Count; } }
        #endregion

        #region Public Methods
        public PivotPointCollection TakeSnapshot()
        {
            var pivotPointCollection = new PivotPointCollection();
            pivotPointCollection._indexOfActivePoint = IndexOfActivePoint;
            pivotPointCollection._pivotPoints = new List<Vector3>(AllPoints);

            return pivotPointCollection;
        }

        public Vector3 GetPointByIndex(int pointIndex)
        {
            return _pivotPoints[pointIndex];
        }

        public void SetPivotPoints(List<Vector3> pivotPoints, int indexOfSelectedPoint)
        {
            if (pivotPoints.Count != 0)
            {
                _pivotPoints = new List<Vector3>(pivotPoints);
                _indexOfActivePoint = Mathf.Clamp(indexOfSelectedPoint, 0, _pivotPoints.Count);
            }
        }

        public void ActivatePoint(int pointIndex)
        {
            _indexOfActivePoint = pointIndex;
        }

        public void ActivateNextPoint()
        {
            ++_indexOfActivePoint;
            _indexOfActivePoint %= _pivotPoints.Count;
        }

        public void MovePoints(Vector3 moveVector)
        {
            _pivotPoints = Vector3Extensions.ApplyOffsetToPoints(_pivotPoints, moveVector);
        }
        #endregion
    }
}
#endif