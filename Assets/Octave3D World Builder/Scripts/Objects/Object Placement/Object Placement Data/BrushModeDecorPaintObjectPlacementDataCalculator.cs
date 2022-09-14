#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    /// <summary>
    /// This class is responsible for calculating the object placement data instances which
    /// describe the objects placed by the decor paint brush. The algorithm ATTEMPTS to generate
    /// the obejcts inside the brush circle in such a way that they don't intersect, howver,
    /// in certain cases, some amount of intersection can still occur.
    /// </summary>
    /// <remarks>
    /// The algorithm uses the object's oriented bounding boxes (OOBBs) to avoid intersection
    /// between the objects that are generated. Mesh-level precision is much more complicated 
    /// to implement and it may also not be very fast. For most common needs, box-level precision 
    /// should be enough. 
    /// </remarks>
    public class BrushModeDecorPaintObjectPlacementDataCalculator
    {
        #region Private Classes
        /// <summary>
        /// This class holds information about an object and the surface on which
        /// it resides.
        /// </summary>
        private class ObjectSurfaceData
        {
            #region Private Variables
            /// <summary>
            /// This is the projected object's box center onto the object surface.
            /// </summary>
            private Vector3 _basePosition;

            /// <summary>
            /// The normal of the surface on which the object resides.
            /// </summary>
            private Vector3 _surfaceNormal;
            #endregion

            #region Public Properties
            public Vector3 BasePosition { get { return _basePosition; } }
            public Vector3 SurfaceNormal { get { return _surfaceNormal; } }
            #endregion

            #region Constructors
            public ObjectSurfaceData(Vector3 basePosition, Vector3 surfaceNormal)
            {
                _basePosition = basePosition;
                _surfaceNormal = surfaceNormal;
            }

            public ObjectSurfaceData(ObjectSurfaceData source)
            {
                _basePosition = source.BasePosition;
                _surfaceNormal = source.SurfaceNormal;
            }
            #endregion

            #region Public Methods
            public Vector3 GetSurfaceTangentVector()
            {
                if (_surfaceNormal.IsAlignedWith(Vector3.up)) return Vector3.right;

                Vector3 tangent = Vector3.Cross(Vector3.up, _surfaceNormal);
                tangent.Normalize();

                return tangent;
            }
            #endregion
        }

        /// <summary>
        /// Holds a pair of object transform matrix and object oriented box.
        /// </summary>
        private class MatrixObjectBoxPair
        {
            #region Private Variables
            /// <summary>
            /// The object's transform matrix;
            /// </summary>
            private TransformMatrix _objectMatrix;

            /// <summary>
            /// The object's oriented box which describes the volume of the object
            /// when its transform is given by '_objectMatrix'.
            /// </summary>
            private OrientedBox _objectBox;
            #endregion

            #region Public Properties
            public TransformMatrix ObjectMatrix { get { return _objectMatrix; } }
            public OrientedBox ObjectBox { get { return new OrientedBox(_objectBox); } }
            #endregion

            #region Constructors
            public MatrixObjectBoxPair(TransformMatrix objectMatrix, OrientedBox objectBox)
            {
                _objectMatrix = objectMatrix;
                _objectBox = new OrientedBox(objectBox);
            }
            #endregion
        }

        /// <summary>
        /// Whenever we are about to create an object placement data instance (and sometimes even
        /// when we are not :) ), we will record a so called node which holds information about the
        /// object's volume and the surface on which it resides.
        /// </summary>
        private class ObjectNode
        {
            #region Private Variables
            /// <summary>
            /// The object's oriented box.
            /// </summary>
            private OrientedBox _objectBox;

            /// <summary>
            /// The object's surface data.
            /// </summary>
            private ObjectSurfaceData _objectSurfaceData;
            #endregion

            #region Public Properties
            public OrientedBox ObjectBox { get { return new OrientedBox(_objectBox); } }
            public ObjectSurfaceData ObjectSurfaceData { get { return new ObjectSurfaceData(_objectSurfaceData); } }
            public Vector3 Center { get { return _objectBox.Center; } }
            #endregion

            #region Constructors
            public ObjectNode(OrientedBox objectBox, ObjectSurfaceData objectSurfaceData)
            {
                _objectBox = new OrientedBox(objectBox);
                _objectSurfaceData = new ObjectSurfaceData(objectSurfaceData);
            }
            #endregion
        }

        /// <summary>
        /// Manages a list of object nodes which are connected between them via segments. The placement
        /// data generation algorithm will use this list to ensure that objects do not intersect with
        /// each other.
        /// </summary>
        private class ObjectNodeNetwork
        {
            #region Private Variables
            /// <summary>
            /// The list of nodes.
            /// </summary>
            private List<ObjectNode> _nodes = new List<ObjectNode>();
            #endregion

            #region Public Properties
            public int NumberOfNodes { get { return _nodes.Count; } }
            public int NumberOfSegments { get { return NumberOfNodes <= 1 ? 0 : NumberOfNodes; } }
            #endregion

            #region Public Static Functions
            public static Plane CalculateSegmentPlaneNormal(ObjectNode firstNode, ObjectNode secondNode)
            {
                Segment3D segment = CalculateSegmentBetweenNodes(firstNode, secondNode);
                Vector3 segmentPlaneNormal = Vector3.Cross(segment.Direction, firstNode.ObjectSurfaceData.SurfaceNormal);
                segmentPlaneNormal.Normalize();

                return new Plane(segmentPlaneNormal, segment.StartPoint);
            }

            public static Segment3D CalculateSegmentBetweenNodes(ObjectNode firstNode, ObjectNode secondNode)
            {
                return new Segment3D(firstNode.ObjectSurfaceData.BasePosition, secondNode.ObjectSurfaceData.BasePosition);
            }
            #endregion

            #region Public Methods
            public void Clear()
            {
                _nodes.Clear();
            }

            public int GetRandomNodeIndex()
            {
                return UnityEngine.Random.Range(0, NumberOfNodes);
            }

            public bool DoesNodeGenerateConcavity(int nodeIndex, Plane brushCirclePlane)
            {
                if (NumberOfNodes <= 3) return false;

                ObjectNode nodeBefore = GetNodeByIndex(nodeIndex - 1);
                ObjectNode node = GetNodeByIndex(nodeIndex);
                ObjectNode nodeAfter = GetNodeByIndex(nodeIndex + 1);

                Plane segmentPlane = CalculateSegmentPlaneNormal(nodeBefore, nodeAfter);

                if (segmentPlane.IsPointBehind(node.ObjectSurfaceData.BasePosition)) return true;
                else return false;
            }

            public void RemoveAllNodesWhichGenerateConcavities(Plane brushCirclePlane)
            {
                int nodeIndex = 0;
                while (nodeIndex < NumberOfNodes)
                {
                    if (DoesNodeGenerateConcavity(nodeIndex, brushCirclePlane)) RemoveNode(nodeIndex);
                    else ++nodeIndex;
                }
            }

            public void RemoveNode(int nodeIndex)
            {
                nodeIndex = WrapNodeIndex(nodeIndex);
                _nodes.RemoveAt(nodeIndex);
            }

            public ObjectNode GetNodeByIndex(int nodeIndex)
            {
                nodeIndex = WrapNodeIndex(nodeIndex);
                return _nodes[nodeIndex];
            }

            public ObjectNode GetFirstNode()
            {
                return _nodes[0];
            }

            public ObjectNode GetLastNode()
            {
                return _nodes[_nodes.Count - 1];
            }

            public void AddNodeToEnd(OrientedBox orientedBox, ObjectSurfaceData objectSurfaceData)
            {
                _nodes.Add(new ObjectNode(orientedBox, objectSurfaceData));
            }

            public void InsertAfterNode(OrientedBox orientedBox, ObjectSurfaceData objectSurfaceData, int indexOfPreviousNode)
            {
                if (NumberOfNodes == 0) return;

                indexOfPreviousNode %= NumberOfNodes;
                _nodes.Insert(indexOfPreviousNode + 1, new ObjectNode(orientedBox, objectSurfaceData));
            }
            #endregion

            #region Private Methods
            private int WrapNodeIndex(int nodeIndex)
            {
                if (NumberOfNodes == 0) return -1;

                nodeIndex %= NumberOfNodes;
                if (nodeIndex < 0) return NumberOfNodes + nodeIndex;
                return nodeIndex;
            }
            #endregion
        }
        #endregion

        #region Private Variables
        private bool _isSessionActive = false;
        private bool _allowObjectIntersection;
        private DecorPaintObjectPlacementBrush _workingBrush;
        private DecorPaintObjectPlacementBrushCircle _workingBrushCircle;

        private List<DecorPaintObjectPlacementBrushElement> _allValidBrushElements;
        private List<OrientedBox> _brushElementsPrefabOrientedBoxes;
        private List<Vector3> _brushElementsPrefabWorldScaleValues;
        private List<Sphere> _brushElementsPrefabSpheres;

        // Note: The following 2 are needed to take stroke alignment into account.
        private Dictionary<DecorPaintObjectPlacementBrushElement, Quaternion> _elementToCurrentPrefabRotation = new Dictionary<DecorPaintObjectPlacementBrushElement, Quaternion>();
        private Dictionary<DecorPaintObjectPlacementBrushElement, Quaternion> _elementToNewPrefabRotation = new Dictionary<DecorPaintObjectPlacementBrushElement, Quaternion>();

        private PointsOnColliderProjector _surfaceColliderProjector;
        private ObjectNodeNetwork _objectNodeNetwork = new ObjectNodeNetwork();
        private Quaternion _rotationToApplyForStrokeAlignment = Quaternion.identity;

        private DecorBrushElementSpawnChanceTable _brushElementSpawnChanceTable;
        #endregion

        #region Public Methods
        public void BeginSession(DecorPaintObjectPlacementBrushCircle workingBrushCircle, DecorPaintObjectPlacementBrush workingBrush)
        {
            if (workingBrushCircle == null || workingBrush == null) return;

            _workingBrushCircle = workingBrushCircle;
            _workingBrush = workingBrush;
            if (!AcquireNecessaryBrushElementData()) return;

            _isSessionActive = true;
            _brushElementSpawnChanceTable = _workingBrush.CalculateElementSpawnChanceTable(true);
        }

        public void EndSession()
        {
            _isSessionActive = false;
        }

        public List<ObjectPlacementData> Calculate(Quaternion rotationToApplyForStrokeAlignment)
        {
            if (!ValidateCalculationRequirements()) return new List<ObjectPlacementData>();

            _objectNodeNetwork.Clear();
            _rotationToApplyForStrokeAlignment = rotationToApplyForStrokeAlignment;
            CreateSurfaceColliderProjector();
            _elementToNewPrefabRotation.Clear();
            _allowObjectIntersection = ObjectPlacementSettings.Get().ObjectIntersectionSettings.AllowIntersectionForDecorPaintBrushModeDrag;

            int currentObjectIndex = 0;
            var objectPlacementDataInstances = new List<ObjectPlacementData>(_workingBrush.MaxNumberOfObjects);
            while (currentObjectIndex < _workingBrush.MaxNumberOfObjects)
            {
                DecorPaintObjectPlacementBrushElement brushElement = _brushElementSpawnChanceTable.PickEntity(UnityEngine.Random.Range(0.0f, 1.0f));
                int brushElementIndex = _allValidBrushElements.FindIndex(item => item == brushElement);
                ++currentObjectIndex;

                // No object nodes were created yet?
                if (_objectNodeNetwork.NumberOfNodes == 0)
                {
                    // Create the first node at a random position inside the brush circle
                    Vector3 randomPositionInsideCircle = _workingBrushCircle.GetRandomPointInside();
                    ObjectSurfaceData objectSurfaceData = CalculateObjectSurfaceData(randomPositionInsideCircle);
                    MatrixObjectBoxPair matrixObjectBoxPair = CalculateMatrixObjectBoxPair(brushElementIndex, objectSurfaceData);

                    TransformMatrix objectMatrix = matrixObjectBoxPair.ObjectMatrix;
                    objectMatrix.Rotation = matrixObjectBoxPair.ObjectBox.Rotation;
                    Vector3 boxCenter = matrixObjectBoxPair.ObjectBox.Center + objectSurfaceData.SurfaceNormal * brushElement.OffsetFromSurface;
                    objectMatrix.Translation = ObjectPositionCalculator.CalculateObjectHierarchyPosition(brushElement.Prefab, boxCenter, objectMatrix.Scale, matrixObjectBoxPair.ObjectBox.Rotation);

                    // We have been modifying the matrix and box data independently so we will ensure that the box uses the latest data
                    OrientedBox finalBox = new OrientedBox(matrixObjectBoxPair.ObjectBox);
                    finalBox.SetTransformMatrix(objectMatrix);

                    // We need to know if the normal of the surface on which the object resides lies within the desired slope range
                    bool passesSlopeTest = DoesObjectSurfacePassSlopeTest(objectSurfaceData, brushElement);

                    // Note: Even if the slope test is not passed, we will still create an object node. The reason for this is that
                    //       we want to have some kind of continuity in the algorithm. Imagine that the brush circle is large and is
                    //       divided by a large terrain mountain which sits in the middle. If the object generation starts on one side
                    //       of the mountain, the algorithm might never get a chance to go over the other side if the slope condition
                    //       is not satisifed. We want to spread objects as much as possible so even though this object will not be
                    //       placed in the scene, we will still add it to the node network.
                    _objectNodeNetwork.AddNodeToEnd(matrixObjectBoxPair.ObjectBox, objectSurfaceData);
                    if (passesSlopeTest && DoesBoxPassObjectIntersectionTest(finalBox, brushElement.Prefab.UnityPrefab, objectMatrix))
                        objectPlacementDataInstances.Add(new ObjectPlacementData(objectMatrix, brushElement.Prefab));
                }
                else
                {
                    // Are there any node segments available?
                    if (_objectNodeNetwork.NumberOfSegments != 0)
                    {
                        // The first step is to generate a random node index and store references to that node and its immediate neighbour
                        _objectNodeNetwork.RemoveAllNodesWhichGenerateConcavities(_workingBrushCircle.Plane);
                        int randomNodeIndex = _objectNodeNetwork.GetRandomNodeIndex();
                        ObjectNode firstNode = _objectNodeNetwork.GetNodeByIndex(randomNodeIndex);
                        ObjectNode secondNode = _objectNodeNetwork.GetNodeByIndex(randomNodeIndex + 1);

                        // Calculate the plane of the segment which unites the 2 nodes. We will also store the 
                        // actual segment and the middle point on the segment. We will use this middle point to
                        // generate the initial object position.
                        Segment3D nodeSegment = ObjectNodeNetwork.CalculateSegmentBetweenNodes(firstNode, secondNode);
                        Vector3 segmentMidPoint = nodeSegment.GetPoint(0.5f);
                        Plane segmentPlane = ObjectNodeNetwork.CalculateSegmentPlaneNormal(firstNode, secondNode);
                        OrientedBox firstNodeBox = firstNode.ObjectBox;
                        OrientedBox secondNodeBox = secondNode.ObjectBox;

                        // Calculate the furthest point in front of the plane using the corner points of the
                        // 2 nodes. The idea is to move the new object as much as possible from the bulk of
                        // objects that have already been generated.
                        Vector3 furthestPointFromPlane;
                        List<Vector3> nodeCornerPoints = firstNodeBox.GetCornerPoints();
                        nodeCornerPoints.AddRange(secondNodeBox.GetCornerPoints());
                        if (!segmentPlane.GetFurthestPointInFront(nodeCornerPoints, out furthestPointFromPlane)) continue;

                        // Use the calculated furthest point from plane and the the existing plane normal to calculate the 
                        // pivot plane. The new object will reside at some distance away from this plane.
                        Plane pivotPlane = new Plane(segmentPlane.normal, furthestPointFromPlane);

                        // Calculate the new object transform data. We will use the segment's mid point to generate the
                        // initial object position.
                        ObjectSurfaceData objectSurfaceData = CalculateObjectSurfaceData(segmentMidPoint);
                        MatrixObjectBoxPair matrixObjectBoxPair = CalculateMatrixObjectBoxPair(brushElementIndex, objectSurfaceData);
                        OrientedBox objectBox = matrixObjectBoxPair.ObjectBox;

                        // Identify the objects's furthest point behind the plane
                        Vector3 objectBoxPivotPoint;
                        List<Vector3> objectBoxCornerPoints = objectBox.GetCornerPoints();
                        if (!pivotPlane.GetFurthestPointBehind(objectBoxCornerPoints, out objectBoxPivotPoint)) continue;

                        // Use the furthest point to move the object in front of the plane and take the distance between objects into account
                        Vector3 fromPivotPointToCenter = objectBox.Center - objectBoxPivotPoint;
                        Vector3 projectedPivotPoint = pivotPlane.ProjectPoint(objectBoxPivotPoint);
                        objectBox.Center = projectedPivotPoint + fromPivotPointToCenter + pivotPlane.normal * _workingBrush.DistanceBetweenObjects;

                        // Generate the object surface data
                        objectSurfaceData = CalculateObjectSurfaceData(objectBox.Center);
                        bool passesSlopeTest = DoesObjectSurfacePassSlopeTest(objectSurfaceData, brushElement);

                        // Now we need to adjust the orientation and center of the box. If the calculated center
                        // lies outside the brush circle, we will ignore this node.
                        AdjustObjectBoxRotationOnSurface(objectBox, objectSurfaceData, brushElement);
                        AdjustObjectBoxCenterOnSurface(objectBox, objectSurfaceData, brushElement);
                        if (!_workingBrushCircle.ContainsPoint(_workingBrushCircle.Plane.ProjectPoint(objectBox.Center))) continue;

                        // Recalculate the object matrix using the new box data
                        TransformMatrix objectMatrix = matrixObjectBoxPair.ObjectMatrix;
                        objectMatrix.Rotation = objectBox.Rotation;
                        Vector3 boxCenter = objectBox.Center + objectSurfaceData.SurfaceNormal * brushElement.OffsetFromSurface;
                        objectMatrix.Translation = ObjectPositionCalculator.CalculateObjectHierarchyPosition(brushElement.Prefab, boxCenter, objectMatrix.Scale, objectBox.Rotation);
                           
                        // We have been modifying the matrix and box data independently so we will ensure that the box uses the latest data
                        OrientedBox finalBox = new OrientedBox(objectBox);
                        finalBox.SetTransformMatrix(objectMatrix);

                        // If the slope test passed, we will calculate an object placement data instance. Otherwise, we will just insert a new node.
                        if (passesSlopeTest && DoesBoxPassObjectIntersectionTest(finalBox, brushElement.Prefab.UnityPrefab, objectMatrix))
                            objectPlacementDataInstances.Add(new ObjectPlacementData(objectMatrix, brushElement.Prefab));

                        _objectNodeNetwork.InsertAfterNode(objectBox, objectSurfaceData, randomNodeIndex);
                    }
                    else
                    {
                        // When there are no segments available it means we have only one node. We will use this node to generate
                        // a new one at some distance away from it. First we will store some data that we will need during the entire
                        // procedure.
                        ObjectNode pivotNode = _objectNodeNetwork.GetFirstNode();
                        Vector3 pivotNodeSurfaceTangent = pivotNode.ObjectSurfaceData.GetSurfaceTangentVector();
                        OrientedBox pivotNodeObjectBox = pivotNode.ObjectBox;

                        // We will place the new node at some distance away from the first node's face which points
                        // along the calculated tangent vector. We will call this the pivot face.
                        BoxFace pivotBoxFace = pivotNodeObjectBox.GetBoxFaceMostAlignedWithNormal(pivotNodeSurfaceTangent);
                        Plane pivotFacePlane = pivotNodeObjectBox.GetBoxFacePlane(pivotBoxFace);

                        // Generate the data for the new node in the same position as the first node.
                        // Note: Although the same position is used, the rotation and scale will differ and they will
                        //       be established by 'CalculateMatrixObjectBoxPair'.
                        MatrixObjectBoxPair matrixObjectBoxPair = CalculateMatrixObjectBoxPair(brushElementIndex, pivotNode.ObjectSurfaceData);
                        OrientedBox objectBox = matrixObjectBoxPair.ObjectBox;

                        // At this point we have to start moving the generated object box to its new positino along the
                        // tangent vector. We will do this by calculating the furthest box point which lies behind the
                        // pivot plane and then move the box so that this point resides on that plane. We will call this
                        // the pivot point.
                        // Note: We will perform a safety check to see if this point could not be calculated and use the
                        //       closest point in front if necessary. However, this check should not be necessary. Because
                        //       we are placing te object box in the center of the previous box, we can be usre that there 
                        //       will always be a point which lies behind the pivot plane.
                        Vector3 objectBoxPivotPoint;
                        List<Vector3> objectBoxCornerPoints = objectBox.GetCornerPoints();
                        if (!pivotFacePlane.GetFurthestPointBehind(objectBoxCornerPoints, out objectBoxPivotPoint) &&
                            !pivotFacePlane.GetClosestPointInFront(objectBoxCornerPoints, out objectBoxPivotPoint)) continue;

                        // Project the pivot point onto the pivot plane. We will also store a vector which goes from the
                        // original pivot point to the box center. This will allow us to maintain the relationship between
                        // the projected pivot point and the box center so that the center can be adjusted accordingly.
                        Vector3 fromPivotPointToCenter = objectBox.Center - objectBoxPivotPoint;
                        Vector3 projectedPivotPoint = pivotFacePlane.ProjectPoint(objectBoxPivotPoint);

                        // Adjust the center using the projected pivot point and also take the distance between objects into account
                        objectBox.Center = projectedPivotPoint + fromPivotPointToCenter + pivotNodeSurfaceTangent * _workingBrush.DistanceBetweenObjects;

                        // Generate the object surface data at the current box position.
                        // Note: This is the step which can actually cause objects to intersect a little bit. The surface data is
                        //       calculated by projecting along the brush circle plane normal. If we are placing objects on a terrain
                        //       and the center of the circle lies somewhere at the base of the terrain where the normal points straight
                        //       up, but the center of the box resides somewhere on a clif, the new center might move the box closer
                        //       or even further away from the pivot node. This however, should not be a problem especially if the distance
                        //       between objects is not 0.
                        ObjectSurfaceData objectSurfaceData = CalculateObjectSurfaceData(objectBox.Center);
                        bool passesSlopeTest = DoesObjectSurfacePassSlopeTest(objectSurfaceData, brushElement);

                        // Now we need to adjust the orientation and center of the box. If the calculated center
                        // lies outside the brush circle, we will ignore this node.
                        AdjustObjectBoxRotationOnSurface(objectBox, objectSurfaceData, brushElement);
                        AdjustObjectBoxCenterOnSurface(objectBox, objectSurfaceData, brushElement);
                        if (!_workingBrushCircle.ContainsPoint(_workingBrushCircle.Plane.ProjectPoint(objectBox.Center))) continue;

                        // Recalculate the object matrix using the new box data
                        TransformMatrix objectMatrix = matrixObjectBoxPair.ObjectMatrix;
                        objectMatrix.Rotation = objectBox.Rotation;
                        Vector3 boxCenter = objectBox.Center + objectSurfaceData.SurfaceNormal * brushElement.OffsetFromSurface;
                        objectMatrix.Translation = ObjectPositionCalculator.CalculateObjectHierarchyPosition(brushElement.Prefab, boxCenter, objectMatrix.Scale, objectBox.Rotation);

                        // We have been modifying the matrix and box data independently so we will ensure that the box uses the latest data
                        OrientedBox finalBox = new OrientedBox(objectBox);
                        finalBox.SetTransformMatrix(objectMatrix);

                        // If the slope test passed, we will calculate an object placement data instance. Otherwise, we will just insert a new node.
                        if (passesSlopeTest && DoesBoxPassObjectIntersectionTest(finalBox, brushElement.Prefab.UnityPrefab, objectMatrix)) objectPlacementDataInstances.Add(new ObjectPlacementData(objectMatrix, brushElement.Prefab));
                        _objectNodeNetwork.InsertAfterNode(objectBox, objectSurfaceData, 0);
                    }
                }
            }

            // Adjust the prefab rotations for the next time the function is called
            if (_elementToNewPrefabRotation.Count != 0)
            {
                foreach (var prefabRotationPair in _elementToNewPrefabRotation)
                {
                    DecorPaintObjectPlacementBrushElement brushElement = prefabRotationPair.Key;
                    if (_elementToCurrentPrefabRotation.ContainsKey(brushElement)) _elementToCurrentPrefabRotation[brushElement] = prefabRotationPair.Value;
                }
            }

            return objectPlacementDataInstances;
        }
        #endregion

        #region Private Methods
        private bool ValidateCalculationRequirements()
        {
            if (!_isSessionActive) return false;
            if (_workingBrush.IsEmpty) return false;
            if (_workingBrush.MaxNumberOfObjects == 0) return false;

            return _workingBrushCircle.IsSittingOnGridCell || (_workingBrushCircle.IsSittingOnTerrain || _workingBrushCircle.IsSittingOnMesh || _workingBrushCircle.IsSittingOnSprite);
        }
        #endregion

        #region Private Methods
        private void CreateSurfaceColliderProjector()
        {
            MouseCursorRayHit brushCursorRayHit = _workingBrushCircle.CursorRayHit;
            if(brushCursorRayHit.WasAnObjectHit)
            {
                GameObjectRayHit objectRayHit = brushCursorRayHit.ClosestObjectRayHit;
                if (objectRayHit.WasMeshHit) _surfaceColliderProjector = new PointsOnColliderProjector(objectRayHit.ObjectMeshHit.HitCollider, _workingBrushCircle.Plane);
                else
                if (objectRayHit.WasTerrainHit) _surfaceColliderProjector = new PointsOnColliderProjector(objectRayHit.ObjectTerrainHit.HitCollider, _workingBrushCircle.Plane);
                else
                if (objectRayHit.WasSpriteHit) _surfaceColliderProjector = new PointsOnColliderProjector(objectRayHit.ObjectSpriteHit.HitCollider, _workingBrushCircle.Plane);
            }
        }

        private bool AcquireNecessaryBrushElementData()
        {
            _allValidBrushElements = _workingBrush.GetAllValidAndActiveBrushElements();
            if (_allValidBrushElements.Count == 0) return false;

            _brushElementsPrefabWorldScaleValues = _workingBrush.GetPrefabWorldScaleForAllValidAndActiveBrushElements();
            _brushElementsPrefabOrientedBoxes = _workingBrush.GetPrefabWorldOrientedBoxesForAllValidAndActiveBrushElements();

            _brushElementsPrefabSpheres = new List<Sphere>(_allValidBrushElements.Count);
            foreach (var orientedBox in _brushElementsPrefabOrientedBoxes)
            {
                _brushElementsPrefabSpheres.Add(orientedBox.GetEncapsulatingSphere());
            }

            _elementToCurrentPrefabRotation.Clear();
            foreach(var element in _allValidBrushElements)
            {
                _elementToCurrentPrefabRotation.Add(element, element.Prefab.UnityPrefab.transform.rotation);
            }

            return true;
        }

        private bool DoesObjectSurfacePassSlopeTest(ObjectSurfaceData objectSurfaceData, DecorPaintObjectPlacementBrushElement brushElement)
        {
            if (!_workingBrushCircle.IsSittingOnTerrain && brushElement.SlopeSettings.UseSlopeOnlyForTerrainObjects) return true;
            return brushElement.SlopeSettings.IsNormalInSlopeRange(objectSurfaceData.SurfaceNormal);
        }

        private ObjectSurfaceData CalculateObjectSurfaceData(Vector3 position)
        {
            Vector3 basePosition = Vector3.zero;
            Vector3 surfaceNormal = Vector3.up;

            if (_workingBrushCircle.IsSittingOnObject)
            {
                if (_workingBrush.IgnoreObjectsOutsideOfPaintSurface || _workingBrushCircle.IsSittingOnTerrain)
                {
                    if (!_surfaceColliderProjector.ProjectPoint(position, out basePosition, out surfaceNormal))
                    {
                        Plane brushCirclePlane = _workingBrushCircle.Plane;
                        basePosition = brushCirclePlane.ProjectPoint(position);
                        surfaceNormal = brushCirclePlane.normal;
                    }
                }
                else
                {
                    bool wasProjected = _surfaceColliderProjector.ProjectPoint(position, out basePosition, out surfaceNormal);
                    if (!wasProjected)
                    {
                        Plane brushCirclePlane = _workingBrushCircle.Plane;
                        basePosition = brushCirclePlane.ProjectPoint(position);
                        surfaceNormal = brushCirclePlane.normal;
                    }
                }
            }
            else
            {
                basePosition = _workingBrushCircle.Plane.ProjectPoint(position);
                surfaceNormal = _workingBrushCircle.CursorRayHit.GridCellRayHit.HitNormal;
            }

            return new ObjectSurfaceData(basePosition, surfaceNormal);
        }

        private MatrixObjectBoxPair CalculateMatrixObjectBoxPair(int brushElementIndex, ObjectSurfaceData objectSurfaceData)
        {
            // Store the needed data for easy access
            DecorPaintObjectPlacementBrushElement brushElement = _allValidBrushElements[brushElementIndex];
            OrientedBox objectBox = new OrientedBox(_brushElementsPrefabOrientedBoxes[brushElementIndex]);

            // Establish the object's scale. Randomize the scale if necessary. Otherwise, just use the brush element's scale.
            Vector3 xyzObjectBoxScale = _brushElementsPrefabWorldScaleValues[brushElementIndex];
            if (brushElement.ScaleRandomizationSettings.RandomizeScale)     
            {
                // Generate a random scale factor and apply it to the current scale
                ObjectUniformScaleRandomizationSettings uniformScaleRandomizationSettings = brushElement.ScaleRandomizationSettings.UniformScaleRandomizationSettings;
                xyzObjectBoxScale *= UnityEngine.Random.Range(uniformScaleRandomizationSettings.MinScale, uniformScaleRandomizationSettings.MaxScale); ;
            }
            else xyzObjectBoxScale *= brushElement.Scale;  

            // Apply the scale that we have calculated
            objectBox.Scale = xyzObjectBoxScale;

            // Now we will calculate the rotation. First we will calculate a quaternion which allow us to take the 
            // specified rotation offset into account. The quaternion rotates around the surface normal by an angle
            // of 'RotationOffset' degrees.
            Quaternion rotationOffset = brushElement.AlignToSurface ? Quaternion.AngleAxis(brushElement.RotationOffsetInDegrees, objectSurfaceData.SurfaceNormal) : Quaternion.identity;

            // If we need to align to stroke, we have some more work to do. Otherwise, we will just set the
            // rotation to be the same as the prefab's rotation, but offset by 'rotationOffset'.
            if (brushElement.AlignToStroke)
            {
                // First calculate the rotation without any offset. This is the rotation of the prefab plus the rotation which
                // must be applied for alignment.
                Quaternion rotationWithoutOffset = _rotationToApplyForStrokeAlignment * _elementToCurrentPrefabRotation[brushElement];

                // The rotation of the box must be set to the rotation which we just calculated (which ensures proper alingment) plus
                // the rotation offset.
                objectBox.Rotation = rotationOffset * rotationWithoutOffset;

                // Store the rotation inside the dictionary if an entry doesn't already exist
                if (!_elementToNewPrefabRotation.ContainsKey(brushElement)) _elementToNewPrefabRotation.Add(brushElement, rotationWithoutOffset);
            }
            else objectBox.Rotation = rotationOffset * brushElement.Prefab.UnityPrefab.transform.rotation;

            // Adjust the rotation of the object so that its axis is aligned with the placement surface if necessary
            if (brushElement.AlignToSurface) AdjustObjectBoxRotationOnSurface(objectBox, objectSurfaceData, brushElement);
  
            // The final step is to rotate the object by a random amount around the placement surface
            if(brushElement.RotationRandomizationMode != BrushElementRotationRandomizationMode.None)
            {
                Vector3 rotationAxis = objectSurfaceData.SurfaceNormal;
                if (brushElement.RotationRandomizationMode == BrushElementRotationRandomizationMode.X) rotationAxis = Vector3.right;
                else if (brushElement.RotationRandomizationMode == BrushElementRotationRandomizationMode.Y) rotationAxis = Vector3.up;
                else if (brushElement.RotationRandomizationMode == BrushElementRotationRandomizationMode.Z) rotationAxis = Vector3.forward;

                if(!brushElement.AlignToStroke)
                {
                    // When stroke alignment is not required, we will generate a random rotation angle and apply it
                    // to the current box rotation.
                    float randomRotationAngle = UnityEngine.Random.Range(0.0f, 360.0f);
                    Quaternion additionalRotation = Quaternion.AngleAxis(randomRotationAngle, rotationAxis);
                    objectBox.Rotation = additionalRotation * objectBox.Rotation;
                }
                else
                {
                    // When both rotation randomization and stroke alingment are turned on, we will proudce a small
                    // random rotation offset to randomize the alingmnet a little bit.
                    float randomRotationAngle = UnityEngine.Random.Range(0.0f, 25.0f);
                    Quaternion additionalRotation = Quaternion.AngleAxis(randomRotationAngle, rotationAxis);
                    objectBox.Rotation = additionalRotation * objectBox.Rotation;
                }
            }
           
            // Place the object on the surface
            AdjustObjectBoxCenterOnSurface(objectBox, objectSurfaceData, brushElement);

            // Construct the object matrix and return the object/marix pair
            TransformMatrix objectMatrix = new TransformMatrix(ObjectPositionCalculator.CalculateObjectHierarchyPosition(brushElement.Prefab,
                objectBox.Center, xyzObjectBoxScale, objectBox.Rotation), objectBox.Rotation, xyzObjectBoxScale);
            return new MatrixObjectBoxPair(objectMatrix, objectBox);
        }

        private void AdjustObjectBoxRotationOnSurface(OrientedBox objectBox, ObjectSurfaceData objectSurfaceData, DecorPaintObjectPlacementBrushElement brushElement)
        {
            if (brushElement.AlignToSurface) objectBox.Rotation = AxisAlignment.CalculateRotationQuaternionForAxisAlignment(objectBox.Rotation, brushElement.AlignmentAxis, objectSurfaceData.SurfaceNormal);
        }

        private void AdjustObjectBoxCenterOnSurface(OrientedBox objectBox, ObjectSurfaceData objectSurfaceData, DecorPaintObjectPlacementBrushElement brushElement)
        {
            if (brushElement.AlignToSurface) objectBox.Center = objectSurfaceData.BasePosition + objectSurfaceData.SurfaceNormal * 0.5f * objectBox.GetSizeAlongDirection(objectSurfaceData.SurfaceNormal);
            else
            {
                BoxFace boxFaceWhichFacesSurfaceNormal = objectBox.GetBoxFaceWhichFacesNormal(_workingBrushCircle.Plane.normal);
                Vector3 faceCenter = objectBox.GetBoxFaceCenter(boxFaceWhichFacesSurfaceNormal);
                Vector3 fromFaceCenterToBoxCenter = objectBox.Center - faceCenter;
                objectBox.Center = objectSurfaceData.BasePosition + fromFaceCenterToBoxCenter;
            }
        }

        private bool DoesBoxPassObjectIntersectionTest(OrientedBox objectBox, GameObject prefab, TransformMatrix objectMatrix)
        {
            if (_allowObjectIntersection) return true;
            return !Octave3DScene.Get().BoxIntersectsAnyObjectBoxes(objectBox, new List<GameObject> { DecorPaintObjectPlacement.Get().DecorPaintSurfaceObject }, true);
        }
        #endregion
    }
}
#endif