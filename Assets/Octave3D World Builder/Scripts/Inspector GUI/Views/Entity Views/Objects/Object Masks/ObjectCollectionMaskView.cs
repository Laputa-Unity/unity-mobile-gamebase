#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectCollectionMaskView : EntityView
    {
        #region Private Variable
        [NonSerialized]
        private ObjectCollectionMask _mask;

        [SerializeField]
        private ObjectCollectionMaskViewData _viewData;
        #endregion

        #region Private Properties
        private ObjectCollectionMaskViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectCollectionMaskViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Constructors
        public ObjectCollectionMaskView(ObjectCollectionMask mask)
        {
            _mask = mask;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            EditorGUILayoutEx.BeginVerticalBox();
            RenderObjectCollectionMaskScrollView();
            RenderActionButtons();
            EditorGUILayoutEx.EndVerticalBox();
        }
        #endregion

        #region Private Methods
        private void RenderObjectCollectionMaskScrollView()
        {
            ViewData.ObjectCollectionViewPosition = EditorGUILayout.BeginScrollView(ViewData.ObjectCollectionViewPosition, GetStyleForObjectCollectionMaskScrollView(), GUILayout.Height(ViewData.ObjectCollectionScrollViewHeight));
            RenderObjectCollectionMaskEntries();
            EditorGUILayout.EndScrollView();
        }

        private GUIStyle GetStyleForObjectCollectionMaskScrollView()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private void RenderObjectCollectionMaskEntries()
        {
            List<GameObject> maskedObjects = _mask.GetAllMaskedGameObjects();
            foreach (GameObject gameObject in maskedObjects) RenderObjectMaskEntry(gameObject);
        }

        private void RenderObjectMaskEntry(GameObject gameObject)
        {
            EditorGUILayout.BeginHorizontal();
            RenderObjectNameLabel(gameObject);
            RenderUnmaskObjectButton(gameObject);
            EditorGUILayout.EndHorizontal();
        }

        private void RenderObjectNameLabel(GameObject gameObject)
        {
            EditorGUILayout.LabelField(GetContentForObjectNameLabel(gameObject));
        }

        private GUIContent GetContentForObjectNameLabel(GameObject gameObject)
        {
            var content = new GUIContent();
            content.text = gameObject.name;
            content.tooltip = "";

            return content;
        }

        private void RenderUnmaskObjectButton(GameObject gameObject)
        {
           if(GUILayout.Button(GetContentForUnmaskObjectButton()))
           {
               UndoEx.RecordForToolAction(_mask);
               _mask.Unmask(gameObject);
           }
        }

        private GUIContent GetContentForUnmaskObjectButton()
        {
            var content = new GUIContent();
            content.text = "Unmask";
            content.tooltip = "Unmasks the object.";

            return content;
        }

        private void RenderActionButtons()
        {
            RenderUnmaskAllButton();
        }

        private void RenderUnmaskAllButton()
        {
            if(GUILayout.Button(GetContentForUnmaskAllButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_mask);
                _mask.UnmaskAll();
            }
        }

        private GUIContent GetContentForUnmaskAllButton()
        {
            var content = new GUIContent();
            content.text = "Unmask all";
            content.tooltip = "Unmasks all objects.";

            return content;
        }
        #endregion
    }
}
#endif