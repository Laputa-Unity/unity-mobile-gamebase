#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public class TextureCache
    {
        #region Private Variables
        private Dictionary<string, Texture2D> _pathToTexture = new Dictionary<string, Texture2D>(StringComparer.Ordinal);
        #endregion

        #region Public Static Functions
        public static TextureCache Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ToolResources.TextureCache;
        }
        #endregion

        #region Public Methods
        public bool IsTextureAvailableForRelativePath(string relativeTexturePath)
        {
            return _pathToTexture.ContainsKey(relativeTexturePath);
        }

        public Texture2D GetTextureAtRelativePath(string relativeTexturePath)
        {
            if (IsTextureAvailableForRelativePath(relativeTexturePath)) return _pathToTexture[relativeTexturePath];
            return LoadTextureAtPathAndStore(relativeTexturePath);
        }

        public void DisposeTextures()
        {
            foreach (var pair in _pathToTexture)
            {
                if (pair.Value != null) Octave3DWorldBuilder.DestroyImmediate(pair.Value, true);
            }
            _pathToTexture.Clear();
        }
        #endregion

        #region Private Methods
        private Texture2D LoadTextureAtPathAndStore(string relativeTexturePath)
        {
            Texture2D loadedTexture = ProjectAssetDatabase.LoadTextureAtPath(FileSystem.GetToolFolderName() + relativeTexturePath);
            if (loadedTexture != null)
            {
                Texture2D clonedTexture = loadedTexture.Clone(PlayerSettings.colorSpace != ColorSpace.Linear);
                _pathToTexture.Add(relativeTexturePath, clonedTexture);

                return clonedTexture;
            }

            return null;
        }
        #endregion
    }
}
#endif