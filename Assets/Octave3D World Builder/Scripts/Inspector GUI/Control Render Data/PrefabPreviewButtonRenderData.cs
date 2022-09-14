#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace O3DWB
{
    public class PrefabPreviewButtonRenderData
    {
        #region Private Variables
        private float _buttonWidth;
        private float _buttonHeight;
        private GUIContent _buttonContent;
        #endregion

        #region Public Properties
        public float ButtonWidth { get { return _buttonWidth; } }
        public float ButtonHeight { get { return _buttonHeight; } }
        public GUIContent ButtonContent { get { return _buttonContent; } }
        #endregion

        #region Public Methods
        public void ExtractFromPrefab(Prefab prefab, float previewScale)
        {
            Texture2D previewTexture = PrefabPreviewTextureCache.Get().GetPrefabPreviewTexture(prefab);

            // Establish size
            _buttonWidth = (previewTexture != null ? previewTexture.width : EditorGUILayoutEx.DefaultPrefabPreviewSize) * previewScale;
            _buttonHeight = (previewTexture != null ? previewTexture.height : EditorGUILayoutEx.DefaultPrefabPreviewSize) * previewScale;

            // Establish GUI content
            _buttonContent = new GUIContent();
            _buttonContent.image = previewTexture;
            _buttonContent.tooltip = prefab.Name;
        }
        #endregion
    }
}
#endif