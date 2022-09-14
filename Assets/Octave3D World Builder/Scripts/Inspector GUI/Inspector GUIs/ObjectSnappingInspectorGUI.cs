#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSnappingInspectorGUI : InspectorGUI
    {
        #region Private Variables
        [SerializeField]
        private ObjectSnappingLookAndFeelSettingsView _objectSnappingLookAndFeelSettingsView = new ObjectSnappingLookAndFeelSettingsView();
        #endregion

        #region Public Methods
        public override void Initialize()
        {
            base.Initialize();

            _objectSnappingLookAndFeelSettingsView.IsVisible = false;

            XZGridCellSizeSettingsView xzGridCellSizeSettingsView = ObjectSnapping.Get().XZSnapGrid.CellSizeSettings.View;
            xzGridCellSizeSettingsView.ToggleVisibilityBeforeRender = true;
            xzGridCellSizeSettingsView.SurroundWithBox = true;
            xzGridCellSizeSettingsView.VisibilityToggleLabel = xzGridCellSizeSettingsView.XAxisName + xzGridCellSizeSettingsView.ZAxisName + " Grid Cell Size Settings";

            XZGridRenderSettingsView xzGridRenderSettingsView = ObjectSnapping.Get().XZSnapGrid.RenderSettings.View;
            xzGridRenderSettingsView.IsVisible = false;
            xzGridRenderSettingsView.ToggleVisibilityBeforeRender = true;
            xzGridRenderSettingsView.VisibilityToggleIndent = 1;
            xzGridRenderSettingsView.IndentContent = true;
            xzGridRenderSettingsView.VisibilityToggleLabel = "XZ Grid";
            xzGridRenderSettingsView.ModifyLineThickness = false;

            XZGridRenderSettingsView colliderSnapSurfaceGrid = ObjectSnapping.Get().RenderSettingsForColliderSnapSurfaceGrid.View;
            ObjectSnapping.Get().RenderSettingsForColliderSnapSurfaceGrid.CellLineThickness = 0.05f;
            colliderSnapSurfaceGrid.IsVisible = false;
            colliderSnapSurfaceGrid.ToggleVisibilityBeforeRender = true;
            colliderSnapSurfaceGrid.VisibilityToggleIndent = 1;
            colliderSnapSurfaceGrid.IndentContent = true;
            colliderSnapSurfaceGrid.VisibilityToggleLabel = "Collider Snap Surface Grid";

            CoordinateSystemRenderSettings xzGridCoordSystemRenderSettings = ObjectSnapping.Get().XZSnapGrid.RenderableCoordinateSystem.RenderSettings;
            CoordinateSystemRenderSettingsView xzGridCoordSystemRenderSettingsView = xzGridCoordSystemRenderSettings.View;
            xzGridCoordSystemRenderSettingsView.IsVisible = false;
            xzGridCoordSystemRenderSettingsView.ToggleVisibilityBeforeRender = true;
            xzGridCoordSystemRenderSettingsView.VisibilityToggleIndent = 1;
            xzGridCoordSystemRenderSettingsView.IndentContent = true;
            xzGridCoordSystemRenderSettingsView.VisibilityToggleLabel = "XZ Grid Coord System";

            CoordinateSystemAxisRenderSettings axisRenderSettings = xzGridCoordSystemRenderSettings.GetAxisRenderSettings(CoordinateSystemAxis.PositiveRight);
            axisRenderSettings.View.VisibilityToggleLabel = axisRenderSettings.Axis.ToString() + " Axis";
            axisRenderSettings = xzGridCoordSystemRenderSettings.GetAxisRenderSettings(CoordinateSystemAxis.NegativeRight);
            axisRenderSettings.View.VisibilityToggleLabel = axisRenderSettings.Axis.ToString() + " Axis";
            axisRenderSettings = xzGridCoordSystemRenderSettings.GetAxisRenderSettings(CoordinateSystemAxis.PositiveUp);
            axisRenderSettings.View.VisibilityToggleLabel = axisRenderSettings.Axis.ToString() + " Axis";
            axisRenderSettings = xzGridCoordSystemRenderSettings.GetAxisRenderSettings(CoordinateSystemAxis.NegativeUp);
            axisRenderSettings.View.VisibilityToggleLabel = axisRenderSettings.Axis.ToString() + " Axis";
            axisRenderSettings = xzGridCoordSystemRenderSettings.GetAxisRenderSettings(CoordinateSystemAxis.PositiveLook);
            axisRenderSettings.View.VisibilityToggleLabel = axisRenderSettings.Axis.ToString() + " Axis";
            axisRenderSettings = xzGridCoordSystemRenderSettings.GetAxisRenderSettings(CoordinateSystemAxis.NegativeLook);
            axisRenderSettings.View.VisibilityToggleLabel = axisRenderSettings.Axis.ToString() + " Axis";

            ObjectSnapping.Get().ObjectSnapMask.View.SurroundWithBox = true;

            ObjectLayerObjectMaskView objectLayerObjectMaskView = ObjectSnapping.Get().ObjectSnapMask.ObjectLayerObjectMask.View;
            objectLayerObjectMaskView.IsVisible = false;
            objectLayerObjectMaskView.ToggleVisibilityBeforeRender = true;
            objectLayerObjectMaskView.VisibilityToggleIndent = 1;
            objectLayerObjectMaskView.VisibilityToggleLabel = "Object Layer Snap Mask";

            ObjectCollectionMaskView objectCollectionMaskView = ObjectSnapping.Get().ObjectSnapMask.ObjectCollectionMask.View;
            objectCollectionMaskView.IsVisible = false;
            objectCollectionMaskView.ToggleVisibilityBeforeRender = true;
            objectCollectionMaskView.VisibilityToggleIndent = 1;
            objectCollectionMaskView.VisibilityToggleLabel = "Object Snap Mask";
        }

        public override void Render()
        {
            ObjectSnapping.Get().Settings.View.Render();
            ObjectSnapping.Get().XZSnapGrid.CellSizeSettings.View.Render();

            EditorGUILayout.Separator();
            ObjectSnapping.Get().ObjectSnapMask.View.Render();

            EditorGUILayout.Separator();
            _objectSnappingLookAndFeelSettingsView.Render();
        }
        #endregion
    }
}
#endif
