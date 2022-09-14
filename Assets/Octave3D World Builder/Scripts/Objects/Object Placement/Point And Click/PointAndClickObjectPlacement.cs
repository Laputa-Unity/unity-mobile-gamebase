#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PointAndClickObjectPlacement
    {
        #region Public Static Functions
        public static PointAndClickObjectPlacement Get()
        {
            return ObjectPlacement.Get().PointAndClickObjectPlacement;
        }
        #endregion

        #region Public Methods
        public void HandleMouseMoveEvent(Event e)
        {
            if (ObjectPlacementGuide.ExistsInSceneAndIsActive) 
            {
                e.DisableInSceneView();
                AdjustGuidePositionAndRotation(e);
            }
        }

        public void HandleMouseButtonDownEvent(Event e)
        {
            if (e.InvolvesLeftMouseButton())
            {
                e.DisableInSceneView();

                if (ObjectPlacementGuide.ExistsInScene)
                {
                    PlaceObject();
                    ApplyRandomizationsForPlacementGuide();
                }
            }
        }
        #endregion

        #region Private Methods
        private void AdjustGuidePositionAndRotation(Event e)
        {
            ObjectPlacementGuide.Instance.Snap();
            AxisAlignment.AlignObjectAxis(ObjectPlacementGuide.SceneObject, PointAndClickObjectPlacementSettings.Get().PlacementGuideSurfaceAlignmentSettings, ObjectSnapping.Get().ObjectSnapSurfacePlane.normal);
        }

        private void ApplyRandomizationsForPlacementGuide()
        {
            PointAndClickObjectPlacementSettings.Get().PlacementGuideRotationRandomizationSettings.CustomAxisRandomizationSettings.Axis = ObjectSnapping.Get().ObjectSnapSurfacePlane.normal;

            ObjectRotationRandomization.Randomize(ObjectPlacementGuide.SceneObject, PointAndClickObjectPlacementSettings.Get().PlacementGuideRotationRandomizationSettings);
            ObjectScaleRandomization.Randomize(ObjectPlacementGuide.SceneObject, PointAndClickObjectPlacementSettings.Get().PlacementGuideScaleRandomizationSettings);
        }

        private void PlaceObject()
        {
            ObjectPlacementGuide placementGuide = ObjectPlacementGuide.Instance;
            GameObject placedHierarchyRoot = Octave3DScene.Get().InstantiateObjectHierarchyFromPrefab(placementGuide.SourcePrefab, placementGuide.WorldPosition, placementGuide.WorldRotation, placementGuide.WorldScale);

            ObjectHierarchyRootsWerePlacedInSceneMessage.SendToInterestedListeners(placedHierarchyRoot, ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.ObjectPlacement);

            if (PointAndClickObjectPlacementSettings.Get().RandomizePrefabsInActiveCategory)
            {
                PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
                if (activePrefabCategory != null) activePrefabCategory.RandomizeActivePrefab();
            }
        }
        #endregion
    }
}
#endif