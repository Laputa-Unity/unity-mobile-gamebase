#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementModeSelectionToolbar : Toolbar
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
                "Decor Paint",
                "Point and Click",
                "Path",
                "Block"
            };
        }

        protected override List<string> GetNormalStateButtonTexturePaths()
        {
            return new List<string>
            { 
                "/Textures/GUI Textures/Object Placement Mode Selection Buttons/DecorPaintMode",
                "/Textures/GUI Textures/Object Placement Mode Selection Buttons/PointAndClickMode", 
                "/Textures/GUI Textures/Object Placement Mode Selection Buttons/PathMode", 
                "/Textures/GUI Textures/Object Placement Mode Selection Buttons/BlockMode"
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
            UndoEx.RecordForToolAction(ObjectPlacementSettings.Get());
            ObjectPlacementSettings.Get().ObjectPlacementMode = (ObjectPlacementMode)buttonIndex;
            SceneView.RepaintAll();
        }

        protected override int GetActiveButtonIndex()
        {
            return (int)ObjectPlacementSettings.Get().ObjectPlacementMode;
        }
        #endregion
    }
}
#endif