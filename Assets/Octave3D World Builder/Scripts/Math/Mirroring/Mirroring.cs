#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class Mirroring
    {
        #region Public Static Functions
        public static Vector3 MirrorPosition(Plane mirrorPlane, Vector3 position)
        {
            float planeDistanceToPoint = mirrorPlane.GetDistanceToPoint(position);
            return mirrorPlane.ProjectPoint(position) - planeDistanceToPoint * mirrorPlane.normal;
        }

        public static Vector3 MirrorNormal(Plane mirrorPlane, Vector3 normal)
        {
            Plane mirrorPlaneAtOrigin = new Plane(mirrorPlane.normal, 0.0f);
            Vector3 mirroredNormal = MirrorPosition(mirrorPlaneAtOrigin, normal);
            mirroredNormal.Normalize();

            return mirroredNormal;
        }

        public static Vector3 MirrorVector(Plane mirrorPlane, Vector3 vector)
        {
            Plane mirrorPlaneAtOrigin = new Plane(mirrorPlane.normal, 0.0f);
            Vector3 mirroredVector = MirrorPosition(mirrorPlaneAtOrigin, vector);

            return mirroredVector;
        }

        public static OrientedBox MirrorOrientedBox(Plane mirrorPlane, OrientedBox orientedBox, bool mirrorRotation)
        {
            OrientedBox mirroredBox = new OrientedBox(orientedBox);
            mirroredBox.AllowNegativeScale = true;
            mirroredBox.Center = MirrorPosition(mirrorPlane, mirroredBox.Center);

            if (mirrorRotation)
            {
                MirroredRotation mirroredRotation = MirrorMatrixRotation(mirrorPlane, mirroredBox.TransformMatrix);
                mirroredBox.Rotation = mirroredRotation.Rotation;
                mirroredBox.Scale = mirroredRotation.AxesScale;
            }
         
            return mirroredBox;
        }

        public static MirroredRotation MirrorMatrixRotation(Plane mirrorPlane, TransformMatrix rotationMatrix)
        {
            Vector3 axesScale = rotationMatrix.Scale;
            Vector3 rightAxis = rotationMatrix.GetNormalizedRightAxisNoScaleSign();
            Vector3 upAxis = rotationMatrix.GetNormalizedUpAxisNoScaleSign();
            Vector3 lookAxis = rotationMatrix.GetNormalizedLookAxisNoScaleSign();

            rightAxis = MirrorNormal(mirrorPlane, rightAxis);
            upAxis = MirrorNormal(mirrorPlane, upAxis);
            lookAxis = MirrorNormal(mirrorPlane, lookAxis);

            Quaternion newRotation = Quaternion.LookRotation(lookAxis, upAxis);
            Vector3 resultRightAxis = Vector3.Cross(upAxis, lookAxis);

            bool pointsInSameDirection = resultRightAxis.IsPointingInSameGeneralDirection(rightAxis);
            if (!pointsInSameDirection) axesScale[0] *= -1.0f;
       
            return new MirroredRotation(newRotation, axesScale);
        }

        public static void MirrorTransformRotation(Plane mirrorPlane, Transform transform)
        {
            MirroredRotation mirroredRotation = MirrorMatrixRotation(mirrorPlane, transform.GetWorldMatrix());
            transform.rotation = mirroredRotation.Rotation;
            transform.SetWorldScale(mirroredRotation.AxesScale);
        }

        public static void MirrorObjectHierarchyTransform(Plane mirrorPlane, GameObject root, bool mirrorRotation)
        {
            Transform rootTransform = root.transform;

            OrientedBox hierarchyWorldOrientedBox = root.GetHierarchyWorldOrientedBox();
            if (!hierarchyWorldOrientedBox.IsValid()) return;

            if (mirrorRotation) MirrorTransformRotation(mirrorPlane, rootTransform);

            Vector3 mirroredCenter = MirrorPosition(mirrorPlane, hierarchyWorldOrientedBox.Center);
            rootTransform.position = ObjectPositionCalculator.CalculateObjectHierarchyPosition(root, mirroredCenter, rootTransform.lossyScale, rootTransform.rotation);
        }
        #endregion
    }
}
#endif