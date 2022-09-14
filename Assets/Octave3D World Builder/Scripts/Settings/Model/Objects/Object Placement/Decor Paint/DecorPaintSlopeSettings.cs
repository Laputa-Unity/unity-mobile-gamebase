#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintSlopeSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private float _minSlopeInDegrees = 0.0f;
        [SerializeField]
        private float _maxSlopeInDegrees = 90.0f;
        [SerializeField]
        private bool _useSlopeOnlyForTerrainObjects = true;

        [SerializeField]
        private DecorPaintSlopeSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinSlopeValueInDegrees { get { return 0.0f; } }
        public static float MaxSlopeValueInDegrees { get { return 90.0f; } }
        #endregion

        #region Public Properties
        public float MinSlopeInDegrees { get { return _minSlopeInDegrees; } set { _minSlopeInDegrees = Mathf.Min(Mathf.Clamp(value, MinSlopeValueInDegrees, MaxSlopeValueInDegrees), _maxSlopeInDegrees); } }
        public float MaxSlopeInDegrees { get { return _maxSlopeInDegrees; } set { _maxSlopeInDegrees = Mathf.Max(Mathf.Clamp(value, MinSlopeValueInDegrees, MaxSlopeValueInDegrees), _minSlopeInDegrees); } }
        public bool UseSlopeOnlyForTerrainObjects { get { return _useSlopeOnlyForTerrainObjects; } set { _useSlopeOnlyForTerrainObjects = value; } }
        public DecorPaintSlopeSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public DecorPaintSlopeSettings()
        {
            _view = new DecorPaintSlopeSettingsView(this);
        }
        #endregion

        #region Public Methods
        public bool IsNormalInSlopeRange(Vector3 normal)
        {
            float angleInDegreesWithUpVector = Vector3.up.AngleWith(normal);
            return angleInDegreesWithUpVector >= _minSlopeInDegrees && angleInDegreesWithUpVector <= _maxSlopeInDegrees;
        }
        #endregion
    }
}
#endif