#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PrefabFactory
    {
        #region Public Static Functions
        public static Prefab Create(GameObject unityPrefab)
        {
            Prefab prefab = new Prefab();
            prefab.UnityPrefab = unityPrefab;

            return prefab;
        }

        public static List<Prefab> Create(List<GameObject> unityPrefabs)
        {
            var prefabs = new List<Prefab>(unityPrefabs.Count);
            foreach(GameObject unityPrefab in unityPrefabs)
            {
                prefabs.Add(Create(unityPrefab));
            }

            return prefabs;
        }

        public static Prefab CreateFromSelectedObjects(Pivot prefabPivot)
        {
            // Ensure that all necessary data is in place
            ObjectSelectionPrefabCreationSettings prefabCreationSettings = ObjectSelectionPrefabCreationSettings.Get();
            if (string.IsNullOrEmpty(prefabCreationSettings.PrefabName) || string.IsNullOrEmpty(prefabCreationSettings.DestinationFolder)) return null;
            if (ObjectSelection.Get().NumberOfSelectedObjects == 0) return null;

            List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
            if (allSelectedObjects.Count == 0) return null;

            // Check if a prefab with the same name already exists
            bool shouldPrefabBeCreated = true;
            GameObject prefabWithSameName = ProjectAssetDatabase.LoadPrefabWithNameInFolder(prefabCreationSettings.PrefabName, prefabCreationSettings.DestinationFolder, false);
            if (prefabWithSameName != null)
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "A prefab with the specified name already exists in the specified folder. Would you like to overwrite it?", "Yes", "No"))
                {
                    // If the user chose 'Yes', we have to remove the existing prefab from its category
                    PrefabCategory categoryWhichContainsSamePrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(prefabWithSameName);
                    if (categoryWhichContainsSamePrefab != null)
                    {
                        categoryWhichContainsSamePrefab.RemoveAndDestroyPrefab(prefabWithSameName);
                    }
                }
                else shouldPrefabBeCreated = false;
            }
            if (!shouldPrefabBeCreated) return null;

            // Create all the objects which will reside in the prefab hierarchy
            GameObject prefabRoot = new GameObject(prefabCreationSettings.PrefabName);
            Box objectCollectionWorldBox = Box.GetInvalid();
            var allObjectsInPrefabHierarchy = new List<GameObject>();
            foreach(GameObject gameObject in allSelectedObjects)
            {
                Transform gameObjectTransform = gameObject.transform;
                GameObject gameObjectClone = Octave3DWorldBuilder.Instantiate(gameObject, gameObjectTransform.position, gameObjectTransform.rotation) as GameObject;
                allObjectsInPrefabHierarchy.Add(gameObjectClone);

                Transform cloneTransform = gameObjectClone.transform;
                gameObjectClone.name = gameObject.name;
                cloneTransform.localScale = gameObjectTransform.lossyScale;

                if (objectCollectionWorldBox.IsValid()) objectCollectionWorldBox.Encapsulate(gameObjectClone.GetWorldBox());
                else objectCollectionWorldBox = gameObjectClone.GetWorldBox();
            }

            // Now calculate the root object's position based on the specified pivot point
            Transform prefabRootTransform = prefabRoot.transform;
            if (prefabPivot == Pivot.Center) prefabRootTransform.position = objectCollectionWorldBox.Center;
            else if (prefabPivot == Pivot.BottomCenter) prefabRootTransform.position = objectCollectionWorldBox.GetBoxFaceCenter(BoxFace.Bottom);

            // Now that the root object's position is in place, attach all objects as children of the root
            foreach(GameObject gameObject in allObjectsInPrefabHierarchy)
            {
                gameObject.transform.parent = prefabRootTransform;
            }

            // Create the prefab and assign it to the chosen category
            GameObject createdUnityPrefab = ProjectAssetDatabase.CreatePrefab(prefabRoot, prefabCreationSettings.PrefabName, prefabCreationSettings.DestinationFolder);
            if (createdUnityPrefab == null) return null;

            UndoEx.RecordForToolAction(prefabCreationSettings.DestinationCategory);
            Prefab createdPrefab = PrefabFactory.Create(createdUnityPrefab);
            prefabCreationSettings.DestinationCategory.AddPrefab(createdPrefab);

            Octave3DWorldBuilder.DestroyImmediate(prefabRoot);
            PrefabManagementWindow.Get().RepaintOctave3DWindow();

            return createdPrefab;
        }
        #endregion
    }
}
#endif