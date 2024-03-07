using System;
using UnityEngine;
using UnityEditor;
using customtools.customhierarchy.phierarchy;
using customtools.customhierarchy.phelper;
using System.Reflection;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomGameObjectIconComponent: CustomBaseComponent
    {
        // PRIVATE
        private MethodInfo getIconMethodInfo;
        private object[] getIconMethodParams;

        // CONSTRUCTOR
        public CustomGameObjectIconComponent ()
        {
            rect.width = 14;
            rect.height = 14;

            getIconMethodInfo   = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.NonPublic | BindingFlags.Static );
            getIconMethodParams = new object[1];

            CustomSettings.getInstance().addEventListener(CustomSetting.GameObjectIconShow                 , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.GameObjectIconShowDuringPlayMode   , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.GameObjectIconSize                          , settingsChanged);
            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged()
        {
            enabled = CustomSettings.getInstance().get<bool>(CustomSetting.GameObjectIconShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.GameObjectIconShowDuringPlayMode);
            CustomHierarchySizeAll size = (CustomHierarchySizeAll)CustomSettings.getInstance().get<int>(CustomSetting.GameObjectIconSize);
            rect.width = rect.height = (size == CustomHierarchySizeAll.Normal ? 15 : (size == CustomHierarchySizeAll.Big ? 16 : 13));     
        }

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < rect.width + 2)
            {
                return CustomLayoutStatus.Failed;
            }
            else
            {
                curRect.x -= rect.width + 2;
                rect.x = curRect.x;
                rect.y = curRect.y - (rect.height - 16) / 2;
                return CustomLayoutStatus.Success;
            }
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {                      
            getIconMethodParams[0] = gameObject;
            Texture2D icon = (Texture2D)getIconMethodInfo.Invoke(null, getIconMethodParams );    
            if (icon != null) 
                GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, true);
        }
                
        public override void eventHandler(GameObject gameObject, CustomObjectList objectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                currentEvent.Use();

                Type iconSelectorType = Assembly.Load("UnityEditor").GetType("UnityEditor.IconSelector");
                MethodInfo showIconSelectorMethodInfo = iconSelectorType.GetMethod("ShowAtPosition", BindingFlags.Static | BindingFlags.NonPublic);
                showIconSelectorMethodInfo.Invoke(null, new object[] { gameObject, rect, true });
            }
        }
    }
}

