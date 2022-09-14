#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class SceneViewCamera : Singleton<SceneViewCamera>
    {
        #region Private Variables
        /// <summary>
        /// This holds the camera's far clip plane. This value is not used to modify the 
        /// actual clip plane value of the camera, but it will be used when constructing
        /// the camera view volume.
        /// </summary>
        private const float _farClipPlane = 1100.0f;

        private CameraViewVolume _viewVolume = new CameraViewVolume();
        private CameraDataSnapshot _cameraDataSnapshot;

        private List<GameObject> _visibleGameObjects = new List<GameObject>();
        private bool _objectVisibilityIsDirty = true;
        #endregion

        #region Public Static Properties
        public static Camera Camera { get { return Camera.current; } }
        #endregion

        #region Public Methods
        public void SetObjectVisibilityDirty()
        {
            _objectVisibilityIsDirty = true;
        }

        public CameraViewVolume GetViewVolume()
        {
            if (HasViewVolumeChanged())
            {
                _viewVolume.BuildForCamera(Camera, _farClipPlane); 
                _objectVisibilityIsDirty = true;
            }
            return _viewVolume;
        }

        public bool IsGameObjectVisible(GameObject gameObject)
        {
            Box worldBox = gameObject.GetWorldBox();
            if (worldBox.IsInvalid()) return false;

            return GetViewVolume().ContainsWorldSpaceAABB(worldBox.ToBounds());
        }

        public bool IsGameObjectHierarchyVisible(GameObject root)
        {
            Box worldBox = root.GetHierarchyWorldBox();
            if (worldBox.IsInvalid()) return false;

            return GetViewVolume().ContainsWorldSpaceAABB(worldBox.ToBounds());
        }

        public List<GameObject> GetVisibleGameObjects()
        {
            if (_objectVisibilityIsDirty)
            {
                List<GameObject> potentiallyVisibleGameObjects = GetPotentiallyVisibleGameObjects();
                if (potentiallyVisibleGameObjects.Count == 0) _visibleGameObjects = new List<GameObject>();
                else
                {
                    _visibleGameObjects = new List<GameObject>(potentiallyVisibleGameObjects.Count);
                    foreach (GameObject gameObject in potentiallyVisibleGameObjects)
                    {
                        if (IsGameObjectVisible(gameObject)) _visibleGameObjects.Add(gameObject);
                    }
                }

                _objectVisibilityIsDirty = false;
            }
     
            return _visibleGameObjects.FindAll(item => item != null);
        }
        #endregion

        #region Private Methods
        private bool HasViewVolumeChanged()
        {
            return EnsureDataSnapshotIsUpToDate();
        }

        private bool EnsureDataSnapshotIsUpToDate()
        {
            // If the snapshot data is null, we have to create it
            if (_cameraDataSnapshot == null)
            {
                _cameraDataSnapshot = new CameraDataSnapshot();
                _cameraDataSnapshot.TakeSnapshot(Camera);
                return true;
            }
            else
            {
                // Build a new camera data snapshot instance and check if it is different
                // from the current one. If it is, we will update the data snapshot reference.
                CameraDataSnapshot newCameraDataSnapshot = new CameraDataSnapshot();
                newCameraDataSnapshot.TakeSnapshot(Camera);
                if (newCameraDataSnapshot != _cameraDataSnapshot)
                {
                    _cameraDataSnapshot = newCameraDataSnapshot;
                    return true;
                }
                else return false;
            }
        }

        private List<GameObject> GetPotentiallyVisibleGameObjects()
        {
            var visibilitySphere = new CameraPotentialVisibleObjectsSphere();
            visibilitySphere.Calculate(Camera, GetViewVolume());

            return Octave3DScene.Get().OverlapSphere(visibilitySphere.Sphere);
        }
        #endregion
    }
}
#endif