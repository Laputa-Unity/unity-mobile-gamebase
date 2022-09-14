#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class InspectorGUIWasChangedMessage : Message
    {
        #region Private Variables
        private InspectorGUIIdentifier _activeInspectorGUIIdentifier;
        #endregion

        #region Public Properties
        public InspectorGUIIdentifier ActiveInspectorGUIIdentifier { get { return _activeInspectorGUIIdentifier; } }
        #endregion

        #region Constructors
        public InspectorGUIWasChangedMessage(InspectorGUIIdentifier activeInspectorGUIIdentifier)
            : base(MessageType.InspectorGUIWasChanged)
        {
            _activeInspectorGUIIdentifier = activeInspectorGUIIdentifier;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(InspectorGUIIdentifier activeInspectorGUIIdentifier)
        {
            var message = new InspectorGUIWasChangedMessage(activeInspectorGUIIdentifier);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif

