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
    public class CustomLockComponent: CustomBaseComponent
    {
        // PRIVATE
        private Color activeColor;
        private Color inactiveColor;
        private Texture2D lockButtonTexture;
        private bool showModifierWarning;
        private int targetLockState = -1;

        // CONSTRUCTOR
        public CustomLockComponent()
        {
            rect.width = 13;

            lockButtonTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomLockButton);

            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalShowModifierWarning , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.LockShow                      , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.LockShowDuringPlayMode        , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalActiveColor         , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalInactiveColor       , settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            showModifierWarning         = CustomSettings.getInstance().get<bool>(CustomSetting.AdditionalShowModifierWarning);
            enabled                     = CustomSettings.getInstance().get<bool>(CustomSetting.LockShow);
            showComponentDuringPlayMode = CustomSettings.getInstance().get<bool>(CustomSetting.LockShowDuringPlayMode);
            activeColor                 = CustomSettings.getInstance().getColor(CustomSetting.AdditionalActiveColor);
            inactiveColor               = CustomSettings.getInstance().getColor(CustomSetting.AdditionalInactiveColor);
        }

        // DRAW
        public override CustomLayoutStatus layout(GameObject gameObject, CustomObjectList objectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < 13)
            {
                return CustomLayoutStatus.Failed;
            }
            else
            {
                curRect.x -= 13;
                rect.x = curRect.x;
                rect.y = curRect.y;
                return CustomLayoutStatus.Success;
            }
        }

        public override void draw(GameObject gameObject, CustomObjectList objectList, Rect selectionRect)
        {  
            bool isLock = isGameObjectLock(gameObject, objectList);

            if (isLock == true && (gameObject.hideFlags & HideFlags.NotEditable) != HideFlags.NotEditable)
            {
                gameObject.hideFlags |= HideFlags.NotEditable;
                EditorUtility.SetDirty(gameObject);
            }
            else if (isLock == false && (gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable)
            {
                gameObject.hideFlags ^= HideFlags.NotEditable;
                EditorUtility.SetDirty(gameObject);
            }

            CustomColorUtils.setColor(isLock ? activeColor : inactiveColor);
            GUI.DrawTexture(rect, lockButtonTexture);
            CustomColorUtils.clearColor();
        }

        public override void eventHandler(GameObject gameObject, CustomObjectList objectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                bool isLock = isGameObjectLock(gameObject, objectList);

                if (currentEvent.type == EventType.MouseDown)
                {
                    targetLockState = ((!isLock) == true ? 1 : 0);
                }
                else if (currentEvent.type == EventType.MouseDrag && targetLockState != -1)
                {
                    if (targetLockState == (isLock == true ? 1 : 0)) return;
                } 
                else
                {
                    targetLockState = -1;
                    return;
                }

                List<GameObject> targetGameObjects = new List<GameObject>();
                if (currentEvent.shift) 
                {
                    if (!showModifierWarning || EditorUtility.DisplayDialog("Change locking", "Are you sure you want to " + (isLock ? "unlock" : "lock") + " this GameObject and all its children? (You can disable this warning in the settings)", "Yes", "Cancel"))
                    {
                        getGameObjectListRecursive(gameObject, ref targetGameObjects);           
                    }
                }
                else if (currentEvent.alt)
                {
                    if (gameObject.transform.parent != null)
                    {
                        if (!showModifierWarning || EditorUtility.DisplayDialog("Change locking", "Are you sure you want to " + (isLock ? "unlock" : "lock") + " this GameObject and its siblings? (You can disable this warning in the settings)", "Yes", "Cancel"))
                        {
                            getGameObjectListRecursive(gameObject.transform.parent.gameObject, ref targetGameObjects, 1);
                            targetGameObjects.Remove(gameObject.transform.parent.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("This action for root objects is supported only for Unity3d 5.3.3 and above");
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
                
                setLock(targetGameObjects, objectList, !isLock);
                currentEvent.Use();
            }
        } 

        public override void disabledHandler(GameObject gameObject, CustomObjectList objectList)
        {	
            if (objectList != null && objectList.lockedObjects.Contains(gameObject))
            {
                objectList.lockedObjects.Remove(gameObject);
                gameObject.hideFlags &= ~HideFlags.NotEditable;
                EditorUtility.SetDirty(gameObject);
            }
        }

        // PRIVATE
        private bool isGameObjectLock(GameObject gameObject, CustomObjectList objectList)
        {
            return objectList == null ? false : objectList.lockedObjects.Contains(gameObject);
        }
        
        private void setLock(List<GameObject> gameObjects, CustomObjectList objectList, bool targetLock)
        {
            if (gameObjects.Count == 0) return;

            if (objectList == null) objectList = CustomObjectListManager.getInstance().getObjectList(gameObjects[0], true);
            Undo.RecordObject(objectList, targetLock ? "Lock" : "Unlock");   
            
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {     
                GameObject curGameObject = gameObjects[i];
                Undo.RecordObject(curGameObject, targetLock ? "Lock" : "Unlock");
                
                if (targetLock)
                {
                    curGameObject.hideFlags |= HideFlags.NotEditable;
                    if (!objectList.lockedObjects.Contains(curGameObject))
                        objectList.lockedObjects.Add(curGameObject);
                }
                else
                {
                    curGameObject.hideFlags &= ~HideFlags.NotEditable;
                    objectList.lockedObjects.Remove(curGameObject);
                }
                
                EditorUtility.SetDirty(curGameObject);
            }
        }
    }
}

