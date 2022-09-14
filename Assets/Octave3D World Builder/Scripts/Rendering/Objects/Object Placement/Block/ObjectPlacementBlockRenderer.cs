#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementBlockRenderer
    {
        #region Public Methods
        public void RenderGizmos(ObjectPlacementBlock block)
        {
            if (block.IsUnderManualConstruction) RenderBlockUnderManualConstruction(block);
        }
        #endregion

        #region Private Methods
        private void RenderBlockUnderManualConstruction(ObjectPlacementBlock block)
        {
            List<ObjectPlacementBoxStackSegment> allBlockSegments = block.GetAllSegments();
            ObjectPlacementBlockManualConstructionRenderSettings renderSettings = block.RenderSettings.ManualConstructionRenderSettings;

            Vector3 boxOffsetAlongExtensionPlaneNormal = block.Settings.ManualConstructionSettings.OffsetAlongGrowDirection * block.ExtensionPlane.normal;
            foreach (ObjectPlacementBoxStackSegment segment in allBlockSegments)
            {
                for (int stackIndex = 0; stackIndex < segment.NumberOfStacks; ++stackIndex)
                {
                    ObjectPlacementBoxStack stack = segment.GetStackByIndex(stackIndex);
                    if (stack.IsOverlappedByAnotherStack) continue;

                    for (int boxIndex = 0; boxIndex < stack.NumberOfBoxes; ++boxIndex)
                    {
                        ObjectPlacementBox placementBox = stack.GetBoxByIndex(boxIndex);
                        if (placementBox.IsHidden) continue;

                        OrientedBox orientedBox = placementBox.OrientedBox;
                        orientedBox.Center += boxOffsetAlongExtensionPlaneNormal;
                        GizmosEx.RenderOrientedBoxEdges(orientedBox, renderSettings.BoxBorderLineColor);
                    }
                }
            }
        }
        #endregion
    }
}
#endif