#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectPlacementPathTileConnectionExtrusion
    {
        #region Public Static Functions
        public static List<OrientedBox> GetTileConnectionExtrusionOrientedBoxes(ObjectPlacementPathTileConnectionGridCell tileConnectionGridCell)
        {
            ObjectPlacementBoxStack tileConnectionStack = tileConnectionGridCell.TileConnectionStack;
            ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings = tileConnectionGridCell.TileConnectionPath.Settings.TileConnectionSettings.GetSettingsForTileConnectionType(tileConnectionGridCell.TileConnectionType);

            if (tileConnectionStack != null)
            {
                if (tileConnectionStack.NumberOfBoxes == 0) return new List<OrientedBox>();
                if (tileConnectionTypeSettings.IsAnyExtrusionNecessary())
                {
                    // Handle upwards extrusion
                    Vector3 nextTilePositionForUpwardsExtrusion;
                    Vector3 extrudeOffset;
                    Vector3 pathExtensionPlaneNormal = tileConnectionGridCell.TileConnectionPath.ExtensionPlane.normal;

                    var tileConnectionYOffsetVectorCalculator = new ObjectPlacementPathTileConnectionYOffsetVectorCalculator();
                    Vector3 yOffsetVector = tileConnectionYOffsetVectorCalculator.Calculate(tileConnectionTypeSettings, tileConnectionGridCell.TileConnectionPath);      

                    extrudeOffset = pathExtensionPlaneNormal * tileConnectionStack.GetBoxSizeAlongNormalizedDirection(pathExtensionPlaneNormal);
                    if (tileConnectionStack.IsGrowingUpwards) nextTilePositionForUpwardsExtrusion = tileConnectionStack.GetBoxByIndex(tileConnectionStack.NumberOfBoxes - 1).Center + extrudeOffset;
                    else nextTilePositionForUpwardsExtrusion = tileConnectionStack.GetBoxByIndex(0).Center + extrudeOffset;

                    var extrudedBox = new OrientedBox();
                    extrudedBox.Rotation = tileConnectionStack.Rotation;
                    extrudedBox.ModelSpaceSize = tileConnectionStack.BoxSize;

                    var extrusionOrientedBoxes = new List<OrientedBox>();
                    for (int extrudeIndex = 0; extrudeIndex < tileConnectionTypeSettings.UpwardsExtrusionAmount; ++extrudeIndex)
                    {
                        extrudedBox.Center = nextTilePositionForUpwardsExtrusion + yOffsetVector;
                        extrusionOrientedBoxes.Add(new OrientedBox(extrudedBox));

                        nextTilePositionForUpwardsExtrusion += extrudeOffset;
                    }

                    // Handle downwards extrusion
                    extrudeOffset = -pathExtensionPlaneNormal * tileConnectionStack.GetBoxSizeAlongNormalizedDirection(pathExtensionPlaneNormal);
                    if (tileConnectionStack.IsGrowingDownwards) nextTilePositionForUpwardsExtrusion = tileConnectionStack.GetBoxByIndex(tileConnectionStack.NumberOfBoxes - 1).Center + extrudeOffset;
                    else nextTilePositionForUpwardsExtrusion = tileConnectionStack.GetBoxByIndex(0).Center + extrudeOffset;

                    for (int extrudeIndex = 0; extrudeIndex < tileConnectionTypeSettings.DownwardsExtrusionAmount; ++extrudeIndex)
                    {
                        extrudedBox.Center = nextTilePositionForUpwardsExtrusion + yOffsetVector;
                        extrusionOrientedBoxes.Add(new OrientedBox(extrudedBox));

                        nextTilePositionForUpwardsExtrusion += extrudeOffset;
                    }

                    return extrusionOrientedBoxes;
                }
                else return new List<OrientedBox>();
            }
            else return new List<OrientedBox>();
        }
        #endregion
    }
}
#endif