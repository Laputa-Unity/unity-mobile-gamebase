#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushDatabase : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private DecorPaintObjectPlacementBrushCollection _brushes = new DecorPaintObjectPlacementBrushCollection();

        [SerializeField]
        private DecorPaintObjectPlacementBrushDatabaseView _view;
        #endregion

        #region Public Properties
        public bool IsEmpty { get { return _brushes.IsEmpty; } }
        public int NumberOfBrushes { get { return _brushes.NumberOfEntities; } }
        public int IndexOfActiveBrush { get { return _brushes.IndexOfMarkedEntity; } }
        public DecorPaintObjectPlacementBrush ActiveBrush { get { return _brushes.MarkedEntity; } }
        public DecorPaintObjectPlacementBrushDatabaseView View { get { return _view; } }
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrushDatabase()
        {
            _view = new DecorPaintObjectPlacementBrushDatabaseView(this);
        }
        #endregion

        #region Public Static Functions
        public static DecorPaintObjectPlacementBrushDatabase Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.DecorPaintObjectPlacementBrushDatabase;
        }
        #endregion

        #region Public Methods
        public DecorPaintObjectPlacementBrush CreateBrush(string brushName)
        {
            if (!string.IsNullOrEmpty(brushName))
            {
                DecorPaintObjectPlacementBrush newBrush = DecorPaintObjectPlacementBrushFactory.Create(brushName, GetAllBrushNames());
                _brushes.AddEntity(newBrush);

                if (NumberOfBrushes == 1) SetActiveBrush(newBrush);

                return newBrush;
            }

            return null;
        }

        public bool ContainsBrush(DecorPaintObjectPlacementBrush brush)
        {
            return _brushes.ContainsEntity(brush);
        }

        public void RenameBrush(DecorPaintObjectPlacementBrush brush, string newName)
        {
            if (ContainsBrush(brush))
            {
                _brushes.RenameEntity(brush, newName);
            }
        }

        public void RemoveAndDestroyBrush(DecorPaintObjectPlacementBrush brush)
        {
            if (ContainsBrush(brush))
            {
                _brushes.RemoveEntity(brush);
                // ToDo: Send message that brush was removed 

                UndoEx.DestroyObjectImmediate(brush);
            }
        }

        public void RemoveAndDestroyAllBrushes()
        {
            List<DecorPaintObjectPlacementBrush> brushes = GetAllBrushes();
            foreach (DecorPaintObjectPlacementBrush brush in brushes)
            {
                _brushes.RemoveEntity(brush);
                // ToDo: Send message that brush was removed
            }

            foreach (DecorPaintObjectPlacementBrush brush in brushes)
            {
                UndoEx.DestroyObjectImmediate(brush);
            }
        }

        public int GetBrushIndex(DecorPaintObjectPlacementBrush brush)
        {
            return _brushes.GetEntityIndex(brush);
        }

        public DecorPaintObjectPlacementBrush GetBrushByIndex(int index)
        {
            return _brushes.GetEntityByIndex(index);
        }

        public DecorPaintObjectPlacementBrush GetBrushByName(string name)
        {
            return _brushes.GetEntityByName(name);
        }

        public List<DecorPaintObjectPlacementBrush> GetAllBrushes()
        {
            return _brushes.GetAllEntities();
        }

        public List<string> GetAllBrushNames()
        {
            return _brushes.GetAllEntityNames();
        }

        public void SetActiveBrush(DecorPaintObjectPlacementBrush newActiveBrush)
        {
            if (!ContainsBrush(newActiveBrush)) return;

            _brushes.MarkEntity(newActiveBrush);
        }

        public void RecordAllBrushesForUndo()
        {
            List<DecorPaintObjectPlacementBrush> allBrushes = GetAllBrushes();
            foreach(var brush in allBrushes)
            {
                UndoEx.RecordForToolAction(brush);
                brush.RecordAllElementsForUndo();
            }
        }

        public void RemovePrefabAssociationForAllBrushElements(Prefab prefab)
        {
            List<DecorPaintObjectPlacementBrush> allBrushes = GetAllBrushes();
            foreach (var brush in allBrushes)
            {
                brush.RemovePrefabAssociationForAllElements(prefab);
            }
        }

        public void RemovePrefabAssociationForAllBrushElements(List<Prefab> prefabs)
        {
            List<DecorPaintObjectPlacementBrush> allBrushes = GetAllBrushes();
            foreach (var brush in allBrushes)
            {
                brush.RemovePrefabAssociationForAllElements(prefabs);
            }
        }

        public void RemoveNullPrefabsFromAllBrushElements()
        {
            List<DecorPaintObjectPlacementBrush> allBrushes = GetAllBrushes();
            foreach(var brush in allBrushes)
            {
                brush.RemoveElementsWithNullPrefabs();
            }
        }
        #endregion
    }
}
#endif