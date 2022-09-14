#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockSubdivisionSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _useSubdivision = false;
        
        [SerializeField]
        private bool _subdivideAlongGrowDirection = true;
        [SerializeField]
        private int _subdivisionSizeAlongGrowDirection = 1;
        [SerializeField]
        private int _subdivisionGapSizeAlongGrowDirection = 1;

        [SerializeField]
        private bool _subdivideAlongExtensionRight = true;
        [SerializeField]
        private int _subdivisionSizeAlongExtensionRight = 1;
        [SerializeField]
        private int _subdivisionGapSizeAlongExtensionRight = 1;

        [SerializeField]
        private bool _subdivideAlongExtensionLook = true;
        [SerializeField]
        private int _subdivisionSizeAlongExtensionLook = 1;
        [SerializeField]
        private int _subdivisionGapSizeAlongExtensionLook = 1;

        [SerializeField]
        private ObjectPlacementBlockSubdivisionSettingsView _view;
        #endregion

        #region Public Static Properties
        public static int MinSubdivisionSize { get { return 1; } }
        public static int MinSubdivisionGapSize { get { return 1; } }
        #endregion

        #region Public Properties
        public bool UseSubdivision 
        { 
            get { return _useSubdivision; } 
            set 
            { 
                _useSubdivision = value;
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public bool SubdivideAlongGrowDirection 
        { 
            get { return _subdivideAlongGrowDirection; } 
            set 
            { 
                _subdivideAlongGrowDirection = value;
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int SubdivisionSizeAlongGrowDirection 
        { 
            get { return _subdivisionSizeAlongGrowDirection; } 
            set 
            { 
                _subdivisionSizeAlongGrowDirection = Mathf.Max(value, MinSubdivisionSize);
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int SubdivisionGapSizeAlongGrowDirection
        {
            get { return _subdivisionGapSizeAlongGrowDirection; }
            set
            {
                _subdivisionGapSizeAlongGrowDirection = Mathf.Max(value, MinSubdivisionGapSize);
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            }
        }
        public bool SubdivideAlongExtensionRight 
        { 
            get { return _subdivideAlongExtensionRight; } 
            set 
            { 
                _subdivideAlongExtensionRight = value;
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int SubdivisionSizeAlongExtensionRight 
        { 
            get { return _subdivisionSizeAlongExtensionRight; }
            set 
            { 
                _subdivisionSizeAlongExtensionRight = Mathf.Max(value, MinSubdivisionSize);
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int SubdivisionGapSizeAlongExtensionRight
        {
            get { return _subdivisionGapSizeAlongExtensionRight; }
            set
            {
                _subdivisionGapSizeAlongExtensionRight = Mathf.Max(value, MinSubdivisionGapSize);
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            }
        }
        public bool SubdivideAlongExtensionLook 
        { 
            get { return _subdivideAlongExtensionLook; } 
            set 
            { 
                _subdivideAlongExtensionLook = value;
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int SubdivisionSizeAlongExtensionLook 
        { 
            get { return _subdivisionSizeAlongExtensionLook; } 
            set 
            { 
                _subdivisionSizeAlongExtensionLook = Mathf.Max(value, MinSubdivisionSize);
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public int SubdivisionGapSizeAlongExtensionLook
        {
            get { return _subdivisionGapSizeAlongExtensionLook; }
            set
            {
                _subdivisionGapSizeAlongExtensionLook = Mathf.Max(value, MinSubdivisionGapSize);
                ObjectPlacementBlockSubdivisionSettingsWereChangedMessage.SendToInterestedListeners(this);
            }
        }
        public ObjectPlacementBlockSubdivisionSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockSubdivisionSettings()
        {
            _view = new ObjectPlacementBlockSubdivisionSettingsView(this);
        }
        #endregion
    }
}
#endif