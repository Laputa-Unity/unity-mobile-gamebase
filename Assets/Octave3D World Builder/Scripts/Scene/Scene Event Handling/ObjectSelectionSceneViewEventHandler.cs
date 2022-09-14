#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectSelectionSceneViewEventHandler : SceneViewEventHandler
    {
        #region Protected Methods
        protected override void HandleRepaintEvent(Event e)
        {
            if (!ObjectSelection.Get()) return;

            if (!CanEventBeHandled(e)) return;
            base.HandleRepaintEvent(e);

            ObjectSelection.Get().HandleRepaintEvent(e);
        }

        protected override void HandleMouseMoveEvent(Event e)
        {
            if (!ObjectSelection.Get()) return;

            if (!CanEventBeHandled(e)) return;
            base.HandleMouseMoveEvent(e);

            ObjectSelection.Get().HandleMouseMoveEvent(e);
        }

        protected override void HandleMouseDragEvent(Event e)
        {
            if (!ObjectSelection.Get()) return;

            if (!CanEventBeHandled(e)) return;
            base.HandleMouseDragEvent(e);

            ObjectSelection.Get().HandleMouseDragEvent(e);
        }

        protected override void HandleMouseButtonDownEvent(Event e)
        {
            if (!ObjectSelection.Get()) return;

            if (!CanEventBeHandled(e)) return;
            base.HandleMouseButtonDownEvent(e);

            ObjectSelection.Get().HandleMouseButtonDownEvent(e);
        }

        protected override void HandleMouseButtonUpEvent(Event e)
        {
            if (!ObjectSelection.Get()) return;

            if (!CanEventBeHandled(e)) return;
            base.HandleMouseButtonUpEvent(e);

            ObjectSelection.Get().HandleMouseButtonUpEvent(e);
        }

        protected override void HandleMouseScrollWheelEvent(Event e)
        {
            if (!ObjectSelection.Get()) return;

            if (!CanEventBeHandled(e)) return;
            base.HandleMouseScrollWheelEvent(e);

            ObjectSelection.Get().HandleMouseScrollWheelEvent(e);
        }

        protected override void HandleKeyboardButtonDownEvent(Event e)
        {
            if (!ObjectSelection.Get()) return;
       
            if (!CanEventBeHandled(e)) return;
            base.HandleKeyboardButtonDownEvent(e);

            ObjectSelection.Get().HandleKeyboardButtonDownEvent(e);
        }

        protected override void HandleKeyboardButtonUpEvent(Event e)
        {
            if (!ObjectSelection.Get()) return;

            if (!CanEventBeHandled(e)) return;
            base.HandleKeyboardButtonUpEvent(e);

            ObjectSelection.Get().HandleKeyboardButtonUpEvent(e);
        }

        protected override void HandleExecuteCommandEvent(Event e)
        {
            if (!ObjectSelection.Get()) return;

            if (!CanEventBeHandled(e)) return;
            base.HandleExecuteCommandEvent(e);

            ObjectSelection.Get().HandleExecuteCommandEvent(e);
        }
        #endregion
    }
}
#endif