#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    // WARNING: CAN NOT BE SERIALIZED...
    [Serializable]
    public class ProbabilityTableEntry<EntityType> where EntityType : class
    {
        [SerializeField]
        public EntityType Entity;
        [SerializeField]
        public float Probability;
        [SerializeField]
        public float NormProbability;
        [SerializeField]
        public float CumulativeProbability;
    }

    [Serializable]
    public class ProbabilityTable<EntityType> where EntityType : class
    {
        [SerializeField]
        private List<ProbabilityTableEntry<EntityType>> _entries = new List<ProbabilityTableEntry<EntityType>>();

        public void AddEntity(EntityType entity, float probability)
        {
            if (entity == null || ContainsEntity(entity)) return;

            var entry = new ProbabilityTableEntry<EntityType>();
            entry.Entity = entity;
            entry.Probability = probability;
            _entries.Add(entry);
        }

        public bool ContainsEntity(EntityType entity)
        {
            return _entries.FindAll(item => item.Entity == entity).Count != 0;
        }

        public void SetEntityProbability(EntityType entity, float probability)
        {
            var entries = _entries.FindAll(item => item.Entity == entity);
            if (entries.Count == 0) return;

            entries[0].Probability = probability;
        }

        public EntityType PickEntity(float randomNumber)
        {
            foreach(var entry in _entries)
            {
                if (entry.CumulativeProbability >= randomNumber) return entry.Entity;
            }

            return null;
        }

        public void RemoveEntity(EntityType entity)
        {
            _entries.RemoveAll(item => item.Entity == entity);
        }

        public void RemoveAllEntities()
        {
            _entries.Clear();
        }

        public void Rebuild()
        {
            CalculateNormProbabilities();
            SortEntriesByNormProbability();
            CalculateCumulProbabilities();
        }

        private void CalculateNormProbabilities()
        {
            float sum = GetProbabilitySum();
            foreach(var entry in _entries)
            {
                entry.NormProbability = entry.Probability / sum;
            }
        }

        private float GetProbabilitySum()
        {
            float sum = 0.0f;
            foreach(var entry in _entries)
            {
                sum += entry.Probability;
            }

            return sum;
        }

        private void SortEntriesByNormProbability()
        {
            _entries.Sort(delegate(ProbabilityTableEntry<EntityType> e0, ProbabilityTableEntry<EntityType> e1)
            {
                return e0.NormProbability.CompareTo(e1.NormProbability);
            });
        }

        private void CalculateCumulProbabilities()
        {
            for(int eIndex = 0; eIndex < _entries.Count; ++eIndex)
            {
                var previous = eIndex > 0 ? _entries[eIndex - 1] : null;
                var current = _entries[eIndex];

                current.CumulativeProbability = current.NormProbability;
                if (previous != null) current.CumulativeProbability += previous.CumulativeProbability;
            }
        }
    }
}
#endif