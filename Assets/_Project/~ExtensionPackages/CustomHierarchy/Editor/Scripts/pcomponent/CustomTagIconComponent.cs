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
    public class CustomTagIconComponent: CustomBaseComponent
    {
        private List<CustomTagTexture> tagTextureList;

        // CONSTRUCTOR
        public CustomTagIconComponent()
        {
            rect.width  = 14;
            rect.height = 14;

            CustomSettings.getInstance().addEventListener(CustomSetting.TagIconShow              , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagIconShowDuringPlayMode, settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagIconSize              , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TagIconList              , settingsChanged);
            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged()
        {
            enabled = CustomSettings.getInstance().get<bool>(CustomSetting.TagIconShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.TagIconShowDuringPlayMode);
            CustomHierarchySizeAll size = (CustomHierarchySizeAll)CustomSettings.getInstance().get<int>(CustomSetting.TagIconSize);
            rect.width = rect.height = (size == CustomHierarchySizeAll.Normal ? 15 : (size == CustomHierarchySizeAll.Big ? 16 : 13));        
            this.tagTextureList = CustomTagTexture.LoadTagTextureList();
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
            string gameObjectTag = "";
            try { gameObjectTag = gameObject.tag; }
            catch {}

            CustomTagTexture tagTexture = tagTextureList.Find(t => t.tag == gameObjectTag);
            if (tagTexture != null && tagTexture.texture != null)
            {
                GUI.DrawTexture(rect, tagTexture.texture, ScaleMode.ScaleToFit, true);
            }
        }
    }
}

