#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class PrefabWithPrefabCategoryAssociationFactory
    {
        #region Public Static Functions
        public static PrefabWithPrefabCategoryAssociation Create(Prefab prefab, PrefabCategory prefabCategory)
        {
            if (prefab != null && prefabCategory != null) return CreateNewAssociation(prefab, prefabCategory);
            else
            {
                Debug.LogWarning("Can not create the requested prefab with prefab category association. Null prefab or prefab category was specified.");
                return null;
            }
        }
        #endregion

        #region Private Static Functions
        private static PrefabWithPrefabCategoryAssociation CreateNewAssociation(Prefab prefab, PrefabCategory prefabCategory)
        {
            var association = new PrefabWithPrefabCategoryAssociation();
            association.Prefab = prefab;
            association.PrefabCategory = prefabCategory;

            return association;
        }
        #endregion
    }
}
#endif