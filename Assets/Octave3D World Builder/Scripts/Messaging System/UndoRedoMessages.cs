#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class UndoRedoWasPerformedMessage : Message
    {
        #region Private Variables
        private Event _undoRedoEvent;
        #endregion

        #region Public Properties
        public Event UndoRedoEvent { get { return _undoRedoEvent; } }
        #endregion

        #region Constructors
        public UndoRedoWasPerformedMessage(Event undoRedoEvent)
            : base(MessageType.UndoRedoWasPerformed)
        {
            _undoRedoEvent = undoRedoEvent;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(Event undoRedoEvent)
        {
            var message = new UndoRedoWasPerformedMessage(undoRedoEvent);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif

