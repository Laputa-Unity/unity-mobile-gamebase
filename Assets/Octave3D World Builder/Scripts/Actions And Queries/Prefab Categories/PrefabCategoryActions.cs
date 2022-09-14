#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabCategoryActions
    {
        #region Public Static Functions
        public static void SetPrefabOffsetFromGridSurface(PrefabCategory prefabCategory, float offsetFromGridSurface)
        {
            List<Prefab> allPrefabsInCategory = prefabCategory.GetAllPrefabs();
            foreach(Prefab prefab in allPrefabsInCategory)
            {
                prefab.OffsetFromGridSurface = offsetFromGridSurface;
            }
        }

        public static void SetPrefabOffsetFromObjectSurface(PrefabCategory prefabCategory, float offsetFromObjectSurface)
        {
            List<Prefab> allPrefabsInCategory = prefabCategory.GetAllPrefabs();
            foreach (Prefab prefab in allPrefabsInCategory)
            {
                prefab.OffsetFromObjectSurface = offsetFromObjectSurface;
            }
        }

        public static void ActivateNextPrefabInPrefabCategory(PrefabCategory prefabCategory)
        {
            List<Prefab> filteredPrefabs = prefabCategory.GetFilteredPrefabs();

            int prefabIndex = prefabCategory.GetPrefabIndex(prefabCategory.ActivePrefab) + 1;
            if (prefabIndex >= prefabCategory.NumberOfPrefabs) prefabIndex = 0;
       
            while(prefabIndex < prefabCategory.NumberOfPrefabs)
            {
                Prefab newActivePrefab = prefabCategory.GetPrefabByIndex(prefabIndex);
                if (filteredPrefabs.Contains(newActivePrefab))
                {
                    prefabCategory.SetActivePrefab(newActivePrefab);
                    return;
                }

                ++prefabIndex;
            }
         
            prefabCategory.SetActivePrefab(null);
        }

        public static void ActivatePreviousPrefabInPrefabCategory(PrefabCategory prefabCategory)
        {
            List<Prefab> filteredPrefabs = prefabCategory.GetFilteredPrefabs();

            int prefabIndex = prefabCategory.GetPrefabIndex(prefabCategory.ActivePrefab) - 1;
            if (prefabIndex < 0) prefabIndex = prefabCategory.NumberOfPrefabs - 1;

            while(prefabIndex >= 0)
            {
                Prefab newActivePrefab = prefabCategory.GetPrefabByIndex(prefabIndex);
                if (filteredPrefabs.Contains(newActivePrefab))
                {
                    prefabCategory.SetActivePrefab(newActivePrefab);
                    return;
                }

                --prefabIndex;
            }

            prefabCategory.SetActivePrefab(null);
        }
        #endregion
    }
}
#endif