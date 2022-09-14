#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    public class MouseButtonStates : Singleton<MouseButtonStates>
    {
        #region Private Variables
        private bool[] _mouseButtonStates = new bool[Enum.GetValues(typeof(MouseButton)).Length];
        #endregion

        #region Public Methods
        public void ClearStates()
        {
            _mouseButtonStates[0] = false;
            _mouseButtonStates[1] = false;
            _mouseButtonStates[2] = false;
        }

        public int GetNumberOfPressedButtons()
        {
            int count = 0;
            if (_mouseButtonStates[0]) ++count;
            if (_mouseButtonStates[1]) ++count;
            if (_mouseButtonStates[2]) ++count;

            return count;
        }

        public void OnMouseButtonPressed(MouseButton mouseButton)
        {
            _mouseButtonStates[(int)mouseButton] = true;
        }

        public void OnMouseButtonReleased(MouseButton mouseButton)
        {
            _mouseButtonStates[(int)mouseButton] = false;
        }

        public bool IsMouseButtonDown(MouseButton mouseButton)
        {
            return _mouseButtonStates[(int)mouseButton];
        }

        public bool IsOnlyThisMouseButtonDown(MouseButton mouseButton)
        {
            return GetNumberOfPressedButtons() == 1 && _mouseButtonStates[(int)mouseButton];
        }
        #endregion
    }
}
#endif