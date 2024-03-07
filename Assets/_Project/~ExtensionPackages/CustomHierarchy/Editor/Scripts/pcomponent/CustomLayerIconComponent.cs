using System;
using System.Collections.Generic;
using System.Reflection;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using UnityEngine;
using UnityEditor;
using customtools.customhierarchy.phierarchy;
using customtools.customhierarchy.phelper;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomLayerIconComponent: CustomBaseComponent
    {
        private List<CustomLayerTexture> layerTextureList;

        // CONSTRUCTOR
        public CustomLayerIconComponent()
        {
            rect.width  = 14;
            rect.height = 14;

            CustomSettings.getInstance().addEventListener(CustomSetting.LayerIconShow              , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.LayerIconShowDuringPlayMode, settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.LayerIconSize              , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.LayerIconList              , settingsChanged);
            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged()
        {
            enabled                     = CustomSettings.getInstance().get<bool>(CustomSetting.LayerIconShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.LayerIconShowDuringPlayMode);
            CustomHierarchySizeAll size      = (CustomHierarchySizeAll)CustomSettings.getInstance().get<int>(CustomSetting.LayerIconSize);
            rect.width = rect.height    = (size == CustomHierarchySizeAll.Normal ? 15 : (size == CustomHierarchySizeAll.Big ? 16 : 13));        
            this.layerTextureList = CustomLayerTexture.LoadLayerTextureList();
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
                rect.y = curRect.y - (rect.height - 16) / 2;
                return CustomLayoutStatus.Success;
            }
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {                       
            string gameObjectLayerName = LayerMask.LayerToName(gameObject.layer);

            CustomLayerTexture layerTexture = layerTextureList.Find(t => t.layer == gameObjectLayerName);
            if (layerTexture != null && layerTexture.texture != null)
            {
                GUI.DrawTexture(rect, layerTexture.texture, ScaleMode.ScaleToFit, true);
            }
        }
    }
}

