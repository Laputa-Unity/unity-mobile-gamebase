#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionPrefabCreationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSelectionPrefabCreationSettings _settings;
        #endregion

        #region Constructors
        public ObjectSelectionPrefabCreationSettingsView(ObjectSelectionPrefabCreationSettings settings)
        {
            _settings = settings;

            SurroundWithBox = true;
            VisibilityToggleLabel = "Prefab Creation Settings";
            ToggleVisibilityBeforeRender = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderDestinationCategorySelectionPopup();
            RenderPivotSelectionPopup();
            RenderPrefabNameField();
            RenderPrefabDestinationFolderField();
            RenderCreatePrefabButton();
        }
        #endregion

        #region Private Methods
        private void RenderDestinationCategorySelectionPopup()
        {
            string newString = EditorGUILayoutEx.Popup(GetContentForDestinationCategorySelectionPopup(), _settings.DestinationCategory.Name, PrefabCategoryDatabase.Get().GetAllPrefabCategoryNames());
            if(newString != _settings.DestinationCategory.Name)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.DestinationCategory = PrefabCategoryDatabase.Get().GetPrefabCategoryByName(newString);
            }
        }

        private GUIContent GetContentForDestinationCategorySelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Destination category";
            content.tooltip = "This is the prefab category in which the prefab will be placed after being created.";

            return content;
        }

        private void RenderPivotSelectionPopup()
        {
            Pivot newPivot = (Pivot)EditorGUILayout.EnumPopup(GetContentForPivotSelectionPopup(), _settings.Pivot);
            if(newPivot != _settings.Pivot)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.Pivot = newPivot;
            }
        }

        private GUIContent GetContentForPivotSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Pivot";
            content.tooltip = "Allows you to choose a predefined pivot for the prefab. Useful when placing objects using the original prefab pivot.";

            return content;
        }

        private void RenderPrefabNameField()
        {
            string newString = EditorGUILayout.TextField(GetContentForPrefabNameField(), _settings.PrefabName);
            if(newString != _settings.PrefabName)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.PrefabName = newString;
            }
        }

        private GUIContent GetContentForPrefabNameField()
        {
            var content = new GUIContent();
            content.text = "Prefab name";
            content.tooltip = "Allows you to specify the name of the prefab asset that will be created.";

            return content;
        }

        private void RenderPrefabDestinationFolderField()
        {
            EditorGUILayout.HelpBox("For the prefab destination folder, the full folder name is required. For example: Assets/Abandoned City/Props/Created Prefabs. You can also " + 
                                    "drag and drop a folder onto the text field area in which case the folder name will automatically appear in the field. ", UnityEditor.MessageType.Info);
            string newString = EditorGUILayout.TextField(GetContentForPrefabDestinationFolderField(), _settings.DestinationFolder);
            if(newString != _settings.DestinationFolder)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.DestinationFolder = newString;
            }

            Rect textFieldRect = GUILayoutUtility.GetLastRect();
            FolderToPrefabCreationFolderField.Get().Handle(Event.current, textFieldRect);
        }

        private GUIContent GetContentForPrefabDestinationFolderField()
        {
            var content = new GUIContent();
            content.text = "Prefab destination folder";
            content.tooltip = "This is the name of the folder in which the prefab asset will be created.";

            return content;
        }

        private void RenderCreatePrefabButton()
        {
            if(GUILayout.Button(GetContentForCreatePrefabButton(), GUILayout.Width(180.0f)))
            {
                PrefabFactory.CreateFromSelectedObjects(_settings.Pivot);
            }
        }

        private GUIContent GetContentForCreatePrefabButton()
        {
            var content = new GUIContent();
            content.text = "Create prefab from selection";
            content.tooltip = "You can press this button to create a prefab from all selected objects. Note: Only mesh, light and particle system objects " + 
                              "are taken into account. Empty game objects are ignored.";

            return content;
        }
        #endregion
    }
}
#endif