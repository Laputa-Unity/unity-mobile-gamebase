#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementInspectorGUI : InspectorGUI
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementLookAndFeelSettingsView _objectPlacementLookAndFeelSettingsView = new ObjectPlacementLookAndFeelSettingsView();
        #endregion

        #region Public Methods
        public override void Initialize()
        {
            base.Initialize();
            if (Octave3DWorldBuilder.ActiveInstance == null) return;

            ObjectPlacementSettings.Get().ObjectIntersectionSettings.View.IsVisible = false;

            _objectPlacementLookAndFeelSettingsView.IsVisible = false;
            _objectPlacementLookAndFeelSettingsView.BlockObjectPlacementLookAndFeelSettingsView.ToggleVisibilityBeforeRender = true;
            _objectPlacementLookAndFeelSettingsView.BlockObjectPlacementLookAndFeelSettingsView.VisibilityToggleLabel = "Block";
            _objectPlacementLookAndFeelSettingsView.BlockObjectPlacementLookAndFeelSettingsView.IndentContent = true;
            _objectPlacementLookAndFeelSettingsView.BlockObjectPlacementLookAndFeelSettingsView.IsVisible = false;

            _objectPlacementLookAndFeelSettingsView.PathObjectPlacementLookAndFeelSettingsView.ToggleVisibilityBeforeRender = true;
            _objectPlacementLookAndFeelSettingsView.PathObjectPlacementLookAndFeelSettingsView.VisibilityToggleLabel = "Path";
            _objectPlacementLookAndFeelSettingsView.PathObjectPlacementLookAndFeelSettingsView.IndentContent = true;
            _objectPlacementLookAndFeelSettingsView.PathObjectPlacementLookAndFeelSettingsView.IsVisible = false;

            _objectPlacementLookAndFeelSettingsView.DecorPaintLookAndFeelSettingsView.ToggleVisibilityBeforeRender = true;
            _objectPlacementLookAndFeelSettingsView.DecorPaintLookAndFeelSettingsView.VisibilityToggleLabel = "Decor Paint";
            _objectPlacementLookAndFeelSettingsView.DecorPaintLookAndFeelSettingsView.IndentContent = true;
            _objectPlacementLookAndFeelSettingsView.DecorPaintLookAndFeelSettingsView.IsVisible = false;

            ObjectGroupDatabaseView objectGroupDatabaseView = Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase.View;
            objectGroupDatabaseView.ToggleVisibilityBeforeRender = true;
            objectGroupDatabaseView.VisibilityToggleLabel = "Object Groups";
            objectGroupDatabaseView.IsVisible = false;
            objectGroupDatabaseView.SurroundWithBox = true;

            ObjectPlacementGuideSettings objectPlacementGuideSettings = ObjectPlacementSettings.Get().ObjectPlacementGuideSettings;
            objectPlacementGuideSettings.KeyboardRotationSettings.XAxisRotationSettings.View.VisibilityToggleLabel = objectPlacementGuideSettings.KeyboardRotationSettings.XAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            objectPlacementGuideSettings.KeyboardRotationSettings.YAxisRotationSettings.View.VisibilityToggleLabel = objectPlacementGuideSettings.KeyboardRotationSettings.YAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            objectPlacementGuideSettings.KeyboardRotationSettings.ZAxisRotationSettings.View.VisibilityToggleLabel = objectPlacementGuideSettings.KeyboardRotationSettings.ZAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            objectPlacementGuideSettings.KeyboardRotationSettings.CustomAxisRotationSettings.View.VisibilityToggleLabel = "Placement Surface Normal Settings";

            objectPlacementGuideSettings.KeyboardRotationSettings.XAxisRotationSettings.View.IsVisible = false;
            objectPlacementGuideSettings.KeyboardRotationSettings.YAxisRotationSettings.View.IsVisible = false;
            objectPlacementGuideSettings.KeyboardRotationSettings.ZAxisRotationSettings.View.IsVisible = false;
            objectPlacementGuideSettings.KeyboardRotationSettings.CustomAxisRotationSettings.View.IsVisible = false;

            objectPlacementGuideSettings.MouseRotationSettings.XAxisRotationSettings.View.VisibilityToggleLabel = objectPlacementGuideSettings.MouseRotationSettings.XAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            objectPlacementGuideSettings.MouseRotationSettings.YAxisRotationSettings.View.VisibilityToggleLabel = objectPlacementGuideSettings.MouseRotationSettings.YAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            objectPlacementGuideSettings.MouseRotationSettings.ZAxisRotationSettings.View.VisibilityToggleLabel = objectPlacementGuideSettings.MouseRotationSettings.ZAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            objectPlacementGuideSettings.MouseRotationSettings.CustomAxisRotationSettings.View.VisibilityToggleLabel = "Placement Surface Normal Settings";

            objectPlacementGuideSettings.MouseRotationSettings.XAxisRotationSettings.View.IsVisible = false;
            objectPlacementGuideSettings.MouseRotationSettings.YAxisRotationSettings.View.IsVisible = false;
            objectPlacementGuideSettings.MouseRotationSettings.ZAxisRotationSettings.View.IsVisible = false;
            objectPlacementGuideSettings.MouseRotationSettings.CustomAxisRotationSettings.View.IsVisible = false;

            ObjectPlacement.Get().MirrorView.IsVisible = false;
            InteractableMirrorSettings mirrorSettings = ObjectPlacement.Get().MirrorSettings;
            mirrorSettings.View.IsVisible = false;
            mirrorSettings.View.ToggleVisibilityBeforeRender = true;
            mirrorSettings.View.VisibilityToggleLabel = "More settings";
            mirrorSettings.View.VisibilityToggleIndent = 1;
            mirrorSettings.View.IndentContent = true;
            mirrorSettings.KeyboardRotationSettings.XAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.KeyboardRotationSettings.XAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.KeyboardRotationSettings.YAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.KeyboardRotationSettings.YAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.KeyboardRotationSettings.ZAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.KeyboardRotationSettings.ZAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.KeyboardRotationSettings.CustomAxisRotationSettings.View.VisibilityToggleLabel = "Hover Surface Normal Settings";

            mirrorSettings.KeyboardRotationSettings.XAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.KeyboardRotationSettings.YAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.KeyboardRotationSettings.ZAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.KeyboardRotationSettings.CustomAxisRotationSettings.View.IsVisible = false;

            mirrorSettings.MouseRotationSettings.XAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.MouseRotationSettings.XAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.MouseRotationSettings.YAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.MouseRotationSettings.YAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.MouseRotationSettings.ZAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.MouseRotationSettings.ZAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.MouseRotationSettings.CustomAxisRotationSettings.View.VisibilityToggleLabel = "Hover Surface Normal Settings";

            mirrorSettings.MouseRotationSettings.XAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.MouseRotationSettings.YAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.MouseRotationSettings.ZAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.MouseRotationSettings.CustomAxisRotationSettings.View.IsVisible = false;

            InteractableMirrorRenderSettingsView mirrorRenderSettingsView = ObjectPlacement.Get().MirrorRenderSettings.View;
            mirrorRenderSettingsView.VisibilityToggleLabel = "Look and feel";
            mirrorRenderSettingsView.ToggleVisibilityBeforeRender = true;
            mirrorRenderSettingsView.IndentContent = true;
            mirrorRenderSettingsView.VisibilityToggleIndent = 1;
            mirrorRenderSettingsView.IsVisible = false;

            ObjectRotationRandomizationSettings placementGuideRotationRandomizationSettings = PointAndClickObjectPlacementSettings.Get().PlacementGuideRotationRandomizationSettings;
            ObjectRotationRandomizationSettingsView placementGuideRotationRandomizationSettingsView = placementGuideRotationRandomizationSettings.View;
            placementGuideRotationRandomizationSettingsView.VisibilityToggleIndent = 1;
            placementGuideRotationRandomizationSettingsView.VisibilityToggleLabel = "Rotation Randomization Settings";
            placementGuideRotationRandomizationSettingsView.IsVisible = false;
            placementGuideRotationRandomizationSettings.XAxisRandomizationSettings.View.VisibilityToggleLabel = placementGuideRotationRandomizationSettings.XAxisRandomizationSettings.Axis + " Axis Settings";
            placementGuideRotationRandomizationSettings.XAxisRandomizationSettings.View.IsVisible = false;
            placementGuideRotationRandomizationSettings.YAxisRandomizationSettings.View.VisibilityToggleLabel = placementGuideRotationRandomizationSettings.YAxisRandomizationSettings.Axis + " Axis Settings";
            placementGuideRotationRandomizationSettings.YAxisRandomizationSettings.View.IsVisible = false;
            placementGuideRotationRandomizationSettings.ZAxisRandomizationSettings.View.VisibilityToggleLabel = placementGuideRotationRandomizationSettings.ZAxisRandomizationSettings.Axis + " Axis Settings";
            placementGuideRotationRandomizationSettings.ZAxisRandomizationSettings.View.IsVisible = false;
            placementGuideRotationRandomizationSettings.CustomAxisRandomizationSettings.View.VisibilityToggleLabel = "Placement Surface Normal Settings";

            placementGuideRotationRandomizationSettings = DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.PlacementGuideRotationRandomizationSettings;
            placementGuideRotationRandomizationSettingsView = placementGuideRotationRandomizationSettings.View;
            placementGuideRotationRandomizationSettingsView.VisibilityToggleIndent = 1;
            placementGuideRotationRandomizationSettingsView.VisibilityToggleLabel = "Rotation Randomization Settings";
            placementGuideRotationRandomizationSettingsView.IsVisible = false;
            placementGuideRotationRandomizationSettings.XAxisRandomizationSettings.View.VisibilityToggleLabel = placementGuideRotationRandomizationSettings.XAxisRandomizationSettings.Axis + " Axis Settings";
            placementGuideRotationRandomizationSettings.XAxisRandomizationSettings.View.IsVisible = false;
            placementGuideRotationRandomizationSettings.YAxisRandomizationSettings.View.VisibilityToggleLabel = placementGuideRotationRandomizationSettings.YAxisRandomizationSettings.Axis + " Axis Settings";
            placementGuideRotationRandomizationSettings.YAxisRandomizationSettings.View.IsVisible = false;
            placementGuideRotationRandomizationSettings.ZAxisRandomizationSettings.View.VisibilityToggleLabel = placementGuideRotationRandomizationSettings.ZAxisRandomizationSettings.Axis + " Axis Settings";
            placementGuideRotationRandomizationSettings.ZAxisRandomizationSettings.View.IsVisible = false;
            placementGuideRotationRandomizationSettings.CustomAxisRandomizationSettings.View.VisibilityToggleLabel = "Placement Surface Normal Settings";

            ObjectScaleRandomizationSettings placementGuideScaleRandomizationSettings = PointAndClickObjectPlacementSettings.Get().PlacementGuideScaleRandomizationSettings;
            ObjectScaleRandomizationSettingsView guideScaleRandomizationSettingsView = placementGuideScaleRandomizationSettings.View;
            guideScaleRandomizationSettingsView.VisibilityToggleIndent = 1;
            guideScaleRandomizationSettingsView.VisibilityToggleLabel = "Scale Randomization Settings";
            guideScaleRandomizationSettingsView.IsVisible = false;
            guideScaleRandomizationSettingsView.IndentContent = true;

            placementGuideScaleRandomizationSettings = DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.PlacementGuideScaleRandomizationSettings;
            guideScaleRandomizationSettingsView = placementGuideScaleRandomizationSettings.View;
            guideScaleRandomizationSettingsView.VisibilityToggleIndent = 1;
            guideScaleRandomizationSettingsView.VisibilityToggleLabel = "Scale Randomization Settings";
            guideScaleRandomizationSettingsView.IsVisible = false;
            guideScaleRandomizationSettingsView.IndentContent = true;

            AxisAlignmentSettingsView placementGuideSurfaceAlignmentSettingsView = PointAndClickObjectPlacementSettings.Get().PlacementGuideSurfaceAlignmentSettings.View;
            placementGuideSurfaceAlignmentSettingsView.VisibilityToggleIndent = 1;
            placementGuideSurfaceAlignmentSettingsView.VisibilityToggleLabel = "Surface Alignment Settings";

            placementGuideSurfaceAlignmentSettingsView = PathObjectPlacementSettings.Get().PlacementGuideSurfaceAlignmentSettings.View;
            placementGuideSurfaceAlignmentSettingsView.VisibilityToggleIndent = 1;
            placementGuideSurfaceAlignmentSettingsView.VisibilityToggleLabel = "Surface Alignment Settings";

            placementGuideSurfaceAlignmentSettingsView = BlockObjectPlacementSettings.Get().PlacementGuideSurfaceAlignmentSettings.View;
            placementGuideSurfaceAlignmentSettingsView.VisibilityToggleIndent = 1;
            placementGuideSurfaceAlignmentSettingsView.VisibilityToggleLabel = "Surface Alignment Settings";

            placementGuideSurfaceAlignmentSettingsView = DecorPaintObjectPlacementSettings.Get().SingleDecorPaintModeSettings.PlacementGuideSurfaceAlignmentSettings.View;
            placementGuideSurfaceAlignmentSettingsView.VisibilityToggleIndent = 1;
            placementGuideSurfaceAlignmentSettingsView.VisibilityToggleLabel = "Surface Alignment Settings";

            ObjectRotationRandomizationSettings blockObjectRotationRandomizationSettings = ObjectPlacement.Get().BlockObjectPlacement.BlockSettings.ManualConstructionSettings.ObjectRotationRandomizationSettings;
            ObjectRotationRandomizationSettingsView blockObjectRotationRandomizationSettingsView = blockObjectRotationRandomizationSettings.View;
            blockObjectRotationRandomizationSettings.CustomAxisRandomizationSettings.View.ToggleVisibilityBeforeRender = false;
            blockObjectRotationRandomizationSettings.CustomAxisRandomizationSettings.View.IsVisible = false;
            blockObjectRotationRandomizationSettings.CustomAxisRandomizationSettings.Randomize = false;
            blockObjectRotationRandomizationSettingsView.SurroundWithBox = false;
            blockObjectRotationRandomizationSettingsView.VisibilityToggleIndent = 1;
            blockObjectRotationRandomizationSettings.XAxisRandomizationSettings.View.VisibilityToggleLabel = blockObjectRotationRandomizationSettings.XAxisRandomizationSettings.Axis + " Axis Settings";
            blockObjectRotationRandomizationSettings.YAxisRandomizationSettings.View.VisibilityToggleLabel = blockObjectRotationRandomizationSettings.YAxisRandomizationSettings.Axis + " Axis Settings";
            blockObjectRotationRandomizationSettings.ZAxisRandomizationSettings.View.VisibilityToggleLabel = blockObjectRotationRandomizationSettings.ZAxisRandomizationSettings.Axis + " Axis Settings";

            XZOrientedEllipseShapeRenderSettingsView xzOrientedElipseShapeRenderSettingsView = DecorPaintObjectPlacement.Get().BrushCircleRenderSettings.View;
            xzOrientedElipseShapeRenderSettingsView.ToggleVisibilityBeforeRender = true;
            xzOrientedElipseShapeRenderSettingsView.IndentContent = true;
            xzOrientedElipseShapeRenderSettingsView.VisibilityToggleLabel = "Brush Circle";
            xzOrientedElipseShapeRenderSettingsView.IsVisible = false;

            ObjectPivotPointsRenderSettingsView guidePivotPointsSettingsView = ObjectPlacement.Get().GuidePivotPointsRenderSettings.View;
            guidePivotPointsSettingsView.ToggleVisibilityBeforeRender = true;
            guidePivotPointsSettingsView.IndentContent = true;
            guidePivotPointsSettingsView.VisibilityToggleLabel = "Guide Pivot Points";
            guidePivotPointsSettingsView.IsVisible = false;

            ObjectPlacement.Get().ObjectVertexSnapSessionRenderSettings.View.IsVisible = false;

            ProjectedBoxFacePivotPointsRenderSettings projectedBoxFacePointsRenderSettings = ObjectPlacement.Get().GuidePivotPointsRenderSettings.ProjectedBoxFacePivotPointsRenderSettings;
            ProjectedBoxFacePivotPointsRenderSettingsView projectedBoxFacePointsRenderSettingsView = projectedBoxFacePointsRenderSettings.View;
            projectedBoxFacePointsRenderSettingsView.ToggleVisibilityBeforeRender = true;
            projectedBoxFacePointsRenderSettingsView.IndentContent = true;
            projectedBoxFacePointsRenderSettingsView.VisibilityToggleLabel = "Projected Guide Pivot Points";
            projectedBoxFacePointsRenderSettingsView.IsVisible = false;

            SingleObjectPivotPointRenderSettingsView activePivotPointRenderSettingsView = projectedBoxFacePointsRenderSettings.ActivePivotPointRenderSettings.View;
            activePivotPointRenderSettingsView.ToggleVisibilityBeforeRender = true;
            activePivotPointRenderSettingsView.VisibilityToggleLabel = "Active Pivot Point";
            activePivotPointRenderSettingsView.IndentContent = true;
            activePivotPointRenderSettingsView.VisibilityToggleIndent = 1;

            SingleObjectPivotPointRenderSettingsView inactivePivotPointRenderSettingsView = projectedBoxFacePointsRenderSettings.InactivePivotPointRenderSettings.View;
            inactivePivotPointRenderSettingsView.ToggleVisibilityBeforeRender = true;
            inactivePivotPointRenderSettingsView.VisibilityToggleLabel = "Inactive Pivot Point";
            inactivePivotPointRenderSettingsView.IndentContent = true;
            inactivePivotPointRenderSettingsView.VisibilityToggleIndent = 1;

            ObjectMaskView objectMaskView = DecorPaintObjectPlacement.Get().DecorPaintMask.View;
            objectMaskView.SurroundWithBox = true;

            ObjectLayerObjectMaskView objectLayerObjectMaskView = DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectLayerObjectMask.View;
            objectLayerObjectMaskView.IsVisible = false;
            objectLayerObjectMaskView.ToggleVisibilityBeforeRender = true;
            objectLayerObjectMaskView.VisibilityToggleIndent = 1;
            objectLayerObjectMaskView.VisibilityToggleLabel = "Object Layer Decor Paint Mask";

            ObjectCollectionMaskView objectCollectionMaskView = DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectCollectionMask.View;
            objectCollectionMaskView.IsVisible = false;
            objectCollectionMaskView.ToggleVisibilityBeforeRender = true;
            objectCollectionMaskView.VisibilityToggleIndent = 1;
            objectCollectionMaskView.VisibilityToggleLabel = "Object Decor Paint Mask";

            LabelRenderSettings labelRenderSettings = ObjectPlacement.Get().BlockObjectPlacement.BlockRenderSettings.ManualConstructionRenderSettings.DimensionsLabelRenderSettings;
            labelRenderSettings.Bold = true;
            labelRenderSettings.FontSize = 15;
            labelRenderSettings.TextColor = Color.white;
            LabelRenderSettingsView labelRenderSettingsView = labelRenderSettings.View;
            labelRenderSettingsView.ToggleVisibilityBeforeRender = true;
            labelRenderSettingsView.VisibilityToggleLabel = "Dimensions Label";
            labelRenderSettingsView.IndentContent = true;
        }

        public override void Render()
        {
            if (ObjectPlacement.Get().IsPlacementLocked)
            {
                string helpMessage = "Object placement is currently locked. Press \'" + AllShortcutCombos.Instance.LockObjectPlacement.ToString() + "\' to toggle the lock state.";
                EditorGUILayout.HelpBox(helpMessage, UnityEditor.MessageType.Warning);
            }

            RenderXZGridRotationField();
            ObjectPlacement.Get().MirrorView.Render();
            Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase.View.Render();
            ObjectPlacementSettings.Get().View.Render();
            if (ObjectPlacementSettings.Get().ObjectPlacementMode == ObjectPlacementMode.DecorPaint) RenderControlsForDecorPaintPlacementMode();

            _objectPlacementLookAndFeelSettingsView.Render();
        }
        #endregion

        #region Private Methods
        private void RenderControlsForDecorPaintPlacementMode()
        {
            if (DecorPaintObjectPlacementSettings.Get().DecorPaintMode == DecorPaintMode.Brush)
            {
                DecorPaintObjectPlacementBrushDatabase.Get().View.Render();
                EditorGUILayout.Separator();
            }

            DecorPaintObjectPlacement.Get().DecorPaintMask.View.Render();
        }

        private void RenderXZGridRotationField()
        {
            XZGrid xzGrid = ObjectSnapping.Get().XZSnapGrid;
            Vector3 currentRotation = xzGrid.Rotation.eulerAngles;
            Vector3 newVector = EditorGUILayout.Vector3Field(GetContentForXZGridRotationField(), currentRotation);
            if(newVector != currentRotation)
            {
                UndoEx.RecordForToolAction(xzGrid);
                xzGrid.SetRotation(Quaternion.Euler(newVector));

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForXZGridRotationField()
        {
            var content = new GUIContent();
            content.text = "XZ grid rotation";
            content.tooltip = "Allows you to adjust the rotation of the XZ grid.";

            return content;
        }
        #endregion
    }
}
#endif
