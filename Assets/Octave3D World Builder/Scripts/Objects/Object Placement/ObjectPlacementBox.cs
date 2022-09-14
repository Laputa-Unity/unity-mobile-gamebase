#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementBox
    {
        #region Private Variables
        private OrientedBox _orientedBox = new OrientedBox();
        private ObjectPlacementBoxHideFlags _hideFlags = ObjectPlacementBoxHideFlags.None;
        #endregion

        #region Public Properties
        public OrientedBox OrientedBox { get { return new OrientedBox(_orientedBox); } }
        public Vector3 Center { get { return _orientedBox.Center; } set { _orientedBox.Center = value; } }
        public Vector3 ModelSpaceSize { get { return _orientedBox.ModelSpaceSize; } set { _orientedBox.ModelSpaceSize = value; } }
        public Vector3 ScaledSize { get { return _orientedBox.ScaledSize; } }
        public Quaternion Rotation { get { return _orientedBox.Rotation; } set { _orientedBox.Rotation = value; } }
        public bool IsHidden { get { return _hideFlags != ObjectPlacementBoxHideFlags.None; } }
        #endregion

        #region Constructors
        public ObjectPlacementBox()
        {
        }

        public ObjectPlacementBox(ObjectPlacementBox source)
        {
            _orientedBox = source.OrientedBox;
        }
        #endregion

        #region Public Methods
        public void ClearAllHideFlags()
        {
            _hideFlags = ObjectPlacementBoxHideFlags.None;
        }

        public void SetHideFlag(ObjectPlacementBoxHideFlags hideFlag)
        {
            _hideFlags |= hideFlag;
        }

        public void ClearHideFlag(ObjectPlacementBoxHideFlags hideFlag)
        {
            _hideFlags &= ~hideFlag;
        }
        #endregion
    }
}
#endif