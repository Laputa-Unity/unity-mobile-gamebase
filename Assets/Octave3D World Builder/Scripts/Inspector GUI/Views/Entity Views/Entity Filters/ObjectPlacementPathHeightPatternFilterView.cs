#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightPatternFilterView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathHeightPatternFilter _heightPatternFilter;
        #endregion

        #region Constructors
        public ObjectPlacementPathHeightPatternFilterView(ObjectPlacementPathHeightPatternFilter heightPatternFilter)
        {
            _heightPatternFilter = heightPatternFilter;
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _heightPatternFilter.NameFilter.View.Render();
            RenderPropertyToggleControls();
        }
        #endregion

        #region Private Methods
        private void RenderPropertyToggleControls()
        {
            RenderOnlyActivePropertyToggle();
        }

        private void RenderOnlyActivePropertyToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForOnlyActivePropertyToggle(), _heightPatternFilter.OnlyActive.IsActive);
            if(newBool != _heightPatternFilter.OnlyActive.IsActive)
            {
                UndoEx.RecordForToolAction(_heightPatternFilter);
                _heightPatternFilter.OnlyActive.SetActive(newBool);
            }
        }

        private GUIContent GetContentForOnlyActivePropertyToggle()
        {
            var content = new GUIContent();
            content.text = "Only active";
            content.tooltip = "Allow only the active pattern.";

            return content;
        }
        #endregion
    }
}
#endif