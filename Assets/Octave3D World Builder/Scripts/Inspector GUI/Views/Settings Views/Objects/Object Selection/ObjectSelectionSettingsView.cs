#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSelectionSettings _settings;
        #endregion

        #region Constructors
        public ObjectSelectionSettingsView(ObjectSelectionSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Selection Settings";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            float newFloat; bool newBool; string newString;

            EditorGUILayout.LabelField("Object groups", EditorStyles.boldLabel);
            var content = new GUIContent();
            content.text = "Attach to object group";
            content.tooltip = "If this is checked, any objects which are created via selection operations will be attached to a group of your choosing.";
            newBool = EditorGUILayout.ToggleLeft(content, _settings.ObjectGroupSettings.AttachToObjectGroup);
            if(newBool != _settings.ObjectGroupSettings.AttachToObjectGroup)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ObjectGroupSettings.AttachToObjectGroup = newBool;
            }

            ObjectGroupDatabase objectGroupDatabase = Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase;
            if (objectGroupDatabase.NumberOfGroups == 0)
            {
                EditorGUILayout.HelpBox("No object groups are currently available.", UnityEditor.MessageType.None);
            }
            else
            {
                if (_settings.ObjectGroupSettings.DestinationGroup == null)
                    _settings.ObjectGroupSettings.DestinationGroup = objectGroupDatabase.GetAllObjectGroups()[0];

                content.text = "Object group";
                content.tooltip = "If \'Attach to object group\' is checked, any objects which are created via selection operations will be attached to this group.";
                newString = EditorGUILayoutEx.Popup(content, _settings.ObjectGroupSettings.DestinationGroup.Name, objectGroupDatabase.GetAllObjectGroupNames());
                if (newString != _settings.ObjectGroupSettings.DestinationGroup.Name)
                {
                    UndoEx.RecordForToolAction(_settings);
                    _settings.ObjectGroupSettings.DestinationGroup = objectGroupDatabase.GetObjectGroupByName(newString);
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
            content.text = "X rotation step";
            content.tooltip = "Allows you to specify how much rotation is applied to the selected objects when the X rotation key is pressed.";
            newFloat = EditorGUILayout.FloatField(content, _settings.XRotationStep);
            if(newFloat != _settings.XRotationStep)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.XRotationStep = newFloat;
            }

            content.text = "Y rotation step";
            content.tooltip = "Allows you to specify how much rotation is applied to the selected objects when the Y rotation key is pressed.";
            newFloat = EditorGUILayout.FloatField(content, _settings.YRotationStep);
            if (newFloat != _settings.YRotationStep)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.YRotationStep = newFloat;
            }

            content.text = "Z rotation step";
            content.tooltip = "Allows you to specify how much rotation is applied to the selected objects when the Z rotation key is pressed.";
            newFloat = EditorGUILayout.FloatField(content, _settings.ZRotationStep);
            if (newFloat != _settings.ZRotationStep)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.ZRotationStep = newFloat;
            }

            content.text = "Rotate around selection center";
            content.tooltip = "If this is checked, the rotation will happen around the selection's world center. Otherwise, each object will be rotated around its own center.";
            newBool = EditorGUILayout.ToggleLeft(content, _settings.RotateAroundSelectionCenter);
            if(newBool != _settings.RotateAroundSelectionCenter)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.RotateAroundSelectionCenter = newBool;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Object 2 object snap", EditorStyles.boldLabel);
            content.text = "Snap epsilon";
            content.tooltip = "The selected objects will always snap to nearby objects which are \'epsilon\' units away from the selected objects.";
            newFloat = EditorGUILayout.FloatField(content, _settings.Object2ObjectSnapSettings.SnapEps);
            if(newFloat != _settings.Object2ObjectSnapSettings.SnapEps)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.Object2ObjectSnapSettings.SnapEps = newFloat;
            }

            content.text = "Can hover objects";
            content.tooltip = "If this is checked, you will be able to hover other objects during a snap session. Otherwise, you will only be able to hover the grid surface.";
            newBool = EditorGUILayout.ToggleLeft(content, _settings.Object2ObjectSnapSettings.CanHoverObjects);
            if (newBool != _settings.Object2ObjectSnapSettings.CanHoverObjects)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.Object2ObjectSnapSettings.CanHoverObjects = newBool;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Misc", EditorStyles.boldLabel);
            RenderAllowPartialOverlapToggle();
            RenderSelectionShapeTypeSelectionPopup();
            RenderSelectionUpdateModeSelectionPopup();
            RenderSelectionModeSelectionPopup();

            if (_settings.SelectionMode == ObjectSelectionMode.Paint) _settings.PaintModeSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderAllowPartialOverlapToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowPartialOverlapToggle(), _settings.AllowPartialOverlap);
            if (newBool != _settings.AllowPartialOverlap)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AllowPartialOverlap = newBool;
            }
        }

        private GUIContent GetContentForAllowPartialOverlapToggle()
        {
            var content = new GUIContent();
            content.text = "Allow partial overlap";
            content.tooltip = "When this is NOT checked, objects will be selected ONLY if their screen rectangle is totally contained by the selection shape.";

            return content;
        }

        private void RenderSelectionShapeTypeSelectionPopup()
        {
            ObjectSelectionShapeType newSelectionShapeType = (ObjectSelectionShapeType)EditorGUILayout.EnumPopup(GetContentForSelectionShapeTypeSelectionPopup(), _settings.SelectionShapeType);
            if (newSelectionShapeType != _settings.SelectionShapeType)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SelectionShapeType = newSelectionShapeType;
            }
        }

        private GUIContent GetContentForSelectionShapeTypeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Shape type";
            content.tooltip = "Allows you to choose the type of shape which is used to select objects in the scene.";

            return content;
        }

        private void RenderSelectionUpdateModeSelectionPopup()
        {
            ObjectSelectionUpdateMode newSelectionUpdateMode = (ObjectSelectionUpdateMode)EditorGUILayout.EnumPopup(GetCotentForSelectionUpdateModeSelectionToggle(), _settings.SelectionUpdateMode);
            if (newSelectionUpdateMode != _settings.SelectionUpdateMode)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SelectionUpdateMode = newSelectionUpdateMode;
            }
        }

        private GUIContent GetCotentForSelectionUpdateModeSelectionToggle()
        {
            var content = new GUIContent();
            content.text = "Selection update mode";
            content.tooltip = "Allows you to control the way in which the object selection is updated.";

            return content;
        }

        private void RenderSelectionModeSelectionPopup()
        {
            ObjectSelectionMode newSelectionMode = (ObjectSelectionMode)EditorGUILayout.EnumPopup(GetContentForSelectionModeSelectionPopup(), _settings.SelectionMode);
            if (newSelectionMode != _settings.SelectionMode)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.SelectionMode = newSelectionMode;
            }
        }

        private GUIContent GetContentForSelectionModeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Selection mode";
            content.tooltip = "Allows you to control the way in which object selection is performed.";

            return content;
        }
        #endregion
    }
}
#endif