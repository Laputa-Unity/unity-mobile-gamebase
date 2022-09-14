#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathAutomaticRandomHeightAdjuster
    {
        #region Public Methods
        public void AdjustSegmentHeight(ObjectPlacementBoxStackSegment segment, ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings automaticRandomHeightAdjustmentSettings)
        {   
            Range<int> randomValueRange = new Range<int>(automaticRandomHeightAdjustmentSettings.MinHeight, automaticRandomHeightAdjustmentSettings.MaxHeight);
            segment.SetHeightForAllStacks(RandomValueGeneration.GenerateIntRandomValuesInRange(randomValueRange, segment.NumberOfStacks));
        }

        public void AdjustHeightForSegments(List<ObjectPlacementBoxStackSegment> segments, ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings automaticRandomHeightAdjustmentSettings)
        {
            foreach (ObjectPlacementBoxStackSegment segment in segments)
            {
                AdjustSegmentHeight(segment, automaticRandomHeightAdjustmentSettings);
            }
        }

        public void AdjustSegmentHeight(ObjectPlacementBoxStackSegment segment, int indexOfFirstStackToAdjust, ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings automaticRandomHeightAdjustmentSettings)
        {
            Range<int> randomValueRange = new Range<int>(automaticRandomHeightAdjustmentSettings.MinHeight, automaticRandomHeightAdjustmentSettings.MaxHeight);
            int numberOfStacksToAdjust = segment.NumberOfStacks - indexOfFirstStackToAdjust;
            segment.SetHeightForStacksStartingAt(indexOfFirstStackToAdjust, RandomValueGeneration.GenerateIntRandomValuesInRange(randomValueRange, numberOfStacksToAdjust));
        }
        #endregion
    }
}
#endif