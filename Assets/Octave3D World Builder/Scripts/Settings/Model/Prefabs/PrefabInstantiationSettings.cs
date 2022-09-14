#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabInstantiationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private PrefabInstantiationSettingsView _view;
        #endregion

        #region Public Properties
        public PrefabInstantiationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabInstantiationSettings()
        {
            _view = new PrefabInstantiationSettingsView(this);
        }
        #endregion
    }
}
#endif