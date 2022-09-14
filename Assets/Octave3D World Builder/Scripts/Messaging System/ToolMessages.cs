#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ToolWasResetMessage : Message
    {
        #region Constructors
        public ToolWasResetMessage()
            : base(MessageType.ToolWasReset)
        {
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners()
        {
            var message = new ToolWasResetMessage();
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ToolWasStartedMessage : Message
    {
        #region Constructors
        public ToolWasStartedMessage()
            : base(MessageType.ToolWasStarted)
        {
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners()
        {
            var message = new ToolWasStartedMessage();
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ToolWasSelectedMessage : Message
    {
        #region Constructors
        public ToolWasSelectedMessage()
            : base(MessageType.ToolWasSelected)
        {
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners()
        {
            var message = new ToolWasSelectedMessage();
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif

