#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabTag _prefabTag;

        [SerializeField]
        private bool _allowTagNameChange = false;
        #endregion

        #region Public Properties
        public bool AllowTagNameChange { get { return _allowTagNameChange; } set { _allowTagNameChange = value; } }
        #endregion

        #region Constructors
        public PrefabTagView(PrefabTag prefabTag)
        {
            _prefabTag = prefabTag;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderActionControls();
        }
        #endregion

        #region Private Methods
        private void RenderActionControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderTagActivationToggle();
            RenderTagNameLabelOrNameChangeField();
            RenderRemoveTagButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderTagActivationToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft("", _prefabTag.IsActive, GUILayout.Width(EditorGUILayoutEx.ToggleButtonWidth));
            if(newBool != _prefabTag.IsActive)
            {
                UndoEx.RecordForToolAction(_prefabTag);
                _prefabTag.IsActive = newBool;
            }
        }

        private void RenderTagNameLabelOrNameChangeField()
        {
            if (_allowTagNameChange) RenderTagNameChangeField();
            else RenderTagNameLabel();
        }

        private void RenderTagNameChangeField()
        {
            string newName = EditorGUILayoutEx.DelayedTextField(GetContentForTagNameChangeField(), _prefabTag.Name);
            if(newName != _prefabTag.Name)
            {
                UndoEx.RecordForToolAction(_prefabTag);
                PrefabTagDatabase.Get().RenamePrefabTag(_prefabTag, newName);
            }
        }

        private GUIContent GetContentForTagNameChangeField()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "Allows you to change the name of the tag.";

            return content;
        }

        private void RenderTagNameLabel()
        {
            EditorGUILayout.LabelField(GetContentForTagNameLabel(_prefabTag.Name));
        }

        private GUIContent GetContentForTagNameLabel(string tagName)
        {
            var content = new GUIContent();
            content.text = tagName;
            content.tooltip = "The name of the prefab tag.";

            return content;
        }

        private void RenderRemoveTagButton()
        {
            if (GUILayout.Button(GetContentForRemoveTagButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(PrefabTagDatabase.Get());
                PrefabTagDatabase.Get().RemoveAndDestroyPrefabTag(_prefabTag);
            }
        }

        private GUIContent GetContentForRemoveTagButton()
        {
            var content = new GUIContent();
            content.text = "Remove";
            content.tooltip = "Removes the tag.";

            return content;
        }
        #endregion
    }
}
#endif