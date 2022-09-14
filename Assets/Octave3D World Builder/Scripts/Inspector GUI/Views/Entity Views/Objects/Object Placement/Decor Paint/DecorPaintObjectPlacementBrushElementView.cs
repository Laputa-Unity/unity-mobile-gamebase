#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushElementView : EntityView
    {
        #region Private Constant Variables
        private const float _prefabPreviewSize = 128.0f;
        #endregion

        #region Private Variables
        [NonSerialized]
        private DecorPaintObjectPlacementBrushElement _brushElement;
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrushElementView(DecorPaintObjectPlacementBrushElement brushElement)
        {
            _brushElement = brushElement;

            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            EditorGUILayout.BeginHorizontal();
            RenderPrefabPreviewBox();
            EditorGUILayout.BeginVertical();
            RenderAlignToSurfaceToggle();

            var content = new GUIContent();
            content.text = "Offset from surface";
            content.tooltip = "Allows you to control the element's offset from the surface on which it resides.";
            float newFloat = EditorGUILayout.FloatField(content, _brushElement.OffsetFromSurface);
            if (newFloat != _brushElement.OffsetFromSurface)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.OffsetFromSurface = newFloat;
            }

            if (_brushElement.AlignToSurface) RenderAlignmentAxisSelectionPopup();
            if (_brushElement.AlignToSurface) RenderRotationOffsetSlider();
            RenderSpawnChanceField();
            RenderAlignToStrokeToggle();
            RenderRotationRandomizationModePopup();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            _brushElement.ScaleRandomizationSettings.View.Render();
            if (!_brushElement.ScaleRandomizationSettings.RandomizeScale) RenderScaleField();
            _brushElement.SlopeSettings.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderPrefabPreviewBox()
        {
            GUILayout.Box(GetContentForPrefabPreviewBox(),
                             GetStyleForPrefabPreviewBox(), GUILayout.Width(_prefabPreviewSize), GUILayout.Height(_prefabPreviewSize));

            Rect previewBoxRect = GUILayoutUtility.GetLastRect();
            RenderIsElementEnabledToggle(previewBoxRect);

            PrefabsToDecorPaintBrushEventHandler.Get().DropDest = PrefabsToDecorPaintBrushEventHandler.DropDestination.Element;
            PrefabsToDecorPaintBrushEventHandler.Get().DestinationDecorPaintBrushElement = _brushElement;
            PrefabsToDecorPaintBrushEventHandler.Get().Handle(Event.current, previewBoxRect);
        }

        private GUIContent GetContentForPrefabPreviewBox()
        {
            var content = new GUIContent();
            content.text = "";

            if (_brushElement.Prefab == null)
            {
                content.text = "(No prefab)";
                content.tooltip = "Drop a prefab here to associate it with this brush element.";
                content.image = null;
            }
            else
            {
                content.tooltip = _brushElement.Prefab.Name;
                content.image = PrefabPreviewTextureCache.Get().GetPrefabPreviewTexture(_brushElement.Prefab);
            }

            return content;
        }

        private void RenderIsElementEnabledToggle(Rect elementPreviewBoxRect)
        {
            elementPreviewBoxRect.x += 5.0f;
            elementPreviewBoxRect.y -= elementPreviewBoxRect.height * 0.4f;
            bool newBool = GUI.Toggle(elementPreviewBoxRect, _brushElement.IsEnabled, GetContentForIsElementActiveToggle());
            if(newBool != _brushElement.IsEnabled)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.IsEnabled = newBool;
            }
        }

        private GUIContent GetContentForIsElementActiveToggle()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "If this is checked, the element wil be taken into acount during object placement. Otherwise, it will be ignored.";

            return content;
        }

        private GUIStyle GetStyleForPrefabPreviewBox()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private void RenderAlignToSurfaceToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAlignToSurfaceToggle(), _brushElement.AlignToSurface);
            if(newBool != _brushElement.AlignToSurface)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.AlignToSurface = newBool;
            }
        }

        private GUIContent GetContentForAlignToSurfaceToggle()
        {
            var content = new GUIContent();
            content.text = "Align to surface";
            content.tooltip = "If this is checked, objects will be aligned to the surface on which they sit based on the specified alignment axis.";

            return content;
        }

        private void RenderAlignmentAxisSelectionPopup()
        {
            CoordinateSystemAxis newAxis = (CoordinateSystemAxis)EditorGUILayout.EnumPopup(GetContentForAlignmentAxisSelectionPopup(), _brushElement.AlignmentAxis);
            if(newAxis != _brushElement.AlignmentAxis)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.AlignmentAxis = newAxis;
            }
        }

        private GUIContent GetContentForAlignmentAxisSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Alingment axis";
            content.tooltip = "Allows you to specify which of the object's local axes must be aligned with the paint surface normal.";

            return content;
        }

        private void RenderRotationOffsetSlider()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForRotationOffsetSlider(), _brushElement.RotationOffsetInDegrees, DecorPaintObjectPlacementBrushElement.MinRotationOffsetInDegrees, DecorPaintObjectPlacementBrushElement.MaxRotationOffsetInDegrees);
            if(newFloat != _brushElement.RotationOffsetInDegrees)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.RotationOffsetInDegrees = newFloat;
            }
        }

        private GUIContent GetContentForRotationOffsetSlider()
        {
            var content = new GUIContent();
            content.text = "Rotation offset";
            content.tooltip = "This is a rotation offset in degrees around the specified alignment axis. " + 
                              "This is useful when the default rotation of the prefab is not satisfactory and can come in handy when stroke alignment " + 
                              "turned on.";

            return content;
        }

        private void RenderSpawnChanceField()
        {
            float newFloat = EditorGUILayout.Slider(GetContentForSpawnChanceSlider(), _brushElement.SpawnChance, DecorPaintObjectPlacementBrushElement.MinSpawnChance, DecorPaintObjectPlacementBrushElement.MaxSpawnChance);
            if(newFloat != _brushElement.SpawnChance)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.SpawnChance = newFloat;
            }
        }

        private GUIContent GetContentForSpawnChanceSlider()
        {
            var content = new GUIContent();
            content.text = "Spawn chance";
            content.tooltip = "Allows you to control the probability that this element will be spawned during painting. The bigger the value, " + 
                              "the bigger the chance that the element will be picked for spawning.";

            return content;
        }

        private void RenderAlignToStrokeToggle()
        {
            bool newBool = EditorGUILayout.ToggleLeft(GetContentForAlignToStrokeToggle(), _brushElement.AlignToStroke);
            if(newBool != _brushElement.AlignToStroke)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.AlignToStroke = newBool;
            }
        }

        private GUIContent GetContentForAlignToStrokeToggle()
        {
            var content = new GUIContent();
            content.text = "Align to stroke";
            content.tooltip = "If this is checked, the prefab's rotation will be adjusted such that it follows the stroke travel direction. " + 
                              "Note: If rotation randomization is also turned on, a small random rotation offset will also be applied.";

            return content;
        }

        private void RenderRotationRandomizationModePopup()
        {
            BrushElementRotationRandomizationMode newMode = (BrushElementRotationRandomizationMode)EditorGUILayout.EnumPopup(GetContentForRotationRandomizationModePopup(), _brushElement.RotationRandomizationMode);
            if (newMode != _brushElement.RotationRandomizationMode)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.RotationRandomizationMode = newMode;
            }
        }

        private GUIContent GetContentForRotationRandomizationModePopup()
        {
            var content = new GUIContent();
            content.text = "Rotation randomization mode";
            content.tooltip = "Allows you to specify an axis for rotation randomization. If \'Align to surface\' is checked, it is recommended that you specify \'Surface Normal\' as the axis of rotation. " + 
                              "You can specify \'None\' if you don't want to randomize rotations. ";

            return content;
        }

        private void RenderScaleField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForScaleField(), _brushElement.Scale);
            if(newFloat != _brushElement.Scale)
            {
                UndoEx.RecordForToolAction(_brushElement);
                _brushElement.Scale = newFloat;
            }
        }

        private GUIContent GetContentForScaleField()
        {
            var content = new GUIContent();
            content.text = "Scale";
            content.tooltip = "When scale randomization is turned off, you can use this to control the scale of the objects.";

            return content;
        }
        #endregion
    }
}
#endif