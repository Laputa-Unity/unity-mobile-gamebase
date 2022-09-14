#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class Object2ObjectBoxSnapData
    {
        public enum SnapBoxID
        {
            Left = 0,
            Right,
            Bottom,
            Top,
            Back,
            Front,
        }
        private static SnapBoxID[] _snapBoxIDs;
        static Object2ObjectBoxSnapData()
        {
            _snapBoxIDs = new SnapBoxID[]
            {
                SnapBoxID.Left,
                SnapBoxID.Right,
                SnapBoxID.Bottom,
                SnapBoxID.Top,
                SnapBoxID.Back,
                SnapBoxID.Front
            };
        }

        private GameObject _meshObject;
        private List<Box> _modelSnapBoxes = new List<Box>();

        public GameObject MeshObject { get { return _meshObject; } }

        public static SnapBoxID[] GetAllSnapBoxIDs()
        {
            return _snapBoxIDs.Clone() as SnapBoxID[];
        }

        public static BoxFace[] GetBoxFaceToSnapBoxIDMap()
        {
            return new BoxFace[]
            {
                BoxFace.Left, BoxFace.Right, BoxFace.Bottom, BoxFace.Top, BoxFace.Back, BoxFace.Front
            };
        }

        public Object2ObjectBoxSnapData(GameObject meshObject, List<Box> modelSnapBoxes)
        {
            _meshObject = meshObject;
            _modelSnapBoxes = new List<Box>(modelSnapBoxes);
        }

        public List<Box> GetModelSnapBoxes()
        {
            return new List<Box>(_modelSnapBoxes);
        }

        public List<OrientedBox> GetWorldSnapBoxes()
        {
            if (_meshObject == null) return new List<OrientedBox>();

            Transform meshObjectTransform = _meshObject.transform;
            var worldSnapBoxes = new List<OrientedBox>(_modelSnapBoxes.Count);
            foreach(var modelBox in _modelSnapBoxes)
            {
                worldSnapBoxes.Add(new OrientedBox(modelBox, meshObjectTransform));
            }

            return worldSnapBoxes;
        }

        public Box GetModelSnapBox(SnapBoxID snapBox)
        {
            return _modelSnapBoxes[(int)snapBox];
        }
    }
}
#endif