#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagAssociations : ScriptableObject, IMessageListener
    {
        #region Private Variables
        [NonSerialized]
        private Prefab _prefab;
      
        [SerializeField]
        private List<PrefabTag> _associatedTags = new List<PrefabTag>();
        [SerializeField]
        private PrefabTagFilter _prefabTagFilter;

        [SerializeField]
        private PrefabTagAssociationsView _view;
        #endregion

        #region Public Properties
        public int NumberOfAssociations { get { return _associatedTags.Count; } }
        public Prefab Prefab { get { return _prefab; } set { if (value != null) _prefab = value; } }
        public PrefabTagFilter PrefabTagFilter
        {
            get
            {
                if (_prefabTagFilter == null) _prefabTagFilter = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabTagFilter>();
                return _prefabTagFilter;
            }
        }
        #endregion

        #region Constructors
        public PrefabTagAssociations()
        {
            _view = new PrefabTagAssociationsView(this);
            MessageListenerDatabase.Instance.RegisterListenerForMessage(MessageType.PrefabTagWasRemovedFromDatabase, this);
        }
        #endregion

        #region Public Properties
        public PrefabTagAssociationsView View { get { return _view; } }
        #endregion

        #region Public Methods
        public void AssociateWithAllTagsInDatabase()
        {
            List<PrefabTag> allPrefabTags = PrefabTagDatabase.Get().GetAllPrefabTags();
            foreach (PrefabTag prefabTag in allPrefabTags)
            {
                Associate(prefabTag);
            }
        }

        public bool IsAnyAssociationActive()
        {
            foreach(PrefabTag prefabTag in _associatedTags)
            {
                if (prefabTag.IsActive) return true;
            }

            return false;
        }

        public bool AreAllTagAssociationsActive()
        {
            foreach (PrefabTag prefabTag in _associatedTags)
            {
                if (!prefabTag.IsActive) return false;
            }

            return true;
        }

        public void Associate(List<string> tagNames)
        {
            foreach(string tagName in tagNames)
            {
                Associate(tagName);
            }
        }

        public void Associate(string tagName)
        {
            PrefabTag prefabTagWithSpecifiedName = PrefabTagDatabase.Get().GetPrefabTagByName(tagName);
            if (prefabTagWithSpecifiedName != null) _associatedTags.Add(prefabTagWithSpecifiedName);           
        }

        public void Associate(List<PrefabTag> prefabTags)
        {
            foreach(PrefabTag prefabTag in prefabTags)
            {
                Associate(prefabTag);
            }
        }

        public void Associate(PrefabTag prefabTag)
        {
            Associate(prefabTag.Name);
        }

        public void RemoveAssociation(string tagName)
        {
            _associatedTags.RemoveAll(item => item.Name == tagName);
        }

        public void RemoveAssociation(PrefabTag prefabTag)
        {
            RemoveAssociation(prefabTag.Name);
        }

        public void RemoveAllAssociations()
        {
            List<string> allTagNames = GetAllAssociatedTagNames();
            foreach (string tagName in allTagNames)
            {
                RemoveAssociation(tagName);
            }
        }

        public void RemoveNullEntries()
        {
            _associatedTags.RemoveAll(item => item == null);
        }

        public void ToggleAssociation(string tagName)
        {
            if (!IsAssociatedWith(tagName)) Associate(tagName);
            else RemoveAssociation(tagName);
        }

        public void ToggleAssociation(PrefabTag prefabTag)
        {
            ToggleAssociation(prefabTag.Name);
        }

        public bool IsAssociatedWith(string tagName)
        {
            PrefabTag prefabTagWithSpecifiedName = PrefabTagDatabase.Get().GetPrefabTagByName(tagName);
            return _associatedTags.Contains(prefabTagWithSpecifiedName);
        }

        public bool IsAssociatedWith(PrefabTag prefabTag)
        {
            return IsAssociatedWith(prefabTag.Name);
        }

        public List<string> GetAllAssociatedTagNames()
        {
            return (from prefabTag in _associatedTags select prefabTag.Name).ToList();
        }
        #endregion

        #region Message Handlers
        public void RespondToMessage(Message message)
        {
            switch(message.Type)
            {
                case MessageType.PrefabTagWasRemovedFromDatabase:

                    RespondToMessage(message as PrefabTagWasRemovedFromDatabaseMessage);
                    break;
            }
        }

        private void RespondToMessage(PrefabTagWasRemovedFromDatabaseMessage message)
        {
            // Note: If we don't do this here, when a tag is removed, its prefab associations will be lost.
            UndoEx.RecordForToolAction(this);
            _associatedTags.Remove(message.PrefabTag);
        }
        #endregion

        #region Private Methods
        private void OnDestroy()
        {
            if (_prefabTagFilter != null) UndoEx.DestroyObjectImmediate(_prefabTagFilter);
            MessageListenerDatabase.Instance.UnregisterListener(this);
        }
        #endregion
    }
}
#endif