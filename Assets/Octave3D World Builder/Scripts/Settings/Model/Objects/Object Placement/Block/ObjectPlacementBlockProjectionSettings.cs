#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    public enum ObjectBlockProjectionDir
    {
        BlockDown = 0,
        BlockUp
    }

    [Serializable]
    public class ObjectPlacementBlockProjectionSettings
    {
        [SerializeField]
        private bool _projectOnSurface = false;
        [SerializeField]
        private bool _rejectNonProjectables = true;
        [SerializeField]
        private ObjectBlockProjectionDir _projectionDirection = ObjectBlockProjectionDir.BlockDown;
        [SerializeField]
        private bool _canProjectOnTerrain = true;
        [SerializeField]
        private bool _canProjectOnMesh = true;
        [SerializeField]
        private bool _alignToSurfaceNormal = true;
        [SerializeField]
        private CoordinateSystemAxis _alignmentAxis = CoordinateSystemAxis.PositiveUp;

        public bool ProjectOnSurface { get { return _projectOnSurface; } set { _projectOnSurface = value; } }
        public bool RejectNonProjectables { get { return _rejectNonProjectables; } set { _rejectNonProjectables = value; } }
        public ObjectBlockProjectionDir ProjectionDirection { get { return _projectionDirection; } set { _projectionDirection = value; } }
        public bool CanProjectOnTerrain { get { return _canProjectOnTerrain; } set { _canProjectOnTerrain = value; } }
        public bool CanProjectOnMesh { get { return _canProjectOnMesh; } set { _canProjectOnMesh = value; } }
        public bool AlignToSurfaceNormal { get { return _alignToSurfaceNormal; } set { _alignToSurfaceNormal = value; } }
        public CoordinateSystemAxis AlignmentAxis { get { return _alignmentAxis; } set { _alignmentAxis = value; } }

        public void RenderGUI(UnityEngine.Object undoRecordObject)
        {
            bool newBool;
            ObjectBlockProjectionDir newProjectionDir;
            CoordinateSystemAxis newAlignmentAxis;

            EditorGUI.indentLevel += 1;
            EditorGUILayout.Foldout(true, "Block projection settings");
            EditorGUI.indentLevel -= 1;
            if (true)
            {
                EditorGUILayoutEx.BeginVerticalBox();

                var content = new GUIContent();
                content.text = "Project on surface";
                content.tooltip = "If this is checked, the objects in the block will be projected on a surface. The projection surface " +
                                  "is selected by casting a projection ray along the chosen projection direction.";
                newBool = EditorGUILayout.ToggleLeft(content, ProjectOnSurface);
                if (newBool != ProjectOnSurface)
                {
                    UndoEx.RecordForToolAction(undoRecordObject);
                    ProjectOnSurface = newBool;
                }

                if(ProjectOnSurface)
                {
                    content.text = "Reject non-projectables";
                    content.tooltip = "If this is checked, the plugin will not spawn objects which can not be projected on a surface.";
                    newBool = EditorGUILayout.ToggleLeft(content, RejectNonProjectables);
                    if (newBool != RejectNonProjectables)
                    {
                        UndoEx.RecordForToolAction(undoRecordObject);
                        RejectNonProjectables = newBool;
                    }

                    content.text = "Projection direction";
                    content.tooltip = "Allows you to specify the direction of projection.";
                    newProjectionDir = (ObjectBlockProjectionDir)EditorGUILayout.EnumPopup(content, ProjectionDirection);
                    if (newProjectionDir != ProjectionDirection)
                    {
                        UndoEx.RecordForToolAction(undoRecordObject);
                        ProjectionDirection = newProjectionDir;
                    }

                    content.text = "Can project on terrain";
                    content.tooltip = "If this is checked, objects can be projected onto terrain surfaces. Otherwise, terrain surfaces will be ignored.";
                    newBool = EditorGUILayout.ToggleLeft(content, CanProjectOnTerrain);
                    if (newBool != CanProjectOnTerrain)
                    {
                        UndoEx.RecordForToolAction(undoRecordObject);
                        CanProjectOnTerrain = newBool;
                    }

                    content.text = "Can project on mesh";
                    content.tooltip = "If this is checked, objects can be projected onto mesh surfaces. Otherwise, mesh surfaces will be ignored.";
                    newBool = EditorGUILayout.ToggleLeft(content, CanProjectOnMesh);
                    if (newBool != CanProjectOnMesh)
                    {
                        UndoEx.RecordForToolAction(undoRecordObject);
                        CanProjectOnMesh = newBool;
                    }

                    content.text = "Align to normal";
                    content.tooltip = "If this is checked, the objects will have one of their axes aligned with the projection surface.";
                    newBool = EditorGUILayout.ToggleLeft(content, AlignToSurfaceNormal);
                    if (newBool != AlignToSurfaceNormal)
                    {
                        UndoEx.RecordForToolAction(undoRecordObject);
                        AlignToSurfaceNormal = newBool;
                    }

                    content.text = "Alignment axis";
                    content.tooltip = "If surface alignment is turned on, this is the axis that will be used to align objects to the projection surface normal.";
                    newAlignmentAxis = (CoordinateSystemAxis)EditorGUILayout.EnumPopup(content, AlignmentAxis);
                    if (newAlignmentAxis != AlignmentAxis)
                    {
                        UndoEx.RecordForToolAction(undoRecordObject);
                        AlignmentAxis = newAlignmentAxis;
                    }
                }

                EditorGUILayoutEx.EndVerticalBox();
            }
        }
    }
}
#endif