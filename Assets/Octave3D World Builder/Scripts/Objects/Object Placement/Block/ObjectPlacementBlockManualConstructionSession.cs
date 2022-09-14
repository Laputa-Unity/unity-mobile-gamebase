#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementBlockManualConstructionSession
    {
        #region Private Variables
        private ObjectPlacementBlock _block;
        private List<ObjectPlacementBoxStackSegment> _blockSegments;
        private ObjectPlacementExtensionPlane _blockExtensionPlane;
        private ObjectPlacementBlockManualConstructionSettings _manualConstructionSettings;
        private ObjectPlacementBlockPaddingSettings _paddingSettings;
        private ObjectPlacementBlockHeightAdjustmentSettings _heightAdjustmentSettings;
        private ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings _automaticRandomHeightAdjustmentSettings;
        private ObjectPlacementBlockSubdivisionSettings _subdivisionSettings;

        private Vector3 _segmentExtensionDirection;
        private Vector3 _segmentCollectionExtensionDirection;

        private int _currentManualBlockHeight;
        private GameObject _startObject;
        private OrientedBox _startObjectHierarchyWorldOrientedBox;

        private BlockObjectPlacementDataCalculator _blockObjectPlacementDataCalculator = new BlockObjectPlacementDataCalculator();
        private ObjectPlacementBlockManualHeightAdjuster _manualHeightAdjuster = new ObjectPlacementBlockManualHeightAdjuster();
        private ObjectPlacementBlockAutomaticRandomHeightAdjuster _automaticRandomHeightAdjuster = new ObjectPlacementBlockAutomaticRandomHeightAdjuster();
        private ObjectPlacementBlockSubdivisionApplyOperation _subdivisionApplyOperation = new ObjectPlacementBlockSubdivisionApplyOperation();

        private bool _isActive = false;
        #endregion

        #region Private Properties
        private ObjectPlacementBoxStackSegment FirstSegment { get { return _blockSegments.Count != 0 ? _blockSegments[0] : null; } }
        private ObjectPlacementBoxStackSegment LastSegment { get { return _blockSegments.Count != 0 ? _blockSegments[_blockSegments.Count - 1] : null; } }
        #endregion

        #region Public Properties
        public bool IsActive { get { return _isActive; } }
        #endregion

        #region Public Methods
        public void SetData(ObjectPlacementBlockManualConstructionSessionData sessionData)
        {
            if (!_isActive)
            {
                _block = sessionData.Block;
                _blockSegments = sessionData.BlockSegments;
                _blockExtensionPlane = sessionData.BlockExtensionPlane;

                _startObject = sessionData.StartObject;
                _startObjectHierarchyWorldOrientedBox = _startObject.GetHierarchyWorldOrientedBox();
                _blockObjectPlacementDataCalculator.Block = _block;

                _manualConstructionSettings = _block.Settings.ManualConstructionSettings;
                _heightAdjustmentSettings = _manualConstructionSettings.HeightAdjustmentSettings;
                _automaticRandomHeightAdjustmentSettings = _heightAdjustmentSettings.AutomaticRandomHeightAdjustmentSettings;
                _paddingSettings = _manualConstructionSettings.PaddingSettings;
                _subdivisionSettings = _manualConstructionSettings.SubdivisionSettings;
            }
        }

        public void Begin()
        {
            if(CanBegin())
            {
                _isActive = true;
                _currentManualBlockHeight = 1;
                _blockSegments.Clear();

                _segmentExtensionDirection = _blockExtensionPlane.RightAxis;
                _segmentCollectionExtensionDirection = _blockExtensionPlane.LookAxis;

                CreateFirstSegmentInBlock();
            }
        }

        public List<ObjectPlacementData> End()
        {
            if (_isActive)
            {
                _isActive = false;
                return _blockObjectPlacementDataCalculator.Calculate();
            }

            return new List<ObjectPlacementData>();
        }

        public void Cancel()
        {
            _isActive = false;
        }

        public void ManualRaiseBlock()
        {
            if (CanManualRaiseOrLowerBlock())
            {
                _currentManualBlockHeight = _manualHeightAdjuster.Raise(_block, _currentManualBlockHeight);
                AdjustCornerExclusionHideFlagsForEntireBlock();
                if (_subdivisionSettings.UseSubdivision) _subdivisionApplyOperation.ApplySubdivisionToEntireBlock(_blockSegments, _subdivisionSettings);
            }
        }

        public void ManualLowerBlock()
        {
            if (CanManualRaiseOrLowerBlock())
            {
                _currentManualBlockHeight = _manualHeightAdjuster.Lower(_block, _currentManualBlockHeight);
                AdjustCornerExclusionHideFlagsForEntireBlock();
                if (_subdivisionSettings.UseSubdivision) _subdivisionApplyOperation.ApplySubdivisionToEntireBlock(_blockSegments, _subdivisionSettings);
            }
        }

        public void UpdateForMouseMoveEvent()
        {
            if (_isActive) ExtendOrShrinkBlockAlongExtensionPlane();
        }

        public void OnExcludeCornersSettingsChanged()
        {
            if(_isActive)
            {
                AdjustCornerExclusionHideFlagsForEntireBlock();
                SceneView.RepaintAll();
            }
        }

        public void OnPaddingSettingsChanged()
        {
            if (_isActive)
            {
                ObjectPlacementBoxStackSegmentActions.SetPaddingForSegments(_blockSegments, _paddingSettings.PaddingAlongExtensionPlane, _paddingSettings.PaddingAlongGrowDirection);
                for (int segmentIndex = 1; segmentIndex < _blockSegments.Count; ++segmentIndex)
                {
                    AppendSegmentToSegment(_blockSegments[segmentIndex], _blockSegments[segmentIndex - 1]);
                }

                SceneView.RepaintAll();
            }
        }

        public void OnHeightAdjustmentModeChanged()
        {
            if (_isActive)
            {
                AdjustHeightForStackRangeInAllSegments(0);
                if (_manualConstructionSettings.ExcludeCorners) AdjustCornerExclusionHideFlagsForEntireBlock();
                SceneView.RepaintAll();
            }
        }

        public void OnAutomaticRandomHeightAdjustmentSettingsChanged()
        {
            if (_isActive)
            {
                AdjustHeightForStackRangeInAllSegments(0);
                if (_manualConstructionSettings.ExcludeCorners) AdjustCornerExclusionHideFlagsForEntireBlock();
                SceneView.RepaintAll();
            }
        }

        public void OnSubdivisionSettingsChanged()
        {
            if(_isActive)
            {
                if (_subdivisionSettings.UseSubdivision) _subdivisionApplyOperation.ApplySubdivisionToEntireBlock(_blockSegments, _subdivisionSettings);
                else ObjectPlacementBoxStackSegmentActions.ClearHideFlagsForAllStacksInSegments(_blockSegments, ObjectPlacementBoxHideFlags.BlockApplySubdivisions);
                SceneView.RepaintAll();
            }
        }
        #endregion

        #region Private Methods
        private bool CanBegin()
        {
            return !_isActive && IsSessionDataReady();
        }

        private bool CanExcludeCorners()
        {
            return _manualConstructionSettings.ExcludeCorners && _blockSegments.Count >= 3 && FirstSegment.NumberOfStacks >= 3;
        }

        private bool CanManualRaiseOrLowerBlock()
        {
            return _isActive && _heightAdjustmentSettings.HeightAdjustmentMode == ObjectPlacementBlockHeightAdjustmentMode.Manual;
        }

        private bool IsSessionDataReady()
        {
            bool isReady = (_block != null && _blockSegments != null && _startObject != null && _blockExtensionPlane != null);
            if (!isReady) return false;

            OrientedBox hierarchyWorldOrientedBox = _startObject.GetHierarchyWorldOrientedBox();
            float absSizeRight = hierarchyWorldOrientedBox.GetRotatedAndScaledSizeAlongDirection(_blockExtensionPlane.RightAxis);
            float absSizeLook = hierarchyWorldOrientedBox.GetRotatedAndScaledSizeAlongDirection(_blockExtensionPlane.LookAxis);
            if (absSizeRight < 1e-4f || absSizeLook < 1e-4f)
            {
                Debug.LogWarning("Can not begin block construction because the object has a 0 size component along the extention plane axes.");
                isReady = false;
            }

            return isReady;
        }

        private void CreateFirstSegmentInBlock()
        {
            ObjectPlacementBoxStackSegment firstSegmentInBlock = CreateNewSegment();
            firstSegmentInBlock.SetExtensionDirection(_segmentExtensionDirection);
            firstSegmentInBlock.SetFirstStackBasePosition(_startObjectHierarchyWorldOrientedBox.Center);
        }

        private ObjectPlacementBoxStackSegment CreateNewSegment()
        {
            var newSegment = new ObjectPlacementBoxStackSegment();
            newSegment.SetExtensionDirection(_blockExtensionPlane.LookAxis);
            newSegment.SetGrowAxis(_blockExtensionPlane.UpAxis);
            newSegment.SetRotationForAllStacks(_startObjectHierarchyWorldOrientedBox.Rotation);
            newSegment.SetBoxSizeForAllStacks(_startObjectHierarchyWorldOrientedBox.ScaledSize);
            newSegment.SetPaddingAlongStackGrowDirection(_paddingSettings.PaddingAlongGrowDirection);
            newSegment.SetPaddingAlongExtensionDirection(_paddingSettings.PaddingAlongExtensionPlane);

            _blockSegments.Add(newSegment);

            return newSegment;
        }

        private void ExtendOrShrinkBlockAlongExtensionPlane()
        {
            // Construct a new extension plane in order to take into account the block's Y offset
            Plane extensionPlane = _blockExtensionPlane.Plane;
            Vector3 pointOnBlockExtensionPlane = _blockExtensionPlane.PlaneQuad.Center;
            pointOnBlockExtensionPlane += extensionPlane.normal * _manualConstructionSettings.OffsetAlongGrowDirection;
            extensionPlane = new Plane(extensionPlane.normal, pointOnBlockExtensionPlane);

            Vector3 extensionPlaneIntersectionPoint;
            if (MouseCursor.Instance.IntersectsPlane(extensionPlane, out extensionPlaneIntersectionPoint))
            {
                Vector3 toIntersectionPoint = extensionPlaneIntersectionPoint - _blockExtensionPlane.Plane.ProjectPoint(FirstSegment.FirstStackBasePosition);
                if (!FirstSegment.ExtensionDirection.IsPointingInSameGeneralDirection(toIntersectionPoint))
                {
                    ObjectPlacementBoxStackSegmentActions.ReverseExtensionDirectionForSegments(_blockSegments);
                    _segmentExtensionDirection = FirstSegment.ExtensionDirection;
                }

                if (!toIntersectionPoint.IsPointingInSameGeneralDirection(_segmentCollectionExtensionDirection))
                {
                    RemoveLastNumberOfSegments(_blockSegments.Count - 1);
                    _segmentCollectionExtensionDirection *= -1.0f;
                }

                // Calculate the number of stacks for all segments
                float adjacentSideLength = FirstSegment.ExtensionDirection.GetAbsDot(toIntersectionPoint);
                float numberOfStacks = adjacentSideLength / (FirstSegment.GetBoxSizeAlongNormalizedDirection(_segmentExtensionDirection) + _paddingSettings.PaddingAlongExtensionPlane);
                int integerNumberOfStacks = (int)numberOfStacks + 1;

                // Calculate the number of segments
                adjacentSideLength = toIntersectionPoint.GetAbsDot(_segmentCollectionExtensionDirection);
                float numberOfSegments = adjacentSideLength / (FirstSegment.GetBoxSizeAlongNormalizedDirection(_segmentCollectionExtensionDirection) + _paddingSettings.PaddingAlongExtensionPlane);
                int integerNumberOfSegments = (int)numberOfSegments + 1;
   
                int newNumberOfStacksInSegments = integerNumberOfStacks;
                int newNumberOfSegmentsInBlock = integerNumberOfSegments;
                if (AllShortcutCombos.Instance.Enable1To1RatioBlockAdjustment.IsActive())
                {
                    int min = Mathf.Min(newNumberOfSegmentsInBlock, newNumberOfStacksInSegments);
                    newNumberOfStacksInSegments = min;
                    newNumberOfSegmentsInBlock = min;
                }

                if (_manualConstructionSettings.ContrainSize)
                {
                    newNumberOfStacksInSegments = Mathf.Min(newNumberOfStacksInSegments, _manualConstructionSettings.MaxSize);
                    newNumberOfSegmentsInBlock = Mathf.Min(newNumberOfSegmentsInBlock, _manualConstructionSettings.MaxSize);
                }

                // Append or remove stacks from the segments based on the new number of stacks
                int deltaNumberOfStacks = newNumberOfStacksInSegments - FirstSegment.NumberOfStacks;
                AppendOrRemoveStacksToAllSegments(deltaNumberOfStacks);

                // Append or remove segments from the block based on the new number of segments
                int deltaNumberOfSegments = newNumberOfSegmentsInBlock - _blockSegments.Count;
                AppendOrRemoveSegmentsToBlock(deltaNumberOfSegments);

                // Apply any subdivision if necessary and adjust the corner exclusion hide flags. We need to do this every
                // time the block is updated because when the block shrinks or grows, its structure is affected and so is
                // the way in which subdivision and corner exclusion is applied.
                if (_subdivisionSettings.UseSubdivision) _subdivisionApplyOperation.ApplySubdivisionToEntireBlock(_blockSegments, _subdivisionSettings);
                AdjustCornerExclusionHideFlagsForEntireBlock();

                SceneView.RepaintAll();
            }
        }

        private void AppendOrRemoveStacksToAllSegments(int deltaNumberOfStacks)
        {
            if (deltaNumberOfStacks > 0)
            {
                if (CanExcludeCorners())
                {
                    LastSegment.GetStackByIndex(0).ClearHideFlagForAllBoxes(ObjectPlacementBoxHideFlags.BlockExcludeCorners);
                    LastSegment.GetStackByIndex(LastSegment.NumberOfStacks - 1).ClearHideFlagForAllBoxes(ObjectPlacementBoxHideFlags.BlockExcludeCorners);
                }

                int currentNumberOfStacksInSegments = FirstSegment.NumberOfStacks;
                ObjectPlacementBoxStackSegmentActions.ExtendSegmentsByAmount(_blockSegments, deltaNumberOfStacks);
                AdjustHeightForStackRangeInAllSegments(currentNumberOfStacksInSegments);
            }
            else
            ObjectPlacementBoxStackSegmentActions.ShrinkSegmentsByAmount(_blockSegments, Mathf.Abs(deltaNumberOfStacks));
        }

        private void AppendOrRemoveSegmentsToBlock(int deltaNumberOfSegments)
        {
            if (deltaNumberOfSegments > 0)
            {
                // New segments will be added, so the last segment must have its exclude corners hide flags cleared.
                if (CanExcludeCorners())
                {
                    LastSegment.GetStackByIndex(0).ClearHideFlagForAllBoxes(ObjectPlacementBoxHideFlags.BlockExcludeCorners);
                    LastSegment.GetStackByIndex(LastSegment.NumberOfStacks - 1).ClearHideFlagForAllBoxes(ObjectPlacementBoxHideFlags.BlockExcludeCorners);
                }

                int currentNumberOfSegmentsInBlock = _blockSegments.Count;
                AppendSegments(deltaNumberOfSegments);
                AdjustHeightForSegmentRange(currentNumberOfSegmentsInBlock);
            }
            else
            {
                int absDelta = Mathf.Abs(deltaNumberOfSegments);
                if (absDelta == _blockSegments.Count) --absDelta;   // We always want at least one segment
                RemoveLastNumberOfSegments(absDelta);
            }
        }

        private void AdjustCornerExclusionHideFlagsForEntireBlock()
        {
            ClearCornerExclusionHideFlagsInFirstAndLastSegments();
            if (CanExcludeCorners())
            {
                AdjustCornerExlcusionHideFlagsInSegment(FirstSegment);
                AdjustCornerExlcusionHideFlagsInSegment(LastSegment);
            }
        }

        private void ClearCornerExclusionHideFlagsInFirstAndLastSegments()
        {
            FirstSegment.ClearHideFlagInAllStacks(ObjectPlacementBoxHideFlags.BlockExcludeCorners);
            LastSegment.ClearHideFlagInAllStacks(ObjectPlacementBoxHideFlags.BlockExcludeCorners);
        }

        private void AdjustCornerExlcusionHideFlagsInSegment(ObjectPlacementBoxStackSegment segment)
        {
            segment.GetStackByIndex(0).SetHideFlagForAllBoxes(ObjectPlacementBoxHideFlags.BlockExcludeCorners);
            segment.GetStackByIndex(LastSegment.NumberOfStacks - 1).SetHideFlagForAllBoxes(ObjectPlacementBoxHideFlags.BlockExcludeCorners);
        }

        private void RemoveLastNumberOfSegments(int numberOfSegmentsToRemove)
        {
            int removedSegments = 0;
            while(removedSegments < numberOfSegmentsToRemove)
            {
                _blockSegments.RemoveAt(_blockSegments.Count - 1);
                ++removedSegments;
            }
        }

        private void AppendSegments(int numberOfSegmentsToAppend)
        {
            for(int segmentIndex = 0; segmentIndex < numberOfSegmentsToAppend; ++segmentIndex)
            {
                ObjectPlacementBoxStackSegment lastSegmentInBlock = LastSegment;
                ObjectPlacementBoxStackSegment segmentToAppend = CreateNewSegment();
                segmentToAppend.SetExtensionDirection(_segmentExtensionDirection);
                segmentToAppend.Extend(FirstSegment.NumberOfStacks);

                AppendSegmentToSegment(segmentToAppend, lastSegmentInBlock);
            }
        }

        private void AppendSegmentToSegment(ObjectPlacementBoxStackSegment sourceSegment, ObjectPlacementBoxStackSegment destinationSegment)
        {
            sourceSegment.ConnectFirstStackToFirstStackInSegment(destinationSegment, GetSegmentConnectionOffsetForAppendOperation());
        }

        private Vector3 GetSegmentConnectionOffsetForAppendOperation()
        {
            return _segmentCollectionExtensionDirection * (FirstSegment.GetBoxSizeAlongNormalizedDirection(_segmentCollectionExtensionDirection) + _paddingSettings.PaddingAlongExtensionPlane);
        }

        private void AdjustHeightForStackRangeInAllSegments(int indexOfFirstStackToAdjust)
        {
            ObjectPlacementBlockHeightAdjustmentMode heightAdjustmentMode = _heightAdjustmentSettings.HeightAdjustmentMode;
            if (heightAdjustmentMode == ObjectPlacementBlockHeightAdjustmentMode.Manual) _manualHeightAdjuster.AdjustHeightForSegments(_blockSegments, indexOfFirstStackToAdjust, _currentManualBlockHeight);
            else _automaticRandomHeightAdjuster.AdjustHeightForSegments(_blockSegments, 0, indexOfFirstStackToAdjust, _automaticRandomHeightAdjustmentSettings);
        }

        private void AdjustHeightForSegmentRange(int indexOfFirstSegmentToAdjust)
        {
            ObjectPlacementBlockHeightAdjustmentMode heightAdjustmentMode = _heightAdjustmentSettings.HeightAdjustmentMode;
            if (heightAdjustmentMode == ObjectPlacementBlockHeightAdjustmentMode.Manual) _manualHeightAdjuster.AdjustHeightForSegments(_blockSegments, _currentManualBlockHeight);
            else _automaticRandomHeightAdjuster.AdjustHeightForSegments(_blockSegments, indexOfFirstSegmentToAdjust, 0, _automaticRandomHeightAdjustmentSettings);
        }
        #endregion
    }
}
#endif