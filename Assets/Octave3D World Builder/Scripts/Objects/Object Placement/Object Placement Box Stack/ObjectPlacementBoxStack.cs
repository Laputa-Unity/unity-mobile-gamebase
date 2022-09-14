#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementBoxStack
    {
        #region Private Variables
        private Vector3 _growDirection;
        private bool _isGrowingDownwards = false;
        private Vector3 _basePosition = Vector3.zero;

        private Quaternion _rotation = Quaternion.identity;
        private TransformMatrix _rotationMatrix = TransformMatrix.GetIdentity();
        private float _paddingAlongGrowDirection = 0.0f;

        private ObjectPlacementBoxStackOverlapData _overlapData = new ObjectPlacementBoxStackOverlapData();

        private Vector3 _boxSize = Vector3.one;
        private List<ObjectPlacementBox> _boxes = new List<ObjectPlacementBox>();
        #endregion

        #region Public Properties
        public int NumberOfBoxes { get { return _boxes.Count; } }
        public Vector3 BasePosition { get { return _basePosition; } }
        public Quaternion Rotation { get { return _rotation; } }
        public bool IsOverlappedByAnotherStack { get { return _overlapData.IsOverlappedByAnotherStack; ; } set { _overlapData.IsOverlappedByAnotherStack = value; } }
        public TransformMatrix RotationMatrix { get { return _rotationMatrix; } }
        public Vector3 UpAxis { get { return _rotationMatrix.GetNormalizedUpAxis(); } }
        public Vector3 GrowDirection { get { return IsGrowingUpwards ? _growDirection : -_growDirection; } }
        public float PaddingAlongGrowDirection { get { return _paddingAlongGrowDirection; } }
        public bool IsGrowingUpwards { get { return !_isGrowingDownwards; } }
        public bool IsGrowingDownwards { get { return _isGrowingDownwards; } }
        public int Height { get { return NumberOfBoxes * (IsGrowingUpwards ? 1 : -1); } }
        public Vector3 BoxSize { get { return _boxSize; } }
        #endregion

        #region Constructors
        public ObjectPlacementBoxStack()
        {
            _growDirection = UpAxis;
        }
        #endregion

        #region Public Methods
        public List<OrientedBox> GetAllOrientedBoxes()
        {
            var allOrientedBoxes = new List<OrientedBox>();
            foreach (var box in _boxes) allOrientedBoxes.Add(box.OrientedBox);

            return allOrientedBoxes;
        }

        public ObjectPlacementBox GetBoxByIndex(int boxIndex)
        {
            return _boxes[boxIndex];
        }

        public Plane GetBasePlane()
        {
            Vector3 growDirection = GrowDirection;
            Vector3 pointOnPlane = BasePosition - growDirection * GetBoxSizeAlongNormalizedDirection(growDirection) * 0.5f;
            return new Plane(growDirection, pointOnPlane);
        }

        public float GetBoxSizeAlongNormalizedDirection(Vector3 direction)
        {
            Vector3 transformedBoxSizeVector = RotationMatrix.MultiplyVector(_boxSize);
            return direction.GetAbsDot(transformedBoxSizeVector);
        }

        public void PlaceOnPlane(Plane plane)
        {
            Vector3 upAxis = UpAxis;
            if (plane.normal.IsAlignedWith(upAxis))
            {
                Vector3 projectedBasePosition = plane.ProjectPoint(BasePosition);
                Vector3 desiredBasePosition = projectedBasePosition + upAxis * GetBoxSizeAlongNormalizedDirection(upAxis) * 0.5f;
                SetBasePosition(desiredBasePosition);
            }
        }

        // Note: This method will reset any hide flags assigned to the placement boxes.
        public void SetGrowDirection(Vector3 growDirection)
        {
            _growDirection = growDirection;

            int numberOfBoxes = NumberOfBoxes;
            RemoveAllBoxes();

            if (_isGrowingDownwards) GrowDownwards(numberOfBoxes);
            else GrowUpwards(numberOfBoxes);
        }

        public void SetBoxSize(Vector3 boxSize)
        {
            _boxSize = boxSize;
            if(NumberOfBoxes != 0)
            {
                for (int boxIndex = 0; boxIndex < NumberOfBoxes; ++boxIndex)
                {
                    ObjectPlacementBox box = _boxes[boxIndex];
                    box.ModelSpaceSize = boxSize;
                   
                    if(boxIndex > 0) box.Center = CalculateBoxCenterToSitRightAfterBox(_boxes[boxIndex - 1]);
                }
            }
        }

        public void SetRotation(Quaternion rotation)
        {
            Quaternion relativeRotation = _rotation.GetRelativeRotation(rotation);

            _growDirection = relativeRotation * _growDirection;  
            _rotation = rotation;
            _rotationMatrix = new TransformMatrix(Vector3.zero, _rotation, Vector3.one);

            foreach (ObjectPlacementBox box in _boxes)
            {
                Vector3 newBoxCenter = box.Center - _basePosition;
                newBoxCenter = relativeRotation * newBoxCenter;
                newBoxCenter += _basePosition;

                box.Center = newBoxCenter;
                box.Rotation = _rotation;
            }
        }

        // Note: This method will reset any hide flags assigned to the placement boxes.
        public void SetHeight(int height)
        {
            RemoveAllBoxes();
            if (height == 0) return;

            float boxSizeAlongGrow = GetBoxSizeAlongNormalizedDirection(GrowDirection);
            if (boxSizeAlongGrow < GetMinAllowedBoxSizeForGrow())
            {
                float sign = Mathf.Sign(height);
                height = (int)(1.0f * sign);
            }

            _isGrowingDownwards = height < 0;

            if (IsGrowingUpwards) GrowUpwards(height);
            else GrowDownwards(Mathf.Abs(height));
        }

        public void SetBasePosition(Vector3 basePosition)
        {
            Vector3 boxMoveOffset = basePosition - _basePosition;
            _basePosition = basePosition;

            OffsetBoxes(boxMoveOffset);
        }

        public void SetPaddingAlongGrowDirection(float paddingAlongGrowDirection)
        {
            if (paddingAlongGrowDirection == _paddingAlongGrowDirection) return;

            _paddingAlongGrowDirection = paddingAlongGrowDirection;
            for (int boxIndex = 1; boxIndex < NumberOfBoxes; ++boxIndex)
            {
                _boxes[boxIndex].Center = CalculateBoxCenterToSitRightAfterBox(_boxes[boxIndex - 1]);
            }
        }

        public void MoveBasePosition(Vector3 moveVector)
        {
            SetBasePosition(_basePosition + moveVector);
        }

        public void RemoveAllBoxes()
        {
            _boxes.Clear();
        }

        public void GrowUpwards(int amount)
        {
            if (amount <= 0) return;
            if (IsHeightRestrictedToOne() && Height == 1) return;
            
            if (!IsGrowingUpwards)
            {
                int numberOfRemovedBoxes = RemoveBoxesFromTopOfStack(amount);
                if (numberOfRemovedBoxes < amount)
                {
                    _isGrowingDownwards = false;

                    int remainingBoxes = amount - numberOfRemovedBoxes;
                    CreateNewBoxesOnTopOfStack(remainingBoxes);
                }
            }
            else CreateNewBoxesOnTopOfStack(amount);
        }

        public void GrowDownwards(int amount)
        {
            if (amount <= 0) return;
            if (IsHeightRestrictedToOne() && Height == 1) return;
            
            if (!IsGrowingDownwards)
            {
                int numberOfRemovedBoxes = RemoveBoxesFromTopOfStack(amount);
                if (numberOfRemovedBoxes < amount)
                {
                    _isGrowingDownwards = true;

                    int remainingBoxes = amount - numberOfRemovedBoxes;
                    CreateNewBoxesOnTopOfStack(remainingBoxes);
                }
            }
            else CreateNewBoxesOnTopOfStack(amount);
        }

        public Vector3 GetBasePositionConnectionVectorTo(ObjectPlacementBoxStack stack)
        {
            Plane basePlane = GetBasePlane();
            Vector3 projectedThisBasePos = basePlane.ProjectPoint(BasePosition);
            Vector3 projectedStackBasePos = basePlane.ProjectPoint(stack.BasePosition);

            return projectedStackBasePos - projectedThisBasePos;
        }

        public Vector3 GetNormalizedBasePositionConnectionVectorTo(ObjectPlacementBoxStack stack)
        {
            Vector3 connectionVector = GetBasePositionConnectionVectorTo(stack);
            connectionVector.Normalize();

            return connectionVector;
        }

        public void ClearHideFlagForAllBoxes(ObjectPlacementBoxHideFlags hideFlag)
        {
            foreach(ObjectPlacementBox box in _boxes)
            {
                box.ClearHideFlag(hideFlag);
            }
        }

        public void SetHideFlagForAllBoxes(ObjectPlacementBoxHideFlags hideFlag)
        {
            foreach (ObjectPlacementBox box in _boxes)
            {
                box.SetHideFlag(hideFlag);
            }
        }
        #endregion

        #region Private Methods
        private void OffsetBoxes(Vector3 offset)
        {
            foreach(ObjectPlacementBox box in _boxes)
            {
                box.Center = box.Center + offset;
            }
        }

        private void CreateNewBoxesOnTopOfStack(int numberOfBoxes)
        {
            for (int boxIndex = 0; boxIndex < numberOfBoxes; ++boxIndex)
            {
                CreateNewBoxOnTopOfStack();
            }
        }

        private void CreateNewBoxOnTopOfStack()
        {
            var newBox = new ObjectPlacementBox();
            newBox.Rotation = _rotation;
            newBox.ModelSpaceSize = _boxSize;
            newBox.Center = CalculateNewBoxCenterToSitOnTopOfStack();

            _boxes.Add(newBox);
        }

        private Vector3 CalculateNewBoxCenterToSitOnTopOfStack()
        {
            if (NumberOfBoxes == 0) return _basePosition;
            return CalculateBoxCenterToSitRightAfterBox(_boxes[NumberOfBoxes - 1]);
        }

        private Vector3 CalculateBoxCenterToSitRightAfterBox(ObjectPlacementBox afterBox)
        {
            return afterBox.Center + GrowDirection * (GetBoxSizeAlongNormalizedDirection(GrowDirection) + _paddingAlongGrowDirection);
        }

        private int RemoveBoxesFromTopOfStack(int numberOfBoxes)
        {
            int numberOfRemovedBoxes = 0;
            for (int boxIndex = 0; boxIndex < numberOfBoxes; ++boxIndex)
            {
                if (RemoveBoxFromTopOfStack()) ++numberOfRemovedBoxes;
            }

            return numberOfRemovedBoxes;
        }

        private bool RemoveBoxFromTopOfStack()
        {
            if (NumberOfBoxes > 0)
            {
                _boxes.RemoveAt(NumberOfBoxes - 1);
                return true;
            }

            return false;
        }

        private bool IsHeightRestrictedToOne()
        {
            return GetBoxSizeAlongNormalizedDirection(GrowDirection) >= GetMinAllowedBoxSizeForGrow();
        }

        private float GetMinAllowedBoxSizeForGrow()
        {
            return 1e-4f;
        }
        #endregion
    }
}
#endif