#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class PrefabCategoryScrollViewLookAndFeelWindow : Octave3DEditorWindow
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _scrollViewPosition = Vector2.zero;
        [NonSerialized]
        private PrefabCategoryPrefabScrollViewData _lookAndFeelData;
        #endregion

        public PrefabCategoryPrefabScrollViewData LookAndFeelData { set { _lookAndFeelData = value; } }

        #region Public Methods
        public override string GetTitle()
        {
            return "Look and Feel";
        }

        public override void ShowOctave3DWindow()
        {
            ShowDockable(true);
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            if (_lookAndFeelData == null) return;

            Color newColor; int newInt; float newFloat; bool newBool;
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);

            var content = new GUIContent();
            content.text = "Active prefab tint color";
            content.tooltip = "Allows you to change the tint color for the active prefab.";
            newColor = EditorGUILayout.ColorField(content, _lookAndFeelData.ActivePrefabTint);
            if(newColor != _lookAndFeelData.ActivePrefabTint)
            {
                UndoEx.RecordForToolAction(_lookAndFeelData);
                _lookAndFeelData.ActivePrefabTint = newColor;
                Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
            }

            content.text = "Prefabs per row";
            content.tooltip = "Allows you to control the number of prefabs which reside in a single row insde the prefab category scroll view area.";
            newInt = EditorGUILayout.IntField(content, _lookAndFeelData.NumberOfPrefabsPerRow);
            if(newInt != _lookAndFeelData.NumberOfPrefabsPerRow)
            {
                UndoEx.RecordForToolAction(_lookAndFeelData);
                _lookAndFeelData.NumberOfPrefabsPerRow = newInt;
                Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
            }

            content.text = "Prefab preview scale";
            content.tooltip = "Allows you to control the scale of the prefab previews.";
            newFloat = EditorGUILayout.FloatField(content, _lookAndFeelData.PrefabPreviewScale);
            if(newFloat != _lookAndFeelData.PrefabPreviewScale)
            {
                UndoEx.RecordForToolAction(_lookAndFeelData);
                _lookAndFeelData.PrefabPreviewScale = newFloat;
                Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
            }

            content.text = "Prefab area height";
            content.tooltip = "Allows you to control the height of the area in which the prefabs are displayed.";
            newFloat = EditorGUILayout.FloatField(content, _lookAndFeelData.PrefabScrollViewHeight);
            if(newFloat != _lookAndFeelData.PrefabScrollViewHeight)
            {
                UndoEx.RecordForToolAction(_lookAndFeelData);
                _lookAndFeelData.PrefabScrollViewHeight = newFloat;
                Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
            }

            EditorGUILayout.Separator();
            content.text = "Show prefab names";
            content.tooltip = "If this is checked, each prefab preview will also contain a label with the name of the prefab.";
            newBool = EditorGUILayout.ToggleLeft(content, _lookAndFeelData.ShowPrefabNames);
            if(newBool != _lookAndFeelData.ShowPrefabNames)
            {
                UndoEx.RecordForToolAction(_lookAndFeelData);
                _lookAndFeelData.ShowPrefabNames = newBool;
                Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
            }

            if(_lookAndFeelData.ShowPrefabNames)
            {
                content.text = "Prefab name label color";
                content.tooltip = "Allows you to change the color of the labels that display the prefab names.";
                newColor = EditorGUILayout.ColorField(content, _lookAndFeelData.PrefabNameLabelColor);
                if (newColor != _lookAndFeelData.PrefabNameLabelColor)
                {
                    UndoEx.RecordForToolAction(_lookAndFeelData);
                    _lookAndFeelData.PrefabNameLabelColor = newColor;
                    Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        #endregion
    }
}
#endif