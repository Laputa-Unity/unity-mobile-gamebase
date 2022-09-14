#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectGrabSettings : ScriptableObject
    {
        [SerializeField]
        private bool _alignAxis = true;
        [SerializeField]
        private CoordinateSystemAxis _alignmentAxis = CoordinateSystemAxis.PositiveUp;
        [SerializeField]
        private float _rotationSensitivity = 1.0f;
        [SerializeField]
        private float _scaleSensitivity = 0.02f;
        [SerializeField]
        private float _offsetFromSurface = 0.0f;
        [SerializeField]
        private bool _embedInSurfaceWhenNoAlign = true;
        [SerializeField]
        private bool _showGrabLines = true;
        [SerializeField]
        private Color _grabLineColor = Color.green;

        private static readonly float _minSensitivity = 1e-2f;

        public bool AlignAxis { get { return _alignAxis; } set { _alignAxis = value; } }
        public CoordinateSystemAxis AlignmentAxis { get { return _alignmentAxis; } set { _alignmentAxis = value; } }
        public float RotationSensitivity { get { return _rotationSensitivity; } set { _rotationSensitivity = Mathf.Clamp(value, _minSensitivity, 1.0f); } }
        public float ScaleSensitivity { get { return _scaleSensitivity; } set { _scaleSensitivity = Mathf.Clamp(value, _minSensitivity, 1.0f); } }
        public float OffsetFromSurface { get { return _offsetFromSurface; } set { _offsetFromSurface = value; } }
        public bool EmbedInSurfaceWhenNoAlign { get { return _embedInSurfaceWhenNoAlign; } set { _embedInSurfaceWhenNoAlign = value; } }
        public bool ShowGrabLines { get { return _showGrabLines; } set { _showGrabLines = value; } }
        public Color GrabLineColor { get { return _grabLineColor; } set { _grabLineColor = value; } }

        public void RenderView()
        {
            bool newBool; float newFloat;

            var content = new GUIContent();
            content.text = "Align axis";
            content.tooltip = "If this is checked, the obejcts' axes will be aligned with the hovered surface. Use the \'Alignment axis\' property to " + 
                              "specify the alignment axis.";
            newBool = EditorGUILayout.ToggleLeft(content, AlignAxis);
            if(newBool != AlignAxis)
            {
                UndoEx.RecordForToolAction(this);
                AlignAxis = newBool;
            }

            if(_alignAxis)
            {
                content.text = "Alignment axis";
                content.tooltip = "Allows you to specify the alignment axis for the grabbed objects.";
                CoordinateSystemAxis newAxis = (CoordinateSystemAxis)EditorGUILayout.EnumPopup(content, AlignmentAxis);
                if(newAxis != AlignmentAxis)
                {
                    UndoEx.RecordForToolAction(this);
                    AlignmentAxis = newAxis;
                }
            }

            content.text = "Rotation sensitivity";
            content.tooltip = "Allows you to control how sensitive the rotation is to mouse movements. Only horizontal movements count.";
            newFloat = EditorGUILayout.Slider(content, RotationSensitivity, _minSensitivity, 1.0f);
            if(newFloat != RotationSensitivity)
            {
                UndoEx.RecordForToolAction(this);
                RotationSensitivity = newFloat;
            }

            content.text = "Scale sensitivity";
            content.tooltip = "Allows you to control how sensitive the scale is to mouse movements. Only horizontal movements count.";
            newFloat = EditorGUILayout.Slider(content, ScaleSensitivity, _minSensitivity, 1.0f);
            if (newFloat != ScaleSensitivity)
            {
                UndoEx.RecordForToolAction(this);
                ScaleSensitivity = newFloat;
            }

            Octave3DWorldBuilder.ActiveInstance.ShowGUIHint("Offset from surface only works when axis alignment is on OR when embedding is off.");
            content.text = "Offset from surface";
            content.tooltip = "Allows you to control how much objects are offset from the surface on which they're sitting.";
            newFloat = EditorGUILayout.FloatField(content, OffsetFromSurface);
            if (newFloat != OffsetFromSurface)
            {
                UndoEx.RecordForToolAction(this);
                OffsetFromSurface = newFloat;
            }

            content.text = "Embed in surface (no align)";
            content.tooltip = "If this is checked, the objects will be embedded inside the surface on which they reside by a specified percentage of their size. " +
                                "This is useful for example when grabbing tress along a terrain surface with bumps/hills when axis alignment is turned off. In this case " +
                                "embedding the tree will ensure the trunk of the tree will not float above the terrain.";
            newBool = EditorGUILayout.ToggleLeft(content, EmbedInSurfaceWhenNoAlign);
            if (newBool != EmbedInSurfaceWhenNoAlign)
            {
                UndoEx.RecordForToolAction(this);
                EmbedInSurfaceWhenNoAlign = newBool;
            }

            EditorGUILayout.Separator();
            content.text = "Show grab lines";
            content.tooltip = "Should the grab lines be drawn during a grab sesson. These are the lines that go from the objects' centers to the grab pivot.";
            newBool = EditorGUILayout.ToggleLeft(content, ShowGrabLines);
            if(newBool != ShowGrabLines)
            {
                UndoEx.RecordForToolAction(this);
                ShowGrabLines = newBool;
            }

            content.text = "Grab line color";
            content.tooltip = "During a grab session, a line will be drawn from each object's center to the grab pivot point. This field allows you to control the color of those lines.";
            Color newColor = EditorGUILayout.ColorField(content, GrabLineColor);
            if(newColor != GrabLineColor)
            {
                UndoEx.RecordForToolAction(this);
                GrabLineColor = newColor;
                SceneView.RepaintAll();
            }
        }
    }
}
#endif