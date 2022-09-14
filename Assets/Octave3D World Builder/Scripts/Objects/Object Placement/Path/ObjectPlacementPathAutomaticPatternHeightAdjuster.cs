#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathAutomaticPatternHeightAdjuster
    {
        #region Public Methods
        public void AdjustSegmentHeight(ObjectPlacementBoxStackSegment segment, List<ObjectPlacementBoxStackSegment> allPathSegments, 
                                        ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings automaticPatternHeightAdjustmentSettings, ObjectPlacementPathHeightPattern heightPattern)
        {
            int initialHeightValueIndex = 0;
            if(automaticPatternHeightAdjustmentSettings.ApplyPatternsContinuously)
            {
                int indexOfSegmentToAdjust = allPathSegments.FindIndex(0, item => item == segment);
                if (indexOfSegmentToAdjust >= 0) initialHeightValueIndex = ObjectPlacementBoxStackSegmentQueries.GetTotalNumberOfStacksInSegments(allPathSegments, indexOfSegmentToAdjust - 1);
            }

            bool wrapPattern = automaticPatternHeightAdjustmentSettings.WrapPatterns;
            for(int stackIndex = 0; stackIndex < segment.NumberOfStacks; ++stackIndex)
            {
                segment.GetStackByIndex(stackIndex).SetHeight(heightPattern.GetHeightValue(initialHeightValueIndex + stackIndex, wrapPattern));
            }
        }

        public void AdjustHeightForAllSegmentsInPath(List<ObjectPlacementBoxStackSegment> allPathSegments, ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings automaticPatternHeightAdjustmentSettings, ObjectPlacementPathHeightPattern heightPattern)
        {
            bool wrapPattern = automaticPatternHeightAdjustmentSettings.WrapPatterns;
            if(automaticPatternHeightAdjustmentSettings.ApplyPatternsContinuously)
            {
                int numberOfTraversedStacks = 0;
                foreach (ObjectPlacementBoxStackSegment segment in allPathSegments)
                {
                    for (int stackIndex = 0; stackIndex < segment.NumberOfStacks; ++stackIndex)
                    {
                        segment.GetStackByIndex(stackIndex).SetHeight(heightPattern.GetHeightValue(numberOfTraversedStacks + stackIndex, wrapPattern));
                    }

                    numberOfTraversedStacks += segment.NumberOfStacks;
                }
            }
            else
            {
                foreach (ObjectPlacementBoxStackSegment segment in allPathSegments)
                {
                    for (int stackIndex = 0; stackIndex < segment.NumberOfStacks; ++stackIndex)
                    {
                        segment.GetStackByIndex(stackIndex).SetHeight(heightPattern.GetHeightValue(stackIndex, wrapPattern));
                    }
               }
            }
        }
        #endregion
    }
}
#endif