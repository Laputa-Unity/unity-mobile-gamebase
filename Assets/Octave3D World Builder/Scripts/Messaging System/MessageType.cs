#if UNITY_EDITOR
namespace O3DWB
{
    public enum MessageType
    {
        // Tool messages
        ToolWasReset = 0,
        ToolWasStarted,
        ToolWasSelected,

        // GUI messages
        InspectorGUIWasChanged,

        // Undo/Redo messages
        UndoRedoWasPerformed,

        // Prefab messages
        PrefabWasRemovedFromCategory,
        PrefabWasTransferredToCategory,
        NewPrefabWasActivated,   

        // Prefab tag messages
        PrefabTagWasCreatedInDatabase,
        PrefabTagWasRemovedFromDatabase,
        PrefabTagActiveStateWasChanged,

        // Prefab category messages
        PrefabCategoryWasRemovedFromDatabase,
        NewPrefabCategoryWasActivated,      

        // Object placement messages
        ObjectPlacementGuideWasInstantiated,
        ObjectPlacementModeWasChanged,
        ObjectHierarchyRootsWerePlacedInScene,

        // Object placement path messages
        ObjectPlacementPathExcludeCornersWasChanged,
        ObjectPlacementPathRotateObjectsToFollowPathWasChanged,
        ObjectPlacementPathPaddingSettingsWereChanged,
        ObjectPlacementPathBorderSettingsWereChanged,
        ObjectPlacementPathHeightAdjustmentModeWasChanged,
        ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsWereChanged,
        ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChanged,

        // Object placement block messages
        ObjectPlacementBlockExcludeCornersWasChanged,
        ObjectPlacementBlockPaddingSettingsWereChanged,
        ObjectPlacementBlockHeightAdjustmentModeWasChanged,
        ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChanged,
        ObjectPlacementBlockSubdivisionSettingsWereChanged,

        // Object placement path height pattern messages
        ObjectPlacementPathHeightPatternWasRemovedFromDatabase,
        NewObjectPlacementPathHeightPatternWasActivated,

        // Transform gizmos
        GizmoTransformedObjects
    }
}
#endif
