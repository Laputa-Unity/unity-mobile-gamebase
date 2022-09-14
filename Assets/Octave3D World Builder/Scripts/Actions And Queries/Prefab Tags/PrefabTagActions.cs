#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabTagActions
    {
        #region Public Static Functions
        public static void ActivatePrefabTags(List<PrefabTag> prefabTags)
        {
            foreach(PrefabTag prefabTag in prefabTags)
            {
                prefabTag.IsActive = true;
            }
        }

        public static void DeactivatePrefabTags(List<PrefabTag> prefabTags)
        {
            foreach (PrefabTag prefabTag in prefabTags)
            {
                prefabTag.IsActive = false;
            }
        }
        #endregion
    }
}
#endif