#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectMouseMoveAlongDirectionSession
    {
        #region Private Variables
        private GameObject _gameObject;
        private Transform _gameObjectTransform;
        private Vector3 _objectPositionAtSessionBegin;

        private Vector2 _cursorPosAtSessionStart;
        private Vector3 _normalizedMoveDirection;
        private bool _isActive = false;
        private ObjectMouseMoveAlongDirectionSettings _settings;
        #endregion

        #region Public Properties
        public bool IsActive { get { return _isActive; } }
        #endregion

        #region Public Methods
        public void Begin(GameObject gameObject, Vector3 moveDirection, ObjectMouseMoveAlongDirectionSettings settings)
        {
            if (_isActive || moveDirection.magnitude == 0 || gameObject == null) return;
       
            _gameObject = gameObject;
            _gameObjectTransform = gameObject.transform;
            _objectPositionAtSessionBegin = _gameObjectTransform.position;

            _settings = settings;

            _cursorPosAtSessionStart = MouseCursor.Instance.Position;
            _normalizedMoveDirection = moveDirection;
            _normalizedMoveDirection.Normalize();

            _isActive = true;
        }

        public void End()
        {
            _isActive = false;
        }

        public void UpdateForMouseMovement(Event e)
        {
            if(_isActive && _gameObject != null)
            {
                Vector2 currentCursorPos = MouseCursor.Instance.Position;
                Vector2 fromPosAtSessionStartToCurrentPos = currentCursorPos - _cursorPosAtSessionStart;

                UndoEx.RecordForToolAction(_gameObjectTransform);
                _gameObjectTransform.position = _objectPositionAtSessionBegin + _normalizedMoveDirection * (_settings.MouseSensitivity * fromPosAtSessionStartToCurrentPos.x);
            }
        }
        #endregion
    }
}
#endif