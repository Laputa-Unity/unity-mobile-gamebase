#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private DecorPaintObjectPlacementBrush _brush;

        [SerializeField]
        private DecorPaintBrushViewData _data;
        #endregion

        private DecorPaintBrushViewData Data
        {
            get
            {
                if (_data == null) _data = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<DecorPaintBrushViewData>();
                return _data;
            }
        }

        #region Constructors
        public DecorPaintObjectPlacementBrushView(DecorPaintObjectPlacementBrush brush)
        {
            _brush = brush;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderNameChangeField();
            RenderRadiusField();
            RenderMaxNumberOfObjectsField();
            RenderDistanceBetweenObjectsField();
            RenderIgnoreObjectsOutsideOfPaintSurfaceToggle();
            RenderDestinationCategoryForElementPrefabsSelectionPopup();

            EditorGUILayout.Separator();
            if (!_brush.IsEmpty) Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("Left click on an element's preview to change its parameters. Right click to remove the element from the brush. SHIFT + Right click to toggle the element on/off.");
            Data.ElementsScrollPos = EditorGUILayout.BeginScrollView(Data.ElementsScrollPos, "Box", GUILayout.Height(Data.ElementsScrollViewHeight));
            if (_brush.IsEmpty) EditorGUILayout.HelpBox("There are no brush elements available. You can drag and drop prefabs onto this area or " +
                                                        "right click on prefabs inside the active category to create new elements. All brush elements will " + 
                                                        "be shown with a preview inside this area.", UnityEditor.MessageType.None);
            else
            {             
                List<DecorPaintObjectPlacementBrushElement> allBrushElements = _brush.GetAllBrushElements();
                for (int brushElemIndex = 0; brushElemIndex < allBrushElements.Count; ++brushElemIndex )
                {
                    if(brushElemIndex % Data.NumElementsPerRow == 0)
                    {
                        if (brushElemIndex != 0) EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }

                    DecorPaintObjectPlacementBrushElement brushElement = allBrushElements[brushElemIndex];
                    var previewButtonRenderData = new PrefabPreviewButtonRenderData();
                    previewButtonRenderData.ExtractFromPrefab(brushElement.Prefab, Data.ElementPreviewScale);

                    Color previewTint = brushElement != _brush.ActiveElement ? Color.white : Data.ActiveElementTintColor;
                    if (_brush.ActiveElement != brushElement && !brushElement.IsEnabled) previewTint = Data.DisabledElementTintColor;

                    EditorGUILayout.BeginVertical(GUILayout.Width(previewButtonRenderData.ButtonWidth));
                    EditorGUIColor.Push(previewTint);
                    if(EditorGUILayoutEx.PrefabPreview(brushElement.Prefab, true, previewButtonRenderData))
                    {
                        if(Event.current.button == (int)MouseButton.Left)
                        {
                            if (brushElement != _brush.ActiveElement)
                            {
                                UndoEx.RecordForToolAction(_brush);
                                _brush.SetActiveElement(brushElement);
                            }
                        }
                        else
                        if (Event.current.button == (int)MouseButton.Right)
                        {
                            if(!Event.current.shift)
                            {
                                UndoEx.RecordForToolAction(_brush);
                                _brush.RemoveAndDestroyElement(brushElement);
                            }
                            else
                            {
                                UndoEx.RecordForToolAction(brushElement);
                                brushElement.IsEnabled = !brushElement.IsEnabled;
                            }
                        }
                    }
                    EditorGUIColor.Pop();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            Rect prefabDropRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.BeginHorizontal();
            var content = new GUIContent();
            content.text = "Load active category";
            content.tooltip = "Loads all the prefabs from the active category inside the active brush. Note: Prefabs which already exist in the brush, will be ignored.";
            if(GUILayout.Button(content, GUILayout.Width(130.0f)))
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.LoadAllPrefabsInActiveCategory();
            }
            RenderRemoveAllElementsButton();

            content.text = "Look and feel...";
            content.tooltip = "Opens up a new window which allows you to control the look and feel of the brush elements view.";
            if(GUILayout.Button(content, GUILayout.Width(110.0f)))
            {
                Octave3DWorldBuilder.ActiveInstance.EditorWindowPool.DecorPaintBrushViewLookAndFeelWindow.ViewData = Data;
                Octave3DWorldBuilder.ActiveInstance.EditorWindowPool.DecorPaintBrushViewLookAndFeelWindow.ShowOctave3DWindow();
            }
            EditorGUILayout.EndHorizontal();

            if (_brush.ActiveElement != null)
            {
                EditorGUILayout.Separator();
                _brush.ActiveElement.View.Render();
            }

            PrefabsToDecorPaintBrushEventHandler.Get().DropDest = PrefabsToDecorPaintBrushEventHandler.DropDestination.Brush;
            PrefabsToDecorPaintBrushEventHandler.Get().DestinationBrush = _brush;
            PrefabsToDecorPaintBrushEventHandler.Get().Handle(Event.current, prefabDropRect);
        }
        #endregion

        #region Private Methods
        private void RenderNameChangeField()
        {
            string newString = EditorGUILayout.TextField(GetContentForNameChangeField(), _brush.Name);
            if(newString != _brush.Name)
            {
                UndoEx.RecordForToolAction(_brush);
                DecorPaintObjectPlacementBrushDatabase.Get().RenameBrush(_brush, newString);
            }
        }

        private GUIContent GetContentForNameChangeField()
        {
            var content = new GUIContent();
            content.text = "Name";
            content.tooltip = "Allows you to change the name of the brush.";

            return content;
        }

        private void RenderRadiusField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForRadiusField(), _brush.Radius);
            if(newFloat != _brush.Radius)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.Radius = newFloat;
            }
        }

        private GUIContent GetContentForRadiusField()
        {
            var content = new GUIContent();
            content.text = "Radius";
            content.tooltip = "Allows you to change the brush radius.";

            return content;
        }

        private void RenderMaxNumberOfObjectsField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForMaxNumberOfObjectsField(), _brush.MaxNumberOfObjects);
            if(newInt != _brush.MaxNumberOfObjects)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.MaxNumberOfObjects = newInt;
            }
        }

        private GUIContent GetContentForMaxNumberOfObjectsField()
        {
            var content = new GUIContent();
            content.text = "Max objects";
            content.tooltip = "This is the maximum number of objects which can be placed inside the brush.";

            return content;
        }

        private void RenderDistanceBetweenObjectsField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForDistanceBetweenObjectsField(), _brush.DistanceBetweenObjects);
            if(newFloat != _brush.DistanceBetweenObjects)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.DistanceBetweenObjects = newFloat;
            }
        }

        private GUIContent GetContentForDistanceBetweenObjectsField()
        {
            var content = new GUIContent();
            content.text = "Distance between objects";
            content.tooltip = "This is the distance between objects in world units.";

            return content;
        }

        private void RenderIgnoreObjectsOutsideOfPaintSurfaceToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForIgnoreObjectsOutsideOfPaintSurfaceToggle(), _brush.IgnoreObjectsOutsideOfPaintSurface);
            if(newBool != _brush.IgnoreObjectsOutsideOfPaintSurface)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.IgnoreObjectsOutsideOfPaintSurface = newBool;
            }
        }

        private GUIContent GetContentForIgnoreObjectsOutsideOfPaintSurfaceToggle()
        {
            var content = new GUIContent();
            content.text = "Ignore objects outside of paint surface";
            content.tooltip = "If this is checked, objects whose positions are generated outside the paint surface will not be placed in the scene. " + 
                              "Note: This only applies to mesh surfaces. For terrians, outside objects will always be ignored.";

            return content;
        }

        private void RenderDestinationCategoryForElementPrefabsSelectionPopup()
        {
            string newString = EditorGUILayoutEx.Popup(GetContentForDestinationCategoryForElementPrefabsSelectionPopup(), _brush.DestinationCategoryForElementPrefabs.Name, PrefabCategoryDatabase.Get().GetAllPrefabCategoryNames());
            if(newString != _brush.DestinationCategoryForElementPrefabs.Name)
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.DestinationCategoryForElementPrefabs = PrefabCategoryDatabase.Get().GetPrefabCategoryByName(newString);
            }
        }

        private GUIContent GetContentForDestinationCategoryForElementPrefabsSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Destination prefab category";
            content.tooltip = "When a prefab is associated with a brush element, that prefab will be assigned to this category.";

            return content;
        }

        private void RenderRemoveAllElementsButton()
        {
            if (GUILayout.Button(GetContentForRemoveAllElementsButton(), GUILayout.Width(140.0f)))
            {
                UndoEx.RecordForToolAction(_brush);
                _brush.RemoveAndDestroyAllElements();
            }
        }

        private GUIContent GetContentForRemoveAllElementsButton()
        {
            var content = new GUIContent();
            content.text = "Remove all elements";
            content.tooltip = "Removes all elements from the brush.";

            return content;
        }
        #endregion
    }
}
#endif