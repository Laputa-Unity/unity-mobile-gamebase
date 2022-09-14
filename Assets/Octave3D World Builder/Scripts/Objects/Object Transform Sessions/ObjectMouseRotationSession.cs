#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectMouseRotationSession
    {
        #region Private Variables
        private TransformAxis _rotationAxis;

        private bool _isActive;
        private GameObject _gameObject;
        private Transform _gameObjectTransform;

        private ObjectMouseRotationSettings _mouseRotationSettings;
        private bool _rotatingAroundCustomAxis;
        private Vector3 _customRotationAxis;
        private float _accumulatedRotationInDegrees;
        #endregion

        #region Public Properties
        public bool IsActive { get { return _isActive; } }
        public TransformAxis RotationAxis { get { return _rotationAxis; } }
        public bool RotatingAroundCustomAxis { get { return _rotatingAroundCustomAxis; } }
        #endregion

        #region Public Methods
        public void BeginRotationAroundAxis(GameObject gameObject, ObjectMouseRotationSettings rotationSettings, TransformAxis rotationAxis)
        {
            if (gameObject != null && !_isActive)
            {
                _isActive = true;
                _gameObject = gameObject;
                _gameObjectTransform = _gameObject.transform;

                _mouseRotationSettings = rotationSettings;
                _rotationAxis = rotationAxis;
                _rotatingAroundCustomAxis = false;
                _accumulatedRotationInDegrees = 0.0f;
            }
        }

        public void BeginRotationAroundCustomAxis(GameObject gameObject, ObjectMouseRotationSettings rotationSettings, Vector3 customRotationAxis)
        {
            if (gameObject != null && !_isActive)
            {
                _isActive = true;
                _gameObject = gameObject;
                _gameObjectTransform = _gameObject.transform;

                _mouseRotationSettings = rotationSettings;
                _rotatingAroundCustomAxis = true;
                _customRotationAxis = customRotationAxis;
                _accumulatedRotationInDegrees = 0.0f;
            }
        }

        public void End()
        {
            _isActive = false;
        }

        public void UpdateForMouseMovement(Event e)
        {
            if (_gameObject != null && _isActive)
            {
                UndoEx.RecordForToolAction(_gameObjectTransform);
                if (_rotatingAroundCustomAxis) RotateObjectAroundCustomAxis();
                else RotateObjectAroundAxis();
            }
        }
        #endregion

        #region Private Methods
        private void RotateObjectAroundAxis()
        {
            AxisMouseRotationSettings axisMouseRotationSettings = _mouseRotationSettings.XAxisRotationSettings;
            if (_rotationAxis == TransformAxis.Y) axisMouseRotationSettings = _mouseRotationSettings.YAxisRotationSettings;
            else if (_rotationAxis == TransformAxis.Z) axisMouseRotationSettings = _mouseRotationSettings.ZAxisRotationSettings;

            float rotationAmountInDegrees = MouseCursor.Instance.OffsetSinceLastMouseMove.x * axisMouseRotationSettings.MouseSensitivity;
            Vector3 rotationAxis = TransformAxes.GetVector(_rotationAxis, TransformSpace.Global, _gameObjectTransform);
            RotateObjectAroundAxis(rotationAxis, rotationAmountInDegrees);
        }

        private void RotateObjectAroundCustomAxis()
        {
            CustomAxisMouseRotationSettings customAxisRotationSettings = _mouseRotationSettings.CustomAxisRotationSettings;
            float rotationAmountInDegrees = MouseCursor.Instance.OffsetSinceLastMouseMove.x * customAxisRotationSettings.MouseSensitivity;
            RotateObjectAroundAxis(_customRotationAxis, rotationAmountInDegrees);
        }

        private void RotateObjectAroundAxis(Vector3 rotationAxis, float rotationAmountInDegrees)
        {
            OrientedBox hierarchyWorldOrientedBox = _gameObject.GetHierarchyWorldOrientedBox();
            if(hierarchyWorldOrientedBox.IsInvalid())
            {
                Box modelSpaceBox = new Box(Vector3.zero, Vector3.one * 0.001f);
                hierarchyWorldOrientedBox = new OrientedBox(modelSpaceBox, _gameObjectTransform);
            }
            if (_mouseRotationSettings.UseSnapping)
            {
                _accumulatedRotationInDegrees += rotationAmountInDegrees;
                float absAccumulatedRotationInDegrees = Mathf.Abs(_accumulatedRotationInDegrees);
                if (absAccumulatedRotationInDegrees >= _mouseRotationSettings.SnapStepInDegrees)
                {
                    float floatNumberOfSteps = absAccumulatedRotationInDegrees / _mouseRotationSettings.SnapStepInDegrees;
                    int integerNumberOfSteps = (int)floatNumberOfSteps;

                    rotationAmountInDegrees = integerNumberOfSteps * _mouseRotationSettings.SnapStepInDegrees * Mathf.Sign(_accumulatedRotationInDegrees);
                    _gameObject.RotateHierarchyBoxAroundPoint(rotationAmountInDegrees, rotationAxis, hierarchyWorldOrientedBox.Center);

                    _accumulatedRotationInDegrees = 0.0f;
                }
            }
            else
            {
                _accumulatedRotationInDegrees = 0.0f;
                _gameObject.RotateHierarchyBoxAroundPoint(rotationAmountInDegrees, rotationAxis, hierarchyWorldOrientedBox.Center);
            }
        }
        #endregion
    }
}
#endif