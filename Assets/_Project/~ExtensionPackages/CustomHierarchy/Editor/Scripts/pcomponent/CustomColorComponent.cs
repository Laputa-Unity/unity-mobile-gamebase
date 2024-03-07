using System;
using System.Collections.Generic;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using customtools.customhierarchy.phelper;
using UnityEngine;
using UnityEditor;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomColorComponent: CustomBaseComponent
    {
        // PRIVATE
        private Color inactiveColor;
        private Texture2D colorTexture;
        private Rect colorRect = new Rect();

        // CONSTRUCTOR
        public CustomColorComponent()
        {
            colorTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomColorButton);

            rect.width = 8;
            rect.height = 16;

            CustomSettings.getInstance().addEventListener(CustomSetting.ColorShow              , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ColorShowDuringPlayMode, settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalInactiveColor, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled                     = CustomSettings.getInstance().get<bool>(CustomSetting.ColorShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.ColorShowDuringPlayMode);
            inactiveColor               = CustomSettings.getInstance().getColor(CustomSetting.AdditionalInactiveColor);
        }

        // LAYOUT
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < 8)
            {
                return CustomLayoutStatus.Failed;
            }
            else
            {
                curRect.x -= 8;
                rect.x = curRect.x;
                rect.y = curRect.y;
                return CustomLayoutStatus.Success;
            }
        }

        // DRAW
        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            if (objectList != null)
            {
                Color newColor;
                if (objectList.gameObjectColor.TryGetValue(gameObject, out newColor))
                {
                    colorRect.Set(rect.x + 1, rect.y + 1, 5, rect.height - 1);
                    EditorGUI.DrawRect(colorRect, newColor);
                    return;
                }
            }

            CustomColorUtils.setColor(inactiveColor);
            GUI.DrawTexture(rect, colorTexture, ScaleMode.StretchToFill, true, 1);
            CustomColorUtils.clearColor();
        }

        // EVENTS
        public override void eventHandler(GameObject gameObject, CustomObjectList objectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown)
                {
                    try {
                        PopupWindow.Show(rect, new CustomColorPickerWindow(Selection.Contains(gameObject) ? Selection.gameObjects : new GameObject[] { gameObject }, colorSelectedHandler, colorRemovedHandler));
                    } 
                    catch {}
                }
                currentEvent.Use();
            }
        }

        // PRIVATE
        private void colorSelectedHandler(GameObject[] gameObjects, Color color)
        {
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjects[i];
                CustomObjectList objectList = CustomObjectListManager.getInstance().getObjectList(gameObjects[i], true);
                Undo.RecordObject(objectList, "Color Changed");
                if (objectList.gameObjectColor.ContainsKey(gameObject))
                {
                    objectList.gameObjectColor[gameObject] = color;
                }
                else
                {
                    objectList.gameObjectColor.Add(gameObject, color);
                }                
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        private void colorRemovedHandler(GameObject[] gameObjects)
        {
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjects[i];
                CustomObjectList objectList = CustomObjectListManager.getInstance().getObjectList(gameObjects[i], true);
                if (objectList.gameObjectColor.ContainsKey(gameObject))                
                {
                    Undo.RecordObject(objectList, "Color Changed");
                    objectList.gameObjectColor.Remove(gameObject);                          
                }
            }
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}

