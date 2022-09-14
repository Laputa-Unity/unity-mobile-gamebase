#if UNITY_EDITOR
namespace O3DWB
{
    public class ObjectPlacementBoxStackOverlapData
    {
        #region Private Variables
        private bool _isOverlappedByAnotherStack = false;
        #endregion

        #region Public Properties
        public bool IsOverlappedByAnotherStack { get { return _isOverlappedByAnotherStack; } set { _isOverlappedByAnotherStack = value; } }
        #endregion
    }
}
#endif