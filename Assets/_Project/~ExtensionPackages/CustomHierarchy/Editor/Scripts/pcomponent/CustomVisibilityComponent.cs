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
    public class CustomVisibilityComponent: CustomBaseComponent
    {
        // PRIVATE
        private Color activeColor;
        private Color inactiveColor;
        private Color specialColor;
        private Texture2D visibilityButtonTexture;
        private Texture2D visibilityOffButtonTexture;
        private int targetVisibilityState = -1;

        // CONSTRUCTOR
        public CustomVisibilityComponent()
        {
            rect.width = 18;

            visibilityButtonTexture    = CustomResources.getInstance().GetTexture(CustomTexture.CustomVisibilityButton);
            visibilityOffButtonTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomVisibilityOffButton);

            CustomSettings.getInstance().addEventListener(CustomSetting.VisibilityShow                , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.VisibilityShowDuringPlayMode  , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalActiveColor         , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalInactiveColor       , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalSpecialColor        , settingsChanged);
            settingsChanged();
        }

        private void settingsChanged()
        {
            enabled                     = CustomSettings.getInstance().get<bool>(CustomSetting.VisibilityShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.VisibilityShowDuringPlayMode);
            activeColor                 = CustomSettings.getInstance().getColor(CustomSetting.AdditionalActiveColor);
            inactiveColor               = CustomSettings.getInstance().getColor(CustomSetting.AdditionalInactiveColor);
            specialColor                = CustomSettings.getInstance().getColor(CustomSetting.AdditionalSpecialColor);
        }

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < 18)
            {
                return CustomLayoutStatus.Failed;
            }
            else
            {
                curRect.x -= 18;
                rect.x = curRect.x;
                rect.y = curRect.y;
                return CustomLayoutStatus.Success;
            }
        }

        public override void disabledHandler(GameObject gameObject, CustomObjectList objectList)
        {
            if (objectList != null)
            {
                if (gameObject.activeSelf && objectList.editModeVisibleObjects.Contains(gameObject))
                {
                    objectList.editModeVisibleObjects.Remove(gameObject);
                    gameObject.SetActive(false);
                    EditorUtility.SetDirty(gameObject);
                }
                else if (!gameObject.activeSelf && objectList.editModeInvisibleObjects.Contains(gameObject))
                {
                    objectList.editModeInvisibleObjects.Remove(gameObject);
                    gameObject.SetActive(true);
                    EditorUtility.SetDirty(gameObject);
                }
            }
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {
            int visibility = gameObject.activeSelf ? 1 : 0;
            
            bool editModeVisibleObjectsContains = isEditModeVisibile(gameObject, objectList);
            bool editModeInvisibleObjectsContains = isEditModeInvisibile(gameObject, objectList);
            
            if (!EditorApplication.isPlayingOrWillChangePlaymode && ((!gameObject.activeSelf && editModeVisibleObjectsContains) || (gameObject.activeSelf && editModeInvisibleObjectsContains)))
                gameObject.SetActive(!gameObject.activeSelf);
                        

            Transform transform = gameObject.transform;
            while (transform.parent != null)
            {
                transform = transform.parent;
                if (!transform.gameObject.activeSelf) 
                {
                    visibility = 2;
                    break;
                }
            }
                       
            if (!EditorApplication.isPlayingOrWillChangePlaymode && (editModeVisibleObjectsContains || editModeInvisibleObjectsContains))
            {
                if (visibility == 0)
                {
                    CustomColorUtils.setColor(specialColor);
                    GUI.DrawTexture(rect, visibilityOffButtonTexture);
                }
                else if (visibility == 1)
                {
                    CustomColorUtils.setColor(specialColor);
                    GUI.DrawTexture(rect, visibilityButtonTexture);
                }
                else
                {
                    CustomColorUtils.setColor(specialColor, 1.0f, 0.4f);
                    GUI.DrawTexture(rect, editModeVisibleObjectsContains ? visibilityButtonTexture : visibilityOffButtonTexture);
                }
            }
            else
            {
                if (visibility == 0)
                {
                    CustomColorUtils.setColor(inactiveColor);
                    GUI.DrawTexture(rect, visibilityOffButtonTexture);
                }
                else if (visibility == 1)
                {
                    CustomColorUtils.setColor(activeColor);
                    GUI.DrawTexture(rect, visibilityButtonTexture);
                }
                else
                {
                    if (gameObject.activeSelf)
                    {
                        CustomColorUtils.setColor(activeColor, 0.65f, 0.65f);
                        GUI.DrawTexture(rect, visibilityButtonTexture);                    
                    }
                    else
                    {
                        CustomColorUtils.setColor(inactiveColor, 0.85f, 0.85f);
                        GUI.DrawTexture(rect, visibilityOffButtonTexture);                    
                    }
                }
            }
            CustomColorUtils.clearColor();
        }

        public override void eventHandler(GameObject gameObject, CustomObjectList objectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown)
                {
                    targetVisibilityState = ((!gameObject.activeSelf) == true ? 1 : 0);
                }
                else if (currentEvent.type == EventType.MouseDrag && targetVisibilityState != -1)
                {
                    if (targetVisibilityState == (gameObject.activeSelf == true ? 1 : 0)) return;
                } 
                else
                {
                    targetVisibilityState = -1;
                    return;
                }
                                                            
                bool showWarning = CustomSettings.getInstance().get<bool>(CustomSetting.AdditionalShowModifierWarning);
                
                List<GameObject> targetGameObjects = new List<GameObject>();
                if (currentEvent.control || currentEvent.command) 
                {
                    if (currentEvent.shift)
                    {
                        if (!showWarning || EditorUtility.DisplayDialog("Change edit-time visibility", "Are you sure you want to turn " + (gameObject.activeSelf ? "off" : "on") + " the edit-time visibility of this GameObject and all its children? (You can disable this warning in the settings)", "Yes", "Cancel"))
                        {
                            getGameObjectListRecursive(gameObject, ref targetGameObjects);
                        }
                    }
                    else if (currentEvent.alt)
                    {
                        if (gameObject.transform.parent != null)
                        {
                            if (!showWarning || EditorUtility.DisplayDialog("Change edit-time visibility", "Are you sure you want to turn " + (gameObject.activeSelf ? "off" : "on") + " the edit-time visibility this GameObject and its siblings? (You can disable this warning in the settings)", "Yes", "Cancel"))
                            {
                                getGameObjectListRecursive(gameObject.transform.parent.gameObject, ref targetGameObjects, 1);
                                targetGameObjects.Remove(gameObject.transform.parent.gameObject);
                            }
                        }
                        else
                        {
                            Debug.Log("This action for root objects is supported for Unity3d 5.3.3 and above");
                            return;
                        }
                    }
                    else
                    {
                        getGameObjectListRecursive(gameObject, ref targetGameObjects, 0);
                    }
                }
                else if (currentEvent.shift)
                {
                    if (!showWarning || EditorUtility.DisplayDialog("Change visibility", "Are you sure you want to turn " + (gameObject.activeSelf ? "off" : "on") + " the visibility of this GameObject and all its children? (You can disable this warning in the settings)", "Yes", "Cancel"))
                    {
                        getGameObjectListRecursive(gameObject, ref targetGameObjects);           
                    }
                }
                else if (currentEvent.alt) 
                {
                    if (gameObject.transform.parent != null)
                    {
                        if (!showWarning || EditorUtility.DisplayDialog("Change visibility", "Are you sure you want to turn " + (gameObject.activeSelf ? "off" : "on") + " the visibility this GameObject and its siblings? (You can disable this warning in the settings)", "Yes", "Cancel"))
                        {
                            getGameObjectListRecursive(gameObject.transform.parent.gameObject, ref targetGameObjects, 1);
                            targetGameObjects.Remove(gameObject.transform.parent.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("This action for root objects is supported for Unity3d 5.3.3 and above");
                        return;
                    }
                }
                else 
                {
                    if (Selection.Contains(gameObject))
                    {
                        targetGameObjects.AddRange(Selection.gameObjects);
                    }
                    else
                    {
                        getGameObjectListRecursive(gameObject, ref targetGameObjects, 0);
                    };
                }
                
                setVisibility(targetGameObjects, objectList, !gameObject.activeSelf, currentEvent.control || currentEvent.command);
                currentEvent.Use();  
            } 
        }

        // PRIVATE
        private bool isEditModeVisibile(GameObject gameObject, CustomObjectList objectList)
        {
            return objectList == null ? false : objectList.editModeVisibleObjects.Contains(gameObject);
        }
        
        private bool isEditModeInvisibile(GameObject gameObject, CustomObjectList objectList)
        {
            return objectList == null ? false : objectList.editModeInvisibleObjects.Contains(gameObject);
        }
        
        private void setVisibility(List<GameObject> gameObjects, CustomObjectList objectList, bool targetVisibility, bool editMode)
        {
            if (gameObjects.Count == 0) return;

            if (objectList == null && editMode) objectList = CustomObjectListManager.getInstance().getObjectList(gameObjects[0], true);
            if (objectList != null) Undo.RecordObject(objectList, "visibility change");
            
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {        
                GameObject curGameObject = gameObjects[i];
                Undo.RecordObject(curGameObject, "visibility change");
                
                if (editMode)
                {
                    if (!targetVisibility)
                    {
                        objectList.editModeVisibleObjects.Remove(curGameObject);        
                        if (!objectList.editModeInvisibleObjects.Contains(curGameObject))
                            objectList.editModeInvisibleObjects.Add(curGameObject);
                    }
                    else
                    {
                        objectList.editModeInvisibleObjects.Remove(curGameObject);                            
                        if (!objectList.editModeVisibleObjects.Contains(curGameObject))
                            objectList.editModeVisibleObjects.Add(curGameObject);
                    }
                }
                else if (objectList != null)
                {
                    objectList.editModeVisibleObjects.Remove(curGameObject);
                    objectList.editModeInvisibleObjects.Remove(curGameObject);
                }
                
                curGameObject.SetActive(targetVisibility);
                EditorUtility.SetDirty(curGameObject);
            }
        }
    }
}

