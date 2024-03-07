using System;
using System.Collections.Generic;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using UnityEngine;
using UnityEditor;
using customtools.customhierarchy.phierarchy;
using customtools.customhierarchy.phelper;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomChildrenCountComponent: CustomBaseComponent 
    {
        // PRIVATE
        private GUIStyle labelStyle;

        // CONSTRUCTOR
        public CustomChildrenCountComponent ()
        {
            labelStyle = new GUIStyle();
            labelStyle.fontSize = 9;
            labelStyle.clipping = TextClipping.Clip;  
            labelStyle.alignment = TextAnchor.MiddleRight;

            rect.width = 22;
            rect.height = 16;

            CustomSettings.getInstance().addEventListener(CustomSetting.ChildrenCountShow              , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ChildrenCountShowDuringPlayMode, settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ChildrenCountLabelSize         , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ChildrenCountLabelColor        , settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled = CustomSettings.getInstance().get<bool>(CustomSetting.ChildrenCountShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.ChildrenCountShowDuringPlayMode);
            CustomHierarchySize labelSize = (CustomHierarchySize)CustomSettings.getInstance().get<int>(CustomSetting.ChildrenCountLabelSize);
            labelStyle.normal.textColor = CustomSettings.getInstance().getColor(CustomSetting.ChildrenCountLabelColor);
            labelStyle.fontSize = labelSize == CustomHierarchySize.Normal ? 8 : 9;
            rect.width = labelSize == CustomHierarchySize.Normal ? 17 : 22;
        }

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
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
                rect.height = EditorGUIUtility.singleLineHeight;
                return CustomLayoutStatus.Success;
            }
        }
        
        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {  
            int childrenCount = gameObject.transform.childCount;
            if (childrenCount > 0) GUI.Label(rect, childrenCount.ToString(), labelStyle);
        }
    }
}

