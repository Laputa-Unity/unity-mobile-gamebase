#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabActivationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Quaternion _worldRotation = Quaternion.identity;
        [SerializeField]
        private Vector3 _worldScale = Vector3.one;

        [SerializeField]
        private PrefabActivationSettingsView _view;
        #endregion

        #region Public Properties
        public Quaternion WorldRotation { get { return _worldRotation; } set { _worldRotation = value; } }
        public Vector3 WorldScale { get { return _worldScale; } set { _worldScale = value; } }
        public PrefabActivationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabActivationSettings()
        {
            _view = new PrefabActivationSettingsView();
        }
        #endregion
    }
}
#endif