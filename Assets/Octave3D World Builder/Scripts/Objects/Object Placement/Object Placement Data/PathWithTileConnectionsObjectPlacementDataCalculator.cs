#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class PathWithTileConnectionsObjectPlacementDataCalculator
    {
        #region Private Variables
        private ObjectPlacementPath _path;
        private float _tileConnectionXZSize;
        private bool _allowObjectIntersection;
        #endregion

        #region Public Static Properties
        public static float MinTileConnectionXZSize { get { return 1e-5f; } }
        #endregion

        #region Public Properties
        public ObjectPlacementPath Path { get { return _path; } set { _path = value; } }
        public float TileConnectionXZSize { get { return _tileConnectionXZSize; } set { _tileConnectionXZSize = Mathf.Max(value, MinTileConnectionXZSize); } }
        #endregion

        #region Public Methods
        public List<ObjectPlacementData> Calculate()
        {
            if (!ValidatePlacementDataCalculationConditions()) return new List<ObjectPlacementData>();

            _allowObjectIntersection = ObjectPlacementSettings.Get().ObjectIntersectionSettings.AllowIntersectionForPathPlacement;
            List<ObjectPlacementBoxStackSegment> allPathSegments = _path.GetAllSegments();
            float objectMissChance = _path.Settings.ManualConstructionSettings.ObjectMissChance;
            bool usingSprites = _path.Settings.TileConnectionSettings.UsesSprites();

            var tileConnectionDetector = new ObjectPlacementPathTileConnectionDetector();
            var tileConnectionRotationCalculator = new ObjectPlacementPathTileConnectionRotationCalculator();

            List<ObjectPlacementPathTileConnectionGridCell> tileConnectionGridCells = tileConnectionDetector.Detect(_path, _tileConnectionXZSize);
            if (tileConnectionGridCells.Count == 0) return new List<ObjectPlacementData>();

            List<Prefab> tileConnectionPrefabsExceptAutofill = PathObjectPlacement.Get().PathSettings.TileConnectionSettings.GetAllTileConnectionPrefabs(true);
            List<Vector3> tileConnectionWorldScaleValuesExceptAutofill = CalculateWorldScaleForAllTileConnectionPrefabsExceptAutofill(PrefabQueries.GetTransformsForAllPrefabs(tileConnectionPrefabsExceptAutofill), PrefabQueries.GetHierarchyWorldOrientedBoxesForAllPrefabs(tileConnectionPrefabsExceptAutofill));
            List<Vector3> tileConnectionOffsetsExceptAutofill = CalculateOffsetsForAllTileConnectionsExceptAutofill();

            var objectPlacementDataInstances = new List<ObjectPlacementData>(allPathSegments.Count * 10);
            foreach (ObjectPlacementPathTileConnectionGridCell tileConnectionGridCell in tileConnectionGridCells)
            {
                ObjectPlacementPathTileConnectionType tileConnectionType = tileConnectionGridCell.TileConnectionType;
                ObjectPlacementBoxStack tileConnectionStack = tileConnectionGridCell.TileConnectionStack;
                if (tileConnectionStack.IsOverlappedByAnotherStack) continue;

                Prefab tileConnectionPrefab = tileConnectionPrefabsExceptAutofill[(int)tileConnectionType];
                Quaternion tileConnectionRotation = tileConnectionRotationCalculator.Calculate(tileConnectionGridCell);
                Vector3 tileConnectionWorldScale = tileConnectionWorldScaleValuesExceptAutofill[(int)tileConnectionType];
                Vector3 tileConnectionOffset = tileConnectionOffsetsExceptAutofill[(int)tileConnectionType];

                for (int stackBoxIndex = 0; stackBoxIndex < tileConnectionStack.NumberOfBoxes; ++stackBoxIndex)
                {
                    ObjectPlacementBox box = tileConnectionStack.GetBoxByIndex(stackBoxIndex);
                    if (box.IsHidden) continue;

                    if (ObjectPlacementMissChance.Missed(objectMissChance, ObjectPlacementPathManualConstructionSettings.MinObjectMissChance, ObjectPlacementPathManualConstructionSettings.MaxObjectMissChance)) continue;
                    if (!_allowObjectIntersection && ObjectQueries.IntersectsAnyObjectsInScene(box.OrientedBox, true)) continue;

                    var objectPlacementData = new ObjectPlacementData();
                    objectPlacementData.WorldPosition = ObjectPositionCalculator.CalculateObjectHierarchyPosition(tileConnectionPrefab, box.Center, tileConnectionWorldScale, tileConnectionRotation);
                    objectPlacementData.WorldPosition += tileConnectionOffset;
               
                    objectPlacementData.WorldScale = tileConnectionWorldScale;
                    objectPlacementData.WorldRotation = tileConnectionRotation;
                    objectPlacementData.Prefab = tileConnectionPrefab;
                    objectPlacementDataInstances.Add(objectPlacementData);
                }

                // Apply extrusion if necessary
                if (!usingSprites)
                {
                    List<OrientedBox> extrusionOrientedBoxes = ObjectPlacementPathTileConnectionExtrusion.GetTileConnectionExtrusionOrientedBoxes(tileConnectionGridCell);
                    foreach (OrientedBox extrusionBox in extrusionOrientedBoxes)
                    {
                        if (ObjectPlacementMissChance.Missed(objectMissChance, ObjectPlacementPathManualConstructionSettings.MinObjectMissChance, ObjectPlacementPathManualConstructionSettings.MaxObjectMissChance)) continue;
                        if (!_allowObjectIntersection && ObjectQueries.IntersectsAnyObjectsInScene(extrusionBox, true)) continue;

                        var objectPlacementData = new ObjectPlacementData();
                        objectPlacementData.WorldPosition = ObjectPositionCalculator.CalculateObjectHierarchyPosition(tileConnectionPrefab, extrusionBox.Center, tileConnectionWorldScale, tileConnectionRotation);
                        objectPlacementData.WorldScale = tileConnectionWorldScale;
                        objectPlacementData.WorldRotation = tileConnectionRotation;
                        objectPlacementData.Prefab = tileConnectionPrefab;
                        objectPlacementDataInstances.Add(objectPlacementData);
                    }
                }
            }

            ObjectPlacementPathTileConnectionSettings tileConnectionSettings = _path.Settings.TileConnectionSettings;
            if(tileConnectionSettings.DoesAutofillTileConnectionHavePrefabAssociated()) objectPlacementDataInstances.AddRange(GetPlacementDataForAutofillTiles(tileConnectionGridCells[0].ParentGrid));

            return objectPlacementDataInstances;
        }
        #endregion

        #region Private Methods
        private bool ValidatePlacementDataCalculationConditions()
        {
            if (_path == null || _path.NumberOfSegments == 0 || !ObjectPlacementGuide.ExistsInScene || _tileConnectionXZSize == 0.0f) return false;

            ObjectPlacementPathTileConnectionSettings tileConnectionSettings = PathObjectPlacement.Get().PathSettings.TileConnectionSettings;
            if (!tileConnectionSettings.DoAllTileConnectionsHavePrefabAssociated(true))
            {
                Debug.LogWarning("Not all tile connections have an associated prefab. Path placement was cancelled.");
                return false;
            }

            return true;
        }

        private List<Vector3> CalculateWorldScaleForAllTileConnectionPrefabsExceptAutofill(List<Transform> tileConnectionPrefabTransforms, List<OrientedBox> tileConnectionPrefabOrientedBoxes)
        {
            var tileConnectionScaleCalculator = new ObjectPlacementPathTileConnectionScaleCalculator();
            var tileConnectionWorldScale = new List<Vector3>(tileConnectionPrefabTransforms.Count);

            for(int tileConnectionIndex = 0; tileConnectionIndex < tileConnectionPrefabTransforms.Count; ++tileConnectionIndex)
            {
                tileConnectionWorldScale.Add(tileConnectionScaleCalculator.CalculateWorldScale(_tileConnectionXZSize, 
                                                                                               tileConnectionPrefabOrientedBoxes[tileConnectionIndex].ScaledSize,
                                                                                               tileConnectionPrefabTransforms[tileConnectionIndex], _path));
            }

            return tileConnectionWorldScale;
        }

        private List<Vector3> CalculateOffsetsForAllTileConnectionsExceptAutofill()
        {
            var offsets = new List<Vector3>(ObjectPlacementPathTileConnectionTypes.Count);
            ObjectPlacementPathTileConnectionSettings tileConnectionSettings = _path.Settings.TileConnectionSettings;

            var tileConnectionYOffsetVectorCalculator = new ObjectPlacementPathTileConnectionYOffsetVectorCalculator();
            List<ObjectPlacementPathTileConnectionType> allTileConnectionTypes = ObjectPlacementPathTileConnectionTypes.GetAllExceptAutofill();
            foreach(ObjectPlacementPathTileConnectionType tileConnectionType in allTileConnectionTypes)
            {
                offsets.Add(tileConnectionYOffsetVectorCalculator.Calculate(tileConnectionSettings.GetSettingsForTileConnectionType(tileConnectionType), _path));
            }

            return offsets;
        }

        private List<ObjectPlacementData> GetPlacementDataForAutofillTiles(ObjectPlacementPathTileConnectionGrid tileConnectionGrid)
        {
            Prefab autofillPrefab = _path.Settings.TileConnectionSettings.GetSettingsForTileConnectionType(ObjectPlacementPathTileConnectionType.Autofill).Prefab;
            OrientedBox autoFillPrefabWorldOrientedBox = autofillPrefab.UnityPrefab.GetHierarchyWorldOrientedBox();
            Vector3 autofillPrefabBoxSize = autoFillPrefabWorldOrientedBox.ScaledSize;
            Plane extensionPlane = _path.ExtensionPlane;
            float objectMissChance = _path.Settings.ManualConstructionSettings.ObjectMissChance;
            bool usingSprites = _path.Settings.TileConnectionSettings.UsesSprites();

            var tileConnectionScaleCalculator = new ObjectPlacementPathTileConnectionScaleCalculator();
            var tileConnectionYOffsetVectorCalculator = new ObjectPlacementPathTileConnectionYOffsetVectorCalculator();
            Vector3 worldScale = tileConnectionScaleCalculator.CalculateWorldScale(_tileConnectionXZSize, autofillPrefabBoxSize, autofillPrefab.UnityPrefab.transform, _path);
            Vector3 yOffsetVector = tileConnectionYOffsetVectorCalculator.Calculate(_path.Settings.TileConnectionSettings.GetSettingsForTileConnectionType(ObjectPlacementPathTileConnectionType.Autofill), _path);

            List<ObjectPlacementPathTileConnectionGridCell> autoFillTileConnectionGridCells = tileConnectionGrid.CreateAndReturnAutofillTileConnectionCells();
            if (autoFillTileConnectionGridCells.Count == 0) return new List<ObjectPlacementData>();

            // Note: All autofill tiles have the same rotation.
            Quaternion worldRotation = (new ObjectPlacementPathTileConnectionRotationCalculator()).Calculate(autoFillTileConnectionGridCells[0]);
            var objectPlacementDataInstances = new List<ObjectPlacementData>(autoFillTileConnectionGridCells.Count);
            foreach(ObjectPlacementPathTileConnectionGridCell autofillGridCell in autoFillTileConnectionGridCells)
            {
                if (ObjectPlacementMissChance.Missed(objectMissChance, ObjectPlacementPathManualConstructionSettings.MinObjectMissChance, ObjectPlacementPathManualConstructionSettings.MaxObjectMissChance)) continue;

                Vector3 cellPosition = tileConnectionGrid.CalculateCellPosition(autofillGridCell.CellIndices);
                Vector3 cellPositionOnPathExtensionPlane = extensionPlane.ProjectPoint(cellPosition);
                
                OrientedBox tileOrientedBox = new OrientedBox(autoFillPrefabWorldOrientedBox);
                tileOrientedBox.Center = cellPositionOnPathExtensionPlane + extensionPlane.normal * (usingSprites ? 0.0f : autofillPrefabBoxSize.y * 0.5f);
                tileOrientedBox.Rotation = worldRotation;
                tileOrientedBox.Scale = worldScale;
                if (!_allowObjectIntersection && ObjectQueries.IntersectsAnyObjectsInScene(tileOrientedBox, true)) continue;

                var objectPlacementData = new ObjectPlacementData();
                objectPlacementData.WorldPosition = ObjectPositionCalculator.CalculateObjectHierarchyPosition(autofillPrefab, tileOrientedBox.Center, worldScale, worldRotation) + yOffsetVector;
                objectPlacementData.WorldScale = worldScale;
                objectPlacementData.WorldRotation = worldRotation;
                objectPlacementData.Prefab = autofillPrefab;
                objectPlacementDataInstances.Add(objectPlacementData);

                // Apply extrusion if necessary
                if(!usingSprites)
                {
                    List<OrientedBox> extrusionOrientedBoxes = ObjectPlacementPathTileConnectionExtrusion.GetTileConnectionExtrusionOrientedBoxes(autofillGridCell);
                    foreach (OrientedBox extrusionBox in extrusionOrientedBoxes)
                    {
                        if (ObjectPlacementMissChance.Missed(objectMissChance, ObjectPlacementPathManualConstructionSettings.MinObjectMissChance, ObjectPlacementPathManualConstructionSettings.MaxObjectMissChance)) continue;
                        if (!_allowObjectIntersection && ObjectQueries.IntersectsAnyObjectsInScene(extrusionBox, true)) continue;

                        objectPlacementData = new ObjectPlacementData();
                        objectPlacementData.WorldPosition = ObjectPositionCalculator.CalculateObjectHierarchyPosition(autofillPrefab, extrusionBox.Center, worldScale, worldRotation);
                        objectPlacementData.WorldScale = worldScale;
                        objectPlacementData.WorldRotation = worldRotation;
                        objectPlacementData.Prefab = autofillPrefab;
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