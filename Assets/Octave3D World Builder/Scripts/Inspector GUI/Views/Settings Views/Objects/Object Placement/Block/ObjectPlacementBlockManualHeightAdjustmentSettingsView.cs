#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockManualHeightAdjustmentSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementBlockManualHeightAdjustmentSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementBlockManualHeightAdjustmentSettingsView(ObjectPlacementBlockManualHeightAdjustmentSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderRaiseAmountField();
            RenderLowerAmountField();
        }
        #endregion

        #region Private Methods
        private void RenderRaiseAmountField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForRaiseAmountField(), _settings.RaiseAmount);
            if(newInt != _settings.RaiseAmount)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RaiseAmount = newInt;
            }
        }

        private GUIContent GetContentForRaiseAmountField()
        {
            var content = new GUIContent();
            content.text = "Raise amount";
            content.tooltip = "Controls the amount by which the block is raised manually.";

            return content;
        }

        private void RenderLowerAmountField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForLowerAmountField(), _settings.LowerAmount);
            if (newInt != _settings.LowerAmount)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.LowerAmount = newInt;
            }
        }

        private GUIContent GetContentForLowerAmountField()
        {
            var content = new GUIContent();
            content.text = "Lower amount";
            content.tooltip = "Controls the amount by which the block is lowered manually.";

            return content;
        }
        #endregion
    }
}
#endif