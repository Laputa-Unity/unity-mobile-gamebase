#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabInstantiationSettingsView : SettingsView
    {
        #region Private Variables
       // [NonSerialized]
        //private PrefabInstantiationSettings _settings;
        #endregion

        #region Constructors
        public PrefabInstantiationSettingsView(PrefabInstantiationSettings settings)
        {
            //_settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
#endif