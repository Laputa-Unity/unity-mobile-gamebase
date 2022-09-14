#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementBlockManualHeightAdjuster
    {
        #region Public Methods
        public int Raise(ObjectPlacementBlock block, int currentBlockHeight)
        {
            currentBlockHeight += block.Settings.ManualConstructionSettings.HeightAdjustmentSettings.ManualHeightAdjustmentSettings.RaiseAmount;
            ObjectPlacementBoxStackSegmentActions.SetHeightForSegments(block.GetAllSegments(), currentBlockHeight);

            return currentBlockHeight;
        }

        public int Lower(ObjectPlacementBlock block, int currentBlockHeight)
        {
            currentBlockHeight -= block.Settings.ManualConstructionSettings.HeightAdjustmentSettings.ManualHeightAdjustmentSettings.LowerAmount;
            ObjectPlacementBoxStackSegmentActions.SetHeightForSegments(block.GetAllSegments(), currentBlockHeight);

            return currentBlockHeight;
        }

        public void AdjustHeightForSegments(List<ObjectPlacementBoxStackSegment> segments, int desiredHeight)
        {
            foreach (ObjectPlacementBoxStackSegment segment in segments)
            {
                AdjustSegmentHeight(segment, desiredHeight);
            }
        }

        public void AdjustHeightForSegments(List<ObjectPlacementBoxStackSegment> segments, int indexOfFirstStackToAdjust, int desiredHeight)
        {
            foreach (ObjectPlacementBoxStackSegment segment in segments)
            {
                AdjustSegmentHeight(segment, indexOfFirstStackToAdjust, desiredHeight);
            }
        }

        public void AdjustSegmentHeight(ObjectPlacementBoxStackSegment segment, int desiredHeight)
        {
            segment.SetHeightForAllStacks(desiredHeight);
        }

        public void AdjustSegmentHeight(ObjectPlacementBoxStackSegment segment, int indexOfFirstStackToAdjust, int desiredHeight)
        {
            segment.SetHeightForStacksStartingAt(indexOfFirstStackToAdjust, desiredHeight);
        }
        #endregion
    }
}
#endif