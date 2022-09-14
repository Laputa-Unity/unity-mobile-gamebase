#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class CaseSensitiveEntityNameMatchOperation : IEntityNameMatchOperation
    {
        #region Public Methods
        public List<INamedEntity> GetEntitiesWithMatchingNames(List<INamedEntity> namedEntities, string nameToMatch)
        {
            var foundEntities = new List<INamedEntity>(namedEntities.Count);

            List<INamedEntity> extactMatchEntities = namedEntities.FindAll(item => item.Name == nameToMatch);
            foundEntities.AddRange(extactMatchEntities);

            StorePartialMatches(foundEntities, namedEntities, nameToMatch);

            return foundEntities;
        }
        #endregion

        #region Private Methods
        private void StorePartialMatches(List<INamedEntity> destination, List<INamedEntity> namedEntities, string nameToMatch)
        {
            foreach (INamedEntity namedEntity in namedEntities)
            {
                if (namedEntity.Name != nameToMatch)
                {
                    if (nameToMatch.Contains(namedEntity.Name)) destination.Add(namedEntity);
                }
            }
        }
        #endregion
    }
}
#endif