#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class EllipseObjectInteractionShape : ObjectInteraction2DShape
    {
        #region Private Variables
        [SerializeField]
        private EllipseShapeRenderSettings _renderSettings;
        #endregion

        #region Public Properties
        public EllipseShapeRenderSettings RenderSettings
        {
            get
            {
                if (_renderSettings == null) _renderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<EllipseShapeRenderSettings>();
                return _renderSettings;
            }
        }
        public Ellipse2D Ellipse { get { return new Ellipse2D(EnclosingRect); } }
        #endregion

        #region Public Methods
        public override void RenderGizmos()
        {
            Ellipse2D ellipse = Ellipse;

            GizmosEx.Render2DFilledEllipse(ellipse, _renderSettings.FillColor);
            GizmosEx.Render2DEllipseBorderLines(ellipse, _renderSettings.BorderLineColor);
        }

        public override List<GameObject> GetOverlappedGameObjects(bool allowPartialOverlap)
        {
            if (allowPartialOverlap) return GetOverlappedGameObjectsForPartialOverlap(GetGameObjectsOverlappedByEnclosingRect(true), SceneViewCamera.Camera);
            else return GetOverlappedGameObjectsForFullOverlap(GetGameObjectsOverlappedByEnclosingRect(false), SceneViewCamera.Camera);
        }
        #endregion

        #region Private Methods
        private List<GameObject> GetOverlappedGameObjectsForPartialOverlap(List<GameObject> objectsOveralppedByEnclosingRect, Camera camera)
        {
            Ellipse2D ellipse = Ellipse;

            List<GameObject> overlappedGameObjects = new List<GameObject>(objectsOveralppedByEnclosingRect.Count);
            foreach (GameObject gameObject in objectsOveralppedByEnclosingRect)
            {
                if (ellipse.OverlapsRectangle(gameObject.GetScreenRectangle(camera))) overlappedGameObjects.Add(gameObject);
            }

            return overlappedGameObjects;
        }

        private List<GameObject> GetOverlappedGameObjectsForFullOverlap(List<GameObject> objectsOveralppedByEnclosingRect, Camera camera)
        {
            Ellipse2D ellipse = Ellipse;

            List<GameObject> overlappedGameObjects = new List<GameObject>(objectsOveralppedByEnclosingRect.Count);
            foreach (GameObject gameObject in objectsOveralppedByEnclosingRect)
            {
                OrientedBox worldOrientedBox = gameObject.gameObject.GetWorldOrientedBox();
                if(worldOrientedBox.IsValid() && ellipse.FullyContainsOrientedBoxCenterAndCornerPointsInScreenSpace(worldOrientedBox, camera)) overlappedGameObjects.Add(gameObject);
            }

            return overlappedGameObjects;
        }
        #endregion
    }
}
#endif
