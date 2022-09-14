#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public abstract class Toolbar
    {
        #region Protected Variables
        [SerializeField]
        protected List<string> _normalStateButtonTexturePaths = new List<string>();
        [SerializeField]
        protected List<string> _activeStateButtonTexturePaths = new List<string>();
        [SerializeField]
        protected List<string> _buttonTooltips = new List<string>();

        [SerializeField]
        protected float _buttonScale = 1.0f;

        [SerializeField]
        protected bool _allowActiveStateForButtons = true;
        [SerializeField]
        protected bool _useBoxStyleForButtons = true;
        #endregion

        #region Public Static Properties
        public static float MinButtonScale { get { return 0.0001f; } }
        #endregion

        #region Public Properties
        public float ButtonScale { get { return _buttonScale; } set { _buttonScale = Mathf.Max(value, MinButtonScale); } }
        public bool AllowActiveStateForButtons { get { return _allowActiveStateForButtons; } set { _allowActiveStateForButtons = value; } }
        public bool UseBoxStyleForButtons { get { return _useBoxStyleForButtons; } set { _useBoxStyleForButtons = value; } }
        #endregion

        #region Constructors
        public Toolbar()
        {
            _normalStateButtonTexturePaths = GetNormalStateButtonTexturePaths();
            _activeStateButtonTexturePaths = GetActiveStateButtonTexturePaths();
            _buttonTooltips = GetButtonTooltips();
        }
        #endregion

        #region Public Methods
        public void Render()
        {
            RenderAllButtons();
        }
        #endregion

        #region Protected Abstract Methods
        protected abstract int GetNumberOfButtons();
        protected abstract List<string> GetButtonTooltips();
        protected abstract List<string> GetNormalStateButtonTexturePaths();
        protected abstract List<string> GetActiveStateButtonTexturePaths();
        protected abstract Color GetButtonColor(int buttonIndex);
        protected abstract int GetActiveButtonIndex();
        protected abstract void HandleButtonClick(int buttonIndex);
        #endregion

        #region Private Methods
        private void RenderAllButtons()
        {
            EditorGUILayoutEx.BeginHorizontalBox();
            int numberOfButtons = GetNumberOfButtons();
            for (int buttonIndex = 0; buttonIndex < numberOfButtons; ++buttonIndex)
            {
                RenderButton(buttonIndex);
            }
            EditorGUILayoutEx.EndHorizontalBox();
        }

        private void RenderButton(int buttonIndex)
        {
            EditorGUIColor.Push(GetButtonColor(buttonIndex));

            Texture2D buttonTexture = GetButtonTexture(buttonIndex);
            var buttonContent = new GUIContent();
            buttonContent.text = "";
            buttonContent.image = buttonTexture;
            buttonContent.tooltip = _buttonTooltips[buttonIndex];

            if (_useBoxStyleForButtons)
            {
                if (GUILayout.Button(buttonContent, "Box", GUILayout.Width(buttonTexture.width * _buttonScale), GUILayout.Height(buttonTexture.height * _buttonScale), GUILayout.ExpandWidth(false)))
                {
                    HandleButtonClick(buttonIndex);
                }
            }
            else
            {
                if (GUILayout.Button(buttonContent, GUILayout.Width(buttonTexture.width * _buttonScale), GUILayout.Height(buttonTexture.height * _buttonScale), GUILayout.ExpandWidth(false)))
                {
                    HandleButtonClick(buttonIndex);
                }
            }
            EditorGUIColor.Pop();
        }

        private Texture2D GetButtonTexture(int buttonIndex)
        {
            if (AllowActiveStateForButtons && buttonIndex == GetActiveButtonIndex() && _activeStateButtonTexturePaths.Count != 0) return TextureCache.Get().GetTextureAtRelativePath(_activeStateButtonTexturePaths[buttonIndex]);
            return TextureCache.Get().GetTextureAtRelativePath(_normalStateButtonTexturePaths[buttonIndex]);
        }
        #endregion
    }
}
#endif
