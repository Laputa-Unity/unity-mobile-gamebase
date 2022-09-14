#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public struct TransformMatrix
    {
        #region Private Static Variables
        private static TransformMatrix _identity = new TransformMatrix(Vector3.zero, Quaternion.identity, Vector3.one);
        #endregion

        #region Private Variables
        [SerializeField]
        private Vector3 _translation;
        [SerializeField]
        private Quaternion _rotation;
        [SerializeField]
        private Vector3 _scale;
        #endregion

        #region Constructors
        // Note: Supports only positive scale values.
        public TransformMatrix(Matrix4x4 matrix)
        {
            _translation = matrix.GetTranslation();
            _rotation = matrix.GetRotation();
            _scale = matrix.GetXYZScale();
        }

        public TransformMatrix(Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            _translation = translation;
            _rotation = rotation;
            _scale = scale;
        }

        public TransformMatrix(TransformMatrix source)
        {
            _translation = source._translation;
            _rotation = source._rotation;
            _scale = source._scale;
        }

        public TransformMatrix(Transform transform)
        {
            _translation = transform.position;
            _rotation = transform.rotation;
            _scale = transform.lossyScale;
        }
        #endregion

        #region Public Properties
        public Vector3 Translation { get { return _translation; } set { _translation = value; } }
        public Quaternion Rotation { get { return _rotation; } set { _rotation = value; } }
        public Vector3 Scale { get { return _scale; } set { _scale = value; } }
        public float AllAxesScale { set { Scale = Vector3.one * value; } }
        public Matrix4x4 ToMatrix4x4x { get { return Matrix4x4.TRS(_translation, _rotation, _scale); } }
        public float XScale { get { return _scale[0]; } set { _scale[0] = value; } }
        public float YScale { get { return _scale[1]; } set { _scale[1] = value; } }
        public float ZScale { get { return _scale[2]; } set { _scale[2] = value; } }
        #endregion

        #region Public Static Functions
        public static TransformMatrix GetIdentity()
        {
            return _identity;
        }

        public static TransformMatrix operator * (TransformMatrix mtx1, TransformMatrix mtx2)
        {
            Matrix4x4 mtx1Scale = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, mtx1.Scale);
            Matrix4x4 mtx2Scale = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, mtx2.Scale);
            Matrix4x4 compScale = mtx1Scale * mtx2Scale;
            Vector3 newScale = compScale.GetSignedXYZScale();

            Matrix4x4 mtx1Rot = Matrix4x4.TRS(Vector3.zero, mtx1.Rotation, mtx1.Scale);
            Matrix4x4 mtx2Rot = Matrix4x4.TRS(Vector3.zero, mtx2.Rotation, mtx2.Scale);
            Matrix4x4 compRot = mtx1Rot * mtx2Rot;
            Quaternion newRotation = compRot.GetRotation();
         
            Vector3 newTranslation = mtx1.MultiplyPoint(mtx2.Translation);
            return new TransformMatrix(newTranslation, newRotation, newScale);
        }
        #endregion

        #region Public Methods
        public Vector3 MultiplyPointInverse(Vector3 point)
        {
            return ToMatrix4x4x.inverse.MultiplyPoint(point);
        }

        public Vector3 MultiplyPoint(Vector3 point)
        {
            return ToMatrix4x4x.MultiplyPoint(point);
        }

        public Vector3 MultiplyVectorInverse(Vector3 vector)
        {
            return ToMatrix4x4x.inverse.MultiplyVector(vector);
        }

        public Vector3 MultiplyVector(Vector3 vector)
        {
            return ToMatrix4x4x.MultiplyVector(vector); 
        }

        public Vector3 GetNormalizedAxis(int axisIndex)
        {
            return ToMatrix4x4x.GetNormalizedAxis(axisIndex);
        }

        public Vector3 GetNormalizedRightAxis()
        {
            return ToMatrix4x4x.GetNormalizedRightAxis();
        }

        public Vector3 GetNormalizedRightAxisNoScaleSign()
        {
            Vector3 rightAxis = ToMatrix4x4x.GetNormalizedRightAxis();
            if (_scale.x < 0.0f) rightAxis *= -1.0f;

            return rightAxis;
        }

        public Vector3 GetNormalizedUpAxis()
        {
            return ToMatrix4x4x.GetNormalizedUpAxis();
        }

        public Vector3 GetNormalizedUpAxisNoScaleSign()
        {
            Vector3 upAxis = ToMatrix4x4x.GetNormalizedUpAxis();
            if (_scale.y < 0.0f) upAxis *= -1.0f;

            return upAxis;
        }

        public Vector3 GetNormalizedLookAxis()
        {
            return ToMatrix4x4x.GetNormalizedLookAxis();
        }

        public Vector3 GetNormalizedLookAxisNoScaleSign()
        {
            Vector3 lookAxis = ToMatrix4x4x.GetNormalizedLookAxis();
            if (_scale.z < 0.0f) lookAxis *= -1.0f;

            return lookAxis;
        }

        public Vector3[] GetNormalizedLocalAxes()
        {
            return ToMatrix4x4x.GetNormalizedLocalAxes();
        }

        public void Translate(Vector3 translationAmount)
        {
            _translation += translationAmount;
        }

        public void InvertAxisScale(CoordinateSystemAxis axis)
        {
            if (axis == CoordinateSystemAxis.PositiveRight || axis == CoordinateSystemAxis.NegativeRight) XScale = -XScale;
            else
            if (axis == CoordinateSystemAxis.PositiveUp || axis == CoordinateSystemAxis.NegativeUp) YScale = -YScale;
            else ZScale = -ZScale;
        }

        public int GetIndexOfAxisMostAlignedWith(Vector3 referenceAxis)
        {
            return ToMatrix4x4x.GetIndexOfAxisMostAlignedWith(referenceAxis);
        }

        public int GetIndexOfAxisAlignedWith(Vector3 referenceAxis)
        {
            return ToMatrix4x4x.GetIndexOfAxisAlignedWith(referenceAxis);
        }
        #endregion
    }
}
#endif