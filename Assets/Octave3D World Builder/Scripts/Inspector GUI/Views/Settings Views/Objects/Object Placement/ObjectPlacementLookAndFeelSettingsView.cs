#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementLookAndFeelSettingsView : SettingsView
    {
        #region Private Variables
        [SerializeField]
        private DecorPaintObjectPlacementLookAndFeelSettingsView _decorPaintLookAndFeelSettingsView = new DecorPaintObjectPlacementLookAndFeelSettingsView();
        [SerializeField]
        private PathObjectPlacementLookAndFeelSettingsView _pathObjectPlacementLookAndFeelSettingsView = new PathObjectPlacementLookAndFeelSettingsView();
        [SerializeField]
        private BlockObjectPlacementLookAndFeelSettingsView _blockObjectPlacementLookAndFeelSettingsView = new BlockObjectPlacementLookAndFeelSettingsView();
        #endregion

        #region Public Properties
        public DecorPaintObjectPlacementLookAndFeelSettingsView DecorPaintLookAndFeelSettingsView { get { return _decorPaintLookAndFeelSettingsView; } }
        public PathObjectPlacementLookAndFeelSettingsView PathObjectPlacementLookAndFeelSettingsView { get { return _pathObjectPlacementLookAndFeelSettingsView; } }
        public BlockObjectPlacementLookAndFeelSettingsView BlockObjectPlacementLookAndFeelSettingsView { get { return _blockObjectPlacementLookAndFeelSettingsView; } }
        #endregion

        #region Constructors
        public ObjectPlacementLookAndFeelSettingsView()
        {
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Look and Feel";
            IndentContent = true;
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            ObjectPlacement.Get().GuidePivotPointsRenderSettings.View.Render();
            ObjectPlacement.Get().ObjectVertexSnapSessionRenderSettings.View.Render();

            ObjectPlacementMode placementMode = ObjectPlacement.Get().ObjectPlacementMode;
            if (placementMode == ObjectPlacementMode.Path) _pathObjectPlacementLookAndFeelSettingsView.Render();
            else if (placementMode == ObjectPlacementMode.Block) _blockObjectPlacementLookAndFeelSettingsView.Render();
            else if (placementMode == ObjectPlacementMode.DecorPaint) _decorPaintLookAndFeelSettingsView.Render();
        }
        #endregion
    }
}
#endif