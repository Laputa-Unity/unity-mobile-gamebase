using System;
using System.Collections.Generic;
using customtools.customhierarchy.pdata;
using UnityEngine;
using UnityEditor;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif

namespace customtools.customhierarchy.phelper
{
    public class CustomObjectListManager
    {
        // CONST
        private const string CustomObjectListName = "CustomHierarchyObjectList";

        // SINGLETON
        private static CustomObjectListManager instance;
        public static CustomObjectListManager getInstance()
        {
            if (instance == null) instance = new CustomObjectListManager();
            return instance;
        }

        // PRIVATE
        private bool showObjectList;
        private bool preventSelectionOfLockedObjects;
        private bool preventSelectionOfLockedObjectsDuringPlayMode;
        private GameObject lastSelectionGameObject = null;
        private int lastSelectionCount = 0;

        // CONSTRUCTOR
        private CustomObjectListManager()
        {
            CustomSettings.getInstance().addEventListener(CustomSetting.AdditionalShowHiddenCustomHierarchyObjectList , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.LockPreventSelectionOfLockedObjects, settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.LockShow              , settingsChanged);
            CustomSettings.getInstance().addEventListener(CustomSetting.LockShowDuringPlayMode, settingsChanged);
            settingsChanged();
        }

        private void settingsChanged()
        {
            showObjectList = CustomSettings.getInstance().get<bool>(CustomSetting.AdditionalShowHiddenCustomHierarchyObjectList);
            preventSelectionOfLockedObjects = CustomSettings.getInstance().get<bool>(CustomSetting.LockShow) && CustomSettings.getInstance().get<bool>(CustomSetting.LockPreventSelectionOfLockedObjects);
            preventSelectionOfLockedObjectsDuringPlayMode = preventSelectionOfLockedObjects && CustomSettings.getInstance().get<bool>(CustomSetting.LockShowDuringPlayMode);
        }

        private bool isSelectionChanged()
        {
            if (lastSelectionGameObject != Selection.activeGameObject || lastSelectionCount  != Selection.gameObjects.Length)
            {
                lastSelectionGameObject = Selection.activeGameObject;
                lastSelectionCount = Selection.gameObjects.Length;
                return true;
            }
            return false;
        }

        public void validate()
        {
            CustomObjectList.instances.RemoveAll(item => item == null);
            foreach (CustomObjectList objectList in CustomObjectList.instances)
                objectList.CheckIntegrity();
            #if UNITY_5_3_OR_NEWER
            _objectListDictionary.Clear();
            foreach (CustomObjectList objectList in CustomObjectList.instances)            
                _objectListDictionary.Add(objectList.gameObject.scene, objectList);
            #endif
        }

        #if UNITY_5_3_OR_NEWER
        private readonly Dictionary<Scene, CustomObjectList> _objectListDictionary = new Dictionary<Scene, CustomObjectList>();
        private Scene _lastActiveScene;
        private int _lastSceneCount = 0;

        public void Update()
        {
            try
            {     
                List<CustomObjectList> objectListList = CustomObjectList.instances;
                int objectListCount = objectListList.Count;
                if (objectListCount > 0) 
                {
                    for (int i = objectListCount - 1; i >= 0; i--)
                    {
                        CustomObjectList objectList = objectListList[i];
                        Scene objectListScene = objectList.gameObject.scene;
						
						if (_objectListDictionary.ContainsKey(objectListScene) && _objectListDictionary[objectListScene] == null)
                            _objectListDictionary.Remove(objectListScene);
							
                        if (_objectListDictionary.ContainsKey(objectListScene))
                        {
                            if (_objectListDictionary[objectListScene] != objectList)
                            {
                                _objectListDictionary[objectListScene].merge(objectList);
                                GameObject.DestroyImmediate(objectList.gameObject);
                            }
                        }
                        else
                        {
                            _objectListDictionary.Add(objectListScene, objectList);
                        }
                    }

                    foreach (KeyValuePair<Scene, CustomObjectList> objectListKeyValue in _objectListDictionary)
                    {
                        CustomObjectList objectList = objectListKeyValue.Value;
                        setupObjectList(objectList);
                        if (( showObjectList && ((objectList.gameObject.hideFlags & HideFlags.HideInHierarchy)  > 0)) ||
                            (!showObjectList && ((objectList.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)))
                        {
                            objectList.gameObject.hideFlags ^= HideFlags.HideInHierarchy;      
                            EditorApplication.DirtyHierarchyWindowSorting();
                        }
                    }
                    
                    if ((!Application.isPlaying && preventSelectionOfLockedObjects) || 
                        ((Application.isPlaying && preventSelectionOfLockedObjectsDuringPlayMode)) && 
                        isSelectionChanged())
                    {
                        GameObject[] selections = Selection.gameObjects;
                        List<GameObject> actual = new List<GameObject>(selections.Length);
                        bool found = false;
                        for (int i = selections.Length - 1; i >= 0; i--)
                        {
                            GameObject gameObject = selections[i];
                            
                            if (_objectListDictionary.ContainsKey(gameObject.scene))
                            {
                                bool isLock = _objectListDictionary[gameObject.scene].lockedObjects.Contains(selections[i]);
                                if (!isLock) actual.Add(selections[i]);
                                else found = true;
                            }
                        }
                        if (found) Selection.objects = actual.ToArray();
                    }   

                    _lastActiveScene = EditorSceneManager.GetActiveScene();
                    _lastSceneCount = SceneManager.loadedSceneCount;
                }
            }
            catch 
            {
            }
        }

        public CustomObjectList getObjectList(GameObject gameObject, bool createIfNotExist = true)
        { 
            CustomObjectList objectList = null;
            _objectListDictionary.TryGetValue(gameObject.scene, out objectList);
            
            if (objectList == null && createIfNotExist)
            {         
                objectList = createObjectList(gameObject);
                if (gameObject.scene != objectList.gameObject.scene) EditorSceneManager.MoveGameObjectToScene(objectList.gameObject, gameObject.scene);
                _objectListDictionary.Add(gameObject.scene, objectList);
            }

            return objectList;
        }

        public bool isSceneChanged()
        {
            if (_lastActiveScene != EditorSceneManager.GetActiveScene() || _lastSceneCount != SceneManager.loadedSceneCount)
                return true;
            else 
                return false;
        }

        #else

        public void update()
        {
            try
            {  
                List<CustomObjectList> objectListList = CustomObjectList.instances;
                int objectListCount = objectListList.Count;
                if (objectListCount > 0) 
                {
                    if (objectListCount > 1)
                    {
                        for (int i = objectListCount - 1; i > 0; i--)
                        {
                            objectListList[0].merge(objectListList[i]);
                            GameObject.DestroyImmediate(objectListList[i].gameObject);
                        }
                    }

                    CustomObjectList objectList = CustomObjectList.instances[0];
                    setupObjectList(objectList);

                    if (( showObjectList && ((objectList.gameObject.hideFlags & HideFlags.HideInHierarchy)  > 0)) ||
                        (!showObjectList && ((objectList.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)))
                    {
                        objectList.gameObject.hideFlags ^= HideFlags.HideInHierarchy; 
                        EditorApplication.DirtyHierarchyWindowSorting();
                    }

                    if ((!Application.isPlaying && preventSelectionOfLockedObjects) || 
                        ((Application.isPlaying && preventSelectionOfLockedObjectsDuringPlayMode))
                        && isSelectionChanged())
                    {
                        GameObject[] selections = Selection.gameObjects;
                        List<GameObject> actual = new List<GameObject>(selections.Length);
                        bool found = false;
                        for (int i = selections.Length - 1; i >= 0; i--)
                        {
                            GameObject gameObject = selections[i];
                            
                            bool isLock = objectList.lockedObjects.Contains(gameObject);                        
                            if (!isLock) actual.Add(selections[i]);
                            else found = true;
                        }
                        if (found) Selection.objects = actual.ToArray();
                    }   
                }
            }
            catch 
            {
            }
        }

        public CustomObjectList getObjectList(GameObject gameObject, bool createIfNotExists = false)
        { 
            List<CustomObjectList> objectListList = CustomObjectList.instances;
            int objectListCount = objectListList.Count;
            if (objectListCount != 1)
            {
                if (objectListCount == 0) 
                {
                    if (createIfNotExists)
                    {
                        createObjectList(gameObject);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
                
            return CustomObjectList.instances[0];
        }

        #endif

        private CustomObjectList createObjectList(GameObject gameObject)
        {
            GameObject gameObjectList = new GameObject();
            gameObjectList.name = CustomObjectListName;
            CustomObjectList objectList = gameObjectList.AddComponent<CustomObjectList>();
            setupObjectList(objectList);
            return objectList;
        }

        private void setupObjectList(CustomObjectList objectList)
        {
            if (objectList.tag == "EditorOnly") objectList.tag = "Untagged";
            MonoScript monoScript = MonoScript.FromMonoBehaviour(objectList);
            if (MonoImporter.GetExecutionOrder(monoScript) != -10000)                    
                MonoImporter.SetExecutionOrder(monoScript, -10000);
        }
    }
}

