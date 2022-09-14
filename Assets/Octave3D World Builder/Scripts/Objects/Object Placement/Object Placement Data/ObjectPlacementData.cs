#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementData
    {
        #region Private Variables
        private Vector3 _worldPosition;
        private Vector3 _worldScale;
        private Quaternion _worldRotation;
        private Prefab _prefab;
        #endregion

        #region Public Properties
        public Vector3 WorldPosition { get { return _worldPosition; } set { _worldPosition = value; } }
        public Vector3 WorldScale { get { return _worldScale; } set { _worldScale = value; } }
        public Quaternion WorldRotation { get { return _worldRotation; } set { _worldRotation = value; } }
        public Prefab Prefab { get { return _prefab; } set {_prefab = value; } }
        #endregion

        #region Public Constructors
        public ObjectPlacementData()
        {
        }

        public ObjectPlacementData(TransformMatrix transformMatrix, Prefab prefab)
        {
            _worldPosition = transformMatrix.Translation;
            _worldRotation = transformMatrix.Rotation;
            _worldScale = transformMatrix.Scale;
            _prefab = prefab;
        }
        #endregion
    }
}
#endif