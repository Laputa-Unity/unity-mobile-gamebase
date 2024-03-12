using System.Globalization;
//#define CustomDEBUG_BRACE_LEVEL
//#define CustomDEBUG_SYMBOL
//#define CustomDEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


#if CustomADDRESSABLE
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;
#endif


using UnityObject = UnityEngine.Object;

namespace custom.find.reference
{
	public enum CustomAssetType
	{
		UNKNOWN,
		FOLDER,
		SCRIPT,
		SCENE,
		DLL,
		REFERENCABLE,
		BINARY_ASSET,
		MODEL,
		TERRAIN,
		NON_READABLE
	}

	public enum CustomAssetState
	{
		NEW,
		CACHE,
		MISSING
	}

	[Serializable]
	public class CustomAsset
	{
		// ------------------------------ CONSTANTS ---------------------------

		private static readonly HashSet<string> SCRIPT_EXTENSIONS = new HashSet<string>
		{
			".cs", ".js", ".boo", ".h", ".java", ".cpp", ".m", ".mm"
		};

		private static readonly HashSet<string> REFERENCABLE_EXTENSIONS = new HashSet<string>
		{
			".anim", ".controller", ".mat", ".unity", ".guiskin", ".prefab",
			".overridecontroller", ".mask", ".rendertexture", ".cubemap", ".flare",
			".mat", ".prefab", ".physicsmaterial", ".fontsettings", ".asset", ".prefs", ".spriteatlas"
		};
		
		private static readonly Dictionary<int, Type> HashClasses = new Dictionary<int, Type>();
		internal static Dictionary<string, GUIContent> cacheImage = new Dictionary<string, GUIContent>();
		
		private bool _isExcluded;
		private Dictionary<string, HashSet<int>> _UseGUIDs;
		private float excludeTS;
		
		public static float ignoreTS;
		

		// ----------------------------- DRAW  ---------------------------------------
		[NonSerialized] private GUIContent fileSizeText;
		
		// ----------------------------- DRAW  ---------------------------------------
		
		[SerializeField] public string guid;
		
		// easy to recalculate: will not cache
		[NonSerialized] private bool m_pathLoaded;
		[NonSerialized] private string m_assetFolder;
		[NonSerialized] private string m_assetName;
		[NonSerialized] private string m_assetPath;
		[NonSerialized] private string m_extension;
		[NonSerialized] private bool m_inEditor;
		[NonSerialized] private bool m_inPlugins;
		[NonSerialized] private bool m_inResources;
		[NonSerialized] private bool m_inStreamingAsset;
		[NonSerialized] private bool m_isAssetFile;
		
		// Need to read FileInfo: soft-cache (always re-read when needed)
		[SerializeField] public CustomAssetType type;
		[SerializeField] private string m_fileInfoHash;
		[SerializeField] private string m_assetbundle;
		[SerializeField] private string m_addressable;
		
		[SerializeField] private string m_atlas;
		[SerializeField] private long m_fileSize;
		
		[SerializeField] private int m_assetChangeTS;			// Realtime when asset changed (trigger by import asset operation)
		[SerializeField] private int m_fileInfoReadTS;			// Realtime when asset being read
		
		[SerializeField] private int m_fileWriteTS;		// file's lastModification (file content + meta)
		[SerializeField] private int m_cachefileWriteTS;	// file's lastModification at the time the content being read
		
		[SerializeField] internal int refreshStamp; // use to check if asset has been deleted (refreshStamp not updated)
		
		
		// Do not cache
		[NonSerialized] internal CustomAssetState state;
		internal Dictionary<string, CustomAsset> UsedByMap = new Dictionary<string, CustomAsset>();
		internal HashSet<int> HashUsedByClassesIds = new HashSet<int>();
		[SerializeField] private List<Classes> UseGUIDsList = new List<Classes>();
		
		public CustomAsset(string guid)
		{
			this.guid = guid;
			type = CustomAssetType.UNKNOWN;
		}
		
		// ----------------------- PATH INFO ------------------------
		public void LoadPathInfo()
		{
			if (m_pathLoaded) return;
			m_pathLoaded = true;
			
			m_assetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(assetPath))
			{
				state = CustomAssetState.MISSING;
				return;
			}
			
#if CustomDEBUG
			Debug.LogWarning("Refreshing ... " + loadInfoTS + ":" + AssetDatabase.GUIDToAssetPath(guid));
			if (!m_assetPath.StartsWith("Assets"))
			{
				Debug.Log("LoadAssetInfo: " + m_assetPath);
			}
#endif
			CustomUnity.SplitPath(m_assetPath, out m_assetName, out m_extension, out m_assetFolder);

			if (m_assetFolder.StartsWith("Assets/"))
			{
				m_assetFolder = m_assetFolder.Substring(7);
			}
			else if (!CustomUnity.StringStartsWith(m_assetPath, "Packages/", "Project Settings/", "Library/"))
			{
				m_assetFolder = "built-in/";
			}
			
			m_inEditor = m_assetPath.Contains("/Editor/") || m_assetPath.Contains("/Editor Default Resources/");
			m_inResources = m_assetPath.Contains("/Resources/");
			m_inStreamingAsset = m_assetPath.Contains("/StreamingAssets/");
			m_inPlugins = m_assetPath.Contains("/Plugins/");
			m_isAssetFile = m_assetPath.EndsWith(".asset", StringComparison.Ordinal);
		}
		
		public string assetName { get {
			LoadPathInfo();
			return m_assetName;
		} }

		public string assetPath
		{
			get
			{
				if (!string.IsNullOrEmpty(m_assetPath)) return m_assetPath;
				m_assetPath = AssetDatabase.GUIDToAssetPath(guid);
				if (string.IsNullOrEmpty(m_assetPath))
				{
					state = CustomAssetState.MISSING;
				}
				return m_assetPath;
			}
		}

		public string parentFolderPath { get { LoadPathInfo(); return m_assetFolder; } }
		public string assetFolder { get { LoadPathInfo(); return m_assetFolder; } }
		public string extension { get { LoadPathInfo(); return m_extension; } }
		
		public bool inEditor { get { LoadPathInfo(); return m_inEditor; } }
		public bool inPlugins { get { LoadPathInfo(); return m_inPlugins; } }
		public bool inResources { get { LoadPathInfo(); return m_inResources; } }
		public bool inStreamingAsset { get { LoadPathInfo(); return m_inStreamingAsset; } }
		
		// ----------------------- TYPE INFO ------------------------
		
		internal bool IsFolder { get { return type == CustomAssetType.FOLDER; } }
		internal bool IsScript { get { return type == CustomAssetType.SCRIPT; } }
		internal bool IsMissing { get { return state == CustomAssetState.MISSING; } }
		internal bool IsReferencable { get {
			return type == CustomAssetType.REFERENCABLE ||
			       type == CustomAssetType.SCENE;
		}}
		internal bool IsBinaryAsset { get {
			return type == CustomAssetType.BINARY_ASSET ||
			       type == CustomAssetType.MODEL ||
			       type == CustomAssetType.TERRAIN;
		}}
		
		// ----------------------- PATH INFO ------------------------
		public bool fileInfoDirty { get { return (type == CustomAssetType.UNKNOWN) || (m_fileInfoReadTS <= m_assetChangeTS); } }
		public bool fileContentDirty { get { return m_fileWriteTS != m_cachefileWriteTS; } }

		public bool isDirty
		{
			get { return fileInfoDirty || fileContentDirty; }
		}
		
		bool ExistOnDisk()
		{
			if (IsMissing) return false; // asset not exist - no need to check FileSystem!
			if (type == CustomAssetType.FOLDER || type == CustomAssetType.UNKNOWN)
			{
				if (Directory.Exists(m_assetPath))
				{
					if (type == CustomAssetType.UNKNOWN) type = CustomAssetType.FOLDER;
					return true;
				}

				if (type == CustomAssetType.FOLDER) return false;
			}
			
			// must be file here
			if (!File.Exists(m_assetPath)) return false;
			
			if (type == CustomAssetType.UNKNOWN) GuessAssetType();
			return true;
		}
		
		internal void LoadFileInfo()
		{
			if (!fileInfoDirty) return;
			if (string.IsNullOrEmpty(m_assetPath)) LoadPathInfo(); // always reload Path Info
			
			//Debug.Log("--> Read: " + assetPath + " --> " + m_fileInfoReadTS + "<" + m_assetChangeTS);
			m_fileInfoReadTS = CustomUnity.Epoch(DateTime.Now);
			
			if (IsMissing)
			{
				Debug.LogWarning("Should never be here! - missing files can not trigger LoadFileInfo()");
				return;
			}
			
			if (!ExistOnDisk())
			{
				state = CustomAssetState.MISSING;
				return;
			}
			
			if (type == CustomAssetType.FOLDER) return; // nothing to read
			
			var assetType = AssetDatabase.GetMainAssetTypeAtPath(m_assetPath);
			if (assetType == typeof(CustomCache)) return;
			
			var info = new FileInfo(m_assetPath);
			m_fileSize = info.Length;
			m_fileInfoHash = info.Length + info.Extension;
			m_addressable = CustomUnity.GetAddressable(guid);
			//if (!string.IsNullOrEmpty(m_addressable)) Debug.LogWarning(guid + " --> " + m_addressable);
			m_assetbundle = AssetDatabase.GetImplicitAssetBundleName(m_assetPath);
			
			if (assetType == typeof(Texture2D))
			{
				var importer = AssetImporter.GetAtPath(m_assetPath);
				if (importer is TextureImporter)
				{
					var tImporter = importer as TextureImporter;
					if (tImporter.qualifiesForSpritePacking)
					{
						m_atlas = tImporter.spritePackingTag;
					}
				}
			}
			
			// check if file content changed
			var metaInfo = new FileInfo(m_assetPath + ".meta");
			var assetTime = CustomUnity.Epoch(info.LastWriteTime);
			var metaTime = CustomUnity.Epoch(metaInfo.LastWriteTime);
			
			// update fileChangeTimeStamp
			m_fileWriteTS = Mathf.Max(metaTime, assetTime);
		}
		
		internal string fileInfoHash { get { LoadFileInfo(); return m_fileInfoHash; } }
		internal long fileSize { get { LoadFileInfo(); return m_fileSize; } }
		
		public string AtlasName { get { LoadFileInfo(); return m_atlas;}}
		public string AssetBundleName { get { LoadFileInfo(); return m_assetbundle;}}

		public string AddressableName { get { LoadFileInfo(); return m_addressable; }}
		

		

		

		


		public Dictionary<string, HashSet<int>> UseGUIDs
		{
			get
			{
				if (_UseGUIDs != null)
				{
					return _UseGUIDs;
				}

				_UseGUIDs = new Dictionary<string, HashSet<int>>(UseGUIDsList.Count);
				for (var i = 0; i < UseGUIDsList.Count; i++)
				{
					string guid = UseGUIDsList[i].guid;
					if (_UseGUIDs.ContainsKey(guid))
					{
						for (var j = 0; j < UseGUIDsList[i].ids.Count; j++)
						{
							int val = UseGUIDsList[i].ids[j];
							if (_UseGUIDs[guid].Contains(val))
							{
								continue;
							}

							_UseGUIDs[guid].Add(UseGUIDsList[i].ids[j]);
						}
					}
					else
					{
						_UseGUIDs.Add(guid, new HashSet<int>(UseGUIDsList[i].ids));
					}
				}

				return _UseGUIDs;
			}
		}

		// ------------------------------- GETTERS -----------------------------


		internal bool IsExcluded
		{
			get
			{
				if (excludeTS >= ignoreTS)
				{
					return _isExcluded;
				}

				excludeTS = ignoreTS;
				_isExcluded = false;

				HashSet<string> h = CustomSetting.IgnoreAsset;
				foreach (string item in CustomSetting.IgnoreAsset)
				{
					if (m_assetPath.StartsWith(item, false, CultureInfo.InvariantCulture))
					{
						_isExcluded = true;
						break;
					}
				}

				return _isExcluded;
			}
		}
		
		public void AddUsedBy(string guid, CustomAsset asset)
		{
			if (UsedByMap.ContainsKey(guid))
			{
				return;
			}

            if(guid == this.guid)
            {
                //Debug.LogWarning("self used");
                return;
            }


			UsedByMap.Add(guid, asset);
			HashSet<int> output;
			if (HashUsedByClassesIds == null)
			{
				HashUsedByClassesIds = new HashSet<int>();
			}

			if (asset.UseGUIDs.TryGetValue(this.guid, out output))
			{
				foreach (int item in output)
				{
					HashUsedByClassesIds.Add(item);
				}
			}

			// int classId = HashUseByClassesIds    
		}

		public int UsageCount()
		{
			return UsedByMap.Count;
		}

		public override string ToString()
		{
			return string.Format("CustomAsset[{0}]", m_assetName);
		}

		//--------------------------------- STATIC ----------------------------

		internal static bool IsValidGUID(string guid)
		{
			return AssetDatabase.GUIDToAssetPath(guid) != CustomCache.CachePath; // just skip CustomCache asset
		}

		internal void MarkAsDirty(bool isMoved = true, bool force = false)
		{
			if (isMoved)
			{
				var newPath = AssetDatabase.GUIDToAssetPath(guid);
				if (newPath != m_assetPath)
				{
					m_pathLoaded = false;
					m_assetPath = newPath;
				}
			}
			
			state = CustomAssetState.CACHE;
			m_assetChangeTS = CustomUnity.Epoch(DateTime.Now); // re-read FileInfo
			if (force) m_cachefileWriteTS = 0;
		}
		
		// --------------------------------- APIs ------------------------------

		internal void GuessAssetType()
		{
			if (SCRIPT_EXTENSIONS.Contains(m_extension))
			{
				type = CustomAssetType.SCRIPT;
			}
			else if (REFERENCABLE_EXTENSIONS.Contains(m_extension))
			{
				bool isUnity = m_extension == ".unity";
				type = isUnity ? CustomAssetType.SCENE : CustomAssetType.REFERENCABLE;

				if (m_extension == ".asset" || isUnity || m_extension == ".spriteatlas")
				{
					var buffer = new byte[5];

					try
					{
						FileStream stream = File.OpenRead(m_assetPath);
						stream.Read(buffer, 0, 5);
						stream.Close();
					}
#if CustomDEBUG
                    catch (Exception e)
                    {
                        Debug.LogWarning("Guess Asset Type error :: " + e + "\n" + m_assetPath); 
#else
					catch
					{
#endif
						state = CustomAssetState.MISSING;
						return;
					}

					string str = string.Empty;
					foreach (byte t in buffer)
					{
						str += (char) t;
					}

					if (str != "%YAML")
					{
						type = CustomAssetType.BINARY_ASSET;
					}
				}
			}
			else if (m_extension == ".fbx")
			{
				type = CustomAssetType.MODEL;
			}
			else if (m_extension == ".dll")
			{
				type = CustomAssetType.DLL;
			}
			else
			{
				type = CustomAssetType.NON_READABLE;
			}
		}
		
		
		internal void LoadContent()
		{
			if (!fileContentDirty) return;
			m_cachefileWriteTS = m_fileWriteTS;
			
			if (IsMissing || type == CustomAssetType.NON_READABLE)
			{
				return;
			}
			
			if (type == CustomAssetType.DLL)
			{
#if CustomDEBUG
            Debug.LogWarning("Parsing DLL not yet supportted ");
#endif
				return;
			}
			
			if (!ExistOnDisk())
			{
				state = CustomAssetState.MISSING;
				return;
			}
			
			ClearUseGUIDs();
			
			if (IsFolder)
			{
				LoadFolder();
			}
			else if (IsReferencable)
			{
				LoadYAML2();
			}
			else if (IsBinaryAsset)
			{
				LoadBinaryAsset();
			}
		}

		internal void AddUseGUID(string fguid, int fFileId = -1, bool checkExist = true)
		{
			// if (checkExist && UseGUIDs.ContainsKey(fguid)) return;
			if (!IsValidGUID(fguid))
			{
				return;
			}

			if (!UseGUIDs.ContainsKey(fguid))
			{
				UseGUIDsList.Add(new Classes
				{
					guid = fguid,
					ids = new List<int>()
				});
				UseGUIDs.Add(fguid, new HashSet<int>());
			}

			if (fFileId != -1)
			{
				if (UseGUIDs[fguid].Contains(fFileId))
				{
					return;
				}

				UseGUIDs[fguid].Add(fFileId);
				Classes i = UseGUIDsList.FirstOrDefault(x => x.guid == fguid);
				if (i != null)
				{
					i.ids.Add(fFileId);
				}
			}
		}

		// ----------------------------- STATIC  ---------------------------------------

		internal static int SortByExtension(CustomAsset a1, CustomAsset a2)
		{
			if (a1 == null)
			{
				return -1;
			}

			if (a2 == null)
			{
				return 1;
			}

			int result = string.Compare(a1.m_extension, a2.m_extension, StringComparison.Ordinal);
			return result == 0 ? string.Compare(a1.m_assetName, a2.m_assetName, StringComparison.Ordinal) : result;
		}

		internal static List<CustomAsset> FindUsage(CustomAsset asset)
		{
			if (asset == null)
			{
				return null;
			}

			List<CustomAsset> refs = CustomCache.Api.FindAssets(asset.UseGUIDs.Keys.ToArray(), true);

			
			return refs;
		}

		internal static List<CustomAsset> FindUsedBy(CustomAsset asset)
		{
			return asset.UsedByMap.Values.ToList();
		}

		internal static List<string> FindUsageGUIDs(CustomAsset asset, bool includeScriptSymbols)
		{
			var result = new HashSet<string>();
			if (asset == null)
			{
				Debug.LogWarning("Asset invalid : " + asset.m_assetName);
				return result.ToList();
			}

			// for (var i = 0;i < asset.UseGUIDs.Count; i++)
			// {
			// 	result.Add(asset.UseGUIDs[i]);
			// }
			foreach (KeyValuePair<string, HashSet<int>> item in asset.UseGUIDs)
			{
				result.Add(item.Key);
			}

			//if (!includeScriptSymbols) return result.ToList();

			//if (asset.ScriptUsage != null)
			//{
			//	for (var i = 0; i < asset.ScriptUsage.Count; i++)
			//	{
			//    	var symbolList = CustomCache.Api.FindAllSymbol(asset.ScriptUsage[i]);
			//    	if (symbolList.Contains(asset)) continue;

			//    	var symbol = symbolList[0];
			//    	if (symbol == null || result.Contains(symbol.guid)) continue;

			//    	result.Add(symbol.guid);
			//	}	
			//}

			return result.ToList();
		}

		internal static List<string> FindUsedByGUIDs(CustomAsset asset)
		{
			return asset.UsedByMap.Keys.ToList();
		}
		
		internal float Draw(Rect r,
			bool highlight,
			bool drawPath = true,
			bool showFileSize = true,
			bool showABName = false,
			bool showAtlasName = false, 
			bool showUsageIcon = true,
			IWindow window = null
		){
			bool singleLine = r.height <= 18f;
			float rw = r.width;
			bool selected = CustomBookmark.Contains(guid);

			r.height = 16f;
			bool hasMouse = Event.current.type == EventType.MouseUp && r.Contains(Event.current.mousePosition);

			if (hasMouse && Event.current.button == 1)
			{
				var menu = new GenericMenu();
				if (m_extension == ".prefab")
				{
					menu.AddItem(new GUIContent("Edit in Scene"), false, EditPrefab);
				}

				menu.AddItem(new GUIContent("Open"), false, Open);
				menu.AddItem(new GUIContent("Ping"), false, Ping);
				menu.AddItem(new GUIContent(guid), false, CopyGUID);
				//menu.AddItem(new GUIContent("Reload"), false, Reload);

				menu.AddSeparator(string.Empty);
				menu.AddItem(new GUIContent("Bookmark"), selected, AddToSelection);
				menu.AddSeparator(string.Empty);
				menu.AddItem(new GUIContent("Copy path"), false, CopyAssetPath);
				menu.AddItem(new GUIContent("Copy full path"), false, CopyAssetPathFull);

				//if (IsScript)
				//{
				//    menu.AddSeparator(string.Empty);
				//    AddArray(menu, ScriptSymbols, "+ ", "Definitions", "No Definition", false);

				//    menu.AddSeparator(string.Empty);
				//    AddArray(menu, ScriptUsage, "-> ", "Depends", "No Dependency", true);
				//}

				menu.ShowAsContext();
				Event.current.Use();
			}

			if (IsMissing)
			{
				if (!singleLine)
				{
					r.y += 16f;
				}

				if (Event.current.type != EventType.Repaint)
				{
					return 0;
				}

				UnityEngine.GUI.Label(r, "(missing) " + guid, EditorStyles.whiteBoldLabel);
				return 0;
			}
			
			Rect iconRect = GUI.LeftRect(16f, ref r);
			if (Event.current.type == EventType.Repaint)
			{
				Texture icon = AssetDatabase.GetCachedIcon(m_assetPath);
				if (icon != null)
				{
					UnityEngine.GUI.DrawTexture(iconRect, icon);
				}
			}

			
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				Rect pingRect = CustomSetting.PingRow ? new Rect(0, r.y, r.x + r.width, r.height) : iconRect;
				if (pingRect.Contains(Event.current.mousePosition))
				{
					if (Event.current.control || Event.current.command)
					{
						if (selected)
						{
							RemoveFromSelection();
						}
						else
						{
							AddToSelection();
						}

						if (window != null)
						{
							window.Repaint();
						}
					}
                    else
                    {
                        Ping();
                    }

					
					//Event.current.Use();
				}
			}

			if (Event.current.type != EventType.Repaint)
			{
				return 0;
			}
		
			if (UsedByMap != null && UsedByMap.Count > 0)
			{
				var str = new GUIContent(UsedByMap.Count.ToString());
				Rect countRect = iconRect;
				countRect.x -= 16f;
				countRect.xMin = -10f;
				UnityEngine.GUI.Label(countRect, str, GUI.miniLabelAlignRight);
			}

			float pathW = drawPath ? EditorStyles.miniLabel.CalcSize(new GUIContent(m_assetFolder)).x : 0;
			float nameW = EditorStyles.boldLabel.CalcSize(new GUIContent(m_assetName)).x;
			Color cc = CustomCache.Api.setting.SelectedColor;

			if (singleLine)
			{
				Rect lbRect = GUI.LeftRect(pathW + nameW, ref r);

				if (selected)
				{
					Color c1 = UnityEngine.GUI.color;
					UnityEngine.GUI.color = cc;
					UnityEngine.GUI.DrawTexture(lbRect, EditorGUIUtility.whiteTexture);
					UnityEngine.GUI.color = c1;
				}

				if (drawPath)
				{
					Color c2 = UnityEngine.GUI.color;
					UnityEngine.GUI.color = new Color(c2.r, c2.g, c2.b, c2.a*0.5f);
					UnityEngine.GUI.Label(GUI.LeftRect(pathW, ref lbRect), m_assetFolder, EditorStyles.miniLabel);
					UnityEngine.GUI.color = c2;

					lbRect.xMin -= 4f;
					UnityEngine.GUI.Label(lbRect, m_assetName, EditorStyles.boldLabel);
				}
				else
				{
					UnityEngine.GUI.Label(lbRect, m_assetName);
				}
			}
			else
			{
				if (drawPath)
				{
					UnityEngine.GUI.Label(new Rect(r.x, r.y + 16f, r.width, r.height), m_assetFolder, EditorStyles.miniLabel);
				}

				Rect lbRect = GUI.LeftRect(nameW, ref r);
				if (selected)
				{
					GUI.Rect(lbRect, cc);
				}
				
				UnityEngine.GUI.Label(lbRect, m_assetName, EditorStyles.boldLabel);
			}

			var rr = GUI.RightRect(10f, ref r); //margin
			if (highlight)
			{
				rr.xMin += 2f;
				rr.width = 1f;
				GUI.Rect(rr, GUI.darkGreen);	
			}

			Color c = UnityEngine.GUI.color;
			UnityEngine.GUI.color = new Color(c.r, c.g, c.b, c.a*0.5f);
			
			if (showFileSize)
			{
				Rect fsRect = GUI.RightRect(40f, ref r); // filesize label

				if (fileSizeText == null)
				{
					fileSizeText = new GUIContent(CustomHelper.GetfileSizeString(fileSize));
				}


				
				UnityEngine.GUI.Label(fsRect, fileSizeText, GUI.miniLabelAlignRight);
			}

			if (!string.IsNullOrEmpty(m_addressable))
			{
				Rect adRect = GUI.RightRect(100f, ref r);
				UnityEngine.GUI.Label(adRect, m_addressable, GUI.miniLabelAlignRight);
			}

			
			
			if (showUsageIcon && HashUsedByClassesIds != null)
			{
				foreach (int item in HashUsedByClassesIds)
				{
					if (!CustomUnity.HashClassesNormal.ContainsKey(item))
					{
						continue;
					}

					string name = CustomUnity.HashClassesNormal[item];
					Type t = null;
					if (!HashClasses.TryGetValue(item, out t))
					{
						t = CustomUnity.GetType(name);
						HashClasses.Add(item, t);
					}
					
					GUIContent content = null;
					var isExisted = cacheImage.TryGetValue(name, out content);
					if (content == null) content = new GUIContent(EditorGUIUtility.ObjectContent(null, t).image, name);

					if (!isExisted)
					{
						cacheImage.Add(name, content);
					} else {
						cacheImage[name] = content;
					}
					
					if (content != null)
					{
						try
						{
							UnityEngine.GUI.Label(GUI.RightRect(15f, ref r), content, GUI.miniLabelAlignRight);
						}
						#if !CustomDEBUG
						catch {}
						#else
						catch (Exception e)
						{
							UnityEngine.Debug.LogWarning(e);
						}
						#endif
					}
				}
			}
			
			if (showAtlasName)
			{
				GUI.RightRect(10f, ref r); //margin
				Rect abRect = GUI.RightRect(120f, ref r); // filesize label
				if (!string.IsNullOrEmpty(m_atlas))
				{
					UnityEngine.GUI.Label(abRect, m_atlas, GUI.miniLabelAlignRight);
				}
			}
			
			if (showABName)
			{
				GUI.RightRect(10f, ref r); //margin
				Rect abRect = GUI.RightRect(100f, ref r); // filesize label
				if (!string.IsNullOrEmpty(m_assetbundle))
				{
					UnityEngine.GUI.Label(abRect, m_assetbundle, GUI.miniLabelAlignRight);
				}
			}

			if (true)
			{
				GUI.RightRect(10f, ref r); //margin
				Rect abRect = GUI.RightRect(100f, ref r); // filesize label
				if (!string.IsNullOrEmpty(m_addressable))
				{
					UnityEngine.GUI.Label(abRect, m_addressable, GUI.miniLabelAlignRight);
				}
			}

			UnityEngine.GUI.color = c;

			if (Event.current.type == EventType.Repaint)
			{
				return rw < pathW + nameW ? 32f : 18f;
			}

			return r.height;
		}

		

		internal GenericMenu AddArray(GenericMenu menu, List<string> list, string prefix, string title,
			string emptyTitle, bool showAsset, int max = 10)
		{
			//if (list.Count > 0)
			//{
			//    if (list.Count > max)
			//    {
			//        prefix = string.Format("{0} _{1}/", title, list.Count) + prefix;
			//    }

			//    //for (var i = 0; i < list.Count; i++)
			//    //{
			//    //    var def = list[i];
			//    //    var suffix = showAsset ? "/" + CustomCache.Api.FindSymbol(def).assetName : string.Empty;
			//    //    menu.AddItem(new GUIContent(prefix + def + suffix), false, () => OpenScript(def));
			//    //}
			//}
			//else
			{
				menu.AddItem(new GUIContent(emptyTitle), true, null);
			}

			return menu;
		}

		internal void CopyGUID()
		{
			EditorGUIUtility.systemCopyBuffer = guid;
			Debug.Log(guid);
		}

		internal void CopyName()
		{
			EditorGUIUtility.systemCopyBuffer = m_assetName;
			Debug.Log(m_assetName);
		}

		internal void CopyAssetPath()
		{
			EditorGUIUtility.systemCopyBuffer = m_assetPath;
			Debug.Log(m_assetPath);
		}

		internal void CopyAssetPathFull()
		{
			string fullName = new FileInfo(m_assetPath).FullName;
			EditorGUIUtility.systemCopyBuffer = fullName;
			Debug.Log(fullName);
		}

		internal void AddToSelection()
		{
			if (!CustomBookmark.Contains(guid))
			{
				CustomBookmark.Add(guid);
			}

			//var list = Selection.objects.ToList();
			//var obj = CustomUnity.LoadAssetAtPath<Object>(assetPath);
			//if (!list.Contains(obj))
			//{
			//    list.Add(obj);
			//    Selection.objects = list.ToArray();
			//}
		}

		internal void RemoveFromSelection()
		{
			if (CustomBookmark.Contains(guid))
			{
				CustomBookmark.Remove(guid);
			}
		}

		internal void Ping()
		{
			EditorGUIUtility.PingObject(
				AssetDatabase.LoadAssetAtPath(m_assetPath, typeof(UnityObject))
			);
		}

		internal void Open()
		{
			AssetDatabase.OpenAsset(
				AssetDatabase.LoadAssetAtPath(m_assetPath, typeof(UnityObject))
			);
		}

		internal void EditPrefab()
		{
			UnityObject prefab = AssetDatabase.LoadAssetAtPath(m_assetPath, typeof(UnityObject));
			UnityObject.Instantiate(prefab);
		}

		//internal void OpenScript(string definition)
		//{
		//    var asset = CustomCache.Api.FindSymbol(definition);
		//    if (asset == null) return;

		//    EditorGUIUtility.PingObject(
		//        AssetDatabase.LoadAssetAtPath(asset.assetPath, typeof(Object))
		//        );
		//}

		// ----------------------------- SERIALIZED UTILS ---------------------------------------

		

		// ----------------------------- LOAD ASSETS ---------------------------------------

		internal void LoadGameObject(GameObject go)
		{
			Component[] compList = go.GetComponentsInChildren<Component>();
			for (var i = 0; i < compList.Length; i++)
			{
				LoadSerialized(compList[i]);
			}
		}

		internal void LoadSerialized(UnityObject target)
		{
			SerializedProperty[] props = CustomUnity.xGetSerializedProperties(target, true);

			for (var i = 0; i < props.Length; i++)
			{
				if (props[i].propertyType != SerializedPropertyType.ObjectReference)
				{
					continue;
				}

				UnityObject refObj = props[i].objectReferenceValue;
				if (refObj == null)
				{
					continue;
				}

				string refGUID = AssetDatabase.AssetPathToGUID(
					AssetDatabase.GetAssetPath(refObj)
				);

				//Debug.Log("Found Reference BinaryAsset <" + assetPath + "> : " + refGUID + ":" + refObj);
				AddUseGUID(refGUID);
			}
		}
		
		internal void LoadTerrainData(TerrainData terrain)
		{
#if UNITY_2018_3_OR_NEWER
			TerrainLayer[] arr0 = terrain.terrainLayers;
			for (var i = 0; i < arr0.Length; i++)
			{
				string aPath = AssetDatabase.GetAssetPath(arr0[i]);
				string refGUID = AssetDatabase.AssetPathToGUID(aPath);
				AddUseGUID(refGUID);
			}
#endif


			DetailPrototype[] arr = terrain.detailPrototypes;

			for (var i = 0; i < arr.Length; i++)
			{
				string aPath = AssetDatabase.GetAssetPath(arr[i].prototypeTexture);
				string refGUID = AssetDatabase.AssetPathToGUID(aPath);
				AddUseGUID(refGUID);
			}

			TreePrototype[] arr2 = terrain.treePrototypes;
			for (var i = 0; i < arr2.Length; i++)
			{
				string aPath = AssetDatabase.GetAssetPath(arr2[i].prefab);
				string refGUID = AssetDatabase.AssetPathToGUID(aPath);
				AddUseGUID(refGUID);
			}

			CustomUnity.TerrainTextureData[] arr3 = CustomUnity.GetTerrainTextureDatas(terrain);
			for (var i = 0; i < arr3.Length; i++)
			{
				CustomUnity.TerrainTextureData texs = arr3[i];
				for (var k = 0; k < texs.textures.Length; k++)
				{
					Texture2D tex = texs.textures[k];
					if (tex == null)
					{
						continue;
					}

					string aPath = AssetDatabase.GetAssetPath(tex);
					if (string.IsNullOrEmpty(aPath))
					{
						continue;
					}

					string refGUID = AssetDatabase.AssetPathToGUID(aPath);
					if (string.IsNullOrEmpty(refGUID))
					{
						continue;
					}

					AddUseGUID(refGUID);
				}
			}
		}

		private void ClearUseGUIDs()
		{
#if CustomDEBUG
		    Debug.Log("ClearUseGUIDs: " + assetPath);
#endif

			UseGUIDs.Clear();
			UseGUIDsList.Clear();
		}
	
		static int binaryLoaded;
		internal void LoadBinaryAsset()
		{
			ClearUseGUIDs();
			
			UnityObject assetData = AssetDatabase.LoadAssetAtPath(m_assetPath, typeof(UnityObject));
			if (assetData is GameObject)
			{
				type = CustomAssetType.MODEL;
				LoadGameObject(assetData as GameObject);
			}
			else if (assetData is TerrainData)
			{
				type = CustomAssetType.TERRAIN;
				LoadTerrainData(assetData as TerrainData);
			}
			else
			{
				LoadSerialized(assetData);
			}

			#if CustomDEBUG
			Debug.Log("LoadBinaryAsset :: " + assetData + ":" + type);
			#endif

			assetData = null;

			if (binaryLoaded++ <= 30) return;
			binaryLoaded = 0;
			CustomUnity.UnloadUnusedAssets();
		}

		internal void LoadYAML()
		{
			if (!File.Exists(m_assetPath))
			{
				state = CustomAssetState.MISSING;
				return;
			}
			
			
			if (m_isAssetFile)
			{
				var s = AssetDatabase.LoadAssetAtPath<CustomCache>(m_assetPath);
				if (s != null)
				{
					return;
				}
			}
			
			string text = string.Empty;
			try
			{
				text = File.ReadAllText(m_assetPath);
			}
#if CustomDEBUG
            catch (Exception e)
            {
                Debug.LogWarning("Guess Asset Type error :: " + e + "\n" + assetPath);
#else
			catch
			{
#endif
				state = CustomAssetState.MISSING;
				return;
			}

#if CustomDEBUG
	        Debug.Log("LoadYAML: " + assetPath);
#endif

			//if(assetPath.Contains("Myriad Pro - Bold SDF"))
			//{
			//    Debug.Log("no ne");
			//}
			// PERFORMANCE HOG!
			// var matches = Regex.Matches(text, @"\bguid: [a-f0-9]{32}\b");
			MatchCollection matches = Regex.Matches(text, @".*guid: [a-f0-9]{32}.*\n");

			foreach (Match match in matches)
			{
				Match guidMatch = Regex.Match(match.Value, @"\bguid: [a-f0-9]{32}\b");
				string refGUID = guidMatch.Value.Replace("guid: ", string.Empty);

				Match fileIdMatch = Regex.Match(match.Value, @"\bfileID: ([0-9]*).*");
				int id = -1;
				try
				{
					id = int.Parse(fileIdMatch.Groups[1].Value) / 100000;
				}
				catch { }

				AddUseGUID(refGUID, id);
			}

			//var idx = text.IndexOf("guid: ");
			//var counter=0;
			//while (idx != -1)
			//{
			//	var guid = text.Substring(idx + 6, 32);
			//	if (UseGUIDs.Contains(guid)) continue;
			//	AddUseGUID(guid);

			//	idx += 39;
			//	if (idx > text.Length-40) break;

			//	//Debug.Log(assetName + ":" +  guid);
			//	idx = text.IndexOf("guid: ", idx + 39);
			//	if (counter++ > 100) break;
			//}

			//if (counter > 100){
			//	Debug.LogWarning("Never finish on " + assetName);
			//}
		}

		internal void LoadYAML2()
		{
			if (!File.Exists(m_assetPath))
			{
				state = CustomAssetState.MISSING;
				return;
			}
			
			if (m_assetPath == "ProjectSettings/EditorBuildSettings.asset")
			{
				var listScenes = EditorBuildSettings.scenes;
				foreach (var scene in listScenes)
				{
					if (!scene.enabled) continue;
					var path = scene.path;
					var guid = AssetDatabase.AssetPathToGUID(path);
					
					AddUseGUID(guid, 0);
					
#if CustomDEBUG
					Debug.Log("AddScene: " + path);
#endif
				}
			}
			
			// var text = string.Empty;
			try
			{
				using (var sr = new StreamReader(m_assetPath))
				{
					while (sr.Peek() >= 0)
					{
						string line = sr.ReadLine();
						int index = line.IndexOf("guid: ");
						if (index < 0)
						{
							continue;
						}

						string refGUID = line.Substring(index + 6, 32);
						int indexFileId = line.IndexOf("fileID: ");
						int fileID = -1;
						if (indexFileId >= 0)
						{
							indexFileId += 8;
							string fileIDStr =
								line.Substring(indexFileId, line.IndexOf(',', indexFileId) - indexFileId);
							try
							{
								fileID = int.Parse(fileIDStr) / 100000;
							}
							catch { }
						}

						AddUseGUID(refGUID, fileID);
					}
				}

#if CustomDEBUG
	            if (UseGUIDsList.Count > 0)
	            {
	            	Debug.Log(assetPath + ":" + UseGUIDsList.Count);
	            }
#endif
			}
#if CustomDEBUG
            catch (Exception e)
            {
                Debug.LogWarning("Guess Asset Type error :: " + e + "\n" + assetPath);
#else
			catch
			{
#endif
				state = CustomAssetState.MISSING;
			}
		}

		internal void LoadFolder()
		{
			if (!Directory.Exists(m_assetPath))
			{
				state = CustomAssetState.MISSING;
				return;
			}
			
			// do not analyse folders outside project
			if (!m_assetPath.StartsWith("Assets/")) return;
			

			try
			{
				string[] files = Directory.GetFiles(m_assetPath);
				string[] dirs = Directory.GetDirectories(m_assetPath);

				foreach (string f in files)
				{
					if (f.EndsWith(".meta", StringComparison.Ordinal))
					{
						continue;
					}
					
					string fguid = AssetDatabase.AssetPathToGUID(f);
					if (string.IsNullOrEmpty(fguid))
					{
						continue;
					}

					// AddUseGUID(fguid, true);
					AddUseGUID(fguid);
				}

				foreach (string d in dirs)
				{
					string fguid = AssetDatabase.AssetPathToGUID(d);
					if (string.IsNullOrEmpty(fguid))
					{
						continue;
					}

					// AddUseGUID(fguid, true);
					AddUseGUID(fguid);
				}
			}
#if CustomDEBUG
            catch (Exception e)
            {
                Debug.LogWarning("LoadFolder() error :: " + e + "\n" + assetPath);
#else
			catch
			{
#endif
				state = CustomAssetState.MISSING;
			}

			//Debug.Log("Load Folder :: " + assetName + ":" + type + ":" + UseGUIDs.Count);
		}

		

		// ----------------------------- REPLACE GUIDS ---------------------------------------

		internal bool ReplaceReference(string fromGUID, string toGUID, TerrainData terrain = null)
		{
			if (IsMissing)
			{
				return false;
			}

			if (IsReferencable)
			{
				string text = string.Empty;

				if (!File.Exists(m_assetPath))
				{
					state = CustomAssetState.MISSING;
					return false;
				}

				try
				{
					text = File.ReadAllText(m_assetPath).Replace("\r", "\n");
					File.WriteAllText(m_assetPath, text.Replace(fromGUID, toGUID));
					// AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.Default);
					return true;
				}
				catch (Exception e)
				{
					state = CustomAssetState.MISSING;
//#if CustomDEBUG
					Debug.LogWarning("Replace Reference error :: " + e + "\n" + m_assetPath);
//#endif
				}

				return false;
			}

			if (type == CustomAssetType.TERRAIN)
			{
				var fromObj = CustomUnity.LoadAssetWithGUID<UnityObject>(fromGUID);
				var toObj = CustomUnity.LoadAssetWithGUID<UnityObject>(toGUID);
				var found = 0;
				// var terrain = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)) as TerrainData;

				if (fromObj is Texture2D)
				{
					DetailPrototype[] arr = terrain.detailPrototypes;
					for (var i = 0; i < arr.Length; i++)
					{
						if (arr[i].prototypeTexture == (Texture2D) fromObj)
						{
							found++;
							arr[i].prototypeTexture = (Texture2D) toObj;
						}
					}

					terrain.detailPrototypes = arr;
					CustomUnity.ReplaceTerrainTextureDatas(terrain, (Texture2D) fromObj, (Texture2D) toObj);
				}

				if (fromObj is GameObject)
				{
					TreePrototype[] arr2 = terrain.treePrototypes;
					for (var i = 0; i < arr2.Length; i++)
					{
						if (arr2[i].prefab == (GameObject) fromObj)
						{
							found++;
							arr2[i].prefab = (GameObject) toObj;
						}
					}

					terrain.treePrototypes = arr2;
				}

				// EditorUtility.SetDirty(terrain);
				// AssetDatabase.SaveAssets();

				fromObj = null;
				toObj = null;
				terrain = null;
				// CustomUnity.UnloadUnusedAssets();

				return found > 0;
			}

			Debug.LogWarning("Something wrong, should never be here - Ignored <" + m_assetPath +
			                 "> : not a readable type, can not replace ! " + type);
			return false;
		}

        internal bool ReplaceReference(string fromGUID, string toGUID, long toFileId, TerrainData terrain = null)
        {
            Debug.Log("ReplaceReference: from " + fromGUID + "  to: " + toGUID + "  toFileId: " + toFileId);
            if (IsMissing)
            {
//				Debug.Log("this asset is missing");
                return false;
            }

            if (IsReferencable)
            {
                string text = string.Empty;

                if (!File.Exists(m_assetPath))
                {
                    state = CustomAssetState.MISSING;
//					Debug.Log("this asset not exits");
                    return false;
                }

                try
                {
                    var sb = new StringBuilder();
                    text = File.ReadAllText(assetPath).Replace("\r", "\n");
                    var lines = text.Split('\n');
                    //string result = "";
                    for(int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i];
                        if(line.IndexOf(fromGUID, StringComparison.Ordinal) >= 0)
                        {
                            
							if(toFileId > 0)
							{
								
	                            const string FileID = "fileID: ";
	                            var index = line.IndexOf(FileID, StringComparison.Ordinal);
	                            if(index >= 0)
	                            {
	                                var fromFileId = line.Substring(index + FileID.Length, line.IndexOf(',', index) - (index + FileID.Length));
									long fileType = 0;
									if(!long.TryParse(fromFileId, out fileType))
									{
                                        Debug.LogWarning("cannot parse file");
                                        return false;
									}
                                    if (fileType.ToString().Substring(0,3) != toFileId.ToString().Substring(0,3))
                                    {
                                        //difference file type
                                        Debug.LogWarning("Difference file type");
                                        return false;
                                    }

                                    Debug.Log("ReplaceReference: fromFileId " + fromFileId + "  to File Id " + toFileId);
                                    line = line.Replace(fromFileId, toFileId.ToString());
	                            }
							}
							line = line.Replace(fromGUID, toGUID);
                        }
                        sb.Append(line);
                        sb.AppendLine();
                        //result += line + "\n";
                    }

                    //File.WriteAllText(assetPath, result.Trim());
                    File.WriteAllText(assetPath, sb.ToString());
                    //AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.Default);
                    return true;
                }
                catch (Exception e)
                {
                    state = CustomAssetState.MISSING;
                    //#if CustomDEBUG
                    Debug.LogWarning("Replace Reference error :: " + e + "\n" + m_assetPath);
                    //#endif
                }

                return false;
            }

            if (type == CustomAssetType.TERRAIN)
            {
                var fromObj = CustomUnity.LoadAssetWithGUID<UnityObject>(fromGUID);
                var toObj = CustomUnity.LoadAssetWithGUID<UnityObject>(toGUID);
                var found = 0;
                // var terrain = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)) as TerrainData;

                if (fromObj is Texture2D)
                {
                    DetailPrototype[] arr = terrain.detailPrototypes;
                    for (var i = 0; i < arr.Length; i++)
                    {
                        if (arr[i].prototypeTexture == (Texture2D)fromObj)
                        {
                            found++;
                            arr[i].prototypeTexture = (Texture2D)toObj;
                        }
                    }

                    terrain.detailPrototypes = arr;
                    CustomUnity.ReplaceTerrainTextureDatas(terrain, (Texture2D)fromObj, (Texture2D)toObj);
                }

                if (fromObj is GameObject)
                {
                    TreePrototype[] arr2 = terrain.treePrototypes;
                    for (var i = 0; i < arr2.Length; i++)
                    {
                        if (arr2[i].prefab == (GameObject)fromObj)
                        {
                            found++;
                            arr2[i].prefab = (GameObject)toObj;
                        }
                    }

                    terrain.treePrototypes = arr2;
                }

                // EditorUtility.SetDirty(terrain);
                // AssetDatabase.SaveAssets();

                fromObj = null;
                toObj = null;
                terrain = null;
                // CustomUnity.UnloadUnusedAssets();

                return found > 0;
            }

            Debug.LogWarning("Something wrong, should never be here - Ignored <" + m_assetPath +
                             "> : not a readable type, can not replace ! " + type);
            return false;
        }

		[Serializable]
		private class Classes
		{
			public string guid;
			public List<int> ids;
		}
	}
}