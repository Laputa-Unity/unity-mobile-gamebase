#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class Prefab : INamedEntity
    {
        #region Private Variables
        [SerializeField]
        private GameObject _unityPrefab;
        [SerializeField]
        private Quaternion _initialWorldRotation;
        [SerializeField]
        private Vector3 _initialWorldScale;

        [SerializeField]
        private PrefabInstantiationSettings _instantiationSettings;
        [SerializeField]
        private PrefabActivationSettings _activationSettings;
        [SerializeField]
        private PrefabTagAssociations _tagAssociations;

        // Note: This may seem redundant since we already have access to the prefab's name via '_unityPrefab'. However,
        //       this variable is still needed in order to be able to undo/redo changes of the prefab's name via the
        //       inspector GUI.
        [SerializeField]
        private string _name = "";

        [SerializeField]
        private float _offsetFromGridSurface = 0.0f;
        [SerializeField]
        private float _offsetFromObjectSurface = 0.0f;

        [SerializeField]
        private PrefabView _view;
        #endregion

        #region Public Properties
        public PrefabInstantiationSettings InstantiationSettings
        {
            get
            {
                if (_instantiationSettings == null) _instantiationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabInstantiationSettings>();
                return _instantiationSettings;
            }
        }
        public PrefabActivationSettings ActivationSettings
        {
            get
            {
                if (_activationSettings == null) _activationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabActivationSettings>();
                return _activationSettings;
            }
        }
        public GameObject UnityPrefab
        { 
            get { return _unityPrefab; }
            set 
            { 
                if (value != null)
                {
                    ActivationSettings.WorldScale = value.transform.lossyScale;
                    ActivationSettings.WorldRotation = value.transform.rotation;
                    _initialWorldScale = ActivationSettings.WorldScale;
                    _initialWorldRotation = ActivationSettings.WorldRotation;

                    _unityPrefab = value;
                    _name = _unityPrefab.name;
                }
            } 
        }
        public Vector3 InitialWorldScale { get { return _initialWorldScale; } }
        public Quaternion InitialWorldRotation { get { return _initialWorldRotation; } }

        public string Name 
        { 
            get 
            {
                // Note: When the value of '_name' changes via Undo/Redo, this ensures that the 'name' property
                //       of the unity prefab is always up to date. 
                // Note: This is a dangerous thing to do because it can break prefab instantiation. When the name
                //       of the prefab changes, it seems that objects instantiated from that prefab can not have
                //       their transform modified accordingly. However, making sure that we only change the name
                //       when it's trully necessary, seems to fix it. A call to 'SetDirty' is also performed to
                //       ensure that Unity knows the preab has been modified. Probably not necessary but it seems
                //       like the right thing to do :).
                if (_unityPrefab != null && _unityPrefab.name != _name)
                {
                    _unityPrefab.name = _name;
                    //EditorUtility.SetDirty(_unityPrefab);
                }
                return _name;
            } 
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    //_unityPrefab.name = value;        
                    //EditorUtility.SetDirty(_unityPrefab);
                    _name = _unityPrefab.name;
                } 
            } 
        }      

        public PrefabTagAssociations TagAssociations
        {
            get
            {
                if (_tagAssociations == null)
                {
                    _tagAssociations = PrefabTagAssociationsFactory.Create(this);
                    _tagAssociations.View.VisibilityToggleIndent = 1;
                }
                return _tagAssociations;
            }
        }

        public float OffsetFromGridSurface { get { return _offsetFromGridSurface; } set { _offsetFromGridSurface = value; } }
        public float OffsetFromObjectSurface { get { return _offsetFromObjectSurface; } set { _offsetFromObjectSurface = value; } }

        public PrefabView View { get { return _view; } }
        #endregion

        #region Constructors
        public Prefab()
        {
            _view = new PrefabView(this);
        }
        #endregion
    }
}
#endif