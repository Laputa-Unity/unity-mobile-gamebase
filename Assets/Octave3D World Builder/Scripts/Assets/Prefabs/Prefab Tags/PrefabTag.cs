#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabTag : ScriptableObject, INamedEntity
    {
        #region Private Variables
        [SerializeField]
        private string _name = "";

        [SerializeField]
        private bool _isActive = true;
       
        [SerializeField]
        private PrefabTagView _view;
        #endregion

        #region Public Properties
        public string Name { get { return _name; } set { if (!string.IsNullOrEmpty(value)) _name = value; } }
        public bool IsActive 
        { 
            get { return _isActive; }
            set 
            {
                _isActive = value;
                PrefabTagActiveStateWasChangedMessage.SendToInterestedListeners(this);
            } 
        }
        public PrefabTagView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabTag()
        {
            _view = new PrefabTagView(this);
        }
        #endregion
    }
}
#endif