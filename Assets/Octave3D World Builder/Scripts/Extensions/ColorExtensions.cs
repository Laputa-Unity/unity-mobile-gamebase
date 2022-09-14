#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ColorExtensions
    {
        #region Utilities
        public static Color FromString(string colorString)
        {
            string[] colorComps = colorString.Split(',');
            Color color = new Color();
            for(int compIndex = 0; compIndex < 4; ++compIndex)
            {
                float colorComp;
                if(float.TryParse(colorComps[compIndex], out colorComp))
                {
                    color[compIndex] = colorComp;
                }
            }

            return color;
        }

        public static Color FromRGBA(byte r, byte g, byte b, byte a)
        {
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }
        #endregion
    }
}
#endif