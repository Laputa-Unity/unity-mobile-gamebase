#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class Texture2DExtensions
    {
        #region Extension Methods
        public static Texture2D Clone(this Texture2D textureToClone, bool useLinearColorSpace)
        {
            Color32[] texturePixels = textureToClone.GetPixels32();

            Texture2D clonedTexture = new Texture2D(textureToClone.width, textureToClone.height, TextureFormat.ARGB32, true, useLinearColorSpace);
            clonedTexture.SetPixels32(texturePixels);
            clonedTexture.Apply();

            return clonedTexture;
        }
        #endregion
    }
}
#endif