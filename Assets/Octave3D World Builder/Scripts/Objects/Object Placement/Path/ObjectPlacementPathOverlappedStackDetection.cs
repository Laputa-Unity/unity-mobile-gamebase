#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectPlacementPathOverlappedStackDetection
    {
        #region Public Static Functions
        public static List<ObjectPlacementBoxStack> GetOverlappedStacksInSegment(int segmentIndex, List<ObjectPlacementBoxStackSegment> pathSegments)
        {
            var overlappedStacks = new List<ObjectPlacementBoxStack>(pathSegments.Count * 10);

            ObjectPlacementBoxStackSegment possiblyOverlappedSegment = pathSegments[segmentIndex];
            for (int possiblyOverlappingSegmentIndex = segmentIndex - 1; possiblyOverlappingSegmentIndex >= 0; --possiblyOverlappingSegmentIndex)
            {
                ObjectPlacementBoxStackSegment possiblyOverlappingSegment = pathSegments[possiblyOverlappingSegmentIndex];

                if (possiblyOverlappingSegment.ExtensionVectorIntersects(possiblyOverlappedSegment) ||
                    possiblyOverlappingSegment.ExtensionVectorOverlaps(possiblyOverlappedSegment))
                {
                    for (int possiblyOverlappedStackIndex = 0; possiblyOverlappedStackIndex < possiblyOverlappedSegment.NumberOfStacks; ++possiblyOverlappedStackIndex)
                    {
                        ObjectPlacementBoxStack possiblyOverlappedStack = possiblyOverlappedSegment.GetStackByIndex(possiblyOverlappedStackIndex);
                        for (int possiblyOverlappingStackIndex = 0; possiblyOverlappingStackIndex < possiblyOverlappingSegment.NumberOfStacks; ++possiblyOverlappingStackIndex)
                        {
                            ObjectPlacementBoxStack possiblyOverlappingStack = possiblyOverlappingSegment.GetStackByIndex(possiblyOverlappingStackIndex);
                            if (IsStackOverlappedBy(possiblyOverlappedStack, possiblyOverlappingStack)) overlappedStacks.Add(possiblyOverlappedStack);
                        }
                    }
                }
            }

            return overlappedStacks;
        }
        #endregion

        #region Private Static Functions
        private static bool IsStackOverlappedBy(ObjectPlacementBoxStack possiblyOveralppedStack, ObjectPlacementBoxStack possiblyOverlappingStack)
        {
            bool stacksAreAligned = possiblyOveralppedStack.UpAxis.IsAlignedWith(possiblyOverlappingStack.UpAxis);
            if (!stacksAreAligned) return false;

            Plane stackBasePlane = possiblyOveralppedStack.GetBasePlane();
            Vector3 basePositionVector = stackBasePlane.ProjectPoint(possiblyOverlappingStack.BasePosition) - stackBasePlane.ProjectPoint(possiblyOveralppedStack.BasePosition);
            if (basePositionVector.magnitude < 1e-3f) return true;

            return false;
        }
        #endregion
    }
}
#endif