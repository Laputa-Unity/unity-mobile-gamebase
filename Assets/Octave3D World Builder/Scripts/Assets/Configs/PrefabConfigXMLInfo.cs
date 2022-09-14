#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class PrefabConfigXMLInfo
    {
        #region Public Static Properties
        public static string RootNode { get { return "PrefabConfig"; } }
        public static string PrefabTagDatabaseNode { get { return "PrefabTags"; } }
        public static string PrefabTagNode { get { return "Tag"; } }
        public static string PrefabTagNameNode { get { return "Name"; } }
        public static string PrefabTagActiveNode { get { return "Active"; } }
        public static string PrefabCategoryDatabaseNode { get { return "PrefabCategories"; } }
        public static string PrefabCategoryFolderNamesParentNode { get { return "FolderNames"; } }
        public static string PrefabCategoryFolderNameNode { get { return "Name"; } }
        public static string PrefabCategoryNode { get { return "Category"; } }
        public static string PrefabCategoryNameNode { get { return "Name"; } }
        public static string PrefabCategoryAssociatedObjectGroupNameNode { get { return "ObjectGroupName"; } }
        public static string PrefabCategoryPrefabNode { get { return "Prefab"; } }
        public static string PrefabNameNode { get { return "Name"; } }
        public static string PrefabPathNode { get { return "Path"; } }
        public static string PrefabAssociatedTagNode { get { return "Tag"; } }
        public static string PrefabOffsetFromGridSurfaceNode { get { return "Offset_From_Grid_Surface"; } }
        public static string PrefabOffsetFromObjectSurfaceNode { get { return "Offset_From_Object_Surface"; } }

        public static string PrefabScrollViewLookAndFeelNode { get { return "PrefabScrollViewLookAndFeel"; } }
        public static string NumPrefabsPerRowNode { get { return "NumPrefabsPerRow"; } }
        public static string PrefabPreviewScaleNode { get { return "PrefabPreviewScale"; } }
        public static string PrefabScrollViewHeightNode { get { return "PrefabScrollViewHeight"; ; } }
        public static string ActivePrefabTintNode { get { return "ActivePrefabTint"; } }
        public static string ShowPrefabNamesNode { get { return "ShowPrefabNames"; } }
        public static string PrefabNameLabelColorNode { get { return "PrefabNameLabelColor"; } }
        #endregion
    }
}
#endif