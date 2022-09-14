#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class MirroredEntityRenderer
    {
        #region Public Methods
        public void RenderMirroredHierarchyOrientedBoxes(Plane mirrorPlane, List<GameObject> hierarchyRoots, bool mirrorRotation, Color boxColor, Color boxBorderLineColor)
        {
            foreach(var root in hierarchyRoots)
                RenderMirroredHierarchyOrientedBox(mirrorPlane, root, mirrorRotation, boxColor, boxBorderLineColor);
        }

        public void RenderMirroredHierarchyOrientedBox(Plane mirrorPlane, GameObject hierarchyRoot, bool mirrorRotation, Color boxColor, Color boxBorderLineColor)
        {
            RenderMirroredEntityOrientedBox(mirrorPlane, hierarchyRoot.GetHierarchyWorldOrientedBox(), mirrorRotation, boxColor, boxBorderLineColor);
        }

        public void RenderMirroredEntityOrientedBox(Plane mirrorPlane, OrientedBox entityBox, bool mirrorRotation, Color boxColor, Color boxBorderLineColor)
        {
            if(!entityBox.IsValid()) return;

            OrientedBox mirroredEntityBox = Mirroring.MirrorOrientedBox(mirrorPlane, entityBox, mirrorRotation);
            GizmosEx.RenderOrientedBox(mirroredEntityBox, boxColor);
            GizmosEx.RenderOrientedBoxEdges(mirroredEntityBox, boxBorderLineColor);
        }

        public void RenderMirroredEntityOrientedBoxes(Plane mirrorPlane, List<OrientedBox> entityBoxes, bool mirrorRotation, Color boxColor, Color boxBorderLineColor)
        {
            foreach (var entityBox in entityBoxes) 
                RenderMirroredEntityOrientedBox(mirrorPlane, entityBox, mirrorRotation, boxColor, boxBorderLineColor);
        }
        #endregion
    }
}
#endif