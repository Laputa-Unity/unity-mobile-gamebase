#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class MirroredRotation
    {
        #region Private Variables
        private Quaternion _rotation;
        private Vector3 _axesScale;
        #endregion

        #region Public Properties
        public Quaternion Rotation { get { return _rotation; } }
        public Vector3 AxesScale { get { return _axesScale; } }
        #endregion

        #region Constructors
        public MirroredRotation(Quaternion rotation, Vector3 axesScale)
        {
            _rotation = rotation;
            _axesScale = axesScale;
        }
        #endregion
    }
}
#endif