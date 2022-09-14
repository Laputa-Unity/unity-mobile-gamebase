#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathHeightPatternTokenExtractor
    {
        #region Public Methods
        public bool ExtractTokensFromPatternString(string patternString, out List<ObjectPlacementPathHeightPatternToken> patternTokens)
        {
            patternTokens = new List<ObjectPlacementPathHeightPatternToken>();
            if (string.IsNullOrEmpty(patternString)) return false;

            string trimmedPattern = patternString.Trim();
            for(int charIndex = 0; charIndex < trimmedPattern.Length; ++charIndex)
            {
                if (!CreateTokenFromCharacterAndStore(trimmedPattern[charIndex], patternTokens)) return false;
            }

            return true;
        }
        #endregion

        #region Private Methods
        private bool CreateTokenFromCharacterAndStore(char character, List<ObjectPlacementPathHeightPatternToken> destinationTokenList)
        {
            string tokenText = new string(character, 1);
            ObjectPlacementPathHeightPatternToken token = ObjectPlacementPathHeightPatternTokenFactory.Create(tokenText);
            if (token != null) destinationTokenList.Add(token);
            else
            {
                LogInvalidCharacterError(character);
                return false;
            }

            return true;
        }

        private void LogInvalidCharacterError(char invalidCharacter)
        {
            Debug.LogError("Invalid character detected inside pattern string: " + invalidCharacter);
        }
        #endregion
    }
}
#endif