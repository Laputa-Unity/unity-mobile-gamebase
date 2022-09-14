#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace O3DWB
{
    public static class Octave3DConfigLoad
    {
        #region Public Static Functions
        public static void LoadConfig(string fileName, Octave3DConfigSaveLoadSettings loadSettings)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            LoadSnapSettings(xmlDoc, loadSettings);
            LoadObjectSelectionSettings(xmlDoc, loadSettings);
            LoadObjectErasingSettings(xmlDoc, loadSettings);
            LoadMirrorLookAndFeel(xmlDoc, loadSettings, ObjectPlacement.Get().MirrorRenderSettings, true);
            LoadMirrorLookAndFeel(xmlDoc, loadSettings, ObjectSelection.Get().MirrorRenderSettings, false);
            LoadSnapLookAndFeel(xmlDoc, loadSettings);
            LoadObjectPlacementLookAndFeel(xmlDoc, loadSettings);
            LoadObjectSelectionLookAndFeel(xmlDoc, loadSettings);
            LoadObjectErasingLookAndFeel(xmlDoc, loadSettings);
        }
        #endregion

        #region Private Static Functions
        private static void LoadSnapSettings(XmlDocument xmlDoc, Octave3DConfigSaveLoadSettings loadSettings)
        {
            if(loadSettings.SnapSettings)
            {
                XmlNode snapSettingsNode = xmlDoc.SelectSingleNode("//" + Octave3DConfigXMLInfo.SnapSettingsNode);
                if(snapSettingsNode != null)
                {
                    ObjectSnapSettings snapSettings = ObjectSnapSettings.Get();

                    XmlNode node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.UseOriginalPivotNode);
                    if(node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) snapSettings.UseOriginalPivot = boolRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.EnableObjectSurfaceGrid);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) snapSettings.EnableObjectSurfaceGrid = boolRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.SnapToCursorHitPointNode);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) snapSettings.SnapToCursorHitPoint = boolRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.SnapCenterToCenterGridNode);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) snapSettings.SnapCenterToCenterForXZGrid = boolRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.SnapCenterToCenterObjectSurfaceNode);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) snapSettings.SnapCenterToCenterForObjectSurface = boolRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.EnableObjectToObjectSnapNode);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) snapSettings.EnableObjectToObjectSnap = boolRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectToObjectSnapEpsilonNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) snapSettings.ObjectToObjectSnapEpsilon = floatRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.SnapGridXOffsetNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) snapSettings.XZSnapGridXOffset = floatRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.SnapGridYOffsetNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) snapSettings.XZSnapGridYOffset = floatRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.SnapGridYOffsetStepNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) snapSettings.XZGridYOffsetStep = floatRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.SnapGridZOffsetNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) snapSettings.XZSnapGridZOffset = floatRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSurfaceGridDesiredCellSizeNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) snapSettings.ObjectColliderSnapSurfaceGridSettings.DesiredCellSize = floatRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.SnapGridCellSizeXNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) ObjectSnapping.Get().XZSnapGrid.CellSizeSettings.CellSizeX = floatRes;
                    }

                    node = snapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.SnapGridCellSizeZNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) ObjectSnapping.Get().XZSnapGrid.CellSizeSettings.CellSizeZ = floatRes;
                    }
                }
            }
        }

        private static void LoadObjectSelectionSettings(XmlDocument xmlDoc, Octave3DConfigSaveLoadSettings loadSettings)
        {
            if(loadSettings.ObjectSelectionSettings)
            {
                XmlNode selectionSettingsNode = xmlDoc.SelectSingleNode("//" + Octave3DConfigXMLInfo.ObjectSelectionSettingsNode);
                if(selectionSettingsNode != null)
                {
                    ObjectSelectionSettings objectSelectionSettings = ObjectSelectionSettings.Get();

                    XmlNode node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionAllowPartialOverlapNode);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) objectSelectionSettings.AllowPartialOverlap = boolRes;
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionAttachToObjectGroupNode);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) objectSelectionSettings.ObjectGroupSettings.AttachToObjectGroup = boolRes;
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionDestinationObjectGroupNode);
                    if (node != null && !string.IsNullOrEmpty(node.InnerText))
                    {
                        ObjectGroupDatabase groupDatabase = Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase;
                        ObjectGroup objectGroup = groupDatabase.GetObjectGroupByName(node.InnerText);
                        if (objectGroup != null) objectSelectionSettings.ObjectGroupSettings.DestinationGroup = objectGroup;
                        else
                        {
                            ObjectGroup newGroup = groupDatabase.CreateObjectGroup(node.InnerText);
                            if (newGroup != null) objectSelectionSettings.ObjectGroupSettings.DestinationGroup = objectGroup;
                        }
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionXRotationStepNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) objectSelectionSettings.XRotationStep = floatRes;
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionYRotationStepNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) objectSelectionSettings.YRotationStep = floatRes;
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionZRotationStepNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) objectSelectionSettings.ZRotationStep = floatRes;
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionRotateAroundCenterNode);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) objectSelectionSettings.RotateAroundSelectionCenter = boolRes;
                    }

                    XmlNode obj2objSnapSettingsNode = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionObject2ObjectSnapSettingsNode);
                    if(obj2objSnapSettingsNode != null)
                    {
                        node = obj2objSnapSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionObj2ObjSnapEpsNode);
                        if (node != null)
                        {
                            float floatRes;
                            if (float.TryParse(node.InnerText, out floatRes)) objectSelectionSettings.Object2ObjectSnapSettings.SnapEps = floatRes;
                        }

                        node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionObj2ObjSnapCanHoverObjectsNode);
                        if (node != null)
                        {
                            bool boolRes;
                            if (bool.TryParse(node.InnerText, out boolRes)) objectSelectionSettings.Object2ObjectSnapSettings.CanHoverObjects = boolRes;
                        }
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionShapeTypeNode);
                    if (node != null)
                    {
                        try
                        {
                            objectSelectionSettings.SelectionShapeType = (ObjectSelectionShapeType)Enum.Parse(typeof(ObjectSelectionShapeType), node.InnerText);
                        }
                        catch (Exception) { }
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionUpdateModeNode);
                    if (node != null)
                    {
                        try
                        {
                            objectSelectionSettings.SelectionUpdateMode = (ObjectSelectionUpdateMode)Enum.Parse(typeof(ObjectSelectionUpdateMode), node.InnerText);
                        }
                        catch (Exception) { }
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionModeNode);
                    if (node != null)
                    {
                        try
                        {
                            objectSelectionSettings.SelectionMode = (ObjectSelectionMode)Enum.Parse(typeof(ObjectSelectionMode), node.InnerText);
                        }
                        catch (Exception) { }
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionPaint_ShapePixelWidthNode);
                    if (node != null)
                    {
                        int intRes;
                        if (int.TryParse(node.InnerText, out intRes)) objectSelectionSettings.PaintModeSettings.SelectionShapeWidthInPixels = intRes;
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionPaint_ShapePixelHeightNode);
                    if (node != null)
                    {
                        int intRes;
                        if (int.TryParse(node.InnerText, out intRes)) objectSelectionSettings.PaintModeSettings.SelectionShapeHeightInPixels = intRes;
                    }

                    node = selectionSettingsNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionPaint_SizeAdjustmentSpeedScrollWheelNode);
                    if (node != null)
                    {
                        int intRes;
                        if (int.TryParse(node.InnerText, out intRes)) objectSelectionSettings.PaintModeSettings.ScrollWheelShapeSizeAdjustmentSpeed = intRes;
                    }
                }
            }
        }

        private static void LoadObjectErasingSettings(XmlDocument xmlDoc, Octave3DConfigSaveLoadSettings loadSettings)
        {
            if(loadSettings.ObjectErasingSettings)
            {
                XmlNode objectErasingNode = xmlDoc.SelectSingleNode("//" + Octave3DConfigXMLInfo.ObjectErasingSettingsNode);
                if (objectErasingNode != null)
                {
                    ObjectEraserSettings objectEraserSettings = ObjectEraserSettings.Get();

                    XmlNode node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectErasingAllowUndoRedoNode);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) objectEraserSettings.AllowUndoRedo = boolRes;
                    }

                    node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectEraseDelayNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) objectEraserSettings.EraseDelayInSeconds = floatRes;
                    }


                    node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectEraseModeNode);
                    if (node != null)
                    {
                        try
                        {
                            objectEraserSettings.EraseMode = (ObjectEraseMode)Enum.Parse(typeof(ObjectEraseMode), node.InnerText);
                        }
                        catch (Exception) { }
                    }

                    node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectEraseOnlyMeshObjectsNode);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) objectEraserSettings.EraseOnlyMeshObjects = boolRes;
                    }

                    node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectMassErase2D_CircleRadiusNode);
                    if (node != null)
                    {
                        int intRes;
                        if (int.TryParse(node.InnerText, out intRes)) objectEraserSettings.Mass2DEraseSettings.CircleShapeRadius = intRes;
                    }

                    node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectMassErase2D_RadiusAdjustmentSpeedScrollWheelNode);
                    if (node != null)
                    {
                        int intRes;
                        if (int.TryParse(node.InnerText, out intRes)) objectEraserSettings.Mass2DEraseSettings.ScrollWheelCircleRadiusAdjustmentSpeed = intRes;
                    }

                    node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectMassErase2D_AllowPartialOverlap);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) objectEraserSettings.Mass2DEraseSettings.AllowPartialOverlap = boolRes;
                    }

                    node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectMassErase3D_CircleRadiusNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) objectEraserSettings.Mass3DEraseSettings.CircleShapeRadius = floatRes;
                    }

                    node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectMassErase3D_RadiusAdjustmentSpeedScrollWheelNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) objectEraserSettings.Mass3DEraseSettings.ScrollWheelCircleRadiusAdjustmentSpeed = floatRes;
                    }

                    node = objectErasingNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectMassErase3D_AllowPartialOverlap);
                    if (node != null)
                    {
                        bool boolRes;
                        if (bool.TryParse(node.InnerText, out boolRes)) objectEraserSettings.Mass3DEraseSettings.AllowPartialOverlap = boolRes;
                    }
                }
            }
        }

        private static void LoadMirrorLookAndFeel(XmlDocument xmlDoc, Octave3DConfigSaveLoadSettings loadSettings, InteractableMirrorRenderSettings renderSettings, bool objectPlacementMirror)
        {
            if (loadSettings.MirrorLookAndFeel)
            {
                XmlNode mirrorLookAndFeelNode = xmlDoc.SelectSingleNode("//" + (objectPlacementMirror ? Octave3DConfigXMLInfo.ObjectPlacementMirrorLookAndFeelNode : Octave3DConfigXMLInfo.ObjectSelectionMirrorLookAndFeelNode));
                if (mirrorLookAndFeelNode != null)
                {
                    XmlNode node = mirrorLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.MirrorWidthNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) renderSettings.MirrorWidth = floatRes;
                    }

                    node = mirrorLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.MirrorHeightNode);
                    if (node != null)
                    {
                        float floatRes;
                        if (float.TryParse(node.InnerText, out floatRes)) renderSettings.MirrorHeight = floatRes;
                    }

                    node = mirrorLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.MirrorHasInfiniteWidthNode);
                    if (node != null)
                    {
                        bool res;
                        if (bool.TryParse(node.InnerText, out res)) renderSettings.UseInfiniteWidth = res;
                    }

                    node = mirrorLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.MirrorHasInfiniteHeightNode);
                    if (node != null)
                    {
                        bool res;
                        if (bool.TryParse(node.InnerText, out res)) renderSettings.UseInfiniteHeight = res;
                    }

                    node = mirrorLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.MirrorColorNode);
                    if (node != null)
                    {
                        renderSettings.MirrorPlaneColor = ColorExtensions.FromString(node.InnerText);
                    }

                    node = mirrorLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.MirrorBorderColorNode);
                    if (node != null)
                    {
                        renderSettings.MirrorPlaneBorderLineColor = ColorExtensions.FromString(node.InnerText);
                    }

                    node = mirrorLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.MirroredBoxColorNode);
                    if (node != null)
                    {
                        renderSettings.MirroredBoxColor = ColorExtensions.FromString(node.InnerText);
                    }

                    node = mirrorLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.MirroredBoxBorderColorNode);
                    if (node != null)
                    {
                        renderSettings.MirroredBoxBorderLineColor = ColorExtensions.FromString(node.InnerText);
                    }
                }
            }
        }

        private static void LoadSnapLookAndFeel(XmlDocument xmlDoc, Octave3DConfigSaveLoadSettings loadSettings)
        {
            if(loadSettings.SnapLookAndFeel)
            {
                XmlNode snapLookAndFeelNode = xmlDoc.SelectSingleNode("//" + Octave3DConfigXMLInfo.SnapLookAndFeelNode);
                if(snapLookAndFeelNode != null)
                {
                    XZGridRenderSettings xzGridRenderSettings = ObjectSnapping.Get().XZSnapGrid.RenderSettings;
                    XZGridRenderSettings objSurfaceGridRenderSettings = ObjectSnapping.Get().RenderSettingsForColliderSnapSurfaceGrid;
                    CoordinateSystemRenderSettings coordSystemRenderSettings = ObjectSnapping.Get().XZSnapGrid.RenderableCoordinateSystem.RenderSettings;

                    XmlNode xzGridLookAndFeelNode = snapLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.XZGridLookAndFeelNode);
                    if(xzGridLookAndFeelNode != null)
                    {
                        XmlNode node = xzGridLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.IsXZGridVisibleNode);
                        if (node != null)
                        {
                            bool res;
                            if (bool.TryParse(node.InnerText, out res)) xzGridRenderSettings.IsVisible = res;
                        }

                        node = xzGridLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.XZGridCellLineColorNode);
                        if (node != null)
                        {
                            xzGridRenderSettings.CellLineColor = ColorExtensions.FromString(node.InnerText);
                        }

                        node = xzGridLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.XZGridCellLineThicknessNode);
                        if (node != null)
                        {
                            float res;
                            if (float.TryParse(node.InnerText, out res)) xzGridRenderSettings.CellLineThickness = res;
                        }

                        node = xzGridLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.XZGridPlaneColorNode);
                        if (node != null)
                        {
                            xzGridRenderSettings.PlaneColor = ColorExtensions.FromString(node.InnerText);
                        }
                    }

                    XmlNode xzGridCoordSystemLookAndFeelNode = snapLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.XZGridCoordSystemLookAndFeelNode);
                    if(xzGridCoordSystemLookAndFeelNode != null)
                    {
                        XmlNode node = xzGridCoordSystemLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.IsXZGridCoordSystemVisibleNode);
                        if(node != null)
                        {
                            bool res;
                            if (bool.TryParse(node.InnerText, out res)) coordSystemRenderSettings.IsVisible = res;
                        }

                        List<CoordinateSystemAxis> allAxes = CoordinateSystemAxes.GetAll();
                        foreach(var axis in allAxes)
                        {
                            node = xzGridCoordSystemLookAndFeelNode.SelectSingleNode(".//" + axis.ToString());
                            if(node != null)
                            {
                                XmlNode axisPropNode = node.SelectSingleNode(".//" + Octave3DConfigXMLInfo.IsXZGridCoordSystemAxisVisibleNode);
                                if(axisPropNode != null)
                                {
                                    bool res;
                                    if (bool.TryParse(axisPropNode.InnerText, out res)) coordSystemRenderSettings.SetAxisVisible(axis, res);
                                }

                                axisPropNode = node.SelectSingleNode(".//" + Octave3DConfigXMLInfo.IsXZGridCoordSystemAxisInfiniteNode);
                                if (axisPropNode != null)
                                {
                                    bool res;
                                    if (bool.TryParse(axisPropNode.InnerText, out res)) coordSystemRenderSettings.SetAxisRenderInfinite(axis, res);
                                }

                                axisPropNode = node.SelectSingleNode(".//" + Octave3DConfigXMLInfo.XZGridCoordSystemAxisFiniteSizeNode);
                                if (axisPropNode != null)
                                {
                                    float res;
                                    if (float.TryParse(axisPropNode.InnerText, out res)) coordSystemRenderSettings.SetAxisFiniteSize(axis, res);
                                }

                                axisPropNode = node.SelectSingleNode(".//" + Octave3DConfigXMLInfo.XZGridCoordSystemAxisColorNode);
                                if (axisPropNode != null)
                                {
                                    coordSystemRenderSettings.SetAxisColor(axis, ColorExtensions.FromString(axisPropNode.InnerText));
                                }
                            }
                        }
                    }

                    XmlNode objectSnapSurfaceLookAndFeelNode = snapLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSnapSurfaceGridLookAndFeelNode);
                    if(objectSnapSurfaceLookAndFeelNode != null)
                    {
                        XmlNode node = objectSnapSurfaceLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.IsObjectSnapSurfaceGridVisibleNode);
                        if(node != null)
                        {
                            bool res;
                            if (bool.TryParse(node.InnerText, out res)) objSurfaceGridRenderSettings.IsVisible = res;
                        }

                        node = objectSnapSurfaceLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSnapSurfaceGridCellLineColorNode);
                        if (node != null)
                        {
                            objSurfaceGridRenderSettings.CellLineColor = ColorExtensions.FromString(node.InnerText);
                        }

                        node = objectSnapSurfaceLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSnapSurfaceGridCellLineThicknessNode);
                        if (node != null)
                        {
                            float res;
                            if (float.TryParse(node.InnerText, out res)) objSurfaceGridRenderSettings.CellLineThickness = res;
                        }

                        node = objectSnapSurfaceLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSnapSurfaceGridPlaneColorNode);
                        if (node != null)
                        {
                            objSurfaceGridRenderSettings.PlaneColor = ColorExtensions.FromString(node.InnerText);
                        }
                    }
                }
            }
        }

        public static void LoadObjectPlacementLookAndFeel(XmlDocument xmlDoc, Octave3DConfigSaveLoadSettings loadSettings)
        {
            if(loadSettings.ObjectPlacementLookAndFeel)
            {
                ObjectPivotPointsRenderSettings pivotPointsRenderSettings = ObjectPlacement.Get().GuidePivotPointsRenderSettings;
                ProjectedBoxFacePivotPointsRenderSettings projectedPivotPointRenderSettings = pivotPointsRenderSettings.ProjectedBoxFacePivotPointsRenderSettings;
                ObjectVertexSnapSessionRenderSettings vertexSnapRenderSettings = ObjectPlacement.Get().ObjectVertexSnapSessionRenderSettings;

                XmlNode objPlacementLookAndFeelNode = xmlDoc.SelectSingleNode("//" + Octave3DConfigXMLInfo.ObjectPlacementLookAndFeelNode);
                if(objPlacementLookAndFeelNode != null)
                {
                    XmlNode guidePivotPointLookAndFeelNode = objPlacementLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.GuidePivotPointsLookAndFeelNode);
                    if(guidePivotPointLookAndFeelNode != null)
                    {
                        XmlNode node = guidePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.GuidePivotPointsShapeTypeNode);
                        if(node != null)
                        {
                            try
                            {
                                pivotPointsRenderSettings.ShapeType = (ObjectPivotPointShapeType)Enum.Parse(typeof(ObjectPivotPointShapeType), node.InnerText);
                            }
                            catch (Exception) { }
                        }

                        node = guidePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.GuidePivotPointsShapeSizeInPixelsNode);
                        if(node != null)
                        {
                            float res;
                            if (float.TryParse(node.InnerText, out res)) pivotPointsRenderSettings.PivotPointSizeInPixels = res;
                        }

                        node = guidePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.DrawPivotPointProjectionLinesNode);
                        if (node != null)
                        {
                            bool res;
                            if (bool.TryParse(node.InnerText, out res)) projectedPivotPointRenderSettings.RenderProjectionLines = res;
                        }

                        node = guidePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointProjectionLineColorNode);
                        if (node != null)
                        {
                            projectedPivotPointRenderSettings.ProjectionLineColor = ColorExtensions.FromString(node.InnerText);
                        }

                        node = guidePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.DrawPivotPointConnectionLinesNode);
                        if (node != null)
                        {
                            bool res;
                            if (bool.TryParse(node.InnerText, out res)) projectedPivotPointRenderSettings.RenderPivotPointConnectionLines = res;
                        }

                        node = guidePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointConnectionLineColorNode);
                        if (node != null)
                        {
                            projectedPivotPointRenderSettings.PivotPointConnectionLineColor = ColorExtensions.FromString(node.InnerText);
                        }

                        XmlNode activePivotPointLookAndFeelNode = guidePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ActivePivotPointLookAndFeelNode);
                        if(activePivotPointLookAndFeelNode != null)
                        {
                            node = activePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointIsVisibleNode);
                            if(node != null)
                            {
                                bool res;
                                if (bool.TryParse(node.InnerText, out res)) projectedPivotPointRenderSettings.ActivePivotPointRenderSettings.IsVisible = res;
                            }

                            node = activePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointFillColorNode);
                            if (node != null)
                            {
                                projectedPivotPointRenderSettings.ActivePivotPointRenderSettings.FillColor = ColorExtensions.FromString(node.InnerText);
                            }

                            node = activePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointBorderColorNode);
                            if (node != null)
                            {
                                projectedPivotPointRenderSettings.ActivePivotPointRenderSettings.BorderLineColor = ColorExtensions.FromString(node.InnerText);
                            }

                            node = activePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointScaleNode);
                            if (node != null)
                            {
                                float res;
                                if (float.TryParse(node.InnerText, out res)) projectedPivotPointRenderSettings.ActivePivotPointRenderSettings.Scale = res;
                            }
                        }

                        XmlNode inactivePivotPointLookAndFeelNode = guidePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.InactivePivotPointLookAndFeelNode);
                        if (activePivotPointLookAndFeelNode != null)
                        {
                            node = inactivePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointIsVisibleNode);
                            if (node != null)
                            {
                                bool res;
                                if (bool.TryParse(node.InnerText, out res)) projectedPivotPointRenderSettings.InactivePivotPointRenderSettings.IsVisible = res;
                            }

                            node = inactivePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointFillColorNode);
                            if (node != null)
                            {
                                projectedPivotPointRenderSettings.InactivePivotPointRenderSettings.FillColor = ColorExtensions.FromString(node.InnerText);
                            }

                            node = inactivePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointBorderColorNode);
                            if (node != null)
                            {
                                projectedPivotPointRenderSettings.InactivePivotPointRenderSettings.BorderLineColor = ColorExtensions.FromString(node.InnerText);
                            }

                            node = inactivePivotPointLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PivotPointScaleNode);
                            if (node != null)
                            {
                                float res;
                                if (float.TryParse(node.InnerText, out res)) projectedPivotPointRenderSettings.InactivePivotPointRenderSettings.Scale = res;
                            }
                        }
                    }

                    XmlNode vertexSnappingLookAndFeelNode = objPlacementLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectVertexSnappingLookAndFeelNode);
                    if(vertexSnappingLookAndFeelNode != null)
                    {
                        XmlNode node = vertexSnappingLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectVertexSnappingDrawSrcVertexNode);
                        if(node != null)
                        {
                            bool res;
                            if (bool.TryParse(node.InnerText, out res)) vertexSnapRenderSettings.RenderSourceVertex = res;
                        }

                        node = vertexSnappingLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectVertexSnappingSrcVertexFillColorNode);
                        if (node != null)
                        {
                            vertexSnapRenderSettings.SourceVertexFillColor = ColorExtensions.FromString(node.InnerText);
                        }

                        node = vertexSnappingLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectVertexSnappingSrcVertexBorderColorNode);
                        if (node != null)
                        {
                            vertexSnapRenderSettings.SourceVertexBorderColor = ColorExtensions.FromString(node.InnerText);
                        }

                        node = vertexSnappingLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectVertexSnappingSrcVertexPixelRadiusNode);
                        if (node != null)
                        {
                            float res;
                            if (float.TryParse(node.InnerText, out res)) vertexSnapRenderSettings.SourceVertexRadiusInPixels = res;
                        }
                    }

                    XmlNode decorPaintLookAndFeelNode = objPlacementLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.DecorPaintLookAndFeelNode);
                    if(decorPaintLookAndFeelNode != null)
                    {
                        XmlNode decorPaintBrushLookAndFeelNode = decorPaintLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.DecorPaintBrushCircleLookAndFeelNode);
                        if(decorPaintBrushLookAndFeelNode != null)
                        {
                            XmlNode node = decorPaintBrushLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.DecorPaintBrushCircleBorderLineColorNode);
                            if(node != null)
                            {
                                ObjectPlacement.Get().DecorPaintObjectPlacement.BrushCircleRenderSettings.BorderLineColor = ColorExtensions.FromString(node.InnerText);
                            }
                        }
                    }

                    XmlNode pathLookAndFeelNode = objPlacementLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PathLookAndFeelNode);
                    if(pathLookAndFeelNode != null)
                    {
                        ObjectPlacementPathRenderSettings pathRenderSettings = ObjectPlacement.Get().PathObjectPlacement.PathRenderSettings;

                        XmlNode node = pathLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.PathBorderLineColorNode);
                        if(node != null)
                        {
                            pathRenderSettings.ManualConstructionRenderSettings.BoxBorderLineColor = ColorExtensions.FromString(node.InnerText);
                        }

                        LoadExtensionPlaneLookAndFeel(ObjectPlacement.Get().PathObjectPlacement.PathExtensionPlaneRenderSettings, pathLookAndFeelNode);
                    }

                    XmlNode blockLookAndFeelNode = objPlacementLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.BlockLookAndFeelNode);
                    if(blockLookAndFeelNode != null)
                    {
                        ObjectPlacementBlockRenderSettings blockRenderSettings = ObjectPlacement.Get().BlockObjectPlacement.BlockRenderSettings;

                        XmlNode node = blockLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.BlockBorderLineColorNode);
                        if (node != null)
                        {
                            blockRenderSettings.ManualConstructionRenderSettings.BoxBorderLineColor = ColorExtensions.FromString(node.InnerText);
                        }

                        node = blockLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.BlockTextColorNode);
                        if (node != null)
                        {
                            blockRenderSettings.ManualConstructionRenderSettings.DimensionsLabelRenderSettings.TextColor = ColorExtensions.FromString(node.InnerText);
                        }

                        node = blockLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.BlockFontSizeNode);
                        if (node != null)
                        {
                            int res;
                            if (int.TryParse(node.InnerText, out res)) blockRenderSettings.ManualConstructionRenderSettings.DimensionsLabelRenderSettings.FontSize = res;
                        }

                        node = blockLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.BlockBoldTextNode);
                        if (node != null)
                        {
                            bool res;
                            if (bool.TryParse(node.InnerText, out res)) blockRenderSettings.ManualConstructionRenderSettings.DimensionsLabelRenderSettings.Bold = res;
                        }

                        LoadExtensionPlaneLookAndFeel(ObjectPlacement.Get().BlockObjectPlacement.BlockExtensionPlaneRenderSettings, blockLookAndFeelNode);
                    }
                }
            }
        }

        private static void LoadExtensionPlaneLookAndFeel(ObjectPlacementExtensionPlaneRenderSettings renderSettings, XmlNode parentNode)
        {
            XmlNode extensionPlaneLookAndFeel = parentNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ExtensionPlaneLookAndFeelNode);
            if(extensionPlaneLookAndFeel != null)
            {
                XmlNode node = extensionPlaneLookAndFeel.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ExtensionPlaneScaleNode);
                if(node != null)
                {
                    float res;
                    if (float.TryParse(node.InnerText, out res)) renderSettings.PlaneScale = res;
                }

                node = extensionPlaneLookAndFeel.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ExtensionPlaneColorNode);
                if (node != null)
                {
                    renderSettings.PlaneColor = ColorExtensions.FromString(node.InnerText);
                }

                node = extensionPlaneLookAndFeel.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ExtensionPlaneBorderColorNode);
                if (node != null)
                {
                    renderSettings.PlaneBorderLineColor = ColorExtensions.FromString(node.InnerText);
                }

                node = extensionPlaneLookAndFeel.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ExtensionPlaneNormalLineLengthNode);
                if (node != null)
                {
                    float res;
                    if (float.TryParse(node.InnerText, out res)) renderSettings.PlaneNormalLineLength = res;
                }

                node = extensionPlaneLookAndFeel.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ExtensionPlaneNormalLineColorNode);
                if (node != null)
                {
                    renderSettings.PlaneNormalLineColor = ColorExtensions.FromString(node.InnerText);
                }
            }
        }

        private static void LoadObjectSelectionLookAndFeel(XmlDocument xmlDoc, Octave3DConfigSaveLoadSettings loadSettings)
        {
            if(loadSettings.ObjectSelectionLookAndFeel)
            {
                XmlNode selectionLookAndFeelNode = xmlDoc.SelectSingleNode("//" + Octave3DConfigXMLInfo.ObjectSelectionLookAndFeelNode);
                if(selectionLookAndFeelNode != null)
                {
                    ObjectSelectionRenderSettings selectionRenderSettings = ObjectSelection.Get().RenderSettings;
                    RectangleShapeRenderSettings rectRenderSettings = ObjectSelection.Get().RectangleSelectionShapeRenderSettings;
                    EllipseShapeRenderSettings ellipseRenderSettings = ObjectSelection.Get().EllipseSelectionShapeRenderSettings;

                    XmlNode node = selectionLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionBoxEdgeDrawModeNode);
                    if(node != null)
                    {
                        try
                        {
                            selectionRenderSettings.BoxRenderModeSettings.EdgeRenderMode = (ObjectSelectionBoxEdgeRenderMode)Enum.Parse(typeof(ObjectSelectionBoxEdgeRenderMode), node.InnerText);
                        }
                        catch (Exception) { }
                    }

                    node = selectionLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionBoxCornerEdgeLengthPercentageNode);
                    if (node != null)
                    {
                        float res;
                        if (float.TryParse(node.InnerText, out res)) selectionRenderSettings.BoxRenderModeSettings.CornerEdgesRenderModeSettings.CornerEdgeLengthPercentage = res;
                    }

                    node = selectionLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionBoxEdgeColorNode);
                    if (node != null)
                    {
                        selectionRenderSettings.BoxRenderModeSettings.EdgeColor = ColorExtensions.FromString(node.InnerText);
                    }

                    node = selectionLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionBoxColorNode);
                    if (node != null)
                    {
                        selectionRenderSettings.BoxRenderModeSettings.BoxColor = ColorExtensions.FromString(node.InnerText);
                    }

                    node = selectionLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionBoxScaleNode);
                    if (node != null)
                    {
                        float res;
                        if (float.TryParse(node.InnerText, out res)) selectionRenderSettings.BoxRenderModeSettings.BoxScale = res;
                    }

                    XmlNode selRectNode = selectionLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionRectLookAndFeelNode);
                    if(selRectNode != null)
                    {
                        node = selRectNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionShapeFillColorNode);
                        if (node != null) rectRenderSettings.FillColor = ColorExtensions.FromString(node.InnerText);

                        node = selRectNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionShapeBorderColorNode);
                        if (node != null) rectRenderSettings.BorderLineColor = ColorExtensions.FromString(node.InnerText);
                    }

                    XmlNode selEllipseNode = selectionLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionEllipseLookAndFeelNode);
                    if (selRectNode != null)
                    {
                        node = selEllipseNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionShapeFillColorNode);
                        if (node != null) ellipseRenderSettings.FillColor = ColorExtensions.FromString(node.InnerText);

                        node = selEllipseNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectSelectionShapeBorderColorNode);
                        if (node != null) ellipseRenderSettings.BorderLineColor = ColorExtensions.FromString(node.InnerText);
                    }
                }
            }
        }

        private static void LoadObjectErasingLookAndFeel(XmlDocument xmlDoc, Octave3DConfigSaveLoadSettings loadSettings)
        {
            if (loadSettings.ObjectErasingLookAndFeel)
            {
                XmlNode erasingLookAndFeelNode = xmlDoc.SelectSingleNode("//" + Octave3DConfigXMLInfo.ObjectErasingLookAndFeelNode);
                if (erasingLookAndFeelNode != null)
                {
                    EllipseShapeRenderSettings circle2DRenderSettings = ObjectEraser.Get().Circle2DMassEraseShapeRenderSettings;
                    XZOrientedEllipseShapeRenderSettings circle3DRenderSettings = ObjectEraser.Get().Circle3DMassEraseShapeRenderSettings;

                    XmlNode mass2DCircleNode = erasingLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectErasing2DCircleLookAndFeelNode);
                    if(mass2DCircleNode != null)
                    {
                        XmlNode node = mass2DCircleNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectErasingShapeFillColorNode);
                        if (node != null) circle2DRenderSettings.FillColor = ColorExtensions.FromString(node.InnerText);

                        node = mass2DCircleNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectErasingShapeBorderColorNode);
                        if (node != null) circle2DRenderSettings.BorderLineColor = ColorExtensions.FromString(node.InnerText);
                    }

                    XmlNode mas3DCircleNode = erasingLookAndFeelNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectErasing3DCircleLookAndFeelNode);
                    if (mas3DCircleNode != null)
                    {
                        XmlNode node = mas3DCircleNode.SelectSingleNode(".//" + Octave3DConfigXMLInfo.ObjectErasingShapeBorderColorNode);
                        if (node != null) circle3DRenderSettings.BorderLineColor = ColorExtensions.FromString(node.InnerText);
                    }
                }
            }
        }
        #endregion
    }
}
#endif