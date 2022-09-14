#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class CameraDataSnapshot
    {
        #region Private Variables
        private TransformSnapshot _transformSnapshot;
        private CameraType _cameraType;
        #endregion

        #region Public Methods
        public void TakeSnapshot(Camera camera)
        {
            _transformSnapshot = new TransformSnapshot();
            _transformSnapshot.TakeSnapshot(camera.transform);
            _cameraType = camera.orthographic ? CameraType.Orthographic : CameraType.Perspective;
        }

        public override bool Equals(object value)
        {
            if (ReferenceEquals(value, null)) return false;
            if (ReferenceEquals(value, this)) return true;

            if (value.GetType() != this.GetType()) return false;
            return IsEqual(value as CameraDataSnapshot);
        }

        public bool Equals(CameraDataSnapshot cameraDataSnapshot)
        {
            if (ReferenceEquals(cameraDataSnapshot, null)) return false;
            if (ReferenceEquals(cameraDataSnapshot, this)) return true;

            return IsEqual(cameraDataSnapshot);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + _transformSnapshot.GetHashCode();
            hash = (hash * 7) + _cameraType.GetHashCode();

            return hash;
        }

        public static bool operator ==(CameraDataSnapshot firstSnapshot, CameraDataSnapshot secondSnapshot)
        {
            return !ReferenceEquals(firstSnapshot, null) &&
                   !ReferenceEquals(secondSnapshot, null) &&
                   (ReferenceEquals(firstSnapshot, secondSnapshot) || firstSnapshot.IsEqual(secondSnapshot));
        }

        public static bool operator !=(CameraDataSnapshot firstSnapshot, CameraDataSnapshot secondSnapshot)
        {
            return !(firstSnapshot == secondSnapshot);
        }
        #endregion

        #region Private Methods
        private bool IsEqual(CameraDataSnapshot cameraDataSnapshot)
        {
            return _transformSnapshot == cameraDataSnapshot._transformSnapshot &&
                   _cameraType == cameraDataSnapshot._cameraType;
        }
        #endregion
    }
}
#endif