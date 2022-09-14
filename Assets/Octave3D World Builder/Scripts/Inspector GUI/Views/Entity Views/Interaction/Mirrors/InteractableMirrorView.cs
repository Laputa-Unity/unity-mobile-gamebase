#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class InteractableMirrorView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private InteractableMirror _mirror;
        #endregion

        #region Constructors
        public InteractableMirrorView(InteractableMirror mirror)
        {
            _mirror = mirror;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Mirroring";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderIsActiveToggle();
            RenderAlignToSurfaceToggle();
            RenderMirrorRotationToggle();
            RenderMirrorSpanningObjectsToggle();
            RenderAllowIntersectionForMirroredObjectsToggle();
            RenderMirrorPositionField();
            RenderMirrorRotationField();
            _mirror.Settings.View.Render();
            _mirror.RenderSettings.View.Render();

            EditorGUILayout.BeginHorizontal();
            RenderBeginInteractionSessionButton();
            RenderResetPositionButton();
            RenderResetRotationButton();
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Private Methods
        private void RenderIsActiveToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIsActiveToggle(), _mirror.IsActive);
            if(newBool != _mirror.IsActive)
            {
                UndoEx.RecordForToolAction(_mirror);
                _mirror.IsActive = newBool;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForIsActiveToggle()
        {
            var content = new GUIContent();
            content.text = "Is active";
            content.tooltip = "Allows you to activate or deactivate the mirror. When a mirror is active, it is rendered in the scene and it can be used to mirror objects.";

            return content;
        }

        private void RenderAlignToSurfaceToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAlignToSurfaceToggle(), _mirror.AlignToSurface);
            if(newBool != _mirror.AlignToSurface)
            {
                UndoEx.RecordForToolAction(_mirror);
                _mirror.AlignToSurface = newBool;
            }
        }

        private GUIContent GetContentForAlignToSurfaceToggle()
        {
            var content = new GUIContent();
            content.text = "Align to surface";
            content.tooltip = "If this is checked, the mirror will be aligned to the hover surface when it is moved in the scene.";

            return content;
        }

        private void RenderMirrorRotationToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForMirrorRotationToggle(), _mirror.MirrorRotation);
            if(newBool != _mirror.MirrorRotation)
            {
                UndoEx.RecordForToolAction(_mirror);
                _mirror.MirrorRotation = newBool;
            }
        }

        private GUIContent GetContentForMirrorRotationToggle()
        {
            var content = new GUIContent();
            content.text = "Mirror rotation";
            content.tooltip = "Check this if you wish to also mirror entity rotations. If this is not checked, only the position of the entities will be mirrored.";

            return content;
        }

        private void RenderMirrorSpanningObjectsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForMirrorSpanningObjectsToggle(), _mirror.MirrorSpanningObjects);
            if (newBool != _mirror.MirrorSpanningObjects)
            {
                UndoEx.RecordForToolAction(_mirror);
                _mirror.MirrorSpanningObjects = newBool;
            }
        }

        private GUIContent GetContentForMirrorSpanningObjectsToggle()
        {
            var content = new GUIContent();
            content.text = "Mirror spanning objects";
            content.tooltip = "If this is checked, objects which span the mirror plane will also be mirrored.";

            return content;
        }

        private void RenderAllowIntersectionForMirroredObjectsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowIntersectionForMirroredObjectsToggle(), _mirror.AllowIntersectionForMirroredObjects);
            if (newBool != _mirror.AllowIntersectionForMirroredObjects)
            {
                UndoEx.RecordForToolAction(_mirror);
                _mirror.AllowIntersectionForMirroredObjects = newBool;
            }
        }

        private GUIContent GetContentForAllowIntersectionForMirroredObjectsToggle()
        {
            var content = new GUIContent();
            content.text = "Allow intersection for mirrored objects";
            content.tooltip = "If this is checked, mirrored objects are allowed to intersect with other objects in the scene.";

            return content;
        }

        private void RenderBeginInteractionSessionButton()
        {
            EditorGUIColor.Push(_mirror.IsInteractionSessionActive ? Color.green : Color.white);
            if(GUILayout.Button(GetContentForBeginInteractionSessionButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.4f)))
            {
                _mirror.ToggleInteractionSession();
            }
            EditorGUIColor.Pop();
        }

        private GUIContent GetContentForBeginInteractionSessionButton()
        {
            var content = new GUIContent();
            content.text = "Manipulate";
            content.tooltip = "Pressing this button will allow you to toggle mirror maniuplation on/off. When it's turned on, you can move and rotate the mirror in the scene.";

            return content;
        }

        private void RenderResetPositionButton()
        {
            if(GUILayout.Button(GetContentForResetPositionButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.5f)))
            {
                UndoEx.RecordForToolAction(_mirror);
                _mirror.WorldCenter = Vector3.zero;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForResetPositionButton()
        {
            var content = new GUIContent();
            content.text = "Reset position";
            content.tooltip = "Places the mirror at the origin of the coordinate system.";

            return content;
        }

        private void RenderResetRotationButton()
        {
            if (GUILayout.Button(GetContentForResetRotationButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.5f)))
            {
                UndoEx.RecordForToolAction(_mirror);
                _mirror.WorldRotation = Vector3.zero;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForResetRotationButton()
        {
            var content = new GUIContent();
            content.text = "Reset rotation";
            content.tooltip = "Resets the rotation of the mirror to 0 on all axes.";

            return content;
        }

        private void RenderMirrorPositionField()
        {
            Vector3 newVector = EditorGUILayout.Vector3Field(GetContentForMirrorPositionField(), _mirror.WorldCenter);
            if(newVector != _mirror.WorldCenter)
            {
                UndoEx.RecordForToolAction(_mirror);
                _mirror.WorldCenter = newVector;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForMirrorPositionField()
        {
            var content = new GUIContent();
            content.text = "Position";
            content.tooltip = "Allows you to change the mirror's position.";

            return content;
        }

        private void RenderMirrorRotationField()
        {
            Vector3 newVector = EditorGUILayout.Vector3Field(GetContentForMirrorRotationField(), _mirror.WorldRotation);
            if(newVector != _mirror.WorldRotation)
            {
                UndoEx.RecordForToolAction(_mirror);
                _mirror.WorldRotation = newVector;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForMirrorRotationField()
        {
            var content = new GUIContent();
            content.text = "Rotation";
            content.tooltip = "Allows you to change the mirror's rotation (angle values in degrees).";

            return content;
        }
        #endregion
    }
}
#endif