using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class UniqueEntityNameGenerator
    {
        #region Public Static Functions
        public static string GenerateUniqueName(string desiredEntityName, List<string> existingEntityNames)
        {
            string entityName = desiredEntityName;
            int numberSuffix = 0;
            while(existingEntityNames.Contains(entityName))
            {
                entityName = desiredEntityName + numberSuffix;
                ++numberSuffix;
            }

            return entityName;
        }
        #endregion
    }
}
