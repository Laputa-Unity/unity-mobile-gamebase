#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class ShortcutCombo
    {
        #region Private Variables
        private List<KeyCode> _keys = new List<KeyCode>();
        private List<MouseButton> _mouseButtons = new List<MouseButton>();
        private List<ShortcutCombo> _possibleOverlaps = new List<ShortcutCombo>();

        private bool _useCmdAsAlternativeForCtrl = true;
        private bool _notActiveWhenMouseButtonsPressed = true;
        private bool _notActiveWhenRightMouseButtonPressed = true;
        #endregion

        #region Public Properties
        public int NumberOfKeys { get { return _keys.Count; } }
        public int NumberOfMouseButtons { get { return _mouseButtons.Count; } }
        public bool UseCmdAsAlternativeForCtrl { get { return _useCmdAsAlternativeForCtrl; } set { _useCmdAsAlternativeForCtrl = value; } }
        public bool NotActiveWhenMouseButtonsPressed { get { return _notActiveWhenMouseButtonsPressed; } set { _notActiveWhenMouseButtonsPressed = value; } }
        public bool NotActiveWhenRightMouseButtonPressed { get { return _notActiveWhenRightMouseButtonPressed; } set { _notActiveWhenRightMouseButtonPressed = value; } }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            string comboString = "";
            for (int keyIndex = 0; keyIndex < NumberOfKeys; ++keyIndex )
            {
                comboString += _keys[keyIndex];
                if (keyIndex < NumberOfKeys - 1) comboString += " + ";
            }

            if(NumberOfMouseButtons != 0)
            {
                if (NumberOfKeys != 0) comboString += " + ";
                for(int mouseButtonIndex = 0; mouseButtonIndex < NumberOfMouseButtons; ++mouseButtonIndex)
                {
                    comboString += _mouseButtons[mouseButtonIndex];
                    if (mouseButtonIndex < NumberOfMouseButtons - 1) comboString += " + ";
                }
            }

            return comboString;
        }

        public void AddKey(KeyCode key)
        {
            if (!ContainsKey(key)) _keys.Add(key);
        }

        public void RemoveKey(KeyCode key)
        {
            _keys.Remove(key);
        }

        public void ClearKeys()
        {
            _keys.Clear();
        }

        public bool ContainsKey(KeyCode key)
        {
            return _keys.Contains(key);
        }

        public bool ContainsAllKeys(List<KeyCode> keys)
        {
            foreach(KeyCode key in keys)
            {
                if (!ContainsKey(key)) return false;
            }

            return true;
        }

        public void AddMouseButton(MouseButton mouseButton)
        {
            if (!ContainsMouseButton(mouseButton)) _mouseButtons.Add(mouseButton);
        }

        public void RemoveMouseButton(MouseButton mouseButton)
        {
            _mouseButtons.Remove(mouseButton);
        }

        public void ClearMouseButtons()
        {
            _mouseButtons.Clear();
        }

        public bool ContainsMouseButton(MouseButton mouseButton)
        {
            return _mouseButtons.Contains(mouseButton);
        }

        public bool ContainsAllMouseButtons(List<MouseButton> mouseButtons)
        {
            foreach(MouseButton mouseButton in mouseButtons)
            {
                if (!ContainsMouseButton(mouseButton)) return false;
            }

            return true;
        }

        public void AddPossibleOverlap(ShortcutCombo combo)
        {
            if (!_possibleOverlaps.Contains(combo)) _possibleOverlaps.Add(combo);
        }

        public void RemovePossibleOverlap(ShortcutCombo combo)
        {
            _possibleOverlaps.Remove(combo);
        }

        public void ClearPossibleOverlaps()
        {
            _possibleOverlaps.Clear();
        }

        public bool ContainsPossibleOverlap(ShortcutCombo combo)
        {
            return _possibleOverlaps.Contains(combo);
        }

        public bool CanOverlapWith(ShortcutCombo shortcutCombo)
        {
            if (shortcutCombo == null) return false;
            if (ReferenceEquals(this, shortcutCombo) || shortcutCombo == this) return false;

            // We can only have an overlap when the number of keys and mouse buttons is <= than the ones in 'shortcutCombo'
            if (NumberOfKeys <= shortcutCombo.NumberOfKeys && NumberOfMouseButtons <= shortcutCombo.NumberOfMouseButtons)
            {
                // We may have an overlap. Return true only if 'shortcutCombo' contains all keys and all mouse buttons which
                // reside in 'this' combo.
                if (shortcutCombo.ContainsAllKeys(_keys) && shortcutCombo.ContainsAllMouseButtons(_mouseButtons)) return true;
            }

            return false;
        }

        public bool IsOverlappedBy(ShortcutCombo shortcutCombo)
        {
            return CanOverlapWith(shortcutCombo) && shortcutCombo.IsActive(false);
        }

        public bool IsActive(bool checkForPossibleOverlaps = true)
        {
            if (!_notActiveWhenMouseButtonsPressed && MouseButtonStates.Instance.GetNumberOfPressedButtons() != 0) return false;
            if (!_notActiveWhenRightMouseButtonPressed && MouseButtonStates.Instance.IsMouseButtonDown(MouseButton.Right)) return false;

            KeyboardButtonStates keyButtonStates = KeyboardButtonStates.Instance;
            foreach(KeyCode key in _keys)
            {
                if (key != KeyCode.LeftShift && key != KeyCode.RightShift)
                {
                    if (!keyButtonStates.IsKeyboardButtonPressed(key))
                    {
                        if (key == KeyCode.LeftControl || key == KeyCode.RightControl)
                        {
                            if (_useCmdAsAlternativeForCtrl)
                            {
                                if (key == KeyCode.LeftControl && !keyButtonStates.IsKeyboardButtonPressed(KeyCode.LeftCommand)) return false;
                                if (key == KeyCode.RightControl && !keyButtonStates.IsKeyboardButtonPressed(KeyCode.RightCommand)) return false;
                            }
                        }
                        else return false;
                    }
                }
                else
                if (!Event.current.shift) return false;
            }

            MouseButtonStates mouseButtonStates = MouseButtonStates.Instance;
            foreach(MouseButton mouseButton in _mouseButtons)
            {
                if (!mouseButtonStates.IsMouseButtonDown(mouseButton)) return false;
            }

            if(checkForPossibleOverlaps)
            {
                foreach(ShortcutCombo combo in _possibleOverlaps)
                {
                    if (IsOverlappedBy(combo)) return false;
                }
            }

            return true;
        }

        public override bool Equals(object value)
        {
            if (ReferenceEquals(value, null)) return false;
            if (ReferenceEquals(value, this)) return true;

            if (value.GetType() != this.GetType()) return false;
            return IsEqual(value as ShortcutCombo);
        }

        public bool Equals(ShortcutCombo combo)
        {
            if (ReferenceEquals(combo, null)) return false;
            if (ReferenceEquals(combo, this)) return true;

            return IsEqual(combo);
        }

        public override int GetHashCode()
        {
            int hash = 13;

            foreach(KeyCode key in _keys)
            {
                hash = (hash * 7) + key.GetHashCode();
            }

            foreach(MouseButton mouseButton in _mouseButtons)
            {
                hash = (hash * 7) + mouseButton.GetHashCode();
            } 

            return hash;
        }

        public static bool operator ==(ShortcutCombo firstCombo, ShortcutCombo secondCombo)
        {
            return !ReferenceEquals(firstCombo, null) &&
                   !ReferenceEquals(secondCombo, null) &&
                   (ReferenceEquals(firstCombo, secondCombo) || firstCombo.IsEqual(secondCombo));
        }

        public static bool operator !=(ShortcutCombo firstCombo, ShortcutCombo secondCombo)
        {
            return !(firstCombo == secondCombo);
        }
        #endregion

        #region Private Methods
        private bool IsEqual(ShortcutCombo combo)
        {
            // If the number of keys or mouse button is different, the combos are different
            if (combo._keys.Count != _keys.Count) return false;
            if (combo._mouseButtons.Count != _mouseButtons.Count) return false;

            // All keys in 'this' combo must exist in 'combo' 
            foreach(KeyCode key in _keys)
            {
                if (!combo.ContainsKey(key)) return false;
            }

            // All mouse buttons in 'this' combo must exist in 'combo' 
            foreach(MouseButton mouseButton in _mouseButtons)
            {
                if (!combo.ContainsMouseButton(mouseButton)) return false;
            }

            return true;
        }
        #endregion
    }
}
#endif
