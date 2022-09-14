#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class Range<DataType> where DataType : struct
    {
        #region Private Variables
        [SerializeField]
        private DataType _min;
        [SerializeField]
        private DataType _max;
        #endregion

        #region Public Properties
        public DataType Min { get { return _min; } set { _min = value; } }
        public DataType Max { get { return _max; } set { _max = value; } }
        #endregion

        #region Constructors
        public Range()
        {
            _min = default(DataType);
            _max = _min;

            ValidateDataType();
        }

        public Range(DataType min, DataType max)
        {
            _min = min;
            _max = max;

            ValidateDataType();
        }
        #endregion

        #region Private Methods
        private void ValidateDataType()
        {
            if (!typeof(DataType).IsNumeric())
                throw new UnityException("Range.ValidateDataType: Only numeric primitive types are allowed.");
        }
        #endregion
    }
}
#endif