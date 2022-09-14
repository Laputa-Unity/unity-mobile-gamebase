#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class PivotPointRendererFactory
    {
        #region Public Static Functions
        public static IPivotPointRenderer Create(ObjectPivotPointShapeType pivotPointShapeType)
        {
            switch(pivotPointShapeType)
            {
                case ObjectPivotPointShapeType.Circle:

                    return new CirclePivotPointRenderer();

                case ObjectPivotPointShapeType.Square:

                    return new SquarePivotPointRenderer();

                default:

                    return null;
            }
        }
        #endregion
    }
}
#endif