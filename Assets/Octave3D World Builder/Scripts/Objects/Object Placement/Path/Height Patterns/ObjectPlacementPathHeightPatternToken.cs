#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementPathHeightPatternToken
    {
        #region Private Variables
        private ObjectPlacementPathHeightPatternTokenType _type;
        private string _text;
        #endregion

        #region Public Properties
        public ObjectPlacementPathHeightPatternTokenType Type { get { return _type; } }
        public string Text { get { return _text; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathHeightPatternToken(ObjectPlacementPathHeightPatternTokenType type, string text)
        {
            _type = type;
            _text = text.ToLower();
        }
        #endregion

        #region Public Methods
        public bool IsLetter()
        {
            return _type == ObjectPlacementPathHeightPatternTokenType.Letter;
        }

        public bool IsRepeatSequenceLetter()
        {
            return _text == "r";
        }

        public bool IsIncrementSequenceLetter()
        {
            return _text == "i";
        }

        public bool IsOpeningParenthesis()
        {
            return _type == ObjectPlacementPathHeightPatternTokenType.OpeningParenthesis;
        }

        public bool IsClosingParanthesis()
        {
            return _type == ObjectPlacementPathHeightPatternTokenType.ClosingParanthesis;
        }

        public bool IsMinusSign()
        {
            return _type == ObjectPlacementPathHeightPatternTokenType.MinusSign;
        }

        public bool IsComma()
        {
            return _type == ObjectPlacementPathHeightPatternTokenType.Comma;
        }

        public bool IsDigit()
        {
            return _type == ObjectPlacementPathHeightPatternTokenType.Digit;
        }

        public bool IsWhitespace()
        {
            return _type == ObjectPlacementPathHeightPatternTokenType.Whitespace;
        }
        #endregion
    }
}
#endif