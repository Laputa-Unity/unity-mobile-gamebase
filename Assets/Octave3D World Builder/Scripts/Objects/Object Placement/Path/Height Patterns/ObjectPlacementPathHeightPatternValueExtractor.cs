#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathHeightPatternValueExtractor
    {
        #region Public Methods
        public List<int> ExtractHeightValuesFromPatternTokens(List<ObjectPlacementPathHeightPatternToken> patternTokens)
        {
            if (patternTokens.Count == 0) return new List<int>();

            List<ObjectPlacementPathHeightPatternToken> patternTokensNoWhitespace = new List<ObjectPlacementPathHeightPatternToken>(patternTokens);
            patternTokensNoWhitespace.RemoveAll(item => item.IsWhitespace());

            var heightValues = new List<int>(patternTokens.Count);

            int tokenIndex = 0;
            while(tokenIndex < patternTokensNoWhitespace.Count)
            {
                ObjectPlacementPathHeightPatternToken token = patternTokensNoWhitespace[tokenIndex];
                if(token.IsDigit() || token.IsMinusSign())
                {
                    heightValues.Add(ExtractHeightValueFromDigitTokenSequence(patternTokensNoWhitespace, tokenIndex, out tokenIndex));
                    continue;
                }

                if(token.IsRepeatSequenceLetter())
                {
                    heightValues.AddRange(ExtractHeightValuesFromRepeatSequence(patternTokensNoWhitespace, tokenIndex + 2, out tokenIndex));
                    continue;
                }

                if (token.IsIncrementSequenceLetter())
                {
                    heightValues.AddRange(ExtractHeightValuesFromIncrementSequence(patternTokensNoWhitespace, tokenIndex + 2, out tokenIndex));
                    continue;
                }

                ++tokenIndex;
            }

            return heightValues;
        }
        #endregion

        #region Private Methods
        private int ExtractHeightValueFromDigitTokenSequence(List<ObjectPlacementPathHeightPatternToken> patternTokens, int indexOfFirstToken, out int nextTokenIndex)
        {
            string digitString = "";
            int currentTokenIndex = indexOfFirstToken;
            if(patternTokens[indexOfFirstToken].IsMinusSign())
            {
                digitString = "-";
                ++currentTokenIndex;
            }

            while (currentTokenIndex < patternTokens.Count)
            {
                ObjectPlacementPathHeightPatternToken token = patternTokens[currentTokenIndex];
                if (token.IsDigit()) digitString += token.Text;
                else break;

                ++currentTokenIndex;
            }

            nextTokenIndex = currentTokenIndex;
            return int.Parse(digitString);
        }

        private List<int> ExtractHeightValuesFromRepeatSequence(List<ObjectPlacementPathHeightPatternToken> patternTokens, int indexOfSequenceBeginToken, out int nextTokenIndex)
        {
            int currentTokenIndex = indexOfSequenceBeginToken;
            int valueToRepeat = ExtractHeightValueFromDigitTokenSequence(patternTokens, currentTokenIndex, out currentTokenIndex);

            ++currentTokenIndex;
            int numberOfTimesToRepeatValue = ExtractHeightValueFromDigitTokenSequence(patternTokens, currentTokenIndex, out currentTokenIndex);

            var heightValues = new List<int>(patternTokens.Count);
            for(int repeatIndex = 0; repeatIndex < numberOfTimesToRepeatValue; ++repeatIndex)
            {
                heightValues.Add(valueToRepeat);
            }

            nextTokenIndex = currentTokenIndex;
            return heightValues;
        }

        private List<int> ExtractHeightValuesFromIncrementSequence(List<ObjectPlacementPathHeightPatternToken> patternTokens, int indexOfSequenceBeginToken, out int nextTokenIndex)
        {
            int currentTokenIndex = indexOfSequenceBeginToken;
            int valueToIncrement = ExtractHeightValueFromDigitTokenSequence(patternTokens, currentTokenIndex, out currentTokenIndex);

            ++currentTokenIndex;
            int incrementAmount = ExtractHeightValueFromDigitTokenSequence(patternTokens, currentTokenIndex, out currentTokenIndex);

            ++currentTokenIndex;
            int numberOfTimesToIncrement = ExtractHeightValueFromDigitTokenSequence(patternTokens, currentTokenIndex, out currentTokenIndex);

            var heightValues = new List<int>(patternTokens.Count);
            int currentValue = valueToIncrement;
            heightValues.Add(currentValue);
            for (int incrementIndex = 0; incrementIndex < numberOfTimesToIncrement; ++incrementIndex)
            {
                currentValue += incrementAmount;
                heightValues.Add(currentValue);
            }

            nextTokenIndex = currentTokenIndex;
            return heightValues;
        }
        #endregion
    }
}
#endif