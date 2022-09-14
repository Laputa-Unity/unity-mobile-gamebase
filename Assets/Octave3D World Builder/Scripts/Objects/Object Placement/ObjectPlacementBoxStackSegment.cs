#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementBoxStackSegment
    {
        #region Private Variables
        private Vector3 _firstStackBasePosition;
        private Vector3 _growAxis = Vector3.up;
        private Vector3 _extensionDirection = Vector3.forward;
        private Quaternion _stackRotation = Quaternion.identity;
        private float _paddingAlongStackGrowDirection = 0.0f;
        private float _paddingAlongExtensionDirection = 0.0f;

        private Vector3 _boxSize = Vector3.one;
        private List<ObjectPlacementBoxStack> _stacks = new List<ObjectPlacementBoxStack>();
        #endregion

        #region Public Static Properties
        public static float MinPadding { get { return 0.0f; } }
        #endregion

        #region Public Properties
        public int NumberOfStacks { get { return _stacks.Count; } }
        public Vector3 BoxSize { get { return _boxSize; } }
        public Vector3 GrowAxis { get { return _growAxis; } }
        public Quaternion StackRotation { get { return _stackRotation; } }
        public Quaternion Rotation { get { return Quaternion.LookRotation(_extensionDirection, _growAxis); } }
        public TransformMatrix RotationMatrix { get { return new TransformMatrix(Vector3.zero, Rotation, Vector3.one); } }
        public Vector3 ExtensionDirection { get { return _extensionDirection; } }
        public Vector3 FirstStackBasePosition { get { return _firstStackBasePosition; } }
        public Vector3 LastStackBasePosition { get { return NumberOfStacks == 0 ? FirstStackBasePosition : _stacks[NumberOfStacks - 1].BasePosition; } }
        #endregion

        #region Public Methods
        public List<OrientedBox> GetAllOrientedBoxes()
        {
            var allOrientedBoxes = new List<OrientedBox>();
            foreach (var stack in _stacks) allOrientedBoxes.AddRange(stack.GetAllOrientedBoxes());

            return allOrientedBoxes;
        }

        public List<ObjectPlacementBoxStack> GetAllStacks()
        {
            return new List<ObjectPlacementBoxStack>(_stacks);
        }

        public ObjectPlacementBoxStack GetStackByIndex(int stackIndex)
        {
            return _stacks[stackIndex];
        }

        public float GetBoxSizeAlongNormalizedDirection(Vector3 normalizedDirection)
        {
            if (NumberOfStacks == 0) return _boxSize.magnitude;
            else return GetStackByIndex(0).GetBoxSizeAlongNormalizedDirection(normalizedDirection);
        }

        public void SetBoxSizeForAllStacks(Vector3 boxSize)
        {
            _boxSize = boxSize;
            foreach (ObjectPlacementBoxStack stack in _stacks)
            {
                stack.SetBoxSize(boxSize);
            }
            RefreshStackBasePositions();
        }

        public void SetRotationForAllStacks(Quaternion rotation)
        {
            _stackRotation = rotation;
            foreach (ObjectPlacementBoxStack stack in _stacks)
            {
                stack.SetRotation(rotation);
            }
            RefreshStackBasePositions();
        }

        public void LookStacksAlongExtensionDirection()
        {
            _stackRotation = Quaternion.LookRotation(_extensionDirection, _growAxis);
            SetRotationForAllStacks(_stackRotation);
        }

        public void LookStacksAlongDirection(Vector3 direction)
        {
            direction.Normalize();

            _stackRotation = Quaternion.LookRotation(direction, _growAxis);
            SetRotationForAllStacks(_stackRotation);
        }

        public void SetFirstStackBasePosition(Vector3 firstStackBasePosition)
        {
            if(firstStackBasePosition != FirstStackBasePosition)
            {
                Vector3 stackMoveVector = firstStackBasePosition - _firstStackBasePosition;
                _firstStackBasePosition = firstStackBasePosition;
                MoveBasePositionsOfAllStacks(stackMoveVector);
            }
        }

        public void SetExtensionDirection(Vector3 extensionDirection)
        {
            if (extensionDirection.magnitude < 1e-5f || extensionDirection == _extensionDirection) return;

            _extensionDirection = extensionDirection;
            _extensionDirection.Normalize();      

            int numberOfStacks = NumberOfStacks;
            List<int> allStackHeightValues = GetHeightValuesOfAllStacks();

            RemoveAllStacks();
            Extend(numberOfStacks, allStackHeightValues);
        }

        public void SetGrowAxis(Vector3 growAxis)
        {
            if (growAxis.magnitude < 1e-5f || growAxis == _growAxis) return;

            _growAxis = growAxis;
            _growAxis.Normalize();

            int numberOfStacks = NumberOfStacks;
            List<int> allStackHeightValues = GetHeightValuesOfAllStacks();

            RemoveAllStacks();
            Extend(numberOfStacks, allStackHeightValues);
        }

        public void SetPaddingAlongStackGrowDirection(float paddingAlongStackDirection)
        {
            if (_paddingAlongStackGrowDirection == paddingAlongStackDirection) return;

            _paddingAlongStackGrowDirection = paddingAlongStackDirection;
            SetPaddingAlongGrowDirectionForAllStacks(_paddingAlongStackGrowDirection);
        }

        public void SetPaddingAlongExtensionDirection(float paddingAlongExtensionDirection)
        {
            if (_paddingAlongExtensionDirection == paddingAlongExtensionDirection) return;

            _paddingAlongExtensionDirection = paddingAlongExtensionDirection;
            RefreshStackBasePositions();
        }

        public void ConnectFirstStackToLastStackInSegment(ObjectPlacementBoxStackSegment destinationSegment, Vector3 connectionOffset)
        {
            Vector3 newFirstStackBasePos = destinationSegment.LastStackBasePosition + connectionOffset;
            SetFirstStackBasePosition(newFirstStackBasePos);
        }

        public void ConnectFirstStackToFirstStackInSegment(ObjectPlacementBoxStackSegment destinationSegment, Vector3 connectionOffset)
        {
            Vector3 newFirstStackBasePos = destinationSegment.FirstStackBasePosition + connectionOffset;
            SetFirstStackBasePosition(newFirstStackBasePos);
        }

        public void ReverseExtensionDirection()
        {
            SetExtensionDirection(-_extensionDirection);
        }

        public void SetHeightForAllStacks(int height)
        {
            for (int stackIndex = 0; stackIndex < NumberOfStacks; ++stackIndex)
            {
                _stacks[stackIndex].SetHeight(height);
            }
        }

        public void SetHeightForAllStacks(List<int> heightValues)
        {
            if (NumberOfStacks != heightValues.Count) return;

            for (int stackIndex = 0; stackIndex < NumberOfStacks; ++stackIndex)
            {
                _stacks[stackIndex].SetHeight(heightValues[stackIndex]);
            }
        }

        public void SetHeightForStacksStartingAt(int indexOfFirstStack, int heightValueForAllStacks)
        {
            for (int stackIndex = indexOfFirstStack; stackIndex < NumberOfStacks; ++stackIndex)
            {
                _stacks[stackIndex].SetHeight(heightValueForAllStacks);
            }
        }

        public void SetHeightForStacksStartingAt(int indexOfFirstStack, List<int> stackHeightValues)
        {
            int numberOfStacksToAdjust = NumberOfStacks - indexOfFirstStack;
            if (numberOfStacksToAdjust != stackHeightValues.Count) return;

            for (int stackIndex = indexOfFirstStack; stackIndex < NumberOfStacks; ++stackIndex)
            {
                _stacks[stackIndex].SetHeight(stackHeightValues[stackIndex - indexOfFirstStack]);
            }
        }

        public List<int> GetHeightValuesOfAllStacks()
        {
            var stackHeightValues = new List<int>(NumberOfStacks);
            for (int stackIndex = 0; stackIndex < NumberOfStacks; ++stackIndex)
            {
                stackHeightValues.Add(_stacks[stackIndex].Height);
            }

            return stackHeightValues;
        }

        public void MarkAllStacksAsNotOverlapped()
        {
            for (int stackIndex = 0; stackIndex < NumberOfStacks; ++stackIndex)
            {
                _stacks[stackIndex].IsOverlappedByAnotherStack = false;
            }
        }

        public void Extend(int amount)
        {
            if (amount <= 0) return;
            CreateNewStacksAlongExtensionDirection(amount);
        }

        public void Extend(int amount, List<int> stackHeightValues)
        {
            if (amount <= 0 || amount != stackHeightValues.Count) return;

            int indexOfFirstNewStack = NumberOfStacks;
            Extend(amount);
            SetHeightForStacksStartingAt(indexOfFirstNewStack, stackHeightValues);
        }

        public void Shrink(int amount)
        {
            if (amount <= 0) return;
            RemoveLastStacks(amount);
        }

        public void RemoveAllStacks()
        {
            _stacks.Clear();
        }

        public Plane GetBasePlane()
        {
            Vector3 pointOnPlane = FirstStackBasePosition - _growAxis * GetBoxSizeAlongNormalizedDirection(_growAxis) * 0.5f;
            return new Plane(_growAxis, pointOnPlane);
        }

        public bool ExtensionVectorIntersects(ObjectPlacementBoxStackSegment querySegment)
        {
            if (NumberOfStacks == 0 || querySegment.NumberOfStacks == 0) return false;

            if (_extensionDirection.IsPerpendicularTo(querySegment.ExtensionDirection))
            {
                Plane segmentBasePlane = GetBasePlane();
                Vector3 thisSegmentStart = segmentBasePlane.ProjectPoint(FirstStackBasePosition);
                Vector3 thisSegmentEnd = segmentBasePlane.ProjectPoint(LastStackBasePosition);
                if (NumberOfStacks == 1) thisSegmentEnd = thisSegmentStart + _extensionDirection * 0.5f * GetBoxSizeAlongNormalizedDirection(_extensionDirection);

                Vector3 querySegmentStart = segmentBasePlane.ProjectPoint(querySegment.FirstStackBasePosition);
                Vector3 querySegmentEnd = segmentBasePlane.ProjectPoint(querySegment.LastStackBasePosition);
                if (querySegment.NumberOfStacks == 1) querySegmentEnd = querySegmentStart + querySegment.ExtensionDirection * 0.5f * querySegment.GetBoxSizeAlongNormalizedDirection(querySegment.ExtensionDirection);

                Segment3D thisSegment3D = new Segment3D(thisSegmentStart, thisSegmentEnd);
                Segment3D querySegment3D = new Segment3D(querySegmentStart, querySegmentEnd);

                return thisSegment3D.IntersectsWith(querySegment3D);
            }

            return false;
        }

        public bool ExtensionVectorOverlaps(ObjectPlacementBoxStackSegment querySegment)
        {
            if (NumberOfStacks == 0 || querySegment.NumberOfStacks == 0) return false;

            if(_extensionDirection.IsAlignedWith(querySegment.ExtensionDirection))
            {
                Plane segmentBasePlane = GetBasePlane();
                Vector3 thisSegmentStart = segmentBasePlane.ProjectPoint(FirstStackBasePosition);
                Vector3 thisSegmentEnd = segmentBasePlane.ProjectPoint(LastStackBasePosition);
                if (NumberOfStacks == 1) thisSegmentEnd = thisSegmentStart + _extensionDirection * 0.5f * GetBoxSizeAlongNormalizedDirection(_extensionDirection);

                Vector3 querySegmentStart = segmentBasePlane.ProjectPoint(querySegment.FirstStackBasePosition);
                Vector3 querySegmentEnd = segmentBasePlane.ProjectPoint(querySegment.LastStackBasePosition);
                if (querySegment.NumberOfStacks == 1) querySegmentEnd = querySegmentStart + querySegment.ExtensionDirection * 0.5f * querySegment.GetBoxSizeAlongNormalizedDirection(querySegment.ExtensionDirection);

                return querySegmentStart.IsOnSegment(thisSegmentStart, thisSegmentEnd) || querySegmentEnd.IsOnSegment(thisSegmentStart, thisSegmentEnd) ||
                       thisSegmentStart.IsOnSegment(querySegmentStart, querySegmentEnd) || thisSegmentEnd.IsOnSegment(querySegmentStart, querySegmentEnd);
            }

            return false;
        }

        public void ClearHideFlagInAllStacks(ObjectPlacementBoxHideFlags hideFlag)
        {
            foreach(ObjectPlacementBoxStack stack in _stacks)
            {
                stack.ClearHideFlagForAllBoxes(hideFlag);
            }
        }

        public void SetHideFlagInAllStacks(ObjectPlacementBoxHideFlags hideFlag)
        {
            foreach (ObjectPlacementBoxStack stack in _stacks)
            {
                stack.SetHideFlagForAllBoxes(hideFlag);
            }
        }
        #endregion

        #region Private Methods
        private void RemoveLastStacks(int numberOfStacksToRemove)
        {
            numberOfStacksToRemove = Mathf.Min(NumberOfStacks, numberOfStacksToRemove);
            for(int stackIndex = 0; stackIndex < numberOfStacksToRemove; ++stackIndex)
            {
                _stacks.RemoveAt(NumberOfStacks - 1);
            }
        }

        private void SetHeightForAllStackss(int height)
        {
            foreach (ObjectPlacementBoxStack stack in _stacks)
            {
                stack.SetHeight(height);
            }
        }

        private void SetPaddingAlongGrowDirectionForAllStacks(float paddingAlongGrowDirection)
        {
            foreach (ObjectPlacementBoxStack stack in _stacks)
            {
                stack.SetPaddingAlongGrowDirection(paddingAlongGrowDirection);
            }
        }

        private void CreateNewStacksAlongExtensionDirection(int numberOfStacks)
        {
            for(int stackIndex = 0; stackIndex < numberOfStacks; ++stackIndex)
            {
                CreateNewStackAtEndOfSegment();
            }
        }

        private void CreateNewStackAtEndOfSegment()
        {
            var newStack = new ObjectPlacementBoxStack();
            newStack.SetRotation(_stackRotation);
            newStack.SetBoxSize(_boxSize);
            newStack.SetGrowDirection(_growAxis);
            newStack.SetBasePosition(CalculateStackBasePositionToSitAtEndOfSegment());
            newStack.SetPaddingAlongGrowDirection(_paddingAlongStackGrowDirection);
            newStack.GrowUpwards(1);
        
            _stacks.Add(newStack);
        }

        private void RefreshStackBasePositions()
        {
            for (int stackIndex = 1; stackIndex < NumberOfStacks; ++stackIndex)
            {
                _stacks[stackIndex].SetBasePosition(CalculateStackBasePositionToSitRightAfterStack(_stacks[stackIndex - 1]));
            }
        }

        private Vector3 CalculateStackBasePositionToSitAtEndOfSegment()
        {
            if (NumberOfStacks == 0) return FirstStackBasePosition;
            return CalculateStackBasePositionToSitRightAfterStack(_stacks[NumberOfStacks - 1]);
        }

        private Vector3 CalculateStackBasePositionToSitRightAfterStack(ObjectPlacementBoxStack afterStack)
        {
            return afterStack.BasePosition + _extensionDirection * (GetBoxSizeAlongNormalizedDirection(_extensionDirection) + _paddingAlongExtensionDirection);
        }

        private void MoveBasePositionsOfAllStacks(Vector3 moveVector)
        {
            foreach(ObjectPlacementBoxStack stack in _stacks)
            {
                stack.MoveBasePosition(moveVector);
            }
        }
        #endregion
    }
}
#endif