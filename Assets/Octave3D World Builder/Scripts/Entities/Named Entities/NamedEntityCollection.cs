#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class NamedEntityCollection<EntityType> where EntityType : class, INamedEntity
    {
        #region Protected Variables
        [SerializeField]
        protected List<EntityType> _namedEntities = new List<EntityType>();
        #endregion

        #region Public Properties
        public bool IsEmpty { get { return NumberOfEntities == 0; } }
        public int NumberOfEntities { get { return _namedEntities.Count; } }
        #endregion

        #region Public Methods
        public bool ContainsEntity(EntityType namedEntity)
        {
            return _namedEntities.Contains(namedEntity);
        }

        public bool ContainsEntityWithName(string entityName)
        {
            List<EntityType> entitiesWithSameName = _namedEntities.FindAll(item => item.Name == entityName);
            return entitiesWithSameName != null && entitiesWithSameName.Count > 0;
        }

        public bool ContainsEntityByPredicate(Predicate<EntityType> predicate)
        {
            List<EntityType> foundEntities = GetEntitiesByPredicate(predicate);
            return foundEntities.Count != 0;
        }

        public void AddEntity(EntityType namedEntity)
        {
            if (CanEntityBeAddedToCollection(namedEntity)) _namedEntities.Add(namedEntity);
        }

        public virtual void RemoveEntity(EntityType namedEntity)
        {
            _namedEntities.Remove(namedEntity);
        }

        public virtual void RemoveEntityAtIndex(int indexOfEntityToRemove)
        {
            _namedEntities.RemoveAt(indexOfEntityToRemove);
        }

        public virtual void RemoveAllEntities()
        {
            _namedEntities.Clear();
        }

        public void RemoveWithPredicate(Predicate<EntityType> predicate)
        {
            _namedEntities.RemoveAll(predicate);
        }

        public void RenameEntity(EntityType entity, string newName)
        {
            if(!string.IsNullOrEmpty(newName))
            {
                entity.Name = UniqueEntityNameGenerator.GenerateUniqueName(newName, GetAllEntityNames());
            }
        }

        public EntityType GetEntityByIndex(int index)
        {
            return _namedEntities[index];
        }

        public EntityType GetEntityByName(string name)
        {
            List<EntityType> entitiesWithName = _namedEntities.FindAll(item => item.Name == name);
            if (entitiesWithName.Count != 0) return entitiesWithName[0];

            return null;
        }

        public EntityType GetEntityByPredicate(Predicate<EntityType> predicate)
        {
            List<EntityType> entities = _namedEntities.FindAll(predicate);
            if (entities.Count != 0) return entities[0];

            return null;
        }

        public List<EntityType> GetEntitiesByPredicate(Predicate<EntityType> predicate)
        {
            return _namedEntities.FindAll(predicate);
        }

        public int GetEntityIndex(EntityType entity)
        {
            return _namedEntities.FindIndex(item => ReferenceEquals(item, entity));
        }

        public List<EntityType> GetAllEntities()
        {
            return new List<EntityType>(_namedEntities);
        }

        public void GetAllEntities(List<EntityType> entities)
        {
            entities.Clear();
            entities.AddRange(_namedEntities);
        }

        public List<string> GetAllEntityNames()
        {
            return (from entity in _namedEntities select entity.Name).ToList();
        }

        public void GetAllEntityNames(List<string> names)
        {
            names.Clear();
            foreach (var entity in _namedEntities)
                names.Add(entity.Name);
        }

        public void RemoveNullEntries()
        {
            _namedEntities.RemoveAll(item => EqualityComparer<EntityType>.Default.Equals(item, default(EntityType)));
        }
        #endregion

        #region Private Methods
        private bool CanEntityBeAddedToCollection(EntityType namedEntity)
        {
            return !ContainsEntity(namedEntity);
        }
        #endregion
    }
}
#endif