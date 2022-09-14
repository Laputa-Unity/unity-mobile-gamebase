#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ActiveObjectPlacementPathTileConnectionConfigurationView : EntityView
    {
        #region Protected Methods
        protected override void RenderContent()
        {
            ObjectPlacementPathTileConnectionConfigurationDatabase configurationDatabase = ObjectPlacementPathTileConnectionConfigurationDatabase.Get();
            if (configurationDatabase.IsEmpty) EditorGUILayout.HelpBox("There are no tile connection configurations currently available.", UnityEditor.MessageType.None);
            else
            {
                RenderActiveConfigurationSelectionPopup();
                RenderActiveConfigurationNameChangeField();
            }
        }
        #endregion

        #region Private Methods
        private void RenderActiveConfigurationSelectionPopup()
        {
            ObjectPlacementPathTileConnectionConfigurationDatabase configurationDatabase = ObjectPlacementPathTileConnectionConfigurationDatabase.Get();
            string newString = EditorGUILayoutEx.Popup(GetContentForActiveConfigurationSelectionPopup(), configurationDatabase.ActiveConfiguration.Name, configurationDatabase.GetAllConfigurationNames());
            if(newString != configurationDatabase.ActiveConfiguration.Name)
            {
                ObjectPlacementPathTileConnectionConfiguration newActiveConfiguration = configurationDatabase.GetConfigurationByName(newString);
                if(newActiveConfiguration != null)
                {
                    UndoEx.RecordForToolAction(configurationDatabase);
                    PathObjectPlacement.Get().PathSettings.TileConnectionSettings.RecordAllTileConnectionTypeSettingsForUndo();
                    configurationDatabase.SetActiveConfiguration(newActiveConfiguration);
                    newActiveConfiguration.ApplyConfigurationDataToSettings(PathObjectPlacement.Get().PathSettings.TileConnectionSettings);
                }
            }
        }

        private GUIContent GetContentForActiveConfigurationSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Active configuration";
            content.tooltip = "Allows you to choose the active tile connection configuration.";

            return content;
        }

        private void RenderActiveConfigurationNameChangeField()
        {
            ObjectPlacementPathTileConnectionConfiguration activeConfiguration = ObjectPlacementPathTileConnectionConfigurationDatabase.Get().ActiveConfiguration;
            string newString = EditorGUILayoutEx.DelayedTextField(GetContentForActiveConfigurationNameChangeField(), activeConfiguration.Name);
            if (newString != activeConfiguration.Name)
            {
                UndoEx.RecordForToolAction(activeConfiguration);
                activeConfiguration.Name = newString;
            }
        }

        private GUIContent GetContentForActiveConfigurationNameChangeField()
        {
            var content = new GUIContent();
            content.text = "Name";
            content.tooltip = "Allows you to change the name of the active configuration.";

            return content;
        }
        #endregion
    }
}
#endif