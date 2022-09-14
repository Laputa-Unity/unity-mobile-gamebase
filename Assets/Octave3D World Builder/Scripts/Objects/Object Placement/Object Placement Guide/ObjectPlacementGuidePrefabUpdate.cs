#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectPlacementGuidePrefabUpdate
    {
        #region Public Static Functions
        public static void EnsureGuideUsesCorrectPrefab()
        {
            if(ObjectPlacement.Get().IsObjectVertexSnapSessionActive)
            {
                ObjectPlacement.Get().DestroyPlacementGuide();
                return;
            }

            if(ObjectPlacement.Get().ObjectPlacementMode == ObjectPlacementMode.VolumeTiles)
            {
                ObjectPlacement.Get().DestroyPlacementGuide();
                return;
            }

            if (ObjectPlacement.Get().UserWantsToPlaceTileConnections) EnsureGuideUsesBeginTileConnectionPrefab();
            else
            if (ObjectPlacement.Get().UsingBrushDecorPaintMode) ObjectPlacement.Get().DestroyPlacementGuide();
            else EnsureGuideUsesActivePrefab();

            // If the placement guide exists in the scene but its source prefab is not available, destroy the guide
            if (ObjectPlacementGuide.ExistsInScene)
            {
                if (!ObjectPlacementGuide.Instance.IsSourcePrefabAvailable) ObjectPlacement.Get().DestroyPlacementGuide();
            }
        }
        #endregion

        #region Private Static Functions
        private static void EnsureGuideUsesBeginTileConnectionPrefab()
        {
            ObjectPlacementPathTileConnectionSettings tileConnectionSettings = ObjectPlacement.Get().PathObjectPlacement.PathSettings.TileConnectionSettings;
            ObjectPlacementPathTileConnectionTypeSettings beginTileConnectionSettings = tileConnectionSettings.GetSettingsForTileConnectionType(ObjectPlacementPathTileConnectionType.Begin);

            if (CanRefreshGuideToUseBeginTileConnectionPrefab(beginTileConnectionSettings))
            {
                PrefabCategory categoryWhichContainsBeginPrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(beginTileConnectionSettings.Prefab);
                if (categoryWhichContainsBeginPrefab == null) return;

                PrefabCategoryDatabase.Get().SetActivePrefabCategory(categoryWhichContainsBeginPrefab);
                categoryWhichContainsBeginPrefab.SetActivePrefab(beginTileConnectionSettings.Prefab);

                ObjectPlacement.Get().DestroyPlacementGuide();
                ObjectPlacementGuide.CreateFromActivePrefabIfNotExists();
            }

            // Note: When using tile connections, we will always use the original prefab scale
            if (ObjectPlacementGuide.ExistsInScene) ObjectPlacementGuide.Instance.WorldScale = beginTileConnectionSettings.Prefab.InitialWorldScale;
        }

        private static bool CanRefreshGuideToUseBeginTileConnectionPrefab(ObjectPlacementPathTileConnectionTypeSettings beginTileConnectionSettings)
        {
            return ((ObjectPlacementGuide.ExistsInScene && beginTileConnectionSettings.Prefab != ObjectPlacementGuide.Instance.SourcePrefab) ||
                    !ObjectPlacementGuide.ExistsInScene);
        }

        private static void EnsureGuideUsesActivePrefab()
        {
            PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
            if(activePrefabCategory != null)
            {
                Prefab activePrefab = activePrefabCategory.ActivePrefab;
                if(activePrefab != null)
                {
                    if (!ObjectPlacementGuide.ExistsInScene) ObjectPlacementGuide.CreateFromActivePrefabIfNotExists();
                    else if(ObjectPlacementGuide.Instance.SourcePrefab != activePrefab)
                    {
                        ObjectPlacement.Get().DestroyPlacementGuide();
                        ObjectPlacementGuide.CreateFromActivePrefabIfNotExists();
                    }
                }
            }
        }
        #endregion
    }
}
#endif