#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabCategoryFactory
    {
        #region Public Static Functions
        public static PrefabCategory Create(string name, List<string> existingCategoryNames)
        {
            if (!string.IsNullOrEmpty(name)) return CreateNewPrefabCategoryWithUniqueName(name, existingCategoryNames);
            else
            {
                Debug.LogWarning("Null or empty category names are not allowed. Please specify a valid prefab category name.");
                return null;
            }
        }
        #endregion

        #region Private Static Functions
        private static PrefabCategory CreateNewPrefabCategoryWithUniqueName(string name, List<string> existingCategoryNames)
        {
            PrefabCategory newPrefabCategory = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabCategory>();
            newPrefabCategory.Name = UniqueEntityNameGenerator.GenerateUniqueName(name, existingCategoryNames);

            return newPrefabCategory;
        }
        #endregion
    }
}
#endif