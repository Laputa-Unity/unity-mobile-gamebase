#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagDatabaseViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _tagScrollPosition = Vector2.zero;
        [SerializeField]
        private string _nameForNewTag = "";
        [SerializeField]
        private bool _allowTagNameChange = false;
        #endregion

        #region Public Static Properties
        public static float TagScrollViewHeight { get { return 200.0f; } }
        #endregion

        #region Public Properties
        public Vector2 TagScrollPosition { get { return _tagScrollPosition; } set { _tagScrollPosition = value; } }
        public string NameForNewTag { get { return _nameForNewTag; } set { if (value != null) _nameForNewTag = value; } }
        public bool AllowTagNameChange { get { return _allowTagNameChange; } set { _allowTagNameChange = value; } }
        #endregion
    }
}
#endif