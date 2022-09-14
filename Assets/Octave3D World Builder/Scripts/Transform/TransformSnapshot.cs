#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class TransformSnapshot
    {
        #region Private Variables
        private Vector3 _worldSpacePosition;
        private Quaternion _worldSpaceRotation;
        private Vector3 _worldSpaceScale;
        #endregion

        #region Public Methods
        public void TakeSnapshot(Transform transform)
        {
            _worldSpacePosition = transform.position;
            _worldSpaceRotation = transform.rotation;
            _worldSpaceScale = transform.lossyScale;
        }

        public override bool Equals(object value)
        {
            if (ReferenceEquals(value, null)) return false;
            if (ReferenceEquals(value, this)) return true;

            if (value.GetType() != this.GetType()) return false;
            return IsEqual(value as TransformSnapshot);
        }

        public bool Equals(TransformSnapshot transformSnapshot)
        {
            if (ReferenceEquals(transformSnapshot, null)) return false;
            if (ReferenceEquals(transformSnapshot, this)) return true;

            return IsEqual(transformSnapshot);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + _worldSpacePosition.GetHashCode();
            hash = (hash * 7) + _worldSpaceRotation.GetHashCode();
            hash = (hash * 7) + _worldSpaceScale.GetHashCode();

            return hash;
        }

        public static bool operator ==(TransformSnapshot firstSnapshot, TransformSnapshot secondSnapshot)
        {
            return !ReferenceEquals(firstSnapshot, null) &&
                   !ReferenceEquals(secondSnapshot, null) &&
                   (ReferenceEquals(firstSnapshot, secondSnapshot) || firstSnapshot.IsEqual(secondSnapshot));
        }

        public static bool operator !=(TransformSnapshot firstSnapshot, TransformSnapshot secondSnapshot)
        {
            return !(firstSnapshot == secondSnapshot);
        }
        #endregion

        #region Private Methods
        private bool IsEqual(TransformSnapshot transformSnapshot)
        {
            return transformSnapshot._worldSpacePosition == _worldSpacePosition &&
                   transformSnapshot._worldSpaceRotation == _worldSpaceRotation &&
                   transformSnapshot._worldSpaceScale == _worldSpaceScale;
        }
        #endregion
    }
}
#endif