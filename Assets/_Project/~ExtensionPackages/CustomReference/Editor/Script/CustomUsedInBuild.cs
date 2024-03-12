using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace custom.find.reference
{
	public class CustomUsedInBuild : IRefDraw
	{
		private readonly CustomTreeUI.GroupDrawer groupDrawer;

		private bool dirty;
		private readonly CustomRefDrawer drawer;
		internal Dictionary<string, CustomRef> refs;

		public CustomUsedInBuild(IWindow window)
		{
			this.window = window;
			drawer = new CustomRefDrawer(window);
			dirty = true;
			drawer.SetDirty();
		}

		public IWindow window { get; set; }


		public int ElementCount()
		{
			return refs == null ? 0 : refs.Count;
		}

		public bool Draw(Rect rect)
		{
			if (dirty)
			{
				RefreshView();
			}

			return drawer.Draw(rect);
		}

		public bool DrawLayout()
		{
			//Debug.Log("draw");
			if (dirty)
			{
				RefreshView();
			}

			return drawer.DrawLayout();
		}

		public void SetDirty()
		{
			dirty = true;
			drawer.SetDirty();
		}

		public void RefreshView()
		{
			var scenes = new HashSet<string>();
			// string[] scenes = new string[sceneCount];
			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				if (scene == null)
				{
					continue;
				}

				if (scene.enabled == false)
				{
					continue;
				}

				string sce = AssetDatabase.AssetPathToGUID(scene.path);
				
				if (scenes.Contains(sce))
				{
					continue;
				}

				scenes.Add(sce);
			}

			refs = CustomRef.FindUsage(scenes.ToArray());

			foreach (string VARIABLE in scenes)
			{
				CustomRef asset = null;
				if (!refs.TryGetValue(VARIABLE, out asset))
				{
					continue;
				}

				
				asset.depth = 1;
			}

			List<CustomAsset> list = CustomCache.Api.AssetList;
			int count = list.Count;
			
			for (var i = 0; i < count; i++)
			{
				CustomAsset item = list[i];
				if (item.inEditor) continue;
				if (item.inPlugins)
				{
					if (item.type == CustomAssetType.SCENE) continue;
				}
				
				if (item.inResources || item.inStreamingAsset || item.inPlugins)
				{
					if (refs.ContainsKey(item.guid))
					{
						continue;
					}
					
					refs.Add(item.guid, new CustomRef(0, 1, item, null));
				}
			}

			// remove ignored items
			var vals = refs.Values.ToArray();
			foreach (var item in vals)
			{
				foreach (var ig in CustomSetting.s.listIgnore)
				{
					if (!item.asset.assetPath.StartsWith(ig)) continue;
					refs.Remove(item.asset.guid);
					//Debug.Log("Remove: " + item.asset.assetPath + "\n" + ig);
					break;
				}
			}
			
			drawer.SetRefs(refs);
			dirty = false;
		}

		internal void RefreshSort()
		{
			drawer.RefreshSort();
		}
	}
}