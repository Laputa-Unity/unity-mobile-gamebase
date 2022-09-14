#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockSubdivisionSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementBlockSubdivisionSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementBlockSubdivisionSettingsView(ObjectPlacementBlockSubdivisionSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderUseSubdivisionToggle();
            if (_settings.UseSubdivision)
            {
                RenderSubdivisionPropertyControls();
            }
        }
        #endregion

        #region Private Methods
        private void RenderUseSubdivisionToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseSubdivisionToggle(), _settings.UseSubdivision);
            if(newBool != _settings.UseSubdivision)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseSubdivision = newBool;
            }
        }

        private GUIContent GetContentForUseSubdivisionToggle()
        {
            var content = new GUIContent();
            content.text = "Use subdivision";
            content.tooltip = "If this is checked, the block will be subdivided in sub-blocks of a specified size along the 3 axes.";

            return content;
        }

        private void RenderSubdivisionPropertyControls()
        {
            RenderSubdivideAlongExtensionRightToggle();
            RenderSubdivisionSizeAlongExtensionRightField();
            RenderSubdivisionGapSizeAlongExtensionRightField();

            RenderSubdivideAlongExtensionLookToggle();
            RenderSubdivisionSizeAlongExtensionLookField();
            RenderSubdivisionGapSizeAlongExtensionLookField();

            RenderSubdivideAlongGrowDirectionToggle();
            RenderSubdivisionSizeAlongGrowDirectionField();
            RenderSubdivisionGapSizeAlongGrowDirectionField();
        }

        private void RenderSubdivideAlongExtensionRightToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSubdivideAlongExtensionRightToggle(), _settings.SubdivideAlongExtensionRight);
            if(newBool != _settings.SubdivideAlongExtensionRight)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SubdivideAlongExtensionRight = newBool;
            }
        }

        private GUIContent GetContentForSubdivideAlongExtensionRightToggle()
        {
            var content = new GUIContent();
            content.text = "Subdivide along extension right";
            content.tooltip = "Use this toggle to turn subdivision on or off along the block's extension plane right axis.";

            return content;
        }

        private void RenderSubdivisionSizeAlongExtensionRightField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForSubdivisionSizeAlongExtensionRightField(), _settings.SubdivisionSizeAlongExtensionRight);
            if (newInt != _settings.SubdivisionSizeAlongExtensionRight)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SubdivisionSizeAlongExtensionRight = newInt;
            }
        }

        private GUIContent GetContentForSubdivisionSizeAlongExtensionRightField()
        {
            var content = new GUIContent();
            content.text = "Subdivision size along extension right";
            content.tooltip = "The subdivision size along the block's extension plane right axis.";

            return content;
        }

        private void RenderSubdivisionGapSizeAlongExtensionRightField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForSubdivisionGapSizeAlongExtensionRightField(), _settings.SubdivisionGapSizeAlongExtensionRight);
            if (newInt != _settings.SubdivisionGapSizeAlongExtensionRight)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SubdivisionGapSizeAlongExtensionRight = newInt;
            }
        }

        private GUIContent GetContentForSubdivisionGapSizeAlongExtensionRightField()
        {
            var content = new GUIContent();
            content.text = "Gap size along extension right";
            content.tooltip = "The gap size between subdivisions along the block's extension plane right axis.";

            return content;
        }

        private void RenderSubdivideAlongExtensionLookToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSubdivideAlongExtensionLookToggle(), _settings.SubdivideAlongExtensionLook);
            if (newBool != _settings.SubdivideAlongExtensionLook)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SubdivideAlongExtensionLook = newBool;
            }
        }

        private GUIContent GetContentForSubdivideAlongExtensionLookToggle()
        {
            var content = new GUIContent();
            content.text = "Subdivide along extension look";
            content.tooltip = "Use this toggle to turn subdivision on or off along the block's extension plane look axis.";

            return content;
        }

        private void RenderSubdivisionGapSizeAlongExtensionLookField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForSubdivisionGapSizeAlongExtensionLookField(), _settings.SubdivisionGapSizeAlongExtensionLook);
            if (newInt != _settings.SubdivisionGapSizeAlongExtensionLook)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SubdivisionGapSizeAlongExtensionLook = newInt;
            }
        }

        private GUIContent GetContentForSubdivisionGapSizeAlongExtensionLookField()
        {
            var content = new GUIContent();
            content.text = "Gap size along extension look";
            content.tooltip = "The gap size between subdivisions along the block's extension plane look axis.";

            return content;
        }

        private void RenderSubdivisionSizeAlongExtensionLookField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForSubdivisionSizeAlongExtensionLookField(), _settings.SubdivisionSizeAlongExtensionLook);
            if (newInt != _settings.SubdivisionSizeAlongExtensionLook)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SubdivisionSizeAlongExtensionLook = newInt;
            }
        }

        private GUIContent GetContentForSubdivisionSizeAlongExtensionLookField()
        {
            var content = new GUIContent();
            content.text = "Subdivision size along extension look";
            content.tooltip = "The subdivision size along the block's extension plane look axis.";

            return content;
        }

        private void RenderSubdivideAlongGrowDirectionToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSubdivideAlongGrowDirectionToggle(), _settings.SubdivideAlongGrowDirection);
            if (newBool != _settings.SubdivideAlongGrowDirection)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SubdivideAlongGrowDirection = newBool;
            }
        }

        private GUIContent GetContentForSubdivideAlongGrowDirectionToggle()
        {
            var content = new GUIContent();
            content.text = "Subdivide along grow direction";
            content.tooltip = "Use this toggle to turn subdivision on or off along the block's grow direction.";

            return content;
        }

        private void RenderSubdivisionSizeAlongGrowDirectionField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForSubdivisionSizeAlongGrowDirectionField(), _settings.SubdivisionSizeAlongGrowDirection);
            if (newInt != _settings.SubdivisionSizeAlongGrowDirection)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SubdivisionSizeAlongGrowDirection = newInt;
            }
        }

        private GUIContent GetContentForSubdivisionSizeAlongGrowDirectionField()
        {
            var content = new GUIContent();
            content.text = "Subdivision size along grow direction";
            content.tooltip = "The subdivision size along the block's grow direction.";

            return content;
        }

        private void RenderSubdivisionGapSizeAlongGrowDirectionField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForSubdivisionGapSizeAlongGrowDirectionField(), _settings.SubdivisionGapSizeAlongGrowDirection);
            if (newInt != _settings.SubdivisionGapSizeAlongGrowDirection)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SubdivisionGapSizeAlongGrowDirection = newInt;
            }
        }

        private GUIContent GetContentForSubdivisionGapSizeAlongGrowDirectionField()
        {
            var content = new GUIContent();
            content.text = "Gap size along grow direction";
            content.tooltip = "The gap size between subdivisions along the block's grow direction.";

            return content;
        }
        #endregion
    }
}
#endif