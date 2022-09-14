#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementBlockAutomaticRandomHeightAdjuster
    {
        #region Public Methods
        public void AdjustHeightForSegments(List<ObjectPlacementBoxStackSegment> segments, int indexOfFirstSegmentToAdjust, int indexOfFirstStackToAdjust, ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings automaticRandomHeightAdjustmentSettings)
        {
            for (int segmentIndex = indexOfFirstSegmentToAdjust; segmentIndex < segments.Count; ++segmentIndex)
            {
                AdjustSegmentHeight(segments[segmentIndex], indexOfFirstStackToAdjust, automaticRandomHeightAdjustmentSettings);
            }
        }

        public void AdjustSegmentHeight(ObjectPlacementBoxStackSegment segment, int indexOfFirstStackToAdjust, ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings automaticRandomHeightAdjustmentSettings)
        {
            Range<int> randomValueRange = new Range<int>(automaticRandomHeightAdjustmentSettings.MinHeight, automaticRandomHeightAdjustmentSettings.MaxHeight);
            int numberOfStacksToAdjust = segment.NumberOfStacks - indexOfFirstStackToAdjust;
            segment.SetHeightForStacksStartingAt(indexOfFirstStackToAdjust, RandomValueGeneration.GenerateIntRandomValuesInRange(randomValueRange, numberOfStacksToAdjust));
        }
        #endregion
    }
}
#endif