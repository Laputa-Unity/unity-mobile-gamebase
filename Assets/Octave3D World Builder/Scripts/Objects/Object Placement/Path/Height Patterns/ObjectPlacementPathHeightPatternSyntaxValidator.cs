#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathHeightPatternSyntaxValidator
    {
        #region Public Methods
        public bool ValidatePatternSyntax(List<ObjectPlacementPathHeightPatternToken> patternTokens)
        {
            if (patternTokens.Count == 0) return false;

            if (!ValidateNoDigitsSeparatedByWhitespace(patternTokens)) return false;

            List<ObjectPlacementPathHeightPatternToken> patternTokensNoWhitespace = new List<ObjectPlacementPathHeightPatternToken>(patternTokens);
            patternTokensNoWhitespace.RemoveAll(item => item.IsWhitespace());

            if (!ValidateTokenSequence(patternTokensNoWhitespace)) return false;

            return true;
        }
        #endregion

        #region Private Methods
        private bool ValidateTokenSequence(List<ObjectPlacementPathHeightPatternToken> patternTokensNoWhitespace)
        {
            bool isInsideRepeatSequence = false;
            bool isInsideIncrementSequence = false;
            int numberOfCommasInRepeatSequence = 0;
            int numberOfCommasInIncrementSequence = 0;

            // If only one token is present, it must be a digit
            if(patternTokensNoWhitespace.Count == 1 && !patternTokensNoWhitespace[0].IsDigit())
            {
                LogInvalidSyntaxMessage("Single character patterns must only be composed of a single digit.");
                return false;
            }

            // Ensure the pattern does not start with an invalid character
            ObjectPlacementPathHeightPatternToken firstToken = patternTokensNoWhitespace[0];
            if (firstToken.IsComma() || firstToken.IsOpeningParenthesis() || firstToken.IsClosingParanthesis())
            {
                LogInvalidSyntaxMessage("A pattern can not start with " + firstToken.Text + ".");
                return false;
            }

            for (int tokenIndex = 0; tokenIndex < patternTokensNoWhitespace.Count; ++tokenIndex)
            {
                // Validate token precedence
                ObjectPlacementPathHeightPatternToken currentToken = patternTokensNoWhitespace[tokenIndex];
                if(tokenIndex < patternTokensNoWhitespace.Count - 1)
                {
                    ObjectPlacementPathHeightPatternToken nextToken = patternTokensNoWhitespace[tokenIndex + 1];
                    if (!CanTokenBeFollowedByToken(currentToken, nextToken))
                    {
                        LogTokenCanNotBeFollowedByTokenMessage(currentToken, nextToken);
                        return false;
                    }

                    // Ensure that we are not dealing with numbers that start with a zero digit
                    if (tokenIndex == 0 || !patternTokensNoWhitespace[tokenIndex - 1].IsDigit() && currentToken.IsDigit() && nextToken.IsDigit())
                    {
                        if(currentToken.Text == "0")
                        {
                            LogInvalidSyntaxMessage("Numbers can not start with 0.");
                            return false;
                        }
                    }
                }

                // Validate nested sequences
                if(currentToken.IsRepeatSequenceLetter())
                {
                    if(isInsideIncrementSequence) 
                    {
                        LogInvalidSyntaxMessage("Can not have a repeat sequence inside an increment sequence.");
                        return false;
                    }
                    else
                    if(isInsideRepeatSequence) 
                    {
                        LogInvalidSyntaxMessage("Nested repeat sequences are not allowed.");
                        return false;
                    }
                    else isInsideRepeatSequence = true;
                }
                else
                if(currentToken.IsIncrementSequenceLetter())
                {
                    if(isInsideIncrementSequence) 
                    {
                        LogInvalidSyntaxMessage("Nested increment sequences are not allowed.");
                        return false;
                    }
                    else
                    if(isInsideRepeatSequence) 
                    {
                        LogInvalidSyntaxMessage("Can not have an increment sequence inside a repeat sequence.");
                        return false;
                    }
                    else isInsideIncrementSequence = true;
                }

                // Validate paranthesis
                if((!isInsideRepeatSequence && !isInsideIncrementSequence) && currentToken.IsOpeningParenthesis()) 
                {
                    LogInvalidSyntaxMessage("The \'(\' must follow a repeat or increment sequence character (e.g. \'i\' or \'r\')");
                    return false;
                }
                if(currentToken.IsClosingParanthesis())
                {
                    if(!isInsideRepeatSequence && !isInsideIncrementSequence)
                    {
                        LogInvalidSyntaxMessage("The \')\' must end a repeat or increment sequence. In this case no sequence is present.");
                        return false;
                    }
                    else
                    {
                        // Ensure we have the proper number of commas inside the sequence which just ended
                        if(isInsideRepeatSequence)
                        {
                            if(numberOfCommasInRepeatSequence != 1)
                            {
                                LogInvalidSyntaxMessage("A repeat sequence must contain the value which must be repeated and the number of times to repeat the value (e.g. r(value, repeatCount)).");
                                return false;
                            }

                            isInsideRepeatSequence = false;
                            numberOfCommasInRepeatSequence = 0;
                        }
                        else
                        if(isInsideIncrementSequence)
                        {
                            if (numberOfCommasInIncrementSequence != 2)
                            {
                                LogInvalidSyntaxMessage("An increment sequence must contain the value which must be incremented, the amount used to increment the value and the number of times to increment (e.g. i(value, incrAmount, numberOfIncrements)).");
                                return false;
                            }

                            isInsideIncrementSequence = false;
                            numberOfCommasInIncrementSequence = 0;
                        }
                    }
                }

                // Adjust the number of commas inside the current sequence
                if(isInsideIncrementSequence)
                {
                    if (currentToken.IsComma()) ++numberOfCommasInIncrementSequence;
                }
                else
                if(isInsideRepeatSequence)
                {
                    if (currentToken.IsComma()) ++numberOfCommasInRepeatSequence;
                }

                // Validate minus sign 
                if(currentToken.IsMinusSign())
                {
                    if(isInsideRepeatSequence && numberOfCommasInRepeatSequence == 1)
                    {
                        LogInvalidSyntaxMessage("The repeat amount can not be negative inside a repeat sequence.");
                        return false;
                    }
                    else
                    if(isInsideIncrementSequence && numberOfCommasInIncrementSequence == 2)
                    {
                        LogInvalidSyntaxMessage("The number of times to increment can not be negative inside an increment sequence.");
                        return false;
                    }
                }
            }

            // If we traversed all tokens and we find ourselves still inside a sequence, it means the sequence has not been ended properyl
            if(isInsideRepeatSequence) 
            {
                LogInvalidSyntaxMessage("A repeat sequence has not been ended properly. All repeat sequences must end with a \')\' character.");
                return false;
            }
            if(isInsideIncrementSequence)
            {
                LogInvalidSyntaxMessage("An increment sequence has not been ended properly. All increment sequences must end with a \')\' character.");
                return false;
            }

            // Ensure the sequence ends with a valid character
            ObjectPlacementPathHeightPatternToken lastToken = patternTokensNoWhitespace[patternTokensNoWhitespace.Count - 1];
            if (lastToken.IsComma() || lastToken.IsMinusSign())
            {
                LogInvalidSyntaxMessage("A pattern can not end with \'" + lastToken.Text + "\'.");
                return false;
            }

            return true;
        }

        private bool CanTokenBeFollowedByToken(ObjectPlacementPathHeightPatternToken token, ObjectPlacementPathHeightPatternToken nextToken)
        {
            if (nextToken.IsWhitespace()) return true;

            if (token.IsClosingParanthesis()) return nextToken.IsComma();
            if (token.IsOpeningParenthesis()) return nextToken.IsDigit() || nextToken.IsMinusSign();
            if (token.IsComma()) return nextToken.IsDigit() || nextToken.IsLetter() || nextToken.IsMinusSign();
            if (token.IsDigit()) return nextToken.IsDigit() || nextToken.IsComma() || nextToken.IsClosingParanthesis();
            if (token.IsLetter()) return nextToken.IsOpeningParenthesis();
           
            return true;
        }

        private bool ValidateNoDigitsSeparatedByWhitespace(List<ObjectPlacementPathHeightPatternToken> patternTokens)
        {
            if (patternTokens.Count < 3) return true;

            int currentTokenIndex = 0;
            while (currentTokenIndex < patternTokens.Count)
            {
                // Find the first whitespace token which follows a digit
                while (currentTokenIndex < patternTokens.Count)
                {
                    ObjectPlacementPathHeightPatternToken token = patternTokens[currentTokenIndex];
                    if (token.IsWhitespace())
                    {
                        bool previousTokenIsDigit = currentTokenIndex != 0 && patternTokens[currentTokenIndex - 1].IsDigit();
                        if (previousTokenIsDigit) break;
                    }

                    ++currentTokenIndex;
                }
             
                // Find the first non-whitespace token. If the token following it is a digit, we have an invalid token sequence.
                ++currentTokenIndex;
                while(currentTokenIndex < patternTokens.Count)
                {
                    ObjectPlacementPathHeightPatternToken token = patternTokens[currentTokenIndex];
                    if (!token.IsWhitespace())
                    {
                        if (token.IsDigit())
                        {
                            LogInvalidSyntaxMessage("2 digits can not be separated by whitespace.");
                            return false;
                        }
                        else
                        {
                            ++currentTokenIndex;
                            break;
                        }
                    }

                    ++currentTokenIndex;
                }
            }

            return true;
        }

        private void LogTokenCanNotBeFollowedByTokenMessage(ObjectPlacementPathHeightPatternToken token, ObjectPlacementPathHeightPatternToken nextToken)
        {
            LogInvalidSyntaxMessage("\'" + token.Text + "\' can not be followed by \'" + nextToken.Text + "\'.");
        }

        private void LogInvalidSyntaxMessage(string message)
        {
            Debug.LogError("Invalid pattern syntax. " + message);
        }
        #endregion
    }
}
#endif