#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class PathNoTileConnectionsObjectPlacementDataCaculator
    {
        #region Private Variables
        private ObjectPlacementPath _path;
        #endregion

        #region Public Properties
        public ObjectPlacementPath Path { get { return _path; } set { _path = value; } }
        #endregion

        #region Public Methods
        public List<ObjectPlacementData> Calculate()
        {
            if (_path == null || _path.NumberOfSegments == 0 || !ObjectPlacementGuide.ExistsInScene) return new List<ObjectPlacementData>();

            Prefab placementGuidePrefab = ObjectPlacementGuide.Instance.SourcePrefab;
            Vector3 placementGuideWorldScale = ObjectPlacementGuide.Instance.WorldScale;
            float objectMissChance = _path.Settings.ManualConstructionSettings.ObjectMissChance;
            Vector3 yOffsetVector = _path.ExtensionPlane.normal * _path.Settings.ManualConstructionSettings.OffsetAlongGrowDirection;
            bool allowObjectIntersection = ObjectPlacementSettings.Get().ObjectIntersectionSettings.AllowIntersectionForPathPlacement;
            Quaternion placementGuideRotation = ObjectPlacementGuide.Instance.WorldRotation;

            bool rotateObjectsToFollowPath = _path.Settings.ManualConstructionSettings.RotateObjectsToFollowPath;
            Vector3 pathExtensionPlaneNormal = _path.ExtensionPlane.normal;
            Vector3 firstSegmentExtensionDir = _path.GetSegmentByIndex(0).ExtensionDirection;

            Quaternion firstSegmentRotation = Quaternion.LookRotation(firstSegmentExtensionDir, pathExtensionPlaneNormal);

            bool randomizePrefabs = _path.Settings.ManualConstructionSettings.RandomizePrefabs;
            PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;

            var objectPlacementDataInstances = new List<ObjectPlacementData>(_path.NumberOfSegments * 10);
            for (int segmentIndex = 0; segmentIndex < _path.NumberOfSegments; ++segmentIndex)
            {
                ObjectPlacementBoxStackSegment segment = _path.GetSegmentByIndex(segmentIndex);

                Quaternion worldRotation = placementGuideRotation;
              
                if(rotateObjectsToFollowPath)
                {
                    // Note: ObjectPlacementPathManualConstructionSession.cs line 451. The design is a wreck. AGAIN :)
                    if ((segmentIndex == 0 && _path.NumberOfSegments > 1 && segment.NumberOfStacks == 1))
                    {
                        Vector3 dirBetweenStacks = segment.GetStackByIndex(0).BasePosition - _path.GetSegmentByIndex(1).GetStackByIndex(0).BasePosition;
                        dirBetweenStacks.Normalize();

                        Quaternion segmentRotation = Quaternion.LookRotation(dirBetweenStacks, pathExtensionPlaneNormal);
                        Quaternion fromPlacementGuideRotationToThis = QuaternionExtensions.GetRelativeRotation(placementGuideRotation, segmentRotation);
                        worldRotation = fromPlacementGuideRotationToThis * worldRotation;
                    }
                    else
                    if (segmentIndex != 0)
                    {
                        Quaternion segmentRotation = Quaternion.LookRotation(segment.ExtensionDirection, pathExtensionPlaneNormal);
                        Quaternion fromFirstToThis = QuaternionExtensions.GetRelativeRotation(firstSegmentRotation, segmentRotation);
                        worldRotation = fromFirstToThis * worldRotation;
                    }

                }
                for (int stackIndex = 0; stackIndex < segment.NumberOfStacks; ++stackIndex)
                {
                    ObjectPlacementBoxStack stack = segment.GetStackByIndex(stackIndex);
                    if (stack.IsOverlappedByAnotherStack) continue;

                    for(int stackBoxIndex = 0; stackBoxIndex < stack.NumberOfBoxes; ++stackBoxIndex)
                    {
                        ObjectPlacementBox box = stack.GetBoxByIndex(stackBoxIndex);
                        if (box.IsHidden) continue;

                        if (ObjectPlacementMissChance.Missed(objectMissChance, ObjectPlacementPathManualConstructionSettings.MinObjectMissChance, ObjectPlacementPathManualConstructionSettings.MaxObjectMissChance)) continue;
                        if (!allowObjectIntersection && ObjectQueries.IntersectsAnyObjectsInScene(box.OrientedBox, true)) continue;

                        Vector3 worldScale = placementGuideWorldScale;
                        Prefab prefab = placementGuidePrefab;
                        if (randomizePrefabs && activePrefabCategory.NumberOfPrefabs != 0)
                        {
                            int randomPrefabIndex = UnityEngine.Random.Range(0, activePrefabCategory.NumberOfPrefabs);
                            Prefab randomPrefab = activePrefabCategory.GetPrefabByIndex(randomPrefabIndex);
                            if (randomPrefab != null && randomPrefab.UnityPrefab != null)
                            {
                                prefab = activePrefabCategory.GetPrefabByIndex(randomPrefabIndex);
                                worldScale = prefab.UnityPrefab.transform.lossyScale;
                            }
                        }

                        var objectPlacementData = new ObjectPlacementData();
                        objectPlacementData.WorldPosition = ObjectPositionCalculator.CalculateObjectHierarchyPosition(prefab, box.Center + yOffsetVector, worldScale, worldRotation);
                        objectPlacementData.WorldScale = worldScale;
                        objectPlacementData.WorldRotation = worldRotation;
                        objectPlacementData.Prefab = prefab;
                        objectPlacementDataInstances.Add(objectPlacementData);
                    }
                }
            }

            return objectPlacementDataInstances;
        }
        #endregion
    }
}
#endif