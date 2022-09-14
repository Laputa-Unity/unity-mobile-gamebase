#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    //[Serializable]
    public class Octave3DMeshDatabase
    {
        #region Private Variables
        //[SerializeField]
        private SerializableOctave3DMeshDictionary _meshes = new SerializableOctave3DMeshDictionary();
        #endregion

        #region Public Static Functions
        public static Octave3DMeshDatabase Get()
        {
            return Octave3DScene.Get().Octave3DMeshDatabase;
        }
        #endregion

        #region Constructors
        public Octave3DMeshDatabase()
        {
            EditorApplication.projectChanged -= RemoveNullMeshEntries;
            EditorApplication.projectChanged += RemoveNullMeshEntries;
        }
        #endregion

        #region Public Methods
        public Octave3DMesh GetOctave3DMesh(Mesh mesh)
        {
            if (mesh == null) return null;

            if (!Contains(mesh)) _meshes.Dictionary.Add(mesh, new Octave3DMesh(mesh));
            return _meshes.Dictionary[mesh];
        }

        public bool Contains(Mesh mesh)
        {
            return mesh != null && _meshes.Dictionary.ContainsKey(mesh);
        }
        #endregion

        #region Private Methods
        private void RemoveNullMeshEntries()
        {
            var newMeshDictionary = new SerializableOctave3DMeshDictionary();
            foreach (KeyValuePair<Mesh, Octave3DMesh> pair in _meshes.Dictionary)
            {
                if (pair.Key != null) newMeshDictionary.Dictionary.Add(pair.Key, pair.Value);
            }
    
            _meshes = newMeshDictionary;
        }
        #endregion
    }
}
#endif