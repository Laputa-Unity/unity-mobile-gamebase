using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using customtools.customhierarchy.pcomponent;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using customtools.customhierarchy.phelper;

namespace customtools.customhierarchy.phierarchy
{
    public class CustomHierarchy
    {
        // PRIVATE
        private HashSet<int> errorHandled = new HashSet<int>();      
        private Dictionary<int, CustomBaseComponent> componentDictionary;          
        private List<CustomBaseComponent> preComponents;
        private List<CustomBaseComponent> orderedComponents;
        private bool hideIconsIfThereIsNoFreeSpace;
        private int indentation;
        private Texture2D trimIcon;
        private Color backgroundColor;
        private Color inactiveColor;

        // CONSTRUCTOR
        public CustomHierarchy ()
        {           
            componentDictionary = new Dictionary<int, CustomBaseComponent>();
            componentDictionary.Add((int)CustomHierarchyComponentEnum.LockComponent             , new CustomLockComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.VisibilityComponent       , new CustomVisibilityComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.StaticComponent           , new CustomStaticComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.RendererComponent         , new CustomRendererComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.TagAndLayerComponent      , new CustomTagLayerComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.GameObjectIconComponent   , new CustomGameObjectIconComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.ErrorComponent            , new CustomErrorComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.TagIconComponent          , new CustomTagIconComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.LayerIconComponent        , new CustomLayerIconComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.ColorComponent            , new CustomColorComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.ComponentsComponent       , new CustomComponentsComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.ChildrenCountComponent    , new CustomChildrenCountComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.PrefabComponent           , new CustomPrefabComponent());
            componentDictionary.Add((int)CustomHierarchyComponentEnum.VerticesAndTrianglesCount , new CustomVerticesAndTrianglesCountComponent());

            preComponents = new List<CustomBaseComponent>();
            preComponents.Add(new CustomMonoBehaviorIconComponent());
            preComponents.Add(new CustomTreeMapComponent());
            preComponents.Add(new CustomSeparatorComponent());

            orderedComponents = new List<CustomBaseComponent>();

            trimIcon = CustomResources.getInstance().GetTexture(CustomTexture.CustomTrimIcon);

            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalIdentation             , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.ComponentsOrder                  , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalHideIconsIfNotFit      , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalBackgroundColor        , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalInactiveColor          , settingsChanged);
            settingsChanged();
        }
         
        // PRIVATE
        private void settingsChanged()
        {
            string componentOrder = CustomSettings.getInstance().get<string>(CustomSetting.ComponentsOrder);
            string[] componentIds = componentOrder.Split(';');
            if (componentIds.Length != CustomSettings.DEFAULT_ORDER_COUNT) 
            {
                CustomSettings.getInstance().set(CustomSetting.ComponentsOrder, CustomSettings.DEFAULT_ORDER, false);
                componentIds = CustomSettings.DEFAULT_ORDER.Split(';');
            }

            orderedComponents.Clear(); 
            for (int i = 0; i < componentIds.Length; i++)                
                orderedComponents.Add(componentDictionary[int.Parse(componentIds[i])]);
            orderedComponents.Add(componentDictionary[(int)CustomHierarchyComponentEnum.ComponentsComponent]);

            indentation                     = CustomSettings.getInstance().get<int>(CustomSetting.AdditionalIdentation);
            hideIconsIfThereIsNoFreeSpace   = CustomSettings.getInstance().get<bool>(CustomSetting.AdditionalHideIconsIfNotFit);
            backgroundColor                 = CustomSettings.getInstance().getColor(CustomSetting.AdditionalBackgroundColor);
            inactiveColor                   = CustomSettings.getInstance().getColor(CustomSetting.AdditionalInactiveColor);
        } 

        public void hierarchyWindowItemOnGUIHandler(int instanceId, Rect selectionRect)
        {
            try
            {
                CustomColorUtils.setDefaultColor(GUI.color);

                GameObject gameObject = (GameObject)EditorUtility.InstanceIDToObject(instanceId);
                if (gameObject == null) return;

                Rect curRect = new Rect(selectionRect);
                curRect.width = 16;
                curRect.x += selectionRect.width - indentation;

                float gameObjectNameWidth = hideIconsIfThereIsNoFreeSpace ? GUI.skin.label.CalcSize(new GUIContent(gameObject.name)).x : 0;

                CustomObjectList objectList = CustomObjectListManager.getInstance().getObjectList(gameObject, false);

                drawComponents(orderedComponents, selectionRect, ref curRect, gameObject, objectList, true, hideIconsIfThereIsNoFreeSpace ? selectionRect.x + gameObjectNameWidth + 7 : 0);    

                errorHandled.Remove(instanceId);
            }
            catch (Exception exception)
            {
                if (errorHandled.Add(instanceId))
                {
                    Debug.LogError(exception.ToString());
                }
            }
        }

        private void drawComponents(List<CustomBaseComponent> components, Rect selectionRect, ref Rect rect, GameObject gameObject, CustomObjectList objectList, bool drawBackground = false, float minX = 50)
        {
            if (Event.current.type == EventType.Repaint)
            {
                int toComponent = components.Count;
                CustomLayoutStatus layoutStatus = CustomLayoutStatus.Success;
                for (int i = 0, n = toComponent; i < n; i++)
                {
                    CustomBaseComponent component = components[i];
                    if (component.isEnabled())
                    {
                        layoutStatus = component.layout(gameObject, objectList, selectionRect, ref rect, rect.x - minX);
                        if (layoutStatus != CustomLayoutStatus.Success)
                        {
                            toComponent = layoutStatus == CustomLayoutStatus.Failed ? i : i + 1;
                            rect.x -= 7;

                            break;
                        }
                    }
                    else
                    {
                        component.disabledHandler(gameObject, objectList);
                    }
                } 

                if (drawBackground)
                {
                    if (backgroundColor.a != 0)
                    {
                        rect.width = selectionRect.x + selectionRect.width - rect.x /*- indentation*/;
                        EditorGUI.DrawRect(rect, backgroundColor);
                    }
                    drawComponents(preComponents    , selectionRect, ref rect, gameObject, objectList);
                }

                for (int i = 0, n = toComponent; i < n; i++)
                {
                    CustomBaseComponent component = components[i];
                    if (component.isEnabled())
                    {
                        component.draw(gameObject, objectList, selectionRect);
                    }
                }

                if (layoutStatus != CustomLayoutStatus.Success)
                {
                    rect.width = 7;
                    CustomColorUtils.setColor(inactiveColor);
                    GUI.DrawTexture(rect, trimIcon);
                    CustomColorUtils.clearColor();
                }
            }
            else if (Event.current.isMouse)
            {
                for (int i = 0, n = components.Count; i < n; i++)
                {
                    CustomBaseComponent component = components[i];
                    if (component.isEnabled())
                    {
                        if (component.layout(gameObject, objectList, selectionRect, ref rect, rect.x - minX) != CustomLayoutStatus.Failed)
                        {
                            component.eventHandler(gameObject, objectList, Event.current);
                        }
                    }
                }
            }
        }
    }
}

