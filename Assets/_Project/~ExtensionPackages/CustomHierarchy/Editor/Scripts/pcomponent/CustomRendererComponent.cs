using System;
using System.Collections.Generic;
using customtools.customhierarchy.pcomponent.pbase;
using customtools.customhierarchy.pdata;
using customtools.customhierarchy.phelper;
using UnityEngine;
using UnityEditor;
using customtools.customhierarchy.phierarchy;

namespace customtools.customhierarchy.pcomponent
{
    public class CustomRendererComponent: CustomBaseComponent
    {
        // PRIVATE
        private Color activeColor;
        private Color inactiveColor;
        private Color specialColor;
        private Texture2D rendererButtonTexture;
        private int targetRendererMode = -1; 

        // CONSTRUCTOR
        public CustomRendererComponent()
        {
            rect.width = 12;

            rendererButtonTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomRendererButton);

            CustomSettings.getInstance().addEventListener(CustomSetting.RendererShow              , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.RendererShowDuringPlayMode, settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalActiveColor     , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalInactiveColor   , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalSpecialColor    , settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled                     = CustomSettings.getInstance().get<bool>(CustomSetting.RendererShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.RendererShowDuringPlayMode);
            activeColor                 = CustomSettings.getInstance().getColor(CustomSetting.AdditionalActiveColor);
            inactiveColor               = CustomSettings.getInstance().getColor(CustomSetting.AdditionalInactiveColor);
            specialColor                = CustomSettings.getInstance().getColor(CustomSetting.AdditionalSpecialColor);
        }

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < 12)
            {
                return CustomLayoutStatus.Failed;
            }
            else
            {
                curRect.x -= 12;
                rect.x = curRect.x;
                rect.y = curRect.y;
                return CustomLayoutStatus.Success;
            }
        }

        public override void disabledHandler(GameObject gameObject, CustomObjectList objectList)
        {
            if (objectList != null && objectList.wireframeHiddenObjects.Contains(gameObject))
            {      
                objectList.wireframeHiddenObjects.Remove(gameObject);
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer != null) setSelectedRenderState(renderer, false);
            }
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                bool wireframeHiddenObjectsContains = isWireframeHidden(gameObject, objectList);
                if (wireframeHiddenObjectsContains)
                {
                    CustomColorUtils.setColor(specialColor);
                    GUI.DrawTexture(rect, rendererButtonTexture);
                    CustomColorUtils.clearColor();
                }
                else if (renderer.enabled)
                {
                    CustomColorUtils.setColor(activeColor);
                    GUI.DrawTexture(rect, rendererButtonTexture);
                    CustomColorUtils.clearColor();
                }
                else
                {
                    CustomColorUtils.setColor(inactiveColor);
                    GUI.DrawTexture(rect, rendererButtonTexture);
                    CustomColorUtils.clearColor();
                }
            }
        }

        public override void eventHandler(GameObject gameObject, CustomObjectList objectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    bool wireframeHiddenObjectsContains = isWireframeHidden(gameObject, objectList);
                    bool isEnabled = renderer.enabled;
                    
                    if (currentEvent.type == EventType.MouseDown)
                    {
                        targetRendererMode = ((!isEnabled) == true ? 1 : 0);
                    }
                    else if (currentEvent.type == EventType.MouseDrag && targetRendererMode != -1)
                    {
                        if (targetRendererMode == (isEnabled == true ? 1 : 0)) return;
                    } 
                    else
                    {
                        targetRendererMode = -1;
                        return;
                    }

                    Undo.RecordObject(renderer, "renderer visibility change");                    
                    
                    if (currentEvent.control || currentEvent.command)
                    {
                        if (!wireframeHiddenObjectsContains)
                        {
                            setSelectedRenderState(renderer, true);
                            SceneView.RepaintAll();
                            setWireframeMode(gameObject, objectList, true);
                        }
                    }
                    else
                    {
                        if (wireframeHiddenObjectsContains)
                        {
                            setSelectedRenderState(renderer, false);
                            SceneView.RepaintAll();
                            setWireframeMode(gameObject, objectList, false);
                        }
                        else
                        {
                            Undo.RecordObject(renderer, isEnabled ? "Disable Component" : "Enable Component");
                            renderer.enabled = !isEnabled;
                        }
                    }
                    
                    EditorUtility.SetDirty(gameObject);
                }
                currentEvent.Use();
            }
        }

        // PRIVATE
        public bool isWireframeHidden(GameObject gameObject, CustomObjectList objectList)
        {
            return objectList == null ? false : objectList.wireframeHiddenObjects.Contains(gameObject);
        }
        
        public void setWireframeMode(GameObject gameObject, CustomObjectList objectList, bool targetWireframe)
        {
            if (objectList == null && targetWireframe) objectList = CustomObjectListManager.getInstance().getObjectList(gameObject, true);
            if (objectList != null)
            {
                Undo.RecordObject(objectList, "Renderer Visibility Change");
                if (targetWireframe) objectList.wireframeHiddenObjects.Add(gameObject);
                else objectList.wireframeHiddenObjects.Remove(gameObject);
                EditorUtility.SetDirty(objectList);
            }
        }

        static public void setSelectedRenderState(Renderer renderer, bool visible)
        {
            #if UNITY_5_5_OR_NEWER
            EditorUtility.SetSelectedRenderState(renderer, visible ? EditorSelectedRenderState.Wireframe : EditorSelectedRenderState.Hidden);
            #else
            EditorUtility.SetSelectedWireframeHidden(renderer, visible);
            #endif
        }
    }
}

