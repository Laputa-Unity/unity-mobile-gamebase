#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectGroupDatabaseViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private string _nameForNewGroup = "";
        private GameObject _replacement;
        #endregion

        #region Public Properties
        public string NameForNewGroup { get { return _nameForNewGroup; } set { if (value != null) _nameForNewGroup = value; } }
        public GameObject Replacement { get { return _replacement; } set { _replacement = value; } }
        #endregion
    }
}
#endif