#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class MouseCursor : Singleton<MouseCursor>
    {
        #region Private Static Variables
        private static bool _defaultObjectMaskEnabledState = true;
        #endregion

        #region Private Variables
        private Vector2 _previousPosition;
        private Vector2 _offsetSinceLastMouseMove;

        private Stack<MouseCursorObjectPickFlags> _objectPickMaskFlagsStack = new Stack<MouseCursorObjectPickFlags>();
        private Stack<ObjectMask> _objectMaskStack = new Stack<ObjectMask>();
        private Stack<bool> _objectMaskEnabledStates = new Stack<bool>();
        #endregion

        #region Private Properties
        private ObjectMask ObjectMask { get { return _objectMaskStack.Count != 0 ? _objectMaskStack.Peek() : null; } }
        #endregion

        #region Public Properties
        public Vector2 PreviousPosition { get { return _previousPosition; } }
        public Vector2 OffsetSinceLastMouseMove { get { return _offsetSinceLastMouseMove; } }
        public Vector2 Position { get { return Event.current.mousePosition; } }
        public MouseCursorObjectPickFlags ObjectPickMaskFlags { get { return _objectPickMaskFlagsStack.Count != 0 ? _objectPickMaskFlagsStack.Peek() : MouseCursorObjectPickFlags.None; } }
        public bool IsObjectMaskEnabled { get { return _objectMaskEnabledStates.Count != 0 ? _objectMaskEnabledStates.Peek() : _defaultObjectMaskEnabledState; } }
        #endregion

        #region Public Methods
        public bool IsObjectPickMaskFlagSet(MouseCursorObjectPickFlags flag)
        {
            return (ObjectPickMaskFlags & flag) != 0;
        }

        public void PushObjectMaskEnabledState(bool enabled)
        {
            _objectMaskEnabledStates.Push(enabled);
        }

        public bool PopObjectMaskEnabledState()
        {
            if (_objectMaskEnabledStates.Count != 0) return _objectMaskEnabledStates.Pop();
            return _defaultObjectMaskEnabledState;
        }

        public void PushObjectPickMaskFlags(MouseCursorObjectPickFlags flags)
        {
            _objectPickMaskFlagsStack.Push(flags);
        }

        public MouseCursorObjectPickFlags PopObjectPickMaskFlags()
        {
            if (_objectPickMaskFlagsStack.Count != 0) return _objectPickMaskFlagsStack.Pop();
            return MouseCursorObjectPickFlags.None;
        }

        public void PushObjectMask(ObjectMask objectMask)
        {
            _objectMaskStack.Push(objectMask);
        }

        public ObjectMask PopObjectMask()
        {
            if (_objectMaskStack.Count != 0) return _objectMaskStack.Pop();
            return null;
        }

        public bool IsInsideSceneView()
        {
            return SceneViewCamera.Camera.pixelRect.Contains(Position);
        }

        public Ray GetWorldRay()
        {
            return HandleUtility.GUIPointToWorldRay(Position);
        }

        public MouseCursorRayHit GetRayHit()
        {
            return new MouseCursorRayHit(GetGridCellRayHit(), GetObjectRayHitInstances());
        }

        public MouseCursorRayHit GetRayHitForMeshAndSpriteObjects(List<GameObject> gameObjects)
        {
            var ray = GetWorldRay();
            var hits = new List<GameObjectRayHit>();
            foreach (var gameObject in gameObjects)
            {
                GameObjectRayHit hit = null;
                if(gameObject.HasMesh())
                {
                    if (gameObject.RaycastMesh(ray, out hit)) hits.Add(hit);
                }
                else
                if(gameObject.IsSprite())
                {
                    if (gameObject.RaycastSprite(ray, out hit)) hits.Add(hit);
                }
            }

            SortObjectRayHitListByHitDistanceFromCamera(hits);
            return new MouseCursorRayHit(null, hits);
        }

        public MouseCursorRayHit GetCursorRayHitForGridCell()
        {
            GridCellRayHit gridCellHit = GetGridCellRayHit();
            if (gridCellHit == null) return null;

            return new MouseCursorRayHit(gridCellHit, new List<GameObjectRayHit>());
        }

        public MouseCursorRayHit GetCursorRayHitForTerrainObject(GameObject gameObject)
        {
            if (!gameObject.HasTerrain()) return new MouseCursorRayHit(null, new List<GameObjectRayHit>());

            GameObjectRayHit gameObjectRayHit;
            if (gameObject.RaycastTerrain(GetWorldRay(), out gameObjectRayHit)) return new MouseCursorRayHit(null, new List<GameObjectRayHit> { gameObjectRayHit });

            return new MouseCursorRayHit(null, new List<GameObjectRayHit>());
        }

        public GridCellRayHit GetGridCellRayHit()
        {
            Ray ray = GetWorldRay();

            float minT;
            XZGrid closestSnapGrid = GetClosestHitSnapGridAndMinT(ObjectSnapping.Get().GetAllSnapGrids(), ray, out minT);

            if (closestSnapGrid != null) return GetGridCellHit(closestSnapGrid, ray, minT);
            else return null;
        }

        public bool IntersectsPlane(Plane plane, out Vector3 intersectionPoint)
        {
            intersectionPoint = Vector3.zero;

            Ray ray = GetWorldRay();
            float t;
            if(plane.Raycast(ray, out t))
            {
                intersectionPoint = ray.GetPoint(t);
                return true;
            }

            return false;
        }

        public void HandleMouseMoveEvent(Event e)
        {
            _offsetSinceLastMouseMove = e.mousePosition - _previousPosition;
            _previousPosition = e.mousePosition;
        }
        #endregion

        #region Private Methods
        private List<GameObjectRayHit> GetObjectRayHitInstances()
        {
            Ray ray = GetWorldRay();
            var gameObjectHits = new List<GameObjectRayHit>();

            RaycastAllTerrainObjects(ray, gameObjectHits);
            RaycastAllObjectsNoTerrains(ray, gameObjectHits);

            ObjectMask activeObjectMask = ObjectMask;
            if (activeObjectMask != null && IsObjectMaskEnabled) gameObjectHits.RemoveAll(item => activeObjectMask.IsGameObjectMasked(item.HitObject));
            gameObjectHits.RemoveAll(item => !item.HitObject.activeSelf);

            SortObjectRayHitListByHitDistanceFromCamera(gameObjectHits);

            Vector3 cameraLook = SceneViewCamera.Camera.transform.forward;
            while (gameObjectHits.Count > 0)
            {
                if(gameObjectHits[0].WasBoxHit && gameObjectHits[0].HitObject.HasMesh())
                {
                    GameObjectRayHit meshHit;
                    if (gameObjectHits[0].HitObject.RaycastMesh(ray, out meshHit))
                    {
                        float dot = Vector3.Dot(meshHit.HitNormal, cameraLook);
                        if (dot > 0.0f)
                        {
                            gameObjectHits.RemoveAt(0);
                        }
                        else break;
                    }
                    else break;
                }
                else
                {
                    float dot = Vector3.Dot(gameObjectHits[0].HitNormal, cameraLook);
                    if (dot > 0.0f)
                    {
                        gameObjectHits.RemoveAt(0);
                    }
                    else break;
                }
            }

            return gameObjectHits;
        }

        private void RaycastAllTerrainObjects(Ray ray, List<GameObjectRayHit> terrainHits)
        {
            // Can we pick terrains?
            if (!IsObjectPickMaskFlagSet(MouseCursorObjectPickFlags.ObjectTerrain))
            {
                // We will use Unity's 'Physics' API for terrain picking because it is reasonable enough
                // to expect users to attach terrain colliders to their terrain objects.
                RaycastHit[] rayHits = Physics.RaycastAll(ray);
                if (rayHits.Length != 0)
                {
                    // Identify all terrain colliders which were picked
                    foreach (RaycastHit rayHit in rayHits)
                    {
                        // Picked a terrain collider?
                        if (rayHit.collider.GetType() == typeof(TerrainCollider))
                        {
                            // Create a game object hit instance and add it to the list
                            var terrainRayHit = new TerrainRayHit(ray, rayHit);
                            var gameObjectRayHit = new GameObjectRayHit(ray, rayHit.collider.gameObject, null, null, terrainRayHit, null);
                            terrainHits.Add(gameObjectRayHit);
                        }
                    }
                }
            }
        }

        private void RaycastAllObjectsNoTerrains(Ray ray, List<GameObjectRayHit> objectHits)
        {
            bool canPickMeshObjects = !IsObjectPickMaskFlagSet(MouseCursorObjectPickFlags.ObjectMesh);
            bool canPickBoxes = !IsObjectPickMaskFlagSet(MouseCursorObjectPickFlags.ObjectBox);
            bool canPickSprites = !IsObjectPickMaskFlagSet(MouseCursorObjectPickFlags.ObjectSprite);

            if (canPickMeshObjects && canPickBoxes && canPickSprites)
            {
                List<GameObjectRayHit> objectMeshHits = Octave3DScene.Get().RaycastAllMesh(ray);
                if (objectMeshHits.Count != 0) objectHits.AddRange(objectMeshHits);

                List<GameObjectRayHit> objectBoxHits = Octave3DScene.Get().RaycastAllBox(ray);
                objectBoxHits.RemoveAll(item => item.HitObject.HasMesh() || item.HitObject.HasSpriteRendererWithSprite());
                if (objectBoxHits.Count != 0) objectHits.AddRange(objectBoxHits);

                List<GameObjectRayHit> objectSpriteHits = Octave3DScene.Get().RaycastAllSprite(ray);
                objectSpriteHits.RemoveAll(item => item.HitObject.HasMesh() || item.HitObject.GetComponent<SpriteRenderer>().IsPixelFullyTransparent(item.HitPoint));
                if (objectSpriteHits.Count != 0) objectHits.AddRange(objectSpriteHits);
            }
            else
            {
                if (!IsObjectPickMaskFlagSet(MouseCursorObjectPickFlags.ObjectMesh))
                {
                    List<GameObjectRayHit> objectMeshHits = Octave3DScene.Get().RaycastAllMesh(ray);
                    if (objectMeshHits.Count != 0) objectHits.AddRange(objectMeshHits);
                }

                if(!IsObjectPickMaskFlagSet(MouseCursorObjectPickFlags.ObjectSprite))
                {
                    List<GameObjectRayHit> objectSpriteHits = Octave3DScene.Get().RaycastAllSprite(ray);
                    objectSpriteHits.RemoveAll(item => objectHits.Contains(item) || item.HitObject.GetComponent<SpriteRenderer>().IsPixelFullyTransparent(item.HitPoint));
                    if (objectSpriteHits.Count != 0) objectHits.AddRange(objectSpriteHits);
                }

                if (!IsObjectPickMaskFlagSet(MouseCursorObjectPickFlags.ObjectBox))
                {
                    List<GameObjectRayHit> objectBoxHits = Octave3DScene.Get().RaycastAllBox(ray);
                    objectBoxHits.RemoveAll(item => objectHits.Contains(item));
                    if (objectBoxHits.Count != 0) objectHits.AddRange(objectBoxHits);
                }
            }
        }

        private XZGrid GetClosestHitSnapGridAndMinT(List<XZGrid> allSnapGrids, Ray ray, out float minT)
        {
            minT = float.MaxValue;

            XZGrid closestSnapGrid = null;
            foreach (XZGrid snapGrid in allSnapGrids)
            {
                float t;
                if (snapGrid.Plane.Raycast(ray, out t) & t < minT)
                {
                    minT = t;
                    closestSnapGrid = snapGrid;
                }
            }

            return closestSnapGrid;
        }

        private GridCellRayHit GetGridCellHit(XZGrid hitGrid, Ray ray, float t)
        {
            XZGridCell hitGridCell = hitGrid.GetCellFromPoint(ray.GetPoint(t));
            return new GridCellRayHit(ray, t, hitGridCell);
        }
                
        private void SortObjectRayHitListByHitDistanceFromCamera(List<GameObjectRayHit> objectRayHitInstances)
        {
            Vector3 sceneCameraPosition = SceneViewCamera.Camera.transform.position;
            objectRayHitInstances.Sort(delegate(GameObjectRayHit firstObjectHit, GameObjectRayHit secondObjectHit)
            {
                float firstPickPointDistanceFromCamera = (firstObjectHit.HitPoint - sceneCameraPosition).magnitude;
                float secondPickPointDistanceFromCamera = (secondObjectHit.HitPoint - sceneCameraPosition).magnitude;

                return firstPickPointDistanceFromCamera.CompareTo(secondPickPointDistanceFromCamera);
            });
        }
        #endregion
    }
}
#endif