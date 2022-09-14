#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathTileConnectionConfigurationDatabase : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementPathTileConnectionConfigurationCollection _configurations = new ObjectPlacementPathTileConnectionConfigurationCollection();

        [SerializeField]
        private ObjectPlacementPathTileConnectionConfigurationDatabaseView _view;
        #endregion

        #region Public Properties
        public bool IsEmpty { get { return _configurations.IsEmpty; } }
        public int NumberOfConfigurations { get { return _configurations.NumberOfEntities; } }
        public ObjectPlacementPathTileConnectionConfiguration ActiveConfiguration { get { return _configurations.MarkedEntity; } }
        public int IndexOfActiveConfiguration { get { return _configurations.IndexOfMarkedEntity; } }
        public ObjectPlacementPathTileConnectionConfigurationDatabaseView View { get { return _view; } }
        #endregion

        #region Public Static Functions
        public static ObjectPlacementPathTileConnectionConfigurationDatabase Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ObjectPlacementPathTileConnectionConfigurationDatabase;
        }
        #endregion

        #region Constructors
        public ObjectPlacementPathTileConnectionConfigurationDatabase()
        {
            _view = new ObjectPlacementPathTileConnectionConfigurationDatabaseView(this);
        }
        #endregion

        #region Public Methods
        public ObjectPlacementPathTileConnectionConfiguration CreateConfiguration(string configurationName)
        {
            if(!string.IsNullOrEmpty(configurationName))
            {
                ObjectPlacementPathTileConnectionConfiguration newConfiguration = ObjectPlacementPathTileConnectionConfigurationFactory.Create(configurationName, GetAllConfigurationNames());
                _configurations.AddEntity(newConfiguration);

                if (NumberOfConfigurations == 1) SetActiveConfiguration(newConfiguration);

                return newConfiguration;
            }

            return null;
        }

        public bool ContainsConfiguration(ObjectPlacementPathTileConnectionConfiguration configuration)
        {
            return _configurations.ContainsEntity(configuration);
        }

        public void RemoveAndDestroyConfiguration(ObjectPlacementPathTileConnectionConfiguration configuration)
        {
            if (ContainsConfiguration(configuration))
            {
                _configurations.RemoveEntity(configuration);
                UndoEx.DestroyObjectImmediate(configuration);
            }
        }

        public void RemoveAndDestroyAllConfigurations()
        {
            List<ObjectPlacementPathTileConnectionConfiguration> allConfigurations = GetAllConfigurations();
            foreach (ObjectPlacementPathTileConnectionConfiguration configuration in allConfigurations)
            {
                _configurations.RemoveEntity(configuration);
            }

            // Note: I can't understand why, but we need to destroy the scriptable object instances in a second pass. Otherwise,
            //       if the active configuration is set to the first configuration in the configuration list, the remove action
            //       can not be undone properly. The active configuration index (i.e. marked entity index) is not restored properly.
            foreach (ObjectPlacementPathTileConnectionConfiguration configuration in allConfigurations)
            {
                UndoEx.DestroyObjectImmediate(configuration);
            }
        }

        public void RenameConfiguration(ObjectPlacementPathTileConnectionConfiguration configuration, string newName)
        {
            if (ContainsConfiguration(configuration)) _configurations.RenameEntity(configuration, newName);
        }

        public ObjectPlacementPathTileConnectionConfiguration GetConfigurationByIndex(int index)
        {
            return _configurations.GetEntityByIndex(index);
        }

        public ObjectPlacementPathTileConnectionConfiguration GetConfigurationByName(string name)
        {
            return _configurations.GetEntityByName(name);
        }

        public List<ObjectPlacementPathTileConnectionConfiguration> GetAllConfigurations()
        {
            return _configurations.GetAllEntities();
        }

        public List<string> GetAllConfigurationNames()
        {
            return _configurations.GetAllEntityNames();
        }

        public void SetActiveConfiguration(ObjectPlacementPathTileConnectionConfiguration newActiveConfiguration)
        {
            if (newActiveConfiguration == null || !ContainsConfiguration(newActiveConfiguration)) return;

            _configurations.MarkEntity(newActiveConfiguration);
        }

        public void RecordAllConfigurationsForUndo()
        {
            List<ObjectPlacementPathTileConnectionConfiguration> allConfigs = GetAllConfigurations();
            foreach (var config in allConfigs) UndoEx.RecordForToolAction(config);
        }

        public void RemovePrefabAssociationForAllConfigurations(Prefab prefab)
        {
            List<ObjectPlacementPathTileConnectionConfiguration> allConfigs = GetAllConfigurations();
            foreach (var config in allConfigs) config.RemovePrefabAssociations(prefab);
        }

        public void RemovePrefabAssociationForAllConfigurations(List<Prefab> prefabs)
        {
            List<ObjectPlacementPathTileConnectionConfiguration> allConfigs = GetAllConfigurations();
            foreach (var config in allConfigs) config.RemovePrefabAssociations(prefabs);
        }
        #endregion
    }
}
#endif