#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace O3DWB
{
    public abstract class DragAndDropEventHandler
    {
        #region Public Methods
        public void Handle(Event dragAndDropEvent, Rect dropAreaRectangle)
        {
            switch (dragAndDropEvent.type)
            {
                case EventType.DragUpdated:

                    // It seems that if we don't set the visual mode to 'DragAndDropVisualMode.Copy' 
                    // for a 'DragUpdated' event, the drag and drop operation doesn't work. 
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    break;

                case EventType.DragPerform:

                    // If the drop area contains the mouse cursor position and if the user is ready to 
                    // perform the drop, we perform the drop. We know that a user wants to perform a drop 
                    // when the event type is set to 'EventType.DragPerform'.
                    if (dropAreaRectangle.Contains(dragAndDropEvent.mousePosition) && 
                        dragAndDropEvent.type == EventType.DragPerform) PerformDrop();
                    break;
            }
        }
        #endregion

        #region Protected Abstract Methods
        protected abstract void PerformDrop();
        #endregion
    }
}
#endif