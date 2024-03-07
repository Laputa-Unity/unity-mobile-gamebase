using UnityEngine;
using UnityEditor;
using System;
using customtools.customhierarchy.phierarchy;
using System.Collections.Generic;
using System.Collections;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using customtools.customhierarchy.phelper;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomTreeMapComponent: CustomBaseComponent
    {
        // CONST
        private const float TREE_STEP_WIDTH  = 14.0f;
        
        // PRIVATE
        private Texture2D treeMapLevelTexture;       
        private Texture2D treeMapLevel4Texture;       
        private Texture2D treeMapCurrentTexture;   
        private Texture2D treeMapLastTexture;
        private Texture2D treeMapObjectTexture;    
        private bool enhanced;
        private bool transparentBackground;
        private Color backgroundColor;
        private Color treeMapColor;
        
        // CONSTRUCTOR
        public CustomTreeMapComponent()
        { 

            treeMapLevelTexture   = CustomResources.getInstance().GetTexture(CustomTexture.CustomTreeMapLevel);
            treeMapLevel4Texture  = CustomResources.getInstance().GetTexture(CustomTexture.CustomTreeMapLevel4);
            treeMapCurrentTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomTreeMapCurrent);
            #if UNITY_2018_3_OR_NEWER
                treeMapObjectTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomTreeMapLine);
            #else
                treeMapObjectTexture  = CustomResources.getInstance().getTexture(CustomTexture.CustomTreeMapObject);
            #endif
            treeMapLastTexture    = CustomResources.getInstance().GetTexture(CustomTexture.CustomTreeMapLast);
            
            rect.width  = 14;
            rect.height = 16;
            
            showComponentDuringPlayMode = true;

            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalBackgroundColor, settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TreeMapShow           , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TreeMapColor          , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TreeMapEnhanced       , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TreeMapTransparentBackground, settingsChanged);
            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged() {
            backgroundColor     = CustomSettings.getInstance().getColor(CustomSetting.AdditionalBackgroundColor);
            enabled             = CustomSettings.getInstance().get<bool>(CustomSetting.TreeMapShow);
            treeMapColor        = CustomSettings.getInstance().getColor(CustomSetting.TreeMapColor);
            enhanced            = CustomSettings.getInstance().get<bool>(CustomSetting.TreeMapEnhanced);
            transparentBackground = CustomSettings.getInstance().get<bool>(CustomSetting.TreeMapTransparentBackground);
        }
        
        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            rect.y = selectionRect.y;
            
            if (!transparentBackground) 
            {
                rect.x = 0;
                
                rect.width = selectionRect.x - 14;
                EditorGUI.DrawRect(rect, backgroundColor);
                rect.width = 14;
            }

            return CustomLayoutStatus.Success;
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            int childCount = gameObject.transform.childCount;
            int level = Mathf.RoundToInt(selectionRect.x / 14.0f);

            if (enhanced)
            {
                Transform gameObjectTransform = gameObject.transform;
                Transform parentTransform = null;

                for (int i = 0, j = level - 1; j >= 0; i++, j--)
                {
                    rect.x = 14 * j;
                    if (i == 0)
                    {
                        if (childCount == 0) {
                            #if UNITY_2018_3_OR_NEWER
                                CustomColorUtils.setColor(treeMapColor);
                            #endif
                            GUI.DrawTexture(rect, treeMapObjectTexture);
                        }
                        gameObjectTransform = gameObject.transform;
                    }
                    else if (i == 1)
                    {
                        CustomColorUtils.setColor(treeMapColor);
                        if (parentTransform == null) {
                            if (gameObjectTransform.GetSiblingIndex() == gameObject.scene.rootCount - 1) {
                                GUI.DrawTexture(rect, treeMapLastTexture);
                            } else {
                                GUI.DrawTexture(rect, treeMapCurrentTexture);
                            }
                        } else if (gameObjectTransform.GetSiblingIndex() == parentTransform.childCount - 1) {
                            GUI.DrawTexture(rect, treeMapLastTexture);
                        } else {
                            GUI.DrawTexture(rect, treeMapCurrentTexture);
                        }
                        gameObjectTransform = parentTransform;
                    }
                    else
                    {
                        if (parentTransform == null) {
                            if (gameObjectTransform.GetSiblingIndex() != gameObject.scene.rootCount - 1)
                                GUI.DrawTexture(rect, treeMapLevelTexture);
                        } else if (gameObjectTransform.GetSiblingIndex() != parentTransform.childCount - 1)
                            GUI.DrawTexture(rect, treeMapLevelTexture);

                        gameObjectTransform = parentTransform;                       
                    }
                    if (gameObjectTransform != null) 
						parentTransform = gameObjectTransform.parent;
					else
                        break;
                }
                CustomColorUtils.clearColor();
            }
            else
            {
                for (int i = 0, j = level - 1; j >= 0; i++, j--)
                {
                    rect.x = 14 * j;
                    if (i == 0)
                    {
                        if (childCount > 0)
                            continue;
                        else {
                            #if UNITY_2018_3_OR_NEWER
                                CustomColorUtils.setColor(treeMapColor);
                            #endif
                            GUI.DrawTexture(rect, treeMapObjectTexture);
                        }
                    }
                    else if (i == 1)
                    {
                        CustomColorUtils.setColor(treeMapColor);
                        GUI.DrawTexture(rect, treeMapCurrentTexture);
                    }
                    else
                    {
                        rect.width = 14 * 4;
                        rect.x -= 14 * 3;
                        j -= 3;
                        GUI.DrawTexture(rect, treeMapLevel4Texture);
                        rect.width = 14;
                    }
                }
                CustomColorUtils.clearColor();
            }
        }
    }
}

