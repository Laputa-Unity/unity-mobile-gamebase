#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionActionToolbar : Toolbar
    {
        #region Constructors
        public ObjectSelectionActionToolbar()
        {
            AllowActiveStateForButtons = false;
            UseBoxStyleForButtons = false;
            ButtonScale = 0.16f;
        }
        #endregion

        #region Protected Methods
        protected override int GetNumberOfButtons()
        {
            return 6;
        }

        protected override List<string> GetButtonTooltips()
        {
            return new List<string>
            {
                "Assign selection to decor paint mask",
                "Remove selection from decor paint mask",
                "Assign selection to snap mask",
                "Remove selection from snap mask",
                "Assign selection to erase mask",
                "Remove selection from erase mask"
            };
        }

        protected override List<string> GetNormalStateButtonTexturePaths()
        {
            return new List<string>
            { 
                "/Textures/GUI Textures/Object Selection Actions/DecorPaintMaskAssignment",
                "/Textures/GUI Textures/Object Selection Actions/DecorPaintMaskRemoval", 
                "/Textures/GUI Textures/Object Selection Actions/SnapMaskAssignment", 
                "/Textures/GUI Textures/Object Selection Actions/SnapMaskRemoval",
                "/Textures/GUI Textures/Object Selection Actions/EraseMaskAssignment",
                "/Textures/GUI Textures/Object Selection Actions/EraseMaskRemoval",
            };
        }

        protected override List<string> GetActiveStateButtonTexturePaths()
        {
            return new List<string>();
        }

        protected override Color GetButtonColor(int buttonIndex)
        {
            return Color.white;
        }

        protected override void HandleButtonClick(int buttonIndex)
        {   
            switch(buttonIndex)
            {
                case 0:

                    //UndoEx.RecordForToolAction(DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectCollectionMask);
                    DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectCollectionMask.Mask(ObjectSelection.Get().GetAllSelectedGameObjects());
                    break;

                case 1:

                    //UndoEx.RecordForToolAction(DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectCollectionMask);
                    DecorPaintObjectPlacement.Get().DecorPaintMask.ObjectCollectionMask.Unmask(ObjectSelection.Get().GetAllSelectedGameObjects());
                    break;

                case 2:

                    //UndoEx.RecordForToolAction(ObjectSnapping.Get().ObjectSnapMask.ObjectCollectionMask);
                    ObjectSnapping.Get().ObjectSnapMask.ObjectCollectionMask.Mask(ObjectSelection.Get().GetAllSelectedGameObjects());
                    break;

                case 3:

                    //UndoEx.RecordForToolAction(ObjectSnapping.Get().ObjectSnapMask.ObjectCollectionMask);
                    ObjectSnapping.Get().ObjectSnapMask.ObjectCollectionMask.Unmask(ObjectSelection.Get().GetAllSelectedGameObjects());
                    break;

                case 4:

                    ObjectEraser.Get().EraseMask.ObjectCollectionMask.Mask(ObjectSelection.Get().GetAllSelectedGameObjects());
                    break;

                case 5:

                    ObjectEraser.Get().EraseMask.ObjectCollectionMask.Unmask(ObjectSelection.Get().GetAllSelectedGameObjects());
                    break;
            }
        }

        protected override int GetActiveButtonIndex()
        {
            return -1;
        }
        #endregion
    }
}
#endif