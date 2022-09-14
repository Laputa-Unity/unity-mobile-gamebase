#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectGroupDatabase : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectGroupCollection _objectGroups = new ObjectGroupCollection();

        [SerializeField]
        private bool _preserveGroupChildren = true;

        [SerializeField]
        private ObjectGroupDatabaseView _view;
        #endregion

        #region Public Properties
        public bool IsEmpty { get { return _objectGroups.IsEmpty; } }
        public int NumberOfGroups { get { return _objectGroups.NumberOfEntities; } }
        public ObjectGroup ActiveGroup { get { return _objectGroups.MarkedEntity; } }
        public int IndexOfActiveGroup { get { return _objectGroups.IndexOfMarkedEntity; } }
        public bool PreserveGroupChildren { get { return _preserveGroupChildren; } set { _preserveGroupChildren = value; } }
        public ObjectGroupDatabaseView View { get { return _view; } }
        #endregion

        public static ObjectGroupDatabase Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase;
        }

        #region Constructors
        public ObjectGroupDatabase()
        {
            _view = new ObjectGroupDatabaseView(this);
        }
        #endregion

        #region Public Methods
        public ObjectGroup CreateObjectGroup(GameObject gameObject)
        {
            if (ContainsObjectGroup(gameObject))
            {
                Debug.LogWarning("This object is already marked as an object group.");
                return GetObjectGroup(gameObject);
            }

            ObjectGroup newObjectGroup = ObjectGroupFactory.Create(gameObject, GetAllObjectGroupNames());

            _objectGroups.AddEntity(newObjectGroup);
            if (NumberOfGroups == 1) SetActiveObjectGroup(newObjectGroup);

            PrefabManagementWindow.Get().Repaint();
            return newObjectGroup;
        }

        public ObjectGroup CreateObjectGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                ObjectGroup newObjectGroup = ObjectGroupFactory.Create(groupName, GetAllObjectGroupNames());

                _objectGroups.AddEntity(newObjectGroup);
                if (NumberOfGroups == 1) SetActiveObjectGroup(newObjectGroup);

                PrefabManagementWindow.Get().Repaint();
                return newObjectGroup;
            }

            return null;
        }

        public void MakeNoLongerGroup(GameObject gameObject)
        {
            if (!ContainsObjectGroup(gameObject)) return;

            UndoEx.RecordForToolAction(this);
            var objectGroup = _objectGroups.GetEntityByPredicate(item => item.GroupObject == gameObject);
            _objectGroups.RemoveEntity(objectGroup);

            UndoEx.DestroyObjectImmediate(objectGroup);
            PrefabManagementWindow.Get().Repaint();
        }

        public void MakeAllNoLongerGroup()
        {
            UndoEx.RecordForToolAction(this);
            var groupsToRemove = _objectGroups.GetAllEntities();
            _objectGroups.RemoveAllEntities();

            foreach(var group in groupsToRemove)
            {
                UndoEx.DestroyObjectImmediate(group);
            }
            PrefabManagementWindow.Get().Repaint();
        }

        public bool ContainsObjectGroup(GameObject objectGroup)
        {
            return _objectGroups.ContainsEntityByPredicate(item => item.GroupObject == objectGroup);
        }

        public bool ContainsObjectGroup(ObjectGroup objectGroup)
        {
            return _objectGroups.ContainsEntity(objectGroup);
        }

        public ObjectGroup GetObjectGroup(GameObject groupParent)
        {
            if (!ContainsObjectGroup(groupParent)) return null;
            return _objectGroups.GetEntityByPredicate(item => item.GroupObject == groupParent);
        }

        public void RenameObjectGroup(ObjectGroup objectGroup, string newName)
        {
            if (ContainsObjectGroup(objectGroup)) _objectGroups.RenameEntity(objectGroup, newName);
            PrefabManagementWindow.Get().Repaint();
        }

        public void RemoveAndDestroyObjectGroup(ObjectGroup objectGroup)
        {
            if (ContainsObjectGroup(objectGroup))
            {
                _objectGroups.RemoveEntity(objectGroup);

                GameObject groupParent = objectGroup.GroupObject;
                if (_preserveGroupChildren) groupParent.MoveImmediateChildrenUpOneLevel(true);
                if (groupParent != null) UndoEx.DestroyObjectImmediate(groupParent);
                UndoEx.DestroyObjectImmediate(objectGroup);

                PrefabManagementWindow.Get().Repaint();
            }
        }

        public void RemoveAndDestroyAllObjectGroups()
        {
            List<ObjectGroup> allGroups = GetAllObjectGroups();
            var groupsToDestroy = new List<ObjectGroup>();
            foreach (ObjectGroup objectGroup in allGroups)
            {
                _objectGroups.RemoveEntity(objectGroup);
                groupsToDestroy.Add(objectGroup);
            }

            foreach (ObjectGroup objectGroup in allGroups)
            {
                GameObject groupParent = objectGroup.GroupObject;
                if (_preserveGroupChildren) groupParent.MoveImmediateChildrenUpOneLevel(true);
                if (groupParent != null) UndoEx.DestroyObjectImmediate(groupParent);
                UndoEx.DestroyObjectImmediate(objectGroup);
            }

            PrefabManagementWindow.Get().Repaint();
        }

        public List<ObjectGroup> GetAllObjectGroups()
        {
            return _objectGroups.GetAllEntities();
        }

        public ObjectGroup GetObjectGroupByIndex(int groupIndex)
        {
            return _objectGroups.GetEntityByIndex(groupIndex);
        }

        public ObjectGroup GetObjectGroupByName(string name)
        {
            if(string.IsNullOrEmpty(name)) return null;
            return _objectGroups.GetEntityByName(name);
        }

        public List<string> GetAllObjectGroupNames()
        {
            return _objectGroups.GetAllEntityNames();
        }

        public void SetActiveObjectGroup(ObjectGroup newActiveObjectGroup)
        {
            if (newActiveObjectGroup != null && !ContainsObjectGroup(newActiveObjectGroup)) return;

            _objectGroups.MarkEntity(newActiveObjectGroup);
        }

        public void RemoveGroupsWithNullParents()
        {
            List<ObjectGroup> groupsWithNullParents = _objectGroups.GetAllEntities();
            groupsWithNullParents = groupsWithNullParents.FindAll(item => item != null && item.GroupObject == null);
            _objectGroups.RemoveWithPredicate(item => item != null && item.GroupObject == null);
            foreach (var group in groupsWithNullParents) ScriptableObject.DestroyImmediate(group);

            if (!IsEmpty)
            {
                if (IndexOfActiveGroup < 0 || IndexOfActiveGroup >= NumberOfGroups) SetActiveObjectGroup(GetObjectGroupByIndex(0));
            }
            else SetActiveObjectGroup(null);
        }
        #endregion
    }
}
#endif