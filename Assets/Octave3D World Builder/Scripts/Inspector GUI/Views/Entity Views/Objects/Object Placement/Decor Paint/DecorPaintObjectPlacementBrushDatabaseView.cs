#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintObjectPlacementBrushDatabaseView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private DecorPaintObjectPlacementBrushDatabase _database;

        [SerializeField]
        private DecorPaintObjectPlacementBrushDatabaseViewData _viewData;
        #endregion

        #region Private Properties
        private DecorPaintObjectPlacementBrushDatabaseViewData ViewData
        {
            get
            {
                if (_viewData == null) _viewData = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<DecorPaintObjectPlacementBrushDatabaseViewData>();
                return _viewData;
            }
        }
        #endregion

        #region Constructors
        public DecorPaintObjectPlacementBrushDatabaseView(DecorPaintObjectPlacementBrushDatabase database)
        {
            _database = database;

            SurroundWithBox = true;
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Decor Paint Brushes";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            if (_database.IsEmpty) EditorGUILayout.HelpBox("There are no brushes currently available.", UnityEditor.MessageType.None);
            else RenderActiveBrushControls();

            EditorGUILayout.Separator();
            RenderBrushActionControls();
        }
        #endregion

        #region Private Methods
        private void RenderActiveBrushControls()
        {
            RenderActiveBrushSelectionPopup();

            EditorGUILayout.Separator();
            _database.ActiveBrush.View.Render();
        }

        private void RenderActiveBrushSelectionPopup()
        {
            string newString = EditorGUILayoutEx.Popup(GetContentForActiveBrushSelectionPopup(), _database.ActiveBrush.Name, _database.GetAllBrushNames());
            if(newString != _database.ActiveBrush.Name)
            {
                UndoEx.RecordForToolAction(_database);
                _database.SetActiveBrush(_database.GetBrushByName(newString));
            }
        }

        private GUIContent GetContentForActiveBrushSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "Active brush";
            content.tooltip = "Allows you to change the active brush.";

            return content;
        }

        private void RenderBrushActionControls()
        {
            RenderCreateNewBrushControls();
            RenderRemoveBrushButtons();
        }

        private void RenderCreateNewBrushControls()
        {
            EditorGUILayout.BeginHorizontal();
            RenderCreateNewBrushButton();
            RenderCreateNewBrushNameChangeField();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderRemoveBrushButtons()
        {
            EditorGUILayout.BeginHorizontal();
            RenderRemoveActiveBrushButton();
            RenderRemoveAllBrushesButton();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderCreateNewBrushButton()
        {
            if(GUILayout.Button(GetContentForCreateNewBrushButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_database);
                DecorPaintObjectPlacementBrush newBrush = _database.CreateBrush(ViewData.NameForNewBrush);
                if(newBrush != null) _database.SetActiveBrush(newBrush);
            }
        }

        private GUIContent GetContentForCreateNewBrushButton()
        {
            var content = new GUIContent();
            content.text = "Create brush";
            content.tooltip = "Creates a new brush using the name specified in the adjacent text field.";

            return content;
        }

        private void RenderCreateNewBrushNameChangeField()
        {
            string newString = EditorGUILayout.TextField(ViewData.NameForNewBrush);
            if(newString != ViewData.NameForNewBrush)
            {
                UndoEx.RecordForToolAction(ViewData);
                ViewData.NameForNewBrush = newString;
            }
        }

        private void RenderRemoveActiveBrushButton()
        {
            if(GUILayout.Button(GetContentForRemoveActivebrushButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth)))
            {
                UndoEx.RecordForToolAction(_database);
                _database.RemoveAndDestroyBrush(_database.ActiveBrush);
            }
        }

        private GUIContent GetContentForRemoveActivebrushButton()
        {
            var content = new GUIContent();
            content.text = "Remove active brush";
            content.tooltip = "Removes the active brush.";

            return content;
        }

        private void RenderRemoveAllBrushesButton()
        {
            if (GUILayout.Button(GetContentForRemoveAllBrushesButton()))
            {
                UndoEx.RecordForToolAction(_database);
                _database.RemoveAndDestroyAllBrushes();
            }
        }

        private GUIContent GetContentForRemoveAllBrushesButton()
        {
            var content = new GUIContent();
            content.text = "Remove all brushes";
            content.tooltip = "Removes all brushes.";

            return content;
        }
        #endregion
    }
}
#endif