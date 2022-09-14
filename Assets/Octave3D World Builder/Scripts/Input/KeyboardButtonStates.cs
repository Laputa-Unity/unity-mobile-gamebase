#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class KeyboardButtonStates : Singleton<KeyboardButtonStates>
    {
        #region Private Variables
        private Dictionary<KeyCode, bool> _keyboardButtonStates = new Dictionary<KeyCode, bool>();
        #endregion

        #region Public Methods
        public void ClearStates()
        {
            _keyboardButtonStates.Clear();
        }

        public void OnKeyboardButtonPressed(KeyCode keyCode)
        {
            AddKeyCodeEntryIfNecessary(keyCode);
            _keyboardButtonStates[keyCode] = true;
        }

        public void OnKeyboardButtonReleased(KeyCode keyCode)
        {
            AddKeyCodeEntryIfNecessary(keyCode);
            _keyboardButtonStates[keyCode] = false;
        }

        public bool IsKeyboardButtonPressed(KeyCode keyCode)
        {
            if (keyCode == KeyCode.LeftShift || keyCode == KeyCode.RightShift) return Event.current.shift;
            if (keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl) return Event.current.control;
            if (keyCode == KeyCode.LeftCommand || keyCode == KeyCode.RightCommand) return Event.current.command;

            if (!DoesKeyCodeEntryExist(keyCode)) return false;

            return _keyboardButtonStates[keyCode];
        }

        public void RepairCTRLAndCMDStates()
        {
            if(!Event.current.control)
            {
                if (IsKeyboardButtonPressed(KeyCode.LeftControl)) SetKeyPressed(KeyCode.LeftControl, false);
                if (IsKeyboardButtonPressed(KeyCode.RightControl)) SetKeyPressed(KeyCode.RightControl, false);
            }

            if(!Event.current.command)
            {
                if (IsKeyboardButtonPressed(KeyCode.LeftCommand)) SetKeyPressed(KeyCode.LeftCommand, false);
                if (IsKeyboardButtonPressed(KeyCode.RightCommand)) SetKeyPressed(KeyCode.RightCommand, false);
            }
        }

        public bool IsAnyShiftKeyPressed()
        {
            return Event.current.shift;
        }

        public bool IsAnyCtrlKeyPressed()
        {
            return Event.current.control;
        }

        public bool IsAnyAltKeyPressed()
        {
            return Event.current.alt;
        }
        #endregion

        #region Private Methods
        private void AddKeyCodeEntryIfNecessary(KeyCode keyCode)
        {
            if (!DoesKeyCodeEntryExist(keyCode)) _keyboardButtonStates.Add(keyCode, false);
        }

        private bool DoesKeyCodeEntryExist(KeyCode keyCode)
        {
            return _keyboardButtonStates.ContainsKey(keyCode);
        }

        private void SetKeyPressed(KeyCode keyCode, bool pressed)
        {
            AddKeyCodeEntryIfNecessary(keyCode);
            _keyboardButtonStates[keyCode] = pressed;
        }
        #endregion
    }
}
#endif