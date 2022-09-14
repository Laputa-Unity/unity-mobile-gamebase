#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathManualConstructionSession
    {
        #region Private Variables
        private ObjectPlacementPath _path;
        private ObjectPlacementPathSettings _pathSettings;
        private ObjectPlacementPathTileConnectionSettings _tileConnectionSettings;
        private List<ObjectPlacementPathTileConnectionGridCell> _tileConnectionGridCells;
        private ObjectPlacementPathManualConstructionSettings _manualConstructionSettings;
        private ObjectPlacementPathHeightAdjustmentSettings _heightAdjustmentSettings;
        private ObjectPlacementPathPaddingSettings _paddingSettings;
        private ObjectPlacementPathBorderSettings _borderSettings;
        private List<ObjectPlacementBoxStackSegment> _pathSegments;
        private ObjectPlacementExtensionPlane _pathExtensionPlane;

        private GameObject _startObject;
        private OrientedBox _startObjectHierarchyWorldOrientedBox;
        private float _tileConnectionXZSize;

        private ObjectPlacementPathManualHeightAdjuster _manualHeightAdjuster = new ObjectPlacementPathManualHeightAdjuster();
        private int _currentManualPathHeight = 1;
        private ObjectPlacementPathAutomaticRandomHeightAdjuster _automaticRandomHeightAdjuster = new ObjectPlacementPathAutomaticRandomHeightAdjuster();
        private ObjectPlacementPathAutomaticPatternHeightAdjuster _automaticPatternHeightAdjuster = new ObjectPlacementPathAutomaticPatternHeightAdjuster();

        private ObjectPlacementPathBorderApplyOperation _borderApplyOperation = new ObjectPlacementPathBorderApplyOperation();
        private ObjectPlacementPathTileConnectionDetector _tileConnectionDetector = new ObjectPlacementPathTileConnectionDetector();

        private PathNoTileConnectionsObjectPlacementDataCaculator _pathNoTileConnectionsObjectPlacementDataCalculator = new PathNoTileConnectionsObjectPlacementDataCaculator();
        private PathWithTileConnectionsObjectPlacementDataCalculator _pathWithTileConnectionsObjectPlacementDataCalculator = new PathWithTileConnectionsObjectPlacementDataCalculator();

        private bool _isActive = false;
        #endregion

        #region Private Properties
        private ObjectPlacementBoxStackSegment LastSegment { get { return _pathSegments.Count != 0 ? _pathSegments[_pathSegments.Count - 1] : null; } }
        private ObjectPlacementBoxStackSegment PenultimateSegment { get { return _pathSegments.Count > 1 ? _pathSegments[_pathSegments.Count - 2] : null; } }
        #endregion

        #region Public Properties
        public bool IsActive { get { return _isActive; } }
        #endregion

        #region Public Methods
        public void SetData(ObjectPlacementPathManualConstructionSessionData sessionData)
        {
            if(!_isActive)
            {
                _path = sessionData.Path;
                _pathSegments = sessionData.PathSegments;
                _pathExtensionPlane = sessionData.PathExtensionPlane;
                _tileConnectionGridCells = sessionData.TileConnectionGridCells;

                _startObject = sessionData.StartObject;
                _startObjectHierarchyWorldOrientedBox = _startObject.GetHierarchyWorldOrientedBox();

                _pathSettings = _path.Settings;
                _tileConnectionSettings = _pathSettings.TileConnectionSettings;
                _manualConstructionSettings = _pathSettings.ManualConstructionSettings;
                _heightAdjustmentSettings = _manualConstructionSettings.HeightAdjustmentSettings;
                _paddingSettings = _manualConstructionSettings.PaddingSettings;
                _borderSettings = _manualConstructionSettings.BorderSettings;

                _pathNoTileConnectionsObjectPlacementDataCalculator.Path = _path;
                _pathWithTileConnectionsObjectPlacementDataCalculator.Path = _path;
            }
        }

        public void Begin()
        {
            if (CanBegin())
            {
                _isActive = true;
                _currentManualPathHeight = 1;
                _pathSegments.Clear();

                if (_tileConnectionSettings.UseTileConnections) EstablshTileConnectionXZSize();
                CreateFirst2Segments();
            }
        }

        public List<ObjectPlacementData> End()
        {
            if(_isActive)
            {
                _isActive = false;
                RemoveSegmentsWithNoStacks();

                if (_tileConnectionSettings.UseTileConnections) return _pathWithTileConnectionsObjectPlacementDataCalculator.Calculate();
                else return _pathNoTileConnectionsObjectPlacementDataCalculator.Calculate();
            }

            return new List<ObjectPlacementData>();
        }

        public void Cancel()
        {
            _isActive = false;
        }

        public void ManualRaisePath()
        {
            if (CanManualRaiseOrLowerPath())
            {
                _currentManualPathHeight = _manualHeightAdjuster.Raise(_path, _currentManualPathHeight);
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
            }
        }

        public void ManualLowerPath()
        {
            if (CanManualRaiseOrLowerPath())
            {
                _currentManualPathHeight = _manualHeightAdjuster.Lower(_path, _currentManualPathHeight);
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
            }
        }

        public void UpdateForMouseMoveEvent()
        {
            if (_isActive) ExtendOrShrinkPathAlongExtensionPlane();
        }

        public void Attach2NewSegments()
        {
            if(_isActive)
            {
                RemoveSegmentsWithNoStacks();

                // First create the 2 new segments which will replace the current penultimate and last segments
                ObjectPlacementBoxStackSegment oldLastSegment = LastSegment;
                ObjectPlacementBoxStackSegment newPenultimateSegment = CreateNewSegment();
                CreateNewSegment();

                // We will want the first stack in the new penultimate segment to replace the last stack in 'LastSegment'.
                // Otherwise, when this function is called, an additional box will suddenly appear in the scene apparently 
                // out of nowhere and it doesn't look very good. Not to mention that it can be quite confusing :).
                oldLastSegment.Shrink(1);
                newPenultimateSegment.Extend(1);

                newPenultimateSegment.SetExtensionDirection(oldLastSegment.ExtensionDirection);
                if (CanRotateObjectsToFollowPath())
                {
                    newPenultimateSegment.SetRotationForAllStacks(oldLastSegment.StackRotation);
                    MakeSegmentFollowPath(LastSegment);
                }
                newPenultimateSegment.ConnectFirstStackToLastStackInSegment(oldLastSegment, CalculateSegmentConnectionOffset(newPenultimateSegment, oldLastSegment));

                AdjustHeightForEntireSegment(PenultimateSegment);

                UpdateStackOverlapDataForLast2Segments();
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
                if (_tileConnectionSettings.UseTileConnections) UpdateTileConnectionInformation();
            }
        }

        public void RemoveLast2Segments()
        {
            if(_isActive)
            {
                _pathSegments.RemoveLast();
                _pathSegments.RemoveLast();

                if (_pathSegments.Count == 0) End();
            }
        }

        public void OnExcludeCornersSettingsChanged()
        {
            if (_isActive && !_tileConnectionSettings.UseTileConnections)
            {
                ReconnectAllSegments();
                HandleStackOverlapForAllStacksInAllSegments();
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);

                SceneView.RepaintAll();
            }
        }

        public void OnRotateObjectsToFollowPathSettingsChanged()
        {
            if (_isActive && !_tileConnectionSettings.UseTileConnections)
            {
                AdjustRotationForAllSegments();
                ReconnectAllSegments();
                HandleStackOverlapForAllStacksInAllSegments();
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);

                SceneView.RepaintAll();
            }
        }

        public void OnPaddingSettingsChanged()
        {
            if (_isActive && !_tileConnectionSettings.UseTileConnections)
            {
                ObjectPlacementBoxStackSegmentActions.SetPaddingForSegments(_pathSegments, _paddingSettings.PaddingAlongExtensionPlane, _paddingSettings.PaddingAlongGrowDirection);
                ReconnectAllSegments();
                HandleStackOverlapForAllStacksInAllSegments();
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);

                SceneView.RepaintAll();
            }
        }

        public void OnBorderSettingsChanged()
        {
            if (_isActive)
            {
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
                else ObjectPlacementBoxStackSegmentActions.ClearHideFlagsForAllStacksInSegments(_pathSegments, ObjectPlacementBoxHideFlags.PathApplyBorders);

                SceneView.RepaintAll();
            }
        }

        public void OnHeightAdjustmentModeChanged()
        {
            if (_isActive)
            {
                AdjustHeightForAllStacksInPath();
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
                SceneView.RepaintAll();
            }
        }

        public void OnAutomaticRandomHeightAdjustmentSettingsChanged()
        {
            if (_isActive)
            {
                AdjustHeightForAllStacksInPath();
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
                SceneView.RepaintAll();
            }
        }

        public void OnAutomaticPatternHeightAdjustmentSettingsChanged()
        {
            if (_isActive)
            {
                AdjustHeightForAllStacksInPath();
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
                SceneView.RepaintAll();
            }
        }

        public void OnHeightPatternRemoved()
        {
            if(_isActive && _heightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticPattern)
            {
                ObjectPlacementPathHeightPattern activePattern = ObjectPlacementPathHeightPatternDatabase.Get().ActivePattern;
                if (activePattern != null)
                {
                    AdjustHeightForAllStacksInPath();
                    if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
                }
                else
                {
                    Debug.LogWarning("There is no height pattern currently available. Path construction was cancelled.");
                    End();
                }
                
                SceneView.RepaintAll();
            }
        }

        public void OnNewHeightPatternWasActivated()
        {
            if (_isActive && _heightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticPattern)
            {
                AdjustHeightForAllStacksInPath();
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
                SceneView.RepaintAll();
            }
        }
        #endregion

        #region Private Methods
        private bool CanBegin()
        {
            return !_isActive && IsSessionDataReady();
        }

        private bool CanRotateObjectsToFollowPath()
        {
            return _manualConstructionSettings.RotateObjectsToFollowPath && !_tileConnectionSettings.UseTileConnections;
        }

        private bool CanUsePadding()
        {
            return !_tileConnectionSettings.UseTileConnections;
        }

        private bool CanExcludeCorners()
        {
            return _manualConstructionSettings.ExcludeCorners && !_tileConnectionSettings.UseTileConnections;
        }

        private bool CanManualRaiseOrLowerPath()
        {
            return _isActive && _heightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.Manual;
        }

        private void EstablshTileConnectionXZSize()
        {
            if(_pathSettings.TileConnectionSettings.UsesSprites())
            {
                Vector3 scaledSize = _startObjectHierarchyWorldOrientedBox.ScaledSize;
                _tileConnectionXZSize = (scaledSize.x + scaledSize.y) * 0.5f;
                _pathWithTileConnectionsObjectPlacementDataCalculator.TileConnectionXZSize = _tileConnectionXZSize;
            }
            else
            {
                Vector3 scaledSize = _startObjectHierarchyWorldOrientedBox.ScaledSize;
                _tileConnectionXZSize = (scaledSize.x + scaledSize.z) * 0.5f;
                _pathWithTileConnectionsObjectPlacementDataCalculator.TileConnectionXZSize = _tileConnectionXZSize;
            }
        }

        private bool IsSessionDataReady()
        {
            bool isReady = (_path != null && _pathSegments != null && _startObject != null && _pathExtensionPlane != null);
            if (!isReady) return false;

            if (_heightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticPattern &&
                ObjectPlacementPathHeightPatternDatabase.Get().ActivePattern == null)
            {
                Debug.LogWarning("Can not begin path construction because the automatic pattern height adjustment mode is selected but there is no active pattern.");
                isReady = false;
            }

            if (_tileConnectionSettings.UseTileConnections && !_tileConnectionSettings.DoAllTileConnectionsHavePrefabAssociated(true))
            {
                Debug.LogWarning("You are using tile connections, but not all tiles have an associated prefab. Please associate a prefab with each tile in order to being path construction.");
                isReady = false;
            }

            OrientedBox hierarchyWorldOrientedBox = _startObject.GetHierarchyWorldOrientedBox();
            float absSizeRight = hierarchyWorldOrientedBox.GetRotatedAndScaledSizeAlongDirection(_pathExtensionPlane.RightAxis);
            float absSizeLook = hierarchyWorldOrientedBox.GetRotatedAndScaledSizeAlongDirection(_pathExtensionPlane.LookAxis);
            if(absSizeRight < 1e-4f || absSizeLook < 1e-4f)
            {
                Debug.LogWarning("Can not begin path construction because the object has a 0 size component along the extention plane axes.");
                isReady = false;
            }

            return isReady;
        }

        private void RemoveSegmentsWithNoStacks()
        {
            _pathSegments.RemoveAll(item => item.NumberOfStacks == 0);
        }

        private void CreateFirst2Segments()
        {
            CreateNewSegment();
            CreateNewSegment();

            Vector3 camLookAxis = SceneViewCamera.Camera.transform.forward;
            Vector3 penultimateSegmentDir = _pathExtensionPlane.LookAxis;

            if(Mathf.Abs(Vector3.Dot(camLookAxis, _pathExtensionPlane.LookAxis)) <
               Mathf.Abs(Vector3.Dot(camLookAxis, _pathExtensionPlane.RightAxis)))
            {
                penultimateSegmentDir = _pathExtensionPlane.RightAxis;
            }

            PenultimateSegment.SetFirstStackBasePosition(_startObjectHierarchyWorldOrientedBox.Center);
            PenultimateSegment.SetExtensionDirection(penultimateSegmentDir);
            PenultimateSegment.Extend(1);
            AdjustHeightForEntireSegment(PenultimateSegment);
        }

        private ObjectPlacementBoxStackSegment CreateNewSegment()
        {
            var newSegment = new ObjectPlacementBoxStackSegment();
            newSegment.SetRotationForAllStacks(_startObjectHierarchyWorldOrientedBox.Rotation);
            newSegment.SetBoxSizeForAllStacks(GetSegmentBoxSize());
            newSegment.SetExtensionDirection(_pathExtensionPlane.LookAxis);
            newSegment.SetGrowAxis(_pathExtensionPlane.UpAxis);
 
            if (CanUsePadding())
            {
                newSegment.SetPaddingAlongStackGrowDirection(_paddingSettings.PaddingAlongGrowDirection);
                newSegment.SetPaddingAlongExtensionDirection(_paddingSettings.PaddingAlongExtensionPlane);
            }

            _pathSegments.Add(newSegment);

            return newSegment;
        }

        private Vector3 GetSegmentBoxSize()
        {
            if (_tileConnectionSettings.UseTileConnections)
            {
                if (_pathSettings.TileConnectionSettings.UsesSprites()) return new Vector3(_tileConnectionXZSize, _tileConnectionXZSize, _startObjectHierarchyWorldOrientedBox.ScaledSize.z);
                return new Vector3(_tileConnectionXZSize, _startObjectHierarchyWorldOrientedBox.ScaledSize.y, _tileConnectionXZSize);
            }
            else return _startObjectHierarchyWorldOrientedBox.ScaledSize;
        }

        private void ExtendOrShrinkPathAlongExtensionPlane()
        {
            // Construct a new extension plane in order to take into account the block's Y offset. Otherwise,
            // it becomes harder to control the block extension.
            Plane extensionPlane = _pathExtensionPlane.Plane;
            Vector3 pointOnBlockExtensionPlane = _pathExtensionPlane.PlaneQuad.Center;
            pointOnBlockExtensionPlane += extensionPlane.normal * _manualConstructionSettings.OffsetAlongGrowDirection;
            extensionPlane = new Plane(extensionPlane.normal, pointOnBlockExtensionPlane);
          
            // We will need to adjust the last 2 segments using the intersection point between the mouse cursor
            // and the extension plane, so the first thing that we will do is find this intersection point. This
            // intersection point will essentially help us visualize a triangle whose adjacent sides represent the
            // extension directions of the 2 segments. The hypotenuse represents the vector which goes from the
            // first stack in the penultimate segment to the last stack in the last segment.
            Vector3 extensionPlaneIntersectionPoint;
            if (MouseCursor.Instance.IntersectsPlane(extensionPlane, out extensionPlaneIntersectionPoint))
            {
                // We will need to adjust the number of stacks in each segment based on the length of the triangle's
                // adjacent sides. We will start with the penultimate segment and construct a vector which goes from
                // the first stack in the penultimate segment to the intersection point with the plane. Projecting this
                // vector on the extension direction of the penultimate segment will give us the length of the adjacent
                // side. This length is then used to calculate the number of stacks in the penultimate segment.
                Vector3 toIntersectionPoint = extensionPlaneIntersectionPoint - _pathExtensionPlane.Plane.ProjectPoint(PenultimateSegment.FirstStackBasePosition);
                
                if (_pathSegments.Count == 2 && PenultimateSegment.NumberOfStacks == 1 && !_pathSettings.TileConnectionSettings.UseTileConnections)
                {
                    var extensionPlaneAxes = new List<Vector3> { _pathExtensionPlane.RightAxis, -_pathExtensionPlane.RightAxis, _pathExtensionPlane.LookAxis, -_pathExtensionPlane.LookAxis };
                    int mostAlignedVector = toIntersectionPoint.GetIndexOfMostAlignedVector(extensionPlaneAxes);
                    if (mostAlignedVector >= 0)
                    {
                        PenultimateSegment.SetExtensionDirection(extensionPlaneAxes[mostAlignedVector]);
                    }
                }
                else if (!PenultimateSegment.ExtensionDirection.IsPointingInSameGeneralDirection(toIntersectionPoint)) PenultimateSegment.ReverseExtensionDirection();

                // Calculate the number of stacks in the penultimate segment
                float adjacentSideLength = PenultimateSegment.ExtensionDirection.GetAbsDot(toIntersectionPoint);                      
                float numberOfStacks = adjacentSideLength / (PenultimateSegment.GetBoxSizeAlongNormalizedDirection(PenultimateSegment.ExtensionDirection) + _paddingSettings.PaddingAlongExtensionPlane);  
                int integerNumberOfStacks = (int)numberOfStacks + 1;                                        
                int currentNumberOfStacks = PenultimateSegment.NumberOfStacks;
                int deltaNumberOfStacks = integerNumberOfStacks - currentNumberOfStacks;
                if (deltaNumberOfStacks > 0 || (deltaNumberOfStacks == 0 && _heightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticPattern))
                {
                    PenultimateSegment.Extend(deltaNumberOfStacks);
                    AdjustHeightForStackRangeInSegment(PenultimateSegment, currentNumberOfStacks);
                }
                else PenultimateSegment.Shrink(Mathf.Abs(deltaNumberOfStacks));

                // We will have to do the same thing for the last segment. However, this time we will first detect its extension direction. The extension direction
                // is one of the extension plane's axes (right or look). The one we choose has to be the one which is not aligned with the penultimate segment's
                // extension direction (i.e. it has to be perpendicular to it).
                toIntersectionPoint = extensionPlaneIntersectionPoint - _pathExtensionPlane.Plane.ProjectPoint(PenultimateSegment.LastStackBasePosition);   
                Vector3 extensionPlaneRight = _pathExtensionPlane.RightAxis;
                Vector3 extensionPlaneLook = _pathExtensionPlane.LookAxis;
                Vector3 lastSegmentExtensionDirection = extensionPlaneRight;          
                if (lastSegmentExtensionDirection.IsAlignedWith(PenultimateSegment.ExtensionDirection)) lastSegmentExtensionDirection = extensionPlaneLook;    
                if (!lastSegmentExtensionDirection.IsPointingInSameGeneralDirection(toIntersectionPoint)) lastSegmentExtensionDirection *= -1.0f;                                                                                                                                                                        
                LastSegment.SetExtensionDirection(lastSegmentExtensionDirection);      

                // Calculate the number of stacks in the segment. 
                // Note: We no longer add 1 to the integer number of stacks because that stack is actually the last stack in the penultimate segment
                numberOfStacks = toIntersectionPoint.magnitude / (LastSegment.GetBoxSizeAlongNormalizedDirection(LastSegment.ExtensionDirection) + _paddingSettings.PaddingAlongExtensionPlane);
                integerNumberOfStacks = (int)numberOfStacks;
                currentNumberOfStacks = LastSegment.NumberOfStacks;
                deltaNumberOfStacks = integerNumberOfStacks - currentNumberOfStacks;
                if (deltaNumberOfStacks > 0 || (deltaNumberOfStacks == 0 && _heightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticPattern))
                {
                    LastSegment.Extend(deltaNumberOfStacks);
                    AdjustHeightForStackRangeInSegment(LastSegment, currentNumberOfStacks);
                }
                else LastSegment.Shrink(Mathf.Abs(deltaNumberOfStacks));

                if (CanRotateObjectsToFollowPath()) MakeSegmentFollowPath(LastSegment);
                LastSegment.ConnectFirstStackToLastStackInSegment(PenultimateSegment, CalculateSegmentConnectionOffset(LastSegment, PenultimateSegment));

                UpdateStackOverlapDataForLast2Segments();
                if (_borderSettings.UseBorders) _borderApplyOperation.ApplyBordersToAllPathSegments(_pathSegments, _borderSettings);
                if (_tileConnectionSettings.UseTileConnections) UpdateTileConnectionInformation();

                SceneView.RepaintAll();
            }
        }

        private void MakeSegmentFollowPath(ObjectPlacementBoxStackSegment segment)
        {
            float angleFromFirstSegment = Vector3.Angle(_pathSegments[0].ExtensionDirection, segment.ExtensionDirection);
            Quaternion rotation = Quaternion.AngleAxis(angleFromFirstSegment, _pathExtensionPlane.Plane.normal);
            segment.SetRotationForAllStacks(rotation * _startObjectHierarchyWorldOrientedBox.Rotation);
        }

        private Vector3 CalculateSegmentConnectionOffset(ObjectPlacementBoxStackSegment sourceSegment, ObjectPlacementBoxStackSegment destinationSegment)
        {
            if (!CanExcludeCorners()) return CalculateSegmentConnectionOffsetForNoCornerExclusion(sourceSegment, destinationSegment);
            else
            {
                // Note: Corner exclusion is applied only if the 2 segments are perpendicular.
                if (sourceSegment.ExtensionDirection.IsPerpendicularTo(destinationSegment.ExtensionDirection)) return CalculateSegmentConnectionOffsetForCornerExclusion(sourceSegment, destinationSegment);
                else return CalculateSegmentConnectionOffsetForNoCornerExclusion(sourceSegment, destinationSegment);
            }
        }

        private Vector3 CalculateSegmentConnectionOffsetForNoCornerExclusion(ObjectPlacementBoxStackSegment sourceSegment, ObjectPlacementBoxStackSegment destinationSegment)
        {
            // No offset if the destination segment doesn't have any stacks
            if (destinationSegment.NumberOfStacks == 0) return Vector3.zero;

            bool srcPointsInSameDirAsDestination;
            bool srcExtensionDirAlignedWithDestDir = sourceSegment.ExtensionDirection.IsAlignedWith(destinationSegment.ExtensionDirection, out srcPointsInSameDirAsDestination);
            float multiplicationSign = 1.0f;
            if(srcExtensionDirAlignedWithDestDir)
            {
                if (!srcPointsInSameDirAsDestination) multiplicationSign *= -1.0f;
            }

            float destBoxSizeAlongDestExtensionDir = destinationSegment.GetBoxSizeAlongNormalizedDirection(destinationSegment.ExtensionDirection);
            float srcBoxSizeAlongDestExtensionDir = sourceSegment.GetBoxSizeAlongNormalizedDirection(destinationSegment.ExtensionDirection);
            Vector3 offsetAlongDestExtensionDir = destinationSegment.ExtensionDirection * (destBoxSizeAlongDestExtensionDir - srcBoxSizeAlongDestExtensionDir) * 0.5f;

            float destBoxSizeAlongSrcExtensionDir = destinationSegment.GetBoxSizeAlongNormalizedDirection(sourceSegment.ExtensionDirection);
            float srcBoxSizeAlongSrcExtensionDir = sourceSegment.GetBoxSizeAlongNormalizedDirection(sourceSegment.ExtensionDirection);
            Vector3 offsetAlongSourceExtensionDir = multiplicationSign * sourceSegment.ExtensionDirection * ((srcBoxSizeAlongSrcExtensionDir + destBoxSizeAlongSrcExtensionDir) * 0.5f + _paddingSettings.PaddingAlongExtensionPlane);
          
            return offsetAlongDestExtensionDir + offsetAlongSourceExtensionDir;
        }

        private Vector3 CalculateSegmentConnectionOffsetForCornerExclusion(ObjectPlacementBoxStackSegment sourceSegment, ObjectPlacementBoxStackSegment destinationSegment)
        {
            bool srcPointsInSameDirAsDestination;
            bool srcExtensionDirAlignedWithDestDir = sourceSegment.ExtensionDirection.IsAlignedWith(destinationSegment.ExtensionDirection, out srcPointsInSameDirAsDestination);
            float multiplicationSign = 1.0f;
            if (srcExtensionDirAlignedWithDestDir)
            {
                if (!srcPointsInSameDirAsDestination) multiplicationSign *= -1.0f;
            }

            float destBoxSizeAlongDestExtensionDir = destinationSegment.GetBoxSizeAlongNormalizedDirection(destinationSegment.ExtensionDirection);
            float srcBoxSizeAlongDestExtensionDir = sourceSegment.GetBoxSizeAlongNormalizedDirection(destinationSegment.ExtensionDirection);
            Vector3 offsetAlongDestExtensionDir = destinationSegment.ExtensionDirection * Mathf.Abs((destBoxSizeAlongDestExtensionDir + srcBoxSizeAlongDestExtensionDir) * 0.5f);

            float destBoxSizeAlongSrcExtensionDir = destinationSegment.GetBoxSizeAlongNormalizedDirection(sourceSegment.ExtensionDirection);
            float srcBoxSizeAlongSrcExtensionDir = sourceSegment.GetBoxSizeAlongNormalizedDirection(sourceSegment.ExtensionDirection);
            Vector3 offsetAlongSourceExtensionDir = multiplicationSign * sourceSegment.ExtensionDirection * (srcBoxSizeAlongSrcExtensionDir + destBoxSizeAlongSrcExtensionDir) * 0.5f;

            return offsetAlongDestExtensionDir + offsetAlongSourceExtensionDir;
        }

        private void AdjustHeightForEntireSegment(ObjectPlacementBoxStackSegment segment)
        {
            ObjectPlacementPathHeightAdjustmentMode heightAdjustmentMode = _heightAdjustmentSettings.HeightAdjustmentMode;
            if (heightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.Manual) _manualHeightAdjuster.AdjustSegmentHeight(segment, _currentManualPathHeight);
            else if (heightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticRandom) _automaticRandomHeightAdjuster.AdjustSegmentHeight(segment, _heightAdjustmentSettings.AutomaticRandomHeightAdjustmentSettings);
            else _automaticPatternHeightAdjuster.AdjustSegmentHeight(segment, _pathSegments, _heightAdjustmentSettings.AutomaticPatternHeightAdjustmentSettings, ObjectPlacementPathHeightPatternDatabase.Get().ActivePattern);
        }

        private void AdjustHeightForStackRangeInSegment(ObjectPlacementBoxStackSegment segment, int indexOfFirstStackToAdjust)
        {
            ObjectPlacementPathHeightAdjustmentMode heightAdjustmentMode = _heightAdjustmentSettings.HeightAdjustmentMode;
            if (heightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.Manual) _manualHeightAdjuster.AdjustSegmentHeight(segment, indexOfFirstStackToAdjust, _currentManualPathHeight);
            else if (heightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticRandom) _automaticRandomHeightAdjuster.AdjustSegmentHeight(segment, indexOfFirstStackToAdjust, _heightAdjustmentSettings.AutomaticRandomHeightAdjustmentSettings);
            else _automaticPatternHeightAdjuster.AdjustSegmentHeight(segment, _pathSegments, _heightAdjustmentSettings.AutomaticPatternHeightAdjustmentSettings, ObjectPlacementPathHeightPatternDatabase.Get().ActivePattern);
        }

        private void ReconnectAllSegments()
        {
            for(int segmentIndex = 1; segmentIndex < _pathSegments.Count; ++segmentIndex)
            {
                ObjectPlacementBoxStackSegment currentSegment = _pathSegments[segmentIndex];
                ObjectPlacementBoxStackSegment previousSegment = _pathSegments[segmentIndex - 1];

                currentSegment.ConnectFirstStackToLastStackInSegment(previousSegment, CalculateSegmentConnectionOffset(currentSegment, previousSegment));
            }
        }

        private void HandleStackOverlapForAllStacksInAllSegments()
        {
            for (int segmentIndex = 0; segmentIndex < _pathSegments.Count; ++segmentIndex)
            {
                _pathSegments[segmentIndex].MarkAllStacksAsNotOverlapped();
                ObjectPlacementBoxStackActions.MarkStacksAsOverlapped(ObjectPlacementPathOverlappedStackDetection.GetOverlappedStacksInSegment(segmentIndex, _pathSegments));
            }
        }

        private void AdjustRotationForAllSegments()
        {
            foreach(ObjectPlacementBoxStackSegment segment in _pathSegments)
            {
                if (_manualConstructionSettings.RotateObjectsToFollowPath)
                {
                    if (_pathSegments.Count >= 2 && segment == _pathSegments[0] && segment.NumberOfStacks == 1 && _pathSegments[1].NumberOfStacks >= 1)
                    {
                        segment.LookStacksAlongDirection(_pathSegments[1].ExtensionDirection);
                    }
                    else segment.LookStacksAlongExtensionDirection();
                }
                else segment.LookStacksAlongDirection(_pathSegments[0].ExtensionDirection);
            }
        }

        private void AdjustHeightForAllStacksInPath()
        {
            if (_heightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.Manual) _manualHeightAdjuster.AdjustHeightForSegments(_pathSegments, _currentManualPathHeight);
            else if (_heightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementPathHeightAdjustmentMode.AutomaticRandom) _automaticRandomHeightAdjuster.AdjustHeightForSegments(_pathSegments, _heightAdjustmentSettings.AutomaticRandomHeightAdjustmentSettings);
            else _automaticPatternHeightAdjuster.AdjustHeightForAllSegmentsInPath(_pathSegments, _heightAdjustmentSettings.AutomaticPatternHeightAdjustmentSettings, ObjectPlacementPathHeightPatternDatabase.Get().ActivePattern);
        }

        private void UpdateTileConnectionInformation()
        {
            List<ObjectPlacementPathTileConnectionGridCell> tileConnectionGridCells = _tileConnectionDetector.Detect(_path, _tileConnectionXZSize);
            if (tileConnectionGridCells.Count == 0) return;
            _tileConnectionGridCells.Clear();
            _tileConnectionGridCells.AddRange(tileConnectionGridCells);

            bool usingSprites = _pathSettings.TileConnectionSettings.UsesSprites();
            List<OrientedBox> tileConnectionPrefabWorldOrientedBoxesExceptAutofill = PrefabQueries.GetHierarchyWorldOrientedBoxesForAllPrefabs(_tileConnectionSettings.GetAllTileConnectionPrefabs(true));
            foreach (ObjectPlacementPathTileConnectionGridCell tileConnectionGridCell in _tileConnectionGridCells)
            {
                ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings = _tileConnectionSettings.GetSettingsForTileConnectionType(tileConnectionGridCell.TileConnectionType);
                tileConnectionGridCell.TileConnectionStack.SetBoxSize(GetTileConnectionSize(tileConnectionPrefabWorldOrientedBoxesExceptAutofill[(int)tileConnectionTypeSettings.TileConnectionType], usingSprites));
                tileConnectionGridCell.TileConnectionStack.PlaceOnPlane(_pathExtensionPlane.Plane);
            }
        }

        private Vector3 GetTileConnectionSize(OrientedBox tilePrefabOOBB, bool usingSprites)
        {
            Vector3 boxSize = GetSegmentBoxSize();
            if (usingSprites) boxSize.z = 0.0f;
            else boxSize.y = tilePrefabOOBB.ScaledSize.y;

            return boxSize;
        }

        private void UpdateStackOverlapDataForLast2Segments()
        {
            PenultimateSegment.MarkAllStacksAsNotOverlapped();
            LastSegment.MarkAllStacksAsNotOverlapped();
            ObjectPlacementBoxStackActions.MarkStacksAsOverlapped(ObjectPlacementPathOverlappedStackDetection.GetOverlappedStacksInSegment(_pathSegments.Count - 2, _pathSegments));
            ObjectPlacementBoxStackActions.MarkStacksAsOverlapped(ObjectPlacementPathOverlappedStackDetection.GetOverlappedStacksInSegment(_pathSegments.Count - 1, _pathSegments));
        }
        #endregion
    }
}
#endif