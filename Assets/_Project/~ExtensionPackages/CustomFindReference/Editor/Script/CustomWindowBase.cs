using UnityEditor;
using UnityEngine;

namespace custom.find.reference
{
	public interface IWindow
	{
		bool WillRepaint { get; set; }
		void Repaint();
		void OnSelectionChange();
	}

	internal interface IRefDraw
	{
		IWindow window { get; }
		int ElementCount();
		bool DrawLayout();
		bool Draw(Rect rect);
	}
	
	public abstract class CustomWindowBase : EditorWindow, IWindow
	{
		public bool WillRepaint { get; set; }
		protected bool showFilter, showIgnore;

		//[NonSerialized] protected bool lockSelection;
		//[NonSerialized] internal List<CustomAsset> Selected;

		public static bool isNoticeIgnore;

		public void AddItemsToMenu(GenericMenu menu)
		{
			CustomCache api = CustomCache.Api;
			if (api == null)
			{
				return;
			}

			menu.AddDisabledItem(new GUIContent("Custom Reference - v1.0.0"));
			menu.AddSeparator(string.Empty);

			menu.AddItem(new GUIContent("Enable"), !api.disabled, () => { api.disabled = !api.disabled; });
			menu.AddItem(new GUIContent("Refresh"), false, () =>
			{
				//CustomAsset.lastRefreshTS = Time.realtimeSinceStartup;
				CustomCache.Api.Check4Changes(true);
				CustomSceneCache.Api.SetDirty();
			});

#if CustomDEV
            menu.AddItem(new GUIContent("Refresh Usage"), false, () => CustomCache.Api.Check4Usage());
            menu.AddItem(new GUIContent("Refresh Selected"), false, ()=> CustomCache.Api.RefreshSelection());
            menu.AddItem(new GUIContent("Clear Cache"), false, () => CustomCache.Api.Clear());
#endif
		}

		public abstract void OnSelectionChange();
		protected abstract void OnGUI();
        
#if UNITY_2018_OR_NEWER
        protected void OnSceneChanged(Scene arg0, Scene arg1)
        {
            if (IsFocusingFindInScene || IsFocusingSceneToAsset || IsFocusingSceneInScene)
            {
                OnSelectionChange();
            }
        }
#endif

		protected bool DrawEnable()
		{
			CustomCache api = CustomCache.Api;
			if (api == null)
			{
				return false;
			}

			bool v = api.disabled;
			if (v)
			{
				EditorGUILayout.HelpBox("Find References is disabled!", MessageType.Warning);
				
				if (GUILayout.Button("Enable"))
				{
					api.disabled = !api.disabled;
					Repaint();
				}

				return !api.disabled;
			}
			
			return !api.disabled;
		}

	}
}