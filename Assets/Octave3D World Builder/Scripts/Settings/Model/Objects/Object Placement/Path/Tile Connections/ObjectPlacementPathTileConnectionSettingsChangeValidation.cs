#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectPlacementPathTileConnectionSettingsChangeValidation
    {
        #region Public Static Functions
        public static bool Validate(bool logMessage)
        {
            if (PathObjectPlacement.Get().IsPathUnderManualConstruction)
            {
                if (logMessage) Debug.LogWarning("This action can not be performed while a path is under construction.");
                return false;
            }

            return true;
        }
        #endregion
    }
}
#endif