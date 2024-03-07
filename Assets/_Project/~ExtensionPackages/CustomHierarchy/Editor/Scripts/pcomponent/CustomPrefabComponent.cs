using System;
using System.Collections.Generic;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using customtools.customhierarchy.phelper;
using UnityEngine;
using UnityEditor;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomPrefabComponent: CustomBaseComponent
    {
        // PRIVATE
        private Color activeColor;
        private Color inactiveColor;
        private Texture2D prefabTexture;
        private bool showPrefabConnectedIcon;

        // CONSTRUCTOR
        public CustomPrefabComponent()
        {
            rect.width = 9;

            prefabTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomPrefabIcon);

            CustomSettings.getInstance().addEventListener(CustomSetting.PrefabShowBreakedPrefabsOnly  , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.PrefabShow                    , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalActiveColor         , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalInactiveColor       , settingsChanged);
            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged()
        {
            showPrefabConnectedIcon = CustomSettings.getInstance().get<bool>(CustomSetting.PrefabShowBreakedPrefabsOnly);
            enabled                 = CustomSettings.getInstance().get<bool>(CustomSetting.PrefabShow);
            activeColor             = CustomSettings.getInstance().getColor(CustomSetting.AdditionalActiveColor);
            inactiveColor           = CustomSettings.getInstance().getColor(CustomSetting.AdditionalInactiveColor);
        }

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < 9)
            {
                return CustomLayoutStatus.Failed;
            }
            else
            {
                curRect.x -= 9;
                rect.x = curRect.x;
                rect.y = curRect.y;
                return CustomLayoutStatus.Success;
            }
        }
        
        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            #if UNITY_2018_3_OR_NEWER
                PrefabInstanceStatus prefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);
                if (prefabStatus == PrefabInstanceStatus.MissingAsset ||
                    prefabStatus == PrefabInstanceStatus.Disconnected) {
                    CustomColorUtils.setColor(inactiveColor);
                    GUI.DrawTexture(rect, prefabTexture);
                    CustomColorUtils.clearColor();
                } else if (!showPrefabConnectedIcon && prefabStatus != PrefabInstanceStatus.NotAPrefab) {
                    CustomColorUtils.setColor(activeColor);
                    GUI.DrawTexture(rect, prefabTexture);
                    CustomColorUtils.clearColor();
                }
           #else
                PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
                if (prefabType == PrefabType.MissingPrefabInstance || 
                    prefabType == PrefabType.DisconnectedPrefabInstance ||
                    prefabType == PrefabType.DisconnectedModelPrefabInstance)
                {
                    CustomColorUtils.setColor(inactiveColor);
                    GUI.DrawTexture(rect, prefabTexture);
                    CustomColorUtils.clearColor();
                }
                else if (!showPrefabConnectedIcon && prefabType != PrefabType.None)
                {
                    CustomColorUtils.setColor(activeColor);
                    GUI.DrawTexture(rect, prefabTexture);
                    CustomColorUtils.clearColor();
                }
            #endif
        }
    }
}

