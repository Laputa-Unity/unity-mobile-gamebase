#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectOnSurfaceProjectSettings : ScriptableObject
    {
        public enum ProjectionDirection
        {
            Down = 0,
            Up
        }

        [SerializeField]
        private bool _isVisible = true;
        private bool _projectOnGrid = false;
        [SerializeField]
        private GameObject _projectionSurface;
        [SerializeField]
        private ProjectionDirection _projectionDirection = ProjectionDirection.Down;

        public bool ProjectOnGrid { get { return _projectOnGrid; } set { _projectOnGrid = value; } }
        public GameObject ProjectionSurface
        {
            get { return _projectionSurface; }
            set
            {
                if (CanBeUsedAsProjectionSurface(value)) _projectionSurface = value;
                else Debug.LogWarning("Invalid projection surface. The projection surface object must be a scene object (not prefab) " + 
                                      "and it must be either a terrain object with a terrain collider or mesh object. If it is a mesh object, it must be a child of Octave3D.");
            }
        }

        public ProjectionDirection ProjectionDir { get { return _projectionDirection; } set { _projectionDirection = value; } }

        public bool CanBeUsedAsProjectionSurface(GameObject gameObject)
        {
            if (gameObject.HasTerrain()) return gameObject.GetComponent<TerrainCollider>() != null;
            else
            if (gameObject.HasMesh()) return Octave3DWorldBuilder.ActiveInstance.IsWorkingObject(gameObject);

            return false;
        }

        public Vector3 GetProjectionDirectionVector()
        {
            return _projectionDirection == ProjectionDirection.Down ? -Vector3.up : Vector3.up;
        }

        public void RenderView()
        {
            bool newBool;

            newBool = EditorGUILayout.Foldout(_isVisible, "Selection Projection Settings");
            if(newBool != _isVisible)
            {
                UndoEx.RecordForToolAction(this);
                _isVisible = newBool;
            }

            GUIContent content = new GUIContent();
            if(_isVisible)
            {
                EditorGUILayoutEx.BeginVerticalBox();

                content.text = "Project on grid";
                content.tooltip = "If this is checked, the projection will be done on the scene grid.";
                newBool = EditorGUILayout.ToggleLeft(content, ProjectOnGrid);
                if(newBool != ProjectOnGrid)
                {
                    UndoEx.RecordForToolAction(this);
                    ProjectOnGrid = newBool;
                }

                if(!ProjectOnGrid)
                {
                    Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("The surface projection object must be a terrain object (with a terrain collider) or mesh (no colliders required). " +
                                                                    "If it is a mesh, it must also be a child of Octave3D.");
                    content.text = "Surface object";
                    content.tooltip = "The object which acts as a projection surface. Must be a terrain (with a mesh collider attached) or mesh object. If it is a mesh object, it must be a child of Octave3D.";
                    GameObject newSurface = EditorGUILayout.ObjectField(content, ProjectionSurface, typeof(GameObject), true) as GameObject;
                    if (newSurface != _projectionSurface)
                    {
                        UndoEx.RecordForToolAction(this);
                        ProjectionSurface = newSurface;
                    }
                }

                content.text = "Projection direction";
                content.tooltip = "The direction in which the objects will be projected on the projection surface.";
                ProjectionDirection newProjectionDir = (ProjectionDirection)EditorGUILayout.EnumPopup(content, ProjectionDir);
                if(newProjectionDir != ProjectionDir)
                {
                    UndoEx.RecordForToolAction(this);
                    ProjectionDir = newProjectionDir;
                }

                content.text = "Project selection";
                content.tooltip = "Projects the selected objects onto the projection surface (if one was specified).";
                if(GUILayout.Button(content, GUILayout.Width(130.0f)))
                {
                    if (ProjectionSurface == null && !ProjectOnGrid) Debug.LogWarning("Projection not possible. No projection surface was specified!");
                    else ObjectSelection.Get().ProjectSelectionOnProjectionSurface();
                }
                EditorGUILayoutEx.EndVerticalBox();
            }
        }
    }
}
#endif