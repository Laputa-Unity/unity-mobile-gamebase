#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class CaseInsensitiveEntityNameMatchOperation : IEntityNameMatchOperation
    {
        #region Public Methods
        public List<INamedEntity> GetEntitiesWithMatchingNames(List<INamedEntity> namedEntities, string nameToMatch)
        {
            var foundEntities = new List<INamedEntity>(namedEntities.Count);
            nameToMatch = nameToMatch.ToLower();

            List<INamedEntity> extactMatchEntities = namedEntities.FindAll(item => item.Name.ToLower() == nameToMatch);
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
                string entityName = namedEntity.Name.ToLower();
                if (entityName != nameToMatch)
                {
                    if (entityName.Contains(nameToMatch)) destination.Add(namedEntity);
                }
            }
        }
        #endregion
    }
}
#endif