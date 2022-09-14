#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabsToPathTileConectionButtonDropSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabsToPathTileConectionButtonDropSettings _settings;
        #endregion

        #region Constructors
        public PrefabsToPathTileConectionButtonDropSettingsView(PrefabsToPathTileConectionButtonDropSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderDestinationCategoryForDroppedPrefabsSelectionPopup();
        }
        #endregion

        #region Private Methods
        private void RenderDestinationCategoryForDroppedPrefabsSelectionPopup()
        {
            string newString = EditorGUILayoutEx.Popup(GetContentForDestinationCategoryForDroppedPrefabsSelectionPopup(),
                                                       _settings.DestinationCategoryForDroppedPrefabs.Name, PrefabCategoryDatabase.Get().GetAllPrefabCategoryNames());
            if (newString != _settings.DestinationCategoryForDroppedPrefabs.Name)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.DestinationCategoryForDroppedPrefabs = PrefabCategoryDatabase.Get().GetPrefabCategoryByName(newString);
            }
        }

        private GUIContent GetContentForDestinationCategoryForDroppedPrefabsSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Destination prefab category";
            content.tooltip = "When a prefab is associated with a tile connection, that prefab will be assigned to this category";

            return content;
        }
        #endregion
    }
}
#endif