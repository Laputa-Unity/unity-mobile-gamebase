#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementGuideWasInstantiatedMessage : Message
    {
        #region Constructors
        public ObjectPlacementGuideWasInstantiatedMessage()
            : base(MessageType.ObjectPlacementGuideWasInstantiated)
        {
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners()
        {
            var message = new ObjectPlacementGuideWasInstantiatedMessage();
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementModeWasChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementMode _newObjectPlacementMode;
        #endregion

        #region Public Properties
        public ObjectPlacementMode NewObjectPlacementMode { get { return _newObjectPlacementMode; } }
        #endregion

        #region Constructors
        public ObjectPlacementModeWasChangedMessage(ObjectPlacementMode newObjectPlacementMode)
            : base(MessageType.ObjectPlacementModeWasChanged)
        {
            _newObjectPlacementMode = newObjectPlacementMode;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementMode newObjectPlacementMode)
        {
            var message = new ObjectPlacementModeWasChangedMessage(newObjectPlacementMode);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectHierarchyRootsWerePlacedInSceneMessage : Message
    {
        #region Public Enums
        public enum PlacementType
        {
            ObjectPlacement = 0,
            Selection
        }
        #endregion

        #region Private Variables
        private List<GameObject> _placedRoots;
        private PlacementType _objectPlacementType;
        #endregion

        #region Public Properties
        public List<GameObject> PlacedRoots { get { return new List<GameObject>(_placedRoots); } }
        public PlacementType ObjectPlacementType { get { return _objectPlacementType; } }
        #endregion

        #region Constructors
        public ObjectHierarchyRootsWerePlacedInSceneMessage(List<GameObject> placedRoots, PlacementType objectPlacementType)
            : base(MessageType.ObjectHierarchyRootsWerePlacedInScene)
        {
            _placedRoots = new List<GameObject>(placedRoots);
            _objectPlacementType = objectPlacementType;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(List<GameObject> placedRoots, PlacementType objectPlacementType)
        {
            var message = new ObjectHierarchyRootsWerePlacedInSceneMessage(placedRoots, objectPlacementType);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }

        public static void SendToInterestedListeners(GameObject placedRoot, PlacementType objectPlacementType)
        {
            var message = new ObjectHierarchyRootsWerePlacedInSceneMessage(new List<GameObject> { placedRoot }, objectPlacementType);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif
