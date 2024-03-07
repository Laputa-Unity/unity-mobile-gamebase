using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomTagLayerComponent: CustomBaseComponent
    {
        // PRIVATE
        private GUIStyle labelStyle;
        private CustomHierarchyTagAndLayerShowType showType;
        private Color layerColor;
        private Color tagColor;       
        private bool showAlways;
        private bool sizeIsPixel;
        private int pixelSize;
        private float percentSize;
        private GameObject[] gameObjects;
        private float labelAlpha;
        private CustomHierarchyTagAndLayerLabelSize labelSize;
        private Rect tagRect = new Rect();
        private Rect layerRect = new Rect();
        private bool needDrawTag;
        private bool needDrawLayer;
        private int layer;
        private string tag;

        // CONSTRUCTOR
        public CustomTagLayerComponent()
        {
            labelStyle = new GUIStyle();
            labelStyle.fontSize = 8;
            labelStyle.clipping = TextClipping.Clip;  
            labelStyle.alignment = TextAnchor.MiddleLeft;

            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerSizeShowType       , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerType               , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerSizeValueType      , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerSizeValuePixel     , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerSizeValuePercent   , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerLabelSize          , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerShow               , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerShowDuringPlayMode , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerTagLabelColor      , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerLayerLabelColor    , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerAligment           , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagAndLayerLabelAlpha         , settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            showAlways  = CustomSettings.getInstance().get<int>(CustomSetting.TagAndLayerType) == (int)CustomHierarchyTagAndLayerType.Always;
            showType    = (CustomHierarchyTagAndLayerShowType)CustomSettings.getInstance().get<int>(CustomSetting.TagAndLayerSizeShowType);
            sizeIsPixel = CustomSettings.getInstance().get<int>(CustomSetting.TagAndLayerSizeValueType) == (int)CustomHierarchyTagAndLayerSizeType.Pixel;
            pixelSize   = CustomSettings.getInstance().get<int>(CustomSetting.TagAndLayerSizeValuePixel);
            percentSize = CustomSettings.getInstance().get<float>(CustomSetting.TagAndLayerSizeValuePercent);
            labelSize   = (CustomHierarchyTagAndLayerLabelSize)CustomSettings.getInstance().get<int>(CustomSetting.TagAndLayerLabelSize);
            enabled     = CustomSettings.getInstance().get<bool>(CustomSetting.TagAndLayerShow);
            tagColor    = CustomSettings.getInstance().getColor(CustomSetting.TagAndLayerTagLabelColor);
            layerColor  = CustomSettings.getInstance().getColor(CustomSetting.TagAndLayerLayerLabelColor);
            labelAlpha  = CustomSettings.getInstance().get<float>(CustomSetting.TagAndLayerLabelAlpha);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.TagAndLayerShowDuringPlayMode);

            CustomHierarchyTagAndLayerAligment aligment = (CustomHierarchyTagAndLayerAligment)CustomSettings.getInstance().get<int>(CustomSetting.TagAndLayerAligment);
            switch (aligment)
            {
                case CustomHierarchyTagAndLayerAligment.Left  : labelStyle.alignment = TextAnchor.MiddleLeft;   break;
                case CustomHierarchyTagAndLayerAligment.Center: labelStyle.alignment = TextAnchor.MiddleCenter; break;
                case CustomHierarchyTagAndLayerAligment.Right : labelStyle.alignment = TextAnchor.MiddleRight;  break;
            }
        }

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            float textWidth = sizeIsPixel ? pixelSize : percentSize * rect.x;
            rect.width = textWidth + 4;

            if (maxWidth < rect.width)
            {
                return CustomLayoutStatus.Failed;
            }
            else
            {
                curRect.x -= rect.width + 2;
                rect.x = curRect.x;
                rect.y = curRect.y;
                rect.y += (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
                //rect.height = EditorGUIUtility.singleLineHeight;

                layer  = gameObject.layer; 
                tag = getTagName(gameObject);             
                
                needDrawTag   = (showType != CustomHierarchyTagAndLayerShowType.Layer) && ((showAlways || tag   != "Untagged"));
                needDrawLayer = (showType != CustomHierarchyTagAndLayerShowType.Tag  ) && ((showAlways || layer != 0         ));

                #if UNITY_2019_1_OR_NEWER
                    if (labelSize == CustomHierarchyTagAndLayerLabelSize.Big || (labelSize == CustomHierarchyTagAndLayerLabelSize.BigIfSpecifiedOnlyTagOrLayer && needDrawTag != needDrawLayer)) 
                        labelStyle.fontSize = 8;
                    else 
                        labelStyle.fontSize = 7;
                #else
                    if (labelSize == CustomHierarchyTagAndLayerLabelSize.Big || (labelSize == CustomHierarchyTagAndLayerLabelSize.BigIfSpecifiedOnlyTagOrLayer && needDrawTag != needDrawLayer)) 
                        labelStyle.fontSize = 9;
                    else 
                        labelStyle.fontSize = 8;
                #endif

                if (needDrawTag) tagRect.Set(rect.x, rect.y - (needDrawLayer ? 4 : 0), rect.width, rect.height);                                                   
                if (needDrawLayer) layerRect.Set(rect.x, rect.y + (needDrawTag ? 4 : 0), rect.width, rect.height);                    

                return CustomLayoutStatus.Success;
            }
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            if (needDrawTag ) 
            {
                tagColor.a = (tag == "Untagged" ? labelAlpha : 1.0f);
                labelStyle.normal.textColor = tagColor;
                EditorGUI.LabelField(tagRect, tag, labelStyle);
            }

            if (needDrawLayer) 
            {
                layerColor.a = (layer == 0 ? labelAlpha : 1.0f);
                labelStyle.normal.textColor = layerColor;
                EditorGUI.LabelField(layerRect, getLayerName(layer), labelStyle);
            }
        }

        public override void eventHandler(GameObject gameObject, CustomObjectList objectList, Event currentEvent)
        {                       
            if (Event.current.isMouse && currentEvent.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (needDrawTag && needDrawLayer)
                {
                    tagRect.height = 8;
                    layerRect.height = 8;
                    tagRect.y += 4;
                    layerRect.y += 4;
                }

                if (needDrawTag && tagRect.Contains(Event.current.mousePosition))
                {
                    gameObjects = Selection.Contains(gameObject) ? Selection.gameObjects : new GameObject[] { gameObject };
                    showTagsContextMenu(tag);
                    Event.current.Use();
                }
                else if (needDrawLayer && layerRect.Contains(Event.current.mousePosition))
                {
                    gameObjects = Selection.Contains(gameObject) ? Selection.gameObjects : new GameObject[] { gameObject };
                    showLayersContextMenu(LayerMask.LayerToName(layer));
                    Event.current.Use();
                }
            }
        }

        private string getTagName(GameObject gameObject)
        {
            string tag = "Undefined";
            try { tag = gameObject.tag; }
            catch {}
            return tag;
        }

        public string getLayerName(int layer)
        {
            string layerName = LayerMask.LayerToName(layer);
            if (layerName.Equals("")) layerName = "Undefined";
            return layerName;
        }

        // PRIVATE
        private void showTagsContextMenu(string tag)
        {
            List<string> tags = new List<string>(UnityEditorInternal.InternalEditorUtility.tags);
            
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Untagged"  ), false, tagChangedHandler, "Untagged");
            
            for (int i = 0, n = tags.Count; i < n; i++)
            {
                string curTag = tags[i];
                menu.AddItem(new GUIContent(curTag), tag == curTag, tagChangedHandler, curTag);
            }
            
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add Tag..."  ), false, addTagOrLayerHandler, "Tags");
            menu.ShowAsContext();
        }

        private void showLayersContextMenu(string layer)
        {
            List<string> layers = new List<string>(UnityEditorInternal.InternalEditorUtility.layers);
            
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Default"  ), false, layerChangedHandler, "Default");
            
            for (int i = 0, n = layers.Count; i < n; i++)
            {
                string curLayer = layers[i];
                menu.AddItem(new GUIContent(curLayer), layer == curLayer, layerChangedHandler, curLayer);
            }
            
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add Layer..."  ), false, addTagOrLayerHandler, "Layers");
            menu.ShowAsContext();
        }

        private void tagChangedHandler(object newTag)
        {
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjects[i];
                Undo.RecordObject(gameObject, "Change Tag");
                gameObject.tag = (string)newTag;
                EditorUtility.SetDirty(gameObject);
            }
        }

        private void layerChangedHandler(object newLayer)
        {
            int newLayerId = LayerMask.NameToLayer((string)newLayer);
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjects[i];
                Undo.RecordObject(gameObject, "Change Layer");
                gameObject.layer = newLayerId;
                EditorUtility.SetDirty(gameObject);
            }
        }

        private void addTagOrLayerHandler(object value)
        {
            PropertyInfo propertyInfo = typeof(EditorApplication).GetProperty("tagManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty);
            UnityEngine.Object obj = (UnityEngine.Object)(propertyInfo.GetValue(null, null));
            obj.GetType().GetField("m_DefaultExpandedFoldout").SetValue(obj, value);
            Selection.activeObject = obj;
        }
    }
}

