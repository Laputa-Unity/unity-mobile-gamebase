#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class DecorBrushElementSpawnChanceTable : ProbabilityTable<DecorPaintObjectPlacementBrushElement> { }

    [Serializable]
    public class DecorPaintObjectPlacementBrush : ScriptableObject, INamedEntity, IMessageListener
    {
        #region Private Variables
        [SerializeField]
        private int _activeElementIndex = -1;

        [SerializeField]
        private string _name = "";

        [SerializeField]
        private float _radius = 1.5f;
        [SerializeField]
        private int _maxNumberOfObjects = 10;
        [SerializeField]
        private float _distanceBetweenObjects = 0.5f;
        [SerializeField]
        private bool _ignoreObjectsOutsideOfPaintSurface = true;

        [SerializeField]
        private List<DecorPaintObjectPlacementBrushElement> _elements = new List<DecorPaintObjectPlacementBrushElement>();
        [SerializeField]
        private PrefabCategory _destinationCategoryForElementPrefabs;

        [SerializeField]
        private DecorPaintObjectPlacementBrushView _view;
        #endregion

        #region Public Static Properties
        public static float MinRadius { get { return 0.1f; } }
        public static int MinObjectAmount { get { return 1; } }
        public static float MinObjectScatter { get { return 0.0f; } }
        #endregion

        #region Public Properties
        public int ActiveElementIndex { get { return _activeElementIndex; } }
        public DecorPaintObjectPlacementBrushElement ActiveElement { get { return _activeElementIndex >= 0 ? _elements[_activeElementIndex] : null; } }
        public string Name { get { return _name; } set { if (!string.IsNullOrEmpty(value)) _name = value; } }
        public float Radius { get { return _radius; } set { _radius = Mathf.Max(value, MinRadius); } }
        public int MaxNumberOfObjects { get { return _maxNumberOfObjects; } set { _maxNumberOfObjects = Mathf.Max(value, MinObjectAmount); } }
        public float DistanceBetweenObjects { get { return _distanceBetweenObjects; } set { _distanceBetweenObjects = value; } }
        public bool IgnoreObjectsOutsideOfPaintSurface { get { return _ignoreObjectsOutsideOfPaintSurface; } set { _ignoreObjectsOutsideOfPaintSurface = value; } }
        public int NumberOfElements { get { return _elements.Count; } }
        public bool IsEmpty { get { return NumberOfElements == 0; } }
        public PrefabCategory DestinationCategoryForElementPrefabs
        {
            get
            {
                if (_destinationCategoryForElementPrefabs == null) _destinationCategoryForElementPrefabs = PrefabCategoryDatabase.Get().GetDefaultPrefabCategory();
                return _destinationCategoryForElementPrefabs;
            }
            set
            {
                if (value != null) _destinationCategoryForElementPrefabs = value;
            }
        }
        public DecorPaintObjectPlacementBrushView View { get { return _view; } }
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrush()
        {
            _view = new DecorPaintObjectPlacementBrushView(this);
        }
        #endregion

        #region Public Methods
        public DecorBrushElementSpawnChanceTable CalculateElementSpawnChanceTable(bool ignoreDisabledElements)
        {
            var table = new DecorBrushElementSpawnChanceTable();

            if (ignoreDisabledElements)
            {
                foreach (var element in _elements)
                {
                    if (element.IsEnabled) table.AddEntity(element, element.SpawnChance);
                }
            }
            else foreach (var element in _elements) table.AddEntity(element, element.SpawnChance);

            table.Rebuild();
            return table;
        }

        public void LoadAllPrefabsInActiveCategory()
        {
            PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
            if(activePrefabCategory != null)
            {
                List<Prefab> allPrefabsInCatgory = activePrefabCategory.GetAllPrefabs();
                foreach(var prefab in allPrefabsInCatgory)
                {
                    if(GetElementWithPrefab(prefab) == null)
                    {
                        var element = CreateNewElement();
                        element.Prefab = prefab;
                    }
                }
            }
        }

        public void RemoveAndDestroyElement(DecorPaintObjectPlacementBrushElement element)
        {
            if(ContainsElement(element) && element != null)
            {
                _elements.Remove(element);
                ClampActiveElementIndex();
                UndoEx.DestroyObjectImmediate(element);
            }
        }

        public bool ContainsElement(DecorPaintObjectPlacementBrushElement element)
        {
            return _elements.Contains(element);
        }

        public DecorPaintObjectPlacementBrushElement GetElementWithPrefab(Prefab prefab)
        {
            foreach(var element in _elements)
            {
                if (element.Prefab == prefab) return element;
            }

            return null;
        }

        public void RemoveAndDestroyAllElements()
        {
            var allElements = GetAllBrushElements();
            _elements.Clear();

            _activeElementIndex = -1;
            foreach (var element in allElements)
            {
                if (element != null)
                    UndoEx.DestroyObjectImmediate(element);
            }
        }

        public DecorPaintObjectPlacementBrushElement CreateNewElement()
        {
            DecorPaintObjectPlacementBrushElement newElement = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<DecorPaintObjectPlacementBrushElement>();
            newElement.ParentBrush = this;
            _elements.Add(newElement);

            if (_activeElementIndex < 0) SetActiveElement(newElement);

            return newElement;
        }

        public DecorPaintObjectPlacementBrushElement GetBrushElementByIndex(int elementIndex)
        {
            return _elements[elementIndex];
        }

        public List<DecorPaintObjectPlacementBrushElement> GetAllBrushElements()
        {
            return new List<DecorPaintObjectPlacementBrushElement>(_elements);
        }

        public List<DecorPaintObjectPlacementBrushElement> GetAllValidAndActiveBrushElements()
        {
            return _elements.FindAll(item => item != null && item.IsValid() && item.IsEnabled);
        }

        public List<Vector3> GetPrefabWorldScaleForAllValidAndActiveBrushElements()
        {
            if (IsEmpty) return new List<Vector3>();

            List<DecorPaintObjectPlacementBrushElement> allValidAndActiveBrushElements = GetAllValidAndActiveBrushElements();
            if (allValidAndActiveBrushElements.Count == 0) return new List<Vector3>();

            var prefabWorldScaleValues = new List<Vector3>(allValidAndActiveBrushElements.Count);
            for(int validElementIndex = 0; validElementIndex < allValidAndActiveBrushElements.Count; ++validElementIndex)
            {
                prefabWorldScaleValues.Add(allValidAndActiveBrushElements[validElementIndex].Prefab.UnityPrefab.transform.lossyScale);
            }

            return prefabWorldScaleValues;
        }

        public List<OrientedBox> GetPrefabWorldOrientedBoxesForAllValidAndActiveBrushElements()
        {
            if (IsEmpty) return new List<OrientedBox>();

            List<DecorPaintObjectPlacementBrushElement> allValidAndActiveBrushElements = GetAllValidAndActiveBrushElements();
            if (allValidAndActiveBrushElements.Count == 0) return new List<OrientedBox>();

            var prefabOrientedBoxes = new List<OrientedBox>(allValidAndActiveBrushElements.Count);
            for (int validElementIndex = 0; validElementIndex < allValidAndActiveBrushElements.Count; ++validElementIndex)
            {
                prefabOrientedBoxes.Add(allValidAndActiveBrushElements[validElementIndex].Prefab.UnityPrefab.GetHierarchyWorldOrientedBox());
            }

            return prefabOrientedBoxes;
        }

        public void RecordAllElementsForUndo()
        {
            foreach(var element in _elements)
            {
                UndoEx.RecordForToolAction(element);
            }
        }

        public void RemovePrefabAssociationForAllElements(Prefab prefab)
        {
            foreach(var element in _elements)
            {
                if (element.Prefab == prefab) element.Prefab = null;
            }
        }

        public void RemovePrefabAssociationForAllElements(List<Prefab> prefabs)
        {
            foreach (var element in _elements)
            {
                if (prefabs.Contains(element.Prefab)) element.Prefab = null;
            }
        }

        public void SetActiveElement(DecorPaintObjectPlacementBrushElement newActiveElement)
        {
            if (newActiveElement == null) _activeElementIndex = -1;
            else
            {
                int newActiveIndex = _elements.FindIndex(item => item == newActiveElement);
                if (newActiveIndex >= 0) _activeElementIndex = newActiveIndex;
            }
        }

        public void RemoveElementsWithNullPrefabs()
        {
            _elements.RemoveAll(item => item.Prefab == null || item.Prefab.UnityPrefab == null);
        }
        #endregion

        #region Private Methods
        private void OnDestroy()
        {
            RemoveAndDestroyAllElements();
        }

        private void OnEnable()
        {
            MessageListenerDatabase.Instance.RegisterListenerForMessage(MessageType.PrefabWasRemovedFromCategory, this);
        }

        private void ClampActiveElementIndex()
        {
            if (NumberOfElements != 0) _activeElementIndex = Mathf.Clamp(_activeElementIndex, 0, NumberOfElements - 1);
            else _activeElementIndex = -1;
        }
        #endregion

        public void RespondToMessage(Message message)
        {
            switch(message.Type)
            {
                case MessageType.PrefabWasRemovedFromCategory:

                    RespondToMessage(message as PrefabWasRemovedFromCategoryMessage);
                    break;
            }
        }

        private void RespondToMessage(PrefabWasRemovedFromCategoryMessage message)
        {
            var elementWithSamePrefab = GetElementWithPrefab(message.PrefabWhichWasRemoved);
            if (elementWithSamePrefab != null)
            {
                UndoEx.RecordForToolAction(this);
                UndoEx.RecordForToolAction(elementWithSamePrefab);
                RemoveAndDestroyElement(elementWithSamePrefab);
            }
        }
    }
}
#endif