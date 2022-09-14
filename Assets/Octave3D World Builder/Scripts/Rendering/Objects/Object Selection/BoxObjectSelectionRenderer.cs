#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class BoxObjectSelectionRenderer : ObjectSelectionRenderer
    {
        #region Public Methods
        public override void Render(List<GameObject> selectedObjects)
        {
            ObjectSelectionRenderSettings selectionRenderSettings = ObjectSelectionRenderSettings.Get();
            ObjectSelectionBoxRenderModeSettings selectionBoxRenderModeSettings = selectionRenderSettings.BoxRenderModeSettings;
            ObjectSelectionBoxCornerEdgesRenderModeSettings selectionBoxCornerEdgesRenderModeSettings = selectionBoxRenderModeSettings.CornerEdgesRenderModeSettings;

            Color boxColor = selectionBoxRenderModeSettings.BoxColor;
            Color edgeColor = selectionBoxRenderModeSettings.EdgeColor;
            float boxScale = selectionBoxRenderModeSettings.BoxScale;

            bool renderBoxes = boxColor.a != 0.0f;
            bool renderEdges = edgeColor.a != 0.0f;
            if (!renderBoxes && !renderEdges) return;

            // Note: Code duplication is intentional here in order to avoid further abstractions which may hinder performance.
            if(selectionBoxRenderModeSettings.EdgeRenderMode == ObjectSelectionBoxEdgeRenderMode.Wire)
            {
                if(renderBoxes && renderEdges)
                {
                    foreach (GameObject gameObject in selectedObjects)
                    {
                        if (!gameObject.activeInHierarchy) continue;

                        OrientedBox worldOrientedBox = gameObject.GetWorldOrientedBox();
                        if (worldOrientedBox.IsValid())
                        {
                            worldOrientedBox.Scale *= boxScale;
                            GizmosEx.RenderOrientedBox(worldOrientedBox, boxColor);
                            GizmosEx.RenderOrientedBoxEdges(worldOrientedBox, edgeColor);
                        }
                    }
                }
                else
                if(renderEdges && !renderBoxes)
                {
                    foreach (GameObject gameObject in selectedObjects)
                    {
                        if (!gameObject.activeInHierarchy) continue;

                        OrientedBox worldOrientedBox = gameObject.GetWorldOrientedBox();
                        if(worldOrientedBox.IsValid())
                        {
                            worldOrientedBox.Scale *= boxScale;
                            GizmosEx.RenderOrientedBoxEdges(worldOrientedBox, edgeColor);
                        }
                    }
                }
                else
                if(renderBoxes && !renderEdges)
                {
                    foreach (GameObject gameObject in selectedObjects)
                    {
                        if (!gameObject.activeInHierarchy) continue;

                        OrientedBox worldOrientedBox = gameObject.GetWorldOrientedBox();
                        if (worldOrientedBox.IsValid())
                        {
                            worldOrientedBox.Scale *= boxScale;
                            GizmosEx.RenderOrientedBox(worldOrientedBox, boxColor);
                        }
                    }
                }

            }
            else
            if(selectionBoxRenderModeSettings.EdgeRenderMode == ObjectSelectionBoxEdgeRenderMode.CornerEdges)
            {
                float cornerEdgeLengthPercentage = selectionBoxCornerEdgesRenderModeSettings.CornerEdgeLengthPercentage;
                if (renderBoxes && renderEdges)
                {
                    foreach (GameObject gameObject in selectedObjects)
                    {
                        if (!gameObject.activeInHierarchy) continue;

                        OrientedBox worldOrientedBox = gameObject.GetWorldOrientedBox();
                        if (worldOrientedBox.IsValid())
                        {
                            worldOrientedBox.Scale *= boxScale;
                            GizmosEx.RenderOrientedBox(worldOrientedBox, boxColor);
                            GizmosEx.RenderOrientedBoxCornerEdges(worldOrientedBox, cornerEdgeLengthPercentage, edgeColor);
                        }
                    }
                }
                else
                if (renderEdges && !renderBoxes)
                {
                    foreach (GameObject gameObject in selectedObjects)
                    {
                        if (!gameObject.activeInHierarchy) continue;

                        OrientedBox worldOrientedBox = gameObject.GetWorldOrientedBox();
                        if (worldOrientedBox.IsValid())
                        {
                            worldOrientedBox.Scale *= boxScale;
                            GizmosEx.RenderOrientedBoxCornerEdges(worldOrientedBox, cornerEdgeLengthPercentage, edgeColor);
                        }
                    }
                }
                else
                if(renderBoxes && !renderEdges)
                {
                    foreach (GameObject gameObject in selectedObjects)
                    {
                        if (!gameObject.activeInHierarchy) continue;

                        OrientedBox worldOrientedBox = gameObject.GetWorldOrientedBox();
                        if (worldOrientedBox.IsValid())
                        {
                            worldOrientedBox.Scale *= boxScale;
                            GizmosEx.RenderOrientedBox(worldOrientedBox, boxColor);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
#endif