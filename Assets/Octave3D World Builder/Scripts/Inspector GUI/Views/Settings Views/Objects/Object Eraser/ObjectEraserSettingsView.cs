#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectEraserSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectEraserSettings _settings;
        #endregion

        #region Constructors
        public ObjectEraserSettingsView(ObjectEraserSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Eraser Settings";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("Usually, you will want to allow Undo/Redo for object erasing. However, please keep in mind that this can " + 
                                                      "add a little bit of overhead when deleting large amounts of objects. If you want to perform a quick cleanup " + 
                                                      "in certain areas of your game level, and if you know for sure that you won't need those objects to come back, " + 
                                                      "you can uncheck this toggle.");
            RenderAllowUndoRedoToggle();
            RenderEraseDelayField();

            EditorGUILayout.Separator();
            if(_settings.EraseMode == ObjectEraseMode.ObjectMass2D)
            {
                Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("This erase mode is ideal for performing quick clean-ups in your game level. Please keep in mind that " + 
                                                          "the 2D erase brush will not take object occlusion into accunt. All objects that are intersected by the 2D " + 
                                                          "brush will get erased even if they are not visible on the screen (i.e. they are occluded by other objects).");
            }
            else 
            if(_settings.EraseMode == ObjectEraseMode.ObjectMass3D)
            {
                Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("The 3D brush is great for erasing large amount of objects quickly and efficiently. One thing to keep in mind is that " + 
                                                          "the brush will only erase objects that reside very close to the brush plane. Objects that reside above or below the " + 
                                                          "brush plane will not be erased.");
            }
            RenderEraseModeSelectionPopup();

            if (_settings.EraseMode == ObjectEraseMode.ObjectMass2D) _settings.Mass2DEraseSettings.View.Render();
            else if (_settings.EraseMode == ObjectEraseMode.ObjectMass3D) _settings.Mass3DEraseSettings.View.Render();
            RenderEraseOnlyMeshObjectsToggle();
        }
        #endregion

        #region Private Methods
        private void RenderAllowUndoRedoToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAllowUndoRedoToggle(), _settings.AllowUndoRedo);
            if (newBool != _settings.AllowUndoRedo)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.AllowUndoRedo = newBool;
            }
        }

        private GUIContent GetContentForAllowUndoRedoToggle()
        {
            var content = new GUIContent();
            content.text = "Allow Undo/Redo";
            content.tooltip = "If this is checked, erase operations can be undone/redone. Leaving this unchecked might speed things up in certain situations.";

            return content;
        }

        private void RenderEraseDelayField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForEraseDelayField(), _settings.EraseDelayInSeconds);
            if (newFloat != _settings.EraseDelayInSeconds)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EraseDelayInSeconds = newFloat;
            }
        }

        private GUIContent GetContentForEraseDelayField()
        {
            var content = new GUIContent();
            content.text = "Erase delay (seconds)";
            content.tooltip = "Allows you to specify how much time has to pass between 2 successive erase operations. The value is expressed in seconds.";

            return content;
        }       

        private void RenderEraseModeSelectionPopup()
        {
            ObjectEraseMode newObjectEraseMode = (ObjectEraseMode)EditorGUILayout.EnumPopup(GetContentForEraseModeSelectionPopup(), _settings.EraseMode);
            if (newObjectEraseMode != _settings.EraseMode)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EraseMode = newObjectEraseMode;
            }
        }

        private GUIContent GetContentForEraseModeSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Erase mode";
            content.tooltip = "Allows you to choose the way in which objects are erased in the scene.";

            return content;
        }

        private void RenderEraseOnlyMeshObjectsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForEraseOnlyMeshObjectsToggle(), _settings.EraseOnlyMeshObjects);
            if (newBool != _settings.EraseOnlyMeshObjects)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.EraseOnlyMeshObjects = newBool;
            }
        }

        private GUIContent GetContentForEraseOnlyMeshObjectsToggle()
        {
            var content = new GUIContent();
            content.text = "Erase only mesh objects";
            content.tooltip = "If this is checked, only mesh objects will be erased. Lights and particle systems will be ingored. " +
                              "Note: Lights and particle systems can still be erased if they are part of a mesh object hierarchy.";

            return content;
        }
        #endregion
    }
}
#endif