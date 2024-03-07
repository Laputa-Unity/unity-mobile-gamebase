using System;
using System.Reflection;
using System.Collections.Generic;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using UnityEngine;
using UnityEditor;
using customtools.customhierarchy.phelper;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomComponentsComponent: CustomBaseComponent
    {
        // PRIVATE
        private GUIStyle hintLabelStyle;
        private Color grayColor;
        private Color backgroundDarkColor;
        private Texture2D componentIcon;
        private List<Component> components = new List<Component>();   
        private Rect eventRect = new Rect(0, 0, 16, 16);
        private int componentsToDraw;
        private List<string> ignoreScripts;

        // CONSTRUCTOR
        public CustomComponentsComponent ()
        {
            this.backgroundDarkColor = CustomResources.getInstance().GetColor(CustomColor.BackgroundDark);
            this.grayColor           = CustomResources.getInstance().GetColor(CustomColor.Gray);
            this.componentIcon       = CustomResources.getInstance().GetTexture(CustomTexture.CustomComponentUnknownIcon);

            hintLabelStyle = new GUIStyle();
            hintLabelStyle.normal.textColor = grayColor;
            hintLabelStyle.fontSize = 11;
            hintLabelStyle.clipping = TextClipping.Clip;  

            CustomSettings.getInstance().addEventListener(CustomSetting.ComponentsShow              , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ComponentsShowDuringPlayMode, settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ComponentsIconSize          , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ComponentsIgnore            , settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled                     = CustomSettings.getInstance().get<bool>(CustomSetting.ComponentsShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.ComponentsShowDuringPlayMode);
            CustomHierarchySizeAll size = (CustomHierarchySizeAll)CustomSettings.getInstance().get<int>(CustomSetting.ComponentsIconSize);
            rect.width = rect.height = (size == CustomHierarchySizeAll.Normal ? 15 : (size == CustomHierarchySizeAll.Big ? 16 : 13));       

            string ignoreString = CustomSettings.getInstance().get<string>(CustomSetting.ComponentsIgnore);
            if (ignoreString != "") 
            {
                ignoreScripts = new List<string>(ignoreString.Split(new char[] { ',', ';', '.', ' ' }));
                ignoreScripts.RemoveAll(item => item == "");
            }
            else ignoreScripts = null;
        }

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            Component[] currentComponents = gameObject.GetComponents<Component>();

            components.Clear();
            if (ignoreScripts != null)
            {
                for (int i = 0; i < currentComponents.Length; i++)
                {
                    string componentName = currentComponents[i].GetType().FullName;
                    bool ignore = false;
                    for (int j = ignoreScripts.Count - 1; j >= 0; j--)
                    {
                        if (componentName.Contains(ignoreScripts[j]))
                        {
                            ignore = true;
                            break;
                        } 
                    }
                    if (!ignore) components.Add(currentComponents[i]);
                }
            }
            else
            {
                components.AddRange(currentComponents);
            }

            int maxComponentsCount = Mathf.FloorToInt((maxWidth - 2) / rect.width);
            componentsToDraw = Math.Min(maxComponentsCount, components.Count - 1);

            float totalWidth = 2 + rect.width * componentsToDraw;
    
            curRect.x -= totalWidth;

            rect.x = curRect.x;
            rect.y = curRect.y - (rect.height - 16) / 2;

            eventRect.width = totalWidth;
            eventRect.x = rect.x;
            eventRect.y = rect.y;

            if (maxComponentsCount >= components.Count - 1) return CustomLayoutStatus.Success;
            else if (maxComponentsCount == 0) return CustomLayoutStatus.Failed;
            else return CustomLayoutStatus.Partly;
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            for (int i = components.Count - componentsToDraw, n = components.Count; i < n; i++)
            {
                Component component = components[i];
                if (component is Transform) continue;
                                
                GUIContent content = EditorGUIUtility.ObjectContent(component, null);

                bool enabled = true;
                try
                {
                    PropertyInfo propertyInfo = component.GetType().GetProperty("enabled");
                    enabled = (bool)propertyInfo.GetGetMethod().Invoke(component, null);
                }
                catch {}

                Color color = GUI.color;
                color.a = enabled ? 1f : 0.3f;
                GUI.color = color;
                GUI.DrawTexture(rect, content.image == null ? componentIcon : content.image, ScaleMode.ScaleToFit);
                color.a = 1;
                GUI.color = color;

                if (rect.Contains(Event.current.mousePosition))
                {        
                    string componentName = "Missing script";
                    if (component != null) componentName = component.GetType().Name;

                    int labelWidth = Mathf.CeilToInt(hintLabelStyle.CalcSize(new GUIContent(componentName)).x);                    
                    selectionRect.x = rect.x - labelWidth / 2 - 4;
                    selectionRect.width = labelWidth + 8;
                    selectionRect.height -= 1;

                    if (selectionRect.y > 16) selectionRect.y -= 16;
                    else selectionRect.x += labelWidth / 2 + 18;

                    EditorGUI.DrawRect(selectionRect, backgroundDarkColor);
                    selectionRect.x += 4;
                    selectionRect.y += (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
                    selectionRect.height = EditorGUIUtility.singleLineHeight;

                    EditorGUI.LabelField(selectionRect, componentName, hintLabelStyle);
                }

                rect.x += rect.width;
            }
        }

        public override void eventHandler(GameObject gameObject, CustomObjectList objectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && eventRect.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown)
                {
                    int id = Mathf.FloorToInt((currentEvent.mousePosition.x - eventRect.x) / rect.width) + components.Count - 1 - componentsToDraw + 1;

                    try
                    {
                        PropertyInfo propertyInfo = components[id].GetType().GetProperty("enabled");
                        bool enabled = (bool)propertyInfo.GetGetMethod().Invoke(components[id], null);
                        Undo.RecordObject(components[id], enabled ? "Disable Component" : "Enable Component");
                        propertyInfo.GetSetMethod().Invoke(components[id], new object[] { !enabled });
                    }
                    catch {}

                    EditorUtility.SetDirty(gameObject);
                }
                currentEvent.Use();
            }
        }
    }
}

