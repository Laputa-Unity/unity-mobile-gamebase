#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class HoveredObjectEraseOperation : IObjectEraseOperation
    {
        #region Public Methods
        public void Perform()
        {
            MouseCursorRayHit cursorRayHit = ObjectEraser.Get().GetMouseCursorRayHit();
            if (cursorRayHit.WasAnObjectHit)
            {
                List<GameObject> objectsToErase = cursorRayHit.GetAllObjectsSortedByHitDistance();
                objectsToErase = ObjectEraser.Get().FilterObjectsWhichCanBeErased(objectsToErase);
                if (objectsToErase.Count == 0) return;

                objectsToErase = new List<GameObject> { objectsToErase[0] };
                ObjectErase.EraseObjectHierarchiesInObjectCollection(objectsToErase);
            }
        }
        #endregion
    }
}
#endif
