#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class SelectionObject2ObjectSnapSettings
    {
        [SerializeField]
        private float _snapEps = 0.5f;
        [SerializeField]
        private bool _canHoverObjects = true;

        public float SnapEps { get { return _snapEps; } set { _snapEps = Mathf.Max(1e-3f, value); } }
        public bool CanHoverObjects { get { return _canHoverObjects; } set { _canHoverObjects = value; } }
    }

    [Serializable]
    public class SelectionObjectGroupSettings
    {
        [SerializeField]
        private bool _attachToObjectGroup = false;
        [SerializeField]
        private ObjectGroup _destinationGroup = null;

        public bool AttachToObjectGroup { get { return _attachToObjectGroup; } set { _attachToObjectGroup = value; } }
        public ObjectGroup DestinationGroup { get { return _destinationGroup; } set { _destinationGroup = value; } }
    }

    [Serializable]
    public class ObjectSelectionSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectSelectionShapeType _selectionShapeType = ObjectSelectionShapeType.Rectangle;
        [SerializeField]
        private ObjectSelectionMode _selectionMode = ObjectSelectionMode.Standard;
        [SerializeField]
        private ObjectSelectionUpdateMode _selectionUpdateMode = ObjectSelectionUpdateMode.EntireHierarchy;

        [SerializeField]
        private SelectionObjectGroupSettings _objectGroupSettings = new SelectionObjectGroupSettings();
        [SerializeField]
        private SelectionObject2ObjectSnapSettings _object2ObjectSnapSettings = new SelectionObject2ObjectSnapSettings();

        [SerializeField]
        private float _xRotationStep = 90.0f;
        [SerializeField]
        private float _yRotationStep = 90.0f;
        [SerializeField]
        private float _zRotationStep = 90.0f;
        [SerializeField]
        private bool _rotateAroundSelectionCenter = true;

        [SerializeField]
        private bool _allowPartialOverlap = true;
        [SerializeField]
        private ObjectSelectionPaintModeSettings _paintModeSettings;

        [SerializeField]
        private ObjectSelectionSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectSelectionShapeType SelectionShapeType { get { return _selectionShapeType; } set { _selectionShapeType = value; } }
        public ObjectSelectionMode SelectionMode { get { return _selectionMode; } set { _selectionMode = value; } }
        public ObjectSelectionUpdateMode SelectionUpdateMode { get { return _selectionUpdateMode; } set { _selectionUpdateMode = value; } }
        public SelectionObjectGroupSettings ObjectGroupSettings { get { return _objectGroupSettings; } }
        public SelectionObject2ObjectSnapSettings Object2ObjectSnapSettings { get { return _object2ObjectSnapSettings; } }
        public float XRotationStep { get { return _xRotationStep; } set { _xRotationStep = Mathf.Clamp(value, 1e-2f, 360.0f); } }
        public float YRotationStep { get { return _yRotationStep; } set { _yRotationStep = Mathf.Clamp(value, 1e-2f, 360.0f); } }
        public float ZRotationStep { get { return _zRotationStep; } set { _zRotationStep = Mathf.Clamp(value, 1e-2f, 360.0f); } }
        public bool RotateAroundSelectionCenter { get { return _rotateAroundSelectionCenter; } set { _rotateAroundSelectionCenter = value; } }
        public bool AllowPartialOverlap { get { return _allowPartialOverlap; } set { _allowPartialOverlap = value; } }
        public ObjectSelectionPaintModeSettings PaintModeSettings
        {
            get
            {
                if (_paintModeSettings == null) _paintModeSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectSelectionPaintModeSettings>();
                return _paintModeSettings;
            }
        }
        public ObjectSelectionSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectSelectionSettings()
        {
            _view = new ObjectSelectionSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectSelectionSettings Get()
        {
            return ObjectSelection.Get().Settings;
        }
        #endregion
    }
}
#endif