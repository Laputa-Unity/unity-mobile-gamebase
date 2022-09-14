#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathManualHeightAdjuster
    {
        #region Public Methods
        public int Raise(ObjectPlacementPath path, int currentPathHeight)
        {
            currentPathHeight += path.Settings.ManualConstructionSettings.HeightAdjustmentSettings.ManualHeightAdjustmentSettings.RaiseAmount;
            ObjectPlacementBoxStackSegmentActions.SetHeightForSegments(path.GetAllSegments(), currentPathHeight);

            return currentPathHeight;
        }

        public int Lower(ObjectPlacementPath path, int currentPathHeight)
        {
            currentPathHeight -= path.Settings.ManualConstructionSettings.HeightAdjustmentSettings.ManualHeightAdjustmentSettings.LowerAmount;
            ObjectPlacementBoxStackSegmentActions.SetHeightForSegments(path.GetAllSegments(), currentPathHeight);

            return currentPathHeight;
        }

        public void AdjustHeightForSegments(List<ObjectPlacementBoxStackSegment> segments, int desiredHeight)
        {
            foreach(ObjectPlacementBoxStackSegment segment in segments)
            {
                AdjustSegmentHeight(segment, desiredHeight);
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