#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionExtrudeGizmo : ObjectGizmo
    {
        [SerializeField]
        private int[] _snapSteps = new int[] { 1, 1, 1 };

        public void SetSnapStep(int axisIndex, int value)
        {
            if (value <= 0 || axisIndex < 0) return;
            _snapSteps[axisIndex] = value;
        }

        public override void RenderHandles(TransformGizmoPivotPoint transformPivotPoint)
        {
            Box targetWorldAABB = Box.FromObjectWorldAABB(_targetObjects);

            //Color[] axesColors = new Color[] { Handles.xAxisColor, Handles.xAxisColor, Handles.yAxisColor, Handles.yAxisColor, Handles.zAxisColor, Handles.zAxisColor };
            //Vector3[] axesDirs = new Vector3[] { Vector3.right, -Vector3.right, Vector3.up, -Vector3.up, Vector3.forward, -Vector3.forward };
            //float[] snapValues = new float[] { targetWorldAABB.Size.x, targetWorldAABB.Size.x, targetWorldAABB.Size.y, targetWorldAABB.Size.y, targetWorldAABB.Size.z, targetWorldAABB.Size.z };

            Color[] axesColors = new Color[] { Handles.xAxisColor, Handles.yAxisColor, Handles.zAxisColor };
            Vector3[] axesDirs = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward };
            float[] snapValues = new float[] { targetWorldAABB.Size.x * _snapSteps[0], targetWorldAABB.Size.y * _snapSteps[1], targetWorldAABB.Size.z * _snapSteps[2] };

            Vector3 oldPosition = WorldPosition;
            for(int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                Handles.color = axesColors[axisIndex];
                Vector3 newGizmoPosition = Handles.Slider(oldPosition, axesDirs[axisIndex], HandleUtility.GetHandleSize(oldPosition), Handles.ArrowHandleCap, snapValues[axisIndex]);
                if (newGizmoPosition != oldPosition)
                {
                    Vector3 moveOffset = (newGizmoPosition - oldPosition);
                    WorldPosition = newGizmoPosition;
                    oldPosition = newGizmoPosition;

                    if (Event.current.control)
                    {
                        float absNumGroups = Mathf.Abs(moveOffset[axisIndex] / snapValues[axisIndex]);
                        float absFractional = absNumGroups - (int)absNumGroups;
                        int numCloneGroups = Mathf.FloorToInt(absNumGroups);
                        if (Mathf.Abs(absFractional - 1.0f) < 1e-4f) ++numCloneGroups;

                        for(int cloneGroupIndex = 0; cloneGroupIndex < numCloneGroups; ++cloneGroupIndex)
                        {
                            var clonedRoots = Octave3DWorldBuilder.ActiveInstance.GetRoots(ObjectActions.Duplicate(_targetObjects));
                            ObjectHierarchyRootsWerePlacedInSceneMessage.SendToInterestedListeners(clonedRoots, ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.Selection);
                            ObjectHierarchyRootsWerePlacedInSceneMessage.SendToInterestedListeners(ObjectSelection.Get().Mirror.MirrorGameObjectHierarchies(clonedRoots), ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.Selection);

                            if(cloneGroupIndex != 0)
                            {
                                Vector3 offset = axesDirs[axisIndex] * snapValues[axisIndex] * cloneGroupIndex * Mathf.Sign(moveOffset[axisIndex]);
                                foreach (var root in clonedRoots) root.transform.position += offset;
                            }
                        }
                    }

                    GameObjectExtensions.RecordObjectTransformsForUndo(_targetObjects);
                    var targetParents = GameObjectExtensions.GetParents(_targetObjects);
                    foreach (var parent in targetParents) parent.transform.position += moveOffset;
                }
            }

            Handles.BeginGUI();
            GUI.BeginGroup(new Rect(0.0f, -15.0f, 200.0f, 200.0f));

            var content = new GUIContent();
            string[] snapStepLabels = new string[] { "Snap step X", "Snap step Y", "Snap step Z" };

            for(int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                EditorGUILayout.BeginHorizontal();
                content.text = snapStepLabels[axisIndex];
                EditorGUILayout.LabelField(content, GUILayout.Width(80.0f));
                int newInt = EditorGUILayout.IntField("", _snapSteps[axisIndex], GUILayout.Width(50.0f));
                if (newInt != _snapSteps[axisIndex])
                {
                    UndoEx.RecordForToolAction(this);
                    SetSnapStep(axisIndex, newInt);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUI.EndGroup();
            Handles.EndGUI();
        }

        public override GizmoType GetGizmoType()
        {
            return GizmoType.Duplicate;
        }
    }
}
#endif
