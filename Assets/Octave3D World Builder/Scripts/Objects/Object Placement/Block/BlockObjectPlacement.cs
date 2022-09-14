#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class BlockObjectPlacement
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementBlock _objectPlacementBlock = new ObjectPlacementBlock();
        #endregion

        #region Public Properties
        public List<OrientedBox> AllOrientedBoxesInBlock { get { return _objectPlacementBlock.GetAllOrientedBoxes(); } }
        public ObjectPlacementBlockSettings BlockSettings { get { return _objectPlacementBlock.Settings; } }
        public ObjectPlacementBlockRenderSettings BlockRenderSettings { get { return _objectPlacementBlock.RenderSettings; } }
        public ObjectPlacementExtensionPlaneRenderSettings BlockExtensionPlaneRenderSettings { get { return _objectPlacementBlock.ExtensionPlaneRenderSettings; } }
        public bool IsBlockUnderManualConstruction { get { return _objectPlacementBlock.IsUnderManualConstruction; } }
        #endregion

        #region Public Static Functions
        public static BlockObjectPlacement Get()
        {
            return ObjectPlacement.Get().BlockObjectPlacement;
        }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            _objectPlacementBlock.RenderGizmos();
        }

        public void RenderHandles()
        {
            _objectPlacementBlock.RenderHandles();
        }

        public void CancelManualBlockConstruction()
        {
            ObjectPlacementGuide.Active = true;
            _objectPlacementBlock.CancelManualConstruction();
        }

        public void HandleMouseMoveEvent(Event e)
        {
            if (CanAdjustGuidePositionAndRotation())
            {
                AdjustGuidePositionAndRotation();

                // Note: This is necessary just in case the placement guide has changed.
                _objectPlacementBlock.SetStartObject(ObjectPlacementGuide.SceneObject);
            }
            else 
            if (CanUpdateBlock()) _objectPlacementBlock.UpdateForMouseMoveEvent();
        }

        public void HandleMouseButtonDownEvent(Event e)
        {
            if (CanPlaceBlock())
            {
                e.DisableInSceneView();
                PlaceBlock();
            }
            else
            if (CanBeginBlockManualConstruction() && e.InvolvesLeftMouseButton())
            {
                e.DisableInSceneView();
                BeginManualBlockConstruction();
            }
        }

        public void HandleKeyboardButtonDownEvent(Event e)
        {
            if (AllShortcutCombos.Instance.CancelManualBlockConstruction.IsActive())
            {
                e.DisableInSceneView();
                CancelManualBlockConstruction();
            }
            else
            if (AllShortcutCombos.Instance.ManualRaiseBlock.IsActive())
            {
                e.DisableInSceneView();
                _objectPlacementBlock.ManualRaise();
            }
            else
            if (AllShortcutCombos.Instance.ManualLowerBlock.IsActive())
            {
                e.DisableInSceneView();
                _objectPlacementBlock.ManualLower();
            }
            else
            if (AllShortcutCombos.Instance.NextBlockExtensionPlane.IsActive())
            {
                e.DisableInSceneView();
                _objectPlacementBlock.NextExtensionPlane();
            }
        }
        #endregion

        #region Private Methods
        private bool CanAdjustGuidePositionAndRotation()
        {
            return !_objectPlacementBlock.IsUnderManualConstruction && ObjectPlacementGuide.ExistsInSceneAndIsActive;
        }

        private bool CanUpdateBlock()
        {
            return ObjectPlacementGuide.ExistsInScene;
        }

        private bool CanPlaceBlock()
        {
            return ObjectPlacementGuide.ExistsInScene && _objectPlacementBlock.IsUnderManualConstruction && AllShortcutCombos.Instance.PlaceBlockOnClick.IsActive();
        }

        private bool CanBeginBlockManualConstruction()
        {
            return ObjectPlacementGuide.ExistsInScene && !_objectPlacementBlock.IsUnderManualConstruction && AllShortcutCombos.Instance.BeginManualBlockConstruction.IsActive();
        }

        private void PlaceBlock()
        {
            List<GameObject> placedHierarchyRoots = Octave3DScene.Get().InstantiateObjectHirarchiesFromPlacementDataCollection(_objectPlacementBlock.EndManualConstruction());
            ObjectHierarchyRootsWerePlacedInSceneMessage.SendToInterestedListeners(placedHierarchyRoots, ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.ObjectPlacement);
            ObjectPlacementGuide.Active = true;
        }

        private void BeginManualBlockConstruction()
        {
            _objectPlacementBlock.SetStartObject(ObjectPlacementGuide.SceneObject);
            _objectPlacementBlock.BeginManualConstruction();
            ObjectPlacementGuide.Active = false;
        }

        private void AdjustGuidePositionAndRotation()
        {
            ObjectPlacementGuide.Instance.Snap();
            AxisAlignment.AlignObjectAxis(ObjectPlacementGuide.SceneObject, BlockObjectPlacementSettings.Get().PlacementGuideSurfaceAlignmentSettings, ObjectSnapping.Get().ObjectSnapSurfacePlane.normal);
        }
        #endregion
    }
}
#endif