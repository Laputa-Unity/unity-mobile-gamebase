#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementPathHeightPattern : ScriptableObject, INamedEntity
    {
        #region Private Variables
        [SerializeField]
        private string _name = "";
        [SerializeField]
        private string _patternString = "";
        [SerializeField]
        private List<int> _heightValues = new List<int>();

        private ObjectPlacementPathHeightPatternTokenExtractor _patternTokenExtractor = new ObjectPlacementPathHeightPatternTokenExtractor();
        private ObjectPlacementPathHeightPatternSyntaxValidator _patternSyntaxValidator = new ObjectPlacementPathHeightPatternSyntaxValidator();
        private ObjectPlacementPathHeightPatternValueExtractor _patternHeightValueExtractor = new ObjectPlacementPathHeightPatternValueExtractor();

        [SerializeField]
        private ObjectPlacementPathHeightPatternView _view;
        #endregion

        #region Public Properties
        public string Name { get { return _name; } set { if (!string.IsNullOrEmpty(value)) _name = value; } }
        public string PatternString { get { return _patternString; } }
        public List<int> HeightValues { get { return new List<int>(_heightValues); } }
        public ObjectPlacementPathHeightPatternView View { get { return _view; } }
        #endregion 

        #region Constructors
        public ObjectPlacementPathHeightPattern()
        {
            _view = new ObjectPlacementPathHeightPatternView(this);
            InitializePatternWithDefaultValues();
        }
        #endregion

        #region Public Methods
        public bool SetPatternString(string patternString)
        {
            List<ObjectPlacementPathHeightPatternToken> patternTokens;
            if(_patternTokenExtractor.ExtractTokensFromPatternString(patternString, out patternTokens))
            {
                if(_patternSyntaxValidator.ValidatePatternSyntax(patternTokens))
                {
                    _patternString = patternString;
                    _heightValues = _patternHeightValueExtractor.ExtractHeightValuesFromPatternTokens(patternTokens);

                    return true;
                }
            }

            return false;
        }

        public int GetHeightValue(int heightValueIndex, bool wrap)
        {
            if (_heightValues.Count == 0) return 0;

            if(wrap)
            {
                heightValueIndex %= _heightValues.Count;
                return _heightValues[heightValueIndex];
            }
            else
            {
                if (heightValueIndex >= _heightValues.Count) return _heightValues[_heightValues.Count - 1];
                else return _heightValues[heightValueIndex];
            }
        }
        #endregion

        #region Private Methods
        private void InitializePatternWithDefaultValues()
        {
            _patternString = "1,2,3";
            _heightValues.Add(1);
            _heightValues.Add(2);
            _heightValues.Add(3);
        }
        #endregion
    }
}
#endif