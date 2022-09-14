#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectGroup : ScriptableObject, INamedEntity
    {
        #region Private Variables
        [SerializeField]
        private string _name;
        [SerializeField]
        private GameObject _groupObject;

        [SerializeField]
        private ObjectGroupView _view;
        #endregion

        #region Public Properties
        public string Name
        { 
            get { return _name; } 
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _name = value;
                    if (_groupObject != null) _groupObject.name = value;
                }
            } 
        }
        public GameObject GroupObject
        { 
            get 
            {
                // Note: This seems like the best place to ensure that the object's name is
                //       the same as the group name. Names can differ when the user changes
                //       the name via the GUI and then performs and Undo. Another solution
                //       would be to capture the Undo event and adjust the name there.
                if (_groupObject != null && _groupObject.name != _name) _groupObject.name = _name;
                return _groupObject;
            } 
            set 
            {
                if (value != null)
                {
                    _groupObject = value;
                    _name = _groupObject.name;
                }
            } 
        }
        public ObjectGroupView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectGroup()
        {
            _view = new ObjectGroupView(this);
        }
        #endregion
    }
}
#endif