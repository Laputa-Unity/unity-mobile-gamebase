#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PathObjectPlacement
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementPath _objectPlacementPath = new ObjectPlacementPath();
        #endregion

        #region Public Properties
        public List<OrientedBox> AllOrientedBoxesInPath { get { return _objectPlacementPath.GetAllOrientedBoxes(); } }
        public ObjectPlacementPathSettings PathSettings { get { return _objectPlacementPath.Settings; } }
        public ObjectPlacementPathRenderSettings PathRenderSettings { get { return _objectPlacementPath.RenderSettings; } }
        public ObjectPlacementExtensionPlaneRenderSettings PathExtensionPlaneRenderSettings { get { return _objectPlacementPath.ExtensionPlaneRenderSettings; } }
        public bool IsPathUnderManualConstruction { get { return _objectPlacementPath.IsUnderManualConstruction; } }
        #endregion

        #region Public Static Functions
        public static PathObjectPlacement Get()
        {
            return ObjectPlacement.Get().PathObjectPlacement;
        }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            _objectPlacementPath.RenderGizmos();
        }

        public void CancelManualPathConstruction()
        {
            ObjectPlacementGuide.Active = true;
            _objectPlacementPath.CancelManualConstruction();
        }

        public void HandleMouseMoveEvent(Event e)
        {
            if (!_objectPlacementPath.IsUnderManualConstruction && ObjectPlacementGuide.ExistsInSceneAndIsActive)
            {
                ObjectPlacementGuide.Instance.Snap();
                AxisAlignment.AlignObjectAxis(ObjectPlacementGuide.SceneObject, PathObjectPlacementSettings.Get().PlacementGuideSurfaceAlignmentSettings, ObjectSnapping.Get().ObjectSnapSurfacePlane.normal);

                // Note: This is necessary just in case the placement guide has changed.
                _objectPlacementPath.SetStartObject(ObjectPlacementGuide.SceneObject);
            }
            else if(ObjectPlacementGuide.ExistsInScene) _objectPlacementPath.UpdateForMouseMoveEvent();
        }

        public void HandleMouseButtonDownEvent(Event e)
        {
            if (ObjectPlacementGuide.ExistsInScene && _objectPlacementPath.IsUnderManualConstruction && AllShortcutCombos.Instance.PlacePathOnClick.IsActive())
            {
                e.DisableInSceneView();
                List<GameObject> placedHierarchyRoots = Octave3DScene.Get().InstantiateObjectHirarchiesFromPlacementDataCollection(_objectPlacementPath.EndManualConstruction());
                ObjectHierarchyRootsWerePlacedInSceneMessage.SendToInterestedListeners(placedHierarchyRoots, ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.ObjectPlacement);

                if (ObjectPlacementGuide.ExistsInScene) ObjectPlacementGuide.Active = true;
                return;
            }

            if (_objectPlacementPath.IsUnderManualConstruction && AllShortcutCombos.Instance.ManualAttach2NewSegmentsToPath.IsActive())
            {
                e.DisableInSceneView();
                _objectPlacementPath.Attach2NewSegmentsIfUnderManualConstruction();
            }

            if (!_objectPlacementPath.IsUnderManualConstruction && AllShortcutCombos.Instance.BeginManualPathConstruction.IsActive() && e.InvolvesLeftMouseButton())
            {
                e.DisableInSceneView();

                if(ObjectPlacementGuide.ExistsInScene)
                {
                    _objectPlacementPath.SetStartObject(ObjectPlacementGuide.SceneObject);
                    _objectPlacementPath.BeginManualConstruction();
                    if (_objectPlacementPath.IsUnderManualConstruction) ObjectPlacementGuide.Active = false;
                }
                else Debug.LogWarning("Can not begin path construction because the placement guide does not exist in the scene. Please activate a prefab to start construction.");
            }              
        }

        public void HandleKeyboardButtonDownEvent(Event e)
        {
            if(AllShortcutCombos.Instance.CancelManualPathConstruction.IsActive())
            {
                e.DisableInSceneView();
                CancelManualPathConstruction();
            }
            else
            if(AllShortcutCombos.Instance.ManualRaisePath.IsActive())
            {
                e.DisableInSceneView();
                _objectPlacementPath.ManualRaise();
            }
            else 
            if(AllShortcutCombos.Instance.ManualLowerPath.IsActive())
            {
                e.DisableInSceneView();
                _objectPlacementPath.ManualLower();
            }
            else
            if(AllShortcutCombos.Instance.ManualRemoveLast2SegmentsInPath.IsActive())
            {
                e.DisableInSceneView();
                _objectPlacementPath.RemoveLast2SegmentsIfUnderManualConstruction();
            }
            else
            if(AllShortcutCombos.Instance.NextPathExtensionPlane.IsActive())
            {
                e.DisableInSceneView();
                _objectPlacementPath.NextExtensionPlane();
            }
        }
        #endregion
    }
}
#endif