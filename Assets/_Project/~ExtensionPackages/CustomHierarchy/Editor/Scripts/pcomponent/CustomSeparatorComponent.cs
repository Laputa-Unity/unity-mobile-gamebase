using System;
using System.Collections.Generic;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using UnityEngine;
using UnityEditor;
using customtools.customhierarchy.phelper;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomSeparatorComponent: CustomBaseComponent
    {
        // PRIVATE
        private Color separatorColor;
        private Color evenShadingColor;
        private Color oddShadingColor;
        private bool showRowShading;

        // CONSTRUCTOR
        public CustomSeparatorComponent ()
        {
            showComponentDuringPlayMode = true;

            CustomSettings.getInstance().addEventListener(CustomSetting.SeparatorShowRowShading   , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.SeparatorShow             , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.SeparatorColor                , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.SeparatorEvenRowShadingColor  , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.SeparatorOddRowShadingColor , settingsChanged);

            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged()
        {
            showRowShading   = CustomSettings.getInstance().get<bool>(CustomSetting.SeparatorShowRowShading);
            enabled          = CustomSettings.getInstance().get<bool>(CustomSetting.SeparatorShow);
            evenShadingColor = CustomSettings.getInstance().getColor(CustomSetting.SeparatorEvenRowShadingColor);
            oddShadingColor  = CustomSettings.getInstance().getColor(CustomSetting.SeparatorOddRowShadingColor);
            separatorColor   = CustomSettings.getInstance().getColor(CustomSetting.SeparatorColor);
        }

        // DRAW
        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            rect.y = selectionRect.y;
            rect.width = selectionRect.width + selectionRect.x;
            rect.height = 1;
            rect.x = 0;

            EditorGUI.DrawRect(rect, separatorColor);

            if (showRowShading)
            {
                selectionRect.width += selectionRect.x;
                selectionRect.x = 0;
                selectionRect.height -=1;
                selectionRect.y += 1;
                EditorGUI.DrawRect(selectionRect, ((Mathf.FloorToInt(((selectionRect.y - 4) / 16) % 2) == 0)) ? evenShadingColor : oddShadingColor);
            }
        }
    }
}

