#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabValidator
    {
        #region Public static functions
        public static bool ValidatePrefab(GameObject prefab, bool logWarningMessages)
        {
            if (prefab == null) return false;

            var prefabType = PrefabUtility.GetPrefabAssetType(prefab);
            if (prefabType == PrefabAssetType.NotAPrefab || prefabType == PrefabAssetType.Model)
            {
                if (logWarningMessages) Debug.LogWarning("The object '" + prefab.name + "' is not a prefab object.");
                return false;
            }

            if (prefab.GetComponent<Octave3DWorldBuilder>() != null)
            {
                if (logWarningMessages) Debug.LogWarning("The prefab '" + prefab.name + "' has a 'Octave3DWorldBuilder' script attached to it. This is not allowed.");
                return false;
            }

            Octave3DWorldBuilder[] octaveScripts = prefab.GetComponentsInChildren<Octave3DWorldBuilder>(true);
            if (octaveScripts != null && octaveScripts.Length != 0)
            {
                if (logWarningMessages) Debug.LogWarning("The prefab '" + prefab.name + "' contains children with a 'Octave3DWorldBuilder' script attached to them. This is not allowed.");
                return false;
            }

            Terrain[] terrains = prefab.GetComponentsInChildren<Terrain>(true);
            if(terrains != null && terrains.Length != 0)
            {
                if (logWarningMessages) Debug.LogWarning("The prefab '" + prefab.name + "' contains children with a 'Terrain' component attached to them. This is not allowed.");
                return false;
            }

            return true;
        }

        public static List<GameObject> GetValidPrefabsFromPrefabCollection(List<GameObject> prefabs, bool logWarningMessages)
        {
            var validPrefabs = new List<GameObject>();
            foreach(GameObject prefab in prefabs)
            {
                if (PrefabValidator.ValidatePrefab(prefab, logWarningMessages)) validPrefabs.Add(prefab);
            }

            return validPrefabs;
        }

        public static List<GameObject> GetValidPrefabsFromEntityCollection(UnityEngine.Object[] entities, bool logWarningMessages)
        {
            var validPrefabs = new List<GameObject>(entities.Length);
            foreach (var entity in entities)
            {
                GameObject prefab = entity as GameObject;
                if (PrefabValidator.ValidatePrefab(prefab, true)) validPrefabs.Add(prefab);
            }

            return validPrefabs;
        }
        #endregion
    }
}
#endif