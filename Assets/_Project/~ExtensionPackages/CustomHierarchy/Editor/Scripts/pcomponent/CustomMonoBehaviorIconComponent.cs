using System;
using System.Collections.Generic;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using customtools.customhierarchy.phelper;
using UnityEngine;
using UnityEditor;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomMonoBehaviorIconComponent: CustomBaseComponent
    {
        // CONST
        private const float TREE_STEP_WIDTH  = 14.0f;
        private const float TREE_STEP_HEIGHT = 16.0f;

        // PRIVATE
        private Texture2D monoBehaviourIconTexture;   
        private Texture2D monoBehaviourIconObjectTexture; 
        private bool ignoreUnityMonobehaviour;
        private Color iconColor;
        private bool showTreeMap;

        // CONSTRUCTOR 
        public CustomMonoBehaviorIconComponent()
        {
            rect.width  = 14;
            rect.height = 16;
            
            monoBehaviourIconTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomMonoBehaviourIcon);
            monoBehaviourIconObjectTexture  = CustomResources.getInstance().GetTexture(CustomTexture.CustomTreeMapObject);

            CustomSettings.getInstance().addEventListener(CustomSetting.MonoBehaviourIconIgnoreUnityMonobehaviour , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.MonoBehaviourIconShow                     , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.MonoBehaviourIconShowDuringPlayMode       , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.MonoBehaviourIconColor                    , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.TreeMapShow                               , settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            ignoreUnityMonobehaviour    = CustomSettings.getInstance().get<bool>(CustomSetting.MonoBehaviourIconIgnoreUnityMonobehaviour);
            enabled                     = CustomSettings.getInstance().get<bool>(CustomSetting.MonoBehaviourIconShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.MonoBehaviourIconShowDuringPlayMode);
            iconColor                   = CustomSettings.getInstance().getColor(CustomSetting.MonoBehaviourIconColor);
            showTreeMap                 = CustomSettings.getInstance().get<bool>(CustomSetting.TreeMapShow);
            EditorApplication.RepaintHierarchyWindow();  
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            bool foundCustomComponent = false;   
            if (ignoreUnityMonobehaviour)
            {
                Component[] components = gameObject.GetComponents<MonoBehaviour>();                
                for (int i = components.Length - 1; i >= 0; i--)
                {
                    if (components[i] != null && !components[i].GetType().FullName.Contains("UnityEngine")) 
                    {
                        foundCustomComponent = true;
                        break;
                    }
                }                
            }
            else
            {
                foundCustomComponent = gameObject.GetComponent<MonoBehaviour>() != null;
            }

            if (foundCustomComponent)
            {
                int ident = Mathf.FloorToInt(selectionRect.x / TREE_STEP_WIDTH) - 1;

                rect.x = ident * TREE_STEP_WIDTH;
                rect.y = selectionRect.y;
                rect.width = 16;

                #if UNITY_2018_3_OR_NEWER
                    rect.x += TREE_STEP_WIDTH + 1;
                    rect.width += 1;
                #elif UNITY_5_6_OR_NEWER
                    
                #elif UNITY_5_3_OR_NEWER
                    rect.x += TREE_STEP_WIDTH;
                #endif                

                CustomColorUtils.setColor(iconColor);
                GUI.DrawTexture(rect, monoBehaviourIconTexture);
                CustomColorUtils.clearColor();

                if (!showTreeMap && gameObject.transform.childCount == 0)
                {
                    rect.width = 14;
                    GUI.DrawTexture(rect, monoBehaviourIconObjectTexture);
                }
            }
        }
    }
}

