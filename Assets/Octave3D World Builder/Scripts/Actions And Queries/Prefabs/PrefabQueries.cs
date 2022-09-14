#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabQueries
    {
        #region Public Static Functions
        public static Prefab GetActivePrefab()
        {
            PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
            return activePrefabCategory != null ? activePrefabCategory.ActivePrefab : null;
        }

        public static List<OrientedBox> GetHierarchyWorldOrientedBoxesForAllPrefabs(List<Prefab> prefabs)
        {
            List<OrientedBox> orientedBoxes = new List<OrientedBox>(prefabs.Count);
            for (int prefabIndex = 0; prefabIndex < prefabs.Count; ++prefabIndex)
            {
                orientedBoxes.Add(prefabs[prefabIndex].UnityPrefab.GetHierarchyWorldOrientedBox());
            }

            return orientedBoxes;
        }

        public static List<Transform> GetTransformsForAllPrefabs(List<Prefab> prefabs)
        {
            List<Transform> transforms = new List<Transform>(prefabs.Count);
            for (int prefabIndex = 0; prefabIndex < prefabs.Count; ++prefabIndex)
            {
                transforms.Add(prefabs[prefabIndex].UnityPrefab.transform);
            }

            return transforms;
        }
        #endregion
    }
}
#endif