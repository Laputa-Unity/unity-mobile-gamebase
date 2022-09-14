#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class MessageListenerRegistration
    {
        #region Public Static Functions
        public static void PerformRegistrationForPrefabManagementWindow(PrefabManagementWindow prefabManagementWindow)
        {
            if (prefabManagementWindow != null)
            {
                MessageListenerDatabase listenerDatabase = MessageListenerDatabase.Instance;
                listenerDatabase.UnregisterListener(prefabManagementWindow);

                listenerDatabase.RegisterListenerForMessage(MessageType.NewPrefabWasActivated, prefabManagementWindow);
                listenerDatabase.RegisterListenerForMessage(MessageType.NewPrefabCategoryWasActivated, prefabManagementWindow);
                listenerDatabase.RegisterListenerForMessage(MessageType.PrefabTagActiveStateWasChanged, prefabManagementWindow);
                listenerDatabase.RegisterListenerForMessage(MessageType.PrefabTagWasCreatedInDatabase, prefabManagementWindow);
                listenerDatabase.RegisterListenerForMessage(MessageType.PrefabTagWasRemovedFromDatabase, prefabManagementWindow);
            }
        }

        public static void PerformRegistrationForNamedEntityCollectionWithMarker<T>(NamedEntityCollectionWithEntityMarker<T> entityCollection) where T : class, INamedEntity
        {
            if (entityCollection != null)
            {
                MessageListenerDatabase listenerDatabase = MessageListenerDatabase.Instance;
                listenerDatabase.UnregisterListener(entityCollection);

                listenerDatabase.RegisterListenerForMessage(MessageType.UndoRedoWasPerformed, entityCollection);
            }
        }

        public static void PerformRegistrationForOctave3DScene(Octave3DScene octave3DScene)
        {
            if(octave3DScene != null)
            {
                MessageListenerDatabase listenerDatabase = MessageListenerDatabase.Instance;
                listenerDatabase.UnregisterListener(octave3DScene);

                listenerDatabase.RegisterListenerForMessage(MessageType.ToolWasReset, octave3DScene);
                listenerDatabase.RegisterListenerForMessage(MessageType.ToolWasSelected, octave3DScene);
            }
        }

        public static void PerformRegistrationForInspector(Inspector inspector)
        {
            if(inspector != null)
            {
                MessageListenerDatabase listenerDatabase = MessageListenerDatabase.Instance;
                listenerDatabase.UnregisterListener(inspector);

                listenerDatabase.RegisterListenerForMessage(MessageType.ToolWasReset, inspector);
                listenerDatabase.RegisterListenerForMessage(MessageType.ToolWasStarted, inspector);
            }
        }

        public static void PerformRegistrationForObjectPlacementGuide(ObjectPlacementGuide objectPlacementGuide)
        {
            if (objectPlacementGuide != null)
            {
                MessageListenerDatabase listenerDatabase = MessageListenerDatabase.Instance;
                listenerDatabase.UnregisterListener(objectPlacementGuide);

                listenerDatabase.RegisterListenerForMessage(MessageType.NewPrefabWasActivated, objectPlacementGuide);
                listenerDatabase.RegisterListenerForMessage(MessageType.NewPrefabCategoryWasActivated, objectPlacementGuide);
                listenerDatabase.RegisterListenerForMessage(MessageType.PrefabWasRemovedFromCategory, objectPlacementGuide);
                listenerDatabase.RegisterListenerForMessage(MessageType.PrefabCategoryWasRemovedFromDatabase, objectPlacementGuide);
                listenerDatabase.RegisterListenerForMessage(MessageType.UndoRedoWasPerformed, objectPlacementGuide);
            }
        }

        public static void PerformRegistrationForObjectPlacementModule(ObjectPlacement objectPlacement)
        {
            if (objectPlacement != null)
            {
                MessageListenerDatabase listenerDatabase = MessageListenerDatabase.Instance;
                listenerDatabase.UnregisterListener(objectPlacement);

                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementModeWasChanged, objectPlacement);
                listenerDatabase.RegisterListenerForMessage(MessageType.UndoRedoWasPerformed, objectPlacement);
                listenerDatabase.RegisterListenerForMessage(MessageType.InspectorGUIWasChanged, objectPlacement);
                listenerDatabase.RegisterListenerForMessage(MessageType.PrefabWasRemovedFromCategory, objectPlacement);
                listenerDatabase.RegisterListenerForMessage(MessageType.PrefabCategoryWasRemovedFromDatabase, objectPlacement);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectHierarchyRootsWerePlacedInScene, objectPlacement);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementGuideWasInstantiated, objectPlacement);
                listenerDatabase.RegisterListenerForMessage(MessageType.ToolWasReset, objectPlacement);
            }
        }

        public static void PerformRegistrationForPrefabCategoryDatabase(PrefabCategoryDatabase prefabCategoryDatabase)
        {
            if (prefabCategoryDatabase != null)
            {
                MessageListenerDatabase listenerDatabase = MessageListenerDatabase.Instance;
                listenerDatabase.UnregisterListener(prefabCategoryDatabase);

                listenerDatabase.RegisterListenerForMessage(MessageType.PrefabTagWasRemovedFromDatabase, prefabCategoryDatabase);
            }
        }

        public static void PerformRegistrationForObjectPlacementPath(ObjectPlacementPath objectPlacementPath)
        {
            if (objectPlacementPath != null)
            {
                MessageListenerDatabase listenerDatabase = MessageListenerDatabase.Instance;
                listenerDatabase.UnregisterListener(objectPlacementPath);

                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementPathExcludeCornersWasChanged, objectPlacementPath);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementPathBorderSettingsWereChanged, objectPlacementPath);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementPathPaddingSettingsWereChanged, objectPlacementPath);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementPathRotateObjectsToFollowPathWasChanged, objectPlacementPath);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementPathHeightAdjustmentModeWasChanged, objectPlacementPath);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsWereChanged, objectPlacementPath);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChanged, objectPlacementPath);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementPathHeightPatternWasRemovedFromDatabase, objectPlacementPath);
                listenerDatabase.RegisterListenerForMessage(MessageType.NewObjectPlacementPathHeightPatternWasActivated, objectPlacementPath);
                listenerDatabase.RegisterListenerForMessage(MessageType.UndoRedoWasPerformed, objectPlacementPath);
            }
        }

        public static void PerformRegistrationForObjectPlacementBlock(ObjectPlacementBlock objectPlacementBlock)
        {
            if (objectPlacementBlock != null)
            {
                MessageListenerDatabase listenerDatabase = MessageListenerDatabase.Instance;
                listenerDatabase.UnregisterListener(objectPlacementBlock);

                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementBlockExcludeCornersWasChanged, objectPlacementBlock);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementBlockPaddingSettingsWereChanged, objectPlacementBlock);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChanged, objectPlacementBlock);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementBlockHeightAdjustmentModeWasChanged, objectPlacementBlock);
                listenerDatabase.RegisterListenerForMessage(MessageType.ObjectPlacementBlockSubdivisionSettingsWereChanged, objectPlacementBlock);
                listenerDatabase.RegisterListenerForMessage(MessageType.UndoRedoWasPerformed, objectPlacementBlock);
            }
        }
        #endregion
    }
}
#endif
