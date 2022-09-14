#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class PrefabTagAssociationsFactory
    {
        #region Public Static Functions
        public static PrefabTagAssociations Create(Prefab prefab)
        {
            if(prefab != null)
            {
                PrefabTagAssociations tagAssociations = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabTagAssociations>();
                tagAssociations.Prefab = prefab;

                return tagAssociations;
            }

            return null;
        }
        #endregion
    }
}
#endif