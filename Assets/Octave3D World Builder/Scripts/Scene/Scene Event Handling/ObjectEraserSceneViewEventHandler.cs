#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectEraserSceneViewEventHandler : SceneViewEventHandler
    {
        #region Protected Methods
        protected override void HandleMouseButtonDownEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseButtonDownEvent(e);

            ObjectEraser.Get().HandleMouseButtonDownEvent(e);
        }

        protected override void HandleMouseButtonUpEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseButtonUpEvent(e);

            ObjectEraser.Get().HandleMouseButtonUpEvent(e);
        }

        protected override void HandleMouseDragEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseDragEvent(e);

            ObjectEraser.Get().HandleMouseDragEvent(e);
        }

        protected override void HandleMouseScrollWheelEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseScrollWheelEvent(e);

            ObjectEraser.Get().HandleMouseScrollWheelEvent(e);
        }

        protected override void HandleMouseMoveEvent(Event e)
        {
            if (!CanEventBeHandled(e)) return;
            base.HandleMouseMoveEvent(e);

            ObjectEraser.Get().HandleMouseMoveEvent(e);
        }
        #endregion
    }
}
#endif