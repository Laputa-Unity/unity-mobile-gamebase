#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectPlacementBoxStackSegmentActions
    {
        #region Public Static Functions
        public static void ClearHideFlagsForAllStacksInSegments(List<ObjectPlacementBoxStackSegment> segments, ObjectPlacementBoxHideFlags hideFlags)
        {
            foreach (ObjectPlacementBoxStackSegment segment in segments)
            {
                segment.ClearHideFlagInAllStacks(hideFlags);
            }
        }

        public static void ReverseExtensionDirectionForSegments(List<ObjectPlacementBoxStackSegment> segments)
        {
            foreach (ObjectPlacementBoxStackSegment segment in segments)
            {
                segment.ReverseExtensionDirection();
            }
        }

        public static void ShrinkSegmentsByAmount(List<ObjectPlacementBoxStackSegment> segments, int amount)
        {
            foreach (ObjectPlacementBoxStackSegment segment in segments)
            {
                segment.Shrink(amount);
            }
        }

        public static void ExtendSegmentsByAmount(List<ObjectPlacementBoxStackSegment> segments, int amount)
        {
            foreach (ObjectPlacementBoxStackSegment segment in segments)
            {
                segment.Extend(amount);
            }
        }

        public static void SetPaddingForSegments(List<ObjectPlacementBoxStackSegment> segments, float paddingAlongExtensionDirection, float paddingAlongStackGrowDirection)
        {
            foreach (ObjectPlacementBoxStackSegment segment in segments)
            {
                segment.SetPaddingAlongExtensionDirection(paddingAlongExtensionDirection);
                segment.SetPaddingAlongStackGrowDirection(paddingAlongStackGrowDirection);
            }
        }

        public static void SetHeightForSegments(List<ObjectPlacementBoxStackSegment> segments, int height)
        {
            foreach(ObjectPlacementBoxStackSegment segment in segments)
            {
                segment.SetHeightForAllStacks(height);
            }
        }

        public static void SetBoxSizeForSegments(List<ObjectPlacementBoxStackSegment> segments, Vector3 boxSize)
        {
            foreach (ObjectPlacementBoxStackSegment segment in segments)
            {
                segment.SetBoxSizeForAllStacks(boxSize);
            }
        }
        #endregion
    }
}
#endif