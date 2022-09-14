#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectEraserSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectEraseMode _eraseMode = ObjectEraseMode.HoveredObject;

        [SerializeField]
        private bool _allowUndoRedo = true;
        [SerializeField]
        private float _eraseDelayInSeconds = 0.01f;
        [SerializeField]
        private bool _eraseOnlyMeshObjects = true;
        [SerializeField]
        private Object2DMassEraseSettings _mass2DEraseSettings;
        [SerializeField]
        private Object3DMassEraseSettings _mass3DEraseSettings;

        [SerializeField]
        private ObjectEraserSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinEraseDelayInSeconds { get { return 0.0f; } }
        public static float MaxEraseDelayInSeconds { get { return 1.0f; } }
        #endregion

        #region Public Properties
        public ObjectEraseMode EraseMode { get { return _eraseMode; } set { _eraseMode = value; } }
        public bool AllowUndoRedo { get { return _allowUndoRedo; } set { _allowUndoRedo = value; } }
        public float EraseDelayInSeconds { get { return _eraseDelayInSeconds; } set { _eraseDelayInSeconds = Mathf.Clamp(value, MinEraseDelayInSeconds, MaxEraseDelayInSeconds); } }
        public bool EraseOnlyMeshObjects { get { return _eraseOnlyMeshObjects; } set { _eraseOnlyMeshObjects = value; } }
        public Object2DMassEraseSettings Mass2DEraseSettings
        {
            get
            {
                if (_mass2DEraseSettings == null) _mass2DEraseSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<Object2DMassEraseSettings>();
                return _mass2DEraseSettings;
            }
        }
        public Object3DMassEraseSettings Mass3DEraseSettings
        {
            get
            {
                if (_mass3DEraseSettings == null) _mass3DEraseSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<Object3DMassEraseSettings>();
                return _mass3DEraseSettings;
            }
        }
        public ObjectEraserSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectEraserSettings()
        {
            _view = new ObjectEraserSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectEraserSettings Get()
        {
            return ObjectEraser.Get().Settings;
        }
        #endregion
    }
}
#endif
