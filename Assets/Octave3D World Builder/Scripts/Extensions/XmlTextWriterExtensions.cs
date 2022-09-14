#if UNITY_EDITOR
using UnityEngine;
using System.Xml;

namespace O3DWB
{
    public static class XmlTextWriterExtensions
    {
        #region Public Static Functions
        public static void WriteNewLine(this XmlTextWriter writer, int indentLevel)
        {
            string text = "\n";
            for (int indentIndex = 0; indentIndex < indentLevel; ++indentIndex) text += "\t";
            writer.WriteWhitespace(text);
        }

        public static void WriteColorString(this XmlTextWriter writer, Color color)
        {
            writer.WriteString(color.r + "," + color.g + "," + color.b + "," + color.a);
        }
        #endregion
    }
}
#endif