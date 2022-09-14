#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabManagementWindow : Octave3DEditorWindow, IMessageListener
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _scrollViewPosition = Vector2.zero;
        #endregion

        #region Constructors
        public PrefabManagementWindow()
        {
            MessageListenerRegistration.PerformRegistrationForPrefabManagementWindow(this);
        }
        #endregion

        #region Public Static Functions
        public static PrefabManagementWindow Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.PrefabManagementWindow;
        }
        #endregion

        #region Public Methods
        public override string GetTitle()
        {
            return "Prefab Management";
        }

        public override void ShowOctave3DWindow()
        {
            ShowDockable(true);
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            RenderContentInScrollView();
            EditorGUILayout.EndScrollView();

            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == (int)(MouseButton.Right))
            {
                GenericMenu contextMenu = new GenericMenu();
                contextMenu.AddItem(new GUIContent("Remove/Active category"), false, delegate()
                {
                    UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                    PrefabCategoryDatabase.Get().RemoveAndDestroyPrefabCategory(PrefabCategoryDatabase.Get().ActivePrefabCategory);
                });
                contextMenu.AddItem(new GUIContent("Remove/All categories"), false, delegate()
                {
                    UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                    PrefabCategoryDatabase.Get().RemoveAndDestroyAllPrefabCategories(true);
                });
                contextMenu.AddItem(new GUIContent("Remove/Empty categories"), false, delegate()
                {
                    UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                    PrefabCategoryDatabase.Get().RemoveAndDestroyEmptyPrefabCategories();
                });
                contextMenu.AddItem(new GUIContent("Clear/Active category"), false, delegate()
                {
                    UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get().ActivePrefabCategory);
                    PrefabCategoryDatabase.Get().ActivePrefabCategory.RemoveAndDestroyAllPrefabs();
                });
                contextMenu.AddItem(new GUIContent("Clear/All categories"), false, delegate()
                {
                    var allCategories = PrefabCategoryDatabase.Get().GetAllPrefabCategories();
                    foreach (var prefabCategory in allCategories)
                    {
                        UndoEx.RecordForToolAction(prefabCategory);
                        prefabCategory.RemoveAndDestroyAllPrefabs();
                    }
                });
                contextMenu.ShowAsContext();
            }
        }
        #endregion

        #region Message Listeners
        public void RespondToMessage(Message message)
        {
            switch(message.Type)
            {
                case MessageType.NewPrefabWasActivated:

                    RespondToMessage(message as NewPrefabWasActivatedMessage);
                    break;

                case MessageType.NewPrefabCategoryWasActivated:

                    RespondToMessage(message as NewPrefabCategoryWasActivatedMessage);
                    break;

                case MessageType.PrefabTagActiveStateWasChanged:

                    RespondToMessage(message as PrefabTagActiveStateWasChangedMessage);
                    break;

                case MessageType.PrefabTagWasCreatedInDatabase:

                    RespondToMessage(message as PrefabTagWasCreatedInDatabaseMessage);
                    break;

                case MessageType.PrefabTagWasRemovedFromDatabase:

                    RespondToMessage(message as PrefabTagWasRemovedFromDatabaseMessage);
                    break;
            }
        }

        private void RespondToMessage(NewPrefabWasActivatedMessage message)
        {
            Repaint();
        }

        private void RespondToMessage(NewPrefabCategoryWasActivatedMessage message)
        {
            Repaint();
        }

        private void RespondToMessage(PrefabTagActiveStateWasChangedMessage message)
        {
            Repaint();
        }

        private void RespondToMessage(PrefabTagWasCreatedInDatabaseMessage message)
        {
            Repaint();
        }

        private void RespondToMessage(PrefabTagWasRemovedFromDatabaseMessage message)
        {
            Repaint();
        }
        #endregion

        #region Private Methods
        private void RenderContentInScrollView()
        {
            Octave3DWorldBuilder.ActiveInstance.ActivePrefabCategoryView.Render();
            Octave3DWorldBuilder.ActiveInstance.ActivePrefabView.Render();
        }
        #endregion
    }
}
#endif