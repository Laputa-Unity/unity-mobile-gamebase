#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class DecorPaintObjectPlacementBrushFactory
    {
        #region Public Static Functions
        public static DecorPaintObjectPlacementBrush Create(string name, List<string> existingBrushNames)
        {
            if (!string.IsNullOrEmpty(name)) return CreateNewBrushWithUniqueName(name, existingBrushNames);
            else
            {
                Debug.LogWarning("Null or empty brush names are not allowed. Please specify a valid brush name.");
                return null;
            }
        }
        #endregion

        #region Private Static Functions
        private static DecorPaintObjectPlacementBrush CreateNewBrushWithUniqueName(string name, List<string> existingBrushNames)
        {
            DecorPaintObjectPlacementBrush newBrush = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<DecorPaintObjectPlacementBrush>();
            newBrush.Name = UniqueEntityNameGenerator.GenerateUniqueName(name, existingBrushNames);

            return newBrush;
        }
        #endregion
    }
}
#endif