#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class Object3DMassEraseShape
    {
        #region Private Variables
        [SerializeField]
        private XZOrientedEllipseObjectInteractionShape _circleShape = new XZOrientedEllipseObjectInteractionShape();
        #endregion

        #region Public Properties
        public XZOrientedEllipseShapeRenderSettings CircleShapeRenderSettings { get { return _circleShape.RenderSettings; } }
        #endregion

        #region Public Methods
        public void RenderGizmos()
        {
            if (IsVisible())
            {
                OnBeforeRender();
                _circleShape.RenderGizmos();
            }
        }

        public List<GameObject> GetOverlappedGameObjectsForEraseOperation()
        {
            List<GameObject> objectsForEraseOperation = _circleShape.GetOverlappedGameObjects(ObjectEraserSettings.Get().Mass3DEraseSettings.AllowPartialOverlap);
            if(_circleShape.HasCursorPickedObject())
            {
                // Filter the elements in the list so that we don't erase objects which reside in the same hierarchy
                // as the object on which the erase shape reisdes.
                GameObject pickedGameObject = _circleShape.GetGameObjectPickedByCursor();
                if(!Octave3DWorldBuilder.ActiveInstance.IsPivotWorkingObject(pickedGameObject))
                {
                    GameObject root = Octave3DWorldBuilder.ActiveInstance.GetRoot(pickedGameObject);
                    if (pickedGameObject != null && root != null)
                    {
                        Transform parentTransform = root.transform;
                        objectsForEraseOperation.RemoveAll(item => item.transform.IsChildOf(parentTransform));
                    }
                }
            }

            return objectsForEraseOperation;
        }

        public bool IsVisible()
        {
            if (ObjectEraserSettings.Get().EraseMode != ObjectEraseMode.ObjectMass3D) return false;
            return true;
        }

        public void HandleMouseMoveEvent(Event e)
        {
            if (IsVisible())
            {
                AdjustCircleRadiusIfNecessary();
                AcquireCursorRayHit();
                _circleShape.AdjustShapeCenterAndNormalBasedOnCursorRayHit();
            }
        }

        public void HandleMouseDragEvent(Event e)
        {
            if (IsVisible())
            {
                AdjustCircleRadiusIfNecessary();
                AcquireCursorRayHit();
                _circleShape.AdjustShapeCenterAndNormalBasedOnCursorRayHit();
            }
        }
        #endregion

        #region Private Methods
        private void OnBeforeRender()
        {
            AdjustCircleRadiusIfNecessary();
        }

        private void AcquireCursorRayHit()
        {
            if(_circleShape.HasCursorPickedTerrainObject())
            {
                 _circleShape.AcquireCursorRayHitForTerrainObject(_circleShape.GetGameObjectPickedByCursor());
            }
            else _circleShape.AcquireCursorRayHit(null, MouseCursorObjectPickFlags.ObjectBox);
        }

        private void AdjustCircleRadiusIfNecessary()
        {
            float desiredCircleRadius = ObjectEraserSettings.Get().Mass3DEraseSettings.CircleShapeRadius;
            if (_circleShape.MaxModelSpaceRadius != desiredCircleRadius) _circleShape.ModelSpaceRadii = new Vector2(desiredCircleRadius, desiredCircleRadius);
        }
        #endregion
    }
}
#endif