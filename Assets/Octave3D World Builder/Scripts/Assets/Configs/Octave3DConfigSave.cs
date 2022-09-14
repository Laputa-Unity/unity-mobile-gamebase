#if UNITY_EDITOR
using UnityEngine;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace O3DWB
{
    public static class Octave3DConfigSave
    {
        #region Public Static Functions
        public static void SaveConfig(string fileName, Octave3DConfigSaveLoadSettings saveSettings)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            using (XmlTextWriter xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteNewLine(0);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.RootNode);

                SaveSnapSettings(xmlWriter, saveSettings);
                SaveObjectSelectionSettings(xmlWriter, saveSettings);
                SaveObjectErasingSettings(xmlWriter, saveSettings);
                SaveMirrorLookAndFeel(ObjectPlacement.Get().MirrorRenderSettings, xmlWriter, saveSettings, true);
                SaveMirrorLookAndFeel(ObjectSelection.Get().MirrorRenderSettings, xmlWriter, saveSettings, false);
                SaveSnapLookAndFeel(xmlWriter, saveSettings);
                SaveObjectPlacementLookAndFeel(xmlWriter, saveSettings);
                SaveObjectSelectionLookAndFeel(xmlWriter, saveSettings);
                SaveObjectErasingLookAndFeel(xmlWriter, saveSettings);

                xmlWriter.WriteNewLine(0);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }
        #endregion

        #region Private Static Functions
        private static void SaveSnapSettings(XmlTextWriter xmlWriter, Octave3DConfigSaveLoadSettings saveSettings)
        {
            if(saveSettings.SnapSettings)
            {
                ObjectSnapSettings snapSettings = ObjectSnapSettings.Get();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapSettingsNode);

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.UseOriginalPivotNode);
                xmlWriter.WriteString(snapSettings.UseOriginalPivot.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.EnableObjectSurfaceGrid);
                xmlWriter.WriteString(snapSettings.EnableObjectSurfaceGrid.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapToCursorHitPointNode);
                xmlWriter.WriteString(snapSettings.SnapToCursorHitPoint.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapCenterToCenterGridNode);
                xmlWriter.WriteString(snapSettings.SnapCenterToCenterForXZGrid.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapCenterToCenterObjectSurfaceNode);
                xmlWriter.WriteString(snapSettings.SnapCenterToCenterForObjectSurface.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.EnableObjectToObjectSnapNode);
                xmlWriter.WriteString(snapSettings.EnableObjectToObjectSnap.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectToObjectSnapEpsilonNode);
                xmlWriter.WriteString(snapSettings.ObjectToObjectSnapEpsilon.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapGridXOffsetNode);
                xmlWriter.WriteString(snapSettings.XZSnapGridXOffset.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapGridYOffsetNode);
                xmlWriter.WriteString(snapSettings.XZSnapGridYOffset.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapGridYOffsetStepNode);
                xmlWriter.WriteString(snapSettings.XZGridYOffsetStep.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapGridZOffsetNode);
                xmlWriter.WriteString(snapSettings.XZSnapGridZOffset.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSurfaceGridDesiredCellSizeNode);
                xmlWriter.WriteString(snapSettings.ObjectColliderSnapSurfaceGridSettings.DesiredCellSize.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapGridCellSizeXNode);
                xmlWriter.WriteString(ObjectSnapping.Get().XZSnapGrid.CellSizeSettings.CellSizeX.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapGridCellSizeZNode);
                xmlWriter.WriteString(ObjectSnapping.Get().XZSnapGrid.CellSizeSettings.CellSizeZ.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteEndElement();
            }
        }

        private static void SaveObjectSelectionSettings(XmlTextWriter xmlWriter, Octave3DConfigSaveLoadSettings saveSettings)
        {
            if(saveSettings.ObjectSelectionSettings)
            {
                ObjectSelectionSettings objectSelectionSettings = ObjectSelectionSettings.Get();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionSettingsNode);

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionAttachToObjectGroupNode);
                xmlWriter.WriteString(objectSelectionSettings.ObjectGroupSettings.AttachToObjectGroup.ToString());
                xmlWriter.WriteEndElement();

                if (objectSelectionSettings.ObjectGroupSettings.DestinationGroup != null &&
                    objectSelectionSettings.ObjectGroupSettings.DestinationGroup.GroupObject != null)
                {
                    xmlWriter.WriteNewLine(2);
                    xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionDestinationObjectGroupNode);
                    xmlWriter.WriteString(objectSelectionSettings.ObjectGroupSettings.DestinationGroup.GroupObject.name);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionXRotationStepNode);
                xmlWriter.WriteString(objectSelectionSettings.XRotationStep.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionYRotationStepNode);
                xmlWriter.WriteString(objectSelectionSettings.YRotationStep.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionZRotationStepNode);
                xmlWriter.WriteString(objectSelectionSettings.ZRotationStep.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionRotateAroundCenterNode);
                xmlWriter.WriteString(objectSelectionSettings.RotateAroundSelectionCenter.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionObject2ObjectSnapSettingsNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionObj2ObjSnapEpsNode);
                xmlWriter.WriteString(objectSelectionSettings.Object2ObjectSnapSettings.SnapEps.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionObj2ObjSnapCanHoverObjectsNode);
                xmlWriter.WriteString(objectSelectionSettings.Object2ObjectSnapSettings.CanHoverObjects.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionAllowPartialOverlapNode);
                xmlWriter.WriteString(objectSelectionSettings.AllowPartialOverlap.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionShapeTypeNode);
                xmlWriter.WriteString(objectSelectionSettings.SelectionShapeType.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionUpdateModeNode);
                xmlWriter.WriteString(objectSelectionSettings.SelectionUpdateMode.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionModeNode);
                xmlWriter.WriteString(objectSelectionSettings.SelectionMode.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionPaint_ShapePixelWidthNode);
                xmlWriter.WriteString(objectSelectionSettings.PaintModeSettings.SelectionShapeWidthInPixels.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionPaint_ShapePixelHeightNode);
                xmlWriter.WriteString(objectSelectionSettings.PaintModeSettings.SelectionShapeHeightInPixels.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionPaint_SizeAdjustmentSpeedScrollWheelNode);
                xmlWriter.WriteString(objectSelectionSettings.PaintModeSettings.ScrollWheelShapeSizeAdjustmentSpeed.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteEndElement();
            }
        }

        private static void SaveObjectErasingSettings(XmlTextWriter xmlWriter, Octave3DConfigSaveLoadSettings saveSettings)
        {
            if(saveSettings.ObjectErasingSettings)
            {
                ObjectEraserSettings eraserSettings = ObjectEraserSettings.Get();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectErasingSettingsNode);

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectErasingAllowUndoRedoNode);
                xmlWriter.WriteString(eraserSettings.AllowUndoRedo.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectEraseDelayNode);
                xmlWriter.WriteString(eraserSettings.EraseDelayInSeconds.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectEraseModeNode);
                xmlWriter.WriteString(eraserSettings.EraseMode.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectEraseOnlyMeshObjectsNode);
                xmlWriter.WriteString(eraserSettings.EraseOnlyMeshObjects.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectMassErase2D_CircleRadiusNode);
                xmlWriter.WriteString(eraserSettings.Mass2DEraseSettings.CircleShapeRadius.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectMassErase2D_RadiusAdjustmentSpeedScrollWheelNode);
                xmlWriter.WriteString(eraserSettings.Mass2DEraseSettings.ScrollWheelCircleRadiusAdjustmentSpeed.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectMassErase2D_AllowPartialOverlap);
                xmlWriter.WriteString(eraserSettings.Mass2DEraseSettings.AllowPartialOverlap.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectMassErase3D_CircleRadiusNode);
                xmlWriter.WriteString(eraserSettings.Mass3DEraseSettings.CircleShapeRadius.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectMassErase3D_RadiusAdjustmentSpeedScrollWheelNode);
                xmlWriter.WriteString(eraserSettings.Mass3DEraseSettings.ScrollWheelCircleRadiusAdjustmentSpeed.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectMassErase3D_AllowPartialOverlap);
                xmlWriter.WriteString(eraserSettings.Mass3DEraseSettings.AllowPartialOverlap.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteEndElement();
            }
        }

        private static void SaveMirrorLookAndFeel(InteractableMirrorRenderSettings renderSettings, XmlTextWriter xmlWriter, Octave3DConfigSaveLoadSettings saveSettings, bool objectPlacementMirror)
        {
            if(saveSettings.MirrorLookAndFeel)
            {
                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteStartElement(objectPlacementMirror ? Octave3DConfigXMLInfo.ObjectPlacementMirrorLookAndFeelNode : Octave3DConfigXMLInfo.ObjectSelectionMirrorLookAndFeelNode);

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.MirrorWidthNode);
                xmlWriter.WriteString(renderSettings.MirrorWidth.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.MirrorHeightNode);
                xmlWriter.WriteString(renderSettings.MirrorHeight.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.MirrorHasInfiniteWidthNode);
                xmlWriter.WriteString(renderSettings.UseInfiniteWidth.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.MirrorHasInfiniteHeightNode);
                xmlWriter.WriteString(renderSettings.UseInfiniteHeight.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.MirrorColorNode);
                xmlWriter.WriteColorString(renderSettings.MirrorPlaneColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.MirrorBorderColorNode);
                xmlWriter.WriteColorString(renderSettings.MirrorPlaneBorderLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.MirroredBoxColorNode);
                xmlWriter.WriteColorString(renderSettings.MirroredBoxColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.MirroredBoxBorderColorNode);
                xmlWriter.WriteColorString(renderSettings.MirroredBoxBorderLineColor);
                xmlWriter.WriteEndElement();
                
                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteEndElement();
            }
        }

        private static void SaveSnapLookAndFeel(XmlTextWriter xmlWriter, Octave3DConfigSaveLoadSettings saveSettings)
        {
            if(saveSettings.SnapLookAndFeel)
            {
                XZGridRenderSettings xzGridRenderSettings = ObjectSnapping.Get().XZSnapGrid.RenderSettings;
                XZGridRenderSettings objSurfaceGridRenderSettings = ObjectSnapping.Get().RenderSettingsForColliderSnapSurfaceGrid;
                CoordinateSystemRenderSettings coordSystemRenderSettings = ObjectSnapping.Get().XZSnapGrid.RenderableCoordinateSystem.RenderSettings;

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.SnapLookAndFeelNode);

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.XZGridLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.IsXZGridVisibleNode);
                xmlWriter.WriteString(xzGridRenderSettings.IsVisible.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.XZGridCellLineColorNode);
                xmlWriter.WriteColorString(xzGridRenderSettings.CellLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.XZGridCellLineThicknessNode);
                xmlWriter.WriteString(xzGridRenderSettings.CellLineThickness.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.XZGridPlaneColorNode);
                xmlWriter.WriteColorString(xzGridRenderSettings.PlaneColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.XZGridCoordSystemLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.IsXZGridCoordSystemVisibleNode);
                xmlWriter.WriteString(coordSystemRenderSettings.IsVisible.ToString());
                xmlWriter.WriteEndElement();

                List<CoordinateSystemAxis> allAxes = CoordinateSystemAxes.GetAll();
                foreach(var axis in allAxes)
                {
                    xmlWriter.WriteNewLine(3);
                    xmlWriter.WriteStartElement(axis.ToString());

                    xmlWriter.WriteNewLine(4);
                    xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.IsXZGridCoordSystemAxisVisibleNode);
                    xmlWriter.WriteString(coordSystemRenderSettings.IsAxisVisible(axis).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteNewLine(4);
                    xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.IsXZGridCoordSystemAxisInfiniteNode);
                    xmlWriter.WriteString(coordSystemRenderSettings.IsAxisRenderedInfinite(axis).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteNewLine(4);
                    xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.XZGridCoordSystemAxisFiniteSizeNode);
                    xmlWriter.WriteString(coordSystemRenderSettings.GetAxisFinitSize(axis).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteNewLine(4);
                    xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.XZGridCoordSystemAxisColorNode);
                    xmlWriter.WriteColorString(coordSystemRenderSettings.GetAxisColor(axis));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteNewLine(3);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSnapSurfaceGridLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.IsObjectSnapSurfaceGridVisibleNode);
                xmlWriter.WriteString(objSurfaceGridRenderSettings.IsVisible.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSnapSurfaceGridCellLineColorNode);
                xmlWriter.WriteColorString(objSurfaceGridRenderSettings.CellLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSnapSurfaceGridCellLineThicknessNode);
                xmlWriter.WriteString(objSurfaceGridRenderSettings.CellLineThickness.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSnapSurfaceGridPlaneColorNode);
                xmlWriter.WriteColorString(objSurfaceGridRenderSettings.PlaneColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteEndElement();
            }
        }

        private static void SaveObjectPlacementLookAndFeel(XmlTextWriter xmlWriter, Octave3DConfigSaveLoadSettings saveSettings)
        {
            if(saveSettings.ObjectPlacementLookAndFeel)
            {
                ObjectPivotPointsRenderSettings pivotPointsRenderSettings = ObjectPlacement.Get().GuidePivotPointsRenderSettings;
                ProjectedBoxFacePivotPointsRenderSettings projectedPivotPointRenderSettings = pivotPointsRenderSettings.ProjectedBoxFacePivotPointsRenderSettings;
                ObjectVertexSnapSessionRenderSettings vertexSnapRenderSettings = ObjectPlacement.Get().ObjectVertexSnapSessionRenderSettings;

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectPlacementLookAndFeelNode);

                // Pivot points
                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.GuidePivotPointsLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.GuidePivotPointsShapeTypeNode);
                xmlWriter.WriteString(pivotPointsRenderSettings.ShapeType.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.GuidePivotPointsShapeSizeInPixelsNode);
                xmlWriter.WriteString(pivotPointsRenderSettings.PivotPointSizeInPixels.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.DrawPivotPointProjectionLinesNode);
                xmlWriter.WriteString(projectedPivotPointRenderSettings.RenderProjectionLines.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointProjectionLineColorNode);
                xmlWriter.WriteColorString(projectedPivotPointRenderSettings.ProjectionLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.DrawPivotPointConnectionLinesNode);
                xmlWriter.WriteString(projectedPivotPointRenderSettings.RenderPivotPointConnectionLines.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointConnectionLineColorNode);
                xmlWriter.WriteColorString(projectedPivotPointRenderSettings.PivotPointConnectionLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ActivePivotPointLookAndFeelNode);

                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointIsVisibleNode);
                xmlWriter.WriteString(projectedPivotPointRenderSettings.ActivePivotPointRenderSettings.IsVisible.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointFillColorNode);
                xmlWriter.WriteColorString(projectedPivotPointRenderSettings.ActivePivotPointRenderSettings.FillColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointBorderColorNode);
                xmlWriter.WriteColorString(projectedPivotPointRenderSettings.ActivePivotPointRenderSettings.BorderLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointScaleNode);
                xmlWriter.WriteString(projectedPivotPointRenderSettings.ActivePivotPointRenderSettings.Scale.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.InactivePivotPointLookAndFeelNode);

                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointIsVisibleNode);
                xmlWriter.WriteString(projectedPivotPointRenderSettings.InactivePivotPointRenderSettings.IsVisible.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointFillColorNode);
                xmlWriter.WriteColorString(projectedPivotPointRenderSettings.InactivePivotPointRenderSettings.FillColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointBorderColorNode);
                xmlWriter.WriteColorString(projectedPivotPointRenderSettings.InactivePivotPointRenderSettings.BorderLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PivotPointScaleNode);
                xmlWriter.WriteString(projectedPivotPointRenderSettings.InactivePivotPointRenderSettings.Scale.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                // Vertex snapping
                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectVertexSnappingLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectVertexSnappingDrawSrcVertexNode);
                xmlWriter.WriteString(vertexSnapRenderSettings.RenderSourceVertex.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectVertexSnappingSrcVertexFillColorNode);
                xmlWriter.WriteColorString(vertexSnapRenderSettings.SourceVertexFillColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectVertexSnappingSrcVertexBorderColorNode);
                xmlWriter.WriteColorString(vertexSnapRenderSettings.SourceVertexBorderColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectVertexSnappingSrcVertexPixelRadiusNode);
                xmlWriter.WriteString(vertexSnapRenderSettings.SourceVertexRadiusInPixels.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                // Decor paint
                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.DecorPaintLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.DecorPaintBrushCircleLookAndFeelNode);

                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.DecorPaintBrushCircleBorderLineColorNode);
                xmlWriter.WriteColorString(ObjectPlacement.Get().DecorPaintObjectPlacement.BrushCircleRenderSettings.BorderLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                // Path
                ObjectPlacementPathRenderSettings pathRenderSettings = ObjectPlacement.Get().PathObjectPlacement.PathRenderSettings;
                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PathLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.PathBorderLineColorNode);
                xmlWriter.WriteColorString(pathRenderSettings.ManualConstructionRenderSettings.BoxBorderLineColor);
                xmlWriter.WriteEndElement();

                SaveExtensionPlaneLookAndFeel(ObjectPlacement.Get().PathObjectPlacement.PathExtensionPlaneRenderSettings, xmlWriter, 3);

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                // Block
                ObjectPlacementBlockRenderSettings blockRenderSettings = ObjectPlacement.Get().BlockObjectPlacement.BlockRenderSettings;
                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.BlockLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.BlockBorderLineColorNode);
                xmlWriter.WriteColorString(blockRenderSettings.ManualConstructionRenderSettings.BoxBorderLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.BlockTextColorNode);
                xmlWriter.WriteColorString(blockRenderSettings.ManualConstructionRenderSettings.DimensionsLabelRenderSettings.TextColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.BlockFontSizeNode);
                xmlWriter.WriteString(blockRenderSettings.ManualConstructionRenderSettings.DimensionsLabelRenderSettings.FontSize.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.BlockBoldTextNode);
                xmlWriter.WriteString(blockRenderSettings.ManualConstructionRenderSettings.DimensionsLabelRenderSettings.Bold.ToString());
                xmlWriter.WriteEndElement();

                SaveExtensionPlaneLookAndFeel(ObjectPlacement.Get().BlockObjectPlacement.BlockExtensionPlaneRenderSettings, xmlWriter, 3);

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteEndElement();
            }
        }

        private static void SaveExtensionPlaneLookAndFeel(ObjectPlacementExtensionPlaneRenderSettings extensionPlaneRenderSettings, XmlTextWriter xmlWriter, int indent)
        {
            xmlWriter.WriteNewLine(indent);
            xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ExtensionPlaneLookAndFeelNode);

            int childIndent = indent + 1;
            xmlWriter.WriteNewLine(childIndent);
            xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ExtensionPlaneScaleNode);
            xmlWriter.WriteString(extensionPlaneRenderSettings.PlaneScale.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(childIndent);
            xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ExtensionPlaneColorNode);
            xmlWriter.WriteColorString(extensionPlaneRenderSettings.PlaneColor);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(childIndent);
            xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ExtensionPlaneBorderColorNode);
            xmlWriter.WriteColorString(extensionPlaneRenderSettings.PlaneBorderLineColor);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(childIndent);
            xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ExtensionPlaneNormalLineLengthNode);
            xmlWriter.WriteString(extensionPlaneRenderSettings.PlaneNormalLineLength.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(childIndent);
            xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ExtensionPlaneNormalLineColorNode);
            xmlWriter.WriteColorString(extensionPlaneRenderSettings.PlaneNormalLineColor);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(indent);
            xmlWriter.WriteEndElement();
        }

        private static void SaveObjectSelectionLookAndFeel(XmlTextWriter xmlWriter, Octave3DConfigSaveLoadSettings saveSettings)
        {
            if(saveSettings.ObjectSelectionLookAndFeel)
            {
                ObjectSelectionRenderSettings selectionRenderSettings = ObjectSelection.Get().RenderSettings;
                RectangleShapeRenderSettings rectRenderSettings = ObjectSelection.Get().RectangleSelectionShapeRenderSettings;
                EllipseShapeRenderSettings ellipseRenderSettings = ObjectSelection.Get().EllipseSelectionShapeRenderSettings;

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionLookAndFeelNode);

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionBoxEdgeDrawModeNode);
                xmlWriter.WriteString(selectionRenderSettings.BoxRenderModeSettings.EdgeRenderMode.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionBoxCornerEdgeLengthPercentageNode);
                xmlWriter.WriteString(selectionRenderSettings.BoxRenderModeSettings.CornerEdgesRenderModeSettings.CornerEdgeLengthPercentage.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionBoxEdgeColorNode);
                xmlWriter.WriteColorString(selectionRenderSettings.BoxRenderModeSettings.EdgeColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionBoxColorNode);
                xmlWriter.WriteColorString(selectionRenderSettings.BoxRenderModeSettings.BoxColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionBoxScaleNode);
                xmlWriter.WriteString(selectionRenderSettings.BoxRenderModeSettings.BoxScale.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionRectLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionShapeFillColorNode);
                xmlWriter.WriteColorString(rectRenderSettings.FillColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionShapeBorderColorNode);
                xmlWriter.WriteColorString(rectRenderSettings.BorderLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionEllipseLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionShapeFillColorNode);
                xmlWriter.WriteColorString(ellipseRenderSettings.FillColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectSelectionShapeBorderColorNode);
                xmlWriter.WriteColorString(ellipseRenderSettings.BorderLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteEndElement();
            }
        }

        private static void SaveObjectErasingLookAndFeel(XmlTextWriter xmlWriter, Octave3DConfigSaveLoadSettings saveSettings)
        {
            if(saveSettings.ObjectErasingLookAndFeel)
            {
                EllipseShapeRenderSettings circle2DRenderSettings = ObjectEraser.Get().Circle2DMassEraseShapeRenderSettings;
                XZOrientedEllipseShapeRenderSettings circle3DRenderSettings = ObjectEraser.Get().Circle3DMassEraseShapeRenderSettings;

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectErasingLookAndFeelNode);

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectErasing2DCircleLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectErasingShapeFillColorNode);
                xmlWriter.WriteColorString(circle2DRenderSettings.FillColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectErasingShapeBorderColorNode);
                xmlWriter.WriteColorString(circle2DRenderSettings.BorderLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectErasing3DCircleLookAndFeelNode);

                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(Octave3DConfigXMLInfo.ObjectErasingShapeBorderColorNode);
                xmlWriter.WriteColorString(circle3DRenderSettings.BorderLineColor);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteNewLine(1);
                xmlWriter.WriteEndElement();
            }
        }
        #endregion
    }
}
#endif