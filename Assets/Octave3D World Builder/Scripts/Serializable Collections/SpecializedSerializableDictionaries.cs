#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class SerializableOctave3DMeshDictionary : SerializableDictionary<Mesh, Octave3DMesh> { }
}
#endif