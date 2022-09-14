#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class SerializableDictionary<SerializedKeyType, SerializedValueType>
        : ISerializationCallbackReceiver
    {
        #region Private Variables
        private Dictionary<SerializedKeyType, SerializedValueType> _dictionary = new Dictionary<SerializedKeyType, SerializedValueType>();

        [SerializeField]
        private List<SerializedKeyType> _serializedKeys = new List<SerializedKeyType>();
        [SerializeField]
        private List<SerializedValueType> _serializedValues = new List<SerializedValueType>();
        #endregion

        #region Public Properties
        public Dictionary<SerializedKeyType, SerializedValueType> Dictionary { get { return _dictionary; } }
        #endregion

        #region Public Methods
        public void OnBeforeSerialize()
        {
            RemoveNullKeys();
            _serializedKeys.Clear();
            _serializedValues.Clear();

            // Store the dictionary keys and values inside the serialized lists
            foreach (var keyValuePair in _dictionary)
            {
                _serializedKeys.Add(keyValuePair.Key);
                _serializedValues.Add(keyValuePair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<SerializedKeyType, SerializedValueType>();

            // Use the serialized lists to populate the new dictionary
            int numberOfKeyValuePairs = Math.Min(_serializedKeys.Count, _serializedValues.Count);
            for (int keyValuePairIndex = 0; keyValuePairIndex < numberOfKeyValuePairs; keyValuePairIndex++)
            {
                _dictionary.Add(_serializedKeys[keyValuePairIndex], _serializedValues[keyValuePairIndex]);
            }

            _serializedKeys.Clear();
            _serializedValues.Clear();
        }

        public void RemoveNullKeys()
        {
            _dictionary = (from keyValuePair in _dictionary
                           where !EqualityComparer<SerializedKeyType>.Default.Equals(keyValuePair.Key, default(SerializedKeyType))
                           select keyValuePair).ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
        }
        #endregion
    }
}
#endif