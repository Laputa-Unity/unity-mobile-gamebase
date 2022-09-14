#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    [CustomEditor(typeof(Octave3DWorldBuilder))]
    public class Octave3DWorldBuilderInspectorGUI : Editor
    {
        #region Private Variables
        private Octave3DWorldBuilder _octave;
        #endregion

        #region Public methods
        public void OnSceneGUI()
        {
            if (_octave.gameObject.activeSelf && _octave.enabled && !Application.isPlaying)
            {
                KeyboardButtonStates.Instance.RepairCTRLAndCMDStates();

                // Just make sure that the transform tools are hidden. They can really get in the
                // way sometimes.
                Tools.current = Tool.None;

                _octave.Inspector.EditorWindow = this;  // Note: This should not be necessary because we could set this in the 'OnEnable' method,
                                                        //       but there were a couple of times when it somehow got reset. I do not know what happened, so
                                                        //       I decided to take the easy way out and just set it here every time. Same thing in 'OnInspectorGUI'. 
                if(ToolSupervisor.Get() != null) ToolSupervisor.Get().Supervise();

                /*if (Event.current.type == EventType.keyDown)
                {
                    if (Event.current.keyCode == KeyCode.P)
                    {
                        string path = "Assets";
                        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(PrefabCategoryDatabase).ToString() + ".asset");

                        AssetDatabase.CreateAsset(PrefabCategoryDatabase.Get(), assetPathAndName);

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        //EditorUtility.FocusProjectWindow();
                    }
                }*/

                _octave.Octave3DScene.OnSceneGUI();
                _octave.SceneRenderer.RenderHandles();

                //var sceneViewEventHandler = SceneViewEventHandlerFactory.Create(_octave.Inspector.ActiveInspectorGUIIdentifier);
                //if (sceneViewEventHandler != null) sceneViewEventHandler.HandleSceneViewEvent(Event.current);
            }
        }

        public override void OnInspectorGUI()
        {
            if(_octave.gameObject.activeSelf && _octave.enabled && !Application.isPlaying)
            {
                bool isKeyDownEvent = Event.current.type == EventType.KeyDown;

                _octave.Inspector.EditorWindow = this;
                ToolSupervisor.Get().Supervise();

                // Note: This helps us avoid exceptions being thrown by Unity when prefabs are dragged onto certain controls
                //       such as when a prefab is associated with a tile connection.
                PrefabWithPrefabCategoryAssociationQueue.Instance.DequeAllAndPerform();

                _octave.Inspector.Render();
              
                // This code attempts to semi-solve the scene view focus problem which prevents the hotkeys from
                // working correctly. If we are dealing with a key-down event which hasn't been consumed, we will
                // transfer the focus over to the scene view window. This means that after settings are changed 
                // in the GUI, the user will have to perform one dummy keypress before they can start using the
                // hotkeys again.
                if (isKeyDownEvent && Event.current.type != EventType.Used && !Event.current.shift)
                {
                    SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                    if (sceneView != null) sceneView.Focus();
                }

                // We will always disable the event when the 'Delete' key is pressed because otherwise, 
                // the Octave3D object will be deleted from the scene.
                if(isKeyDownEvent && Event.current.keyCode == KeyCode.Delete)
                {
                    Event.current.DisableInSceneView();
                }
            }
        }
        #endregion

        #region Private methods
        private void OnEnable()
        {
            _octave = target as Octave3DWorldBuilder;
            _octave.OnInspectorEnabled();

            // Note: Just ensure that the Inspector will be redrawn on Undo/Redo.
            Undo.undoRedoPerformed -= Repaint;
            Undo.undoRedoPerformed += Repaint;

            // Let the world know that the tool was selected :)
            ToolWasSelectedMessage.SendToInterestedListeners();

            ToolSupervisor.Get().RemoveNullPrefabReferences();

            //if (PrefabPreviewTextureCache.Get().NumPreviews == 0) PrefabPreviewTextureCache.Get().GeneratePreviewForAllPrefabCategories(true);

            _octave.RepaintAllEditorWindows();
        }
        #endregion
    }
}
#endif