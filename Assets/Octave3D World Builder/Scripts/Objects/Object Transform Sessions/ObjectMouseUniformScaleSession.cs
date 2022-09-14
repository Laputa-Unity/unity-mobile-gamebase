#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectMouseUniformScaleSession
    {
        #region Private Variables
        private Vector2 _cursorPosAtSessionStart;
        private Vector3 _objectGlobalScaleAtSessionStart;

        private bool _scaleFromPivotPoint = true;
        private Vector3 _pivotPoint;
        private Vector3 _fromPivotPointToObjectPos;
        private float _accumulatedScale = 0.0f;

        private GameObject _gameObject;
        private Transform _gameObjectTransform;
        private bool _isActive;

        private ObjectMouseUniformScaleSettings _uniformScaleSettings;
        #endregion

        #region Public Properties
        public bool IsActive { get { return _isActive; } }
        public bool ScaleFromPivotPoint { get { return _scaleFromPivotPoint; } set { _scaleFromPivotPoint = value; } }
        public Vector3 PivotPoint { get { return _pivotPoint; } set { _pivotPoint = value; } }
        #endregion

        #region Public Methods
        public void Begin(GameObject gameObject, ObjectMouseUniformScaleSettings uniformScaleSettings)
        {
            if (gameObject != null && !_isActive)
            {
                _isActive = true;
                _gameObject = gameObject;
                _cursorPosAtSessionStart = MouseCursor.Instance.Position;

                _gameObject = gameObject;
                _gameObjectTransform = gameObject.transform;
                _uniformScaleSettings = uniformScaleSettings;
                _accumulatedScale = 0.0f;

                _objectGlobalScaleAtSessionStart = _gameObjectTransform.lossyScale;
                _fromPivotPointToObjectPos = _gameObjectTransform.position - _pivotPoint;
            }
        }

        public void End()
        {
            _isActive = false;
        }

        public void UpdateForMouseMovement(Event e)
        {
            if(_isActive && _gameObject != null)
            {
                UndoEx.RecordForToolAction(_gameObjectTransform);

                // In order to scale the object, we will calculate a scale factor which we will use to scale the
                // object's original scale vector (i.e. the one which was established when the session was started).
                // This scale factor is calculated by constructing a vector which goes from the position of the
                // mouse cursor as established at the start of the session to the current cursor position. The abs value
                // of the X component of the resulting vector represents the scale factor (which of course will also be 
                // scaled by the mouse sensitivity value) and the sign of the X component represents the sign of the scale.
                Vector2 currentCursorPos = MouseCursor.Instance.Position;
                Vector2 fromPosAtSessionStartToCurrentPos = currentCursorPos - _cursorPosAtSessionStart;

                float mouseSensitivityScale = CalculateMouseSensitivityScale();
                float mouseSensitivity = _uniformScaleSettings.MouseSensitivity * mouseSensitivityScale;

                // Calculate the amount which needs to be added to the scale factor
                float addToScale = fromPosAtSessionStartToCurrentPos.x * mouseSensitivity;
               
                // Using snapping?
                if (_uniformScaleSettings.UseSnapping)
                {
                    // Adjust the accumulated scale
                    _accumulatedScale += addToScale;

                    // Check if the accumulated scaled has reached its destination step value
                    if (HasAccumulatedScaleReachedSnapStep(mouseSensitivityScale))
                    {
                        float floateNumberOfSteps = Mathf.Abs(_accumulatedScale) / _uniformScaleSettings.SnapStep;
                        int integerNumberOfSteps = (int)floateNumberOfSteps;
                        float scaleFactor = (1.0f + integerNumberOfSteps * _uniformScaleSettings.SnapStep * Mathf.Sign(_accumulatedScale));

                        ScaleObject(scaleFactor);
                        _accumulatedScale = 0.0f;
                    }
                }
                else
                {
                    // Note: We will set this to 0 whenever we reach this point because the user may be
                    //       toggling snapping on/off during the same session and the next time snaping
                    //       is activated we want to start with a clean slate.
                    _accumulatedScale = 0.0f;

                    float scaleFactor = 1.0f + addToScale;
                    ScaleObject(scaleFactor);
                }
            }
        }
        #endregion

        #region Private Methods
        private bool HasAccumulatedScaleReachedSnapStep(float mouseSensitivityScale)
        {
            return Mathf.Abs(_accumulatedScale) >= (_uniformScaleSettings.SnapStep * mouseSensitivityScale);
        }

        private float CalculateMouseSensitivityScale()
        {
            // Note: We will adjust the mouse sensitivity such that it will decrease for scale values greater than 1
            //       and it will increase for scale values smaller than 1. So for example, if the object's scale vector
            //       is <3, 1, 1>, the mouse sensitivity will be divided by 3. This is necessary in order to make the
            //       scale process more controllable. Otherwise, if the scale of the object contained values which are 
            //       too big, the scale will just quickly snap to wild values and this is totally undesirable. Note that
            //       we are using the component of the object's scale vector which has the biggest absolute value.
            float maxAbsScaleComponent = Mathf.Abs(_objectGlobalScaleAtSessionStart.GetComponentWithBiggestAbsValue());
            if (maxAbsScaleComponent < 1e-5f) maxAbsScaleComponent = 0.001f;
            return 1.0f / maxAbsScaleComponent;
        }

        private void ScaleObject(float scaleFactor)
        {
            // Apply the calculated scale factor
            _gameObject.SetWorldScale(_objectGlobalScaleAtSessionStart * scaleFactor);

            // If we must scale from the pivot point, it means that the scale that we have just applied must
            // also be applied to the original relationship between the object's position and the specified 
            // pivot point.
            if (_scaleFromPivotPoint)
            {
                // Scale the relationship vector and adjust the object's position
                Vector3 fromPivotPointToObjectPosScaled = _fromPivotPointToObjectPos * scaleFactor;
                _gameObjectTransform.position = _pivotPoint + fromPivotPointToObjectPosScaled;
            }
        }
        #endregion
    }
}
#endif