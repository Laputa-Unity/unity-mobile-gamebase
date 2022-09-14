#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementPathHeightPatternWasRemovedFromDatabaseMessage : Message
    {
        #region Private Variables
        private ObjectPlacementPathHeightPattern _removedPattern;
        #endregion

        #region Public Properties
        public ObjectPlacementPathHeightPattern RemovedPattern { get { return _removedPattern; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathHeightPatternWasRemovedFromDatabaseMessage(ObjectPlacementPathHeightPattern removedPattern)
            : base(MessageType.ObjectPlacementPathHeightPatternWasRemovedFromDatabase)
        {
            _removedPattern = removedPattern;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementPathHeightPattern removedPattern)
        {
            var message = new ObjectPlacementPathHeightPatternWasRemovedFromDatabaseMessage(removedPattern);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class NewObjectPlacementPathHeightPatternWasActivatedMessage : Message
    {
         #region Private Variables
        private ObjectPlacementPathHeightPattern _newActivePattern;
        #endregion

        #region Public Properties
        public ObjectPlacementPathHeightPattern NewActivePattern { get { return _newActivePattern; } }
        #endregion

        #region Constructors
        public NewObjectPlacementPathHeightPatternWasActivatedMessage(ObjectPlacementPathHeightPattern newActivePattern)
            : base(MessageType.NewObjectPlacementPathHeightPatternWasActivated)
        {
            _newActivePattern = newActivePattern;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementPathHeightPattern newActivePattern)
        {
            var message = new NewObjectPlacementPathHeightPatternWasActivatedMessage(newActivePattern);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif