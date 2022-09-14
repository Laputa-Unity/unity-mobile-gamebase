#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSnapSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _useOriginalPivot = false;
        [SerializeField]
        private bool _enableObjectSurfaceGrid = true;
        [SerializeField]
        private bool _snapToCursorHitPoint = false;
        [SerializeField]
        private bool _snapCenterToCenterForXZGrid = false;
        [SerializeField]
        private bool _snapCenterToCenterForObjectSurface = false;

        [SerializeField]
        private bool _enableObjectToObjectSnap = false;
        [SerializeField]
        private float _objectToObjectSnapEpsilon = 0.5f;

        [SerializeField]
        private float _xzGridYOffsetStep = 1.0f;

        [SerializeField]
        private ObjectColliderSnapSurfaceGridSettings _objectColliderSnapSurfaceGridSettings;

        [SerializeField]
        private ObjectSnapSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinObjectToObjectSnapEpsilon { get { return 1e-5f; } }
        public static float MaxObjectToObjectSnapEpsilon { get { return 1.0f; } }
        public static float MinXzGridYOffsetStep { get { return 1e-3f; } }
        #endregion

        #region Public Properties
        public bool UseOriginalPivot { get { return _useOriginalPivot; } set { _useOriginalPivot = value; } }
        public bool EnableObjectSurfaceGrid { get { return _enableObjectSurfaceGrid; } set { _enableObjectSurfaceGrid = value; } }
        public bool SnapToCursorHitPoint { get { return _snapToCursorHitPoint; } set { _snapToCursorHitPoint = value; } }
        public bool SnapCenterToCenterForXZGrid { get { return _snapCenterToCenterForXZGrid; } set { _snapCenterToCenterForXZGrid = value; } }
        public bool SnapCenterToCenterForObjectSurface { get { return _snapCenterToCenterForObjectSurface; } set { _snapCenterToCenterForObjectSurface = value; } }
        public bool EnableObjectToObjectSnap { get { return _enableObjectToObjectSnap; } set { _enableObjectToObjectSnap = value; } }
        public float ObjectToObjectSnapEpsilon { get { return _objectToObjectSnapEpsilon; } set { _objectToObjectSnapEpsilon = Mathf.Clamp(value, MinObjectToObjectSnapEpsilon, MaxObjectToObjectSnapEpsilon); } }
        public float XZGridYOffsetStep { get { return _xzGridYOffsetStep; } set { _xzGridYOffsetStep = Mathf.Max(value, MinXzGridYOffsetStep); } }
        public float XZSnapGridXOffset
        {
            get { return ObjectSnapping.Get().XZSnapGrid.GetOriginPosition().x; }
            set
            {
                UndoEx.RecordForToolAction(ObjectSnapping.Get().XZSnapGrid);
                ObjectSnapping.Get().XZSnapGrid.SetXOriginPosition(value);
            }
        }
        public float XZSnapGridYOffset 
        { 
            get 
            {
                XZGrid xzGrid = ObjectSnapping.Get().XZSnapGrid;
                return xzGrid.GetOriginPosition().y;
            }
            set 
            {
                XZGrid xzGrid = ObjectSnapping.Get().XZSnapGrid;
                UndoEx.RecordForToolAction(xzGrid);
                ObjectSnapping.Get().XZSnapGrid.SetYOriginPosition(value); 
            }
        }
        public float XZSnapGridZOffset
        {
            get { return ObjectSnapping.Get().XZSnapGrid.GetOriginPosition().z; }
            set
            {
                UndoEx.RecordForToolAction(ObjectSnapping.Get().XZSnapGrid);
                ObjectSnapping.Get().XZSnapGrid.SetZOriginPosition(value);
            }
        }
        public ObjectColliderSnapSurfaceGridSettings ObjectColliderSnapSurfaceGridSettings
        {
            get
            {
                if (_objectColliderSnapSurfaceGridSettings == null) _objectColliderSnapSurfaceGridSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectColliderSnapSurfaceGridSettings>();
                return _objectColliderSnapSurfaceGridSettings;
            }
        }
        public ObjectSnapSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectSnapSettings()
        {
            _view = new ObjectSnapSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectSnapSettings Get()
        {
            return ObjectSnapping.Get().Settings;
        }
        #endregion
    }
}
#endif