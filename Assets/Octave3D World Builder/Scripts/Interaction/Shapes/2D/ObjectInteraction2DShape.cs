#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public abstract class ObjectInteraction2DShape
    {
        #region Private Variables
        private Rect _enclosingRect;
        #endregion

        #region Public Properties
        public Rect EnclosingRect { get { return _enclosingRect; } set { _enclosingRect = value; } }
        public Vector2 EnclosingRectCenter { get { return _enclosingRect.center; } set { _enclosingRect.center = value; } }
        public float EnclosingRectWidth { get { return _enclosingRect.width; } set { _enclosingRect.width = value; } }
        public float EnclosingRectHeight { get { return _enclosingRect.height; } set { _enclosingRect.height = value; } }
        #endregion

        #region Public Methods
        public void SetEnclosingRectMinMaxPoints(Vector2 minMaxPoint)
        {
            SetEnclosingRectMinPoint(minMaxPoint);
            SetEnclosingRectMaxPoint(minMaxPoint);
        }

        public void SetEnclosingRectMinMaxPoints(Vector2 minPoint, Vector2 maxPoint)
        {
            SetEnclosingRectMinPoint(minPoint);
            SetEnclosingRectMaxPoint(maxPoint);
        }

        public void SetEnclosingRectMinPoint(Vector2 minPoint)
        {
            _enclosingRect.xMin = minPoint.x;
            _enclosingRect.yMin = minPoint.y;
        }

        public void SetEnclosingRectMaxPoint(Vector2 maxPoint)
        {
            _enclosingRect.xMax = maxPoint.x;
            _enclosingRect.yMax = maxPoint.y;
        }

        public void AdjustEnclosingRectSizeUsing1To1Ratio(Vector2 mousePosition, Vector2 pivotPoint)
        {
            List<Vector2> rectangleCornerPoints = _enclosingRect.GetCornerPoints();
            Vector2 fromPivotPointToMousePos = mousePosition - pivotPoint;
            
            float deltaX = fromPivotPointToMousePos.x;
            float deltaY = fromPivotPointToMousePos.y;

            float absDeltaX = Mathf.Abs(deltaX);
            float absDeltaY = Mathf.Abs(deltaY);

            Vector2 absRectSize = _enclosingRect.size.GetVectorWithAbsoluteValueComponents();

            if (absDeltaY > absRectSize.y && absDeltaX <= absRectSize.x)
            {
                Vector2 bottomLeftRectPoint = rectangleCornerPoints[3];
                float distanceFromSegment = mousePosition.GetPointDistanceFromSegment(pivotPoint, bottomLeftRectPoint, true);

                _enclosingRect.max = _enclosingRect.min + new Vector2(distanceFromSegment * Mathf.Sign(deltaX), distanceFromSegment * Mathf.Sign(deltaY));
            }
            else
            if(absDeltaX > absRectSize.x && absDeltaY <= absRectSize.y)
            {
                Vector2 topRightRectPoint = rectangleCornerPoints[1];
                float distanceFromSegment = mousePosition.GetPointDistanceFromSegment(pivotPoint, topRightRectPoint, true);

                _enclosingRect.max = _enclosingRect.min + new Vector2(distanceFromSegment * Mathf.Sign(deltaX), distanceFromSegment * Mathf.Sign(deltaY));
            }
            else
            if ((absDeltaX > absRectSize.x && absDeltaY > absRectSize.y) || _enclosingRect.Contains(mousePosition, true))
            {
                float minAbsValue = absDeltaX;
                if (minAbsValue > absDeltaY) minAbsValue = absDeltaY;

                _enclosingRect.max = _enclosingRect.min + new Vector2(minAbsValue * Mathf.Sign(deltaX), minAbsValue * Mathf.Sign(deltaY));
            }
        }
        #endregion

        #region Public Abstract Methods
        public abstract void RenderGizmos();
        public abstract List<GameObject> GetOverlappedGameObjects(bool allowPartialOverlap);
        #endregion

        #region Protected Methods
        protected List<GameObject> GetGameObjectsOverlappedByEnclosingRect(bool allowPartialOverlap)
        {          
            Camera camera = SceneViewCamera.Camera;
            List<GameObject> visibleGameObjects = SceneViewCamera.Instance.GetVisibleGameObjects();
            if (visibleGameObjects.Count == 0) return new List<GameObject>();

            if (allowPartialOverlap) return GetGameObjectsOverlappedByEnclosingRectForPartialOverlap(visibleGameObjects, camera);
            else return GetGameObjectsOverlappedByEnclosingRectForFullOverlap(visibleGameObjects, camera);
        }
        #endregion

        #region Private Methods
        private List<GameObject> GetGameObjectsOverlappedByEnclosingRectForPartialOverlap(List<GameObject> visibleGameObjects, Camera camera)
        {
            var overlappedGameObjects = new List<GameObject>(visibleGameObjects.Count);
            foreach (GameObject visibleGameObject in visibleGameObjects)
            {
                if (visibleGameObject.GetScreenRectangle(camera).Overlaps(_enclosingRect, true)) overlappedGameObjects.Add(visibleGameObject);
            }

            return overlappedGameObjects;
        }

        private List<GameObject> GetGameObjectsOverlappedByEnclosingRectForFullOverlap(List<GameObject> visibleGameObjects, Camera camera)
        {
            var overlappedGameObjects = new List<GameObject>(visibleGameObjects.Count);
            foreach (GameObject visibleGameObject in visibleGameObjects)
            {
                OrientedBox worldOrientedBox = visibleGameObject.GetWorldOrientedBox();
                if (worldOrientedBox.IsValid() && _enclosingRect.FullyContainsOrientedBoxCenterAndCornerPointsInScreenSpace(worldOrientedBox, camera)) overlappedGameObjects.Add(visibleGameObject);
            }

            return overlappedGameObjects;
        }
        #endregion
    }
}
#endif
