// Toony Colors Pro 2
// (c) 2014-2021 Jean Moreno

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ToonyColorsPro.Utilities;

//Extended GUILayout for Shader Generator 2

namespace ToonyColorsPro
{
	namespace ShaderGenerator
	{
		static class SGUILayout
		{
			public static float Indent = 0f;

			//--------------------------------------------------------------------------------------------------------------------------------
			// UI Constants

			public static class Constants
			{
				public const string screenSpaceUVLabel = "Screen Space";
				public const string worldPosUVLabel = "World Position";
				public const string triplanarUVLabel = "Triplanar";
				public const string shaderPropertyUVLabel = "Other Shader Property";
				public const string customMaterialPropertyUVLabel = "Custom Material Property";

				public static readonly string[] DefaultTextureValues =
				{
					"white",
					"black",
					"gray",
					"bump"
				};

				public static readonly string[] UvChannelOptions =
				{
					"texcoord0",
					"texcoord1",
					"texcoord2",
					"texcoord3",
					screenSpaceUVLabel,
					worldPosUVLabel,
					triplanarUVLabel,
					shaderPropertyUVLabel,
					customMaterialPropertyUVLabel
				};

				public static readonly string[] UvChannelOptionsVertex =
				{
					"texcoord0",
					"texcoord1",
					"texcoord2",
					"texcoord3",
					worldPosUVLabel,
					triplanarUVLabel,
					shaderPropertyUVLabel,
					customMaterialPropertyUVLabel
				};

				public static string[] LockedUvChannelOptions =
				{
					"computed in shader"
				};

				public static readonly string[] UvAnimationOptions =
				{
					"Off",
					"Scrolling",
					"Random Offset",
					"Sine Distortion"
				};
			}

			//--------------------------------------------------------------------------------------------------------------------------------
			// GUIStyles

			internal static class Styles
			{
#if UNITY_2019_3_OR_NEWER
				public const float shurikenLineHeight = 13;
#else
				public const float shurikenLineHeight = 16;
#endif

				internal static Color colorFieldBorderColor = new Color(0, 0, 0, 0.17f);
				internal static Color colorFieldBorderColorHover = new Color(0, 0, 0, 0.5f);
				internal static Color colorFieldBorderColorPro = new Color(0, 0, 0, 0.4f);
				internal static Color colorFieldBorderColorHoverPro = new Color(1, 1, 1, 0.22f);

				static GUIStyle _GrayLabel;
				internal static GUIStyle GrayLabel
				{
					get
					{
						if(_GrayLabel == null)
						{
							var color = EditorGUIUtility.isProSkin ? new Color32(130, 130, 130, 255) : new Color32(100, 100, 100, 255);
							_GrayLabel = new GUIStyle(EditorStyles.label);
							_GrayLabel.normal.textColor = color;
							_GrayLabel.active.textColor = color;
							_GrayLabel.focused.textColor = color;
							_GrayLabel.hover.textColor = color;
						}
						return _GrayLabel;
					}
				}

				internal static Color OrangeColor { get { return EditorGUIUtility.isProSkin ? new Color32(250, 130, 0, 255) : new Color32(200, 100, 20, 255); } }

				static GUIStyle _OrangeBoldLabel;
				internal static GUIStyle OrangeBoldLabel
				{
					get
					{
						if(_OrangeBoldLabel == null)
						{
							var color = OrangeColor;
							_OrangeBoldLabel = new GUIStyle(EditorStyles.label);
							_OrangeBoldLabel.normal.textColor = color;
							_OrangeBoldLabel.active.textColor = color;
							_OrangeBoldLabel.focused.textColor = color;
							_OrangeBoldLabel.hover.textColor = color;
							_OrangeBoldLabel.fontStyle = FontStyle.Bold;
						}
						return _OrangeBoldLabel;
					}
				}

				static GUIStyle _OrangeHeader;
				internal static GUIStyle OrangeHeader
				{
					get
					{
						if(_OrangeHeader == null)
						{
							_OrangeHeader = new GUIStyle(OrangeBoldLabel);
							_OrangeHeader.fontSize = 16;
						}
						return _OrangeHeader;
					}
				}

				static GUIStyle _GrayBoldLabel;
				internal static GUIStyle GrayBoldLabel
				{
					get
					{
						if(_GrayBoldLabel == null)
						{
							_GrayBoldLabel = new GUIStyle(GrayLabel);
							_GrayBoldLabel.fontStyle = FontStyle.Bold;
						}
						return _GrayBoldLabel;
					}
				}

				static GUIStyle _GrayMiniLabel;
				internal static GUIStyle GrayMiniLabel
				{
					get
					{
						if(_GrayMiniLabel == null)
						{
							_GrayMiniLabel = new GUIStyle("ShurikenLabel")
							{
								fixedHeight = shurikenLineHeight,
								padding = new RectOffset(2, 4, 0, 0),
								fontSize = shurikenFontSize
							};
							var c = EditorGUIUtility.isProSkin ? .7f : .3f;
							_GrayMiniLabel.normal.textColor = new Color(c, c, c, 1.0f);
						}
						return _GrayMiniLabel;
					}
				}

				static GUIStyle _GrayMiniLabelWrap;
				internal static GUIStyle GrayMiniLabelWrap
				{
					get
					{
						if (_GrayMiniLabelWrap == null)
						{
							_GrayMiniLabelWrap = new GUIStyle(GrayMiniLabel)
							{
								wordWrap = true,
								fixedHeight = 0,
								stretchHeight = false,
								stretchWidth = false
							};
						}
						return _GrayMiniLabelWrap;
					}
				}

				static GUIStyle _GrayMiniLabelWrapHighlighted;
				internal static GUIStyle GrayMiniLabelWrapHighlighted
				{
					get
					{
						if (_GrayMiniLabelWrapHighlighted == null)
						{
							_GrayMiniLabelWrapHighlighted = new GUIStyle(GrayMiniLabelWrap)
							{
								fontStyle = FontStyle.Bold
							};
							var textColor = EditorGUIUtility.isProSkin ? new Color(0.0f, 0.574f, 0.488f) : new Color(0.03f, 0.46f, 0.4f);
							_GrayMiniLabelWrapHighlighted.normal.textColor = textColor;
						}
						return _GrayMiniLabelWrapHighlighted;
					}
				}


				static GUIStyle _GrayMiniBoldLabel;
				internal static GUIStyle GrayMiniBoldLabel
				{
					get
					{
						if(_GrayMiniBoldLabel == null)
						{
							_GrayMiniBoldLabel = new GUIStyle(GrayMiniLabel)
							{
								fontStyle = FontStyle.Bold
							};
						}
						return _GrayMiniBoldLabel;
					}
				}

				static GUIStyle _GrayMiniLabelHighlighted;
				internal static GUIStyle GrayMiniLabelHighlighted
				{
					get
					{
						if (_GrayMiniLabelHighlighted == null)
						{
							_GrayMiniLabelHighlighted = new GUIStyle(GrayMiniLabel)
							{
								fontStyle = FontStyle.Bold
							};

							var textColor = EditorGUIUtility.isProSkin ? new Color(0.0f, 0.574f, 0.488f) : new Color(0.03f, 0.46f, 0.4f);
							_GrayMiniLabelHighlighted.normal.textColor = textColor;
						}
						return _GrayMiniLabelHighlighted;
					}
				}

				private static GUIStyle _GrayMiniFoldout;
				public static GUIStyle GrayMiniFoldout
				{
					get
					{
						if (_GrayMiniFoldout == null)
						{
							_GrayMiniFoldout = new GUIStyle(EditorStyles.foldout);

							var grayMiniLabel = GrayMiniLabel;
							_GrayMiniFoldout.alignment = grayMiniLabel.alignment;
							_GrayMiniFoldout.font = grayMiniLabel.font;
							_GrayMiniFoldout.fontStyle = grayMiniLabel.fontStyle;
							_GrayMiniFoldout.margin = grayMiniLabel.margin;
							_GrayMiniFoldout.padding = new RectOffset(16, 0, 0, 0);
							_GrayMiniFoldout.richText = grayMiniLabel.richText;
							_GrayMiniFoldout.stretchHeight = grayMiniLabel.stretchHeight;
							_GrayMiniFoldout.stretchWidth = grayMiniLabel.stretchWidth;
							_GrayMiniFoldout.fixedHeight = 0;
							_GrayMiniFoldout.fixedWidth = 0;

							_GrayMiniFoldout.normal.textColor = grayMiniLabel.normal.textColor;
							_GrayMiniFoldout.onNormal.textColor = grayMiniLabel.normal.textColor;
							_GrayMiniFoldout.focused.textColor = grayMiniLabel.normal.textColor;
							_GrayMiniFoldout.onFocused.textColor = grayMiniLabel.normal.textColor;
							_GrayMiniFoldout.hover.textColor = grayMiniLabel.normal.textColor;
							_GrayMiniFoldout.onHover.textColor = grayMiniLabel.normal.textColor;

							var gray = EditorGUIUtility.isProSkin ? 0.4f : 0.45f;
							var textColorActive = new Color(gray, gray, gray);
							_GrayMiniFoldout.active.textColor = textColorActive;
							_GrayMiniFoldout.onActive.textColor = textColorActive;

							_GrayMiniFoldout.normal.background = TCP2_GUI.GetCustomTexture("TCP2_FoldoutArrowRight");
							_GrayMiniFoldout.active.background = _GrayMiniFoldout.normal.background;
							_GrayMiniFoldout.focused.background = _GrayMiniFoldout.normal.background;
							_GrayMiniFoldout.hover.background = _GrayMiniFoldout.normal.background;

							_GrayMiniFoldout.onNormal.background = TCP2_GUI.GetCustomTexture("TCP2_FoldoutArrowDown");
							_GrayMiniFoldout.onActive.background = _GrayMiniFoldout.onNormal.background;
							_GrayMiniFoldout.onFocused.background = _GrayMiniFoldout.onNormal.background;
							_GrayMiniFoldout.onHover.background = _GrayMiniFoldout.onNormal.background;

						}
						return _GrayMiniFoldout;
					}
				}

				static GUIStyle _GrayMiniFoldoutHighlighted;
				internal static GUIStyle GrayMiniFoldoutHighlighted
				{
					get
					{
						if (_GrayMiniFoldoutHighlighted == null)
						{
							_GrayMiniFoldoutHighlighted = new GUIStyle(GrayMiniFoldout)
							{
								fontStyle = FontStyle.Bold,
							};

							var textColor = EditorGUIUtility.isProSkin ? new Color(0.0f, 0.574f, 0.488f) : new Color(0.03f, 0.46f, 0.4f);
							_GrayMiniFoldoutHighlighted.normal.textColor = textColor;
							_GrayMiniFoldoutHighlighted.active.textColor = textColor;
							_GrayMiniFoldoutHighlighted.focused.textColor = textColor;
							_GrayMiniFoldoutHighlighted.hover.textColor = textColor;
							_GrayMiniFoldoutHighlighted.onNormal.textColor = textColor;
							_GrayMiniFoldoutHighlighted.onActive.textColor = textColor;
							_GrayMiniFoldoutHighlighted.onFocused.textColor = textColor;
							_GrayMiniFoldoutHighlighted.onHover.textColor = textColor;
						}
						return _GrayMiniFoldoutHighlighted;
					}
				}

				static GUIStyle _GrayInlineLabel;
				internal static GUIStyle GrayInlineLabel
				{
					get
					{
						if(_GrayInlineLabel == null)
						{
							_GrayInlineLabel = new GUIStyle(GrayLabel);
						}
						return _GrayInlineLabel;
					}
				}

				static GUIStyle _LineStyle;
				internal static GUIStyle LineStyle
				{
					get
					{
						if(_LineStyle == null)
						{
							_LineStyle = new GUIStyle();
							_LineStyle.normal.background = EditorGUIUtility.whiteTexture;
							_LineStyle.stretchWidth = true;
						}

						return _LineStyle;
					}
				}

				// ----------------------------------------------------------------
				// SHURIKEN STYLES OVERRIDES

				const int shurikenFontSize = 10;

				static GUIStyle _ShurikenValue;
				internal static GUIStyle ShurikenValue
				{
					get
					{
						if (_ShurikenValue == null)
						{
							_ShurikenValue = new GUIStyle("ShurikenValue")
							{
								fontSize = shurikenFontSize
							};
						}
						return _ShurikenValue;
					}
				}
				
				static GUIStyle _ShurikenValueMonospace;
				internal static GUIStyle ShurikenValueMonospace
				{
					get
					{
						if (_ShurikenValueMonospace == null)
						{
							_ShurikenValueMonospace = new GUIStyle(ShurikenValue);
							var robotoMonospace = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath("64bf6567ab0269a47bfa164e4156cc4f"));
							if (robotoMonospace != null)
							{
								_ShurikenValueMonospace.font = robotoMonospace;
							}
						}
						return _ShurikenValueMonospace;
					}
				}

				static GUIStyle _ShurikenPopup;
				internal static GUIStyle ShurikenPopup
				{
					get
					{
						if (_ShurikenPopup == null)
						{
							_ShurikenPopup = new GUIStyle("ShurikenPopup")
							{
								fontSize = shurikenFontSize,
								clipping = TextClipping.Clip
							};
						}
						return _ShurikenPopup;
					}
				}

				static GUIStyle _ShurikenToggle;
				internal static GUIStyle ShurikenToggle
				{
					get
					{
						if (_ShurikenToggle == null)
						{
							_ShurikenToggle = new GUIStyle("ShurikenToggle")
							{
								fontSize = shurikenFontSize
							};
						}
						return _ShurikenToggle;
					}
				}

				static GUIStyle _ShurikenTextArea;
				internal static GUIStyle ShurikenTextArea
				{
					get
					{
						if (_ShurikenTextArea == null)
						{
							_ShurikenTextArea = new GUIStyle(ShurikenValue)
							{
								fixedHeight = 0,
								alignment = TextAnchor.UpperLeft
							};
						}
						return _ShurikenTextArea;
					}
				}

				static GUIStyle _ShurikenTextAreaMonospace;
				internal static GUIStyle ShurikenTextAreaMonospace
				{
					get
					{
						if (_ShurikenTextAreaMonospace == null)
						{
							_ShurikenTextAreaMonospace = new GUIStyle(ShurikenTextArea);
						}
						var robotoMonospace = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath("64bf6567ab0269a47bfa164e4156cc4f"));
						if (robotoMonospace != null)
						{
							_ShurikenTextAreaMonospace.font = robotoMonospace;
						}
						return _ShurikenTextAreaMonospace;
					}
				}

				static GUIStyle _ShurikenObjectField;
				internal static GUIStyle ShurikenObjectField
				{
					get
					{
						if (_ShurikenObjectField == null)
						{
							_ShurikenObjectField = new GUIStyle(EditorStyles.objectField)
							{
								fixedHeight = shurikenLineHeight,
								fontSize = shurikenFontSize
							};
						}
						return _ShurikenObjectField;
					}
				}

				// For custom channels selector
				static GUIStyle _ShurikenMiniButtonCustom;
				internal static GUIStyle ShurikenMiniButtonCustom
				{
					get
					{
						if (_ShurikenMiniButtonCustom == null)
						{
							_ShurikenMiniButtonCustom = new GUIStyle(EditorStyles.miniButton)
							{
								fixedWidth = 30,
								fixedHeight = 13,
								fontSize = shurikenFontSize,
								border = new RectOffset(2,2,2,2)
							};
							var margin = _ShurikenMiniButtonCustom.margin;
							margin.top -= 3;
							_ShurikenMiniButtonCustom.margin = margin;
						}
						return _ShurikenMiniButtonCustom;
					}
				}

				static GUIStyle _ShurikenMiniButtonFlexible;
				internal static GUIStyle ShurikenMiniButtonFlexible
				{
					get
					{
						if (_ShurikenMiniButtonFlexible == null)
						{
							_ShurikenMiniButtonFlexible = new GUIStyle(ShurikenMiniButtonCustom);
							_ShurikenMiniButtonFlexible.fixedWidth = 0;
						}
						return _ShurikenMiniButtonFlexible;
					}
				}

#if UNITY_2019_3_OR_NEWER
				const int MINI_BUTTON_FONT_SIZE = 10;
#endif
				
				static GUIStyle _MiniButtonLeft;
				internal static GUIStyle MiniButtonLeft
				{
					get
					{
#if !UNITY_2019_3_OR_NEWER
						return EditorStyles.miniButtonLeft;
#else
						if (_MiniButtonLeft == null)
						{
							_MiniButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft){ fontSize = MINI_BUTTON_FONT_SIZE };
						}
						return _MiniButtonLeft;
#endif
					}
				}
				static GUIStyle _MiniButtonMid;
				internal static GUIStyle MiniButtonMid
				{
					get
					{
#if !UNITY_2019_3_OR_NEWER
						return EditorStyles.miniButtonMid;
#else

						if (_MiniButtonMid == null)
						{
							_MiniButtonMid = new GUIStyle(EditorStyles.miniButtonMid){ fontSize = MINI_BUTTON_FONT_SIZE };
						}
						return _MiniButtonMid;
#endif
					}
				}

				static GUIStyle _MiniButtonRight;
				internal static GUIStyle MiniButtonRight
				{
					get
					{
#if !UNITY_2019_3_OR_NEWER
						return EditorStyles.miniButtonRight;
#else
						if (_MiniButtonRight == null)
						{
							_MiniButtonRight = new GUIStyle(EditorStyles.miniButtonRight){ fontSize = MINI_BUTTON_FONT_SIZE };
						}
						return _MiniButtonRight;
#endif
					}
				}
				
				static GUIStyle _MiniButton;
				internal static GUIStyle MiniButton
				{
					get
					{
#if !UNITY_2019_3_OR_NEWER
						return EditorStyles.miniButton;
#else
						if (_MiniButton == null)
						{
							_MiniButton = new GUIStyle(EditorStyles.miniButton){ fontSize = MINI_BUTTON_FONT_SIZE };
						}
						return _MiniButton;
#endif
					}
				}
			}

			//--------------------------------------------------------------------------------------------------------------------------------
			// GUILayout-like Methods

			public static Rect GetControlRect(GUIStyle style, float height = Styles.shurikenLineHeight, float width = 0f)
			{
				return GUILayoutUtility.GetRect(width, height, style);
			}

			static string RGBAOptions = "RGBA";
			public static char RGBASelector(char currentChannel)
			{
				return GenericSelector(RGBAOptions, currentChannel);
			}
			public static string RGBASelector(string currentChannel)
			{
				return RGBASelector(currentChannel[0]).ToString();
			}

			static string XYZWOptions = "XYZW";
			public static char XYZWSelector(char currentChannel)
			{
				return GenericSelector(XYZWOptions, currentChannel);
			}
			public static string XYZWSelector(string currentChannel)
			{
				return XYZWSelector(currentChannel[0]).ToString();
			}

			static string XYZOptions = "XYZ";
			public static char XYZSelector(char currentChannel)
			{
				return GenericSelector(XYZOptions, currentChannel);
			}
			public static string XYZSelector(string currentChannel)
			{
				return XYZSelector(currentChannel[0]).ToString();
			}

			public static string GenericSelector(string options, string current, float buttonWidth = 25)
			{
				return GenericSelector(options, current[0], buttonWidth).ToString();
			}
			public static char GenericSelector(string options, char current, float buttonWidth = 25)
			{
				var upperCurrent = char.ToUpperInvariant(current);
				var selected = options.IndexOf(upperCurrent);
				if(selected < 0) selected = 0;

#if !UNITY_2019_3_OR_NEWER
				float w = buttonWidth;
#else
				float w = Styles.ShurikenMiniButtonCustom.fixedWidth;
#endif
				for (var i = 0; i < options.Length; i++)
				{
#if !UNITY_2019_3_OR_NEWER
					var rect = GUILayoutUtility.GetRect(GUIContent.none, TCP2_GUI.ShurikenMiniButton, GUILayout.Height(15), GUILayout.Width(w));
					rect.height = 12;
					rect.y -= 1; //small hack to align with the shuriken ui components

					//button style
					var style = TCP2_GUI.ShurikenMiniButton;
					if(options.Length == 2)
						style = (i == 0) ? TCP2_GUI.ShurikenMiniButtonLeft : TCP2_GUI.ShurikenMiniButtonRight;
					else if(options.Length > 1)
						style = (i == 0) ? TCP2_GUI.ShurikenMiniButtonLeft : (i == (options.Length-1) ? TCP2_GUI.ShurikenMiniButtonRight : TCP2_GUI.ShurikenMiniButtonMid);
#else
					var rect = GetControlRect(Styles.ShurikenMiniButtonCustom, width: w);
					var style = Styles.ShurikenMiniButtonCustom;
#endif

					if (GUI.Toggle(rect, selected == i, options[i].ToString(), style))
					{
						selected = i;
					}
				}
				return options[selected];
			}

			public static string RGBASwizzle(string selected, int channelsCount)
			{
				return GenericSwizzle(selected, channelsCount, "RGBA");
			}

			public static string XYZWSwizzle(string selected, int channelsCount)
			{
				return GenericSwizzle(selected, channelsCount, "XYZW");
			}

			public static string XYZSwizzle(string selected, int channelsCount)
			{
				return GenericSwizzle(selected, channelsCount, "XYZ");
			}

			public static string GenericSwizzle(string selected, int channelsCount, string options, float width = 50, bool showAvailableChannels = true)
			{
				EditorGUI.BeginChangeCheck();
				Rect rect = GetControlRect(Styles.ShurikenValue, width: width);
				var newSelected = EditorGUI.DelayedTextField(rect, selected, Styles.ShurikenValue);
				if(EditorGUI.EndChangeCheck())
				{
					// empty string
					if (newSelected.Length == 0)
					{
						return selected;
					}
					
					// not enough characters
					if (newSelected.Length < channelsCount)
					{
						// expand the last valid character
						char lastChar = newSelected[newSelected.Length - 1];
						newSelected += new string(lastChar, channelsCount - newSelected.Length);
					}

					// remove extra characters
					if (newSelected.Length > channelsCount)
					{
						newSelected = newSelected.Substring(0, channelsCount);
					}

					newSelected = newSelected.ToUpperInvariant();
					foreach(var c in newSelected)
					{
						if (!options.Contains(c.ToString()))
						{
							return selected;
						}
					}
				}

				if (showAvailableChannels)
				{
					GUILayout.Space(4);
					GUILayout.Label(string.Format("(available channels: {0})", options), Styles.GrayMiniLabel);
				}

				return newSelected.ToUpperInvariant();
			}

			static int foldoutHash = "TCP2 Foldout".GetHashCode();
			public static bool Foldout(bool foldout, string label, string tooltip = null, bool highlighted = false)
			{
				return Foldout(foldout, TCP2_GUI.TempContent(label, tooltip), highlighted);
			}
			public static bool Foldout(bool foldout, string label, bool highlighted)
			{
				return Foldout(foldout, TCP2_GUI.TempContent(label), highlighted);
			}
			public static bool Foldout(bool foldout, GUIContent label, bool highlighted = false, float width = 130)
			{
				GUILayout.Space(Indent);

				var rect = GUILayoutUtility.GetRect(label, highlighted ? Styles.GrayMiniLabelHighlighted : Styles.GrayMiniLabel, GUILayout.Height(Styles.shurikenLineHeight), GUILayout.Width(width));
				bool hover = rect.Contains(Event.current.mousePosition);

				if (hover)
				{
					EditorGUI.DrawRect(rect, Color.black * 0.1f);
				}

				label.text = string.Format("{0} {1}", foldout ? "▼" : "►", label.text);
				InlineLabel(rect, label, highlighted);

				int controlId = GUIUtility.GetControlID(foldoutHash, FocusType.Keyboard, rect);

				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && hover)
				{
					Event.current.Use();
					GUIUtility.hotControl = controlId;
				}

				if (GUIUtility.hotControl == controlId && Event.current.type == EventType.MouseUp && Event.current.button == 0 && hover)
				{
					Event.current.Use();
					GUI.changed = true;
					GUIUtility.hotControl = 0;
					return !foldout;
				}
				return foldout;
			}

			public static Rect InlineLabel(string label, string tooltip = null, bool highlight = false)
			{
				return InlineLabel(TCP2_GUI.TempContent(label, tooltip), highlight);
			}
			public static Rect InlineLabel(string label, bool highlight)
			{
				return InlineLabel(TCP2_GUI.TempContent(label), highlight);
			}
			public static Rect InlineLabel(GUIContent label, bool highlight = false, float width = 130)
			{
				GUILayout.Space(Indent);
				var rect = GUILayoutUtility.GetRect(label, highlight ? Styles.GrayMiniLabelHighlighted : Styles.GrayMiniLabel, GUILayout.Height(Styles.shurikenLineHeight), GUILayout.Width(width));
#if !UNITY_2019_3_OR_NEWER
				rect.y -= 2;
#endif
				GUI.Label(rect, label, highlight ? Styles.GrayMiniLabelHighlighted : Styles.GrayMiniLabel);
				return rect;
			}
			public static Rect InlineLabel(Rect rect, GUIContent label, bool highlight = false, float width = 130)
			{
				GUILayout.Space(Indent);
#if !UNITY_2019_3_OR_NEWER
				rect.y -= 2;
#endif
				GUI.Label(rect, label, highlight ? Styles.GrayMiniLabelHighlighted : Styles.GrayMiniLabel);
				return rect;
			}

			public static void InlineHeader(string label, string tooltip = null)
			{
				InlineHeader(TCP2_GUI.TempContent(label, tooltip));
			}
			public static void InlineHeader(GUIContent label)
			{
				GUILayout.Space(Indent);
				var rect = GUILayoutUtility.GetRect(label, Styles.GrayMiniBoldLabel);
				rect.y -= 2;
				GUI.Label(rect, label, Styles.GrayMiniBoldLabel);
			}

			//Property fields for Shader Property: UI is harmonized and easy to update
			public static Enum EnumPopup(Enum enm)
			{
				Rect rect = GetControlRect(Styles.ShurikenPopup);
				return EditorGUI.EnumPopup(rect, enm, Styles.ShurikenPopup);
			}
			public static int Popup(int index, string[] values)
			{
				Rect rect = GetControlRect(Styles.ShurikenPopup);
				return EditorGUI.Popup(rect, index, values, Styles.ShurikenPopup);
			}
			public static string TextField(string str, bool delayed = false, bool monospace = false)
			{
				Rect rect = GetControlRect(monospace ? Styles.ShurikenValueMonospace : Styles.ShurikenValue);
				return TextField(rect, str, delayed, monospace);
			}
			public static string TextField(Rect rect, string str, bool delayed = false, bool monospace = false)
			{
				var style = monospace ? Styles.ShurikenValueMonospace : Styles.ShurikenValue;
				if (delayed)
				{
					return EditorGUI.DelayedTextField(rect, GUIContent.none, str, style);
				}
				else
				{
					return EditorGUI.TextField(rect, GUIContent.none, str, style);
				}
			}
			
			static readonly List<char> ValidVariableCharacters = new List<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789".ToCharArray());
			public static string TextFieldShaderVariable(Rect rect, string str)
			{
				//special version with that only accepts alphanumerical and underscore
				var result = TextField(rect, str, monospace: true);
				for (var i = result.Length - 1; i >= 0; i--)
				{
					if (!ValidVariableCharacters.Contains(result[i]))
					{
						result = result.Remove(i, 1);
					}
				}

				return result;
			}

			public static string TextArea(string str, float height = 0, bool monospace = false)
			{
				var style = monospace ? Styles.ShurikenTextAreaMonospace : Styles.ShurikenTextArea;
				return height > 0 ?
					EditorGUI.TextArea(GetControlRect(Styles.ShurikenTextArea, height), str, style) :
					EditorGUI.TextArea(GetControlRect(Styles.ShurikenTextArea), str, style);
			}
			public static T ObjectField<T>(T obj) where T : UnityEngine.Object
			{
				//return DrawProObjectField<T>(obj);
				Rect rect = GetControlRect(Styles.ShurikenObjectField);
				return (T)EditorGUI.ObjectField(rect, GUIContent.none, obj, typeof(T), false);
			}

			public static T DrawProObjectField<T>(T obj, params GUILayoutOption[] options) where T : UnityEngine.Object
			{
				int pickerID = "ShurikenObjectField".GetHashCode();

				var rect = EditorGUILayout.GetControlRect(false, Styles.shurikenLineHeight, Styles.ShurikenValue, options);
				var btnRect = rect;
				btnRect.width = 20;
				rect.xMax -= btnRect.width;
				btnRect.x += rect.width;

				GUI.Label(rect, TCP2_GUI.TempContent(obj != null ? obj.name : "None (" + typeof(T).ToString() + ")"), Styles.ShurikenValue);
				if (GUI.Button(btnRect, "...", "MiniToolbarButton"))
				{
					EditorGUIUtility.ShowObjectPicker<T>(obj, false, "", pickerID);
				}
				if (Event.current.commandName == "ObjectSelectorUpdated")
				{
					if (EditorGUIUtility.GetObjectPickerControlID() == pickerID)
					{
						obj = EditorGUIUtility.GetObjectPickerObject() as T;
					}
				}
				return obj;
			}

			public static bool ButtonPopup(string label)
			{
				return ButtonPopup(TCP2_GUI.TempContent(label));
			}
			
			public static bool ButtonPopup(GUIContent content)
			{
				return GUILayout.Button(content, Styles.ShurikenPopup, GUILayout.MinWidth(248), GUILayout.MinHeight(Styles.shurikenLineHeight));
			}
			public static int IntField(int value)
			{
				Rect rect = GetControlRect(Styles.ShurikenValue);
				return EditorGUI.IntField(rect, value, Styles.ShurikenValue);
			}
			public static int IntField(int value, int min, int max)
			{
				return Mathf.Clamp(IntField(value), min, max);
			}
			public static float FloatField(float value)
			{
				Rect rect = GetControlRect(Styles.ShurikenValue);
				return EditorGUI.FloatField(rect, value, Styles.ShurikenValue);
			}
			public static Vector2 Vector2Field(Vector2 v2) { return VectorFieldCustomStyle(v2, 2); }
			public static Vector3 Vector3Field(Vector3 v3) { return VectorFieldCustomStyle(v3, 3); }
			public static Vector4 Vector4Field(Vector4 v4) { return VectorFieldCustomStyle(v4, 4); }
			public static Color ColorField(Color c, bool alpha, bool hdr = false)
			{
				Rect rect = GetControlRect(Styles.ShurikenValue);
				Color color;
				if (EditorGUIUtility.isProSkin)
				{
					color = rect.Contains(Event.current.mousePosition) ? Styles.colorFieldBorderColorHoverPro : Styles.colorFieldBorderColorPro;
				}
				else
				{
					color = rect.Contains(Event.current.mousePosition) ? Styles.colorFieldBorderColorHover : Styles.colorFieldBorderColor;
				}
				EditorGUI.DrawRect(rect, color);

				rect.xMin++;
				rect.xMax--;
				rect.yMin++;
				rect.yMax--;

#if UNITY_2018_1_OR_NEWER
				return EditorGUI.ColorField(rect, GUIContent.none, c, false, alpha, hdr);
#else
				return EditorGUI.ColorField(rect, GUIContent.none, c, false, alpha, hdr, new ColorPickerHDRConfig(0f, 99f, 0.01010101f, 3f));
#endif
			}
			public static bool Toggle(bool toggle)
			{
				var rect = EditorGUILayout.GetControlRect(false, Styles.shurikenLineHeight, Styles.ShurikenToggle, GUILayout.MinWidth(248));
				return EditorGUI.Toggle(rect, GUIContent.none, toggle, Styles.ShurikenToggle);
			}

			static Vector4 VectorFieldCustomStyle(Vector4 vec, int channels)
			{
				EditorGUILayout.BeginHorizontal();
				if(channels > 0)
				{
					GUILayout.Label("x", Styles.GrayMiniLabel, GUILayout.ExpandWidth(false));
					vec.x = FloatField(vec.x);
				}
				if(channels > 1)
				{
					GUILayout.Label("y", Styles.GrayMiniLabel, GUILayout.ExpandWidth(false));
					vec.y = FloatField(vec.y);
				}
				if(channels > 2)
				{
					GUILayout.Label("z", Styles.GrayMiniLabel, GUILayout.ExpandWidth(false));
					vec.z = FloatField(vec.z);
				}
				if(channels > 3)
				{
					GUILayout.Label("w", Styles.GrayMiniLabel, GUILayout.ExpandWidth(false));
					vec.w = FloatField(vec.w);
				}
				EditorGUILayout.EndHorizontal();

				return vec;
			}

			public static void DrawLine()
			{
				var c = EditorGUIUtility.isProSkin ? new Color(0.15f, 0.15f, 0.15f, 1.0f) : new Color(0.5f, 0.5f, 0.5f, 1.0f);
				DrawLine(c);
			}

			public static void DrawLine(Color color)
			{
				var rect = GUILayoutUtility.GetRect(GUIContent.none, Styles.LineStyle, GUILayout.Height(1));
				if(Event.current.type == EventType.Repaint)
				{
					var guiColor = GUI.color;
					GUI.color *= color;
					Styles.LineStyle.Draw(rect, GUIContent.none, "line".GetHashCode());
					GUI.color = guiColor;
				}
			}

			static readonly GUIContent gcInspectorLock = EditorGUIUtility.IconContent("InspectorLock");
			public static void DrawLockIcon(Color color)
			{
				if (gcInspectorLock != null)
				{
					var c = GUI.color;
					GUI.color *= color;
					var lockIconRect = EditorGUILayout.GetControlRect(false, 14, GUILayout.Width(14));
					GUI.DrawTexture(lockIconRect, gcInspectorLock.image);
					GUI.color = c;
				}
			}

			public static class Utils
			{
				public static string RemoveWhitespaces(string input)
				{
					return input.Replace(" ", "");
				}

				public static string VariableNameToReadable(string input)
				{
					string output = "";

					int start = 0;
					if (input[0] == '_') start = 1;

					bool lastWasLowercase = false;
					for(int i = start; i < input.Length; i++)
					{
						if ((Char.IsUpper(input[i]) || Char.IsDigit(input[i])) && lastWasLowercase && output.Length > 0)
						{
							output += " ";
						}

						char c = input[i];
						if (c == '_') c = ' ';

						output += c;
						lastWasLowercase = Char.IsLower(input[i]);
					}

					return output;
				}
			}

			public struct IndentedLine : IDisposable
			{
				public IndentedLine(float indent = -1)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(indent < 0 ? Indent : indent);
				}

				public void Dispose()
				{
					GUILayout.EndHorizontal();
				}
			}
		}
	}
}