#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PersistentObjectPlacementGuideData
    {
        #region Private Variables
        [SerializeField]
        private Vector3 _lastUsedWorldPosition = Vector3.zero;
        #endregion

        #region Public Properties
        public Vector3 LastUsedWorldPosition { get { return _lastUsedWorldPosition; } set { _lastUsedWorldPosition = value; } }
        #endregion

        #region Public Static Functions
        public static PersistentObjectPlacementGuideData Get()
        {
            return ObjectPlacement.Get().PersistentObjectPlacementGuideData;
        }
        #endregion
    }
}
#endif