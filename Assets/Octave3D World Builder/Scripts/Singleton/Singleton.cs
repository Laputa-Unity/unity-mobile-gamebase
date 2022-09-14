#if UNITY_EDITOR
namespace O3DWB
{
    public abstract class Singleton<DataType> where DataType : Singleton<DataType>, new()
    {
        #region Private Static Variables
        /// <summary>
        /// The singleton instance.
        /// </summary>
        /// <remarks>
        /// Note: This assumes that the derived class must always have a public parameterless constructor.
        ///       This implies that the client code can create instances of the derived classes. We could
        ///       solve this using reflection, but in order to keep things simple and clean, we will avoid
        ///       doing that.
        /// </remarks>
        private static DataType _instance = new DataType();
        #endregion

        #region Public Static Properties
        public static DataType Instance { get { return _instance; } }
        #endregion
    }
}
#endif