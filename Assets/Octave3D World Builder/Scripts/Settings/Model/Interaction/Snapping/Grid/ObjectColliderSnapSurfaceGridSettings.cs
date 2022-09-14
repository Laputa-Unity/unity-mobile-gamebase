#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectColliderSnapSurfaceGridSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _desiredCellSize = 0.2f;

        [SerializeField]
        private ObjectColliderSnapSurfaceGridSettingsView _view;
        #endregion

        #region Public Static Functions
        public static float MinDesiredCellSize { get { return XZGridCellSizeSettings.MinCellSize; } }
        #endregion

        #region Public Properties
        public float DesiredCellSize { get { return _desiredCellSize; } set { _desiredCellSize = Mathf.Max(value, MinDesiredCellSize); } }
        public ObjectColliderSnapSurfaceGridSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectColliderSnapSurfaceGridSettings()
        {
            _view = new ObjectColliderSnapSurfaceGridSettingsView(this);
        }
        #endregion
    }
}
#endif