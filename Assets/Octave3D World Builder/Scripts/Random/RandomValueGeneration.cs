#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class RandomValueGeneration
    {
        #region Public Static Functions
        public static List<int> GenerateIntRandomValuesInRange(Range<int> range, int numberOfValuesToGenerate)
        {
            if (numberOfValuesToGenerate == 0) return new List<int>();

            var randomValues = new List<int>(numberOfValuesToGenerate);
            for(int valueIndex = 0; valueIndex < numberOfValuesToGenerate; ++valueIndex)
            {
                randomValues.Add(UnityEngine.Random.Range(range.Min, range.Max + 1));
            }

            return randomValues;
        }
        #endregion
    }
}
#endif