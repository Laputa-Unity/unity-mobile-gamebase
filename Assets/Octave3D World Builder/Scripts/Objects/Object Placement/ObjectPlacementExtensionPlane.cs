#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementExtensionPlane
    {
        #region Private Variables
        private XZOrientedQuad3D _planeQuad = new XZOrientedQuad3D();

        [SerializeField]
        private ObjectPlacementExtensionPlaneRenderSettings _renderSettings;
        private ObjectPlacementExtensionPlaneRenderer _renderer = new ObjectPlacementExtensionPlaneRenderer();
        #endregion

        #region Public Properties
        public ObjectPlacementExtensionPlaneRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectPlacementExtensionPlaneRenderSettings>();
                return _renderSettings;
            }
        }

        public XZOrientedQuad3D PlaneQuad { get { return new XZOrientedQuad3D(_planeQuad); } }
        public Plane Plane { get { return _planeQuad.Plane; } }
        public Vector3 RightAxis { get { return _planeQuad.GetLocalAxis(CoordinateSystemAxis.PositiveRight); } }
        public Vector3 UpAxis { get { return Plane.normal; } }
        public Vector3 LookAxis { get { return _planeQuad.GetLocalAxis(CoordinateSystemAxis.PositiveLook); } }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            _renderer.RenderGizmos(this);
        }

        public void FromOrientedBoxFace(OrientedBox orientedBox, BoxFace boxFace)
        {
            CalculateQuadCoordinateSystem(orientedBox, boxFace);
            CalculateQuadXZSize(orientedBox, boxFace);
        }
        #endregion

        #region Private Methods
        private void CalculateQuadCoordinateSystem(OrientedBox orientedBox, BoxFace boxFace)
        {
            _planeQuad.InheritCoordinateSystem(orientedBox.GetBoxFaceCoordinateSystem(boxFace));
            _planeQuad.FaceInOppositeDirection();
        }

        private void CalculateQuadXZSize(OrientedBox orientedBox, BoxFace boxFace)
        {
            Vector2 boxFaceSize = orientedBox.GetBoxFaceSizeAlongFaceLocalXZAxes(boxFace);
            float quadSize = boxFaceSize.x;
            if (quadSize < boxFaceSize.y) quadSize = boxFaceSize.y;
            _planeQuad.SizeOnBothXZAxes = quadSize;
        }
        #endregion
    }
}
#endif