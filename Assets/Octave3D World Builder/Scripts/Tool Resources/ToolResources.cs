#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ToolResources
    {
        #region Private Variables
        private ProceduralMeshResources _meshResources = new ProceduralMeshResources();
        private PrefabPreviewTextureCache _prefabPreviewTextureCache = new PrefabPreviewTextureCache();
        private TextureCache _textureCache = new TextureCache();
        #endregion

        #region Public Properties
        public ProceduralMeshResources MeshResources { get { return _meshResources; } }
        public PrefabPreviewTextureCache PrefabPreviewTextureCache { get { return _prefabPreviewTextureCache; } }
        public TextureCache TextureCache { get { return _textureCache; } }
        #endregion

        #region Public Methods
        public void DisposeResources()
        {
            _meshResources.DisposeMeshes();
            _prefabPreviewTextureCache.DisposeTextures();
            _textureCache.DisposeTextures();
        }
        #endregion
    }
}
#endif