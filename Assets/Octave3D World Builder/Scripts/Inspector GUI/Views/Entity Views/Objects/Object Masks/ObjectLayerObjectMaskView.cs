#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectLayerObjectMaskView : EntityView
    {
        #region Private Variable
        [NonSerialized]
        private ObjectLayerObjectMask _mask;

        [SerializeField]
        private ObjectLayerObjectMaskViewData _viewData;
        #endregion

        #region Private Properties
        private ObjectLayerObjectMaskViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectLayerObjectMaskViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Constructors
        public ObjectLayerObjectMaskView(ObjectLayerObjectMask mask)
        {
            _mask = mask;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            EditorGUILayoutEx.BeginVerticalBox();
            RenderLayerMaskScrollView();
            RenderActionButtons();
            EditorGUILayoutEx.EndVerticalBox();
        }
        #endregion

        #region Private Methods
        private void RenderLayerMaskScrollView()
        {
            ViewData.LayerScrollViewPosition = EditorGUILayout.BeginScrollView(ViewData.LayerScrollViewPosition, GetStyleForLayerMaskScrollView(), GUILayout.Height(ViewData.LayerScrollViewHeight));
            RenderLayerMaskEntries();
            EditorGUILayout.EndScrollView();
        }

        private GUIStyle GetStyleForLayerMaskScrollView()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private void RenderLayerMaskEntries()
        {
            List<string> allLayerNames = LayerExtensions.GetAllAvailableLayerNames();
            foreach (string layerName in allLayerNames) RenderLayerMaskEntry(layerName);
        }

        private void RenderLayerMaskEntry(string layerName)
        {
            RenderLayerMaskEntryToggle(layerName);
        }

        private void RenderLayerMaskEntryToggle(string layerName)
        {
            int layerNumber = LayerMask.NameToLayer(layerName);
            bool isLayerMasked = _mask.IsMasked(layerNumber);

            bool newBool = EditorGUILayout.ToggleLeft(GetContentForLayerMaskEntryToggle(layerName), isLayerMasked);
            if(newBool != isLayerMasked)
            {
                UndoEx.RecordForToolAction(_mask);
                if (newBool) _mask.Mask(layerNumber);
                else _mask.Unmask(layerNumber);
            }
        }

        private GUIContent GetContentForLayerMaskEntryToggle(string layerName)
        {
            var content = new GUIContent();
            content.text = layerName;
            content.tooltip = "If this is checked, the layer is masked. Uncheck this to unmask the layer.";

            return content;
        }

        private void RenderActionButtons()
        {
            EditorGUILayout.BeginHorizontal();
            RenderMaskAllButton();
            RenderUnmaskAllButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderMaskAllButton()
        {
            if(GUILayout.Button(GetContentForMaskAllButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_mask);
                _mask.MaskAll();
            }
        }

        private GUIContent GetContentForMaskAllButton()
        {
            var content = new GUIContent();
            content.text = "Mask all";
            content.tooltip = "Masks all layers.";

            return content;
        }

        private void RenderUnmaskAllButton()
        {
            if (GUILayout.Button(GetContentForUnmaskAllButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_mask);
                _mask.UnmaskAll();
            }
        }

        private GUIContent GetContentForUnmaskAllButton()
        {
            var content = new GUIContent();
            content.text = "Unmask all";
            content.tooltip = "Unmasks all layers.";

            return content;
        }
        #endregion
    }
}
#endif