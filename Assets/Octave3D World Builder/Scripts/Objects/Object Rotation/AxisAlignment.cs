#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class AxisAlignment
    {
        #region Public Static Functions
        public static void AlignObjectAxis(GameObject gameObject, CoordinateSystemAxis alignmentAxis, Vector3 destinationAxis)
        {
            Transform objectTransform = gameObject.transform;
            objectTransform.rotation = CalculateRotationQuaternionForAxisAlignment(objectTransform.rotation, alignmentAxis, destinationAxis);
        }

        public static void AlignObjectAxis(GameObject gameObject, AxisAlignmentSettings adjustmentSettings, Vector3 destinationAxis)
        {
            if (adjustmentSettings.IsEnabled)
            {
                Transform objectTransform = gameObject.transform;
                objectTransform.rotation = CalculateRotationQuaternionForAxisAlignment(objectTransform.rotation, adjustmentSettings.AlignmentAxis, destinationAxis);
            }
        }

        public static Quaternion CalculateRotationQuaternionForAxisAlignment(Quaternion currentRotation, CoordinateSystemAxis alignmentAxis, Vector3 destinationAxis)
        {
            TransformMatrix rotationMatrix = TransformMatrix.GetIdentity();
            rotationMatrix.Rotation = currentRotation;

            Vector3 axisToAlign = CoordinateSystemAxes.GetLocalVector(alignmentAxis, rotationMatrix);
            bool isAlignmentAxisNegative = CoordinateSystemAxes.IsNegativeAxis(alignmentAxis);

            // Already aligned?
            bool pointsInSameDirection;
            bool isAlignedWithDestinationAxis = destinationAxis.IsAlignedWith(axisToAlign, out pointsInSameDirection);
            if (isAlignedWithDestinationAxis && pointsInSameDirection) return currentRotation;

            if (!isAlignedWithDestinationAxis)
            {
                Vector3 rotationAxis = Vector3.Cross(axisToAlign, destinationAxis);
                rotationAxis.Normalize();
                return Quaternion.AngleAxis(axisToAlign.AngleWith(destinationAxis), rotationAxis) * currentRotation;
            }
            else
            {
                // If this point is reached, it means the axis is aligned with the destination axis but points in the opposite
                // direction. In this case we can not use the cross product, so we will have to regenerate the axes.
                Vector3 newRightAxis, newUpAxis, newLookAxis;
                if (alignmentAxis == CoordinateSystemAxis.PositiveRight || alignmentAxis == CoordinateSystemAxis.NegativeRight)
                {
                    newRightAxis = destinationAxis;
                    if (isAlignmentAxisNegative) newRightAxis *= -1.0f;

                    bool worldUpPointsSameDirAsRightAxis;
                    if (newRightAxis.IsAlignedWith(Vector3.up, out worldUpPointsSameDirAsRightAxis)) newLookAxis = worldUpPointsSameDirAsRightAxis ? Vector3.forward : Vector3.back;
                    else newLookAxis = Vector3.Cross(newRightAxis, Vector3.up);

                    if (isAlignmentAxisNegative) newLookAxis *= -1.0f;
                    newUpAxis = Vector3.Cross(newLookAxis, newRightAxis);
                }
                if (alignmentAxis == CoordinateSystemAxis.PositiveUp || alignmentAxis == CoordinateSystemAxis.NegativeUp)
                {
                    newUpAxis = destinationAxis;
                    if (isAlignmentAxisNegative) newUpAxis *= -1.0f;

                    bool worldUpPointsSameDirAsUpAxis;
                    if (newUpAxis.IsAlignedWith(Vector3.up, out worldUpPointsSameDirAsUpAxis)) newLookAxis = worldUpPointsSameDirAsUpAxis ? Vector3.forward : Vector3.back;
                    else newLookAxis = Vector3.Cross(newUpAxis, Vector3.up);

                    if (isAlignmentAxisNegative) newLookAxis *= -1.0f;
                }
                else
                {
                    newLookAxis = destinationAxis;
                    if (isAlignmentAxisNegative) newLookAxis *= -1.0f;

                    bool worldUpPointsSameDirAsLookAxis;
                    if (newLookAxis.IsAlignedWith(Vector3.up, out worldUpPointsSameDirAsLookAxis)) newUpAxis = worldUpPointsSameDirAsLookAxis ? Vector3.forward : Vector3.back;
                    else newUpAxis = Vector3.Cross(newLookAxis, Vector3.up);

                    if (isAlignmentAxisNegative) newUpAxis *= -1.0f;
                }

                // Normalize the axes which were calculated
                newUpAxis.Normalize();
                newLookAxis.Normalize();

                // Now use the axes to calculate the rotation quaternion
                return Quaternion.LookRotation(newLookAxis, newUpAxis);
            }
        }
        #endregion
    }
}
#endif