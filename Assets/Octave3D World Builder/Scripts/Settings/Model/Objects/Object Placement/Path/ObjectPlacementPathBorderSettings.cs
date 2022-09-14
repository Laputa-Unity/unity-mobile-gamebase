#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathBorderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _useBorders = false;

        [SerializeField]
        private int _beginBorderWidth = 1;
        [SerializeField]
        private int _endBorderWidth = 1;
        [SerializeField]
        private int _bottomBorderWidth = 1;
        [SerializeField]
        private int _topBorderWidth = 1;

        [SerializeField]
        private ObjectPlacementPathBorderSettingsView _view;
        #endregion

        #region Public Static Functions
        public static int MinBorderWidth { get { return 0; } }
        #endregion

        #region Public Properties
        public bool UseBorders 
        { 
            get { return _useBorders; } 
            set 
            { 
                _useBorders = value;
                ObjectPlacementPathBorderSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int BeginBorderWidth 
        { 
            get { return _beginBorderWidth; } 
            set 
            { 
                _beginBorderWidth = Mathf.Max(value, MinBorderWidth);
                ObjectPlacementPathBorderSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int EndBorderWidth 
        { 
            get { return _endBorderWidth; } 
            set 
            { 
                _endBorderWidth = Mathf.Max(value, MinBorderWidth);
                ObjectPlacementPathBorderSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int BottomBorderWidth 
        { 
            get { return _bottomBorderWidth; } 
            set 
            { 
                _bottomBorderWidth = Mathf.Max(value, MinBorderWidth);
                ObjectPlacementPathBorderSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int TopBorderWidth 
        {
            get { return _topBorderWidth; } 
            set 
            { 
                _topBorderWidth = Mathf.Max(value, MinBorderWidth);
                ObjectPlacementPathBorderSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public ObjectPlacementPathBorderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathBorderSettings()
        {
            _view = new ObjectPlacementPathBorderSettingsView(this);
        }
        #endregion
    }
}
#endif