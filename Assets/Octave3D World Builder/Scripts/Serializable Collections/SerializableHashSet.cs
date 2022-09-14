#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class SerializableHashSet<SerializedType> : ISerializationCallbackReceiver
        where SerializedType : class
    {
        #region Private Variables
        private HashSet<SerializedType> _hashSet = new HashSet<SerializedType>();

        [SerializeField]
        private List<SerializedType> _serializedList = new List<SerializedType>();
        #endregion

        #region Public Properties
        public HashSet<SerializedType> HashSet { get { return _hashSet; } }
        public int Count { get { return HashSet.Count; } }
        #endregion

        #region Public Methods
        public void OnBeforeSerialize()
        {
            RemoveNullEntries();
            _serializedList.Clear();

            // Store the hash set values in the serialized list
            foreach (SerializedType serializedEntity in _hashSet)
            {
                _serializedList.Add(serializedEntity);
            }
        }

        public void OnAfterDeserialize()
        {
            // Create the hash set based on the serialized list
            _hashSet = new HashSet<SerializedType>(_serializedList);
            _serializedList.Clear();
        }

        public void RemoveNullEntries()
        {
            _hashSet.RemoveWhere(item => EqualityComparer<SerializedType>.Default.Equals(item, default(SerializedType)));
        }

        public bool Contains(SerializedType entity)
        {
            return _hashSet.Contains(entity);
        }

        public void Add(SerializedType entity)
        {
            _hashSet.Add(entity);
        }

        public void Remove(SerializedType entity)
        {
            _hashSet.Remove(entity);
        }

        public void RemoveWhere(Predicate<SerializedType> match)
        {
            _hashSet.RemoveWhere(match);
        }

        public void Clear()
        {
            _hashSet.Clear();
        }

        public void FromEnumerable(IEnumerable<SerializedType> entities)
        {
            _hashSet = new HashSet<SerializedType>(entities);
        }
        #endregion
    }
}
#endif
