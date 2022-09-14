#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class EventExtensions
    {
        #region Extension Methods
        public static bool IsUndoRedo(this Event e)
        {
            return (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed");
        }

        public static bool IsDuplicateSelectionCommand(this Event e)
        {
            return e.type == EventType.ExecuteCommand && e.commandName == "Duplicate";
        }

        public static bool IsMouseSpecific(this Event e)
        {
            return e.type == EventType.MouseDown || e.type == EventType.MouseUp ||
                   e.type == EventType.MouseDrag || e.type == EventType.MouseMove;
        }

        public static bool InvolvesLeftMouseButton(this Event e)
        {
            return e.button == (int)MouseButton.Left;
        }

        public static bool InvolvesRightMouseButton(this Event e)
        {
            return e.button == (int)MouseButton.Right;
        }

        public static bool InvolvesMiddleMouseButton(this Event e)
        {
            return e.button == (int)MouseButton.Middle;
        }

        public static bool InvolvesMouseButton(this Event e, MouseButton mouseButton)
        {
            return e.button == (int)mouseButton;
        }

        public static Vector2 InvMousePos(this Event e, Camera camera)
        {
            return new Vector2(e.mousePosition.x, camera.pixelHeight - e.mousePosition.y);
        }

        public static void DisableInSceneView(this Event e)
        {
            GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
            e.Use();
            GUIUtility.hotControl = 0;
        }
        #endregion
    }
}
#endif