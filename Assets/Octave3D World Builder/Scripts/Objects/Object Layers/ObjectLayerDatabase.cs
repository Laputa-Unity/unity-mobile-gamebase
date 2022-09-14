#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectLayerDatabase : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private int _activeLayer = LayerExtensions.GetDefaultLayer();
        [SerializeField]
        private ObjectLayerDatabaseView _view;
        #endregion

        #region Public Properties
        public int ActiveLayer
        {
            get { return _activeLayer; }
            set
            {
                if (LayerExtensions.IsLayerNumberValid(value)) _activeLayer = value;
            }
        }
        public ObjectLayerDatabaseView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectLayerDatabase()
        {
            _view = new ObjectLayerDatabaseView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectLayerDatabase Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ObjectLayerDatabase;
        }
        #endregion

        #region Public Methods
        public List<GameObject> GetAllGameObjectsInLayer(int objectLayer)
        {
            List<GameObject> allWorkingObjects = Octave3DWorldBuilder.ActiveInstance.GetAllWorkingObjects();
            return allWorkingObjects.FindAll(item => item.layer == objectLayer);
        }

        public List<GameObject> GetAllGameObjectsInAllLayers()
        {
            return Octave3DWorldBuilder.ActiveInstance.GetAllWorkingObjects();
        }

        public List<int> GetAllObjectLayers()
        {
            return LayerExtensions.GetAllAvailableLayers();
        }

        public void AssignObjectsToLayer(List<GameObject> gameObjects, int objectLayer)
        {
            GameObjectExtensions.AssignGameObjectsToLayer(gameObjects, objectLayer, true);
        }

        public void RemoveObjectsFromLayer(List<GameObject> gameObjects, int objectLayer)
        {
            if (objectLayer == LayerExtensions.GetDefaultLayer()) return;

            List<GameObject> gameObjectsInLayer = gameObjects.FindAll(item => item.layer == objectLayer);
            GameObjectExtensions.AssignGameObjectsToLayer(gameObjectsInLayer, LayerExtensions.GetDefaultLayer(), true);
        }

        public void MakeLayerDynamic(int objectLayer)
        {
            List<GameObject> allGameObjectsInLayer = GetAllGameObjectsInLayer(objectLayer);
            foreach(GameObject gameObject in allGameObjectsInLayer)
            {
                UndoEx.RecordForToolAction(gameObject);
                gameObject.isStatic = false;
            }
        }

        public void MakeAllLayersDynamic()
        {
            List<int> allLayers = GetAllObjectLayers();
            foreach(int layer in allLayers)
            {
                MakeLayerDynamic(layer);
            }
        }

        public void MakeLayerStatic(int objectLayer)
        {
            List<GameObject> allGameObjectsInLayer = GetAllGameObjectsInLayer(objectLayer);
            foreach (GameObject gameObject in allGameObjectsInLayer)
            {
                UndoEx.RecordForToolAction(gameObject);
                gameObject.isStatic = true;
            }
        }

        public void MakeAllLayersStatic()
        {
            List<int> allLayers = GetAllObjectLayers();
            foreach (int layer in allLayers)
            {
                MakeLayerStatic(layer);
            }
        }

        public void HideLayer(int objectLayer)
        {
            List<GameObject> allGameObjectsInLayer = GetAllGameObjectsInLayer(objectLayer);
            foreach(GameObject gameObject in allGameObjectsInLayer)
            {
                UndoEx.RecordForToolAction(gameObject);
                gameObject.SetActive(false);
            }
        }

        public void HideAllLayers()
        {
            List<int> allLayers = GetAllObjectLayers();
            foreach (int layer in allLayers)
            {
                HideLayer(layer);
            }
        }

        public void ShowLayer(int objectLayer)
        {
            List<GameObject> allGameObjectsInLayer = GetAllGameObjectsInLayer(objectLayer);
            foreach (GameObject gameObject in allGameObjectsInLayer)
            {
                UndoEx.RecordForToolAction(gameObject);
                gameObject.SetActive(true);
            }
        }

        public void ShowAllLayers()
        {
            List<int> allLayers = GetAllObjectLayers();
            foreach (int layer in allLayers)
            {
                ShowLayer(layer);
            }
        }
        #endregion
    }
}
#endif