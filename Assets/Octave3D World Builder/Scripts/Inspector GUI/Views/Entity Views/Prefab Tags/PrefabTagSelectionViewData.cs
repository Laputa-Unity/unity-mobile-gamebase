#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabTagSelectionViewData : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private SerializableStringHashSet _namesOfSelectedTags = new SerializableStringHashSet();
        [SerializeField]
        private bool _hasSelectionChanged = false;
        [SerializeField]
        private Vector2 _prefabTagScrollPosition = Vector2.zero;
        #endregion

        #region Public Static Properties
        public static float PrefabTagScrollViewHeight { get { return 200.0f; } }
        #endregion

        #region Public Properties
        public List<string> ListOfSelectedTagNames { get { return new List<string>(_namesOfSelectedTags.HashSet); } set { if (value != null) _namesOfSelectedTags.FromEnumerable(value); } }
        public bool HasSelectionChanged { get { return _hasSelectionChanged; } set { _hasSelectionChanged = value; } }
        public Vector2 PrefabTagScrollPosition { get { return _prefabTagScrollPosition; } set { _prefabTagScrollPosition = value; } }
        #endregion

        #region Public Methods
        public bool IsTagSelected(string tagName)
        {
            return _namesOfSelectedTags.Contains(tagName);
        }

        public void AddTagNameToTagSelection(string tagName)
        {
            _namesOfSelectedTags.Add(tagName);
        }

        public void RemoveTagNameFromTagSelection(string tagName)
        {
            _namesOfSelectedTags.Remove(tagName);
        }

        public void RemoveTagNameByPredicate(Predicate<string> predicate)
        {
            _namesOfSelectedTags.RemoveWhere(predicate);
        }

        public void ClearSelectedTagNames()
        {
            _namesOfSelectedTags.Clear();
        }
        #endregion
    }
}
#endif