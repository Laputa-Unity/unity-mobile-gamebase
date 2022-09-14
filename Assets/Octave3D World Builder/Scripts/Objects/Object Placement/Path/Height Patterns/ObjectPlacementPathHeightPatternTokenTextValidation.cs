#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectPlacementPathHeightPatternTokenTextValidation
    {
        #region Private Static Variables
        private static List<string> _validTokens = new List<string>()
        {
            ",", "i", "r", "(", ")", "0", "1", "2", "3", "4", "5",
            "6", "7", "8", "9", "-"
        };
        #endregion

        #region Public Static Methods
        public static bool IsTokenTextValid(string tokenText)
        {
            return tokenText.ContainsOnlyWhiteSpace() || _validTokens.Contains(tokenText.ToLower()); 
        }
        #endregion
    }
}
#endif