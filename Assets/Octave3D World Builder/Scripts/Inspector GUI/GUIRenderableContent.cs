#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public abstract class GUIRenderableContent
    {
        #region Protected Variables
        [SerializeField]
        protected bool _isVisible = true;
        [SerializeField]
        protected bool _toggleVisibilityBeforeRender = false;
        [SerializeField]
        protected string _visibilityToggleLabel = "";
        [SerializeField]
        protected int _visibilityToggleIndent = 0;
        [SerializeField]
        protected bool _indentContent = false;
        [SerializeField]
        protected bool _surroundWithBox = false;
        #endregion

        #region Public Properties
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        public bool ToggleVisibilityBeforeRender { get { return _toggleVisibilityBeforeRender; } set { _toggleVisibilityBeforeRender = value; } }
        public string VisibilityToggleLabel { get { return _visibilityToggleLabel; } set { _visibilityToggleLabel = value; } }
        public int VisibilityToggleIndent { get { return _visibilityToggleIndent; } set { _visibilityToggleIndent = value; } }
        public bool IndentContent { get { return _indentContent; } set { _indentContent = value; } }
        public bool SurroundWithBox { get { return _surroundWithBox; } set { _surroundWithBox = value; } }
        #endregion

        #region Public Methods
        public void Render()
        {
            EditorGUILayout.BeginVertical();
            if (_toggleVisibilityBeforeRender) ToggleVisibilityAndRender();
            else if(IsVisible) RenderGUIContent();
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Protected Abstract Methods
        protected abstract void RenderContent();
        #endregion

        #region Private Methods
        private void ToggleVisibilityAndRender()
        {
            ToggleVisibility();
            if (IsVisible) RenderGUIContent();
        }

        private void ToggleVisibility()
        {
            EditorGUI.indentLevel += _visibilityToggleIndent;
            bool newBool = EditorGUILayout.Foldout(IsVisible, _visibilityToggleLabel);
            EditorGUI.indentLevel -= _visibilityToggleIndent;

            if (newBool != IsVisible) IsVisible = newBool;
        }

        private void RenderGUIContent()
        {
            if (_indentContent) IndentAndRender();
            else RenderNoIndent();
        }

        private void IndentAndRender()
        {
            int indentAmount = _toggleVisibilityBeforeRender ? _visibilityToggleIndent + 1 : 1;
            EditorGUI.indentLevel += indentAmount;

            if (_surroundWithBox) SurroundWithBoxAndRender();
            else RenderContent();

            EditorGUI.indentLevel -= indentAmount;
        }

        private void RenderNoIndent()
        {
            if (_surroundWithBox) SurroundWithBoxAndRender();
            else RenderContent();
        }

        private void SurroundWithBoxAndRender()
        {
            EditorGUILayoutEx.BeginVerticalBox();
            RenderContent();
            EditorGUILayoutEx.EndVerticalBox();
        }
        #endregion
    }
}
#endif