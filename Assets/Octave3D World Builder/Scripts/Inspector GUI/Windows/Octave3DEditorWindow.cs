#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public abstract class Octave3DEditorWindow : EditorWindow
    {
        #region Protected Variables
        [SerializeField]
        protected bool _isVisible = false;
        #endregion

        #region Public Properties
        public bool IsVisible { get { return _isVisible; } }
        #endregion

        #region Constructors
        public Octave3DEditorWindow()
        {
            // Note: This is needed in order to allow for Undo/Redo.
            EditorApplication.modifierKeysChanged -= Repaint;
            EditorApplication.modifierKeysChanged += Repaint;
        }
        #endregion

        #region Public Static Functions
        public static T Create<T>() where T : Octave3DEditorWindow
        {
            T[] windows = Resources.FindObjectsOfTypeAll<T>();
            if (windows.Length != 0) return windows[0];

            T window = (T)EditorWindow.CreateInstance<T>();
            return window;
        }

        public static void Destroy<T>(T window) where T : Octave3DEditorWindow
        {
            if (window.IsVisible) window.Close();
            else DestroyImmediate(window);
        }
        #endregion

        #region Public Methods
        public void RepaintOctave3DWindow()
        {
            if (_isVisible) Repaint();
        }
        #endregion

        #region Public Abstract Methods
        public abstract void ShowOctave3DWindow();
        public abstract string GetTitle();
        #endregion

        #region Protected Abstract Methods
        protected abstract void RenderContent();
        #endregion

        #region Protected Methods
        protected GUIContent GetTitleContent()
        {
            var titleContent = new GUIContent();
            titleContent.text = GetTitle();

            return titleContent;
        }

        protected void ShowDockable(bool focus)
        {
            _isVisible = true;
            AdjustTitle();
            base.Show();
            if (focus) Focus();
        }

        protected void ShowUtility(bool focus)
        {
            _isVisible = true;
            AdjustTitle();
            base.ShowUtility();
            if (focus) Focus();
        }

        protected void MakeNonResizable(Vector2 size)
        {
            maxSize = size;
            minSize = maxSize;
        }
        #endregion

        #region Private Methods
        private void AdjustTitle()
        {
            titleContent = GetTitleContent();
        }

        private void OnGUI()
        {
            bool isKeyDownEvent = Event.current.type == EventType.KeyDown;

            // Note: This is important. If the user presses the Delete key when the window has focus,
            //       it will delete the tool objects, so we will check for the delete key and disable
            //       it.
            if(isKeyDownEvent && Event.current.keyCode == KeyCode.Delete)
            {
                Event.current.DisableInSceneView();
                return;
            }

            // Null can happen when the tool object is disabled in the scene view
            if(Octave3DWorldBuilder.ActiveInstance != null)
            {
                RenderContent();

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
            }
        }

        /// <summary>
        /// Use this to repaint more often. This is useful to solve the problem of repainting
        /// the window is certain key situations like Undo/Redo. Calling repaint directly won't
        /// work in those cases if the window doesn't hae focus.
        /// </summary>
        private void Update()
        {
            // Note: At the moment there seems to be a bug in Unity which causes any tooltips to not show
            //       properly when calling Repaint. So fo the moment 'Repaint' is not called here. We will
            //       call it when it's necessary.
            //Repaint();
        }
        #endregion
    }
}
#endif