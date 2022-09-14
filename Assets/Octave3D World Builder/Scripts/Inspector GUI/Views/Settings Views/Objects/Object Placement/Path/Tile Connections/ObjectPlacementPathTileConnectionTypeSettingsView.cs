#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathTileConnectionTypeSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementPathTileConnectionTypeSettings _settings;
        #endregion

        #region Public Static Properties
        public static float TileConnectionPreviewButtonSize { get { return 128.0f; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathTileConnectionTypeSettingsView(ObjectPlacementPathTileConnectionTypeSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            EditorGUILayoutEx.BeginVerticalBox();
            RenderTileConnectionTypeBoxContent();
            EditorGUILayoutEx.EndVerticalBox();
        }
        #endregion

        #region Private Methods
        private void RenderTileConnectionTypeBoxContent()
        {
            EditorGUILayout.BeginHorizontal();
            RenderTileConnectionTypeControls();
            RenderTileConnectionTypePreview();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderTileConnectionTypeControls()
        {
            EditorGUILayout.BeginVertical();
            RenderYAxisRotationSelectionPopup();
            RenderYOffsetField();
            RenderExtrudeControls();
            RenderRemovePrefabAssociationButton();
            EditorGUILayout.EndVertical();
        }

        private void RenderYAxisRotationSelectionPopup()
        {
            ObjectPlacementPathTileConnectionYAxisRotation newYAxisRotation = (ObjectPlacementPathTileConnectionYAxisRotation)EditorGUILayout.EnumPopup(GetContentForYAxisRotationSelectionPopup(), _settings.YAxisRotation);
            if(newYAxisRotation != _settings.YAxisRotation)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.YAxisRotation = newYAxisRotation;
            }
        }

        private GUIContent GetContentForYAxisRotationSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Y axis rotation";
            content.tooltip = "This is the amount of rotation to apply around the tile's Y axis. Note: This rotation value is applied relative to the identity rotation. " + 
                              "Any existing rotation information inside the tile's prefab is ignored.";

            return content;
        }

        private void RenderYOffsetField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForYOffsetField(), _settings.YOffset);
            if(newFloat != _settings.YOffset)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.YOffset = newFloat;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForYOffsetField()
        {
            var content = new GUIContent();
            content.text = "Y offset";
            content.tooltip = "Allows you to control the tile connection Y offset relative to the plane on which it resides.";

            return content;
        }

        private void RenderExtrudeControls()
        {
            RenderUpwardsExtrudeAmountField();
            RenderDownwardsExtrudeAmountField();
        }

        private void RenderUpwardsExtrudeAmountField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForUpwardsExtrudeAmountField(), _settings.UpwardsExtrusionAmount);
            if(newInt != _settings.UpwardsExtrusionAmount)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.UpwardsExtrusionAmount = newInt;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForUpwardsExtrudeAmountField()
        {
            var content = new GUIContent();
            content.text = "Upwards extrusion amount";
            content.tooltip = "Allows you to control how much the tile extrudes along the direction of the path plane normal.";

            return content;
        }

        private void RenderDownwardsExtrudeAmountField()
        {
            int newInt = EditorGUILayout.IntField(GetContentForDownwardsExtrudeAmountField(), _settings.DownwardsExtrusionAmount);
            if (newInt != _settings.DownwardsExtrusionAmount)
            {
                UndoEx.RecordForToolAction(_settings);
                _settings.DownwardsExtrusionAmount = newInt;

                SceneView.RepaintAll();
            }
        }

        private GUIContent GetContentForDownwardsExtrudeAmountField()
        {
            var content = new GUIContent();
            content.text = "Downwards extrusion amount";
            content.tooltip = "Allows you to control how much the tile extrudes along the reverse of the direction of the path plane normal.";

            return content;
        }

        private void RenderTileConnectionTypePreview()
        {
            GUILayout.Button(GetContentForTileConnectionTypePreviewButton(),
                             GetStyleForTileConnectionTypePreviewButton(),
                             GUILayout.Width(TileConnectionPreviewButtonSize), GUILayout.Height(TileConnectionPreviewButtonSize));
            Rect tileConnectionButtonRect = GUILayoutUtility.GetLastRect();
            RenderTileConnectionTypePreviewLabel(tileConnectionButtonRect);

            PrefabsToPathTileConectionDropEventHandler.Get().DestinationTileConnectionType = _settings.TileConnectionType;
            PrefabsToPathTileConectionDropEventHandler.Get().Handle(Event.current, tileConnectionButtonRect);
        }

        private void RenderTileConnectionTypePreviewLabel(Rect previewRect)
        {
            string labelText = _settings.TileConnectionType.ToString();
            EditorGUILayoutEx.LabelInMiddleOfControlRect(previewRect, labelText, TileConnectionPreviewButtonSize, GetStyleForTileConnectionTypePreviewLabel());
        }

        private GUIStyle GetStyleForTileConnectionTypePreviewLabel()
        {
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.black;

            return style;
        }

        private GUIContent GetContentForTileConnectionTypePreviewButton()
        {
            var content = new GUIContent();
            content.text = "";

            if(_settings.Prefab == null)
            {
                content.tooltip = "Drop a prefab here to associate it with this tile connection type.";
                content.image = TextureCache.Get().GetTextureAtRelativePath(ObjectPlacementPathTileConnectionTypeTexturePaths.GetRelativeTexturePathForTileConnectionType(_settings.TileConnectionType));
            }
            else
            {
                content.tooltip = _settings.Prefab.Name;
                content.image = PrefabPreviewTextureCache.Get().GetPrefabPreviewTexture(_settings.Prefab);
            }

            return content;
        }

        private GUIStyle GetStyleForTileConnectionTypePreviewButton()
        {
            var style = new GUIStyle("Box");
            return style;
        }

        private void RenderRemovePrefabAssociationButton()
        {
            if (GUILayout.Button(GetContentForRemovePrefabAssociationButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                if (!ObjectPlacementPathTileConnectionSettingsChangeValidation.Validate(true)) return;

                UndoEx.RecordForToolAction(_settings);
                _settings.Prefab = null;
            }
        }

        private GUIContent GetContentForRemovePrefabAssociationButton()
        {
            var content = new GUIContent();
            content.text = "Remove prefab association";
            content.tooltip = "Removes the association between this tile and it's prefab.";

            return content;
        }
        #endregion
    }
}
#endif