#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class CoordinateSystem
    {
        #region Private Variables
        [SerializeField]
        private Vector3[] _localAxes = new Vector3[6];
        [SerializeField]
        private TransformMatrix _transformMatrix = TransformMatrix.GetIdentity();
        #endregion

        #region Public Properties
        public TransformMatrix TransformMatrix { get { return _transformMatrix; } }
        #endregion

        #region Constructors
        public CoordinateSystem()
        {
            SetTransformMatrix(TransformMatrix.GetIdentity());
        }

        public CoordinateSystem(TransformMatrix transformMatrix)
        {
            SetTransformMatrix(transformMatrix);
        }
        #endregion

        #region Public Methods
        public void SetTransformMatrix(TransformMatrix transformMatrix)
        {
            _transformMatrix = transformMatrix;
            RecalculateLocalAxes();
        }

        public void Transform(TransformMatrix transformMatrix)
        {
            _transformMatrix = transformMatrix * _transformMatrix;
            RecalculateLocalAxes();
        }

        public void InvertAxisScale(CoordinateSystemAxis axis)
        {
            _transformMatrix.InvertAxisScale(axis);
            RecalculateLocalAxes();
        }

        public void Translate(Vector3 translationAmount)
        {
            _transformMatrix.Translate(translationAmount);
        }

        public void SetScaleOnAllAxes(float scale)
        {
            _transformMatrix.AllAxesScale = scale;
            RecalculateLocalAxes();
        }

        public void SetScale(Vector3 scale)
        {
            _transformMatrix.Scale = scale;
            RecalculateLocalAxes();
        }

        public Vector3 GetAxisVector(CoordinateSystemAxis axis)
        {
            return _localAxes[(int)axis];
        }

        public List<Vector3> GetAllAxesVectors()
        {
            List<CoordinateSystemAxis> allAxes = CoordinateSystemAxes.GetAll();
            var allAxesVectors = new List<Vector3>(allAxes.Count);

            foreach (CoordinateSystemAxis axis in allAxes)
            {
                allAxesVectors.Add(GetAxisVector(axis));
            }

            return allAxesVectors;
        }

        public Vector3 GetOriginPosition()
        {
            return _transformMatrix.Translation;
        }

        public Quaternion GetRotation()
        {
            return _transformMatrix.Rotation;
        }

        public void SetOriginPosition(Vector3 originPosition)
        {
            _transformMatrix.Translation = originPosition;
        }

        public void SetRotation(Quaternion rotation)
        {
            // Note: Because of floating point rounding errors it is possible that the rotation
            //       quaternion is not normalized. So we will first normalize it before doing
            //       anything else with it. Otherwise invalid quaternion exceptions will be thrown.
            rotation = rotation.NormalizeEx();

            _transformMatrix.Rotation = rotation;
            RecalculateLocalAxes();
        }
        #endregion

        #region Private Methods
        private void RecalculateLocalAxes()
        {
            RecalculatePositiveAndNegativeRightAxes();
            RecalculatePositiveAndNegativeUpAxes();
            RecalculatePositiveAndNegativeLookAxes();
        }

        private void RecalculatePositiveAndNegativeRightAxes()
        {
            _localAxes[0] = _transformMatrix.GetNormalizedRightAxis();
            _localAxes[1] = -_localAxes[0];
        }

        private void RecalculatePositiveAndNegativeUpAxes()
        {
            _localAxes[2] = _transformMatrix.GetNormalizedUpAxis();
            _localAxes[3] = -_localAxes[2];
        }

        private void RecalculatePositiveAndNegativeLookAxes()
        {
            _localAxes[4] = _transformMatrix.GetNormalizedLookAxis();
            _localAxes[5] = -_localAxes[4];
        }
        #endregion
    }
}
#endif