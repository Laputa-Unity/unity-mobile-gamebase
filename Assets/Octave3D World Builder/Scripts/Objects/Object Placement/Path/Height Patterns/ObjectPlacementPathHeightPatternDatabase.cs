#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightPatternDatabase : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectPlacementPathHeightPatternCollection _pathHeightPatterns = new ObjectPlacementPathHeightPatternCollection();

        [SerializeField]
        private ObjectPlacementPathHeightPatternFilter _heightPatternFilter;

        [SerializeField]
        private ObjectPlacementPathHeightPatternDatabaseView _view;
        #endregion

        #region Public Properties
        public bool IsEmpty { get { return _pathHeightPatterns.IsEmpty; } }
        public int NumberOfPatterns { get { return _pathHeightPatterns.NumberOfEntities; } }
        public int IndexOfActivePattern { get { return _pathHeightPatterns.IndexOfMarkedEntity; } }
        public ObjectPlacementPathHeightPattern ActivePattern { get { return _pathHeightPatterns.MarkedEntity; } }
        public ObjectPlacementPathHeightPatternFilter HeightPatternFilter
        {
            get
            {
                if (_heightPatternFilter == null) _heightPatternFilter = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementPathHeightPatternFilter>();
                return _heightPatternFilter;
            }
        }
        public ObjectPlacementPathHeightPatternDatabaseView View { get { return _view; } }

        #endregion

        #region Constructors
        public ObjectPlacementPathHeightPatternDatabase()
        {
            _view = new ObjectPlacementPathHeightPatternDatabaseView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectPlacementPathHeightPatternDatabase Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ObjectPlacementPathHeightPatternDatabase;
        }
        #endregion

        #region Public Methods
        public ObjectPlacementPathHeightPattern CreateHeightPattern(string patternName)
        {
            if (!string.IsNullOrEmpty(patternName))
            {
                ObjectPlacementPathHeightPattern newHeightPattern = ObjectPlacementPathHeightPatternFactory.Create(patternName, GetAllHeightPatternNames());
                _pathHeightPatterns.AddEntity(newHeightPattern);

                if (NumberOfPatterns == 1) SetActivePattern(newHeightPattern);

                return newHeightPattern;
            }

            return null;
        }

        public void AddPattern(ObjectPlacementPathHeightPattern pathHeightPattern)
        {
            if (!ContainsPattern(pathHeightPattern)) _pathHeightPatterns.AddEntity(pathHeightPattern);
        }

        public bool ContainsPattern(ObjectPlacementPathHeightPattern pathHeightPattern)
        {
            return _pathHeightPatterns.ContainsEntity(pathHeightPattern);
        }

        public void RenamePattern(ObjectPlacementPathHeightPattern pathHeightPattern, string newName)
        {
            if (ContainsPattern(pathHeightPattern))
            {
                _pathHeightPatterns.RenameEntity(pathHeightPattern, newName);
            }
        }

        public void RemoveAndDestroyPattern(ObjectPlacementPathHeightPattern pathHeightPattern)
        {
            if (ContainsPattern(pathHeightPattern))
            {
                _pathHeightPatterns.RemoveEntity(pathHeightPattern);
                ObjectPlacementPathHeightPatternWasRemovedFromDatabaseMessage.SendToInterestedListeners(pathHeightPattern);

                UndoEx.DestroyObjectImmediate(pathHeightPattern);
            }
        }

        public void RemoveAndDestroyAllPatterns()
        {
            List<ObjectPlacementPathHeightPattern> patterns = GetAllPatterns();
            foreach (ObjectPlacementPathHeightPattern pattern in patterns)
            {
                _pathHeightPatterns.RemoveEntity(pattern);
                ObjectPlacementPathHeightPatternWasRemovedFromDatabaseMessage.SendToInterestedListeners(pattern);
            }

            foreach (ObjectPlacementPathHeightPattern pattern in patterns)
            {
                UndoEx.DestroyObjectImmediate(pattern);
            }
        }

        public int GetPatternIndex(ObjectPlacementPathHeightPattern pathHeightPattern)
        {
            return _pathHeightPatterns.GetEntityIndex(pathHeightPattern);
        }

        public ObjectPlacementPathHeightPattern GetPatternByIndex(int index)
        {
            return _pathHeightPatterns.GetEntityByIndex(index);
        }

        public List<ObjectPlacementPathHeightPattern> GetAllPatterns()
        {
            return _pathHeightPatterns.GetAllEntities();
        }

        public List<ObjectPlacementPathHeightPattern> GetFilteredPatterns()
        {
            return HeightPatternFilter.GetFilteredPatterns(GetAllPatterns());
        }

        public List<string> GetAllHeightPatternNames()
        {
            return _pathHeightPatterns.GetAllEntityNames();
        }

        public void SetActivePattern(ObjectPlacementPathHeightPattern newActivePattern)
        {
            if (!ContainsPattern(newActivePattern)) return;

            _pathHeightPatterns.MarkEntity(newActivePattern);
            NewObjectPlacementPathHeightPatternWasActivatedMessage.SendToInterestedListeners(newActivePattern);
        }
        #endregion
    }
}
#endif