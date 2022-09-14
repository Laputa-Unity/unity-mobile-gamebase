#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectGroupFactory
    {
        #region Public Static Functions
        public static ObjectGroup Create(string name, List<string> existingGroupNames)
        {
            if (!string.IsNullOrEmpty(name)) return CreateNewObjectGroupWithUniqueName(name, existingGroupNames);
            else
            {
                Debug.LogWarning("Null or empty object group names are not allowed. Please specify a valid object group name.");
                return null;
            }
        }

        public static ObjectGroup Create(GameObject gameObject, List<string> existingGroupNames)
        {
            if(gameObject == null || string.IsNullOrEmpty(gameObject.name)) return null;

            ObjectGroup newObjectGroup = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectGroup>();
            newObjectGroup.Name = UniqueEntityNameGenerator.GenerateUniqueName(gameObject.name, existingGroupNames);
            newObjectGroup.GroupObject = gameObject;

            return newObjectGroup;
        }
        #endregion

        #region Private Static Functions
        private static ObjectGroup CreateNewObjectGroupWithUniqueName(string name, List<string> existingGroupNames)
        {
            GameObject groupParent = new GameObject();
            UndoEx.RegisterCreatedGameObject(groupParent);

            ObjectGroup newObjectGroup = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectGroup>();
            newObjectGroup.GroupObject = groupParent;
            newObjectGroup.Name = UniqueEntityNameGenerator.GenerateUniqueName(name, existingGroupNames);

            return newObjectGroup;
        }
        #endregion
    }
}
#endif