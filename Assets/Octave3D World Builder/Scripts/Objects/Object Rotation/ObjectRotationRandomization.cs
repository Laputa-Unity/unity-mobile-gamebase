#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectRotationRandomization
    {
        #region Public Static Functions
        public static void Randomize(GameObject gameObject, ObjectRotationRandomizationSettings randomizationSettings)
        {
            if(randomizationSettings.RandomizeRotation)
            {
                Transform objectTransform = gameObject.transform;
                if (randomizationSettings.XAxisRandomizationSettings.Randomize) RandomizeRotationForAxis(objectTransform, randomizationSettings.XAxisRandomizationSettings.Axis, randomizationSettings.XAxisRandomizationSettings.RandomizationModeSettings);
                if (randomizationSettings.YAxisRandomizationSettings.Randomize) RandomizeRotationForAxis(objectTransform, randomizationSettings.YAxisRandomizationSettings.Axis, randomizationSettings.YAxisRandomizationSettings.RandomizationModeSettings);
                if (randomizationSettings.ZAxisRandomizationSettings.Randomize) RandomizeRotationForAxis(objectTransform, randomizationSettings.ZAxisRandomizationSettings.Axis, randomizationSettings.ZAxisRandomizationSettings.RandomizationModeSettings);
                if (randomizationSettings.CustomAxisRandomizationSettings.Randomize) RandomizeRotationForAxis(objectTransform, randomizationSettings.CustomAxisRandomizationSettings.Axis, randomizationSettings.CustomAxisRandomizationSettings.RandomizationModeSettings);
            }
        }

        public static Quaternion GenerateRandomRotationQuaternion(ObjectRotationRandomizationSettings randomizationSettings)
        {
            Quaternion randomRotationQuaternion = Quaternion.identity;
            if (randomizationSettings.RandomizeRotation)
            {
                if (randomizationSettings.XAxisRandomizationSettings.Randomize) randomRotationQuaternion = GenerateRandomRotationQuaternionForAxis(Vector3.right, randomizationSettings.XAxisRandomizationSettings.RandomizationModeSettings) * randomRotationQuaternion;
                if (randomizationSettings.YAxisRandomizationSettings.Randomize) randomRotationQuaternion = GenerateRandomRotationQuaternionForAxis(Vector3.up, randomizationSettings.YAxisRandomizationSettings.RandomizationModeSettings) * randomRotationQuaternion;
                if (randomizationSettings.ZAxisRandomizationSettings.Randomize) randomRotationQuaternion = GenerateRandomRotationQuaternionForAxis(Vector3.forward, randomizationSettings.ZAxisRandomizationSettings.RandomizationModeSettings) * randomRotationQuaternion;
                if (randomizationSettings.CustomAxisRandomizationSettings.Randomize) randomRotationQuaternion = GenerateRandomRotationQuaternionForAxis(randomizationSettings.CustomAxisRandomizationSettings.Axis, randomizationSettings.CustomAxisRandomizationSettings.RandomizationModeSettings) * randomRotationQuaternion;
            }

            return randomRotationQuaternion;
        }
        #endregion

        #region Private Static Functions
        private static void RandomizeRotationForAxis(Transform objectTransform, TransformAxis axis, AxisRotationRandomizationModeSettings rotationRandomizationModeSettings)
        {
            Vector3 axisVector = TransformAxes.GetVector(axis, TransformSpace.Global, objectTransform);
            RandomizeRotationForAxis(objectTransform, axisVector, rotationRandomizationModeSettings);
        }

        private static void RandomizeRotationForAxis(Transform objectTransform, Vector3 axis, AxisRotationRandomizationModeSettings rotationRandomizationModeSettings)
        {
            if (rotationRandomizationModeSettings.RandomizationMode == AxisRotationRandomizationMode.RandomRotationStep)
                RandomizeRotationForAxis(objectTransform, axis, rotationRandomizationModeSettings.RandomRotationStepAxisRandomizationSettings.StepSizeInDegrees);
            else
            if (rotationRandomizationModeSettings.RandomizationMode == AxisRotationRandomizationMode.RandomRotationValue)
            {
                RandomizeTransformRotationForAxis(objectTransform, axis,
                                            rotationRandomizationModeSettings.RandomRotationValueAxisRandomizationSettings.MinRotationInDegrees,
                                            rotationRandomizationModeSettings.RandomRotationValueAxisRandomizationSettings.MaxRotationInDegrees);
            }
        }

        private static Quaternion GenerateRandomRotationQuaternionForAxis(Vector3 axis, AxisRotationRandomizationModeSettings rotationRandomizationModeSettings)
        {
            if (rotationRandomizationModeSettings.RandomizationMode == AxisRotationRandomizationMode.RandomRotationStep)
                return GenerateRandomRotationQuaternionForAxis(axis, rotationRandomizationModeSettings.RandomRotationStepAxisRandomizationSettings.StepSizeInDegrees);
            else
            if (rotationRandomizationModeSettings.RandomizationMode == AxisRotationRandomizationMode.RandomRotationValue)
            {
                return GenerateRandomRotationQuaternionForAxis(axis,
                                                                rotationRandomizationModeSettings.RandomRotationValueAxisRandomizationSettings.MinRotationInDegrees,
                                                                rotationRandomizationModeSettings.RandomRotationValueAxisRandomizationSettings.MaxRotationInDegrees);
            }
            else return Quaternion.identity;
        }

        private static void RandomizeRotationForAxis(Transform objectTransform, Vector3 axis, float stepSizeInDegrees)
        {
            int numberOfStepsInFullRotation = (int)(360.0f / stepSizeInDegrees + 0.5f);

            int randomMultiple = UnityEngine.Random.Range(0, numberOfStepsInFullRotation + 1);
            float randomRotation = randomMultiple * stepSizeInDegrees;

            objectTransform.Rotate(axis, randomRotation, Space.World);
        }

        private static Quaternion GenerateRandomRotationQuaternionForAxis(Vector3 axis, float stepSizeInDegrees)
        {
            int numberOfStepsInFullRotation = (int)(360.0f / stepSizeInDegrees + 0.5f);

            int randomMultiple = UnityEngine.Random.Range(0, numberOfStepsInFullRotation + 1);
            float randomRotation = randomMultiple * stepSizeInDegrees;

            return Quaternion.AngleAxis(randomRotation, axis);
        }

        private static void RandomizeTransformRotationForAxis(Transform objectTransform, Vector3 axis, float minRotationValueInDegrees, float maxRotationValueInDegrees)
        {
            float randomRotation = UnityEngine.Random.Range(minRotationValueInDegrees, maxRotationValueInDegrees);
            objectTransform.Rotate(axis, randomRotation, Space.World);
        }

        private static Quaternion GenerateRandomRotationQuaternionForAxis(Vector3 axis, float minRotationValueInDegrees, float maxRotationValueInDegrees)
        {
            float randomRotation = UnityEngine.Random.Range(minRotationValueInDegrees, maxRotationValueInDegrees);
            return Quaternion.AngleAxis(randomRotation, axis);
        }
        #endregion
    }
}
#endif