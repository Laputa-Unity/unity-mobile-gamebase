#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectScaleRandomization
    {
        #region Public Static Functions
        public static void Randomize(GameObject gameObject, ObjectScaleRandomizationSettings randomizationSettings)
        {
            if (randomizationSettings.RandomizeScale)
            {
                float randomUniformScale = UnityEngine.Random.Range(randomizationSettings.UniformScaleRandomizationSettings.MinScale,
                                                                    randomizationSettings.UniformScaleRandomizationSettings.MaxScale);
                gameObject.SetWorldScale(randomUniformScale);
            }
        }
        #endregion
    }
}
#endif