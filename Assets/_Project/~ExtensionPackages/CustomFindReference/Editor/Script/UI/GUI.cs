using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace custom.find.reference
{
    public interface IDrawer
    {
        bool Draw(Rect rect);
        bool DrawLayout();
    }
    
    public static class GUI
    {
        public static void Color(Action a, Color c, float? alpha = null)
        {
            if (a == null) return;

            var cColor = UnityEngine.GUI.color;
            if (alpha != null) c.a = alpha.Value;

            UnityEngine.GUI.color = c;
            a();
            UnityEngine.GUI.color = cColor;
        }
        
        public static void ContentColor(Action a, Color c, float? alpha = null)
        {
            if (a == null) return;

            var cColor = UnityEngine.GUI.contentColor;
            if (alpha != null) c.a = alpha.Value;

            UnityEngine.GUI.contentColor = c;
            a();
            UnityEngine.GUI.contentColor = cColor;
        }
        
        public static void BackgroundColor(Action a, Color c, float? alpha = null)
        {
            if (a == null) return;

            var cColor = UnityEngine.GUI.backgroundColor;
            if (alpha != null) c.a = alpha.Value;

            UnityEngine.GUI.backgroundColor = c;
            a();
            UnityEngine.GUI.backgroundColor = cColor;
        }

        public static Color Theme(Color proColor, Color indieColor)
        {
            return EditorGUIUtility.isProSkin ? proColor : indieColor;
        }

        public static Color Alpha(Color c, float a)
        {
            c.a = a;
            return c;
        }

        public static void Rect(Rect r, Color c, float? alpha = null)
        {
            var cColor = UnityEngine.GUI.color;
            if (alpha != null) c.a = alpha.Value;

            UnityEngine.GUI.color = c;
            UnityEngine.GUI.DrawTexture(r, Texture2D.whiteTexture);
            UnityEngine.GUI.color = cColor;
        }
        
        public static UnityEngine.Object[] DropZone(string title, float w, float h)
        {
            var rect = GUILayoutUtility.GetRect(w, h);
            UnityEngine.GUI.Box(rect, GUIContent.none, EditorStyles.textArea);

            var cx = rect.x + w/ 2f;
            var cy = rect.y + h/ 2f;
            var pz = w / 3f; // plus size
            
            var plusRect = new Rect(cx - pz/2f, (cy - pz/2f), pz, pz);
            GUI.Color(() => { UnityEngine.GUI.DrawTexture(plusRect, CustomIcon.Plus.image, ScaleMode.ScaleToFit); }, UnityEngine.Color.white, 0.1f);
            
            UnityEngine.GUI.Label(rect, title, EditorStyles.wordWrappedMiniLabel);
            
            EventType eventType = Event.current.type;
            bool isAccepted = false;
   
            if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
       
                if (eventType == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();
                    isAccepted = true;
                }
                Event.current.Use();
            }
   
            return isAccepted ? DragAndDrop.objectReferences : null;
        }

        public static bool ColorIconButton(Rect r, Texture icon, Color? c)
        {
            var oColor = UnityEngine.GUI.color;
            if (c != null) UnityEngine.GUI.color = c.Value;
            var result = UnityEngine.GUI.Button(r, icon, GUIStyle.none);
            UnityEngine.GUI.color = oColor;
            return result;
        }

        public static bool Toggle(ref bool value, string label, GUIStyle style, params GUILayoutOption[] options)
        {
            var vv = GUILayout.Toggle(value, label, style, options);
            if (vv == value) return false;
            value = vv;
            return true;
        }
        
        public static bool Toggle(ref bool value, Texture2D tex, GUIStyle style, params GUILayoutOption[] options)
        {
            var vv = GUILayout.Toggle(value, tex, style, options);
            if (vv == value) return false;
            value = vv;
            return true;
        }
        
        public static bool Toggle(ref bool value, GUIContent tex, GUIStyle style, params GUILayoutOption[] options)
        {
            var vv = GUILayout.Toggle(value, tex, style, options);
            if (vv == value) return false;
            value = vv;
            return true;
        }
        
        public static bool Toggle(Rect rect, ref bool value, GUIContent tex)
        {
            var vv = UnityEngine.GUI.Toggle(rect, value, tex, GUIStyle.none);
            if (vv == value) return false;
            value = vv;
            return true;
        }
        
        public static bool Toggle(Rect rect, ref bool value)
        {
            var vv = UnityEngine.GUI.Toggle(rect, value, GUIContent.none);
            if (vv == value) return false;
            value = vv;
            return true;
        }
        
        internal static bool Toggle(bool v, string label, Action<bool> setter)
        {
            var v1 = GUILayout.Toggle(v, label);
            if (v1 == v) return false;
            if (setter != null) setter(v1);
            return true;
        }

        internal static Dictionary<string, GUIContent> tooltipCache = new Dictionary<string, GUIContent>();
        internal static GUIContent GetTooltip(string tooltip)
        {
            if (string.IsNullOrEmpty(tooltip)) return GUIContent.none;

            GUIContent result;
            if (tooltipCache.TryGetValue(tooltip, out result)) return result;
            result = new GUIContent(string.Empty, tooltip);
            tooltipCache.Add(tooltip, result);
            return result;
        }
        
        internal static bool ToolbarToggle(ref bool value, Texture icon, Vector2 padding, string tooltip = null)
        {
            var vv = GUILayout.Toggle(value, GetTooltip(tooltip), EditorStyles.toolbarButton, GUILayout.Width(22f));
            
            if (icon != null)
            {
                var rect = GUILayoutUtility.GetLastRect();
                rect = Padding(rect, padding.x, padding.y);
                UnityEngine.GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
            }

            if (vv == value) return false;
            value = vv;
            return true;
        }
        
        // TODO : optimize for performance
        public static bool EnumPopup<T>(ref T mode, string label, float labelWidth, GUIStyle style, params GUILayoutOption[] options)
        {
            var sz = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
            {
                var obj = (Enum)(object) mode;
                var vv = EditorGUILayout.EnumPopup(label, obj, style, options);
                if (Equals(vv, obj))
                {
                    EditorGUIUtility.labelWidth = sz;
                    return false;
                }
                
                mode = (T)(object)vv;   
            }
            EditorGUIUtility.labelWidth = sz;
            return true;
        }
        
        public static bool EnumPopup<T>(ref T mode, GUIContent icon, GUIStyle style, params GUILayoutOption[] options)
        {
            var obj = (Enum)(object) mode;
            var cRect = GUILayoutUtility.GetRect(16f, 16f);
            cRect.xMin -= 2f;
            cRect.yMin += 2f;
            UnityEngine.GUI.Label(cRect, icon);
            
            var vv = EditorGUILayout.EnumPopup(obj, style, options);
            if (Equals(vv, obj))
            {
                return false;
            }
            
            mode = (T)(object)vv;
            return true;
        }

        public static Rect Padding(Rect r, float x, float y)
        {
            return new Rect(r.x + x, r.y + y, r.width - 2 * x, r.height - 2 * y);
        }
        
        public static Rect LeftRect(float w, ref Rect rect)
        {
            rect.x += w;
            rect.width -= w;
            return new Rect(rect.x - w, rect.y, w, rect.height);
        }

        public static Rect RightRect(float w, ref Rect rect)
        {
            rect.width -= w;
            return new Rect(rect.x + rect.width, rect.y, w, rect.height);
        }
        
        // -----------------------

        private static GUIStyle _miniLabelAlignRight; 
        public static GUIStyle miniLabelAlignRight
        {
            get { return _miniLabelAlignRight ??= new GUIStyle(EditorStyles.miniLabel) {alignment = TextAnchor.MiddleRight};
            }
        }
        
        public static Color darkRed = new Color(0.5f, .0f, 0f, 1f);
        public static Color darkGreen = new Color(0, .5f, 0f, 1f);
        public static Color darkBlue = new Color(0, .0f, 0.5f, 1f);
        public static Color lightRed = new Color(1f, 0.5f, 0.5f, 1f);
    }
}