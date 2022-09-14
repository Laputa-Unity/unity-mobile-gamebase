#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSnapSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSnapSettings _settings;
        #endregion

        #region Constructors
        public ObjectSnapSettingsView(ObjectSnapSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Snap Settings";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderUseOriginalPivotToggle();
            RenderIgnoreObjectsWhenSnappingToggle();
            RenderSnapToCursorHitPointToggle();

            if(!_settings.UseOriginalPivot)
            {
                RenderSnapCenterToCenterForXZGridToggle();
                RenderSnapCenterToCenterForObjectSurfaceToggle();
            }

            EditorGUILayout.Separator();
            RenderEnableObjectToObjectSnapToggle();
            RenderObjectToObjectSnapEpsilonField();

            EditorGUILayout.Separator();
            RenderXZGridXOffset();
            RenderXZGridZOffset();

            EditorGUILayout.Separator();
            RenderXZSnapGridYOffsetField();
            RenderXZGridYOffsetStep();
            _settings.ObjectColliderSnapSurfaceGridSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderUseOriginalPivotToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForUseOriginalPivotToggle(), _settings.UseOriginalPivot);
            if (newBool != _settings.UseOriginalPivot)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UseOriginalPivot = newBool;
            }
        }

        private GUIContent GetContentForUseOriginalPivotToggle()
        {
            var content = new GUIContent();
            content.text = "Use original pivot";
            content.tooltip = "If this is checked, the tool will snap using the pivot defined in the modelling package.";

            return content;
        }

        private void RenderIgnoreObjectsWhenSnappingToggle()
        {
            var content = new GUIContent();
            content.text = "Enable object surface grid";
            content.tooltip = "If this is checked, you will be able to snap across the surfaces of other objects using a surface snap grid.";

            bool newBool = EditorGUILayout.ToggleLeft(content, _settings.EnableObjectSurfaceGrid);
            if(newBool != _settings.EnableObjectSurfaceGrid)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EnableObjectSurfaceGrid = newBool;
                SceneView.RepaintAll();
            }
        }

        private void RenderSnapToCursorHitPointToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSnapToCursorHitPointToggle(), _settings.SnapToCursorHitPoint);
            if (newBool != _settings.SnapToCursorHitPoint)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapToCursorHitPoint = newBool;
            }
        }

        private GUIContent GetContentForSnapToCursorHitPointToggle()
        {
            var content = new GUIContent();
            content.text = "Snap to cursor hit point";
            content.tooltip = "If this is checked, the active pivot point will be snapped to the intersection point between the mouse cursor and the hovered surface.";

            return content;
        }

        private void RenderSnapCenterToCenterForXZGridToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSnapCenterToCenterForXZGridToggle(), _settings.SnapCenterToCenterForXZGrid);
            if (newBool != _settings.SnapCenterToCenterForXZGrid)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapCenterToCenterForXZGrid = newBool;
            }
        }

        private GUIContent GetContentForSnapCenterToCenterForXZGridToggle()
        {
            var content = new GUIContent();
            content.text = "Snap center to center (grid)";
            content.tooltip = "If this is checked, the tool will always snap the center pivot point to the center of the hovered XZ grid cell.";

            return content;
        }

        private void RenderSnapCenterToCenterForObjectSurfaceToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForSnapCenterToCenterForObjectSurfaceToggle(), _settings.SnapCenterToCenterForObjectSurface);
            if (newBool != _settings.SnapCenterToCenterForObjectSurface)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SnapCenterToCenterForObjectSurface = newBool;
            }
        }

        private GUIContent GetContentForSnapCenterToCenterForObjectSurfaceToggle()
        {
            var content = new GUIContent();
            content.text = "Snap center to center (object surface)";
            content.tooltip = "If this is checked, the tool will always snap the center pivot point to the center of the hovered object surface.";

            return content;
        }

        private void RenderXZGridXOffset()
        {
            var content = new GUIContent();
            content.text = "Snap grid X offset";
            content.tooltip = "Allows you to control the snap grid's X offset.";

            float newOffset = EditorGUILayout.FloatField(content, _settings.XZSnapGridXOffset);
            if (newOffset != _settings.XZSnapGridXOffset)
            {
                _settings.XZSnapGridXOffset = newOffset;
                SceneView.RepaintAll();
            }
        }

        private void RenderXZGridZOffset()
        {
            var content = new GUIContent();
            content.text = "Snap grid Z offset";
            content.tooltip = "Allows you to control the snap grid's Z offset.";

            float newOffset = EditorGUILayout.FloatField(content, _settings.XZSnapGridZOffset);
            if (newOffset != _settings.XZSnapGridZOffset)
            {
                _settings.XZSnapGridZOffset = newOffset;
                SceneView.RepaintAll();
            }
        }

        private void RenderXZSnapGridYOffsetField()
        {
            float newOffset = EditorGUILayout.FloatField(GetContentForXZSnapGridYOffsetField(), _settings.XZSnapGridYOffset);
            if (newOffset != _settings.XZSnapGridYOffset)
            {
                _settings.XZSnapGridYOffset = newOffset;
                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForXZSnapGridYOffsetField()
        {
            var content = new GUIContent();
            content.text = "Snap grid Y offset";
            content.tooltip = "Allows you to control the snap grid's Y offset.";

            return content;
        }

        private void RenderXZGridYOffsetStep()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForXZGridYOffsetStep(), _settings.XZGridYOffsetStep);
            if(newFloat != _settings.XZGridYOffsetStep)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.XZGridYOffsetStep = newFloat;
            }
        }

        private GUIContent GetContentForXZGridYOffsetStep()
        {
            var content = new GUIContent();
            content.text = "Snap grid Y offset step";
            content.tooltip = "The amount by which the grid Y position is adjusted up or down when using the shortcut keys.";

            return content;
        }

        private void RenderEnableObjectToObjectSnapToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForEnableObjectToObjectSnapToggle(), _settings.EnableObjectToObjectSnap);
            if(newBool != _settings.EnableObjectToObjectSnap)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EnableObjectToObjectSnap = newBool;
                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForEnableObjectToObjectSnapToggle()
        {
            var content = new GUIContent();
            content.text = "Enable object to object snap";
            content.tooltip = "If this is checked, the tool will snap the object placement guide to nearby objects based on a specified object snap epsilon.";

            return content;
        }

        private void RenderObjectToObjectSnapEpsilonField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForObjectToObjectSnapEpsilonField(), _settings.ObjectToObjectSnapEpsilon);
            if(newFloat != _settings.ObjectToObjectSnapEpsilon)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectToObjectSnapEpsilon = newFloat;
            }
        }

        private GUIContent GetContentForObjectToObjectSnapEpsilonField()
        {
            var content = new GUIContent();
            content.text = "Object to object snap epsilon";
            content.tooltip = "When object to object snapping is enabled, this value will be used to determine how close object snap points " + 
                              "have to be to one another in order to allow the object to snap.";

            return content;
        }
        #endregion
    }
}
#endif