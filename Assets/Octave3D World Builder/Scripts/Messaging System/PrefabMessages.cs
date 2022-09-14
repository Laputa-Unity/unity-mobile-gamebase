#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class PrefabWasTransferredToCategoryMessage : Message
    {
        #region Private Variables
        private PrefabCategory _sourceCategory;
        private PrefabCategory _destinationCategory;
        private Prefab _prefab;
        #endregion

        #region Public Properties
        public PrefabCategory SourceCategory { get { return _sourceCategory; } }
        public PrefabCategory DestinationCategory { get { return _destinationCategory; } }
        public Prefab Prefab { get { return _prefab; } }
        #endregion

        #region Public Properties
        public PrefabWasTransferredToCategoryMessage(Prefab prefab, PrefabCategory sourceCategory, PrefabCategory destinationCategory)
            : base(MessageType.PrefabWasTransferredToCategory)
        {
            _sourceCategory = sourceCategory;
            _destinationCategory = destinationCategory;
            _prefab = prefab;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(Prefab prefab, PrefabCategory sourceCategory, PrefabCategory destinationCategory)
        {
            var message = new PrefabWasTransferredToCategoryMessage(prefab, sourceCategory, destinationCategory);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class PrefabWasRemovedFromCategoryMessage : Message
    {
        #region Private Variables
        private PrefabCategory _prefabCategory;
        private Prefab _prefabWhichWasRemoved;
        #endregion

        #region Public Properties
        public PrefabCategory PrefabCategory { get { return _prefabCategory; } }
        public Prefab PrefabWhichWasRemoved { get { return _prefabWhichWasRemoved; } }
        #endregion

        #region Public Properties
        public PrefabWasRemovedFromCategoryMessage(PrefabCategory prefabCategory, Prefab prefabWhichWasRemoved)
            : base(MessageType.PrefabWasRemovedFromCategory)
        {
            _prefabCategory = prefabCategory;
            _prefabWhichWasRemoved = prefabWhichWasRemoved;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(PrefabCategory prefabCategory, Prefab prefabWhichWasRemoved)
        {
            var message = new PrefabWasRemovedFromCategoryMessage(prefabCategory, prefabWhichWasRemoved);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class NewPrefabWasActivatedMessage : Message
    {
        #region Private Variables
        private Prefab _newActivePrefab;
        #endregion

        #region Public Properties
        public Prefab NewActivePrefab { get { return _newActivePrefab; } }
        #endregion

        #region Constructors
        public NewPrefabWasActivatedMessage(Prefab newActivePrefab)
            : base(MessageType.NewPrefabWasActivated)
        {
            _newActivePrefab = newActivePrefab;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(Prefab newActivePrefab)
        {
            var message = new NewPrefabWasActivatedMessage(newActivePrefab);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif
