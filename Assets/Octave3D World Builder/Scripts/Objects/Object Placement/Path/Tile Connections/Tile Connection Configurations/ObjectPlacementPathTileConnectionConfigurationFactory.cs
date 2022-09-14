#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectPlacementPathTileConnectionConfigurationFactory
    {
        #region Public Static Functions
        public static ObjectPlacementPathTileConnectionConfiguration Create(string configurationName, List<string> existingConfigurationNames)
        {
            if (!string.IsNullOrEmpty(configurationName)) return CreateNewConfigurationWithUniqueName(configurationName, existingConfigurationNames);
            else
            {
                Debug.LogWarning("Null or empty configuration names are not allowed. Please specify a valid configuration name.");
                return null;
            }
        }
        #endregion

        #region Private Static Functions
        private static ObjectPlacementPathTileConnectionConfiguration CreateNewConfigurationWithUniqueName(string configurationName, List<string> existingConfigurationNames)
        {
            ObjectPlacementPathTileConnectionConfiguration newConfiguration = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathTileConnectionConfiguration>();
            newConfiguration.Name = UniqueEntityNameGenerator.GenerateUniqueName(configurationName, existingConfigurationNames);

            return newConfiguration;
        }
        #endregion
    }
}
#endif