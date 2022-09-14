#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    public enum ObjectIntersectPrecision
    {
        Box = 0,
        Mesh
    }

    [Serializable]
    public class ObjectIntersectionSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _allowIntersectionForDecorPaintSingleModeDrag = true;
        [SerializeField]
        private bool _allowIntersectionForDecorPaintBrushModeDrag = true;
        [SerializeField]
        private bool _allowIntersectionForPathPlacement = true;
        [SerializeField]
        private bool _allowIntersectionForBlockPlacement = true;

        [SerializeField]
        private ObjectIntersectionSettingsView _view;
        #endregion

        #region Public Properties
        public bool AllowIntersectionForDecorPaintSingleModeDrag { get { return _allowIntersectionForDecorPaintSingleModeDrag; } set { _allowIntersectionForDecorPaintSingleModeDrag = value; } }
        public bool AllowIntersectionForDecorPaintBrushModeDrag { get { return _allowIntersectionForDecorPaintBrushModeDrag; } set { _allowIntersectionForDecorPaintBrushModeDrag = value; } }
        public bool AllowIntersectionForPathPlacement { get { return _allowIntersectionForPathPlacement; } set { _allowIntersectionForPathPlacement = value; } }
        public bool AllowIntersectionForBlockPlacement { get { return _allowIntersectionForBlockPlacement; } set { _allowIntersectionForBlockPlacement = value; } }
        public ObjectIntersectionSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectIntersectionSettings()
        {
            _view = new ObjectIntersectionSettingsView(this);
        }
        #endregion
    }
}
#endif