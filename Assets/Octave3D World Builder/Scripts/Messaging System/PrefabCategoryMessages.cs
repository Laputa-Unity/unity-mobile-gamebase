#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class PrefabCategoryWasRemovedFromDatabaseMessage : Message
    {
        #region Private Variables
        private PrefabCategory _prefabCategoryWhichWasRemoved;
        #endregion

        #region Public Properties
        public PrefabCategory PrefabCategoryWhichWasRemoved { get { return _prefabCategoryWhichWasRemoved; } }
        #endregion

        #region Public Properties
        public PrefabCategoryWasRemovedFromDatabaseMessage(PrefabCategory prefabCategoryWhichWasRemoved)
            : base(MessageType.PrefabCategoryWasRemovedFromDatabase)
        {
            _prefabCategoryWhichWasRemoved = prefabCategoryWhichWasRemoved;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(PrefabCategory prefabCategoryWhichWasRemoved)
        {
            var message = new PrefabCategoryWasRemovedFromDatabaseMessage(prefabCategoryWhichWasRemoved);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class NewPrefabCategoryWasActivatedMessage : Message
    {
        #region Private Variables
        private PrefabCategory _newActivePrefabCategory;
        #endregion

        #region Public Properties
        public PrefabCategory NewActivePrefabCategory { get { return _newActivePrefabCategory; } }
        #endregion

        #region Constructors
        public NewPrefabCategoryWasActivatedMessage(PrefabCategory newActivePrefabCategory)
            : base(MessageType.NewPrefabCategoryWasActivated)
        {
            _newActivePrefabCategory = newActivePrefabCategory;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(PrefabCategory newActivePrefabCategory)
        {
            var message = new NewPrefabCategoryWasActivatedMessage(newActivePrefabCategory);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif