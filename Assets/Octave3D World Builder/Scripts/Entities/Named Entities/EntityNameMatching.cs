#if UNITY_EDITOR
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    public static class EntityNameMatching
    {
        #region Public Static Functions
        public static List<INamedEntity> GetEntitiesWithMatchingNames(List<INamedEntity> namedEntities, string nameToMatch, bool useCaseSensitiveMatch)
        {
            if (namedEntities == null || namedEntities.Count == 0 || nameToMatch == null) return new List<INamedEntity>();

            var matchOperation = EntityNameMatchOperationFactory.Create(useCaseSensitiveMatch);
            return matchOperation.GetEntitiesWithMatchingNames(namedEntities, nameToMatch);          
        }
        #endregion
    }
}
#endif