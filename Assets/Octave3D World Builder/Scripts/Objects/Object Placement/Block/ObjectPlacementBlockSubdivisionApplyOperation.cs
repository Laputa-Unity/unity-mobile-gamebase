#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementBlockSubdivisionApplyOperation
    {
        #region Public Methods
        public void ApplySubdivisionToEntireBlock(List<ObjectPlacementBoxStackSegment> allBlockSegments, ObjectPlacementBlockSubdivisionSettings subdivisionSettings)
        {
            // No subdivision required?
            if (!subdivisionSettings.SubdivideAlongGrowDirection && !subdivisionSettings.SubdivideAlongExtensionRight && !subdivisionSettings.SubdivideAlongExtensionLook) return;

            // Note: The following code assumes the segments' stacks extend along the extension plane right axis
            //       and the segments themselves are arranged along the plane's look axis.
            for (int segmentIndex = 0; segmentIndex < allBlockSegments.Count; ++segmentIndex)
            {
                int numberOfTraversedSegments = segmentIndex + 1;
                ObjectPlacementBoxStackSegment segment = allBlockSegments[segmentIndex];
                segment.ClearHideFlagInAllStacks(ObjectPlacementBoxHideFlags.BlockApplySubdivisions);

                // If we must subdivide along the extension look axis, we may need to hide the entire segment based
                // on the number of segments we have traversed so far.
                if (subdivisionSettings.SubdivideAlongExtensionLook)
                {
                    // First calculate the remainder of how many pairs of <subidvision, gap> we have traversed
                    int subdivisionGapPairSize = subdivisionSettings.SubdivisionGapSizeAlongExtensionLook + subdivisionSettings.SubdivisionSizeAlongExtensionLook;
                    int remainder = numberOfTraversedSegments % subdivisionGapPairSize;

                    // If the remainder is 0 it means we are at the end of a pair and the end of a pair is always a gap. So we activate
                    // the hide flags. We also activate the hide flags if the remainder is bigger than the subdivision size. In that case
                    // it means we reside somewhere inside the gap.
                    if (remainder == 0 || remainder > subdivisionSettings.SubdivisionSizeAlongExtensionLook) segment.SetHideFlagInAllStacks(ObjectPlacementBoxHideFlags.BlockApplySubdivisions);
                }

                for (int stackIndex = 0; stackIndex < segment.NumberOfStacks; ++stackIndex)
                {
                    int numberOfTraversedStacks = stackIndex + 1;
                    ObjectPlacementBoxStack stack = segment.GetStackByIndex(stackIndex);
                    if (subdivisionSettings.SubdivideAlongExtensionRight)
                    {
                        int subdivisionGapPairSize = subdivisionSettings.SubdivisionGapSizeAlongExtensionRight + subdivisionSettings.SubdivisionSizeAlongExtensionRight;
                        int remainder = numberOfTraversedStacks % subdivisionGapPairSize;
                        if (remainder == 0 || remainder > subdivisionSettings.SubdivisionSizeAlongExtensionRight) stack.SetHideFlagForAllBoxes(ObjectPlacementBoxHideFlags.BlockApplySubdivisions);
                    }

                    for (int boxIndex = 0; boxIndex < stack.NumberOfBoxes; ++boxIndex)
                    {
                        int numberOfTraversedBoxes = boxIndex + 1;
                        if (subdivisionSettings.SubdivideAlongGrowDirection)
                        {
                            int subdivisionGapPairSize = subdivisionSettings.SubdivisionGapSizeAlongGrowDirection + subdivisionSettings.SubdivisionSizeAlongGrowDirection;
                            int remainder = numberOfTraversedBoxes % subdivisionGapPairSize;
                            if (remainder == 0 || remainder > subdivisionSettings.SubdivisionSizeAlongGrowDirection) stack.GetBoxByIndex(boxIndex).SetHideFlag(ObjectPlacementBoxHideFlags.BlockApplySubdivisions);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
#endif