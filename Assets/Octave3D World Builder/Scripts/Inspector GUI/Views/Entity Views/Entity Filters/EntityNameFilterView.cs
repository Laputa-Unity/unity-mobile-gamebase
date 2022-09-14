#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class EntityNameFilterView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private EntityNameFilter _entityNameFilter;      
        #endregion

        #region Constructors
        public EntityNameFilterView(EntityNameFilter entityNameFilter)
        {
            _entityNameFilter = entityNameFilter;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderNameFilterTextField();
        }
        #endregion

        #region Private Methods
        private void RenderNameFilterTextField()
        {
            string newString = EditorGUILayout.TextField(GetContentForLayerNameFilterTextField(), _entityNameFilter.NameFilter);
            if (newString != _entityNameFilter.NameFilter)
            {
                UndoEx.RecordForToolAction(_entityNameFilter);
                _entityNameFilter.NameFilter = newString;
            }
        }
        private GUIContent GetContentForLayerNameFilterTextField()
        {
            var content = new GUIContent();
            content.text = "Name filter";
            content.tooltip = "Allow only entities whose names match the name written in this field.";

            return content;
        }
        #endregion
    }
}
#endif