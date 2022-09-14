#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabConfigLoad
    {
        #region Public Static Functions
        public static void LoadConfig(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            UndoEx.RecordForToolAction(PrefabTagDatabase.Get());
            PrefabTagDatabase.Get().RemoveAndDestroyAllPrefabTags();
            XmlNodeList prefabTagsDatabaseNodes = xmlDoc.SelectNodes("//" + PrefabConfigXMLInfo.PrefabTagDatabaseNode);
            if (prefabTagsDatabaseNodes.Count != 0) ReadAllPrefabTags(prefabTagsDatabaseNodes[0]);

            UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
            PrefabCategoryDatabase.Get().RemoveAndDestroyAllPrefabCategories(true);
            XmlNodeList prefabCategoryDatabaseNodes = xmlDoc.SelectNodes("//" + PrefabConfigXMLInfo.PrefabCategoryDatabaseNode);
            if (prefabCategoryDatabaseNodes.Count != 0) ReadAllPrefabCategories(prefabCategoryDatabaseNodes[0]);
          
            PrefabPreviewTextureCache.Get().GeneratePreviewForAllPrefabCategories(true);

            PrefabCategory firstNonEmptyCategory = PrefabCategoryDatabase.Get().GetFirstNonEmptyCategory();
            if (firstNonEmptyCategory != null) PrefabCategoryDatabase.Get().SetActivePrefabCategory(firstNonEmptyCategory);

            XmlNodeList prefabScrollViewLookAndFeelNodes = xmlDoc.SelectNodes("//" + PrefabConfigXMLInfo.PrefabScrollViewLookAndFeelNode);
            if (prefabScrollViewLookAndFeelNodes.Count != 0) ReadPrefabScrollViewLookAndFeel(prefabScrollViewLookAndFeelNodes[0]);
        }
        #endregion

        #region Private Static Functions
        private static void ReadPrefabScrollViewLookAndFeel(XmlNode prefabScrollViewLookAndFeelNode)
        {
            PrefabCategoryPrefabScrollViewData lookAndFeelData = Octave3DWorldBuilder.ActiveInstance.EditorWindowPool.ActivePrefabCategoryView.PrefabScrollView.ViewData;
            UndoEx.RecordForToolAction(lookAndFeelData);

            XmlNode node = prefabScrollViewLookAndFeelNode.SelectSingleNode(PrefabConfigXMLInfo.NumPrefabsPerRowNode);
            if(node != null)
            {
                try
                {
                    lookAndFeelData.NumberOfPrefabsPerRow = Int32.Parse(node.InnerText);
                }
                catch (Exception) { }
            }

            node = prefabScrollViewLookAndFeelNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabPreviewScaleNode);
            if (node != null)
            {
                try
                {
                    lookAndFeelData.PrefabPreviewScale = float.Parse(node.InnerText);
                }
                catch (Exception) { }
            }

            node = prefabScrollViewLookAndFeelNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabScrollViewHeightNode);
            if (node != null)
            {
                try
                {
                    lookAndFeelData.PrefabScrollViewHeight = float.Parse(node.InnerText);
                }
                catch (Exception) { }
            }

            node = prefabScrollViewLookAndFeelNode.SelectSingleNode(PrefabConfigXMLInfo.ActivePrefabTintNode);
            if (node != null)
            {
                try
                {
                    lookAndFeelData.ActivePrefabTint = ColorExtensions.FromString(node.InnerText);
                }
                catch (Exception) { }
            }

            node = prefabScrollViewLookAndFeelNode.SelectSingleNode(PrefabConfigXMLInfo.ShowPrefabNamesNode);
            if (node != null)
            {
                try
                {
                    lookAndFeelData.ShowPrefabNames = bool.Parse(node.InnerText);
                }
                catch (Exception) { }
            }

            node = prefabScrollViewLookAndFeelNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabNameLabelColorNode);
            if (node != null)
            {
                try
                {
                    lookAndFeelData.PrefabNameLabelColor = ColorExtensions.FromString(node.InnerText);
                }
                catch (Exception) { }
            }
        }

        private static void ReadAllPrefabTags(XmlNode prefabTagsDatabaseNode)
        {
            XmlNodeList prefabTagNodes = prefabTagsDatabaseNode.ChildNodes;
            if (prefabTagNodes.Count == 0) return;

            for(int tagNodeIndex = 0; tagNodeIndex < prefabTagNodes.Count; ++tagNodeIndex)
            {
                EditorUtility.DisplayProgressBar("Loading prefab tags...", "", (tagNodeIndex + 1) / (float)prefabTagNodes.Count);

                XmlNode tagNode = prefabTagNodes[tagNodeIndex];
                XmlNode tagNameNode = tagNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabTagNameNode);
                if (tagNameNode == null) continue;
                XmlNode tagIsActiveNode = tagNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabTagActiveNode);

                string prefabTagName = tagNameNode.InnerText;
                if (string.IsNullOrEmpty(prefabTagName)) continue;

                bool isTagActive = true;
                try
                {
                    if (tagIsActiveNode != null) isTagActive = bool.Parse(tagIsActiveNode.InnerText);
                }
                catch (Exception) { }

                var prefabTag = PrefabTagDatabase.Get().CreatePrefabTag(prefabTagName);
                if (prefabTag == null) continue;

                prefabTag.IsActive = isTagActive;
            }
            EditorUtility.ClearProgressBar();
        }

        private static void ReadAllPrefabCategories(XmlNode prefabCategoryDatabaseNodes)
        {
            XmlNodeList prefabCategoryNodes = prefabCategoryDatabaseNodes.ChildNodes;
            if (prefabCategoryNodes.Count == 0) return;

            for (int categoryNodeIndex = 0; categoryNodeIndex < prefabCategoryNodes.Count; ++categoryNodeIndex)
            {
                EditorUtility.DisplayProgressBar("Loading prefab categories...", "", (categoryNodeIndex + 1) / (float)prefabCategoryNodes.Count);

                XmlNode categoryNode = prefabCategoryNodes[categoryNodeIndex];
                XmlNode categoryNameNode = categoryNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabCategoryNameNode);
                if (categoryNameNode == null) continue;
                XmlNode objGroupNameNode = categoryNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabCategoryAssociatedObjectGroupNameNode);
                XmlNode folderNamesParentNode = categoryNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabCategoryFolderNamesParentNode);

                string categoryName = categoryNameNode.InnerText;
                if (string.IsNullOrEmpty(categoryName)) continue;

                PrefabCategory prefabCategory = null;
                PrefabCategory defaultCategory = PrefabCategoryDatabase.Get().GetDefaultPrefabCategory();
                if (categoryName != defaultCategory.Name)
                {
                    prefabCategory = PrefabCategoryDatabase.Get().CreatePrefabCategory(categoryName);
                    if (prefabCategory == null) continue;
                }
                else prefabCategory = defaultCategory;

                UndoEx.RecordForToolAction(prefabCategory);
                if(prefabCategory == defaultCategory) prefabCategory.RemoveAndDestroyAllPrefabs();

                ObjectGroupDatabase groupDatabase = Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase;

                // Check if the prefab category has an object group associated. If it does, attempt to either 
                // load it from the database or create a new one if it doesn't exist.
                if(objGroupNameNode != null && !string.IsNullOrEmpty(objGroupNameNode.InnerText))
                {
                    ObjectGroup objectGroup = groupDatabase.GetObjectGroupByName(objGroupNameNode.InnerText);
                    if (objectGroup != null) prefabCategory.SetObjectGroup(objectGroup);
                    else
                    {
                        ObjectGroup newGroup = groupDatabase.CreateObjectGroup(objGroupNameNode.InnerText);
                        if (newGroup != null) prefabCategory.SetObjectGroup(newGroup);
                    }
                }

                if (folderNamesParentNode != null)
                {
                    var allChildren = folderNamesParentNode.ChildNodes;
                    List<string> allFolderNames = new List<string>();
                    foreach (XmlNode child in allChildren) allFolderNames.Add(child.InnerText);
                    prefabCategory.SetPathFolderNames(allFolderNames);
                }

                ReadAllPrefabsInCategory(prefabCategory, categoryNode.SelectNodes(PrefabConfigXMLInfo.PrefabCategoryPrefabNode));
            }
            EditorUtility.ClearProgressBar();
        }

        private static void ReadAllPrefabsInCategory(PrefabCategory prefabCategory, XmlNodeList prefabNodes)
        {
            if (prefabNodes.Count == 0) return;

            for(int prefabNodeIndex = 0; prefabNodeIndex < prefabNodes.Count; ++prefabNodeIndex)
            {
                XmlNode prefabNode = prefabNodes[prefabNodeIndex];
                XmlNode prefabNameNode = prefabNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabNameNode);
                XmlNode prefabPathNode = prefabNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabPathNode);
                if (prefabPathNode == null) continue;
                XmlNode offsetFromGridNode = prefabNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabOffsetFromGridSurfaceNode);
                XmlNode offsetFromObjectNode = prefabNode.SelectSingleNode(PrefabConfigXMLInfo.PrefabOffsetFromObjectSurfaceNode);
                
                GameObject unityPrefab = AssetDatabase.LoadAssetAtPath(prefabPathNode.InnerText, typeof(GameObject)) as GameObject;
                if (unityPrefab == null) continue;

                Prefab prefab = PrefabFactory.Create(unityPrefab);
                if (prefabNameNode != null && !string.IsNullOrEmpty(prefabNameNode.InnerText)) prefab.Name = prefabNameNode.InnerText;
                
                float offsetFromGrid = 0.0f;
                if(offsetFromGridNode != null)
                {
                    try { offsetFromGrid = float.Parse(offsetFromGridNode.InnerText); }
                    catch (Exception) { }
                }
                prefab.OffsetFromGridSurface = offsetFromGrid;

                float offsetFromObject = 0.0f;
                if (offsetFromObjectNode != null)
                {
                    try { offsetFromObject = float.Parse(offsetFromObjectNode.InnerText); }
                    catch (Exception) { }
                }
                prefab.OffsetFromObjectSurface = offsetFromObject;

                ReadPrefabTagAssociations(prefab, prefabNode.SelectNodes(PrefabConfigXMLInfo.PrefabAssociatedTagNode));
                prefabCategory.AddPrefab(prefab);
            }
        }

        private static void ReadPrefabTagAssociations(Prefab prefab, XmlNodeList associatedPrefabTagNodes)
        {
            if (associatedPrefabTagNodes.Count == 0) return;
            for(int tagIndex = 0; tagIndex < associatedPrefabTagNodes.Count; ++tagIndex)
            {
                XmlNode tagNode = associatedPrefabTagNodes[tagIndex];
                if (!string.IsNullOrEmpty(tagNode.InnerText))
                {
                    prefab.TagAssociations.Associate(tagNode.InnerText);
                }
            }
        }
        #endregion
    }
}
#endif