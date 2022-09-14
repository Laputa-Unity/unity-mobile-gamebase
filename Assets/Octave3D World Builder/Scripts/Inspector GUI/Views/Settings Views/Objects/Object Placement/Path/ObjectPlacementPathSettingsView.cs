#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathSettings _settings;

        [SerializeField]
        private ActiveObjectPlacementPathTileConnectionConfigurationView _activeObjectPlacementPathTileConnectionConfigurationView = new ActiveObjectPlacementPathTileConnectionConfigurationView();
        #endregion

        #region Constructors
        public ObjectPlacementPathSettingsView(ObjectPlacementPathSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _settings.TileConnectionSettings.View.Render();

            if (_settings.TileConnectionSettings.UseTileConnections)
            {
                _activeObjectPlacementPathTileConnectionConfigurationView.Render();
                ObjectPlacementPathTileConnectionConfigurationDatabase.Get().View.Render();
            }

            EditorGUILayout.Separator();
            _settings.ManualConstructionSettings.View.Render();
        }
        #endregion
    }
}
#endif