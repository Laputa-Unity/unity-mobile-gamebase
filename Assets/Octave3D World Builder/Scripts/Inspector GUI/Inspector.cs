#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    public class Inspector : ScriptableObject, IMessageListener
    {
        #region Private Variables
        [SerializeField]
        private Editor _editorWindow;

        [SerializeField]
        private InspectorGUIIdentifier _activeInspectorGUIIdentifier = InspectorGUIIdentifier.ObjectPlacement;

        [SerializeField]
        private InspectorGUISelectionToolbar _inpectorGUISelectionTolbar = new InspectorGUISelectionToolbar();

        [SerializeField]
        private ObjectPlacementInspectorGUI _objectPlacementInspectorGUI = new ObjectPlacementInspectorGUI();
        [SerializeField]
        private ObjectEraseInspectorGUI _objectEraseInspectorGUI = new ObjectEraseInspectorGUI();
        [SerializeField]
        private ObjectSnappingInspectorGUI _objectSnappingInspectorGUI = new ObjectSnappingInspectorGUI();
        [SerializeField]
        private ObjectSelectionInspectorGUI _objectSelectionInspectorGUI = new ObjectSelectionInspectorGUI();
        #endregion

        #region Public Properties
        public InspectorGUIIdentifier ActiveInspectorGUIIdentifier 
        { 
            get { return _activeInspectorGUIIdentifier; } 
            set 
            {
                _activeInspectorGUIIdentifier = value;
                InspectorGUIWasChangedMessage.SendToInterestedListeners(_activeInspectorGUIIdentifier);

                SceneView.RepaintAll();
                if (_editorWindow != null) _editorWindow.Repaint();
            }
        }
        public InspectorGUISelectionToolbar InspectorGUISelectionToolbar { get { return _inpectorGUISelectionTolbar; } }
        public ObjectPlacementInspectorGUI ObjectPlacementInspectorGUI { get { return _objectPlacementInspectorGUI; } }
        public ObjectEraseInspectorGUI ObjectEraseInspectorGUI { get { return _objectEraseInspectorGUI; } }
        public ObjectSnappingInspectorGUI ObjectSnappingInspectorGUI { get { return _objectSnappingInspectorGUI; } }
        public ObjectSelectionInspectorGUI ObjectSelectionInspectorGUI { get { return _objectSelectionInspectorGUI; } }
        public Editor EditorWindow { get { return _editorWindow; } set { if (value != null) _editorWindow = value; } }
        #endregion

        #region Constructors
        public Inspector()
        {
            _inpectorGUISelectionTolbar.ButtonScale = 0.25f;
        }
        #endregion

        #region Public Static Functions
        public static Inspector Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.Inspector;
        }
        #endregion

        #region Public Methods
        public void Repaint()
        {
            if (_editorWindow != null) _editorWindow.Repaint();
        }

        public void Render()
        {
            RenderShowGUIHintsToggle();
            Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("In order to use the hotkeys, the scene view window must have focus. This means that if you click on the " +
                                                      "Inspector or an Editor Window to modify settings, you will then have to click again inside the scene view " +
                                                      "window before you can use any hotkeys. Any mouse button can be used for the click. Another way to work around this " + 
                                                      "is to perform a dummy keypress which will transfer the focus back to the scene view window.");
            Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("Almost all controls have tooltips which can contain useful info. Hover the controls with the mouse cursor to allow the tooltips to appear.");

            _inpectorGUISelectionTolbar.Render();
            RenderActionControls();

            GetActiveGUI().Render();
        }

        public void Initialize()
        {
            if (Octave3DWorldBuilder.ActiveInstance == null) return;

            _objectPlacementInspectorGUI.Initialize();
            _objectEraseInspectorGUI.Initialize();
            _objectSnappingInspectorGUI.Initialize();
            _objectSelectionInspectorGUI.Initialize();
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            MessageListenerRegistration.PerformRegistrationForInspector(this);
        }

        private void RenderShowGUIHintsToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForShowGUIHintsToggle(), Octave3DWorldBuilder.ActiveInstance.ShowGUIHints);
            if(newBool != Octave3DWorldBuilder.ActiveInstance.ShowGUIHints)
            {
                UndoEx.RecordForToolAction(Octave3DWorldBuilder.ActiveInstance);
                Octave3DWorldBuilder.ActiveInstance.ShowGUIHints = newBool;
                Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
            }
        }

        private GUIContent GetContentForShowGUIHintsToggle()
        {
            var content = new GUIContent();
            content.text = "Show GUI hints";
            content.tooltip = "If this is checked, the GUI will display message boxes that contain useful hints about how to use the tool.";

            return content;
        }

        private void RenderActionControls()
        {
            EditorGUILayout.BeginHorizontal();
            var content = new GUIContent();
            content.text = "Mesh combine...";
            content.tooltip = "Opens up a new window which allows you to perform mesh combine operations.";
            if(GUILayout.Button(content, GUILayout.Width(110.0f)))
            {
                Octave3DWorldBuilder.ActiveInstance.MeshCombineSettings.ShowEditorWindow();
            }

            RenderRefreshSceneButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            RenderSaveOctave3DConfigButton();
            RenderLoadOctave3DConfigButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderRefreshSceneButton()
        {
            if (GUILayout.Button(GetContentForRefreshSceneButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.45f)))
            {
                Octave3DScene.Get().Refresh(true);
            }
        }

        private GUIContent GetContentForRefreshSceneButton()
        {
            var content = new GUIContent();
            content.text = "Refresh scene";
            content.tooltip = "Refreshes the internal scene data. One use case for this button is when you are working with 2D sprites and you cahnge the pivot point for one " +
                              "or more sprites. In that case the internal representation of the sprite objects needs to be rebuilt and pressing this button will do that.";

            return content;
        }

        private void RenderSaveOctave3DConfigButton()
        {
            if (GUILayout.Button(GetContentForSaveOctave3DConfigButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.73f)))
            {
                Octave3DConfigSaveWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForSaveOctave3DConfigButton()
        {
            var content = new GUIContent();
            content.text = "Save Octave3D config...";
            content.tooltip = "Opens up a new window which allows you to save different types of settings to a config file which can be loaded when needed.";

            return content;
        }

        private void RenderLoadOctave3DConfigButton()
        {
            if (GUILayout.Button(GetContentForLoadOctave3DConfigButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.75f)))
            {
                Octave3DConfigLoadWindow.Get().ShowOctave3DWindow();
            }
        }

        private GUIContent GetContentForLoadOctave3DConfigButton()
        {
            var content = new GUIContent();
            content.text = "Load Octave3D config...";
            content.tooltip = "Opens up a new window which allows you to load different types of settings from a specfied config file.";

            return content;
        }

        private InspectorGUI GetActiveGUI()
        {
            switch(_activeInspectorGUIIdentifier)
            {
                case InspectorGUIIdentifier.ObjectPlacement:

                    return _objectPlacementInspectorGUI;

                case InspectorGUIIdentifier.ObjectErase:

                    return _objectEraseInspectorGUI;

                case InspectorGUIIdentifier.ObjectSelection:

                    return _objectSelectionInspectorGUI;

                case InspectorGUIIdentifier.ObjectSnapping:

                    return _objectSnappingInspectorGUI;

                default:

                    return null;
            }
        }
        #endregion

        #region Message Handlers
        public void RespondToMessage(Message message)
        {
            switch(message.Type)
            {
                case MessageType.ToolWasReset:

                    RespondToMessage(message as ToolWasResetMessage);
                    break;

                case MessageType.ToolWasStarted:

                    RespondToMessage(message as ToolWasStartedMessage);
                    break;
            }
        }

        private void RespondToMessage(ToolWasResetMessage message)
        {
            Initialize();
        }

        private void RespondToMessage(ToolWasStartedMessage message)
        {
            Initialize();
        }
        #endregion
    }
}
#endif