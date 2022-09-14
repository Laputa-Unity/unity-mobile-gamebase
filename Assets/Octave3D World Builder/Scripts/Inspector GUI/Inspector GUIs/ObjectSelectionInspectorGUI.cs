#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionInspectorGUI : InspectorGUI
    {
        #region Private Constant Variables
        private const float _selectionGizmoSpecificPopupWidth = 68.0f;
        #endregion

        #region Private Variables
        [SerializeField]
        private ObjectSelectionTransformGizmoSelectionToolbar _objectSelectionTransformGizmoSelectionToolbar = new ObjectSelectionTransformGizmoSelectionToolbar();
        [SerializeField]
        private ObjectSelectionActionToolbar _objectSelectionActionsToolbar = new ObjectSelectionActionToolbar();
        [SerializeField]
        private ObjectSelectionActionsView _objectSelectionActionsView = new ObjectSelectionActionsView();

        [SerializeField]
        private ObjectSelectionLookAndFeelSettingsView _lookAndFeelSettingsView = new ObjectSelectionLookAndFeelSettingsView();
        #endregion

        #region Public Methods
        public override void Initialize()
        {
            _lookAndFeelSettingsView.IsVisible = false;

            RectangleShapeRenderSettingsView rectangleShapeRenderSettingsView = ObjectSelection.Get().RectangleSelectionShapeRenderSettings.View;
            rectangleShapeRenderSettingsView.ToggleVisibilityBeforeRender = true;
            rectangleShapeRenderSettingsView.IndentContent = true;
            rectangleShapeRenderSettingsView.VisibilityToggleLabel = "Rectangle Selection Shape";

            EllipseShapeRenderSettingsView ellipseShapeRenderSettingsView = ObjectSelection.Get().EllipseSelectionShapeRenderSettings.View;
            ellipseShapeRenderSettingsView.ToggleVisibilityBeforeRender = true;
            ellipseShapeRenderSettingsView.IndentContent = true;
            ellipseShapeRenderSettingsView.VisibilityToggleLabel = "Ellipse Selection Shape";

            ObjectSelection.Get().MirrorView.IsVisible = false;
            InteractableMirrorSettings mirrorSettings = ObjectSelection.Get().MirrorSettings;
            mirrorSettings.View.IsVisible = false;
            mirrorSettings.View.ToggleVisibilityBeforeRender = true;
            mirrorSettings.View.VisibilityToggleLabel = "More settings";
            mirrorSettings.View.VisibilityToggleIndent = 1;
            mirrorSettings.View.IndentContent = true;
            mirrorSettings.KeyboardRotationSettings.XAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.KeyboardRotationSettings.XAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.KeyboardRotationSettings.YAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.KeyboardRotationSettings.YAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.KeyboardRotationSettings.ZAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.KeyboardRotationSettings.ZAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.KeyboardRotationSettings.CustomAxisRotationSettings.View.VisibilityToggleLabel = "Hover Surface Normal Settings";

            mirrorSettings.KeyboardRotationSettings.XAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.KeyboardRotationSettings.YAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.KeyboardRotationSettings.ZAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.KeyboardRotationSettings.CustomAxisRotationSettings.View.IsVisible = false;

            mirrorSettings.MouseRotationSettings.XAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.MouseRotationSettings.XAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.MouseRotationSettings.YAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.MouseRotationSettings.YAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.MouseRotationSettings.ZAxisRotationSettings.View.VisibilityToggleLabel = mirrorSettings.MouseRotationSettings.ZAxisRotationSettings.RotationAxis.ToString() + " Axis Settings";
            mirrorSettings.MouseRotationSettings.CustomAxisRotationSettings.View.VisibilityToggleLabel = "Hover Surface Normal Settings";

            mirrorSettings.MouseRotationSettings.XAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.MouseRotationSettings.YAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.MouseRotationSettings.ZAxisRotationSettings.View.IsVisible = false;
            mirrorSettings.MouseRotationSettings.CustomAxisRotationSettings.View.IsVisible = false;

            InteractableMirrorRenderSettingsView mirrorRenderSettingsView = ObjectSelection.Get().MirrorRenderSettings.View;
            mirrorRenderSettingsView.VisibilityToggleLabel = "Look and feel";
            mirrorRenderSettingsView.ToggleVisibilityBeforeRender = true;
            mirrorRenderSettingsView.IndentContent = true;
            mirrorRenderSettingsView.VisibilityToggleIndent = 1;
            mirrorRenderSettingsView.IsVisible = false;
        }

        public override void Render()
        {
            RenderSelectionTransformGizmoSystemControls();

            _objectSelectionActionsToolbar.Render();
            _objectSelectionActionsView.Render();

            ObjectSelection.Get().MirrorView.Render();
            Octave3DWorldBuilder.ActiveInstance.PlacementObjectGroupDatabase.View.Render();
            ObjectSelectionSettings.Get().View.Render();
            ObjectSelectionPrefabCreationSettings.Get().View.Render();
            ObjectSelection.Get().ObjectOnSurfaceProjectSettings.RenderView();
            _lookAndFeelSettingsView.Render();
        }
        #endregion

        #region Private Methods
        private void RenderSelectionTransformGizmoSystemControls()
        {
            bool newBool;
            if (ObjectSelection.Get().ObjectSelectionGizmos.AreGizmosActive)
            {
                _objectSelectionTransformGizmoSelectionToolbar.Render();
                EditorGUILayout.BeginHorizontal();
                RenderGizmoTransformPivotPointSelectionPopup();
                RenderGizmoTransformSpaceSelectionPopup();

                var content = new GUIContent("Toggle gizmos", "Allows you to toggle gizmos on/off.");
                newBool = EditorGUILayout.ToggleLeft(content, ObjectSelection.Get().ObjectSelectionGizmos.AreGizmosActive);
                if (newBool != ObjectSelection.Get().ObjectSelectionGizmos.AreGizmosActive)
                {
                    UndoEx.RecordForToolAction(ObjectSelection.Get());
                    ObjectSelection.Get().ObjectSelectionGizmos.AreGizmosActive = !ObjectSelection.Get().ObjectSelectionGizmos.AreGizmosActive;
                    Octave3DWorldBuilder.ActiveInstance.Inspector.EditorWindow.Repaint();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                string helpMessage = "Gizmos are currently turned off.";
                EditorGUILayout.HelpBox(helpMessage, UnityEditor.MessageType.Info);

                var content = new GUIContent("Toggle gizmos", "Allows you to toggle gizmos on/off.");
                newBool = EditorGUILayout.ToggleLeft(content, ObjectSelection.Get().ObjectSelectionGizmos.AreGizmosActive);
                if (newBool != ObjectSelection.Get().ObjectSelectionGizmos.AreGizmosActive)
                {
                    UndoEx.RecordForToolAction(ObjectSelection.Get());
                    ObjectSelection.Get().ObjectSelectionGizmos.AreGizmosActive = !ObjectSelection.Get().ObjectSelectionGizmos.AreGizmosActive;
                    Octave3DWorldBuilder.ActiveInstance.Inspector.EditorWindow.Repaint();
                }
            }
        }

        private void RenderGizmoTransformSpaceSelectionPopup()
        {
            ObjectSelectionGizmos selectionGizmos = ObjectSelection.Get().ObjectSelectionGizmos;
            TransformSpace newTransformSpace = (TransformSpace)EditorGUILayout.EnumPopup(GetContentForGizmoTransformSpaceSelectionPopup(), selectionGizmos.GizmoTransformSpace, GUILayout.Width(_selectionGizmoSpecificPopupWidth));
            if(newTransformSpace != selectionGizmos.GizmoTransformSpace)
            {
                UndoEx.RecordForToolAction(selectionGizmos);
                selectionGizmos.GizmoTransformSpace = newTransformSpace;
            }
        }

        private GUIContent GetContentForGizmoTransformSpaceSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "Transform space: " + ObjectSelection.Get().ObjectSelectionGizmos.GizmoTransformSpace;

            return content;
        }

        private void RenderGizmoTransformPivotPointSelectionPopup()
        {
            ObjectSelectionGizmos selectionGizmos = ObjectSelection.Get().ObjectSelectionGizmos;
            TransformGizmoPivotPoint newPivotPoint = (TransformGizmoPivotPoint)EditorGUILayout.EnumPopup(GetContentForGizmoTransformPivotPointSelectionPopup(), selectionGizmos.GizmoTransformPivotPoint, GUILayout.Width(_selectionGizmoSpecificPopupWidth));
            if (newPivotPoint != selectionGizmos.GizmoTransformPivotPoint)
            {
                UndoEx.RecordForToolAction(selectionGizmos);
                selectionGizmos.GizmoTransformPivotPoint = newPivotPoint;
            }
        }

        private GUIContent GetContentForGizmoTransformPivotPointSelectionPopup()
        {
            var content = new GUIContent();
            content.text = "";
            content.tooltip = "Pivot point: " + ObjectSelection.Get().ObjectSelectionGizmos.GizmoTransformPivotPoint;

            return content;
        }
        #endregion
    }
}
#endif