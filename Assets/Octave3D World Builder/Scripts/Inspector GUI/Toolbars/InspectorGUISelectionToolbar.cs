#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class InspectorGUISelectionToolbar : Toolbar
    {
        #region Protected Methods
        protected override int GetNumberOfButtons()
        {
            return 4;
        }

        protected override List<string> GetButtonTooltips()
        {
            return new List<string>
            {
                "Object Placement",
                "Object Selection",
                "Object Erase",
                "Object Snapping",
            };
        }

        protected override List<string> GetNormalStateButtonTexturePaths()
        {
            return new List<string> 
            { 
                "/Textures/GUI Textures/Inspector GUI Selection Buttons/ObjectPlacementGUIButton", 
                "/Textures/GUI Textures/Inspector GUI Selection Buttons/ObjectSelectionGUIButton",
                "/Textures/GUI Textures/Inspector GUI Selection Buttons/ObjectEraseGUIButton",
                "/Textures/GUI Textures/Inspector GUI Selection Buttons/ObjectSnappingGUIButton",
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
            UndoEx.RecordForToolAction(Inspector.Get());
            Inspector.Get().ActiveInspectorGUIIdentifier = (InspectorGUIIdentifier)buttonIndex;
        }

        protected override int GetActiveButtonIndex()
        {
            return (int)Inspector.Get().ActiveInspectorGUIIdentifier;
        }
        #endregion
    }
}
#endif
