#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectPlacementPathHeightPatternTokenFactory
    {
        #region Public Static Functions
        public static ObjectPlacementPathHeightPatternToken Create(string tokenText)
        {
            if (ObjectPlacementPathHeightPatternTokenTextValidation.IsTokenTextValid(tokenText))
            {
                if (tokenText.IsSingleDigit()) return new ObjectPlacementPathHeightPatternToken(ObjectPlacementPathHeightPatternTokenType.Digit, tokenText);
                if (tokenText.IsSingleChar(',')) return new ObjectPlacementPathHeightPatternToken(ObjectPlacementPathHeightPatternTokenType.Comma, tokenText);
                if (tokenText.IsSingleLetter()) return new ObjectPlacementPathHeightPatternToken(ObjectPlacementPathHeightPatternTokenType.Letter, tokenText);
                if (tokenText.IsSingleChar('(')) return new ObjectPlacementPathHeightPatternToken(ObjectPlacementPathHeightPatternTokenType.OpeningParenthesis, tokenText);
                if (tokenText.IsSingleChar(')')) return new ObjectPlacementPathHeightPatternToken(ObjectPlacementPathHeightPatternTokenType.ClosingParanthesis, tokenText);
                if (tokenText.IsSingleChar('-')) return  new ObjectPlacementPathHeightPatternToken(ObjectPlacementPathHeightPatternTokenType.MinusSign, tokenText);
                if (tokenText.ContainsOnlyWhiteSpace()) return new ObjectPlacementPathHeightPatternToken(ObjectPlacementPathHeightPatternTokenType.Whitespace, tokenText);
            }

            return null;
        }
        #endregion
    }
}
#endif