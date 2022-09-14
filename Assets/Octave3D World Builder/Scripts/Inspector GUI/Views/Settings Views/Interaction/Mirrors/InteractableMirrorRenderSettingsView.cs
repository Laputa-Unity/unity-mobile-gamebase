#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class InteractableMirrorRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private InteractableMirrorRenderSettings _settings;
        #endregion

        #region Constructors
        public InteractableMirrorRenderSettingsView(InteractableMirrorRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderWidthField();
            RenderHeightField();

            EditorGUILayout.Separator();
            RenderUseInfiniteWidthToggle();
            RenderUseInfiniteHeightToggle();

            EditorGUILayout.Separator();
            RenderMirrorPlaneColorField();
            RenderMirrorPlaneBorderLineColorField();
            RenderMirroredBoxColorField();
            RenderMirroredBoxBorderLineColorField();
        }
        #endregion

        #region Private Methods
        private void RenderMirrorPlaneColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForMirrorPlaneColorField(), _settings.MirrorPlaneColor);
            if(newColor != _settings.MirrorPlaneColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MirrorPlaneColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForMirrorPlaneColorField()
        {
            var content = new GUIContent();
            content.text = "Mirror color";
            content.tooltip = "Allows you to change the mirror plane color.";

            return content;
        }

        private void RenderMirrorPlaneBorderLineColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForMirrorPlaneBorderLineColorField(), _settings.MirrorPlaneBorderLineColor);
            if (newColor != _settings.MirrorPlaneBorderLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MirrorPlaneBorderLineColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForMirrorPlaneBorderLineColorField()
        {
            var content = new GUIContent();
            content.text = "Mirror border line color";
            content.tooltip = "Allows you to change the mirror plane border line color.";

            return content;
        }

        private void RenderWidthField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForSizeField(), _settings.MirrorWidth);
            if (newFloat != _settings.MirrorWidth)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MirrorWidth = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForSizeField()
        {
            var content = new GUIContent();
            content.text = "Mirror width";
            content.tooltip = "Allows you to control the width of the mirror plane.";

            return content;
        }

        private void RenderHeightField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForHeightField(), _settings.MirrorHeight);
            if (newFloat != _settings.MirrorHeight)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MirrorHeight = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForHeightField()
        {
            var content = new GUIContent();
            content.text = "Mirror height";
            content.tooltip = "Allows you to control the height of the mirror plane.";

            return content;
        }

        private void RenderMirroredBoxColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForMirroredBoxColorField(), _settings.MirroredBoxColor);
            if(newColor != _settings.MirroredBoxColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MirroredBoxColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForMirroredBoxColorField()
        {
            var content = new GUIContent();
            content.text = "Mirrored box color";
            content.tooltip = "Allows you to control the color of the boxes which act as visual guides for the mirrored entities.";

            return content;
        }

        private void RenderMirroredBoxBorderLineColorField()
        {
            Color newColor = EditorGUILayout.ColorField(GetContentForMirroredBoxBorderLineColorField(), _settings.MirroredBoxBorderLineColor);
            if (newColor != _settings.MirroredBoxBorderLineColor)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MirroredBoxBorderLineColor = newColor;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForMirroredBoxBorderLineColorField()
        {
            var content = new GUIContent();
            content.text = "Mirrored box line color";
            content.tooltip = "Allows you to control the border line color of the boxes which act as visual guides for the mirrored entities.";

            return content;
        }

        private void RenderUseInfiniteWidthToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseInfiniteWidthToggle(), _settings.UseInfiniteWidth);
            if(newBool != _settings.UseInfiniteWidth)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseInfiniteWidth = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForUseInfiniteWidthToggle()
        {
            var content = new GUIContent();
            content.text = "Has infinite width";
            content.tooltip = "If this is checked, the mirror will be drawn using an infinite width.";

            return content;
        }

        private void RenderUseInfiniteHeightToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseInfiniteHeightToggle(), _settings.UseInfiniteHeight);
            if (newBool != _settings.UseInfiniteHeight)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseInfiniteHeight = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForUseInfiniteHeightToggle()
        {
            var content = new GUIContent();
            content.text = "Has infinite height";
            content.tooltip = "If this is checked, the mirror will be drawn using an infinite height.";

            return content;
        }
        #endregion
    }
}
#endif