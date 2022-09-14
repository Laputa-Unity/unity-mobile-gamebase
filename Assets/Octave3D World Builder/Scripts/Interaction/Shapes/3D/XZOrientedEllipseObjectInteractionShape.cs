#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class XZOrientedEllipseObjectInteractionShape : ObjectInteraction3DShape
    {
        #region Private Constant Variables
        private const int _numberOfEllipseSlices = 50;
        #endregion

        #region Private Variables
        private XZOrientedEllipse3D _ellipse = new XZOrientedEllipse3D();
        [SerializeField]
        private XZOrientedEllipseShapeRenderSettings _renderSettings;
        #endregion

        #region Public Properties
        public override Vector3 Normal
        {
            get { return _ellipse.Normal; }
            set
            {
                if(_ellipse.Normal != value)
                {
                    _ellipse.SetNormal(value);
                    _renderPointsMustBeCalculated = true;
                }
            }
        }
        public override Vector3 Center
        {
            get { return _ellipse.Center; }
            set
            {
                if(_ellipse.Center != value)
                {
                    _ellipse.Center = value;
                    _renderPointsMustBeCalculated = true;
                }
            }
        }
        public Vector2 ModelSpaceRadii
        {
            get { return _ellipse.ModelSpaceRadii; }
            set
            {
                if (_ellipse.ModelSpaceRadii != value)
                {
                    _ellipse.ModelSpaceRadii = value;
                    _renderPointsMustBeCalculated = true;
                }
            }
        }
        public float MaxModelSpaceRadius { get { return _ellipse.ModelSpaceRadiusX > _ellipse.ModelSpaceRadiusZ ? _ellipse.ModelSpaceRadiusX : _ellipse.ModelSpaceRadiusZ; } }
        public XZOrientedEllipseShapeRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<XZOrientedEllipseShapeRenderSettings>();
                return _renderSettings;
            }
        }
        #endregion

        #region Public Methods
        public override void RenderGizmos()
        {
            GizmosEx.RenderLinesBetweenPoints(GetRenderPoints(), RenderSettings.BorderLineColor, _ellipse.Normal * 0.03f);
        }

        public override bool OverlapsPolygon(Polygon3D polygon)
        {
            return _ellipse.OverlapsPolygon(polygon);
        }

        public override bool ContainsPoint(Vector3 point)
        {
            return _ellipse.ContainsPoint(point);
        }

        public override bool ContainsAllPoints(List<Vector3> points)
        {
            return _ellipse.ContainsAllPoints(points);
        }

        public override bool ContainsAnyPoint(List<Vector3> points)
        {
            return _ellipse.ContainsAnyPoint(points);
        }

        public Vector3 GetLocalAxis(CoordinateSystemAxis axis)
        {
            return _ellipse.GetLocalAxis(axis);
        }

        public Vector3 GetRandomPointInside()
        {
            return _ellipse.GetRandomPointInside();
        }
        #endregion

        #region Protected Methods
        protected override List<Vector3> GenerateRenderPoints()
        {
            return Vector3Extensions.Get3DEllipseCircumferencePoints(ModelSpaceRadii.x, ModelSpaceRadii.y, _ellipse.GetLocalAxis(CoordinateSystemAxis.PositiveRight), _ellipse.GetLocalAxis(CoordinateSystemAxis.PositiveLook), Center, _numberOfEllipseSlices);
        }
        #endregion
    }
}
#endif