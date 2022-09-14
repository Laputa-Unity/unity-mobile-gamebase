#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabConfigSave
    {
        #region Public Static Functions
        public static void SaveConfig(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            using (XmlTextWriter xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteNewLine(0);
                xmlWriter.WriteStartElement(PrefabConfigXMLInfo.RootNode);

                WritePrefabTagsDatabase(xmlWriter);
                WritePrefabCategoryDatabase(xmlWriter);
                WritePrefabScrollViewLookAndFeel(xmlWriter);

                xmlWriter.WriteNewLine(0);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }
        #endregion

        #region Private Static Functions
        private static void WritePrefabTagsDatabase(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteNewLine(1);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabTagDatabaseNode);

            List<PrefabTag> allPrefabTags = PrefabTagDatabase.Get().GetAllPrefabTags();
            for (int tagIndex = 0; tagIndex < allPrefabTags.Count; ++tagIndex )
            {
                EditorUtility.DisplayProgressBar("Saving prefab tags...", "", (tagIndex + 1) / (float)allPrefabTags.Count);
                WritePrefabTag(xmlWriter, allPrefabTags[tagIndex]);
            }
            EditorUtility.ClearProgressBar();

            xmlWriter.WriteNewLine(1);
            xmlWriter.WriteEndElement();
        }

        private static void WritePrefabTag(XmlTextWriter xmlWriter, PrefabTag prefabTag)
        {
            xmlWriter.WriteNewLine(2);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabTagNode);

            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabTagNameNode);
            xmlWriter.WriteString(prefabTag.Name);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabTagActiveNode);
            xmlWriter.WriteString(prefabTag.IsActive.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(2);
            xmlWriter.WriteEndElement();
        }

        private static void WritePrefabScrollViewLookAndFeel(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteNewLine(2);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabScrollViewLookAndFeelNode);

            PrefabCategoryPrefabScrollViewData lookAndFeelData = Octave3DWorldBuilder.ActiveInstance.EditorWindowPool.ActivePrefabCategoryView.PrefabScrollView.ViewData;

            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.NumPrefabsPerRowNode);
            xmlWriter.WriteString(lookAndFeelData.NumberOfPrefabsPerRow.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabPreviewScaleNode);
            xmlWriter.WriteString(lookAndFeelData.PrefabPreviewScale.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabScrollViewHeightNode);
            xmlWriter.WriteString(lookAndFeelData.PrefabScrollViewHeight.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.ActivePrefabTintNode);
            xmlWriter.WriteColorString(lookAndFeelData.ActivePrefabTint);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.ShowPrefabNamesNode);
            xmlWriter.WriteString(lookAndFeelData.ShowPrefabNames.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabNameLabelColorNode);
            xmlWriter.WriteColorString(lookAndFeelData.PrefabNameLabelColor);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(2);
            xmlWriter.WriteEndElement();
        }

        private static void WritePrefabCategoryDatabase(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteNewLine(1);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabCategoryDatabaseNode);

            List<PrefabCategory> allPrefabCategories = PrefabCategoryDatabase.Get().GetAllPrefabCategories();
            for (int categoryIndex = 0; categoryIndex < allPrefabCategories.Count; ++categoryIndex )
            {
                EditorUtility.DisplayProgressBar("Saving prefab categories...", "", (categoryIndex + 1) / (float)allPrefabCategories.Count);
                WritePrefabCategory(xmlWriter, allPrefabCategories[categoryIndex]);
            }
            EditorUtility.ClearProgressBar();

            xmlWriter.WriteNewLine(1);
            xmlWriter.WriteEndElement();
        }

        private static void WritePrefabCategory(XmlTextWriter xmlWriter, PrefabCategory prefabCategory)
        {
            xmlWriter.WriteNewLine(2);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabCategoryNode);
            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabCategoryNameNode);
            xmlWriter.WriteString(prefabCategory.Name);
            xmlWriter.WriteEndElement();

            if(prefabCategory.ObjectGroup != null && prefabCategory.ObjectGroup.GroupObject != null)
            {
                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabCategoryAssociatedObjectGroupNameNode);
                xmlWriter.WriteString(prefabCategory.ObjectGroup.GroupObject.name);
                xmlWriter.WriteEndElement();
            }

            List<string> pathFolderNames = prefabCategory.PathFolderNames;
            if (pathFolderNames != null && pathFolderNames.Count != 0)
            {
                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabCategoryFolderNamesParentNode);
                for (int folderNameIndex = 0; folderNameIndex < pathFolderNames.Count; ++folderNameIndex)
                {
                    string folderName = pathFolderNames[folderNameIndex];
                    xmlWriter.WriteNewLine(4);
                    xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabCategoryFolderNameNode + folderNameIndex.ToString());
                    xmlWriter.WriteString(folderName);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteNewLine(3);
                xmlWriter.WriteEndElement();
            }

            List<Prefab> allPrefabs = prefabCategory.GetAllPrefabs();
            foreach (var prefab in allPrefabs)
            {
                WritePrefab(xmlWriter, prefab);
            }

            xmlWriter.WriteNewLine(2);
            xmlWriter.WriteEndElement();
        }

        private static void WritePrefab(XmlTextWriter xmlWriter, Prefab prefab)
        {
            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabCategoryPrefabNode);
            xmlWriter.WriteNewLine(4);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabNameNode);
            xmlWriter.WriteString(prefab.Name);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(4);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabPathNode);
            xmlWriter.WriteString(AssetDatabase.GetAssetPath(prefab.UnityPrefab));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(4);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabOffsetFromGridSurfaceNode);
            xmlWriter.WriteString(prefab.OffsetFromGridSurface.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteNewLine(4);
            xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabOffsetFromObjectSurfaceNode);
            xmlWriter.WriteString(prefab.OffsetFromObjectSurface.ToString());
            xmlWriter.WriteEndElement();

            PrefabTagAssociations tagAssociations = prefab.TagAssociations;
            List<string> allAssociatedTagNames = tagAssociations.GetAllAssociatedTagNames();
            foreach(var tagName in allAssociatedTagNames)
            {
                xmlWriter.WriteNewLine(4);
                xmlWriter.WriteStartElement(PrefabConfigXMLInfo.PrefabAssociatedTagNode);
                xmlWriter.WriteString(tagName);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteNewLine(3);
            xmlWriter.WriteEndElement();
        }
        #endregion
    }
}
#endif