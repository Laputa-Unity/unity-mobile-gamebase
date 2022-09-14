#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public interface IPivotPointRenderer
    {
        #region Interface Methods
        void Render(Vector3 pivotPoint, Color fillColor, Color borderLineColor, float sizeInPixels);
        #endregion
    }
}
#endif