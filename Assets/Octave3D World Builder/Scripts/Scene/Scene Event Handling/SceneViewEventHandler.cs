#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public abstract class SceneViewEventHandler
    {
        #region Public Methods
        public void HandleSceneViewEvent(Event e)
        {
            if(e.IsUndoRedo())
            {
                SceneViewCamera.Instance.SetObjectVisibilityDirty();
                UndoRedoWasPerformedMessage.SendToInterestedListeners(e);

                return;
            }
 
            switch(e.type)
            {
                case EventType.Ignore:

                    MouseButtonStates.Instance.ClearStates();
                    break;

                case EventType.Repaint:

                    HandleRepaintEvent(e);
                    break;

                case EventType.MouseMove:

                    MouseCursor.Instance.HandleMouseMoveEvent(e);
                    HandleMouseMoveEvent(e);
                    break;

                case EventType.MouseDrag:

                    HandleMouseDragEvent(e);
                    break;

                case EventType.MouseDown:

                    // Always disable the left mouse button down event in order to avoid deselecting the Octave3D object.
                    MouseButtonStates.Instance.OnMouseButtonPressed((MouseButton)e.button);
                    if (!e.alt && e.type == EventType.MouseDown && e.button == (int)MouseButton.Left && MouseButtonStates.Instance.GetNumberOfPressedButtons() <= 1) e.DisableInSceneView();

                    HandleMouseButtonDownEvent(e);
                    break;

                case EventType.MouseUp:

                    MouseButtonStates.Instance.OnMouseButtonReleased((MouseButton)e.button);
                    HandleMouseButtonUpEvent(e);
                    break;

                case EventType.ScrollWheel:

                    HandleMouseScrollWheelEvent(e);
                    break;

                case EventType.KeyDown:

                    KeyboardButtonStates.Instance.OnKeyboardButtonPressed(e.keyCode);
                    if (HandleGeneralShortcutKeys(e)) return;

                    // Always disable the 'Delete' key in order to avoid deleting the Octave3D object from the scene.
                    if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
                    {
                        e.DisableInSceneView();
                    }

                    if (e.keyCode == KeyCode.F)
                    {
                        Octave3DWorldBuilder activeInstance = Octave3DWorldBuilder.ActiveInstance;
                        if (activeInstance != null)
                        {
                            List<GameObject> selectedObjects = new List<GameObject>(activeInstance.ObjectSelection.GetAllSelectedGameObjects());
                            Selection.objects = selectedObjects.ToArray();
                            if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.FrameSelected();

                            activeInstance.OnCamFocused();
                        }
                        break;
                    }

                    HandleKeyboardButtonDownEvent(e);
                    break;

                case EventType.KeyUp:

                    KeyboardButtonStates.Instance.OnKeyboardButtonReleased(e.keyCode);
                    HandleKeyboardButtonUpEvent(e);
                    break;

                case EventType.ExecuteCommand:

                    HandleExecuteCommandEvent(e);
                    break;
            }
        }
        #endregion

        #region Protected Methods
        protected virtual void HandleRepaintEvent(Event e)
        {

        }

        protected virtual void HandleMouseMoveEvent(Event e)
        {
        }

        protected virtual void HandleMouseDragEvent(Event e)
        {
        }

        protected virtual void HandleMouseScrollWheelEvent(Event e)
        {
        }

        protected virtual void HandleMouseButtonDownEvent(Event e)
        {
        }

        protected virtual void HandleMouseButtonUpEvent(Event e)
        {
        }

        protected virtual void HandleKeyboardButtonDownEvent(Event e)
        {
        }

        protected virtual void HandleKeyboardButtonUpEvent(Event e)
        {
        }

        protected virtual void HandleExecuteCommandEvent(Event e)
        {
        }

        protected virtual bool CanEventBeHandled(Event e)
        {
            int numberOfPressedMouseButtons = MouseButtonStates.Instance.GetNumberOfPressedButtons();

            // Note: Pressing some of the keys on the keyboard can result in Unity sending 2 events: one with the
            //       actual key code and one with a keycode set to 'None'. We will ignore the latter since it can
            //       lead to certain actions being executed 2 times instead of once (i.e. we need one notification
            //       for a keystroke, not 2).
            if ((e.type == EventType.KeyDown || e.type == EventType.KeyUp) && e.keyCode == KeyCode.None) return false;

            // When the mouse is dragged while holding down the right mouse button, we will not process the event
            // because this is a Unity Editor action which allows the user to look around the scene.
            if (e.type == EventType.MouseDrag && e.InvolvesRightMouseButton()) return false;
        
            if (e.control && e.type == EventType.KeyDown && e.keyCode == KeyCode.S) return false;   // Allow CTRL + S for saving the scene
            if (e.control && e.type == EventType.KeyDown && e.keyCode == KeyCode.D) return false;   // Allow CTRL + D for object duplication
            if (e.alt && e.type == EventType.KeyDown && e.keyCode == KeyCode.D) return false;       // Allow menu item ALT + D
            if (e.alt && e.type == EventType.KeyDown && e.keyCode == KeyCode.R) return false;       // Allow menu item ALT + S
            if (e.alt && numberOfPressedMouseButtons != 0) return false;                            // Allow the user to orbit the camera

            // We have to allow scene navigation. So, when at least one mouse button is pressed, and either of
            // the W, Q, E, A, S, and D keys are held down, we will return false.
            if (e.type == EventType.KeyDown && numberOfPressedMouseButtons != 0)
            {
                if (e.keyCode == KeyCode.W || e.keyCode == KeyCode.Q ||
                    e.keyCode == KeyCode.E || e.keyCode == KeyCode.A ||
                    e.keyCode == KeyCode.S || e.keyCode == KeyCode.D)
                {
                    return false;
                }
            }

            // Note: Input gets blocked if we press a key while another mouse button is also pressed. For this
            //       reason, we will not allow 'KeyDown' events to be processed if at least one mouse button is
            //       pressed.
            if (e.type == EventType.KeyDown && MouseButtonStates.Instance.GetNumberOfPressedButtons() != 0) return false;

            // Input can also block if we press more than one button at a time. So mouse button presses should only
            // be processed only when no other buttons are pressed.
            if (e.type == EventType.MouseDown && MouseButtonStates.Instance.GetNumberOfPressedButtons() > 1) return false;

            // If the ALT key is pressed, we will not process any mouse message. Same reason: it will block input.
            if (e.alt && e.IsMouseSpecific()) return false;

            if (e.type == EventType.KeyDown)
            {
                // Let the user navigate the scene using the arrow keys
                if (e.keyCode == KeyCode.UpArrow || e.keyCode == KeyCode.DownArrow || 
                    e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.RightArrow) return false;

                #if UNITY_2018_3_OR_NEWER
                #if UNITY_EDITOR_WIN
                if (e.keyCode == KeyCode.Z || e.keyCode == KeyCode.Y)
                {
                    if (e.control) return false;
                }
#elif UNITY_EDITOR_OSX
                if (e.keyCode == KeyCode.Z && e.command) return false;
                else if (e.keyCode == KeyCode.Z && e.command && e.shift) return false;
#endif
#endif

                // Toggle fullscreen mode?
                if (e.shift && e.keyCode == KeyCode.Space) return false;
            }

            return true;
        }
        #endregion

        #region Private Methods
        private bool HandleGeneralShortcutKeys(Event e)
        {
            if (!CanEventBeHandled(e)) return false;

            if(e.type == EventType.KeyDown)
            {
                if(AllShortcutCombos.Instance.ActivateObjectPlacementGUI.IsActive())
                {
                    e.DisableInSceneView();
                    UndoEx.RecordForToolAction(Octave3DWorldBuilder.ActiveInstance.Inspector);
                    Octave3DWorldBuilder.ActiveInstance.Inspector.ActiveInspectorGUIIdentifier = InspectorGUIIdentifier.ObjectPlacement;
                    return true;
                }
                else 
                if(AllShortcutCombos.Instance.ActivateObjectSelectionGUI.IsActive())
                {
                    e.DisableInSceneView();
                    UndoEx.RecordForToolAction(Octave3DWorldBuilder.ActiveInstance.Inspector);
                    Octave3DWorldBuilder.ActiveInstance.Inspector.ActiveInspectorGUIIdentifier = InspectorGUIIdentifier.ObjectSelection;
                    return true;
                }
                else
                if(AllShortcutCombos.Instance.ActivateObjectErasingGUI.IsActive())
                {
                    e.DisableInSceneView();
                    UndoEx.RecordForToolAction(Octave3DWorldBuilder.ActiveInstance.Inspector);
                    Octave3DWorldBuilder.ActiveInstance.Inspector.ActiveInspectorGUIIdentifier = InspectorGUIIdentifier.ObjectErase;
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
#endif