#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class NamedEntityCollectionWithEntityMarker<EntityType> : NamedEntityCollection<EntityType>, IMessageListener where EntityType : class, INamedEntity
    {
        #region Protected Variables
        [SerializeField]
        protected int _indexOfMarkedEntity = -1;
        #endregion

        #region Public Properties
        public EntityType MarkedEntity { get { return _indexOfMarkedEntity >= 0 ? _namedEntities[_indexOfMarkedEntity] : null; } }
        public int IndexOfMarkedEntity { get { return _indexOfMarkedEntity; } }
        #endregion

        #region Constructors
        public NamedEntityCollectionWithEntityMarker()
        {
            MessageListenerRegistration.PerformRegistrationForNamedEntityCollectionWithMarker(this);
        }
        #endregion

        #region Public Methods
        public void MarkEntity(EntityType namedEntity)
        {
            _indexOfMarkedEntity = _namedEntities.FindIndex(item => ReferenceEquals(namedEntity, item));
        }

        public override void RemoveEntity(EntityType namedEntity)
        {
            int indexOfRemovedEntity = _namedEntities.FindIndex(item => ReferenceEquals(item, namedEntity));
            base.RemoveEntity(namedEntity);

            UpdateMarkedEntityAfterEntityRemoval(indexOfRemovedEntity);
        }

        public override void RemoveAllEntities()
        {
            base.RemoveAllEntities();
            _indexOfMarkedEntity = -1;
        }
        #endregion

        #region Message Handlers
        public void RespondToMessage(Message message)
        {
            switch(message.Type)
            {
                case MessageType.UndoRedoWasPerformed:

                    RespondToMessage(message as UndoRedoWasPerformedMessage);
                    break;
            }
        }

        private void RespondToMessage(UndoRedoWasPerformedMessage message)
        {
            if (_indexOfMarkedEntity >= NumberOfEntities) _indexOfMarkedEntity = NumberOfEntities - 1;
        }
        #endregion

        #region Private Methods
        private void UpdateMarkedEntityAfterEntityRemoval(int indexOfRemovedEntity)
        {
            if(indexOfRemovedEntity == _indexOfMarkedEntity)
            {
                int indexOfNewMarkedEntity = indexOfRemovedEntity;
                if (indexOfNewMarkedEntity >= _namedEntities.Count) --indexOfNewMarkedEntity;

                if (indexOfNewMarkedEntity >= 0) MarkEntity(_namedEntities[indexOfNewMarkedEntity]);
                else _indexOfMarkedEntity = -1;
            }
            else
            {
                if (_indexOfMarkedEntity >= _namedEntities.Count) --_indexOfMarkedEntity;
            }
        }
        #endregion
    }
}
#endif