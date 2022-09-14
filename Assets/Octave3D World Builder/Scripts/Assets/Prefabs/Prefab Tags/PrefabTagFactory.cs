#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabTagFactory
    {
        #region Public Static Functions
        public static PrefabTag Create(string name, List<string> existingTagNames)
        {
            if (!string.IsNullOrEmpty(name)) return CreatePrefabTagWithUniqueName(name, existingTagNames);
            else
            {
                Debug.LogWarning("Null or empty tag names are not allowed. Please specify a valid tag name.");
                return null;
            }
        }
        #endregion

        #region Private Static Functions
        private static PrefabTag CreatePrefabTagWithUniqueName(string name, List<string> existingTagNames)
        {
            PrefabTag prefabTag = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabTag>();
            prefabTag.Name = UniqueEntityNameGenerator.GenerateUniqueName(name, existingTagNames);

            return prefabTag;
        }
        #endregion
    }
}
#endif