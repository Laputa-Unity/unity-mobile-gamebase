#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class BoxFaces
    {
        #region Private Static Variables
        private static readonly BoxFace[] _faces;
        private static readonly Vector3[] _faceNormals;
        private static readonly Vector3[] _faceRightAxes;
        private static readonly Vector3[] _faceLookAxes;
        private static readonly int _count;
        #endregion

        #region Constructors
        static BoxFaces()
        {
            _count = Enum.GetValues(typeof(BoxFace)).Length;

            _faces = new BoxFace[_count];
            _faces[(int)BoxFace.Back] = BoxFace.Back;
            _faces[(int)BoxFace.Front] = BoxFace.Front;
            _faces[(int)BoxFace.Left] = BoxFace.Left;
            _faces[(int)BoxFace.Right] = BoxFace.Right;
            _faces[(int)BoxFace.Top] = BoxFace.Top;
            _faces[(int)BoxFace.Bottom] = BoxFace.Bottom;

            _faceNormals = new Vector3[_count];
            _faceNormals[(int)BoxFace.Back] = Vector3.forward;
            _faceNormals[(int)BoxFace.Front] = Vector3.back;
            _faceNormals[(int)BoxFace.Left] = Vector3.left;
            _faceNormals[(int)BoxFace.Right] = Vector3.right;
            _faceNormals[(int)BoxFace.Top] = Vector3.up;
            _faceNormals[(int)BoxFace.Bottom] = Vector3.down;

            _faceRightAxes = new Vector3[_count];
            _faceRightAxes[(int)BoxFace.Back] = Vector3.left;
            _faceRightAxes[(int)BoxFace.Front] = Vector3.right;
            _faceRightAxes[(int)BoxFace.Left] = Vector3.back;
            _faceRightAxes[(int)BoxFace.Right] = Vector3.forward;
            _faceRightAxes[(int)BoxFace.Top] = Vector3.right;
            _faceRightAxes[(int)BoxFace.Bottom] = Vector3.right;

            _faceLookAxes = new Vector3[_count];
            _faceLookAxes[(int)BoxFace.Back] = Vector3.up;
            _faceLookAxes[(int)BoxFace.Front] = Vector3.up;
            _faceLookAxes[(int)BoxFace.Left] = Vector3.up;
            _faceLookAxes[(int)BoxFace.Right] = Vector3.up;
            _faceLookAxes[(int)BoxFace.Top] = Vector3.forward;
            _faceLookAxes[(int)BoxFace.Bottom] = Vector3.back;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<BoxFace> GetAll()
        {
            return new List<BoxFace>(_faces);
        }

        public static BoxFace GetNext(BoxFace boxFace)
        {
            return (BoxFace)(((int)boxFace + 1) % _count);
        }

        public static List<Vector3> GetAllFaceNormals()
        {
            return new List<Vector3>(_faceNormals);
        }

        public static List<Vector3> GetAllFaceRightAxes()
        {
            return new List<Vector3>(_faceRightAxes);
        }

        public static List<Vector3> GetAllFaceLookAxes()
        {
            return new List<Vector3>(_faceLookAxes);
        }

        public static Vector3 GetFaceNormal(BoxFace boxFace)
        {
            return _faceNormals[(int)boxFace];
        }

        public static Vector3 GetFaceRightAxis(BoxFace boxFace)
        {
            return _faceRightAxes[(int)boxFace];
        }

        public static Vector3 GetFaceLookAxis(BoxFace boxFace)
        {
            return _faceLookAxes[(int)boxFace];
        }

        public static BoxFace GetOpposite(BoxFace boxFace)
        {
            if (boxFace == BoxFace.Back) return BoxFace.Front;
            if (boxFace == BoxFace.Front) return BoxFace.Back;

            if (boxFace == BoxFace.Left) return BoxFace.Right;
            if (boxFace == BoxFace.Right) return BoxFace.Left;

            if (boxFace == BoxFace.Bottom) return BoxFace.Top;
            return BoxFace.Bottom;
        }
        #endregion
    }
}
#endif