#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectPlacementSurfaceInfo
    {
        #region Public Static Functions
        public static Vector3 GetSurfaceNormal()
        {
            return GetSurfacePlane().normal;
        }

        public static Plane GetSurfacePlane()
        {
            ObjectPlacementSettings objectPlacementSettings = ObjectPlacementSettings.Get();
            if (objectPlacementSettings.ObjectPlacementMode == ObjectPlacementMode.DecorPaint) return ObjectPlacement.Get().DecorPaintObjectPlacement.DecorPaintSurfacePlane;
            else return ObjectSnapping.Get().ObjectSnapSurfacePlane;
        }

        public static GameObject GetSurfaceObject()
        {
            ObjectPlacementSettings objectPlacementSettings = ObjectPlacementSettings.Get();
            if (objectPlacementSettings.ObjectPlacementMode == ObjectPlacementMode.DecorPaint) return ObjectPlacement.Get().DecorPaintObjectPlacement.DecorPaintSurfaceObject;
            else return ObjectSnapping.Get().ObjectSnapSurfaceObject;
        }
        #endregion
    }
}
#endif