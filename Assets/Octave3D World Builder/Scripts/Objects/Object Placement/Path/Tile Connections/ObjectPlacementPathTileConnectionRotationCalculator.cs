#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementPathTileConnectionRotationCalculator
    {
        #region Public Methods
        public Quaternion Calculate(ObjectPlacementPathTileConnectionGridCell tileConnectionGridCell)
        {
            ObjectPlacementPath path = tileConnectionGridCell.TileConnectionPath;
            ObjectPlacementPathTileConnectionSettings tileConnectionSettings = path.Settings.TileConnectionSettings;
            Plane extensionPlane = path.ExtensionPlane;
            ObjectPlacementBoxStackSegment tileConnectionSegment = tileConnectionGridCell.TileConnectionSegment;
            ObjectPlacementBoxStack tileConnectionStack = tileConnectionGridCell.TileConnectionStack;

            bool usingSprites = tileConnectionSettings.UsesSprites();
            float yAxisRotationInDegrees = ObjectPlacementPathTileConnectionYAxisRotations.GetAngleInDegrees(tileConnectionSettings.GetSettingsForTileConnectionType(tileConnectionGridCell.TileConnectionType).YAxisRotation);
            Quaternion yAxisRotation = Quaternion.AngleAxis(yAxisRotationInDegrees, extensionPlane.normal);

            Quaternion defaultRotation = Quaternion.LookRotation(path.ExtensionPlaneLookAxis, extensionPlane.normal);
            if (usingSprites) defaultRotation = Quaternion.LookRotation(extensionPlane.normal, path.ExtensionPlaneLookAxis);

            ObjectPlacementPathTileConnectionType tileConnectionType = tileConnectionGridCell.TileConnectionType;
            if (tileConnectionType == ObjectPlacementPathTileConnectionType.Autofill) return yAxisRotation * defaultRotation;
            else
                if (tileConnectionType == ObjectPlacementPathTileConnectionType.Begin)
                {
                    if (tileConnectionGridCell.NumberOfNeighbours == 0) return yAxisRotation * defaultRotation;
                    else
                    {
                        ObjectPlacementBoxStack neighbour = tileConnectionGridCell.GetFirstNeighbour().TileConnectionStack;
                        Vector3 toNeighbour = tileConnectionStack.GetNormalizedBasePositionConnectionVectorTo(neighbour);

                        if (usingSprites) return yAxisRotation * Quaternion.LookRotation(extensionPlane.normal, toNeighbour);
                        return yAxisRotation * Quaternion.LookRotation(toNeighbour, extensionPlane.normal);
                    }
                }
                else if (tileConnectionType == ObjectPlacementPathTileConnectionType.End)
                {
                    ObjectPlacementBoxStack neighbour = tileConnectionGridCell.GetFirstNeighbour().TileConnectionStack;
                    Vector3 toEndTile = neighbour.GetNormalizedBasePositionConnectionVectorTo(tileConnectionStack);

                    if (usingSprites) return yAxisRotation * Quaternion.LookRotation(extensionPlane.normal, toEndTile);
                    return yAxisRotation * Quaternion.LookRotation(toEndTile, extensionPlane.normal);
                }
                else
                    if (tileConnectionType == ObjectPlacementPathTileConnectionType.Cross) return yAxisRotation * defaultRotation;
                    else
                        if (tileConnectionType == ObjectPlacementPathTileConnectionType.TJunction)
                        {
                            ObjectPlacementBoxStack baseOfTJunction = null;
                            if (tileConnectionGridCell.RightNeighbour != null && tileConnectionGridCell.LeftNeighbour != null) baseOfTJunction = tileConnectionGridCell.ForwardNeighbour != null ? tileConnectionGridCell.ForwardNeighbour.TileConnectionStack : tileConnectionGridCell.BackNeighbour.TileConnectionStack;
                            else baseOfTJunction = tileConnectionGridCell.RightNeighbour != null ? tileConnectionGridCell.RightNeighbour.TileConnectionStack : tileConnectionGridCell.LeftNeighbour.TileConnectionStack;

                            Vector3 toTJunction = baseOfTJunction.GetNormalizedBasePositionConnectionVectorTo(tileConnectionStack);

                            if (usingSprites) return yAxisRotation * Quaternion.LookRotation(extensionPlane.normal, toTJunction);
                            return yAxisRotation * Quaternion.LookRotation(toTJunction, extensionPlane.normal);
                        }
                        else
                            if (tileConnectionType == ObjectPlacementPathTileConnectionType.Forward)
                            {
                                if (tileConnectionSegment.NumberOfStacks == 1)
                                {
                                    if (tileConnectionGridCell.HasForwardAndBackNeightbours())
                                    {
                                        ObjectPlacementBoxStack forwardNeighbour = tileConnectionGridCell.ForwardNeighbour.TileConnectionStack;
                                        ObjectPlacementBoxStack backNeighbour = tileConnectionGridCell.BackNeighbour.TileConnectionStack;
                                        Vector3 toForwardNeighbour = backNeighbour.GetNormalizedBasePositionConnectionVectorTo(forwardNeighbour);

                                        if (usingSprites) return yAxisRotation * Quaternion.LookRotation(extensionPlane.normal, toForwardNeighbour);
                                        return yAxisRotation * Quaternion.LookRotation(toForwardNeighbour, extensionPlane.normal);
                                    }
                                    else
                                    {
                                        ObjectPlacementBoxStack leftNeighbour = tileConnectionGridCell.LeftNeighbour.TileConnectionStack;
                                        ObjectPlacementBoxStack rightNeighbour = tileConnectionGridCell.RightNeighbour.TileConnectionStack;
                                        Vector3 toRightNeighbour = leftNeighbour.GetNormalizedBasePositionConnectionVectorTo(rightNeighbour);

                                        if (usingSprites) return yAxisRotation * Quaternion.LookRotation(extensionPlane.normal, toRightNeighbour);
                                        return yAxisRotation * Quaternion.LookRotation(toRightNeighbour, extensionPlane.normal);
                                    }
                                }
                                else
                                {
                                    if (usingSprites) return yAxisRotation * Quaternion.LookRotation(extensionPlane.normal, tileConnectionSegment.ExtensionDirection);
                                    return yAxisRotation * Quaternion.LookRotation(tileConnectionSegment.ExtensionDirection, extensionPlane.normal);
                                }
                            }
                            else
                            {
                                ObjectPlacementBoxStack firstNeighbour, secondNeighbour;
                                if (tileConnectionGridCell.RightNeighbour != null)
                                {
                                    firstNeighbour = tileConnectionGridCell.RightNeighbour.TileConnectionStack;
                                    secondNeighbour = tileConnectionGridCell.ForwardNeighbour != null ? tileConnectionGridCell.ForwardNeighbour.TileConnectionStack : tileConnectionGridCell.BackNeighbour.TileConnectionStack;
                                }
                                else
                                    if (tileConnectionGridCell.LeftNeighbour != null)
                                    {
                                        firstNeighbour = tileConnectionGridCell.LeftNeighbour.TileConnectionStack;
                                        secondNeighbour = tileConnectionGridCell.ForwardNeighbour != null ? tileConnectionGridCell.ForwardNeighbour.TileConnectionStack : tileConnectionGridCell.BackNeighbour.TileConnectionStack;
                                    }
                                    else
                                        if (tileConnectionGridCell.ForwardNeighbour != null)
                                        {
                                            firstNeighbour = tileConnectionGridCell.ForwardNeighbour.TileConnectionStack;
                                            secondNeighbour = tileConnectionGridCell.RightNeighbour != null ? tileConnectionGridCell.RightNeighbour.TileConnectionStack : tileConnectionGridCell.LeftNeighbour.TileConnectionStack;
                                        }
                                        else
                                        {
                                            firstNeighbour = tileConnectionGridCell.BackNeighbour.TileConnectionStack;
                                            secondNeighbour = tileConnectionGridCell.RightNeighbour != null ? tileConnectionGridCell.RightNeighbour.TileConnectionStack : tileConnectionGridCell.LeftNeighbour.TileConnectionStack;
                                        }

                                Vector3 tileLook, tileRight;
                                tileLook = firstNeighbour.GetNormalizedBasePositionConnectionVectorTo(tileConnectionStack);
                                tileRight = tileConnectionStack.GetNormalizedBasePositionConnectionVectorTo(secondNeighbour);

                                Vector3 tileUp = Vector3.Cross(tileLook, tileRight);
                                if (Vector3.Dot(tileUp, extensionPlane.normal) < 0.0f)
                                {
                                    tileLook = -tileRight;
                                }

                                if (usingSprites) return yAxisRotation * Quaternion.LookRotation(extensionPlane.normal, tileLook);
                                return yAxisRotation * Quaternion.LookRotation(tileLook, extensionPlane.normal);
                            }
        }
        #endregion
    }
}
#endif