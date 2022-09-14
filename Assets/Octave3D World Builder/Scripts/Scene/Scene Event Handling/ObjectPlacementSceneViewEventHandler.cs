#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementSceneViewEventHandler : SceneViewEventHandler
    {
        #region Protected Methods
        protected override void HandleRepaintEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleRepaintEvent(e);

            ObjectPlacement.Get().HandleRepaintEvent(e);
        }

        protected override void HandleMouseMoveEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseMoveEvent(e);

            ObjectPlacement.Get().HandleMouseMoveEvent(e);
        }

        protected override void HandleMouseDragEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseDragEvent(e);

            ObjectPlacement.Get().HandleMouseDragEvent(e);
        }

        protected override void HandleMouseButtonDownEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseButtonDownEvent(e);

            ObjectPlacement.Get().HandleMouseButtonDownEvent(e);
        }

        protected override void HandleMouseButtonUpEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseButtonUpEvent(e);

            ObjectPlacement.Get().HandleMouseButtonUpEvent(e);
        }

        protected override void HandleKeyboardButtonDownEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleKeyboardButtonDownEvent(e);

            ObjectPlacement.Get().HandleKeyboardButtonDownEvent(e);
        }

        protected override void HandleKeyboardButtonUpEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleKeyboardButtonUpEvent(e);

            ObjectPlacement.Get().HandleKeyboardButtonUpEvent(e);
        }

        protected override void HandleMouseScrollWheelEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseScrollWheelEvent(e);

            ObjectPlacement.Get().HandleMouseScrollWheelEvent(e);
        }
        #endregion
    }
}
#endif