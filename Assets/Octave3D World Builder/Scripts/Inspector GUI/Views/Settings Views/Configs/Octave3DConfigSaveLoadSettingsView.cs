#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class Octave3DConfigSaveLoadSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private Octave3DConfigSaveLoadSettings _settings;
        #endregion

        #region Constructors
        public Octave3DConfigSaveLoadSettingsView(Octave3DConfigSaveLoadSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderSnapSettingsToggle();
            RenderObjectSelectionSettingsToggle();
            RenderObjectErasingSettingsToggle();

            EditorGUILayout.Separator();
            RenderMirrorLookAndFeelToggle();
            RenderSnapLookAndFeelToggle();
            RenderObjectPlacementLookAndFeelToggle();
            RenderObjectSelectionLookAndFeelToggle();
            RenderObjectErasingLookAndFeelToggle();

            EditorGUILayout.BeginHorizontal();
            RenderCheckAllButton();
            RenderUncheckAllButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderSnapSettingsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSnapSettingsToggle(), _settings.SnapSettings);
            if(newBool != _settings.SnapSettings)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapSettings = newBool;
            }
        }

        private GUIContent GetContentForSnapSettingsToggle()
        {
            var content = new GUIContent();
            content.text = "Snap settings";
            content.tooltip = "If checked, the current snap settings will be saved/loaded.";

            return content;
        }

        private void RenderObjectSelectionSettingsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForObjectSelectionSettingsToggle(), _settings.ObjectSelectionSettings);
            if (newBool != _settings.ObjectSelectionSettings)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectSelectionSettings = newBool;
            }
        }

        private GUIContent GetContentForObjectSelectionSettingsToggle()
        {
            var content = new GUIContent();
            content.text = "Object selection settings";
            content.tooltip = "If checked, the current object selection settings will be saved/loaded.";

            return content;
        }

        private void RenderObjectErasingSettingsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForObjectErasingSettingsToggle(), _settings.ObjectErasingSettings);
            if (newBool != _settings.ObjectErasingSettings)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectErasingSettings = newBool;
            }
        }

        private GUIContent GetContentForObjectErasingSettingsToggle()
        {
            var content = new GUIContent();
            content.text = "Object erasing settings";
            content.tooltip = "If checked, the current object erasing settings will be saved/loaded.";

            return content;
        }

        private void RenderMirrorLookAndFeelToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForMirrorLookAndFeelToggle(), _settings.MirrorLookAndFeel);
            if (newBool != _settings.MirrorLookAndFeel)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.MirrorLookAndFeel = newBool;
            }
        }

        private GUIContent GetContentForMirrorLookAndFeelToggle()
        {
            var content = new GUIContent();
            content.text = "Mirror look and feel";
            content.tooltip = "If checked, the current mirror look and feel settings will be saved/loaded.";

            return content;
        }

        private void RenderSnapLookAndFeelToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSnapLookAndFeelToggle(), _settings.SnapLookAndFeel);
            if (newBool != _settings.SnapLookAndFeel)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapLookAndFeel = newBool;
            }
        }

        private GUIContent GetContentForSnapLookAndFeelToggle()
        {
            var content = new GUIContent();
            content.text = "Snap look and feel";
            content.tooltip = "If checked, the current snap look and feel settings will be saved/loaded.";

            return content;
        }

        private void RenderObjectPlacementLookAndFeelToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForObjectPlacementLookAndFeelToggle(), _settings.ObjectPlacementLookAndFeel);
            if (newBool != _settings.ObjectPlacementLookAndFeel)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectPlacementLookAndFeel = newBool;
            }
        }

        private GUIContent GetContentForObjectPlacementLookAndFeelToggle()
        {
            var content = new GUIContent();
            content.text = "Object placement look and feel";
            content.tooltip = "If checked, the current object placement look and feel settings will be saved/loaded.";

            return content;
        }

        private void RenderObjectSelectionLookAndFeelToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForObjectSelectionLookAndFeelToggle(), _settings.ObjectSelectionLookAndFeel);
            if (newBool != _settings.ObjectSelectionLookAndFeel)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectSelectionLookAndFeel = newBool;
            }
        }

        private GUIContent GetContentForObjectSelectionLookAndFeelToggle()
        {
            var content = new GUIContent();
            content.text = "Object selection look and feel";
            content.tooltip = "If checked, the current object selection look and feel settings will be saved/loaded.";

            return content;
        }

        private void RenderObjectErasingLookAndFeelToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForObjectErasingLookAndFeelToggle(), _settings.ObjectErasingLookAndFeel);
            if (newBool != _settings.ObjectErasingLookAndFeel)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectErasingLookAndFeel = newBool;
            }
        }

        private GUIContent GetContentForObjectErasingLookAndFeelToggle()
        {
            var content = new GUIContent();
            content.text = "Object erasing look and feel";
            content.tooltip = "If checked, the current object erasing look and feel settings will be saved/loaded.";

            return content;
        }
        
        private void RenderCheckAllButton()
        {
            if(GUILayout.Button(GetContentForCheckAllButton(), GUILayout.Width(100.0f)))
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ToggleAll(true);
            }
        }

        private GUIContent GetContentForCheckAllButton()
        {
            var content = new GUIContent();
            content.text = "Check all";
            content.tooltip = "All settings can be saved/loaded.";

            return content;
        }

        private void RenderUncheckAllButton()
        {
            if (GUILayout.Button(GetContentForUncheckAllButton(), GUILayout.Width(100.0f)))
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ToggleAll(false);
            }
        }

        private GUIContent GetContentForUncheckAllButton()
        {
            var content = new GUIContent();
            content.text = "Uncheck all";
            content.tooltip = "None of the settings can be saved/loaded.";

            return content;
        }
        #endregion
    }
}
#endif