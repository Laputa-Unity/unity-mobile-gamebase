#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectPlacementBoxStackActions
    {
        #region Public Static Functions
        public static void MarkStacksAsOverlapped(List<ObjectPlacementBoxStack> stacks)
        {
            foreach (ObjectPlacementBoxStack stack in stacks)
            {
                stack.IsOverlappedByAnotherStack = true;
            }
        }
        #endregion
    }
}
#endif