#if UNITY_EDITOR
namespace O3DWB
{
    public static class StringExtensions
    {
        #region Extension Methods
        public static string RemoveTrailingSlashes(this string str)
        {
            string finalString = str;

            while (finalString.LastChar() == '\\' || finalString.LastChar() == '/')
            {
                finalString = finalString.Substring(0, finalString.LastCharIndex());
            }

            return finalString;
        }

        public static char LastChar(this string str)
        {
            return str[str.LastCharIndex()];
        }

        public static int LastCharIndex(this string str)
        {
            return str.Length - 1;
        }

        public static bool ContainsOnlyWhiteSpace(this string str)
        {
            for(int charIndex = 0; charIndex < str.Length; ++charIndex)
            {
                if (!char.IsWhiteSpace(str[charIndex])) return false;
            }

            return true;
        }

        public static bool IsSingleDigit(this string str)
        {
            return str.Length == 1 && char.IsDigit(str[0]);
        }

        public static bool IsSingleLetter(this string str)
        {
            return str.Length == 1 && char.IsLetter(str[0]);
        }

        public static bool IsSingleChar(this string str, char character)
        {
            return str.Length == 1 && str[0] == character;
        }
        #endregion
    }
}
#endif