#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabFilterView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabFilter _prefabFilter;
        #endregion

        #region Constructors
        public PrefabFilterView(PrefabFilter prefabFilter)
        {
            _prefabFilter = prefabFilter;
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _prefabFilter.NameFilter.View.Render();
            RenderPropertyToggleControls();
        }

        private void RenderPropertyToggleControls()
        {
            var content = new GUIContent();
            content.text = "Tag filter mode";
            content.tooltip = "Allows you to specify how prefabs are filtered based on tags.";

            PrefabTagFilterMode newFilterMode = (PrefabTagFilterMode)EditorGUILayout.EnumPopup(content, _prefabFilter.PrefabTagFilterMode);
            if (newFilterMode != _prefabFilter.PrefabTagFilterMode)
            {
                UndoEx.RecordForToolAction(_prefabFilter);
                _prefabFilter.PrefabTagFilterMode = newFilterMode;
            }

            //RenderFilterByActiveTagsToggle();
        }

/*
        private void RenderFilterByActiveTagsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForFilterByActiveTagsToggle(), _prefabFilter.FilterByActiveTags.IsActive);
            if(newBool != _prefabFilter.FilterByActiveTags.IsActive)
            {
                UndoEx.RecordForToolAction(_prefabFilter);
                _prefabFilter.FilterByActiveTags.SetActive(newBool);
            }
        }

        private GUIContent GetContentForFilterByActiveTagsToggle()
        {
            var content = new GUIContent();
            content.text = _prefabFilter.FilterByActiveTags.Name;
            content.tooltip = "If this is checked, only prefabs that are associated with at least one active tag are allowed.";

            return content;
        }*/
        #endregion
    }
}
#endif