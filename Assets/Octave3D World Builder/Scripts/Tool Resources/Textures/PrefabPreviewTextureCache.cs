#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class PrefabPreviewTextureCache
    {
        #region Private Variables
        private Dictionary<Prefab, Texture2D> _prefabToPreviewTexture = new Dictionary<Prefab, Texture2D>();
        #endregion

        public int NumPreviews { get { return _prefabToPreviewTexture.Count; } }

        #region Public Static Functions
        public static PrefabPreviewTextureCache Get()
        {
            if (Octave3DWorldBuilder.ActiveInstance == null) return null;
            return Octave3DWorldBuilder.ActiveInstance.ToolResources.PrefabPreviewTextureCache;
        }
        #endregion

        #region Public Methods
        public bool IsPreviewTextureAvailableForPrefab(Prefab prefab)
        {
            return _prefabToPreviewTexture.ContainsKey(prefab);
        }

        public Texture2D GetPrefabPreviewTexture(Prefab prefab)
        {
            if (IsPreviewTextureAvailableForPrefab(prefab)) return _prefabToPreviewTexture[prefab];
            return GeneratePrefabPreviewTextureAndStore(prefab);
        }

        public void DisposeTextures()
        {
            foreach (var pair in _prefabToPreviewTexture)
            {
                if (pair.Value != null) Octave3DWorldBuilder.DestroyImmediate(pair.Value, true);
            }
            _prefabToPreviewTexture.Clear();
        }

        public void DestroyTexturesForNullPrefabEntries()
        {
            Dictionary<Prefab, Texture2D> newPrefabToPreviewTexture = GenerateNewDictionaryExcludingPairsWithNullPrefabReferences();

            _prefabToPreviewTexture.Clear();
            _prefabToPreviewTexture = newPrefabToPreviewTexture;
        }

        public void GeneratePreviewForAllPrefabCategories(bool showProgress)
        {
            PrefabPreviewGenerator.Get().BeginPreviewGenSession();
            if (!showProgress)
            {
                PrefabCategoryDatabase categoryDatabase = PrefabCategoryDatabase.Get();
                List<PrefabCategory> allCategories = categoryDatabase.GetAllPrefabCategories();
                foreach(var category in allCategories)
                {
                    int numPrefabs = category.NumberOfPrefabs;
                    if (numPrefabs == 0) continue;

                    List<Prefab> allPrefabs = category.GetAllPrefabs();
                    foreach(var prefab in allPrefabs)
                    {
                        if (prefab == null || prefab.UnityPrefab == null || IsPreviewTextureAvailableForPrefab(prefab)) continue;
                        Texture2D prefabPreview = PrefabPreviewGenerator.Get().GeneratePreview(prefab);
                        if (prefabPreview != null) _prefabToPreviewTexture.Add(prefab, prefabPreview);
                    }
                }
            }
            else
            {
                PrefabCategoryDatabase categoryDatabase = PrefabCategoryDatabase.Get();
                List<PrefabCategory> allCategories = categoryDatabase.GetAllPrefabCategories();
                foreach (var category in allCategories)
                {
                    int numPrefabs = category.NumberOfPrefabs;
                    if (numPrefabs == 0) continue;

                    float invPrefabCount = 1.0f / numPrefabs;
                    List<Prefab> allPrefabs = category.GetAllPrefabs();
                    for(int prefabIndex = 0; prefabIndex < allPrefabs.Count; ++prefabIndex)
                    {
                        Prefab prefab = allPrefabs[prefabIndex];
                        if (prefab == null || prefab.UnityPrefab == null || IsPreviewTextureAvailableForPrefab(prefab)) continue;
                   
                        EditorUtility.DisplayProgressBar("Generating prefab previews (" + category.Name + ")", "Prefab: " + prefab.UnityPrefab.name, (float)(prefabIndex + 1) * invPrefabCount);
                        Texture2D prefabPreview = PrefabPreviewGenerator.Get().GeneratePreview(prefab);
                        if (prefabPreview != null) _prefabToPreviewTexture.Add(prefab, prefabPreview);
                    }
                }
                EditorUtility.ClearProgressBar();
            }
            PrefabPreviewGenerator.Get().EndPreviewGenSession();
        }

        public void GeneratePreviewsForPrefabCollection(List<Prefab> prefabs, bool showProgress)
        {
            int numPrefabs = prefabs.Count;
            if (numPrefabs == 0) return;

            PrefabPreviewGenerator.Get().BeginPreviewGenSession();
            if(!showProgress)
            {
                foreach (var prefab in prefabs)
                {
                    if (prefab == null || prefab.UnityPrefab == null || IsPreviewTextureAvailableForPrefab(prefab)) continue;
                    Texture2D prefabPreview = PrefabPreviewGenerator.Get().GeneratePreview(prefab);
                    if (prefabPreview != null) _prefabToPreviewTexture.Add(prefab, prefabPreview);
                }
            }
            else
            {
                float invPrefabCount = 1.0f / numPrefabs;
                for(int prefabIndex = 0; prefabIndex < numPrefabs; ++prefabIndex)
                {
                    Prefab prefab = prefabs[prefabIndex];
                    if (prefab == null || prefab.UnityPrefab == null || IsPreviewTextureAvailableForPrefab(prefab)) continue;
                    EditorUtility.DisplayProgressBar("Generating prefab previews...", "Prefab: " + prefab.UnityPrefab.name, (float)(prefabIndex + 1) * invPrefabCount);
                    Texture2D prefabPreview = PrefabPreviewGenerator.Get().GeneratePreview(prefab);
                    if (prefabPreview != null) _prefabToPreviewTexture.Add(prefab, prefabPreview);
                }
                EditorUtility.ClearProgressBar();
            }
            PrefabPreviewGenerator.Get().EndPreviewGenSession();
        }
        #endregion

        #region Private Methods
        private Texture2D GeneratePrefabPreviewTextureAndStore(Prefab prefab)
        {
            PrefabPreviewGenerator.Get().BeginPreviewGenSession();
            Texture2D prefabPreview = PrefabPreviewGenerator.Get().GeneratePreview(prefab);
            PrefabPreviewGenerator.Get().EndPreviewGenSession();
            if (prefabPreview != null) _prefabToPreviewTexture.Add(prefab, prefabPreview);

            return prefabPreview;
        }

       /* private Texture2D ClonePrefabPreviewAndStore(Prefab prefab, Texture2D prefabPreview)
        {
            if (prefabPreview != null)
            {
                Texture2D clonedPreviewTexture = prefabPreview.Clone(true);
                if (clonedPreviewTexture != null)
                {
                    _prefabToPreviewTexture.Add(prefab, clonedPreviewTexture);
                    return clonedPreviewTexture;
                }
            }

            return null;
        }*/

        private Dictionary<Prefab, Texture2D> GenerateNewDictionaryExcludingPairsWithNullPrefabReferences()
        {
            var newPrefabPreviewTextureDictionary = new Dictionary<Prefab, Texture2D>();
            foreach (KeyValuePair<Prefab, Texture2D> pair in _prefabToPreviewTexture)
            {
                if ((pair.Key == null || pair.Key.UnityPrefab == null) && pair.Value != null) Octave3DWorldBuilder.DestroyImmediate(pair.Value);
                else newPrefabPreviewTextureDictionary.Add(pair.Key, pair.Value);
            }

            return newPrefabPreviewTextureDictionary;
        }
        #endregion
    }
}
#endif