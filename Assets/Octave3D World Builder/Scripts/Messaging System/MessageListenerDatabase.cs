#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class MessageListenerDatabase : Singleton<MessageListenerDatabase>
    {
        #region Private Variables
        private Dictionary<MessageType, HashSet<IMessageListener>> _messageTypeToMessageListeners = new Dictionary<MessageType, HashSet<IMessageListener>>();
        #endregion

        #region Public Methods
        public void Clear()
        {
            _messageTypeToMessageListeners.Clear();
        }

        public bool IsListenerRegistered(IMessageListener listener)
        {
            foreach (var pair in _messageTypeToMessageListeners)
            {
                if (IsListenerRegisteredForMessageType(listener, pair.Key)) return true;
            }

            return false;
        }

        public bool IsListenerRegisteredForMessageType(IMessageListener listener, MessageType messageType)
        {
            if (_messageTypeToMessageListeners.ContainsKey(messageType)) return _messageTypeToMessageListeners[messageType].Contains(listener);
            else return false;
        }

        public void UnregisterListener(IMessageListener listener)
        {
            foreach(var pair in _messageTypeToMessageListeners)
            {
                UnregisterListenerForMessageType(listener, pair.Key);
            }
        }

        public void UnregisterListenerForMessageType(IMessageListener listener, MessageType messageType)
        {
            if (IsListenerRegisteredForMessageType(listener, messageType))
            {
                _messageTypeToMessageListeners[messageType].Remove(listener);
            }
        }

        public bool IsEmpty()
        {
            return _messageTypeToMessageListeners.Count == 0;
        }

        public void SendMessageToInterestedListeners(Message message)
        {
            HashSet<IMessageListener> interestedListeners = null;
            if (TryGetListenersForMessage(message, out interestedListeners)) SendMessageToListeners(message, interestedListeners);
        }

        public void RegisterListenerForMessage(MessageType messageType, IMessageListener listener)
        {
            if (IsListenerRegisteredForMessageType(listener, messageType)) return;

            RegisterNewMessageTypeIfNecessary(messageType);
            _messageTypeToMessageListeners[messageType].Add(listener);
        }
        #endregion

        #region Private Methods
        private void RegisterNewMessageTypeIfNecessary(MessageType messageType)
        {
            if (!_messageTypeToMessageListeners.ContainsKey(messageType)) _messageTypeToMessageListeners.Add(messageType, new HashSet<IMessageListener>());
        }

        private bool TryGetListenersForMessage(Message message, out HashSet<IMessageListener> listeners)
        {
            listeners = null;
            if (_messageTypeToMessageListeners.ContainsKey(message.Type))
            {
                listeners = _messageTypeToMessageListeners[message.Type];
                return true;
            }

            return false;
        }

        private void SendMessageToListeners(Message message, HashSet<IMessageListener> listeners)
        {
            if (listeners == null || listeners.Count == 0 || Octave3DWorldBuilder.ActiveInstance == null) return;

            // Note: Operate on a copy of the listeners collection. This is needed in case one of
            //       the listsners unregisters itself (or other listeners) while responding to the
            //       message. In that case the hash set will be modified while executing the 'foreach' 
            //       loop.
            var listenersCopy = new HashSet<IMessageListener>(listeners);
            foreach (IMessageListener listener in listenersCopy)
            {
                if (listener != null) listener.RespondToMessage(message);
            }
        }
        #endregion
    }
}
#endif