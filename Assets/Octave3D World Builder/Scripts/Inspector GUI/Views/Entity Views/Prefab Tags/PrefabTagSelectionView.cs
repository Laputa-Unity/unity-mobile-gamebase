#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagSelectionView : EntityView
    {
        #region Private Variables
        [SerializeField]
        private PrefabTagSelectionViewData _viewData;

        // Note: This could be created here and serialized, but Unity's serialization system won't allow it.
        //       Serialization depth limit is exceeded. Moreover, the '[NonSerialized]' attribute has to be
        //       used because otherwise Unity will attempt to serialize the filter. Is there any logic here?
        [NonSerialized]
        private PrefabTagFilter _prefabTagFilter;
        [NonSerialized]
        private List<PrefabTag> _filteredTags;
        #endregion

        #region Private Properties
        private PrefabTagSelectionViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabTagSelectionViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Public Properties
        public PrefabTagFilter PrefabTagFilter { set { _prefabTagFilter = value; } }
        public List<string> ListOfSelectedTagNames { get { return ViewData.ListOfSelectedTagNames; } set { ViewData.ListOfSelectedTagNames = value; } }
        public bool HasSelectionChanged { get { return ViewData.HasSelectionChanged; } }
        #endregion

        #region Public Methods
        public bool IsTagSelected(PrefabTag prefabTag)
        {
            return IsTagSelected(prefabTag.Name);
        }

        public bool IsTagSelected(string tagName)
        {
            return ViewData.IsTagSelected(tagName);
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            if (PrefabTagDatabase.Get().IsEmpty) EditorGUILayout.HelpBox("There are no prefab tags to display.", UnityEditor.MessageType.None);
            else
            {
                AcquireFilteredPrefabTags();

                if (_prefabTagFilter != null) _prefabTagFilter.View.Render();
                RenderPrefabTagSelectionScrollView();
                RenderSelectDeselectAllTagsButtons();
                RenderSelectDeselectOnlyFilteredTagsButtons();
            }
        }
        #endregion

        #region Private Methods
        private void AcquireFilteredPrefabTags()
        {
            PrefabTagDatabase prefabTagDatabase = PrefabTagDatabase.Get();
            List<PrefabTag> prefabTags = prefabTagDatabase.GetAllPrefabTags();

            _filteredTags = _prefabTagFilter != null ? _prefabTagFilter.GetFilteredPrefabTags(prefabTags) : prefabTags;
        }

        private void RenderPrefabTagSelectionScrollView()
        {
            ViewData.PrefabTagScrollPosition = EditorGUILayout.BeginScrollView(ViewData.PrefabTagScrollPosition, "Box", GUILayout.Height(PrefabTagSelectionViewData.PrefabTagScrollViewHeight));
            RenderSelectionToggleControlsForFilteredTags();
            EditorGUILayout.EndScrollView();
        }

        private void RenderSelectionToggleControlsForFilteredTags()
        {
            foreach (PrefabTag prefabTag in _filteredTags)
            {
                bool isCurrentlySelected = IsTagSelected(prefabTag);
                bool newBool = EditorGUILayout.ToggleLeft(GetContentForTagSelectionToggle(prefabTag.Name), isCurrentlySelected);
                if (isCurrentlySelected != newBool)
                {
                    UndoEx.RecordForToolAction(ViewData);
                    SetTagSelected(prefabTag, newBool);
                }
            }
        }

        private void SetTagSelected(PrefabTag prefabTag, bool isSelected)
        {
            ViewData.HasSelectionChanged = true;

            if (isSelected) ViewData.AddTagNameToTagSelection(prefabTag.Name);
            else ViewData.RemoveTagNameFromTagSelection(prefabTag.Name);
        }

        private GUIContent GetContentForTagSelectionToggle(string tagName)
        {
            var content = new GUIContent();
            content.text = tagName;
            content.tooltip = "Check/uncheck to select/deselect the tag.";

            return content;
        }

        private void RenderSelectDeselectAllTagsButtons()
        {
            EditorGUILayout.BeginHorizontal();
            RenderSelectAllTagsButton();
            RenderDeselectAllTagsButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderSelectAllTagsButton()
        {
            if (GUILayout.Button(GetContentForSelectAllTagsButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(ViewData);

                ViewData.ListOfSelectedTagNames = PrefabTagDatabase.Get().GetAllPrefabTagNames();
                ViewData.HasSelectionChanged = true;
            }
        }

        private GUIContent GetContentForSelectAllTagsButton()
        {
            var content = new GUIContent();
            content.text = "Select all";
            content.tooltip = "Selects all tags.";

            return content;
        }

        private void RenderDeselectAllTagsButton()
        {
            if (GUILayout.Button(GetContentForDeselectAllTagsButton()))
            {
                UndoEx.RecordForToolAction(ViewData);
                ClearSelection();
            }
        }

        private void ClearSelection()
        {
            ViewData.ClearSelectedTagNames();
            ViewData.HasSelectionChanged = true;
        }

        private GUIContent GetContentForDeselectAllTagsButton()
        {
            var content = new GUIContent();
            content.text = "Deselect all";
            content.tooltip = "Deselects all tags.";

            return content;
        }

        private void RenderSelectDeselectOnlyFilteredTagsButtons()
        {
            EditorGUILayout.BeginHorizontal();
            RenderSelectOnlyFilteredTagsButton();
            RenderDeselectFilteredTagsButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderSelectOnlyFilteredTagsButton()
        {
            if (GUILayout.Button(GetContentForSelectFilteredTagsButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(ViewData);
                SelectOnlyFilteredTags();
            }
        }

        private void SelectOnlyFilteredTags()
        {
            List<string> namesOfFilteredTags = GetNamesOfFilteredTags();
            if (namesOfFilteredTags.Count != 0)
            {
                ClearSelection();
                ViewData.ListOfSelectedTagNames = namesOfFilteredTags;
            }
        }

        private List<string> GetNamesOfFilteredTags()
        {
            return (from prefabTag in _filteredTags select prefabTag.Name).ToList();
        }

        private GUIContent GetContentForSelectFilteredTagsButton()
        {
            var content = new GUIContent();
            content.text = "Select only filtered";
            content.tooltip = "Selects only the filtered tags.";

            return content;
        }

        private void RenderDeselectFilteredTagsButton()
        {
            if (GUILayout.Button(GetContentForDeselectFilteredTagsButton()))
            {
                UndoEx.RecordForToolAction(ViewData);
                DeselectFilteredTags();
            }
        }

        private void DeselectFilteredTags()
        {
            List<string> namesOfFilteredTags = GetNamesOfFilteredTags();
            if (namesOfFilteredTags.Count != 0)
            {
                ViewData.RemoveTagNameByPredicate(item => namesOfFilteredTags.Contains(item));
                ViewData.HasSelectionChanged = true;
            }
        }

        private GUIContent GetContentForDeselectFilteredTagsButton()
        {
            var content = new GUIContent();
            content.text = "Deselect filtered";
            content.tooltip = "Deselects the filtered tags.";

            return content;
        }
        #endregion
    }
}
#endif