#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectMassEraseOperation : IObjectEraseOperation
    {
        #region Public Methods
        public void Perform()
        {
            List<GameObject> gameObjectsForMassEraseOperation = ObjectEraser.Get().GetGameObjectsForMassEraseOperation();        
            ObjectErase.EraseObjectHierarchiesInObjectCollection(gameObjectsForMassEraseOperation);
        }
        #endregion
    }
}
#endif