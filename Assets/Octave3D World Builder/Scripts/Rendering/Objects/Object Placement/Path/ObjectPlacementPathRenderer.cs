#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathRenderer
    {
        #region Public Methods
        public void RenderGizmos(ObjectPlacementPath path)
        {
            if (path.IsUnderManualConstruction) RenderPathUnderManualConstruction(path);
        }
        #endregion

        #region Private Methods
        private void RenderPathUnderManualConstruction(ObjectPlacementPath path)
        {
            ObjectPlacementPathTileConnectionSettings tileConnectionSettings = path.Settings.TileConnectionSettings;
            if (tileConnectionSettings.UseTileConnections) RenderPathUnderManualConstructionWhenUsingTileConnections(path, tileConnectionSettings);
            else RenderPathUnderManualConstructionWhenNotUsingTileConnections(path);
        }

        private void RenderPathUnderManualConstructionWhenUsingTileConnections(ObjectPlacementPath path, ObjectPlacementPathTileConnectionSettings tileConnectionSettings)
        {
            List<ObjectPlacementPathTileConnectionGridCell> tileConnectionGridCells = path.TileConnectionGridCells;
            ObjectPlacementPathManualConstructionRenderSettings renderSettings = path.RenderSettings.ManualConstructionRenderSettings;
            var tileConnectionYOffsetVectorCalculator = new ObjectPlacementPathTileConnectionYOffsetVectorCalculator();

            // Loop through all tile connection grid cells. Each cell will give us access to all the information
            // that we need to perform the rendering operation.
            foreach (ObjectPlacementPathTileConnectionGridCell tileConnectionGridCell in tileConnectionGridCells)
            {
                // Use the cell to access the tile connection stack. If the stack is overlapped by another stack
                // or if no boxes exist in the stack, we can move on to the next cell.
                ObjectPlacementBoxStack tileConnectionStack = tileConnectionGridCell.TileConnectionStack;
                if (tileConnectionStack.IsOverlappedByAnotherStack || tileConnectionStack.NumberOfBoxes == 0) continue;

                // Calculate the Y offset vector which applies to all tiles in the current stack and then render each
                // box which resides inside the stack.
                Vector3 tileConnectionYOffsetVector = tileConnectionYOffsetVectorCalculator.Calculate(tileConnectionSettings.GetSettingsForTileConnectionType(tileConnectionGridCell.TileConnectionType), path);
                for (int boxIndex = 0; boxIndex < tileConnectionStack.NumberOfBoxes; ++boxIndex)
                {
                    // If the box is hidden, we can move on to the next box
                    ObjectPlacementBox placementBox = tileConnectionStack.GetBoxByIndex(boxIndex);
                    if (placementBox.IsHidden) continue;

                    // Retrieve the box and apply the Y offset vector. The render the box.
                    OrientedBox orientedBox = placementBox.OrientedBox;
                    orientedBox.Center += tileConnectionYOffsetVector;
                    GizmosEx.RenderOrientedBoxEdges(orientedBox, renderSettings.BoxBorderLineColor);
                }

                // We have to take tile connection extrusion into account, so we will need to retrieve the extrusion
                // boxes and render those too.
                List<OrientedBox> extrusionOrientedBoxes = ObjectPlacementPathTileConnectionExtrusion.GetTileConnectionExtrusionOrientedBoxes(tileConnectionGridCell);
                foreach (OrientedBox extrusionBox in extrusionOrientedBoxes) GizmosEx.RenderOrientedBoxEdges(extrusionBox, renderSettings.BoxBorderLineColor);
            }
        }

        private void RenderPathUnderManualConstructionWhenNotUsingTileConnections(ObjectPlacementPath path)
        {
            List<ObjectPlacementBoxStackSegment> allPathSegments = path.GetAllSegments();
            ObjectPlacementPathManualConstructionRenderSettings renderSettings = path.RenderSettings.ManualConstructionRenderSettings;
            Vector3 boxYOffsetvector = path.Settings.ManualConstructionSettings.OffsetAlongGrowDirection * path.ExtensionPlane.normal;

            foreach (ObjectPlacementBoxStackSegment segment in allPathSegments)
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
                        orientedBox.Center += boxYOffsetvector;
                        GizmosEx.RenderOrientedBoxEdges(orientedBox, renderSettings.BoxBorderLineColor);
                    }
                }
            }
        }
        #endregion
    }
}
#endif