#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectPlacementBoxStackSegmentQueries
    {
        #region Public Static Functions
        public static int GetTotalNumberOfStacksInSegments(List<ObjectPlacementBoxStackSegment> segments)
        {
            int totalNumberOfStacks = 0;
            foreach(ObjectPlacementBoxStackSegment segment in segments)
            {
                totalNumberOfStacks += segment.NumberOfStacks;
            }

            return totalNumberOfStacks;
        }

        public static int GetTotalNumberOfStacksInSegments(List<ObjectPlacementBoxStackSegment> segments, int lastSegmentIndex)
        {
            int totalNumberOfStacks = 0;
            for (int segmentIndex = 0; segmentIndex <= lastSegmentIndex; ++segmentIndex)
            {
                totalNumberOfStacks += segments[segmentIndex].NumberOfStacks;
            }

            return totalNumberOfStacks;
        }
        #endregion
    }
}
#endif