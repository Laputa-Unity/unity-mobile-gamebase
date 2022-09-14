#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionActionsViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private int _selectionAssignmentLayer = LayerExtensions.GetDefaultLayer();
        [SerializeField]
        private string _destObjectGroupName = "";
        #endregion

        #region Public Properties
        public int SelectionAssignmentLayer { get { return _selectionAssignmentLayer; } set { if (LayerExtensions.IsLayerNumberValid(value)) _selectionAssignmentLayer = value; } }
        public string DestObjectGroupName { get { return _destObjectGroupName; } set { if (!string.IsNullOrEmpty(value)) _destObjectGroupName = value; } }
        #endregion
    }
}
#endif