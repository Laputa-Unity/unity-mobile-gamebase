#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public struct Box
    {
        #region Private Variables
        private Vector3 _min;
        private Vector3 _max;
        #endregion

        #region Public Properties
        public Vector3 Min { get { return _min; } set { _min = value; } }
        public Vector3 Max { get { return _max; } set { _max = value; } }
        public Vector3 Extents { get { return Size * 0.5f; } }
        public Vector3 Size
        {
            get { return _max - _min; }
            set
            {
                Vector3 currentCenter = Center;
                Vector3 extents = (value * 0.5f).GetVectorWithPositiveComponents();

                _max = currentCenter + extents;
                _min = currentCenter - extents;
            }
        }
        public Vector3 Center 
        { 
            get { return (_min + _max) * 0.5f; } 
            set
            {
                Vector3 extents = Extents;

                _max = value + extents;
                _min = value - extents;
            }
        }
        #endregion

        #region Constructors
        public Box(Bounds bounds)
        {
            _min = bounds.min;
            _max = bounds.max;
        }

        public Box(Vector3 center, Vector3 size)
        {
            _min = Vector3.zero;
            _max = Vector3.zero;

            Size = size;
            Center = center;
        }

        public Box(Box source)
        {
            _min = source.Min;
            _max = source.Max;
        }
        #endregion

        #region Public Static Functions
        public static Box GetInvalid()
        {
            var box = new Box();
            box.MakeInvalid();

            return box;
        }

        public static Box FromObjectWorldAABB(IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null) return Box.GetInvalid();

            Box finalAABB = Box.GetInvalid();
            foreach(var gameObject in gameObjects)
            {
                Box worldAABB = gameObject.GetWorldBox();
                if(worldAABB.IsValid())
                {
                    if (finalAABB.IsValid()) finalAABB.Encapsulate(worldAABB);
                    else finalAABB = worldAABB;
                }
            }

            return finalAABB;
        }

        public static Box FromPoints(List<Vector3> points, float sizeScale = 1.0f)
        {
            if (points.Count == 0) return GetInvalid();

            Vector3 min = points[0];
            Vector3 max = points[0];

            for(int pointIndex = 1; pointIndex < points.Count; ++pointIndex)
            {
                Vector3 point = points[pointIndex];
                min = Vector3.Min(min, point);
                max = Vector3.Max(max, point);
            }

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = (max - min) * sizeScale;

            return new Box(center, size);
        }

        public static Box FromMinMax(Vector3 min, Vector3 max)
        {
            Vector3 center = (min + max) * 0.5f;
            Vector3 size = (max - min);
            return new Box(center, size);
        }
        #endregion

        #region Public Methods
        public Bounds ToBounds()
        {
            return new Bounds(Center, Size);
        }

        public void MoveInFrontOfPlane(Plane plane)
        {
            List<Vector3> boxPts = GetCornerPoints();

            int ptIndex = plane.GetIndexOfFurthestPointBehind(boxPts);
            if (ptIndex < 0) ptIndex = plane.GetIndexOfClosestPointInFrontOrOnPlane(boxPts);

            if (ptIndex >= 0)
            {
                float distToPt = plane.GetDistanceToPoint(boxPts[ptIndex]);
                Center -= plane.normal * distToPt;
            }
        }

        public OrientedBox ToOrientedBox()
        {
            Box modelSpaceBox = new Box(Vector3.zero, Size);
            OrientedBox orientedBox = new OrientedBox(modelSpaceBox, Quaternion.identity);
            orientedBox.Center = Center;
            return orientedBox;
        }

        public Sphere GetEncpasulatingSphere()
        {
            return new Sphere(Center, Extents.magnitude);
        }

        public void Encapsulate(Bounds bounds)
        {
            Encapsulate(new Box(bounds));
        }

        public void Encapsulate(Box box)
        {
            AddPoint(box.Min);
            AddPoint(box.Max);
        }

        public void AddPoint(Vector3 point)
        {         
            if (point.x < _min.x) _min.x = point.x;
            if (point.y < _min.y) _min.y = point.y;
            if (point.z < _min.z) _min.z = point.z;
            if (point.x > _max.x) _max.x = point.x;
            if (point.y > _max.y) _max.y = point.y;
            if (point.z > _max.z) _max.z = point.z;
        }

        public bool IntersectsBox(Box box, bool allowFacesToTouch = false, float intersectionEpsilon = 1e-5f)
        {
            Vector3 center = Center;
            Vector3 radius = Extents;

            Vector3 secondBoxRadius = box.Extents;
            Vector3 secondBoxCenter = box.Center;

            float distanceBetweenCentersOnX = Mathf.Abs(center.x - secondBoxCenter.x);
            float distanceBetweenCentersOnY = Mathf.Abs(center.y - secondBoxCenter.y);
            float distanceBetweenCentersOnZ = Mathf.Abs(center.z - secondBoxCenter.z);

            float radiiSumOnX = radius.x + secondBoxRadius.x;
            float radiiSumOnY = radius.y + secondBoxRadius.y;
            float radiiSumOnZ = radius.z + secondBoxRadius.z;

            // 2 boxes intersect if they intersect on all 3 axes. If the distance between the 2 bounds' centers
            // is greater than the 2 radii along any of the 3 axes, it means the 2 bounds don't intersect.
            if (!allowFacesToTouch)
            {
                // Note: We use the equal sign because we don't want to return true if the boxes are touching 
                //       faces when 'allowFacesToTouch' is set to false.
                if (distanceBetweenCentersOnX >= radiiSumOnX) return false;
                if (distanceBetweenCentersOnY >= radiiSumOnY) return false;
                if (distanceBetweenCentersOnZ >= radiiSumOnZ) return false;
            }
            else
            {
                // Note: We will add 'intersectionEpsilon' to the distance between centers. 
                if (distanceBetweenCentersOnX + intersectionEpsilon > radiiSumOnX) return false;
                if (distanceBetweenCentersOnY + intersectionEpsilon > radiiSumOnY) return false;
                if (distanceBetweenCentersOnZ + intersectionEpsilon > radiiSumOnZ) return false;
            }

            return true;
        }

        public bool TouchesFacesWith(Box box)
        {
            Vector3 firstBoxRadius = Extents;
            Vector3 secondBoxRadius = box.Extents;
            Vector3 firstBoxCenter = Center;
            Vector3 secondBoxCenter = box.Center;

            float distanceBetweenCentersX = Mathf.Abs(firstBoxCenter.x - secondBoxCenter.x);
            float distanceBetweenCentersY = Mathf.Abs(firstBoxCenter.y - secondBoxCenter.y);
            float distanceBetweenCentersZ = Mathf.Abs(firstBoxCenter.z - secondBoxCenter.z);

            float radiiSumX = firstBoxRadius.x + secondBoxRadius.x;
            float radiiSumY = firstBoxRadius.y + secondBoxRadius.y;
            float radiiSumZ = firstBoxRadius.z + secondBoxRadius.z;

            // In order to detect if 2 faces are touching, we will first have to check for each axis if the
            // difference between the center distance and the sum of the radii is close enough to 0. If it
            // is, we only have to make sure that the distance between the bounds' centers on the remaining
            // 2 axes does not exceed the sum of the radii in which case it would mean that the faces touch
            // outside of the permitted area. If the faces are not close enough on a certain axis, we will
            // also check if the distance between the centers along the same axis is greater than the sum
            // of the radii along that axis. If it is, we return false because it means the 2 AABBs are not
            // close enough to be touching in any way.
            const float epsilon = 1e-4f;
            if (Mathf.Abs(distanceBetweenCentersX - radiiSumX) < epsilon)
            {
                // Note: We have to make sure the faces touch in the permitted area. If the distance between the 2 centers
                //       on the remaining 2 axis is greater or equal (with epsilon) to the corresponding radii sum, it means
                //       the faces touch outside the permitted area.
                if (distanceBetweenCentersY > radiiSumY || Mathf.Abs(distanceBetweenCentersY - radiiSumY) < epsilon) return false;
                if (distanceBetweenCentersZ > radiiSumZ || Mathf.Abs(distanceBetweenCentersZ - radiiSumZ) < epsilon) return false;

                return true;
            }
            else if (distanceBetweenCentersX > radiiSumX) return false;

            if (Mathf.Abs(distanceBetweenCentersY - radiiSumY) < epsilon)
            {
                if (distanceBetweenCentersX > radiiSumX || Mathf.Abs(distanceBetweenCentersX - radiiSumX) < epsilon) return false;
                if (distanceBetweenCentersZ > radiiSumZ || Mathf.Abs(distanceBetweenCentersZ - radiiSumZ) < epsilon) return false;

                return true;
            }
            else if (distanceBetweenCentersY > radiiSumY) return false;

            if (Mathf.Abs(distanceBetweenCentersZ - radiiSumZ) < epsilon)
            {
                if (distanceBetweenCentersX > radiiSumX || Mathf.Abs(distanceBetweenCentersX - radiiSumX) < epsilon) return false;
                if (distanceBetweenCentersY > radiiSumY || Mathf.Abs(distanceBetweenCentersY - radiiSumY) < epsilon) return false;

                return true;
            }
            else if (distanceBetweenCentersZ > radiiSumZ) return false;

            return false;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.x >= _min.x && point.x <= _max.x &&
                   point.y >= _min.y && point.y <= _max.y &&
                   point.z >= _min.z && point.z <= _max.z;
        }

        public List<Vector3> GetCenterAndCornerPoints()
        {
            Vector3[] points = new Vector3[BoxPoints.Count];

            points[(int)BoxPoint.Center] = GetBoxPoint(BoxPoint.Center);
            points[(int)BoxPoint.FrontTopLeft] = GetBoxPoint(BoxPoint.FrontTopLeft);
            points[(int)BoxPoint.FrontTopRight] = GetBoxPoint(BoxPoint.FrontTopRight);
            points[(int)BoxPoint.FrontBottomRight] = GetBoxPoint(BoxPoint.FrontBottomRight);
            points[(int)BoxPoint.FrontBottomLeft] = GetBoxPoint(BoxPoint.FrontBottomLeft);
            points[(int)BoxPoint.BackTopLeft] = GetBoxPoint(BoxPoint.BackTopLeft);
            points[(int)BoxPoint.BackTopRight] = GetBoxPoint(BoxPoint.BackTopRight);
            points[(int)BoxPoint.BackBottomRight] = GetBoxPoint(BoxPoint.BackBottomRight);
            points[(int)BoxPoint.BackBottomLeft] = GetBoxPoint(BoxPoint.BackBottomLeft);

            return new List<Vector3>(points);
        }

        public List<Vector3> GetCornerPoints()
        {
            Vector3[] points = new Vector3[BoxCornerPoints.Count];

            points[(int)BoxCornerPoint.FrontTopLeft] = GetBoxPoint(BoxPoint.FrontTopLeft);
            points[(int)BoxCornerPoint.FrontTopRight] = GetBoxPoint(BoxPoint.FrontTopRight);
            points[(int)BoxCornerPoint.FrontBottomRight] = GetBoxPoint(BoxPoint.FrontBottomRight);
            points[(int)BoxCornerPoint.FrontBottomLeft] = GetBoxPoint(BoxPoint.FrontBottomLeft);
            points[(int)BoxCornerPoint.BackTopLeft] = GetBoxPoint(BoxPoint.BackTopLeft);
            points[(int)BoxCornerPoint.BackTopRight] = GetBoxPoint(BoxPoint.BackTopRight);
            points[(int)BoxCornerPoint.BackBottomRight] = GetBoxPoint(BoxPoint.BackBottomRight);
            points[(int)BoxCornerPoint.BackBottomLeft] = GetBoxPoint(BoxPoint.BackBottomLeft);

            return new List<Vector3>(points);
        }

        public Vector3 GetBoxPoint(BoxPoint boxPoint)
        {
            Vector3 center = Center;
            Vector3 extents = Extents;

            switch (boxPoint)
            {
                case BoxPoint.Center:

                    return center;

                case BoxPoint.FrontTopLeft:

                    return center - BoxFaces.GetFaceRightAxis(BoxFace.Front) * extents.x + 
                                    BoxFaces.GetFaceLookAxis(BoxFace.Front) * extents.y + 
                                    BoxFaces.GetFaceNormal(BoxFace.Front) * extents.z;

                case BoxPoint.FrontTopRight:

                    return center + BoxFaces.GetFaceRightAxis(BoxFace.Front) * extents.x +
                                    BoxFaces.GetFaceLookAxis(BoxFace.Front) * extents.y +
                                    BoxFaces.GetFaceNormal(BoxFace.Front) * extents.z;

                case BoxPoint.FrontBottomRight:

                    return center + BoxFaces.GetFaceRightAxis(BoxFace.Front) * extents.x -
                                    BoxFaces.GetFaceLookAxis(BoxFace.Front) * extents.y +
                                    BoxFaces.GetFaceNormal(BoxFace.Front) * extents.z;

                case BoxPoint.FrontBottomLeft:

                    return center - BoxFaces.GetFaceRightAxis(BoxFace.Front) * extents.x -
                                    BoxFaces.GetFaceLookAxis(BoxFace.Front) * extents.y +
                                    BoxFaces.GetFaceNormal(BoxFace.Front) * extents.z;

                case BoxPoint.BackTopLeft:

                    return center - BoxFaces.GetFaceRightAxis(BoxFace.Back) * extents.x +
                                    BoxFaces.GetFaceLookAxis(BoxFace.Back) * extents.y +
                                    BoxFaces.GetFaceNormal(BoxFace.Back) * extents.z;

                case BoxPoint.BackTopRight:

                    return center + BoxFaces.GetFaceRightAxis(BoxFace.Back) * extents.x +
                                    BoxFaces.GetFaceLookAxis(BoxFace.Back) * extents.y +
                                    BoxFaces.GetFaceNormal(BoxFace.Back) * extents.z;

                case BoxPoint.BackBottomRight:

                    return center + BoxFaces.GetFaceRightAxis(BoxFace.Back) * extents.x -
                                    BoxFaces.GetFaceLookAxis(BoxFace.Back) * extents.y +
                                    BoxFaces.GetFaceNormal(BoxFace.Back) * extents.z;

                case BoxPoint.BackBottomLeft:

                    return center - BoxFaces.GetFaceRightAxis(BoxFace.Back) * extents.x -
                                    BoxFaces.GetFaceLookAxis(BoxFace.Back) * extents.y +
                                    BoxFaces.GetFaceNormal(BoxFace.Back) * extents.z;

                default:

                    return Vector3.zero;
            }
        }

        public BoxFace GetBoxFaceClosestToPoint(Vector3 point)
        {
            List<Plane> facePlanes = GetBoxFacePlanes();

            float minAbsDistanceFromPlane = float.MaxValue;
            BoxFace closestFace = BoxFace.Back;

            for(int faceIndex = 0; faceIndex < facePlanes.Count; ++faceIndex)
            {
                float absDistanceFromPlane = Mathf.Abs(facePlanes[faceIndex].GetDistanceToPoint(point));
                if(absDistanceFromPlane < minAbsDistanceFromPlane)
                {
                    minAbsDistanceFromPlane = absDistanceFromPlane;
                    closestFace = (BoxFace)faceIndex;
                }
            }

            return closestFace;
        }

        public List<Plane> GetBoxFacePlanes()
        {
            Plane[] facePlanes = new Plane[Enum.GetValues(typeof(BoxFace)).Length];

            facePlanes[(int)BoxFace.Back] = GetBoxFacePlane(BoxFace.Back);
            facePlanes[(int)BoxFace.Front] = GetBoxFacePlane(BoxFace.Front);
            facePlanes[(int)BoxFace.Left] = GetBoxFacePlane(BoxFace.Left);
            facePlanes[(int)BoxFace.Right] = GetBoxFacePlane(BoxFace.Right);
            facePlanes[(int)BoxFace.Top] = GetBoxFacePlane(BoxFace.Top);
            facePlanes[(int)BoxFace.Bottom] = GetBoxFacePlane(BoxFace.Bottom);

            return new List<Plane>(facePlanes);
        }

        public CoordinateSystem GetBoxFaceCoordinateSystem(BoxFace boxFace)
        {
            var coordSystem = new CoordinateSystem();

            Vector3 origin = GetBoxFaceCenter(boxFace);
            Vector3 boxFaceUp = BoxFaces.GetFaceNormal(boxFace);
            Vector3 boxFaceLook = BoxFaces.GetFaceLookAxis(boxFace);

            coordSystem.SetOriginPosition(origin);
            coordSystem.SetRotation(Quaternion.LookRotation(boxFaceLook, boxFaceUp));
       
            return coordSystem;
        }

        public List<Vector3> GetBoxFaceCenterAndCornerPoints(BoxFace boxFace)
        {
            var points = new Vector3[BoxFacePoints.Count];

            Vector3 boxFaceCenter = GetBoxFaceCenter(boxFace);
            Vector2 boxFaceXZSize = GetBoxFaceSizeAlongFaceLocalXZAxes(boxFace, Vector3.one);
            Vector2 halfXZSize = boxFaceXZSize * 0.5f;

            points[(int)BoxFacePoint.Center] = boxFaceCenter;

            points[(int)BoxFacePoint.TopLeft] = boxFaceCenter - BoxFaces.GetFaceRightAxis(boxFace) * halfXZSize.x + BoxFaces.GetFaceLookAxis(boxFace) * halfXZSize.y;
            points[(int)BoxFacePoint.TopRight] = boxFaceCenter + BoxFaces.GetFaceRightAxis(boxFace) * halfXZSize.x + BoxFaces.GetFaceLookAxis(boxFace) * halfXZSize.y;
            points[(int)BoxFacePoint.BottomRight] = boxFaceCenter + BoxFaces.GetFaceRightAxis(boxFace) * halfXZSize.x - BoxFaces.GetFaceLookAxis(boxFace) * halfXZSize.y;
            points[(int)BoxFacePoint.BottomLeft] = boxFaceCenter - BoxFaces.GetFaceRightAxis(boxFace) * halfXZSize.x - BoxFaces.GetFaceLookAxis(boxFace) * halfXZSize.y;

            return new List<Vector3>(points);
        }

        public List<Vector3> GetBoxFaceCornerPoints(BoxFace boxFace)
        {
            var points = new Vector3[BoxFaceCornerPoints.Count];
          
            Vector3 boxFaceCenter = GetBoxFaceCenter(boxFace);
            Vector2 boxFaceXZSize = GetBoxFaceSizeAlongFaceLocalXZAxes(boxFace, Vector3.one);
            Vector2 halfXZSize = boxFaceXZSize * 0.5f;

            points[(int)BoxFaceCornerPoint.TopLeft] = boxFaceCenter - BoxFaces.GetFaceRightAxis(boxFace) * halfXZSize.x + BoxFaces.GetFaceLookAxis(boxFace) * halfXZSize.y;
            points[(int)BoxFaceCornerPoint.TopRight] = boxFaceCenter + BoxFaces.GetFaceRightAxis(boxFace) * halfXZSize.x + BoxFaces.GetFaceLookAxis(boxFace) * halfXZSize.y;
            points[(int)BoxFaceCornerPoint.BottomRight] = boxFaceCenter + BoxFaces.GetFaceRightAxis(boxFace) * halfXZSize.x - BoxFaces.GetFaceLookAxis(boxFace) * halfXZSize.y;
            points[(int)BoxFaceCornerPoint.BottomLeft] = boxFaceCenter - BoxFaces.GetFaceRightAxis(boxFace) * halfXZSize.x - BoxFaces.GetFaceLookAxis(boxFace) * halfXZSize.y;

            return new List<Vector3>(points);
        }

        public Plane GetBoxFacePlane(BoxFace boxFace)
        {
            switch(boxFace)
            {
                case BoxFace.Back:

                    return new Plane(BoxFaces.GetFaceNormal(BoxFace.Back), GetBoxPoint(BoxPoint.BackBottomLeft));

                case BoxFace.Front:

                    return new Plane(BoxFaces.GetFaceNormal(BoxFace.Front), GetBoxPoint(BoxPoint.FrontBottomLeft));

                case BoxFace.Left:

                    return new Plane(BoxFaces.GetFaceNormal(BoxFace.Left), GetBoxPoint(BoxPoint.FrontBottomLeft));

                case BoxFace.Right:

                    return new Plane(BoxFaces.GetFaceNormal(BoxFace.Right), GetBoxPoint(BoxPoint.FrontBottomRight));

                case BoxFace.Top:

                    return new Plane(BoxFaces.GetFaceNormal(BoxFace.Top), GetBoxPoint(BoxPoint.FrontTopLeft));

                case BoxFace.Bottom:

                    return new Plane(BoxFaces.GetFaceNormal(BoxFace.Bottom), GetBoxPoint(BoxPoint.FrontBottomLeft));

                default:

                    return new Plane();
            }
        }

        public Vector2 GetBoxFaceSizeAlongFaceLocalXZAxes(BoxFace boxFace, Vector3 boxXYZScale)
        {
            Vector3 size = Size;

            switch(boxFace)
            {
                case BoxFace.Front:
                case BoxFace.Back:

                    return new Vector2(size.x * boxXYZScale.x, size.y * boxXYZScale.y);

                case BoxFace.Left:
                case BoxFace.Right:

                    return new Vector2(size.z * boxXYZScale.z, size.y * boxXYZScale.y);

                case BoxFace.Top:
                case BoxFace.Bottom:

                    return new Vector2(size.x * boxXYZScale.x, size.z * boxXYZScale.z);

                default:

                    return Vector2.zero;
            }
        }

        public Vector3 GetBoxFaceCenter(BoxFace boxFace)
        {
            Vector3 center = Center;
            Vector3 extents = Extents;

            switch(boxFace)
            {
                case BoxFace.Back:

                    return center + BoxFaces.GetFaceNormal(BoxFace.Back) * extents.z;

                case BoxFace.Front:

                    return center + BoxFaces.GetFaceNormal(BoxFace.Front) * extents.z;

                case BoxFace.Left:

                    return center + BoxFaces.GetFaceNormal(BoxFace.Left) * extents.x;

                case BoxFace.Right:

                    return center + BoxFaces.GetFaceNormal(BoxFace.Right) * extents.x;

                case BoxFace.Top:

                    return center + BoxFaces.GetFaceNormal(BoxFace.Top) * extents.y;

                case BoxFace.Bottom:

                    return center + BoxFaces.GetFaceNormal(BoxFace.Bottom) * extents.y;

                default:

                    return Vector3.zero;
            }
        }

        public List<Vector2> GetScreenCornerPoints(Camera camera)
        {
            Vector3 boxCenter = Center;
            Vector3 boxExtents = Extents;

            return new List<Vector2>
            {
                Vector3Extensions.WorldToScreenPoint(new Vector3(boxCenter.x - boxExtents.x, boxCenter.y - boxExtents.y, boxCenter.z - boxExtents.z)),
                Vector3Extensions.WorldToScreenPoint(new Vector3(boxCenter.x + boxExtents.x, boxCenter.y - boxExtents.y, boxCenter.z - boxExtents.z)),
                Vector3Extensions.WorldToScreenPoint(new Vector3(boxCenter.x + boxExtents.x, boxCenter.y + boxExtents.y, boxCenter.z - boxExtents.z)),
                Vector3Extensions.WorldToScreenPoint(new Vector3(boxCenter.x - boxExtents.x, boxCenter.y + boxExtents.y, boxCenter.z - boxExtents.z)),

                Vector3Extensions.WorldToScreenPoint(new Vector3(boxCenter.x - boxExtents.x, boxCenter.y - boxExtents.y, boxCenter.z + boxExtents.z)),
                Vector3Extensions.WorldToScreenPoint(new Vector3(boxCenter.x + boxExtents.x, boxCenter.y - boxExtents.y, boxCenter.z + boxExtents.z)),
                Vector3Extensions.WorldToScreenPoint(new Vector3(boxCenter.x + boxExtents.x, boxCenter.y + boxExtents.y, boxCenter.z + boxExtents.z)),
                Vector3Extensions.WorldToScreenPoint(new Vector3(boxCenter.x - boxExtents.x, boxCenter.y + boxExtents.y, boxCenter.z + boxExtents.z)),
            };
        }

        public Rect GetScreenRectangle(Camera camera)
        {
            List<Vector2> screenPoints = GetScreenCornerPoints(camera);

            Vector2 minScreenPoint = screenPoints[0];
            Vector2 maxScreenPoint = screenPoints[0];
            foreach (Vector2 point in screenPoints)
            {
                minScreenPoint = Vector2.Min(minScreenPoint, point);
                maxScreenPoint = Vector2.Max(maxScreenPoint, point);
            }

            return new Rect(minScreenPoint.x, minScreenPoint.y, maxScreenPoint.x - minScreenPoint.x, maxScreenPoint.y - minScreenPoint.y);
        }

        public bool Raycast(Ray ray, out float t)
        {
            Bounds bounds = ToBounds();
            return bounds.IntersectRay(ray, out t);
        }

        public Box Transform(TransformMatrix transformMatrix)
        {
            Vector3 rightAxis = transformMatrix.GetNormalizedRightAxisNoScaleSign();
            Vector3 upAxis = transformMatrix.GetNormalizedUpAxisNoScaleSign();
            Vector3 lookAxis = transformMatrix.GetNormalizedLookAxisNoScaleSign();
            Vector3 scale = transformMatrix.Scale;

            Vector3 newCenter = transformMatrix.MultiplyPoint(Center);

            Vector3 boxExtents = Extents;
            Vector3 newExtentsRight = rightAxis * boxExtents.x * scale.x;
            Vector3 newExtentsUp = upAxis * boxExtents.y * scale.y;
            Vector3 newExtentsLook = lookAxis * boxExtents.z * scale.z;

            float newExtentsX = Mathf.Abs(newExtentsRight.x) + Mathf.Abs(newExtentsUp.x) + Mathf.Abs(newExtentsLook.x);
            float newExtentsY = Mathf.Abs(newExtentsRight.y) + Mathf.Abs(newExtentsUp.y) + Mathf.Abs(newExtentsLook.y);
            float newExtentsZ = Mathf.Abs(newExtentsRight.z) + Mathf.Abs(newExtentsUp.z) + Mathf.Abs(newExtentsLook.z);

            Vector3 newSize = new Vector3(newExtentsX, newExtentsY, newExtentsZ) * 2.0f;
            return new Box(newCenter, newSize);
        }

        public void MakeInvalid()
        {
            _min = Vector3.one;
            _max = -Vector3.one;
        }

        public bool IsValid()
        {
            return _min.x <= _max.x && _min.y <= _max.y && _min.z <= _max.z;
        }

        public bool IsInvalid()
        {
            return !IsValid();
        }
        #endregion
    }
}
#endif