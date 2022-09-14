#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabActions
    {
        #region Public Static Functions
        public static void AssociatePrefabsWithTagCollection(List<Prefab> prefabs, List<string> tagNames)
        {
            foreach (Prefab prefab in prefabs)
            {
                prefab.TagAssociations.Associate(tagNames);
            }
        }

        public static void AssociatePrefabsWithTagCollection(List<Prefab> prefabs, List<PrefabTag> tags)
        {
            foreach (Prefab prefab in prefabs)
            {
                prefab.TagAssociations.Associate(tags);
            }
        }

        public static void AssociatePrefabsWithTag(List<Prefab> prefabs, string tagName)
        {
            foreach (Prefab prefab in prefabs)
            {
                prefab.TagAssociations.Associate(tagName);
            }
        }

        public static void AssociatePrefabsWithTag(List<Prefab> prefabs, PrefabTag tag)
        {
            foreach (Prefab prefab in prefabs)
            {
                prefab.TagAssociations.Associate(tag);
            }
        }
        #endregion
    }
}
#endif