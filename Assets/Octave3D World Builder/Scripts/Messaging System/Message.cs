#if UNITY_EDITOR
namespace O3DWB
{
    public abstract class Message
    {
        #region Private Variables
        private MessageType _type;
        #endregion

        #region Public Properties
        public MessageType Type { get { return _type; } }
        #endregion

        #region Constructors
        public Message(MessageType messageType)
        {
            _type = messageType;
        }
        #endregion
    }
}
#endif