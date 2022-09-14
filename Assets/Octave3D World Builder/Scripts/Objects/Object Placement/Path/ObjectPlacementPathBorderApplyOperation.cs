#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathBorderApplyOperation
    {
        #region Public Methods
        public void ApplyBordersToAllPathSegments(List<ObjectPlacementBoxStackSegment> pathSegments, ObjectPlacementPathBorderSettings borderSettings)
        {
            int totalNumberOfStacksInSegments = ObjectPlacementBoxStackSegmentQueries.GetTotalNumberOfStacksInSegments(pathSegments);
            int numberOfTraversedStacks = 0;
            for(int segmentIndex = 0; segmentIndex < pathSegments.Count; ++segmentIndex)
            {
                ObjectPlacementBoxStackSegment segment = pathSegments[segmentIndex];
                for(int stackIndex = 0; stackIndex < segment.NumberOfStacks; ++stackIndex)
                {
                    ObjectPlacementBoxStack stack = segment.GetStackByIndex(stackIndex);
                    for(int boxIndex = 0; boxIndex < stack.NumberOfBoxes; ++boxIndex)
                    {
                        ObjectPlacementBox box = stack.GetBoxByIndex(boxIndex);
                        box.ClearHideFlag(ObjectPlacementBoxHideFlags.PathApplyBorders);   // Not hidden by default. We have to check against the borders to see if the box needs to be hidden.

                        // Check if the box is outside the bottom and top borders
                        if (boxIndex >= borderSettings.BottomBorderWidth && boxIndex < (stack.NumberOfBoxes - borderSettings.TopBorderWidth))
                        {
                            // Check special case in which the current segment has the same extension direction as the one which precedes it
                            // but is aiming in the opposite direction. In that case, the first stack will never be masked because it looks
                            // weird.
                            if (stackIndex == 0 && segmentIndex != 0)
                            {
                                ObjectPlacementBoxStackSegment previousSegment = pathSegments[segmentIndex - 1];
                                bool extendingInSameDirection;
                                if(previousSegment.ExtensionDirection.IsAlignedWith(segment.ExtensionDirection, out extendingInSameDirection))
                                {
                                    // If the segments are aligned but they are extending in opposite directions, it means that 
                                    // we have to leave the boxes in this stack untouched.
                                    if (!extendingInSameDirection) continue;
                                }
                            }

                            if (numberOfTraversedStacks >= borderSettings.BeginBorderWidth &&
                                numberOfTraversedStacks < totalNumberOfStacksInSegments - borderSettings.EndBorderWidth) box.SetHideFlag(ObjectPlacementBoxHideFlags.PathApplyBorders);
                        }
                    }

                    ++numberOfTraversedStacks;
                }
            }
        }
        #endregion
    }
}
#endif