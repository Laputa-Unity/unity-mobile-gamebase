#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightPatternView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathHeightPattern _heightPattern;

        [SerializeField]
        private ObjectPlacementPathHeightPatternViewData _viewData;
        #endregion

        #region Private Properties
        private ObjectPlacementPathHeightPatternViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathHeightPatternViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Constructors
        public ObjectPlacementPathHeightPatternView(ObjectPlacementPathHeightPattern heightPattern)
        {
            _heightPattern = heightPattern;

            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderPatternViewVisibilityToggleAndPatternActivationButton();
            if (ViewData.IsViewVisible)
            {
                EditorGUILayout.Separator();
                RenderPatternNameChangeTextField();
                RenderPatternEditControls();
                RenderRemovePatternButton();
            }
        }
        #endregion

        #region Private Methods
        private void RenderPatternViewVisibilityToggleAndPatternActivationButton()
        {
            EditorGUILayout.BeginHorizontal();
            RenderPatternViewVisibilityToggle();
            RenderPatternActivationButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderPatternViewVisibilityToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft("", ViewData.IsViewVisible, GUILayout.Width(EditorGUILayoutEx.ToggleButtonWidth));
            if (newBool != ViewData.IsViewVisible)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.IsViewVisible = newBool;
            }
        }

        private void RenderPatternActivationButton()
        {
            EditorGUIColor.Push(GetPatternActivationButtonColor());
            if (GUILayout.Button(GetContentForPatternActivationButton(), GetStyleForPatternActivationButton(), GUILayout.ExpandWidth(true)))
            {
                UndoEx.RecordForToolAction(ObjectPlacementPathHeightPatternDatabase.Get());
                ObjectPlacementPathHeightPatternDatabase.Get().SetActivePattern(_heightPattern);
            }
            EditorGUIColor.Pop();
        }

        private GUIStyle GetStyleForPatternActivationButton()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private Color GetPatternActivationButtonColor()
        {
            if (_heightPattern == ObjectPlacementPathHeightPatternDatabase.Get().ActivePattern) return ObjectPlacementPathHeightPatternDatabase.Get().View.ColorForActivePatternButton;
            else return GUI.color;
        }

        private GUIContent GetContentForPatternActivationButton()
        {
            var content = new GUIContent();
            content.text = _heightPattern.Name;
            content.tooltip = "Clicking on this button will activate this pattern.";

            return content;
        }

        private void RenderPatternNameChangeTextField()
        {
            ObjectPlacementPathHeightPatternDatabase patternDatabase = ObjectPlacementPathHeightPatternDatabase.Get();
            string newString = EditorGUILayoutEx.DelayedTextField(GetContentForPatternNameChangeField(), _heightPattern.Name);
            if (newString != _heightPattern.Name)
            {
                UndoEx.RecordForToolAction(_heightPattern);
                patternDatabase.RenamePattern(_heightPattern, newString);
            }
        }

        private GUIContent GetContentForPatternNameChangeField()
        {
            var content = new GUIContent();
            content.text = "Name:";
            content.tooltip = "Allows you to change the name of the pattern.";

            return content;
        }

        private void RenderPatternEditControls()
        {
            RenderAllowPatternEditToggle();
            if (_viewData.AllowPatternStringEdit)
            {
                RenderPatternStringTextArea();
                RenderPatternTextAreaHeightSlider();
                RenderApplyPatternButton();
            }
        }

        private void RenderAllowPatternEditToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowPatternEditToggle(), _viewData.AllowPatternStringEdit);
            if(newBool != _viewData.AllowPatternStringEdit)
            {
                UndoEx.RecordForToolAction(_viewData);
                _viewData.CurrentPathHeightPatternString = _heightPattern.PatternString;
                _viewData.AllowPatternStringEdit = newBool;
            }
        }

        private GUIContent GetContentForAllowPatternEditToggle()
        {
            var content = new GUIContent();
            content.text = "Allow edit";
            content.tooltip = "When this is checked a text area will appear which lets you modify the pattern string.";

            return content;
        }

        private void RenderPatternStringTextArea()
        {
            // Note: We also have to specify a width in order for wrapping to work accordingly. Also, 'ExpandWidth' is used
            //       to ensure that the width expands as the Inspector area is enlarged.
            string newString = EditorGUILayoutEx.TextArea(_viewData.CurrentPathHeightPatternString, true, GUILayout.Height(_viewData.PatternTextAreaHeight), GUILayout.Width(300.0f), GUILayout.ExpandWidth(true));
            if(newString != _viewData.CurrentPathHeightPatternString)
            {
                UndoEx.RecordForToolAction(_viewData);
                _viewData.CurrentPathHeightPatternString = newString;
            }
        }

        private void RenderPatternTextAreaHeightSlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForPatternTextAreaHeightSlider(), _viewData.PatternTextAreaHeight, ObjectPlacementPathHeightPatternViewData.MinPatternTextAreaHeight, ObjectPlacementPathHeightPatternViewData.MaxPatternTextAreaHeight);
            if(newFloat != _viewData.PatternTextAreaHeight)
            {
                UndoEx.RecordForToolAction(_viewData);
                _viewData.PatternTextAreaHeight = newFloat;
            }
        }

        private GUIContent GetContentForPatternTextAreaHeightSlider()
        {
            var content = new GUIContent();
            content.text = "Pattern text area height";
            content.tooltip = "Allows you to control the height of the pattern text area (min: " + ObjectPlacementPathHeightPatternViewData.MinPatternTextAreaHeight + ", max: " + ObjectPlacementPathHeightPatternViewData.MaxPatternTextAreaHeight + ")";

            return content;
        }

        private void RenderApplyPatternButton()
        {
            if(GUILayout.Button(GetContentForApplyPatternButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_heightPattern);
                if(_heightPattern.SetPatternString(_viewData.CurrentPathHeightPatternString))
                {
                    Debug.Log("The pattern string was applied successfully.");
                }
            }
        }

        private GUIContent GetContentForApplyPatternButton()
        {
            var content = new GUIContent();
            content.text = "Apply pattern";
            content.tooltip = "Press this button to apply the pattern string written in the text area. If the string is valid, the pattern will be modified accordingly.";

            return content;
        }

        private void RenderRemovePatternButton()
        {
            if(GUILayout.Button(GetContentForRemovePatternButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(ObjectPlacementPathHeightPatternDatabase.Get());
                ObjectPlacementPathHeightPatternDatabase.Get().RemoveAndDestroyPattern(_heightPattern);
            }
        }

        private GUIContent GetContentForRemovePatternButton()
        {
            var content = new GUIContent();
            content.text = "Remove";
            content.tooltip = "Removes the pattern from the height pattern database.";

            return content;
        }
        #endregion
    }
}
#endif