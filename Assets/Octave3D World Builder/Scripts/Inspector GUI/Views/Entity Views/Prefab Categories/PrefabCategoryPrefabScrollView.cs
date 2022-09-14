#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabCategoryPrefabScrollView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private PrefabCategory _prefabCategory;
        [NonSerialized]
        private List<Prefab> _filteredPrefabs;
        [NonSerialized]
        private GUIStyle _prefabNameLabelStyle = null;

        [SerializeField]
        private PrefabCategoryPrefabScrollViewData _viewData;
        #endregion

        public PrefabCategoryPrefabScrollViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabCategoryPrefabScrollViewData>();
                return _viewData;
            }
        }
        public PrefabCategoryScrollViewLookAndFeelWindow LookAndFeelWindow
        {
            get
            {
                return ViewData.LookAndFeelWindow;
            }
        }
        public PrefabCategory PrefabCategory { set { _prefabCategory = value; } }

        #region Protected Methods
        protected override void RenderContent()
        {
            AcquireFilteredPrefabs();
            if (_prefabCategory != null)
            {
                _prefabCategory.PrefabFilter.View.Render();
                RenderPrefabScrollView();
            }
        }
        #endregion

        #region Private Methods
        private void AcquireFilteredPrefabs()
        {
            if (_prefabCategory == null) _filteredPrefabs = new List<Prefab>();
            else _filteredPrefabs = _prefabCategory.GetFilteredPrefabs();
        }

        private void RenderPrefabScrollView()
        {
            ViewData.PrefabScrollPosition = EditorGUILayout.BeginScrollView(ViewData.PrefabScrollPosition, GetStyleForPrefabScrollView(), GUILayout.Height(ViewData.PrefabScrollViewHeight));
            if (_prefabCategory.IsEmpty) EditorGUILayout.HelpBox("Drop prefabs and prefab folders here to populate the category. Drop operations might take a while when dropping folders that contain a large number of prefabs.", UnityEditor.MessageType.None);
            else RenderPrefabPreviewRows();
            EditorGUILayout.EndScrollView();

            HandleDragAndDropEvent(GUILayoutUtility.GetLastRect());
        }

        private GUIStyle GetStyleForPrefabScrollView()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private void RenderPrefabPreviewRows()
        {
            for (int prefabIndex = 0; prefabIndex < _filteredPrefabs.Count; ++prefabIndex)
            {
                // Start a new row?
                if (prefabIndex % ViewData.NumberOfPrefabsPerRow == 0)
                {
                    if (prefabIndex != 0) EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                // Render the prefab entry
                Prefab prefab = _filteredPrefabs[prefabIndex];
                var previewButtonRenderData = new PrefabPreviewButtonRenderData();
                previewButtonRenderData.ExtractFromPrefab(prefab, ViewData.PrefabPreviewScale);
   
                EditorGUILayout.BeginVertical(GUILayout.Width(previewButtonRenderData.ButtonWidth));
                
                // Render the prefab preview button
                EditorGUIColor.Push(prefab == _prefabCategory.ActivePrefab ? ViewData.ActivePrefabTint : Color.white);
                if (EditorGUILayoutEx.PrefabPreview(prefab, true, previewButtonRenderData))
                {
                    ObjectPlacementSettings placementSettings = ObjectPlacementSettings.Get();
                    if(placementSettings.ObjectPlacementMode == ObjectPlacementMode.DecorPaint &&
                       placementSettings.DecorPaintObjectPlacementSettings.DecorPaintMode == DecorPaintMode.Brush &&
                       DecorPaintObjectPlacementBrushDatabase.Get().ActiveBrush != null && Event.current.button == (int)MouseButton.Right)
                    {
                        UndoEx.RecordForToolAction(DecorPaintObjectPlacementBrushDatabase.Get().ActiveBrush);
                        DecorPaintObjectPlacementBrushElement brushElement = DecorPaintObjectPlacementBrushDatabase.Get().ActiveBrush.CreateNewElement();
                        brushElement.Prefab = prefab;
                        Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
                        Octave3DWorldBuilder.ActiveInstance.Inspector.Repaint();
                    }
                    else
                    if(Octave3DWorldBuilder.ActiveInstance.Inspector.ActiveInspectorGUIIdentifier == InspectorGUIIdentifier.ObjectSelection &&
                       AllShortcutCombos.Instance.ReplacePrefabsForSelectedObjects_Preview.IsActive())
                    {
                        ObjectSelection.Get().ReplaceSelectedObjectsWithPrefab(prefab);
                    }
                    else
                    {
                        UndoEx.RecordForToolAction(_prefabCategory);
                        _prefabCategory.SetActivePrefab(prefab);
                    }
                }
                EditorGUIColor.Pop();

                // Render the prefab name labels if necessary
                if(ViewData.ShowPrefabNames)
                {
                    Rect previewRectangle = GUILayoutUtility.GetLastRect();
                    GUI.Label(previewRectangle, prefab.Name, GetStyleForPrefabNameLabel());
                }
       
                // Render the remove prefab button
                if (GUILayout.Button(GetRemovePrefabButtonContent()))
                {
                    UndoEx.RecordForToolAction(_prefabCategory);
                    _prefabCategory.RemoveAndDestroyPrefab(prefab);
                    Octave3DWorldBuilder.ActiveInstance.Inspector.Repaint();
                }

                EditorGUILayout.EndVertical();
            }

            // End the last row (if any)
            if (_filteredPrefabs.Count != 0) EditorGUILayout.EndHorizontal();
        }

        private GUIStyle GetStyleForPrefabNameLabel()
        {
            if (_prefabNameLabelStyle == null)
            {
                _prefabNameLabelStyle = new GUIStyle();
                _prefabNameLabelStyle.fontStyle = FontStyle.Bold;
                _prefabNameLabelStyle.normal.textColor = ViewData.PrefabNameLabelColor;
                _prefabNameLabelStyle.wordWrap = true;
            }
            return _prefabNameLabelStyle;
        }

        private GUIContent GetRemovePrefabButtonContent()
        {
            var content = new GUIContent();
            content.text = "Remove";
            content.tooltip = "Removes the prefab from the active category.";

            return content;
        }

        private void HandleDragAndDropEvent(Rect dropRectangle)
        {
            PrefabsToCategoryDropEventHandler.Get().Handle(Event.current, dropRectangle);
        }
        #endregion
    }
}
#endif