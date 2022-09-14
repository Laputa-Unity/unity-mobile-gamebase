#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class BlockObjectPlacementDataCalculator
    {
        #region Private Variables
        private ObjectPlacementBlock _block;
        #endregion

        #region Public Properties
        public ObjectPlacementBlock Block { get { return _block; } set { _block = value; } }
        #endregion

        #region Public Methods
        public List<ObjectPlacementData> Calculate()
        {
            if (_block == null || _block.NumberOfSegments == 0 || !ObjectPlacementGuide.ExistsInScene) return new List<ObjectPlacementData>();

            Prefab placementGuidePrefab = ObjectPlacementGuide.Instance.SourcePrefab;
            Vector3 placementGuideWorldScale = ObjectPlacementGuide.Instance.WorldScale;
            Quaternion placementGuideWorldRotation = ObjectPlacementGuide.Instance.WorldRotation;
            float objectMissChance = _block.Settings.ManualConstructionSettings.ObjectMissChance;
            ObjectRotationRandomizationSettings blockObjectRotationRandomizationSettings = _block.Settings.ManualConstructionSettings.ObjectRotationRandomizationSettings;
            bool randomizeRotations = blockObjectRotationRandomizationSettings.RandomizeRotation;
            Vector3 objectOffsetAlongExtensionPlaneNormal = _block.Settings.ManualConstructionSettings.OffsetAlongGrowDirection * _block.ExtensionPlane.normal;
            bool allowObjectIntersection = ObjectPlacementSettings.Get().ObjectIntersectionSettings.AllowIntersectionForBlockPlacement;
            bool randomizePrefabs = _block.Settings.ManualConstructionSettings.RandomizePrefabs;
            PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
            ObjectPlacementBlockProjectionSettings projectionSettings = _block.Settings.BlockProjectionSettings;
            bool canProject = projectionSettings.ProjectOnSurface &&
                        (projectionSettings.CanProjectOnMesh || projectionSettings.CanProjectOnTerrain);

            var objectPlacementDataInstances = new List<ObjectPlacementData>(_block.NumberOfSegments * 10);
            for (int segmentIndex = 0; segmentIndex < _block.NumberOfSegments; ++segmentIndex)
            {
                ObjectPlacementBoxStackSegment segment = _block.GetSegmentByIndex(segmentIndex);
                for (int stackIndex = 0; stackIndex < segment.NumberOfStacks; ++stackIndex)
                {
                    ObjectPlacementBoxStack stack = segment.GetStackByIndex(stackIndex);
                    if (stack.IsOverlappedByAnotherStack || stack.NumberOfBoxes == 0) continue;

                    Vector3 projectionOffset = Vector3.zero;
                    Vector3 projectionDirection = Vector3.zero;
                    Quaternion prjAlignRotation = Quaternion.identity;
                    GameObjectRayHit projectionSurfaceHit = null;

                    if (canProject)
                    {
                        Vector3 rayOrigin = stack.GetBoxByIndex(0).Center;
                        Vector3 rayDir = Vector3.zero;
                        
                        if (projectionSettings.ProjectionDirection == ObjectBlockProjectionDir.BlockUp) rayDir = _block.ExtensionPlane.normal;
                        else rayDir = -_block.ExtensionPlane.normal;

                        projectionDirection = rayDir;
            
                        Ray ray = new Ray(rayOrigin, rayDir);
                        GameObjectRayHit closestMeshHit = null;
                        GameObjectRayHit closestTerrainHit = null;

                        if (projectionSettings.CanProjectOnMesh) closestMeshHit = Octave3DScene.Get().RaycastAllMeshClosest(ray);
                        if (projectionSettings.CanProjectOnTerrain) closestTerrainHit = Octave3DScene.Get().RaycastAllTerainsClosest(ray);

                        // Ignore stack if no surface was found and non-projectables must be rejected
                        if (closestMeshHit == null && closestTerrainHit == null)
                        {
                            if (projectionSettings.RejectNonProjectables) continue;
                        }
                        else
                        {
                            projectionSurfaceHit = closestMeshHit;
                            if (closestMeshHit == null || (closestTerrainHit != null && closestMeshHit.HitEnter > closestTerrainHit.HitEnter)) projectionSurfaceHit = closestTerrainHit;
                        }

                        if (projectionSurfaceHit != null)
                        {
                            ObjectPlacementBox projectionBox = stack.GetBoxByIndex(0);
                            projectionOffset = projectionSurfaceHit.HitPoint - stack.GetBoxByIndex(0).Center;

                            if (projectionOffset.sqrMagnitude > (projectionSurfaceHit.HitPoint - stack.GetBoxByIndex(stack.NumberOfBoxes - 1).Center).sqrMagnitude)
                            {
                                projectionBox = stack.GetBoxByIndex(stack.NumberOfBoxes - 1);
                                projectionOffset = projectionSurfaceHit.HitPoint - projectionBox.Center;
                            }

                            if (!projectionSettings.AlignToSurfaceNormal)
                            {
                                var oobb = projectionBox.OrientedBox;
                                Vector3 oldCenter = oobb.Center;
                                GameObjectExtensions.EmbedObjectBoxInSurface(oobb, projectionDirection, projectionSurfaceHit.HitObject);
                                projectionOffset = oobb.Center - oldCenter;
                            }
                        }
                    }

                    for (int stackBoxIndex = 0; stackBoxIndex < stack.NumberOfBoxes; ++stackBoxIndex)
                    {
                        ObjectPlacementBox box = stack.GetBoxByIndex(stackBoxIndex);
                        if (box.IsHidden) continue;

                        if (ObjectPlacementMissChance.Missed(objectMissChance, ObjectPlacementBlockManualConstructionSettings.MinObjectMissChance, 
                            ObjectPlacementBlockManualConstructionSettings.MaxObjectMissChance)) continue;

                        if (!allowObjectIntersection && ObjectQueries.IntersectsAnyObjectsInScene(box.OrientedBox, true)) continue;

                        Quaternion worldRotation = placementGuideWorldRotation;
                        if (randomizeRotations) worldRotation = ObjectRotationRandomization.GenerateRandomRotationQuaternion(blockObjectRotationRandomizationSettings);

                        Vector3 worldScale = placementGuideWorldScale;
                        Prefab prefab = placementGuidePrefab;
                        if (randomizePrefabs && activePrefabCategory.NumberOfPrefabs != 0)
                        {
                            int randomPrefabIndex = UnityEngine.Random.Range(0, activePrefabCategory.NumberOfPrefabs);
                            Prefab randomPrefab = activePrefabCategory.GetPrefabByIndex(randomPrefabIndex);
                            if(randomPrefab != null && randomPrefab.UnityPrefab != null)
                            {
                                prefab = activePrefabCategory.GetPrefabByIndex(randomPrefabIndex);
                                worldScale = prefab.UnityPrefab.transform.lossyScale;
                            }
                        }

                        Vector3 boxCenter = box.Center + objectOffsetAlongExtensionPlaneNormal + projectionOffset;
                        if (projectionSurfaceHit != null)
                        {
                            if (projectionSettings.AlignToSurfaceNormal)
                            {
                                worldRotation = AxisAlignment.CalculateRotationQuaternionForAxisAlignment(worldRotation, projectionSettings.AlignmentAxis, projectionSurfaceHit.HitNormal);
                                OrientedBox prefabWorldOOBB = prefab.UnityPrefab.GetHierarchyWorldOrientedBox();
                                Vector3 oobbSize = prefabWorldOOBB.ScaledSize;
                                int axisIndex = (int)((int)(projectionSettings.AlignmentAxis) * 0.5f);
                                boxCenter = projectionSurfaceHit.HitPoint + projectionSurfaceHit.HitNormal * oobbSize[axisIndex] * 0.5f + (oobbSize[axisIndex] * stackBoxIndex * projectionSurfaceHit.HitNormal);
                            }
                        }
                        
                        var objectPlacementData = new ObjectPlacementData();
                        objectPlacementData.WorldPosition = ObjectPositionCalculator.CalculateObjectHierarchyPosition(prefab, boxCenter, worldScale, worldRotation);
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