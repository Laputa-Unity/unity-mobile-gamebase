#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectPlacementPathHeightPatternFactory
    {
        #region Public Static Functions
        public static ObjectPlacementPathHeightPattern Create(string name, List<string> existingPatternNames)
        {
            if (!string.IsNullOrEmpty(name)) return CreateNewPatternWithUniqueName(name, existingPatternNames);
            else
            {
                Debug.LogWarning("Null or empty pattern names are not allowed. Please specify a valid pattern name.");
                return null;
            }
        }
        #endregion

        #region Private Methods
        private static ObjectPlacementPathHeightPattern CreateNewPatternWithUniqueName(string name, List<string> existingPatternNames)
        {
            ObjectPlacementPathHeightPattern newPattern = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathHeightPattern>();
            newPattern.Name = UniqueEntityNameGenerator.GenerateUniqueName(name, existingPatternNames);

            return newPattern;
        }
        #endregion
    }
}
#endif