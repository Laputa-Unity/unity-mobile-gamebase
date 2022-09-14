#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [ExecuteInEditMode]
    public class EditorWindowPool : MonoBehaviour
    {
        [SerializeField]
        private List<Octave3DEditorWindow> _allWindows = new List<Octave3DEditorWindow>();

        [SerializeField]
        private PrefabManagementWindow _prefabManagementWindow;
        [SerializeField]
        private ObjectPlacementSettingsWindow _objectPlacementSettingsWindow;
        [SerializeField]
        private PrefabsToCategoryDropSettingsWindow _prefabsToCategoryDropSettingsWindow;
        [SerializeField]
        private PrefabTagsWindow _prefabTagsWindow;
        [SerializeField]
        private ObjectLayersWindow _objectLayersWindow;
        [SerializeField]
        private ObjectGrabSettingsWindow _selectionGrabSettingsWindow;

        [SerializeField]
        private Octave3DConfigSaveWindow _octave3DConfigSaveWindow;
        [SerializeField]
        private Octave3DConfigLoadWindow _octave3DConfigLoadWindow;
        [SerializeField]
        private DecorPaintBrushViewLookAndFeelWindow _decorPaintBrushViewLookAndFeelWindow;

        // Note: These would normally have to be associated with the Prefab Management Window. However,
        //       there seem to be some problems with serialization. It may be possible to solve those
        //       problems using 'EditorPrefs', but I would rather keep things simple and store these 
        //       variables here.
        [SerializeField]
        private ActivePrefabCategoryView _activePrefabCategoryView = new ActivePrefabCategoryView();
        [SerializeField]
        private ActivePrefabView _activePrefabView = new ActivePrefabView();

        public ActivePrefabCategoryView ActivePrefabCategoryView { get { return _activePrefabCategoryView; } }
        public ActivePrefabView ActivePrefabView { get { return _activePrefabView; } }

        public Octave3DConfigSaveWindow Octave3DConfigSaveWindow
        {
            get
            {
                if (_octave3DConfigSaveWindow == null) _octave3DConfigSaveWindow = CreateWindow<Octave3DConfigSaveWindow>();
                return _octave3DConfigSaveWindow;
            }
        }

        public Octave3DConfigLoadWindow Octave3DConfigLoadWindow
        {
            get
            {
                if (_octave3DConfigLoadWindow == null) _octave3DConfigLoadWindow = CreateWindow<Octave3DConfigLoadWindow>();
                return _octave3DConfigLoadWindow;
            }
        }
        public PrefabManagementWindow PrefabManagementWindow
        {
            get
            {
                if (_prefabManagementWindow == null) _prefabManagementWindow = CreateWindow<PrefabManagementWindow>();
                return _prefabManagementWindow;
            }
        }
        public ObjectPlacementSettingsWindow ObjectPlacementSettingsWindow
        {
            get
            {
                if (_objectPlacementSettingsWindow == null) _objectPlacementSettingsWindow = CreateWindow<ObjectPlacementSettingsWindow>();
                return _objectPlacementSettingsWindow;
            }
        }
        public PrefabsToCategoryDropSettingsWindow PrefabsToCategoryDropSettingsWindow
        {
            get
            {
                if (_prefabsToCategoryDropSettingsWindow == null) _prefabsToCategoryDropSettingsWindow = CreateWindow<PrefabsToCategoryDropSettingsWindow>();
                return _prefabsToCategoryDropSettingsWindow;
            }
        }
        public PrefabTagsWindow PrefabTagsWindow
        {
            get
            {
                if (_prefabTagsWindow == null) _prefabTagsWindow = CreateWindow<PrefabTagsWindow>();
                return _prefabTagsWindow;
            }
        }
        public ObjectLayersWindow ObjectLayersWindow
        {
            get
            {
                if (_objectLayersWindow == null) _objectLayersWindow = CreateWindow<ObjectLayersWindow>();
                return _objectLayersWindow;
            }
        }
        public DecorPaintBrushViewLookAndFeelWindow DecorPaintBrushViewLookAndFeelWindow
        {
            get
            {
                if (_decorPaintBrushViewLookAndFeelWindow == null) _decorPaintBrushViewLookAndFeelWindow = CreateWindow<DecorPaintBrushViewLookAndFeelWindow>();
                return _decorPaintBrushViewLookAndFeelWindow;
            }
        }
        public ObjectGrabSettingsWindow SelectionGrabSettingsWindow
        {
            get
            {
                if (_selectionGrabSettingsWindow == null) _selectionGrabSettingsWindow = CreateWindow<ObjectGrabSettingsWindow>();
                return _selectionGrabSettingsWindow;
            }
        }

        public void DestroyAllWindows()
        {
            foreach(var window in _allWindows)
            {
                if (window != null) Octave3DEditorWindow.Destroy(window);
            }
            _allWindows.Clear();
        }

        public void RepaintAll()
        {
            foreach (var window in _allWindows)
            {
                if (window != null) window.RepaintOctave3DWindow();
            }
        }

        public T CreateWindow<T>() where T : Octave3DEditorWindow
        {
            T window = Octave3DEditorWindow.Create<T>();
            if (window != null) _allWindows.Add(window);

            return window;
        }

        private void OnEnable()
        {
            _allWindows.Clear();

            Octave3DEditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<Octave3DEditorWindow>();
            foreach (var window in allWindows) _allWindows.Add(window);

            RepaintAll();
        }
    }
}
#endif