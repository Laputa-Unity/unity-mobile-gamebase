#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionTransformGizmoSelectionToolbar : Toolbar
    {
        #region Constructors
        public ObjectSelectionTransformGizmoSelectionToolbar()
        {
            ButtonScale = 0.08f;
        }
        #endregion

        #region Protected Methods
        protected override int GetNumberOfButtons()
        {
            return 4;
        }

        protected override List<string> GetButtonTooltips()
        {
            return new List<string>
            {
                "Move Gizmo",
                "Rotation Gizmo",
                "Scale Gizmo",
                "Extrude Gizmo"
            };
        }

        protected override List<string> GetNormalStateButtonTexturePaths()
        {
            return new List<string>
            { 
                "/Textures/GUI Textures/Gizmo Activation Buttons/MoveGizmo", 
                "/Textures/GUI Textures/Gizmo Activation Buttons/RotationGizmo", 
                "/Textures/GUI Textures/Gizmo Activation Buttons/ScaleGizmo",
                "/Textures/GUI Textures/Gizmo Activation Buttons/ObjectExtrudeGizmo",
            };
        }

        protected override List<string> GetActiveStateButtonTexturePaths()
        {
            return new List<string>();
        }

        protected override Color GetButtonColor(int buttonIndex)
        {
            if (buttonIndex == GetActiveButtonIndex()) return new Color(1.0f, 1.0f, 1.0f, 0.5f);
            else return Color.white;
        }

        protected override void HandleButtonClick(int buttonIndex)
        {
            UndoEx.RecordForToolAction(ObjectSelection.Get().ObjectSelectionGizmos);
            ObjectSelection.Get().ObjectSelectionGizmos.ActiveGizmoType = (GizmoType)buttonIndex;
        }

        protected override int GetActiveButtonIndex()
        {
            return (int)ObjectSelection.Get().ObjectSelectionGizmos.ActiveGizmoType;
        }
        #endregion
    }
}
#endif