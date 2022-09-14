#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class SingleObjectPivotPointRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private SingleObjectPivotPointRenderSettings _settings;
        #endregion

        #region Constructors
        public SingleObjectPivotPointRenderSettingsView(SingleObjectPivotPointRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderIsVisibleToggle();
            RenderFillColorSelectionField();
            RenderBorderLineColorSelectionField();
            RenderScaleField();
        }
        #endregion

        #region Private Methods
        private void RenderIsVisibleToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIsVisibleToggle(), _settings.IsVisible);
            if(newBool != _settings.IsVisible)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.IsVisible = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForIsVisibleToggle()
        {
            var content = new GUIContent();
            content.text = "Visible";
            content.tooltip = "Allows you to control the visibility of the pivot point.";

            return content;
        }

        private void RenderFillColorSelectionField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForFillColorSelectionField(), _settings.FillColor);
            if(newColor != _settings.FillColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.FillColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForFillColorSelectionField()
        {
            var content = new GUIContent();
            content.text = "Fill color";
            content.tooltip = "Allows you to specify the pivot point fill color.";

            return content;
        }

        private void RenderBorderLineColorSelectionField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForBorderLineColorSelectionField(), _settings.BorderLineColor);
            if (newColor != _settings.BorderLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.BorderLineColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForBorderLineColorSelectionField()
        {
            var content = new GUIContent();
            content.text = "Border line color";
            content.tooltip = "Allows you to specify the pivot point border line color.";

            return content;
        }

        private void RenderScaleField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForScaleField(), _settings.Scale);
            if(newFloat != _settings.Scale)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.Scale = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForScaleField()
        {
            var content = new GUIContent();
            content.text = "Scale";
            content.tooltip = "Allows you to modify the pivot point scale.";

            return content;
        }
        #endregion
    }
}
#endif