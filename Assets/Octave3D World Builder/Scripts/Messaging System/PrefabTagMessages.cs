#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class PrefabTagWasCreatedInDatabaseMessage : Message
    {
        #region Private Variables
        private PrefabTag _prefabTag;
        #endregion

        #region Public Properties
        public PrefabTag PrefabTag { get { return _prefabTag; } }
        #endregion

        #region Constructors
        public PrefabTagWasCreatedInDatabaseMessage(PrefabTag prefabTag)
            : base(MessageType.PrefabTagWasCreatedInDatabase)
        {
            _prefabTag = prefabTag;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(PrefabTag prefabTag)
        {
            var message = new PrefabTagWasCreatedInDatabaseMessage(prefabTag);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class PrefabTagWasRemovedFromDatabaseMessage : Message
    {
        #region Private Variables
        private PrefabTag _prefabTag;
        #endregion

        #region Public Properties
        public PrefabTag PrefabTag { get { return _prefabTag; } }
        #endregion

        #region Constructors
        public PrefabTagWasRemovedFromDatabaseMessage(PrefabTag prefabTag)
            : base(MessageType.PrefabTagWasRemovedFromDatabase)
        {
            _prefabTag = prefabTag;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(PrefabTag prefabTag)
        {
            var message = new PrefabTagWasRemovedFromDatabaseMessage(prefabTag);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class PrefabTagActiveStateWasChangedMessage : Message
    {
        #region Private Variables
        private PrefabTag _prefabTag;
        #endregion

        #region Public Properties
        public PrefabTag PrefabTag { get { return _prefabTag; } }
        #endregion

        #region Constructors
        public PrefabTagActiveStateWasChangedMessage(PrefabTag prefabTag)
            : base(MessageType.PrefabTagActiveStateWasChanged)
        {
            _prefabTag = prefabTag;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(PrefabTag prefabTag)
        {
            var message = new PrefabTagActiveStateWasChangedMessage(prefabTag);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif