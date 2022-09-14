#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushDatabaseViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private string _nameForNewBrush = "";
        #endregion

        #region Public Properties
        public string NameForNewBrush { get { return _nameForNewBrush; } set { if (value != null) _nameForNewBrush = value; } }
        #endregion
    }
}
#endif