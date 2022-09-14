#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPivotPointsRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPivotPointShapeType _shapeType = ObjectPivotPointShapeType.Circle;

        [SerializeField]
        private float _pivotPointSizeInPixels = 10.0f;

        [SerializeField]
        private ProjectedBoxFacePivotPointsRenderSettings _projectedBoxFacePivotPointsRenderSettings;

        [SerializeField]
        private ObjectPivotPointsRenderSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinPivotPointSize { get { return 2.0f; } }
        #endregion

        #region Public Properties
        public ObjectPivotPointShapeType ShapeType { get { return _shapeType; } set { _shapeType = value; } }
        public float PivotPointSizeInPixels { get { return _pivotPointSizeInPixels; } set { _pivotPointSizeInPixels = Mathf.Max(MinPivotPointSize, value); } }
        public ProjectedBoxFacePivotPointsRenderSettings ProjectedBoxFacePivotPointsRenderSettings
        {
            get
            {
                if (_projectedBoxFacePivotPointsRenderSettings == null) _projectedBoxFacePivotPointsRenderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ProjectedBoxFacePivotPointsRenderSettings>();
                return _projectedBoxFacePivotPointsRenderSettings;
            }
        }
        public ObjectPivotPointsRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPivotPointsRenderSettings()
        {
            _view = new ObjectPivotPointsRenderSettingsView(this);
        }
        #endregion
    }
}
#endif