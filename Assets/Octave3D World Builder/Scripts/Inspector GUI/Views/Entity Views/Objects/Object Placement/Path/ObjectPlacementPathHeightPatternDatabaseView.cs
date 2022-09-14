#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightPatternDatabaseView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathHeightPatternDatabase _database;

        [SerializeField]
        private ObjectPlacementPathHeightPatternDatabaseViewData _viewData;
        #endregion

        #region Private Properties
        private ObjectPlacementPathHeightPatternDatabaseViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathHeightPatternDatabaseViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Public Properties
        public Color ColorForActivePatternButton { get { return _viewData.ColorForActivePatternButton; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathHeightPatternDatabaseView(ObjectPlacementPathHeightPatternDatabase database)
        {
            _database = database;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleIndent = 1;
            VisibilityToggleLabel = "Path Height Patterns";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            if (_database.IsEmpty) EditorGUILayout.HelpBox("There are no height patterns currently available.", UnityEditor.MessageType.None);
            else
            {
                _database.HeightPatternFilter.View.Render();
                RenderPatternScrollView();
                RenderActivePatternButtonColorField();
                RenderPatternScrollViewHeightSlider();
            }
            RenderActionButtons();
        }
        #endregion

        #region Private Methods
        private void RenderPatternScrollView()
        {
            ViewData.PatternScrollPosition = EditorGUILayout.BeginScrollView(ViewData.PatternScrollPosition, GetStyleForPatternScrollView(), GUILayout.Height(ViewData.PatternScrollViewHeight));
            RenderViewsForFilteredPatterns();
            EditorGUILayout.EndScrollView();
        }

        private GUIStyle GetStyleForPatternScrollView()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private void RenderViewsForFilteredPatterns()
        {
            List<ObjectPlacementPathHeightPattern> filteredHeightPatterns = ObjectPlacementPathHeightPatternDatabase.Get().GetFilteredPatterns();
            foreach (ObjectPlacementPathHeightPattern heightPattern in filteredHeightPatterns)
            {
                heightPattern.View.Render();
            }
        }

        private void RenderActionButtons()
        {
            RenderCreateNewHeightPatternControls();
            RenderRemoveAllPatternsButton();
        }

        private void RenderCreateNewHeightPatternControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderCreateNewHeightPatternButton();
            RenderCreateNewHeightPatternNameChangeField();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderCreateNewHeightPatternButton()
        {
            if (GUILayout.Button(GetContentForCreateNewHeightPatternButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(ObjectPlacementPathHeightPatternDatabase.Get());
                ObjectPlacementPathHeightPatternDatabase.Get().CreateHeightPattern(ViewData.NameForNewHeightPattern);
            }
        }

        private GUIContent GetContentForCreateNewHeightPatternButton()
        {
            var content = new GUIContent();
            content.text = "Create pattern";
            content.tooltip = "Creates a new height pattern using the name specified in the adjacent text field. " +
                              "Names are automatically adjusted such that each pattern name is unique.";

            return content;
        }

        private void RenderCreateNewHeightPatternNameChangeField()
        {
            string newString = EditorGUILayout.TextField(ViewData.NameForNewHeightPattern);
            if (newString != ViewData.NameForNewHeightPattern)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.NameForNewHeightPattern = newString;
            }
        }

        private void RenderRemoveAllPatternsButton()
        {
            if(GUILayout.Button(GetContentForRemoveAllPatternsButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(ObjectPlacementPathHeightPatternDatabase.Get());
                ObjectPlacementPathHeightPatternDatabase.Get().RemoveAndDestroyAllPatterns();
            }
        }

        private GUIContent GetContentForRemoveAllPatternsButton()
        {
            var content = new GUIContent();
            content.text = "Remove all";
            content.tooltip = "Removes all height patterns.";

            return content;
        }

        private void RenderActivePatternButtonColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForActivePatternButtonColorField(), ViewData.ColorForActivePatternButton);
            if (newColor != ViewData.ColorForActivePatternButton)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.ColorForActivePatternButton = newColor;
            }
        }

        private GUIContent GetContentForActivePatternButtonColorField()
        {
            var content = new GUIContent();
            content.text = "Active pattern button color";
            content.tooltip = "Allows you to specify the color of the button which is used to highlight the active height pattern.";

            return content;
        }

        private void RenderPatternScrollViewHeightSlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForPatternScrollViewHeightSlider(), ViewData.PatternScrollViewHeight, ObjectPlacementPathHeightPatternDatabaseViewData.MinPatternScrollViewHeight, ObjectPlacementPathHeightPatternDatabaseViewData.MaxPatternScrollViewHeight);
            if (newFloat != ViewData.PatternScrollViewHeight)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.PatternScrollViewHeight = newFloat;
            }
        }

        private GUIContent GetContentForPatternScrollViewHeightSlider()
        {
            var content = new GUIContent();
            content.text = "Pattern scroll view height";
            content.tooltip = "Allows you to adjust the height of the scroll view which displays all height patterns (min: " + ObjectPlacementPathHeightPatternDatabaseViewData.MinPatternScrollViewHeight + ", max: " + ObjectPlacementPathHeightPatternDatabaseViewData.MaxPatternScrollViewHeight + ").";

            return content;
        }
        #endregion
    }
}
#endif