#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectPlacementMissChance
    {
        #region Public Static Functions
        public static bool Missed(float missChance, float minMissChance, float maxMissChance)
        {
            float objectMissRandomValue = UnityEngine.Random.Range(minMissChance, maxMissChance);
            if (missChance != 0.0f && objectMissRandomValue <= missChance) return true;

            return false;
        }
        #endregion
    }
}
#endif