using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace custom.find.reference
{
	public class DrawCallback
	{
		public Action BeforeDraw;
		public Action AfterDraw;
	}
	
	
	public class CustomTabView
	{
		public int current;
		public GUIContent[] labels;
		public IWindow window;
		public Action onTabChange;
		public DrawCallback callback;
		public bool canDeselectAll; // can there be no active tabs
		
		public CustomTabView(IWindow w, bool canDeselectAll)
		{
			this.window = w;
			this.canDeselectAll = canDeselectAll;
		}
		
		public bool DrawLayout()
		{
			bool result = false;
			
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				if (callback != null && callback.BeforeDraw != null) callback.BeforeDraw();
				
				for (var i = 0; i < labels.Length; i++)
				{
					var isActive = (i == current);
					
					var lb = labels[i];
					var clicked = (lb.image != null)
						? GUI.ToolbarToggle(ref isActive, lb.image, Vector2.zero, lb.tooltip)
						: GUI.Toggle(ref isActive, lb, EditorStyles.toolbarButton);
								
					if (!clicked) continue;
					
					current = (!isActive && canDeselectAll) ? -1 : i;
					result = true;
				
					if (onTabChange != null) onTabChange();
					if (window == null) continue;
					window.OnSelectionChange(); // force refresh tabs
					window.WillRepaint = true;
				}
				
				if (callback != null && callback.AfterDraw != null) callback.AfterDraw();
			}
			GUILayout.EndHorizontal();

			return result;
		}

		public static CustomTabView FromEnum(Type enumType, IWindow w, bool canDeselectAll = false)
		{
			var values = Enum.GetValues(enumType);
			var labels = new List<GUIContent>();

			foreach (var item in values)
			{
				labels.Add(new GUIContent(item.ToString()));
			}

			return new CustomTabView( w, canDeselectAll) {current = 0, labels = labels.ToArray()};
		}
		public static GUIContent GetGUIContent(object tex)
		{
			if (tex is GUIContent) return (GUIContent) tex;
			if (tex is Texture) return new GUIContent((Texture)tex);
			if (tex is string) return new GUIContent((string)tex);
			return GUIContent.none;
		}
		
		public static CustomTabView Create(IWindow w, bool canDeselectAll = false, params object[] titles)
		{
			var labels = new List<GUIContent>();
			foreach (var item in titles)
			{
				labels.Add(GetGUIContent(item));
			}
			return new CustomTabView(w, canDeselectAll) {current = 0, labels = labels.ToArray()};
		}
	}
}