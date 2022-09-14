#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagDatabase : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private PrefabTagCollection _prefabTags = new PrefabTagCollection();
        [SerializeField]
        private PrefabTagFilter _prefabTagFilter;

        [SerializeField]
        private PrefabTagDatabaseView _view;
        #endregion

        #region Public Properties
        public bool IsEmpty { get { return NumberOfTags == 0; } }
        public int NumberOfTags { get { return _prefabTags.NumberOfEntities; } }
        public PrefabTagDatabaseView View { get { return _view; } }
        #endregion

        #region Constructors
        public PrefabTagDatabase()
        {
            _view = new PrefabTagDatabaseView(this);
        }
        #endregion

        #region Public Properties
        public PrefabTagFilter PrefabTagFilter
        {
            get
            {
                if (_prefabTagFilter == null) _prefabTagFilter = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabTagFilter>();
                return _prefabTagFilter;
            }
        }
        #endregion

        #region Public Static Functions
        public static PrefabTagDatabase Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.PrefabTagDatabase;
        }
        #endregion

        #region Public Methods
        public PrefabTag CreatePrefabTag(string tagName)
        {
            if (!string.IsNullOrEmpty(tagName))
            {
                PrefabTag newPrefabTag = PrefabTagFactory.Create(tagName, GetAllPrefabTagNames());
   
                _prefabTags.AddEntity(newPrefabTag);
                PrefabTagWasCreatedInDatabaseMessage.SendToInterestedListeners(newPrefabTag);

                return newPrefabTag;
            }

            return null;
        }

        public bool ContainsPrefabTag(PrefabTag prefabTag)
        {
            return _prefabTags.ContainsEntity(prefabTag);
        }

        public bool ContainsPrefabTag(string prefabTagName)
        {
            return GetPrefabTagByName(prefabTagName) != null;
        }

        public void RemoveAndDestroyPrefabTag(PrefabTag prefabTag)
        {
            if(ContainsPrefabTag(prefabTag))
            {
                _prefabTags.RemoveEntity(prefabTag);
                PrefabTagWasRemovedFromDatabaseMessage.SendToInterestedListeners(prefabTag);

                UndoEx.DestroyObjectImmediate(prefabTag);
            }
        }

        public void RemoveAndDestroyAllPrefabTags()
        {
            List<PrefabTag> allPrefabTags = GetAllPrefabTags();
            foreach (PrefabTag prefabTag in allPrefabTags)
            {
                _prefabTags.RemoveEntity(prefabTag);
                PrefabTagWasRemovedFromDatabaseMessage.SendToInterestedListeners(prefabTag);
            }

            foreach (PrefabTag prefabTag in allPrefabTags)
            {
                UndoEx.DestroyObjectImmediate(prefabTag);
            }
        }

        public void RenamePrefabTag(PrefabTag prefabTag, string newName)
        {
            if (ContainsPrefabTag(prefabTag)) _prefabTags.RenameEntity(prefabTag, newName);
        }

        public void ActivateAllTags()
        {
            List<PrefabTag> allPrefabTags = _prefabTags.GetAllEntities();
            foreach (PrefabTag prefabTag in allPrefabTags)
            {
                prefabTag.IsActive = true;
            }
        }

        public void DeactivateAllTags()
        {
            List<PrefabTag> allPrefabTags = _prefabTags.GetAllEntities();
            foreach (PrefabTag prefabTag in allPrefabTags)
            {
                prefabTag.IsActive = false;
            }
        }

        public PrefabTag GetPrefabTagByIndex(int index)
        {
            return _prefabTags.GetEntityByIndex(index);
        }

        public PrefabTag GetPrefabTagByName(string name)
        {
            return _prefabTags.GetEntityByName(name);
        }

        public List<PrefabTag> GetAllPrefabTags()
        {
            return _prefabTags.GetAllEntities();
        }

        public List<PrefabTag> GetFilteredPrefabTags()
        {
            return PrefabTagFilter.GetFilteredPrefabTags(GetAllPrefabTags());
        }

        public List<string> GetAllPrefabTagNames()
        {
            return _prefabTags.GetAllEntityNames();
        }

        public bool ContainsNullEntries()
        {
            List<PrefabTag> allTags = GetAllPrefabTags();
            return allTags.FindAll(item => item == null).Count != 0;
        }

        public void RemoveNullEntries()
        {
            _prefabTags.RemoveNullEntries();
        }
        #endregion
    }
}
#endif