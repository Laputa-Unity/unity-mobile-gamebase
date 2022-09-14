#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectGroupView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectGroup _objectGroup;
        #endregion

        #region Constructors
        public ObjectGroupView(ObjectGroup objectGroup)
        {
            _objectGroup = objectGroup;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderNameChangeTextField();
        }
        #endregion

        #region Private Methods
        private void RenderNameChangeTextField()
        {
            string newString = EditorGUILayoutEx.DelayedTextField(GetContentForNameChangeTextField(), _objectGroup.Name);
            if(newString != _objectGroup.Name)
            {
                UndoEx.RecordForToolAction(_objectGroup);
                _objectGroup.Name = newString;
            }
        }

        private GUIContent GetContentForNameChangeTextField()
        {
            var content = new GUIContent();
            content.text = "Name";
            content.tooltip = "Allows you to change the name of the group.";

            return content;
        }
        #endregion
    }
}
#endif