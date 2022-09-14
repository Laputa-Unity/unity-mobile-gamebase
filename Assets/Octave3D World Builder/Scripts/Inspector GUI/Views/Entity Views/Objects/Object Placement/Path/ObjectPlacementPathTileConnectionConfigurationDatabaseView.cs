#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathTileConnectionConfigurationDatabaseView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathTileConnectionConfigurationDatabase _database;

        [SerializeField]
        private ObjectPlacementPathTileConnectionConfigurationDatabaseViewData _viewData;
        #endregion

        #region Private Properties
        public ObjectPlacementPathTileConnectionConfigurationDatabaseViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathTileConnectionConfigurationDatabaseViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Constructors
        public ObjectPlacementPathTileConnectionConfigurationDatabaseView(ObjectPlacementPathTileConnectionConfigurationDatabase database)
        {
            _database = database;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderCreateNewConfigurationControls();
            RenderRemoveConfigurationControls();

            EditorGUILayout.BeginHorizontal();
            RenderSaveActiveConfigurationButton();
            RenderLoadActiveConfigurationButton();
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Private Methods
        private void RenderCreateNewConfigurationControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderCreateNewConfigurationButton();
            RenderCreateNewConfigurationNameChangeField();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderCreateNewConfigurationButton()
        {
            if(GUILayout.Button(GetContentForCreateNewConfigurationButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_database);
                ObjectPlacementPathTileConnectionConfiguration newConfig = _database.CreateConfiguration(ViewData.NameForNewConfiguration);
                newConfig.ExtractConfigurationDataFromSettings(PathObjectPlacement.Get().PathSettings.TileConnectionSettings);
            }
        }

        private GUIContent GetContentForCreateNewConfigurationButton()
        {
            var content = new GUIContent();
            content.text = "Create new configuration";
            content.tooltip = "Creates a new tile connection configuration using the name specified in the adjacent text field.";

            return content;
        }

        private void RenderCreateNewConfigurationNameChangeField()
        {
            string newString = EditorGUILayout.TextField(ViewData.NameForNewConfiguration);
            if (newString != ViewData.NameForNewConfiguration)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.NameForNewConfiguration = newString;
            }
        }

        private void RenderRemoveConfigurationControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderRemoveActiveConfigurationButton();
            RenderRemoveAllConfigurationsButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderRemoveActiveConfigurationButton()
        {
            if(GUILayout.Button(GetContentForRemoveActiveConfigurationButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_database);
                _database.RemoveAndDestroyConfiguration(_database.ActiveConfiguration);
            }
        }

        private GUIContent GetContentForRemoveActiveConfigurationButton()
        {
            var content = new GUIContent();
            content.text = "Remove active configuration";
            content.tooltip = "Removes the active configuration.";

            return content;
        }

        private void RenderRemoveAllConfigurationsButton()
        {
            if (GUILayout.Button(GetContentForRemoveAllConfigurationsButton()))
            {
                UndoEx.RecordForToolAction(_database);
                _database.RemoveAndDestroyAllConfigurations();
            }
        }

        private GUIContent GetContentForRemoveAllConfigurationsButton()
        {
            var content = new GUIContent();
            content.text = "Remove all configurations";
            content.tooltip = "Removes all configurations.";

            return content;
        }

        private void RenderSaveActiveConfigurationButton()
        {
            if(GUILayout.Button(GetContentForSaveActiveConfigurationButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)) &
               _database.ActiveConfiguration != null)
            {
                UndoEx.RecordForToolAction(_database.ActiveConfiguration);
                _database.ActiveConfiguration.ExtractConfigurationDataFromSettings(PathObjectPlacement.Get().PathSettings.TileConnectionSettings);
            }
        }

        private GUIContent GetContentForSaveActiveConfigurationButton()
        {
            var content = new GUIContent();
            content.text = "Save active configuration";
            content.tooltip = "Saves the current tile connection settings in the active configuration.";

            return content;
        }

        private void RenderLoadActiveConfigurationButton()
        {
            if (GUILayout.Button(GetContentForLoadActiveConfigurationButton()) && _database.ActiveConfiguration != null)
            {
                PathObjectPlacement.Get().PathSettings.TileConnectionSettings.RecordAllTileConnectionTypeSettingsForUndo();
                _database.ActiveConfiguration.ApplyConfigurationDataToSettings(PathObjectPlacement.Get().PathSettings.TileConnectionSettings);
            }
        }

        private GUIContent GetContentForLoadActiveConfigurationButton()
        {
            var content = new GUIContent();
            content.text = "Load active configuration";
            content.tooltip = "Loads the active configuration.";

            return content;
        }
        #endregion
    }
}
#endif