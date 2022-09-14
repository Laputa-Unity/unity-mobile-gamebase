#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class Object2ObjectBoxSnapDatabase : Singleton<Object2ObjectBoxSnapDatabase>
    {
        private Dictionary<GameObject, Object2ObjectBoxSnapData> _meshObjectToBoxSnapData = new Dictionary<GameObject, Object2ObjectBoxSnapData>();

        public Object2ObjectBoxSnapData GetObject2ObjectBoxSnapData(GameObject meshObject)
        {
            if (_meshObjectToBoxSnapData.ContainsKey(meshObject)) return _meshObjectToBoxSnapData[meshObject];

            var snapData = Object2ObjectBoxSnapDataFactory.Create(meshObject);
            if (snapData != null) _meshObjectToBoxSnapData.Add(meshObject, snapData);
            return snapData;
        }
    }
}
#endif
