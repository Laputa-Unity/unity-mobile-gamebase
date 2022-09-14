#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectCollectionMask : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private SerializableGameObjectHashSet _gameObjects = new SerializableGameObjectHashSet();

        [SerializeField]
        private ObjectCollectionMaskView _view;
        #endregion

        #region Public Properties
        public ObjectCollectionMaskView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectCollectionMask()
        {
            _view = new ObjectCollectionMaskView(this);
        }
        #endregion

        #region Public Methods
        public bool IsMasked(GameObject gameObject)
        {
            return _gameObjects.Contains(gameObject);
        }

        public void Mask(GameObject gameObject)
        {
            if(gameObject != null) _gameObjects.Add(gameObject);
        }

        public void Mask(List<GameObject> gameObjects)
        {
            foreach(GameObject gameObject in gameObjects)
            {
                Mask(gameObject);
            }
        }

        public void Unmask(GameObject gameObject)
        {
            _gameObjects.Remove(gameObject);
        }

        public void Unmask(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                Unmask(gameObject);
            }
        }

        public void UnmaskAll()
        {
            _gameObjects.Clear();
        }

        public List<GameObject> GetAllMaskedGameObjects()
        {
            _gameObjects.HashSet.RemoveWhere(item => item == null);
            return new List<GameObject>(_gameObjects.HashSet);
        }

        public void RemoveNullEntries()
        {
            _gameObjects.RemoveNullEntries();
        }
        #endregion
    }
}
#endif