#if UNITY_EDITOR
namespace O3DWB
{
    public interface IMessageListener
    {
        #region Interface Methods
        void RespondToMessage(Message message);
        #endregion
    }
}
#endif