#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class SpriteRendererExtensions
    {
        #region Extension Methods
        public static Vector3 GetModelSpaceSize(this SpriteRenderer spriteRenderer)
        {
            return spriteRenderer.GetModelSpaceBox().Size;
        }

        public static Box GetModelSpaceBox(this SpriteRenderer spriteRenderer)
        {
            Sprite sprite = spriteRenderer.sprite;
            if (sprite == null) return Box.GetInvalid();

            List<Vector2> spriteVerts = new List<Vector2>(sprite.vertices);
            return Vector2Extensions.GetBoxFromPointCloud(spriteVerts);
        }

        // Works only when the Read/Write enabled flag is set inside the sprite texture properties. 
        // Otherwise, it will always return false.
        public static bool IsPixelFullyTransparent(this SpriteRenderer spriteRenderer, Vector3 worldPos)
        {
            Sprite sprite = spriteRenderer.sprite;
            if(sprite == null) return true;
            Texture2D spriteTexture = sprite.texture;
            if(spriteTexture == null) return true;

            Transform spriteTransform = spriteRenderer.transform;
            Vector3 modelSpacePos = spriteTransform.InverseTransformPoint(worldPos);

            Vector3 projectedPos = new Plane(Vector3.forward, 0.0f).ProjectPoint(modelSpacePos);
            Box modelSpaceBox = spriteRenderer.GetModelSpaceBox();
            modelSpaceBox.Size = new Vector3(modelSpaceBox.Size.x, modelSpaceBox.Size.y, 1.0f);
            if (!modelSpaceBox.ContainsPoint(projectedPos)) return true;

            List<Vector3> boxFaceCornerPoints = modelSpaceBox.GetBoxFaceCornerPoints(BoxFace.Front);
            Vector3 bottomLeft = boxFaceCornerPoints[(int)BoxFaceCornerPoint.BottomLeft];
            Vector3 fromTopLeftToPos = projectedPos - bottomLeft;

            Vector2 pixelCoords = new Vector2(fromTopLeftToPos.x * sprite.pixelsPerUnit, fromTopLeftToPos.y * sprite.pixelsPerUnit);
            pixelCoords += sprite.textureRectOffset;

            try
            {
                float alpha = spriteTexture.GetPixel((int)(pixelCoords.x + 0.5f), (int)(pixelCoords.y + 0.5f)).a;
                return alpha <= 1e-3f;
            }
            catch(UnityException e)
            {
                // Ternary operator needed to avoid 'variable not used' warning
                return e != null ? false : false;
            }
        }
        #endregion
    }
}
#endif