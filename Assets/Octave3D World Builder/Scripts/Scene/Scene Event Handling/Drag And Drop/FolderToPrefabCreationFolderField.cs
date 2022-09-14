#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace O3DWB
{
    public class FolderToPrefabCreationFolderField : DragAndDropEventHandler
    {
        #region Public Static Functions
        public static FolderToPrefabCreationFolderField Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.FolderToPrefabCreationFolderField;
        }
        #endregion

        #region Protected Methods
        protected override void PerformDrop()
        {
            string[] folderPaths = DragAndDrop.paths;
            if(folderPaths.Length > 0)
            {
                UndoEx.RecordForToolAction(ObjectSelection.Get().PrefabCreationSettings);
                ObjectSelection.Get().PrefabCreationSettings.DestinationFolder = folderPaths[0];
            }
        }
        #endregion
    }
}
#endif