//#define BUILD_REPORT_TOOL_EXPERIMENTS

using UnityEngine;
using UnityEditor;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DldUtil;
using UnityEditor.Compilation;

/*

Editor
Editor log can be brought up through the Open Editor Log button in Unity's Console window.

Mac OS X	~/Library/Logs/Unity/Editor.log (or /Users/username/Library/Logs/Unity/Editor.log)
Windows XP *	C:\Documents and Settings\username\Local Settings\Application Data\Unity\Editor\Editor.log
Windows Vista/7 *	C:\Users\username\AppData\Local\Unity\Editor\Editor.log

(*) On Windows the Editor log file is stored in the local application data folder: %LOCALAPPDATA%\Unity\Editor\Editor.log, where LOCALAPPDATA is defined by CSIDL_LOCAL_APPDATA.





need to parse contents of editor log.
this part is what we're interested in:

[quote]
Textures      196.4 kb	 3.4%
Meshes        0.0 kb	 0.0%
Animations    0.0 kb	 0.0%
Sounds        0.0 kb	 0.0%
Shaders       0.0 kb	 0.0%
Other Assets  37.4 kb	 0.6%
Levels        8.5 kb	 0.1%
Scripts       228.4 kb	 3.9%
Included DLLs 5.2 mb	 91.7%
File headers  12.5 kb	 0.2%
Complete size 5.7 mb	 100.0%

Used Assets, sorted by uncompressed size:
 39.1 kb	 0.7% Assets/BTX/GUI/Skin/Window.png
 21.0 kb	 0.4% Assets/BTX/GUI/BehaviourTree/Resources/BehaviourTreeGuiSkin.guiskin
 20.3 kb	 0.3% Assets/BTX/Fonts/DejaVuSans-SmallSize.ttf
 20.2 kb	 0.3% Assets/BTX/Fonts/DejaVuSans-Bold.ttf
 20.1 kb	 0.3% Assets/BTX/Fonts/DejaVuSansCondensed 1.ttf
 12.0 kb	 0.2% Assets/BTX/Fonts/DejaVuSansCondensed.ttf
 10.8 kb	 0.2% Assets/BTX/GUI/BehaviourTree/Nodes2/White.png
 8.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/Nodes/RoundedBox.png
 8.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/Nodes/Decorator.png
 4.9 kb	 0.1% Assets/BTX/GUI/Skin/Box.png
 4.6 kb	 0.1% Assets/BTX/GUI/BehaviourTree/GlovedHand.png
 4.5 kb	 0.1% Assets/BTX/GUI/Skin/TextField_Normal.png
 4.5 kb	 0.1% Assets/BTX/GUI/Skin/Button_Toggled.png
 4.5 kb	 0.1% Assets/BTX/GUI/Skin/Button_Normal.png
 4.5 kb	 0.1% Assets/BTX/GUI/Skin/Button_Active.png
 4.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/RunState/Visiting.png
 4.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/RunState/Success.png
 4.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/RunState/Running.png
 (etc. goes on and on until all files used are listed)
[/quote]


This part can also be helpful:

[quote]
Mono dependencies included in the build
Boo.Lang.dll
Mono.Security.dll
System.Core.dll
System.Xml.dll
System.dll
UnityScript.Lang.dll
mscorlib.dll
Assembly-CSharp.dll
Assembly-UnityScript.dll

[/quote]


so we're gonna flex our string parsing skills here.

just get this string since it seems to be constant enough:
"Used Assets, sorted by uncompressed size:"

then starting from that line going upwards, get the line that begins with "Textures"

we're relying on the assumption that this format won't get changed

in short, this is all complete hackery that won't be futureproof

hopefully UT would provide proper script access to this

*/

namespace CustomBuildReport
{
	public struct ExtraData
	{
		public string SavedPath;
		public string Contents;
	}

	[System.Serializable]
#if UNITY_2018_1_OR_NEWER
	public partial class ReportGenerator : UnityEditor.Build.IPreprocessBuildWithReport, UnityEditor.Build.IPostprocessBuildWithReport
#elif UNITY_5_6_OR_NEWER
	public partial class ReportGenerator : UnityEditor.Build.IPreprocessBuild, UnityEditor.Build.IPostprocessBuild
#else
	public partial class ReportGenerator
#endif
	{
		public int callbackOrder { get { return 99999; } }

		static CustomBuildReport.BuildInfo _lastKnownBuildInfo;
		static CustomBuildReport.AssetDependencies _lastKnownAssetDependencies;
		static CustomBuildReport.TextureData _lastKnownTextureData;
		static CustomBuildReport.MeshData _lastKnownMeshData;
		static CustomBuildReport.PrefabData _lastKnownPrefabData;
		static CustomBuildReport.UnityBuildReport _lastKnownUnityBuildReport;

		static Assembly[] _lastAssemblies;

		public static CustomBuildReport.UnityBuildReport LastKnownUnityBuildReport { get { return _lastKnownUnityBuildReport; } }

		static bool _shouldCalculateBuildSize = true;

		static string _lastEditorLogPath = "";

		static string _lastPathToBuiltProject = string.Empty;

		/// <summary>
		/// <para>Used to collect all prefabs used in scenes that are included in build.</para>
		///
		/// <para>We need to manually track prefabs (and 3d model files, which are
		/// considered implicit prefabs) used in scenes, because if they are marked
		/// as Static in the scenes they're used in, they will not end up being
		/// reported as included in the build.</para>
		///
		/// <para>That's because static meshes are merged into one big mesh.
		/// That one big mesh is considered a new asset, and has no
		/// connection to the prefabs/3d model files that its parts originally came from.</para>
		/// </summary>
		static readonly HashSet<string> PrefabsUsedInScenes = new HashSet<string>();

		/// <summary>
		/// <para><see cref="PrefabsUsedInScenes"/> converted into a list, for quick iteration.</para>
		/// <inheritdoc cref="PrefabsUsedInScenes"/>
		/// </summary>
		static readonly List<string> PrefabsUsedInScenesList = new List<string>();


		static string _lastSavePath = "";

		public const string TIME_OF_BUILD_FORMAT = "yyyy MMM dd ddd h:mm:ss tt UTCz";

		static bool _gotCommandLineArguments;
		static bool _unityHasNoLogArgument;


		static BuildInfo CreateNewBuildInfo()
		{
			return new BuildInfo();
			//return ScriptableObject.CreateInstance<BuildInfo>();
		}

		/// <summary>
		/// Called to get project's values from the Unity Editor API after the project is built.
		/// Has to be called from the main thread.
		/// </summary>
		static void Init()
		{
			Init(true, ref _lastKnownBuildInfo, null);
		}

		/// <summary>
		/// Get and store data that are only allowed to be accessed
		/// from the main thread here so it won't generate errors
		/// when we access them from threads.
		///
		/// Which means this function has to be called from the main
		/// thread.</summary>
		/// <param name="fromBuild">True: this method is called after a build is made.
		/// False: this method is called after user pressed the "Get Log" button.</param>
		/// <param name="buildInfo">The BuildInfo to save the values to.</param>
		/// <param name="scenes">You can specify a custom list of scenes,
		/// if project was built with a custom build script.
		/// Otherwise, leave null so that it will just use
		/// UnityEditor.EditorBuildSettings.scenes instead.</param>
		static void Init(bool fromBuild, ref BuildInfo buildInfo, string[] scenes)
		{
			// --------------------

			if (buildInfo == null)
			{
				buildInfo = CreateNewBuildInfo();
			}

			// --------------------

			//Debug.LogFormat("CustomBuildReport.ReportGenerator.Init() called");

			buildInfo.SetBuildTargetUsed(CustomBuildReport.Util.BuildTargetOfLastBuild);

			// --------------------

			if (scenes != null)
			{
				buildInfo.SetScenes(scenes);
			}
			else
			{
				buildInfo.SetScenes(CustomBuildReport.Util.GetAllScenesInBuild());
			}

			//for (int n = 0, len = buildInfo.ScenesIncludedInProject.Length; n < len; ++n)
			//{
			//	Debug.Log("scene " + n + ": " + buildInfo.ScenesIncludedInProject[n]);
			//}

			// --------------------

			if (!string.IsNullOrEmpty(_lastPathToBuiltProject))
			{
				buildInfo.BuildFilePath = _lastPathToBuiltProject;
			}
			else
			{
				buildInfo.BuildFilePath =
					EditorUserBuildSettings.GetBuildLocation(CustomBuildReport.Util.BuildTargetOfLastBuild);
			}
			//Debug.Log("BuildTargetOfLastBuild: " + CustomBuildReport.Util.BuildTargetOfLastBuild);

			// --------------------

			buildInfo.EditorAppContentsPath = EditorApplication.applicationContentsPath;
			buildInfo.ProjectAssetsPath = Application.dataPath;

			// --------------------

			buildInfo.UnityVersion = string.Format("Unity {0}", Application.unityVersion);

			buildInfo.IncludedSvnInUnused = CustomBuildReport.Options.IncludeSvnInUnused;
			buildInfo.IncludedGitInUnused = CustomBuildReport.Options.IncludeGitInUnused;
			buildInfo.IncludedBuildReportToolAssetsInUnused = CustomBuildReport.Options.IncludeBuildReportToolAssetsInUnused;

			var ignorePatternCopy = new List<SavedOptions.IgnorePattern>(CustomBuildReport.Options.IgnorePatternsForUnused.Count);
			for (int n = 0, len = CustomBuildReport.Options.IgnorePatternsForUnused.Count; n < len; ++n)
			{
				ignorePatternCopy.Add(CustomBuildReport.Options.IgnorePatternsForUnused[n]);
			}
			buildInfo.IgnorePatternsForUnused = ignorePatternCopy;

			buildInfo.ProcessUnusedAssetsInBatches = CustomBuildReport.Options.ProcessUnusedAssetsInBatches;
			buildInfo.UnusedAssetsEntriesPerBatch = CustomBuildReport.Options.UnusedAssetsEntriesPerBatch;

			// --------------------

#if UNITY_2023_1_OR_NEWER
			BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
			var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);

			buildInfo.MonoLevel =
				PlayerSettings.GetApiCompatibilityLevel(namedBuildTarget);
#elif UNITY_5_6_OR_NEWER
			buildInfo.MonoLevel =
				PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup);
#else
			buildInfo.MonoLevel = PlayerSettings.apiCompatibilityLevel;
#endif

#if !UNITY_2018_3_OR_NEWER
			buildInfo.CodeStrippingLevel = PlayerSettings.strippingLevel;
#endif

			// --------------------

			if (CustomBuildReport.Options.GetProjectSettings)
			{
				buildInfo.HasUnityBuildSettings = true;
				buildInfo.UnityBuildSettings = new UnityBuildSettings();
				UnityBuildSettingsUtility.Populate(buildInfo.UnityBuildSettings);
			}
			else
			{
				buildInfo.HasUnityBuildSettings = false;
				buildInfo.UnityBuildSettings = null;
			}

			// --------------------

#if UNITY_2023_1_OR_NEWER
			buildInfo.AndroidUseAPKExpansionFiles = PlayerSettings.Android.splitApplicationBinary;
#else
			buildInfo.AndroidUseAPKExpansionFiles = PlayerSettings.Android.useAPKExpansionFiles;
#endif

			buildInfo.AndroidCreateProject = buildInfo.BuildTargetUsed == BuildTarget.Android &&
			                                 !Util.IsFileOfType(buildInfo.BuildFilePath, ".apk");

			//Debug.Log("buildInfo.AndroidCreateProject: " + buildInfo.AndroidCreateProject);
			//Debug.Log("PlayerSettings.Android.useAPKExpansionFiles: " + PlayerSettings.Android.useAPKExpansionFiles);
			//Debug.Log("BuildOptions.AcceptExternalModificationsToPlayer: " + BuildOptions.AcceptExternalModificationsToPlayer);

			// --------------------

			buildInfo.UsedAssetsIncludedInCreation = CustomBuildReport.Options.IncludeUsedAssetsInReportCreation;
			buildInfo.UnusedAssetsIncludedInCreation = CustomBuildReport.Options.IncludeUnusedAssetsInReportCreation;
			buildInfo.UnusedPrefabsIncludedInCreation = CustomBuildReport.Options.IncludeUnusedPrefabsInReportCreation;

			// --------------------

			_shouldCalculateBuildSize = CustomBuildReport.Options.IncludeBuildSizeInReportCreation;

			// --------------------

			// clear old values if any
			buildInfo.ProjectName = null;
			buildInfo.UsedAssets = null;
			buildInfo.UnusedAssets = null;

			// --------------------

			bool gotExtraBuildData = false;

			string regularEditorLogPath = CustomBuildReport.Util.UsedEditorLogPath;
			if (!DoesEditorLogHaveBuildInfo(regularEditorLogPath))
			{
				string lastSuccessfulBuildEditorLog =
					CustomBuildReport.Util.LastSuccessfulBuildFilePath(Application.dataPath);
				if (DoesEditorLogHaveBuildInfo(lastSuccessfulBuildEditorLog))
				{
					bool playerDataNotRebuilt = DoesEditorLogHaveNoBuildInfoDueToNoPlayerRebuilt(regularEditorLogPath);

					_lastEditorLogPath = lastSuccessfulBuildEditorLog;
					if (playerDataNotRebuilt)
					{
						Debug.LogWarning($"No new build was created since no changes were detected. Do a clean build to force creation of a new build.\nReusing last successful build's Editor.log file from: {lastSuccessfulBuildEditorLog}");
					}
					else
					{
						Debug.LogWarning($"No build data found in {regularEditorLogPath}\nReusing last successful build's Editor.log file from: {lastSuccessfulBuildEditorLog}");
					}

					CustomBuildReport.Util.LastBuildExtraData extraData =
						CustomBuildReport.Util.OpenSerialized<CustomBuildReport.Util.LastBuildExtraData>(
							CustomBuildReport.Util.LastSuccessfulBuildExtraDataFilePath(Application.dataPath));
					if (extraData != null && !playerDataNotRebuilt)
					{
						buildInfo.BuildDurationTime = extraData.GetBuildDuration();

						buildInfo.BuildTimeGot = extraData.GetBuildTimeStarted();
						buildInfo.BuildTimeGotReadable = buildInfo.BuildTimeGot.ToString(TIME_OF_BUILD_FORMAT);
						gotExtraBuildData = true;
					}
				}
			}
			else
			{
				_lastEditorLogPath = regularEditorLogPath;
			}

			if (!gotExtraBuildData)
			{
				if (fromBuild && CustomBuildReport.Util.HasBuildTime())
				{
					var timeBuildStarted = CustomBuildReport.Util.LoadBuildTime();
					buildInfo.BuildDurationTime = CustomBuildReport.Util.LoadBuildTimeDuration();

					buildInfo.BuildTimeGot = timeBuildStarted;
					buildInfo.BuildTimeGotReadable = timeBuildStarted.ToString(TIME_OF_BUILD_FORMAT);
				}
				else
				{
					buildInfo.BuildDurationTime = new TimeSpan(0);
					buildInfo.BuildTimeGot = new DateTime();
					buildInfo.BuildTimeGotReadable = string.Empty;
				}
			}

			_lastSavePath = CustomBuildReport.Options.BuildReportSavePath;
			_lastAssemblies = CompilationPipeline.GetAssemblies();
		}

#if UNITY_2018_1_OR_NEWER
		public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
#else
		public void OnPreprocessBuild(BuildTarget target, string pathToBuiltProject)
#endif
		{
			if (!CustomBuildReport.Options.CollectBuildInfo)
			{
				return;
			}

			OnPreBuild();
		}

#if UNITY_2018_1_OR_NEWER
		public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
#elif UNITY_5_6_OR_NEWER
		public void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
#else
		[UnityEditor.Callbacks.PostProcessBuildAttribute(1)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
#endif
		{
			if (!Application.isEditor || Application.isPlaying)
			{
				return;
			}

			//Debug.Log("post process build called in editor. pathToBuiltProject: " + pathToBuiltProject);

#if UNITY_2018_1_OR_NEWER
			if (!string.IsNullOrEmpty(report.summary.outputPath))
			{
				_lastPathToBuiltProject = report.summary.outputPath;
			}
#else
			if (!string.IsNullOrEmpty(pathToBuiltProject))
			{
				_lastPathToBuiltProject = pathToBuiltProject;
			}
#endif

			CustomBuildReport.Util.BuildTargetOfLastBuild = EditorUserBuildSettings.activeBuildTarget;
			//Debug.Log("OnPostprocessBuild: got new BuildTargetOfLastBuild: " + CustomBuildReport.Util.BuildTargetOfLastBuild);

			if (!CustomBuildReport.Options.CollectBuildInfo)
			{
				return;
			}

			// Note: useless to call Init() and CommitAdditionalInfoToCache() here since an assembly reload will happen

			// Record the time it took to get from OnPreprocessBuild to now (OnPostprocessBuild)
			// (this value is saved in an xml file, so it will survive the assembly reload).
			// Note: There's a report.summary.totalTime, but oftentimes it has a value of 00:00:00
			// because report.summary.buildStartedAt and report.summary.buildEndedAt have the same value,
			// so we rely on our own time recording instead. There's still the option to use the BuildSummary
			// using the BRT_USE_BUILD_SUMMARY_TIME scripting define.
#if UNITY_2018_1_OR_NEWER
#if BRT_USE_BUILD_SUMMARY_TIME
			CustomBuildReport.Util.SaveBuildTime(report.summary.buildStartedAt);
			CustomBuildReport.Util.SaveBuildTimeDuration(report.summary.totalTime);
#else
			CustomBuildReport.Util.SaveBuildTimeDuration();
#endif

			//CustomBuildReport.Util.DebugLogBuildReport(report);

			// Since there will be an assembly reload, we can't just store `report` into a variable
			// and expect to be able to access it using that variable later.
			// We have to save the data to a file then read that file later.
			CustomBuildReport.Util.SaveUnityBuildReportToCurrent(report);
#else
			CustomBuildReport.Util.SaveBuildTimeDuration();
#endif

			// Later on, in BRT_BuildReportWindow.OnInspectorUpdate(),
			// when `CustomBuildReport.ReportGenerator.IsFinishedGettingValues` is true,
			// the code will finally save the created build report
			// (this value is saved in an xml file, so it will survive the assembly reload).
			CustomBuildReport.Util.SaveGetBuildReportNow();

			if (BRT_BuildReportWindow.IsOpen || CustomBuildReport.Options.ShouldShowWindowAfterBuild)
			{
				ShowBuildReportWithLastValues();
			}

			//Debug.Log("post process build finished");
		}

		static void AddAllPrefabsUsedInScene(string sceneFilename)
		{
#if UNITY_5_3_OR_NEWER
			string[] assetsUsedInScene = AssetDatabase.GetDependencies(sceneFilename);
#else
			string[] assetsUsedInScene = AssetDatabase.GetDependencies(new []{sceneFilename});
#endif

			//Debug.Log(string.Format("AddAllPrefabsUsedInScene() {0}: {1}", sceneFilename, assetsUsedInScene.Length.ToString()));

			for (int n = 0, len = assetsUsedInScene.Length; n < len; ++n)
			{
				var assetInScene = assetsUsedInScene[n];
				//Debug.Log($"  {n.ToString()}: {assetInScene}");

				// check for prefab and all known 3d model file types that Unity supports
				if (assetInScene.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) ||
				    assetInScene.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) ||
				    assetInScene.EndsWith(".dae", StringComparison.OrdinalIgnoreCase) ||
				    assetInScene.EndsWith(".mb", StringComparison.OrdinalIgnoreCase) ||
				    assetInScene.EndsWith(".ma", StringComparison.OrdinalIgnoreCase) ||
				    assetInScene.EndsWith(".max", StringComparison.OrdinalIgnoreCase) ||
				    assetInScene.EndsWith(".blend", StringComparison.OrdinalIgnoreCase) ||
				    assetInScene.EndsWith(".obj", StringComparison.OrdinalIgnoreCase) ||
				    assetInScene.EndsWith(".3ds", StringComparison.OrdinalIgnoreCase) ||
				    assetInScene.EndsWith(".dxf", StringComparison.OrdinalIgnoreCase))
				{
					if (!PrefabsUsedInScenes.Contains(assetInScene))
					{
						//Debug.Log($"    added prefab used: {assetInScene} from scene {sceneFilename}");
						PrefabsUsedInScenes.Add(assetInScene);
					}
				}
			}
		}

		static void ClearListOfAllPrefabsUsedInAllScenes()
		{
			PrefabsUsedInScenes.Clear();
		}

		static void RefreshListOfAllPrefabsUsedInAllScenesIncludedInBuild()
		{
			ClearListOfAllPrefabsUsedInAllScenes();

			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				//Debug.Log(S.path);
				if (scene != null && !string.IsNullOrEmpty(scene.path) && scene.enabled) // is checkbox for this scene in build settings checked?
				{
					AddAllPrefabsUsedInScene(scene.path);
				}
			}
		}

		static void CommitAdditionalInfoToCache()
		{
			if (PrefabsUsedInScenes != null)
			{
				//Debug.Log("addInfo: " + (addInfo != null));

				//buildInfo.PrefabsUsedInScenes = new string[_prefabsUsedInScenes.Keys.Count];
				//_prefabsUsedInScenes.Keys.CopyTo(buildInfo.PrefabsUsedInScenes, 0);

				PrefabsUsedInScenesList.Clear();
				PrefabsUsedInScenesList.AddRange(PrefabsUsedInScenes);

				//Debug.Log("assigned to addInfo.PrefabsUsedInScenes: " + addInfo.PrefabsUsedInScenes.Length);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public static string GetUnityVersionFromEditorLog(string editorLogPath)
		{
			if (!File.Exists(editorLogPath))
			{
				return null;
			}

			const string ENGINE_VERSION_KEY = "Initialize engine version: ";

			string gotLine = DldUtil.BigFileReader.SeekFirstText(editorLogPath, ENGINE_VERSION_KEY);

			if (string.IsNullOrWhiteSpace(gotLine))
			{
				return null;
			}

			int versionIdx = gotLine.LastIndexOf(ENGINE_VERSION_KEY, StringComparison.Ordinal);
			if (versionIdx == -1)
			{
				return string.Empty;
			}

			return gotLine.Substring(versionIdx + ENGINE_VERSION_KEY.Length, gotLine.Length - (versionIdx + ENGINE_VERSION_KEY.Length));
		}

		public static string GetUnityEditorPathFromEditorLog(string editorLogPath)
		{
			if (!File.Exists(editorLogPath))
			{
				return null;
			}

			const string EDITOR_KEY = @"\Unity.exe";
			const string EDITOR_KEY_2 = "/Unity.exe";
			const string EDITOR_KEY_3 = "COMMAND LINE ARGUMENTS:";

			string gotLine = DldUtil.BigFileReader.SeekFirstTextEndingWith(editorLogPath, EDITOR_KEY, EDITOR_KEY_2);

			if (string.IsNullOrWhiteSpace(gotLine))
			{
				gotLine = DldUtil.BigFileReader.SeekNextLineAfter(editorLogPath, EDITOR_KEY_3);
			}

			if (string.IsNullOrWhiteSpace(gotLine))
			{
				return null;
			}

			int lastPipeIdx = gotLine.LastIndexOf('|');
			if (lastPipeIdx != -1 && lastPipeIdx < editorLogPath.Length - 1)
			{
				gotLine = gotLine.Substring(lastPipeIdx+1);
			}

			if (string.IsNullOrWhiteSpace(gotLine))
			{
				return null;
			}

			return gotLine.Replace(@"\", "/");
		}

		public static string GetProjectAssetsPathFromEditorLog(string editorLogPath)
		{
			if (!File.Exists(editorLogPath))
			{
				return null;
			}

			const string PATH_KEY = "WorkingDir: ";

			string gotLine = DldUtil.BigFileReader.SeekLastText(editorLogPath, PATH_KEY);

			if (string.IsNullOrWhiteSpace(gotLine))
			{
				return null;
			}

			int keyIdx = gotLine.IndexOf(PATH_KEY, StringComparison.Ordinal);
			if (keyIdx > 0)
			{
				string datePrefix = gotLine.Substring(0, 11);

				int lastDateIdx = gotLine.LastIndexOf(datePrefix, StringComparison.Ordinal);
				if (lastDateIdx > 0)
				{
					gotLine = gotLine.Substring(0, lastDateIdx);
				}
				gotLine = gotLine.Substring(keyIdx + PATH_KEY.Length, gotLine.Length - (keyIdx + PATH_KEY.Length));

				return gotLine;
			}

			return gotLine.Substring(PATH_KEY.Length);
		}

		public static string GetBuildTypeFromEditorLog(string editorLogPath)
		{
			if (!File.Exists(editorLogPath))
			{
				return null;
			}

			const string BUILD_TYPE_KEY = "*** Completed 'Build.";
			const string CANCELED_BUILD_TYPE_KEY = "*** Canceled 'Build.";

			string returnValue = GetBuildTypeFromEditorLog(editorLogPath, BUILD_TYPE_KEY);
			if (string.IsNullOrEmpty(returnValue))
			{
				returnValue = GetBuildTypeFromEditorLog(editorLogPath, CANCELED_BUILD_TYPE_KEY);
			}

			return returnValue;
		}

		static string GetBuildTypeFromEditorLog(string editorLogPath, string buildTypeKey)
		{
			if (!File.Exists(editorLogPath))
			{
				return null;
			}

			//Debug.Log("GetBuildTypeFromEditorLog path: " + editorLogPath);
			var gotLines = DldUtil.BigFileReader.SeekAllText(editorLogPath, buildTypeKey);

			if (gotLines.Count == 0)
			{
				//Debug.LogFormat("no buildType got");
				return string.Empty;
			}

			var lastLine = gotLines[gotLines.Count - 1].Text;

			if (!string.IsNullOrEmpty(lastLine))
			{
				//Debug.LogFormat("GetBuildTypeFromEditorLog line: {0} for key: {1}", line, buildTypeKey);

				int buildTypeIdx = lastLine.LastIndexOf(buildTypeKey, StringComparison.Ordinal);
				//Debug.Log("buildTypeIdx: " + buildTypeIdx);

				if (buildTypeIdx == -1)
				{
					return string.Empty;
				}

				int buildTypeEndIdx = lastLine.IndexOf("' in ", buildTypeIdx, StringComparison.Ordinal);
				//Debug.Log("buildTypeEndIdx: " + buildTypeEndIdx);

				string buildType = lastLine.Substring(buildTypeIdx + buildTypeKey.Length,
					buildTypeEndIdx - buildTypeIdx - buildTypeKey.Length);

				int anotherDotIdx = buildType.IndexOf(".", StringComparison.Ordinal);
				if (anotherDotIdx > -1)
				{
					buildType = buildType.Substring(anotherDotIdx + 1, buildType.Length - anotherDotIdx - 1);
				}

				//Debug.LogFormat("buildType got: {0}", buildType);
				return buildType;
			}
			//else
			//{
			//	Debug.LogFormat("no buildType got");
			//}

			return string.Empty;
		}

		static bool HasInvalidPercentValue(string line)
		{
			return line.IndexOf("inf%", StringComparison.Ordinal) >= 0 ||
			       line.IndexOf("nan%", StringComparison.Ordinal) >= 0 ||
			       line.IndexOf("-1.$%", StringComparison.Ordinal) >= 0 ||
			       line.IndexOf("1.$%", StringComparison.Ordinal) >= 0;
		}

		const string SIZE_PARTS_KEY = "Textures      ";
		const string DATE_TIME_PREFIX =
			@"\d{4}-(0[1-9]|1[012])-([012]\d|3[01])T([01]\d|2[0-3]):([0-5]\d):([0-5]\d)\.\d{3}Z\|0x[\da-f]{4,}\|";

		const string TOTAL_USER_ASSETS_SIZE_KEY = "Total User Assets";

		static CustomBuildReport.SizePart[] ParseSizePartsFromString(string editorLogPath)
		{
			// now parse the build parts to an array of `CustomBuildReport.SizePart`
			List<CustomBuildReport.SizePart> buildSizes = new List<CustomBuildReport.SizePart>();

			bool gotDateTimePrefix = false;
			int dateTimePrefixLen = 0;

			foreach (string line in DldUtil.BigFileReader.ReadFile(editorLogPath, false, true, SIZE_PARTS_KEY))
			{
				string gotLine = line;
				if (!gotDateTimePrefix)
				{
					Match dateTime = Regex.Match(line, DATE_TIME_PREFIX, RegexOptions.IgnoreCase);
					if (dateTime.Success)
					{
						dateTimePrefixLen = dateTime.Groups[0].Value.Length;
						gotDateTimePrefix = true;
						gotLine = line.Substring(dateTimePrefixLen);
					}
				}
				else if (dateTimePrefixLen > 0 && line.Length > dateTimePrefixLen && char.IsDigit(line[0]))
				{
					gotLine = line.Substring(dateTimePrefixLen);
				}

				CustomBuildReport.SizePart inPart = CreateSizePartFromLine(gotLine, true, out bool stop);

				if (stop)
				{
					break;
				}

				buildSizes.Add(inPart);
			}

			CustomBuildReport.SizePart streamingAssetsSize = new CustomBuildReport.SizePart();
			streamingAssetsSize.SetNameToStreamingAssets();
			streamingAssetsSize.Size = "0";
			streamingAssetsSize.SizeBytes = 0;
			streamingAssetsSize.Percentage = 0;

			buildSizes.Add(streamingAssetsSize);

			return buildSizes.ToArray();
		}

		static CustomBuildReport.SizePart CreateSizePartFromLine(string line, bool ignoreTotalUserAssets, out bool stop)
		{
			// blank line signifies end of list
			if (string.IsNullOrEmpty(line) || line == "\n" || line == "\r\n")
			{
				stop = true;
				return null;
			}
			//Debug.LogFormat("ParseSizePartsFromString: line:\n{0}", line);

			string b = line;

			string gotName = "???";
			string gotSize = "?";
			string gotPercent;

			Match match = Regex.Match(b, @"[a-z \t]+[^0-9]", RegexOptions.IgnoreCase);
			if (match.Success)
			{
				gotName = match.Groups[0].Value;
				gotName = gotName.Trim();

				if (gotName == "Included DLLs")
				{
					gotName = "System DLLs";
				}

				if (ignoreTotalUserAssets && gotName == TOTAL_USER_ASSETS_SIZE_KEY)
				{
					// No need for this, we calculate our own total size.
					// The "Total User Assets" entry also signifies the
					// last part has been parsed already, so no need
					// to process further.
					stop = true;
					return null;
				}

				//Debug.LogFormat("    got name: {0}", gotName);
			}

			match = Regex.Match(b, @"[0-9.]+ (kb|mb|b|gb)", RegexOptions.IgnoreCase);
			if (match.Success)
			{
				gotSize = match.Groups[0].Value.ToUpper();
				//Debug.LogFormat("    got size: {0}", gotSize);
			}

			if (HasInvalidPercentValue(b))
			{
				gotPercent = "0";
				//Debug.LogFormat("    got percent (inf): {0}", gotPercent);
			}
			else
			{
				match = Regex.Match(b, @"[0-9.]+%", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					gotPercent = match.Groups[0].Value;
					gotPercent = gotPercent.Substring(0, gotPercent.Length - 1);
					//Debug.LogFormat("    got percent: {0}", gotPercent);
				}
				else
				{
					gotPercent = "0";
				}
			}

			if ((ignoreTotalUserAssets && line.IndexOf("100.0%", StringComparison.Ordinal) != -1) ||
			    line.IndexOf("nan%", StringComparison.Ordinal) != -1 ||
			    gotName.IndexOf("Complete size", StringComparison.Ordinal) != -1 ||
			    gotName.IndexOf("Complete build size", StringComparison.Ordinal) != -1)
			{
				// that was the final part of the list
				stop = true;
				return null;
			}

			CustomBuildReport.SizePart inPart = new CustomBuildReport.SizePart();
			inPart.Name = gotName;
			inPart.Size = gotSize;
			inPart.Percentage = double.Parse(gotPercent, CultureInfo.InvariantCulture);
			inPart.DerivedSize = CustomBuildReport.Util.GetApproxSizeFromString(gotSize);

			//Debug.LogFormat("SizePart: {0} size: {1} percent: {2}", inPart.Name, inPart.Size, inPart.Percentage);

			stop = false;
			return inPart;
		}

		const string ASSET_SIZES_KEY = "Used Assets, sorted by uncompressed size:";
		const string ASSET_SIZES_KEY_2 = "Used Assets and files from the Resources folder, sorted by uncompressed size:";

		static List<CustomBuildReport.SizePart> ParseAssetSizesFromEditorLog(string editorLogPath,
			List<string> prefabsUsedInScenes)
		{
			List<CustomBuildReport.SizePart> assetSizes = new List<CustomBuildReport.SizePart>();
			HashSet<string> prefabsInBuildDict = new HashSet<string>();


			// note: list gotten from editor log is already sorted by raw size, descending

			foreach (string line in DldUtil.BigFileReader.ReadFile(editorLogPath, true, true,
				         ASSET_SIZES_KEY, ASSET_SIZES_KEY_2))
			{
				CustomBuildReport.SizePart assetSizePart = CreateAssetSizeFromLine(line, prefabsInBuildDict, out bool stop);
				if (stop)
				{
					break;
				}

				if (assetSizePart != null)
				{
					assetSizes.Add(assetSizePart);
				}
			}

			// Additional Step:
			// include prefabs that are instantiated in scenes (they are not by default)
			//Debug.Log("addInfo.PrefabsUsedInScenes: " + addInfo.PrefabsUsedInScenes.Length);
			foreach (string p in prefabsUsedInScenes)
			{
				if (p.IndexOf("/Resources/", StringComparison.Ordinal) != -1)
					continue; // prefabs in resources folder are already included in the editor log build info

				if (prefabsInBuildDict.Contains(p)) continue; // if already in assetSizes, continue

				CustomBuildReport.SizePart inPart = new CustomBuildReport.SizePart();
				inPart.Name = p;
				inPart.Size = "N/A";
				inPart.Percentage = -1;

				//Debug.Log("   prefab added in used assets: " + p);

				assetSizes.Add(inPart);
			}

			return assetSizes;
		}

		static CustomBuildReport.SizePart CreateAssetSizeFromLine(string line, HashSet<string> prefabsInBuildDict, out bool stop)
		{
			if (string.IsNullOrEmpty(line) || line == "\n" || line == "\r\n")
			{
				stop = true;
				return null;
			}

			var input = line.Replace("\\", "/");

			//Debug.LogFormat("from line: {0}", line);

			Match match = Regex.Match(input, @"^.* [0-9]+\.[0-9]+ (kb|mb|b|gb|tb)\s+[0-9.]+%\s+.+", RegexOptions.IgnoreCase);
			if (match.Success)
			{
				stop = false;
				// it's an asset entry. parse it
				//string b = match.Groups[0].Value;

				string gotName = "???";
				string gotSize = "?";
				string gotPercent = "?";

				match = Regex.Match(input, @"Assets/.+", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					gotName = match.Groups[0].Value;
					gotName = gotName.Trim();
					//Debug.Log("    name? " + gotName);
				}
				else
				{
					match = Regex.Match(input, @"Built-in.+:.+", RegexOptions.IgnoreCase);
					if (match.Success)
					{
						gotName = match.Groups[0].Value;
						gotName = gotName.Trim();
						//Debug.Log("    built-in?: " + gotName);
					}
					else
					{
						match = Regex.Match(input, @"Resources/.+", RegexOptions.IgnoreCase);
						if (match.Success)
						{
							gotName = match.Groups[0].Value;
							gotName = gotName.Trim();
							//Debug.Log("    built-in?: " + gotName);
						}
						else
						{
							match = Regex.Match(input, @"UnityExtensions/.+", RegexOptions.IgnoreCase);
							if (match.Success)
							{
								gotName = match.Groups[0].Value;
								gotName = gotName.Trim();
								//Debug.Log("    extension?: " + gotName);
							}
							else
							{
								match = Regex.Match(input, @"Packages/.+", RegexOptions.IgnoreCase);
								if (match.Success)
								{
									gotName = match.Groups[0].Value;
									gotName = gotName.Trim();
									//Debug.Log("    extension?: " + gotName);
								}
								else
								{
									match = Regex.Match(input, @"AssetBundle Object$", RegexOptions.IgnoreCase);
									if (match.Success)
									{
										gotName = "AssetBundle Object";
									}
								}
							}
						}
					}
				}

				match = Regex.Match(input, @"[0-9.]+ (kb|mb|b|gb|tb)", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					gotSize = match.Groups[0].Value.ToUpper();
					//Debug.Log("    size? " + gotSize);
				}
				else
				{
					Debug.Log("didn't find size for :" + input);
				}

				if (HasInvalidPercentValue(input))
				{
					gotPercent = "0";
				}
				else
				{
					match = Regex.Match(input, @"[0-9.]+%", RegexOptions.IgnoreCase);
					if (match.Success)
					{
						gotPercent = match.Groups[0].Value;
						gotPercent = gotPercent.Substring(0, gotPercent.Length - 1);
						//Debug.Log("    percent? " + gotPercent);
					}
					else
					{
						Debug.Log("didn't find percent for :" + input);
					}
				}
				//Debug.LogFormat("got: {0} size: {1} percent: {2}", gotName, gotSize, gotPercent);

				// UnityEngine dll files show up in the used assets list so don't add them in
				// (those will be in a separate list)
				var filename = gotName.GetFileNameOnly();
				if (filename.IsFileOfType(".dll") && CustomBuildReport.Util.IsAUnityEngineDLL(filename))
				{
					//Debug.Log("Found UnityEngine dll in Used Assets: " + filename);
					return null;
				}
				else
				{
					CustomBuildReport.SizePart inPart = new CustomBuildReport.SizePart();
					inPart.Name = System.Security.SecurityElement.Escape(gotName);
					inPart.Size = gotSize;
					inPart.SizeBytes = -1;
					inPart.DerivedSize = CustomBuildReport.Util.GetApproxSizeFromString(gotSize);
					inPart.Percentage = Double.Parse(gotPercent, CultureInfo.InvariantCulture);


					// since this is a used asset, the size we got from the editor log *is* already the imported size
					// so don't bother computing imported size.
					long importedSizeBytes = -1;
					inPart.ImportedSizeBytes = importedSizeBytes;
					inPart.ImportedSize = CustomBuildReport.Util.GetBytesReadable(importedSizeBytes);

					//if (inPart.Name.IndexOf("Rocks_lighup.tif") > -1)
					//{
					//	Debug.LogFormat("Rocks_lighup.tif: got Size: {0} Imported Size: {1}", inPart.Size, inPart.ImportedSize);
					//}

					if (prefabsInBuildDict != null)
					{
						if (gotName.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) ||
						    gotName.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) ||
						    gotName.EndsWith(".dae", StringComparison.OrdinalIgnoreCase) ||
						    gotName.EndsWith(".mb", StringComparison.OrdinalIgnoreCase) ||
						    gotName.EndsWith(".ma", StringComparison.OrdinalIgnoreCase) ||
						    gotName.EndsWith(".max", StringComparison.OrdinalIgnoreCase) ||
						    gotName.EndsWith(".blend", StringComparison.OrdinalIgnoreCase) ||
						    gotName.EndsWith(".obj", StringComparison.OrdinalIgnoreCase) ||
						    gotName.EndsWith(".3ds", StringComparison.OrdinalIgnoreCase) ||
						    gotName.EndsWith(".dxf", StringComparison.OrdinalIgnoreCase))
						{
							prefabsInBuildDict.Add(gotName);
						}
					}

					return inPart;
				}
			}
			else
			{
				stop = true;
				return null;
			}
		}

		public static CustomBuildReport.SizePart[][] SegregateAssetSizesPerCategory(
			CustomBuildReport.SizePart[] assetSizesAll, FileFilterGroup filters)
		{
			if (assetSizesAll == null || assetSizesAll.Length == 0) return null;

			// we do filters.Count+1 for Unrecognized category
			List<List<CustomBuildReport.SizePart>> ret = new List<List<CustomBuildReport.SizePart>>(filters.Count + 1);
			for (int n = 0, len = filters.Count + 1; n < len; ++n)
			{
				ret.Add(new List<CustomBuildReport.SizePart>());
			}

			for (int idxAll = 0, lenAll = assetSizesAll.Length; idxAll < lenAll; ++idxAll)
			{
				BRT_BuildReportWindow.GetValueMessage =
					string.Format("Segregating assets {0} of {1}...", (idxAll + 1).ToString(), assetSizesAll.Length.ToString());

				var foundAtLeastOneMatch = false;
				for (int n = 0, len = filters.Count; n < len; ++n)
				{
					if (filters[n].IsFileInFilter(assetSizesAll[idxAll].Name))
					{
						foundAtLeastOneMatch = true;
						ret[n].Add(assetSizesAll[idxAll]);
					}
				}

				if (!foundAtLeastOneMatch)
				{
					ret[ret.Count - 1].Add(assetSizesAll[idxAll]);
				}
			}

			BRT_BuildReportWindow.GetValueMessage = "";

			CustomBuildReport.SizePart[][] retArr = new CustomBuildReport.SizePart[filters.Count + 1][];
			for (int n = 0, len = filters.Count + 1; n < len; ++n)
			{
				retArr[n] = ret[n].ToArray();
			}

			return retArr;
		}


		public static void MoveUnusedAssetsBatchToNext(BuildInfo buildInfo, FileFilterGroup filtersToUse)
		{
			buildInfo.MoveUnusedAssetsBatchNumToNext();
			RefreshUnusedAssetsBatch(buildInfo, filtersToUse);
		}

		public static void MoveUnusedAssetsBatchToPrev(BuildInfo buildInfo, FileFilterGroup filtersToUse)
		{
			if (buildInfo.UnusedAssetsBatchIdx == 0)
			{
				return;
			}

			buildInfo.MoveUnusedAssetsBatchNumToPrev();
			RefreshUnusedAssetsBatch(buildInfo, filtersToUse);
		}

		static void RefreshUnusedAssetsBatch(BuildInfo buildInfo, FileFilterGroup filtersToUse)
		{
			if (buildInfo.UnusedAssetsIncludedInCreation)
			{
				BRT_BuildReportWindow.GetValueMessage = "Getting list of unused assets...";

				List<CustomBuildReport.SizePart> allUsed = buildInfo.UsedAssets.GetAllAsList();

				CustomBuildReport.SizePart[] allUnused;
				CustomBuildReport.SizePart[][] perCategoryUnused;

				BuildPlatform buildPlatform = GetBuildPlatformFromString(buildInfo.BuildType, buildInfo.BuildTargetUsed);


				allUnused = GetAllUnusedAssets(buildInfo, buildInfo.ProjectAssetsPath, buildPlatform, allUsed);

				if (allUnused != null && allUnused.Length > 0)
				{
					perCategoryUnused = SegregateAssetSizesPerCategory(allUnused, filtersToUse);

					AssetList.SortType previousUnusedSortType = buildInfo.UnusedAssets.LastSortType;
					AssetList.SortOrder previousUnusedSortOrder = buildInfo.UnusedAssets.LastSortOrder;

					buildInfo.UnusedAssets = new AssetList();
					buildInfo.UnusedAssets.Init(allUnused, perCategoryUnused,
						CustomBuildReport.Options.NumberOfTopLargestUnusedAssetsToShow, filtersToUse,
						previousUnusedSortType, previousUnusedSortOrder);
					buildInfo.UnusedAssets.PopulateImportedSizes();

					if (allUsed.Count != buildInfo.UsedAssets.AllCount)
					{
						// it means GetAllUnusedAssets() found new used assets
						// (something from the StreamingAssets or Resources folder, a dll, etc.)
						// re-assign it to the all used list in the build report, and re-sort
						CustomBuildReport.SizePart[] newAllUsedArray = allUsed.ToArray();

						CustomBuildReport.SizePart[][] newPerCategoryUsed =
							SegregateAssetSizesPerCategory(newAllUsedArray, filtersToUse);


						AssetList.SortType previousUsedSortType = buildInfo.UsedAssets.LastSortType;
						AssetList.SortOrder previousUsedSortOrder = buildInfo.UsedAssets.LastSortOrder;

						buildInfo.UsedAssets = new AssetList();
						buildInfo.UsedAssets.Init(newAllUsedArray, newPerCategoryUsed,
							CustomBuildReport.Options.NumberOfTopLargestUsedAssetsToShow, filtersToUse,
							previousUsedSortType, previousUsedSortOrder);
						buildInfo.UsedAssets.PopulateImportedSizes();
					}
				}
				else
				{
					// no assets found. this only happens when we tried to move to next batch but it turns out to be the last
					// so we move back
					buildInfo.MoveUnusedAssetsBatchNumToPrev();
				}


				BRT_BuildReportWindow.GetValueMessage = "";

				buildInfo.FlagOkToRefresh();
			}
		}

		static void AssignAllUnusedAssets(CustomBuildReport.BuildInfo buildInfo,
			string projectAssetsPath,
			BuildPlatform buildPlatform,
			List<CustomBuildReport.SizePart> inOutAllUsedAssets)
		{
			// Note: We pass the inOutAllUsedAssets list because our checks can add these assets as used:
			// 1. Resources assets
			// 2. StreamingAssets
			// 3. Managed DLLs
			// 4. For Android build: .jar files in Assets/Plugins/Android/
			// 5. For iOS build: any .a, .m, .mm, .c, or .cpp files in Assets/Plugins/iOS
			// 6. For Mac build: .bundle files in Assets/Plugins/
			// 7. For Windows build: .dll files in Assets/Plugins/
			// 8. For Linux build: .so files in Assets/Plugins/
			buildInfo.ResetUnusedAssetsBatchData();
			var allUnused = GetAllUnusedAssets(buildInfo, projectAssetsPath, buildPlatform, inOutAllUsedAssets);

			var perCategoryUnused = SegregateAssetSizesPerCategory(allUnused, buildInfo.FileFilters);

			buildInfo.UnusedAssets = new AssetList();
			buildInfo.UnusedAssets.Init(allUnused, perCategoryUnused,
				CustomBuildReport.Options.NumberOfTopLargestUnusedAssetsToShow, buildInfo.FileFilters);

			buildInfo.UnusedTotalSize =
				CustomBuildReport.Util.GetBytesReadable(buildInfo.UnusedAssets.GetTotalSizeInBytes());
		}

		static CustomBuildReport.SizePart[] GetAllUnusedAssets(
			CustomBuildReport.BuildInfo buildInfo,
			string projectAssetsPath,
			BuildPlatform buildPlatform,
			List<CustomBuildReport.SizePart> inOutAllUsedAssets)
		{
			CustomBuildReport.SizePart[] scriptDLLs = buildInfo.ScriptDLLs;
			bool includeSvn = buildInfo.IncludedSvnInUnused;
			bool includeGit = buildInfo.IncludedGitInUnused;
			bool includeBrt = buildInfo.IncludedBuildReportToolAssetsInUnused;
			List<SavedOptions.IgnorePattern> ignorePatterns = buildInfo.IgnorePatternsForUnused;
			bool includeUnusedPrefabs = buildInfo.UnusedPrefabsIncludedInCreation;
			bool processInBatches = buildInfo.ProcessUnusedAssetsInBatches;
			int fileCountLimit = buildInfo.UnusedAssetsEntriesPerBatch;

			List<CustomBuildReport.SizePart> unusedAssets = new List<CustomBuildReport.SizePart>();


			// now loop through all assets in the whole project,
			// check if that file exists in the usedAssetsDict,
			// if not, include it in the unusedAssets list,
			// then sort by size

			int projectStringLen = projectAssetsPath.Length - "Assets".Length;

			bool has32BitPluginsFolder = Directory.Exists(projectAssetsPath + "/Plugins/x86");
			bool has64BitPluginsFolder = Directory.Exists(projectAssetsPath + "/Plugins/x86_64");

			string currentAsset;
			bool prevSkipped = false;

			int assetNum = 0;

			int fileCountOffset;
			if (buildInfo.UnusedAssetsBatchIdx > 0 &&
			    buildInfo.UnusedAssetsBatchIdx <= buildInfo.UnusedAssetsBatchFinalNum.Count)
			{
				// use the asset num of the previous batch
				fileCountOffset = buildInfo.UnusedAssetsBatchFinalNum[buildInfo.UnusedAssetsBatchIdx-1];
			}
			else
			{
				// just guess based on the batch count
				fileCountOffset = buildInfo.UnusedAssetsBatchIdx * fileCountLimit;
			}

			foreach (string fullAssetPath in DldUtil.TraverseDirectory.Do(projectAssetsPath))
			{
				++assetNum;

				if (processInBatches && assetNum <= fileCountOffset)
				{
					prevSkipped = true;
					continue;
				}

				if (prevSkipped)
				{
					prevSkipped = false;
				}

				BRT_BuildReportWindow.GetValueMessage =
					string.Format("Getting list of used assets {0} ...", assetNum.ToString());

				//string fullAssetPath = allAssets[assetIdx];

				// get the path but starting from the "Assets/" folder
				currentAsset = fullAssetPath.Substring(projectStringLen, fullAssetPath.Length - projectStringLen).Replace("\\", "/");

				//Debug.Log(currentAsset);

				// --------------------------
				// Unity .meta files are not considered part of the assets
				// Unity .mask (Avatar masks): whether a .mask file is used or not currently cannot be reliably found out, so they are skipped
				if (Util.IsFileOfType(currentAsset, ".meta") ||
				    Util.IsFileOfType(currentAsset, ".mask"))
				{
					continue;
				}

				// --------------------------
				// anything in a /Resources/ folder will always be in the build, as long as it's not in an Editor folder
				if (Util.IsFileInAPath(currentAsset, "/Resources/") && !Util.IsFileInAnEditorFolder(currentAsset))
				{
					// ensure this Resources asset is in the used assets list
					if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
						    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
					{
						inOutAllUsedAssets.Add(CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
					}

					continue;
				}

				// --------------------------
				// Include version control files only if requested to do so
				if (!includeSvn && Util.IsFileInAPath(currentAsset, "/.svn/"))
				{
					continue;
				}

				if (!includeGit && Util.IsFileInAPath(currentAsset, "/.git/"))
				{
					continue;
				}

				if (!includeBrt && Util.IsFileInAPath(currentAsset, "/BuildReport/"))
				{
					continue;
				}

				if (ignorePatterns != null && ignorePatterns.Count > 0)
				{
					bool currentAssetMatchedIgnore = false;
					for (int p = 0, pLen = ignorePatterns.Count; p < pLen; ++p)
					{
						if (string.IsNullOrEmpty(ignorePatterns[p].Pattern))
						{
							continue;
						}

						bool match;
						switch (ignorePatterns[p].SearchType)
						{
							case SavedOptions.SEARCH_METHOD_REGEX:
								try
								{
									match = System.Text.RegularExpressions.Regex.IsMatch(currentAsset, ignorePatterns[p].Pattern, RegexOptions.CultureInvariant);
								}
								catch (ArgumentException)
								{
									match = false;
								}
								break;
							default:
								// default SearchType is Basic
								match = System.Text.RegularExpressions.Regex.IsMatch(currentAsset, CustomBuildReport.Util.WildCardToRegex(ignorePatterns[p].Pattern), RegexOptions.CultureInvariant);
								break;
						}

						if (match)
						{
							currentAssetMatchedIgnore = true;
							break;
						}
					}

					if (currentAssetMatchedIgnore)
					{
						continue;
					}
				}

				// --------------------------
				// NOTE: if a .dll is present in the Script DLLs list, that means
				// it is a managed DLL, and thus, is always used in the build

				if (scriptDLLs != null && Util.IsFileOfType(currentAsset, ".dll"))
				{
					string assetFilenameOnly = currentAsset.GetFileNameOnly();
					//Debug.Log(assetFilenameOnly);

					bool foundMatch = false;

					// is current asset found in the script/managed DLLs list?
					for (int mdllIdx = 0; mdllIdx < scriptDLLs.Length; ++mdllIdx)
					{
						if (scriptDLLs[mdllIdx].Name == assetFilenameOnly)
						{
							// it's a managed DLL. Managed DLLs are always included in the build.
							foundMatch = true;
							var sizePartForThisScriptDLL =
								CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath);

							if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
								    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
							{
								inOutAllUsedAssets.Add(sizePartForThisScriptDLL);
							}

							// update the file size in the build report with the values that we found
							scriptDLLs[mdllIdx].Percentage = sizePartForThisScriptDLL.Percentage;
							scriptDLLs[mdllIdx].RawSize = sizePartForThisScriptDLL.RawSize;
							scriptDLLs[mdllIdx].RawSizeBytes = sizePartForThisScriptDLL.RawSizeBytes;
							scriptDLLs[mdllIdx].DerivedSize = sizePartForThisScriptDLL.DerivedSize;
							scriptDLLs[mdllIdx].ImportedSize = sizePartForThisScriptDLL.ImportedSize;
							scriptDLLs[mdllIdx].ImportedSizeBytes = sizePartForThisScriptDLL.ImportedSizeBytes;

							break;
						}
					}

					if (foundMatch)
					{
						// this DLL file has been taken into account since it was detected to be a managed DLL
						// so move on to the next file
						continue;
					}
				}


				// per platform special cases
				// involving native plugins

				// in windows and linux, the issue gets dicey as we have to check if its a 32 bit, 64 bit, or universal build

				// so for windows/linux 32 bit, if Assets/Plugins/x86 exists, it will include all dll/so in those. if that folder does not exist, all dll/so in Assets/Plugins are included instead.
				//
				// what if there's a 64 bit dll/so in Assets/Plugins? surely it would not get included in a 32 bit build?

				// for windows/linux 64 bit, if Assets/Plugins/x86_64 exists, it will include all dll/so in those. if that folder does not exist, all dll/so in Assets/Plugins are included instead.

				// right now there is no such thing as a windows universal build

				// For linux universal build, any .so in Assets/Plugins/x86 and Assets/Plugins/x86_64 are included. No .so in Assets/Plugins will be included (as it wouldn't be able to determine if such an .so in that folder is 32 or 64 bit) i.e. it relies on the .so being in the x86 or x86_64 subfolder to determine which is the 32 bit and which is the 64 bit version


				// NOTE: in Unity 3.x there is no Linux build target, but there is Windows 32/64 bit

/*
			from http://docs.unity3d.com/Documentation/Manual/PluginsForDesktop.html

			On Windows and Linux, plugins can be managed manually (e.g, before building a 64-bit player, you copy the 64-bit library into the Assets/Plugins folder, and before building a 32-bit player, you copy the 32-bit library into the Assets/Plugins folder)

				OR you can place the 32-bit version of the plugin in Assets/Plugins/x86 and the 64-bit version of the plugin in Assets/Plugins/x86_64.

			By default the editor will look in the architecture-specific sub-directory first, and if that directory does not exist, it will use plugins from the root Assets/Plugins folder instead.

			Note that for the Universal Linux build, you are required to use the architecture-specific sub-directories (when building a Universal Linux build, the Editor will not copy any plugins from the root Assets/Plugins folder).

			For Mac OS X, you should build your plugin as a universal binary that contains both 32-bit and 64-bit architectures.
*/

				switch (buildPlatform)
				{
					case BuildPlatform.Android:
						// .jar files inside /Assets/Plugins/Android/ are always included in the build if built for Android
						if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/Android/") &&
						    (Util.IsFileOfType(currentAsset, ".jar") ||
						     Util.IsFileOfType(currentAsset, ".so")))
						{
							//Debug.Log(".jar file in android " + currentAsset);
							if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
								    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
							{
								inOutAllUsedAssets.Add(
									CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							}

							continue;
						}

						break;

					case BuildPlatform.iOS:
						if (Util.IsFileOfType(currentAsset, ".a") ||
						    Util.IsFileOfType(currentAsset, ".m") ||
						    Util.IsFileOfType(currentAsset, ".mm") ||
						    Util.IsFileOfType(currentAsset, ".c") ||
						    Util.IsFileOfType(currentAsset, ".cpp"))
						{
							// any .a, .m, .mm, .c, or .cpp files inside Assets/Plugins/iOS are automatically symlinked/used
							if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/iOS/"))
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}
							}

							// if there are any .a, .m, .mm, .c, or .cpp files outside of Assets/Plugins/iOS
							// we can't determine if they are really used or not because the user may manually copy them to the Xcode project, or a post-process .sh script may copy them to the Xcode project.
							// so we don't put them in the unused assets list
							continue;
						}

						break;


					case BuildPlatform.MacOSX32:
						// when in mac build, .bundle files that are in Assets/Plugins are always included
						// supposedly, Unity expects all .bundle files as universal builds (even if this is only a 32-bit build?)
						if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/") &&
						    Util.IsFileOfType(currentAsset, ".bundle"))
						{
							if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
								    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
							{
								inOutAllUsedAssets.Add(
									CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							}

							continue;
						}

						break;
					case BuildPlatform.MacOSX64:
						// when in mac build, .bundle files that are in Assets/Plugins are always included
						// supposedly, Unity expects all .bundle files as universal builds (even if this is only a 64-bit build?)
						if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/") &&
						    Util.IsFileOfType(currentAsset, ".bundle"))
						{
							if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
								    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
							{
								inOutAllUsedAssets.Add(
									CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							}

							continue;
						}

						break;
					case BuildPlatform.MacOSXUniversal:
						// when in mac build, .bundle files that are in Assets/Plugins are always included
						// supposedly, Unity expects all .bundle files as universal builds
						if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/") &&
						    Util.IsFileOfType(currentAsset, ".bundle"))
						{
							if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
								    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
							{
								inOutAllUsedAssets.Add(
									CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							}

							continue;
						}

						break;


					case BuildPlatform.Windows32:
						if (Util.IsFileOfType(currentAsset, ".dll"))
						{
							if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/x86/") &&
							    !Util.IsFileInAnEditorFolder(currentAsset))
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}

								continue;
							}
							// Unity only makes use of Assets/Plugins/ if Assets/Plugins/x86/ does not exist
							else if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/") &&
							         !Util.IsFileInAnEditorFolder(currentAsset) && !has32BitPluginsFolder)
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}

								continue;
							}
						}

						break;

					case BuildPlatform.Windows64:
						if (Util.IsFileOfType(currentAsset, ".dll"))
						{
							if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/x86_64/") &&
							    !Util.IsFileInAnEditorFolder(currentAsset))
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}

								continue;
							}
							// Unity only makes use of Assets/Plugins/ if Assets/Plugins/x86_64/ does not exist
							else if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/") &&
							         !Util.IsFileInAnEditorFolder(currentAsset) && !has64BitPluginsFolder)
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}

								continue;
							}
						}

						break;


					case BuildPlatform.Linux32:
						if (Util.IsFileOfType(currentAsset, ".so"))
						{
							if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/x86/"))
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}

								continue;
							}
							// Unity only makes use of Assets/Plugins/ if Assets/Plugins/x86/ does not exist
							else if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/") && !has32BitPluginsFolder)
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}

								continue;
							}
						}

						break;

					case BuildPlatform.Linux64:
						if (Util.IsFileOfType(currentAsset, ".so"))
						{
							if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/x86_64/"))
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}

								continue;
							}
							// Unity only makes use of Assets/Plugins/ if Assets/Plugins/x86_64/ does not exist
							else if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/") && !has64BitPluginsFolder)
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}

								continue;
							}
						}

						break;

					case BuildPlatform.LinuxUniversal:
						if (Util.IsFileOfType(currentAsset, ".so"))
						{
							if (Util.DoesFileStartIn(currentAsset, "Assets/Plugins/x86/") ||
							    Util.DoesFileStartIn(currentAsset, "Assets/Plugins/x86_64/"))
							{
								if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
									    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
								{
									inOutAllUsedAssets.Add(
										CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
								}

								continue;
							}
						}

						break;
				}

				// check prefabs only when requested to do so
				if (Util.IsFileOfType(currentAsset, ".prefab"))
				{
					//Debug.Log("GetAllUnusedAssets: found prefab: " + currentAsset.GetFileNameOnly());
					if (!includeUnusedPrefabs)
					{
						continue;
					}
				}

				// assets in StreamingAssets folder are always included
				if (Util.DoesFileStartIn(currentAsset, "Assets/StreamingAssets/"))
				{
					if (inOutAllUsedAssets != null && !inOutAllUsedAssets.Exists(part =>
						    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
					{
						inOutAllUsedAssets.Add(CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
					}

					continue;
				}

				// add asset to unused list, but only if it's not in the used list
				//if (!usedAssetsDict.ContainsKey(currentAsset))
				if (inOutAllUsedAssets == null || !inOutAllUsedAssets.Exists(part =>
					    string.Equals(part.Name, currentAsset, StringComparison.InvariantCultureIgnoreCase)))
				{
					// when all other checks pass through, then that simply means this asset is unused
					unusedAssets.Add(CustomBuildReport.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
				}

				if (processInBatches && unusedAssets.Count >= fileCountLimit)
				{
					break;
				}
			}

			while (buildInfo.UnusedAssetsBatchFinalNum.Count <= buildInfo.UnusedAssetsBatchIdx)
			{
				buildInfo.UnusedAssetsBatchFinalNum.Add(0);
			}
			buildInfo.UnusedAssetsBatchFinalNum[buildInfo.UnusedAssetsBatchIdx] = assetNum;

			return unusedAssets.ToArray();
		}


		static void ParseDLLs(string editorLogPath, bool wasWebBuild, bool wasWebGLBuild, string buildFilePath,
			string projectAssetsPath, string editorAppContentsPath, ApiCompatibilityLevel monoLevel,
			StrippingLevel codeStrippingLevel, out CustomBuildReport.SizePart[] systemDLLs,
			out CustomBuildReport.SizePart[] unityEngineDLLs, out CustomBuildReport.SizePart[] scriptDLLs)
		{
			List<CustomBuildReport.SizePart> systemDLLsList = new List<CustomBuildReport.SizePart>();
			List<CustomBuildReport.SizePart> unityEngineDLLsList = new List<CustomBuildReport.SizePart>();
			List<CustomBuildReport.SizePart> scriptDLLsList = new List<CustomBuildReport.SizePart>();

			string buildManagedDLLsFolder = CustomBuildReport.Util.GetBuildManagedFolder(buildFilePath);
			string buildScriptDLLsFolder = buildManagedDLLsFolder;
			string buildManagedDLLsFolderHigherPriority;

			bool wasAndroidApkBuild = buildFilePath.EndsWith(".apk", StringComparison.OrdinalIgnoreCase);
			bool wasAndroidAppBundleBuild = buildFilePath.EndsWith(".aab", StringComparison.OrdinalIgnoreCase);

			if (wasWebBuild || wasWebGLBuild)
			{
				string tryPath;
				bool success = CustomBuildReport.Util.AttemptGetWebTempStagingArea(projectAssetsPath, out tryPath);
				if (success)
				{
					buildManagedDLLsFolder = tryPath;
					buildScriptDLLsFolder = tryPath;
				}
				else
				{
					buildManagedDLLsFolder = CustomBuildReport.Util.GetProjectWebGLManagedStrippedPath(projectAssetsPath);
					buildScriptDLLsFolder = buildManagedDLLsFolder;
				}
			}
			else if (wasAndroidApkBuild || wasAndroidAppBundleBuild)
			{
				string tryPath;
				bool success = CustomBuildReport.Util.AttemptGetAndroidTempStagingArea(projectAssetsPath, out tryPath);
				if (success)
				{
					buildManagedDLLsFolder = tryPath;
					buildScriptDLLsFolder = tryPath;
				}
				else
				{
					buildScriptDLLsFolder = CustomBuildReport.Util.GetProjectScriptAssembliesPath(projectAssetsPath);
				}
			}
			else
			{
				// Check if buildFilePath is a folder, check if it's a gradle project (check if there's a "settings.gradle" file).
				// If so, the managed DLLs folder is in "unityLibrary/src/main/assets/bin/Data/Managed".
				string gradleSettings = buildFilePath + "/settings.gradle";
				if (File.Exists(gradleSettings))
				{
					buildManagedDLLsFolder = buildFilePath + "/unityLibrary/src/main/assets/bin/Data/Managed/";
					buildScriptDLLsFolder = buildManagedDLLsFolder;
				}
			}

			bool buildManagedDLLsFolderHasContents = !string.IsNullOrEmpty(buildManagedDLLsFolder) && Directory.Exists(buildManagedDLLsFolder) && System.IO.Directory.EnumerateFiles(buildManagedDLLsFolder, "*.dll").Any();

			CustomBuildReport.SizePart inPart;

			bool checkApkItself = (wasAndroidApkBuild || wasAndroidAppBundleBuild) && System.IO.File.Exists(buildFilePath);

			if (checkApkItself)
			{
#if UNITY_2021_1_OR_NEWER
				var apkContents = System.IO.Compression.ZipFile.Open(buildFilePath, System.IO.Compression.ZipArchiveMode.Read);
				{
#else
				using (System.IO.FileStream fs = System.IO.File.OpenRead(buildFilePath))
				{
					System.IO.Compression.ZipArchive apkContents = new System.IO.Compression.ZipArchive(fs, System.IO.Compression.ZipArchiveMode.Read);
#endif
					if (apkContents != null)
					{
						string managedFolderPath = wasAndroidAppBundleBuild
							? "base/assets/bin/Data/Managed/"
							: "assets/bin/Data/Managed/";
						foreach (var e in apkContents.Entries)
						{
							string filepath = e.FullName;
							if (filepath.StartsWith(managedFolderPath) && filepath.EndsWith(".dll"))
							{
								inPart = new CustomBuildReport.SizePart();
								inPart.Name = System.Security.SecurityElement.Escape(e.Name);
								inPart.RawSizeBytes = e.Length;
								inPart.RawSize = CustomBuildReport.Util.GetBytesReadable(inPart.RawSizeBytes);
								inPart.ImportedSizeBytes = -1;
								inPart.ImportedSize = "N/A";
								inPart.Percentage = -1;

								if (CustomBuildReport.Util.IsAUnityEngineDLL(e.Name))
								{
									unityEngineDLLsList.Add(inPart);
								}
								else if (CustomBuildReport.Util.IsAScriptDLL(e.Name))
								{
									scriptDLLsList.Add(inPart);
								}
								else if (CustomBuildReport.Util.IsAKnownSystemDLL(e.Name))
								{
									systemDLLsList.Add(inPart);
								}
								else
								{
									scriptDLLsList.Add(inPart);
								}
							}
						}
					}
				}
			}
			else if (buildManagedDLLsFolderHasContents)
			{
				foreach (string filepath in DldUtil.TraverseDirectory.Do(buildManagedDLLsFolder))
				{
					var filename = filepath.GetFileNameOnly();

					if (CustomBuildReport.Util.IsFileOfType(filename, ".dll"))
					{
						inPart = CustomBuildReport.Util.CreateSizePartFromFile(filename, filepath);

						if (CustomBuildReport.Util.IsAUnityEngineDLL(filename))
						{
							unityEngineDLLsList.Add(inPart);
						}
						else if (CustomBuildReport.Util.IsAScriptDLL(filename))
						{
							scriptDLLsList.Add(inPart);
						}
						else if (CustomBuildReport.Util.IsAKnownSystemDLL(filename))
						{
							systemDLLsList.Add(inPart);
						}
						else
						{
							scriptDLLsList.Add(inPart);
						}
					}
				}
			}

			if ((!checkApkItself && !buildManagedDLLsFolderHasContents) || wasWebGLBuild)
			{
				// folder inside the Unity installation where mono system dlls are
				string unityFolderManagedDLLs;

				bool unityFoldersSuccess = CustomBuildReport.Util.AttemptGetUnityFolderMonoDLLs(wasWebBuild || wasWebGLBuild,
					wasAndroidApkBuild, editorAppContentsPath, monoLevel,
					codeStrippingLevel, out unityFolderManagedDLLs,
					out buildManagedDLLsFolderHigherPriority);

				//Debug.Log("buildManagedDLLsFolder: " + buildManagedDLLsFolder);
				//Debug.Log("Application.dataPath: " + Application.dataPath);

				if (unityFoldersSuccess &&
				    (string.IsNullOrEmpty(buildManagedDLLsFolder) || !Directory.Exists(buildManagedDLLsFolder)))
				{
#if BRT_SHOW_MINOR_WARNINGS
					Debug.LogWarning("Could not find build folder. Using Unity install folder instead for getting mono DLL file sizes.");
#endif
					buildManagedDLLsFolder = unityFolderManagedDLLs;
				}

#if BRT_SHOW_MINOR_WARNINGS
				if (!Directory.Exists(buildManagedDLLsFolder))
				{
					Debug.LogWarning("Could not find folder for getting DLL file sizes. Got: \"" + buildManagedDLLsFolder + "\"");
				}
#endif


				const string PREFIX_REMOVE = "Dependency assembly - ";


				const string MONO_DLL_KEY = "Mono dependencies included in the build";

				bool gotDateTimePrefix = false;
				int dateTimePrefixLen = 0;

				string dllArtifactsPath = CustomBuildReport.Util.GetProjectManagedDLLArtifacts(projectAssetsPath);
				bool dllArtifactsFolderExists = System.IO.Directory.Exists(dllArtifactsPath);

				bool hasUnityEngineDLL = false;
				if (wasWebGLBuild)
				{
					hasUnityEngineDLL = unityEngineDLLsList.Exists("UnityEngine.dll");
				}
				foreach (string line in DldUtil.BigFileReader.ReadFile(editorLogPath, MONO_DLL_KEY))
				{
					// blank line signifies end of dll list
					if (string.IsNullOrEmpty(line) || line == "\n" || line == "\r\n")
					{
						break;
					}

					if (line.IndexOf(MONO_DLL_KEY, StringComparison.Ordinal) != -1)
					{
						continue;
					}

					string filename = line;
					if (!gotDateTimePrefix)
					{
						Match dateTime = Regex.Match(filename, DATE_TIME_PREFIX, RegexOptions.IgnoreCase);
						if (dateTime.Success)
						{
							dateTimePrefixLen = dateTime.Groups[0].Value.Length;
							gotDateTimePrefix = true;
							filename = filename.Substring(dateTimePrefixLen);
						}
					}
					else if (filename.Length >= dateTimePrefixLen)
					{
						filename = filename.Substring(dateTimePrefixLen);
					}

					if (!filename.StartsWith(PREFIX_REMOVE))
					{
						continue;
					}

					filename = CustomBuildReport.Util.RemovePrefix(PREFIX_REMOVE, filename);

					if (wasWebGLBuild)
					{
						if (systemDLLsList.Exists(filename) ||
						    unityEngineDLLsList.Exists(filename) ||
						    scriptDLLsList.Exists(filename))
						{
							continue;
						}
					}

					if (filename == "UnityEngine.dll")
					{
						hasUnityEngineDLL = true;
					}

					string filepath = null;

					bool foundFileInArtifactsPath = false;
					if (dllArtifactsFolderExists)
					{
						// If we can find the DLL's path from the .info file, favor using it, because it's the most accurate
						// Older versions of Unity won't have this, in those cases, we resort to our usual code.

						// Check the .info files (which are just json text files) in "Library/Bee/artifacts/csharpactions"
						// Look for the .info file in that folder whose filename starts with the same filename of our DLL.
						string nameSearch = System.IO.Path.GetFileNameWithoutExtension(filename) + "*";
						foreach (string foundFile in System.IO.Directory.EnumerateFiles(dllArtifactsPath, nameSearch,
							         SearchOption.TopDirectoryOnly))
						{
							object infoObj = MiniJSON.Json.Deserialize(System.IO.File.ReadAllText(foundFile));
							if (infoObj is Dictionary<string, object> info)
							{
								if (info.TryGetValue("Bee.TundraBackend.CSharpActionInvocationInformation", out object invocationsObj) &&
								    invocationsObj is Dictionary<string, object> invocations)
								{
									if (invocations.TryGetValue("inputs", out object inputsObj) && inputsObj is List<object> inputs &&
									    inputs.Count > 0 && inputs[0] is string inputString && !string.IsNullOrEmpty(inputString))
									{
										if (System.IO.File.Exists(inputString))
										{
											filepath = inputString;
											foundFileInArtifactsPath = true;
											break;
										}
									}
								}
							}
						}
					}

					if (!foundFileInArtifactsPath || string.IsNullOrEmpty(filepath))
					{
						if (CustomBuildReport.Util.IsAScriptDLL(filename))
						{
							filepath = buildScriptDLLsFolder + filename;
						}
						else
						{
							filepath = buildManagedDLLsFolder + filename;

							if (!File.Exists(filepath) &&
							    unityFoldersSuccess &&
							    (buildManagedDLLsFolder != unityFolderManagedDLLs))
							{
#if BRT_SHOW_MINOR_WARNINGS
							Debug.LogWarning("Failed to find file \"" + filepath + "\". Attempting to get from Unity folders.");
#endif
								filepath = unityFolderManagedDLLs + filename;

								if (!string.IsNullOrEmpty(buildManagedDLLsFolderHigherPriority) &&
								    File.Exists(buildManagedDLLsFolderHigherPriority + filename))
								{
									filepath = buildManagedDLLsFolderHigherPriority + filename;
								}
							}
						}

						if ((buildManagedDLLsFolder == unityFolderManagedDLLs) &&
						    !string.IsNullOrEmpty(buildManagedDLLsFolderHigherPriority) &&
						    File.Exists(buildManagedDLLsFolderHigherPriority + filename))
						{
							filepath = buildManagedDLLsFolderHigherPriority + filename;
						}
					}

					//Debug.Log(filename + " " + filepath);

					inPart = CustomBuildReport.Util.CreateSizePartFromFile(filename, filepath);

					//gotTotalSizeBytes += inPart.SizeBytes;

					if (CustomBuildReport.Util.IsAUnityEngineDLL(filename))
					{
						unityEngineDLLsList.Add(inPart);
					}
					else if (CustomBuildReport.Util.IsAKnownSystemDLL(filename))
					{
						systemDLLsList.Add(inPart);
					}
					else if (CustomBuildReport.Util.IsAScriptDLL(filename) || !File.Exists(unityFolderManagedDLLs + filename))
					{
						scriptDLLsList.Add(inPart);
					}
					else
					{
						systemDLLsList.Add(inPart);
					}
				}

				// somehow, the editor logfile
				// doesn't include UnityEngine.dll
				// even though it gets included in the final build (for desktop builds)
				if (!hasUnityEngineDLL)
				{
					string filename = "UnityEngine.dll";
					string filepath = buildManagedDLLsFolder + filename;

					if (File.Exists(filepath))
					{
						inPart = CustomBuildReport.Util.CreateSizePartFromFile(filename, filepath);
						//gotTotalSizeBytes += inPart.SizeBytes;
						unityEngineDLLsList.Add(inPart);
					}
				}


				//Debug.Log("total size: " + EditorUtility.FormatBytes(gotTotalSizeBytes) + " (" + gotTotalSizeBytes + " bytes)");
				//Debug.Log("total assembly size: " + EditorUtility.FormatBytes(gotScriptTotalSizeBytes) + " (" + gotScriptTotalSizeBytes + " bytes)");
				//Debug.Log("total size without assembly: " + EditorUtility.FormatBytes(gotTotalSizeBytes - gotScriptTotalSizeBytes) + " (" + (gotTotalSizeBytes-gotScriptTotalSizeBytes) + " bytes)");
			}

			systemDLLs = systemDLLsList.ToArray();
			unityEngineDLLs = unityEngineDLLsList.ToArray();
			scriptDLLs = scriptDLLsList.ToArray();
		}


		const string NO_BUILD_INFO_WARNING =
			"Build Report Tool: No build info found. Build the project first. If you have more than one instance of the Unity Editor open, close all of them and open only one.";

		const string NO_BUILD_INFO_NO_LOG_WARNING =
			"Build Report Tool: No build info found. Unity was launched with the -nolog argument. Build Report Tool can't obtain build info if there are no logs. Please relaunch Unity without the -nolog argument.";

		const string NO_BUILD_INFO_OVERRIDDEN_LOG_WARNING =
			"Build Report Tool: No build info found.\n\nWarning: Build Report Tool is configured to use a custom log file to obtain build data from ({0}). Perhaps this was not intended?\n\nClear the override log in Build Report Tool's Options, or set the EditorLogOverridePath tag to empty in {1}.\n\n";

		const string NO_BUILD_INFO_NO_PLAYER_REBUILT =
			"Information on used Assets is not available, since player data was not rebuilt.";

		public static bool DoesEditorLogHaveBuildInfo(string editorLogPath)
		{
			return DldUtil.BigFileReader.FileHasText(editorLogPath, ASSET_SIZES_KEY, ASSET_SIZES_KEY_2);
		}

		public static bool DoesEditorLogHaveNoBuildInfoDueToNoPlayerRebuilt(string editorLogPath)
		{
			return DldUtil.BigFileReader.FileHasText(editorLogPath, NO_BUILD_INFO_NO_PLAYER_REBUILT);
		}

		public static BuildSettingCategory GetBuildSettingCategoryFromBuildValues(BuildInfo buildReport)
		{
			if (!CustomBuildReport.Util.BuildInfoHasContents(buildReport))
			{
				return BuildSettingCategory.None;
			}

			return GetBuildSettingCategoryFromBuildValues(buildReport.BuildType, buildReport.BuildTargetUsed);
		}

		public static BuildSettingCategory GetBuildSettingCategoryFromBuildValues(string gotBuildType,
			BuildTarget buildTarget)
		{
			BuildPlatform b = GetBuildPlatformFromString(gotBuildType, buildTarget);

			switch (b)
			{
				case BuildPlatform.Windows32:
					return BuildSettingCategory.WindowsDesktopStandalone;
				case BuildPlatform.Windows64:
					return BuildSettingCategory.WindowsDesktopStandalone;


				case BuildPlatform.MacOSX64:
					return BuildSettingCategory.MacStandalone;
				case BuildPlatform.MacOSXUniversal:
					return BuildSettingCategory.MacStandalone;


				case BuildPlatform.Linux32:
					return BuildSettingCategory.LinuxStandalone;
				case BuildPlatform.Linux64:
					return BuildSettingCategory.LinuxStandalone;
				case BuildPlatform.LinuxUniversal:
					return BuildSettingCategory.LinuxStandalone;


				case BuildPlatform.Web:
					return BuildSettingCategory.WebPlayer;
				case BuildPlatform.Flash:
					return BuildSettingCategory.FlashPlayer;
				case BuildPlatform.WebGL:
					return BuildSettingCategory.WebGL;


				case BuildPlatform.iOS:
					return BuildSettingCategory.iOS;
				case BuildPlatform.tvOS:
					return BuildSettingCategory.tvOS;
				case BuildPlatform.Android:
					return BuildSettingCategory.Android;
				case BuildPlatform.Blackberry:
					return BuildSettingCategory.Blackberry;


				case BuildPlatform.Xbox360:
					return BuildSettingCategory.Xbox360;
				case BuildPlatform.XboxOne:
					return BuildSettingCategory.XboxOne;
				case BuildPlatform.XboxSeries:
					return BuildSettingCategory.XboxSeries;
				case BuildPlatform.PS3:
					return BuildSettingCategory.PS3;
				case BuildPlatform.PS4:
					return BuildSettingCategory.PS4;
				case BuildPlatform.PS5:
					return BuildSettingCategory.PS5;
				case BuildPlatform.Switch:
					return BuildSettingCategory.Switch;
			}

			return BuildSettingCategory.None;
		}

		public static BuildPlatform GetBuildPlatformFromString(string gotBuildType, BuildTarget buildTarget)
		{
			BuildPlatform buildPlatform = BuildPlatform.None;


			if (string.IsNullOrEmpty(gotBuildType))
			{
				// log has no build type
				// have to resort to looking at current build settings
				// which may be inaccurate (if generating report from custom log file)
				buildPlatform = CustomBuildReport.Util.GetBuildPlatformBasedOnUnityBuildTarget(buildTarget);
			}

			// mobile

			else if (gotBuildType.IndexOf("Android", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.Android;
			}
			else if (gotBuildType.IndexOf("iPhone", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.iOS;
			}
			else if (gotBuildType.IndexOf("iOS", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.iOS;
			}
			else if (gotBuildType.IndexOf("tvOS", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.tvOS;
			}

			// browser

			else if (gotBuildType.IndexOf("WebPlayer", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.Web;
			}
			else if (gotBuildType.IndexOf("Flash", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.Flash;
			}
			else if (gotBuildType.IndexOf("WebGL", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.WebGL;
			}

			// Windows

			else if (gotBuildType.IndexOf("Windows64", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.Windows64;
			}
			else if (gotBuildType.IndexOf("Windows", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.Windows32;
			}

			// Linux

			else if (gotBuildType.IndexOf("Linux64", StringComparison.Ordinal) != -1)
			{
				buildPlatform = BuildPlatform.Linux64;
			}
			else if (gotBuildType.IndexOf("Linux", StringComparison.Ordinal) != -1)
			{
				// unfortunately we don't know if this is a 32-bit or universal build
				// we'll have to rely on current build settings which may be inaccurate
				buildPlatform = CustomBuildReport.Util.GetBuildPlatformBasedOnUnityBuildTarget(buildTarget);

				if (buildPlatform != BuildPlatform.Linux32 &&
				    buildPlatform != BuildPlatform.Linux64 &&
				    buildPlatform != BuildPlatform.LinuxUniversal &&
				    buildPlatform != BuildPlatform.LinuxHeadless &&
				    buildPlatform != BuildPlatform.EmbeddedLinux)
				{
					// build platform was not detected properly
					// default to 64-bit Linux
					buildPlatform = BuildPlatform.Linux64;
				}
			}

			// Mac OS X

			else if (gotBuildType.IndexOf("Mac", StringComparison.Ordinal) != -1)
			{
				// unfortunately we don't know if this is a 32-bit, 64-bit, or universal build
				// we'll have to rely on current build settings which may be inaccurate
				buildPlatform = CustomBuildReport.Util.GetBuildPlatformBasedOnUnityBuildTarget(buildTarget);

				if (buildPlatform != BuildPlatform.MacOSX32 &&
				    buildPlatform != BuildPlatform.MacOSX64 &&
				    buildPlatform != BuildPlatform.MacOSXUniversal)
				{
					// build platform was not detected properly
					// default to 64-bit Mac
					buildPlatform = BuildPlatform.MacOSX64;
				}
			}

			// ???

			else
			{
				//Debug.LogFormat("Could not determine build type from: {0}", gotBuildType);
				// could not determine from log
				// have to resort to looking at current build settings
				// which may be inaccurate
				buildPlatform = CustomBuildReport.Util.GetBuildPlatformBasedOnUnityBuildTarget(buildTarget);
			}

			return buildPlatform;
		}


		/// <summary>
		/// Note: This doesn't work anymore in Unity 5.3.2
		/// </summary>
		/// <returns></returns>
		public static string GetCompressedSizeReadingFromLog()
		{
			const string COMPRESSED_BUILD_SIZE_STA_KEY = "Total compressed size ";
			const string COMPRESSED_BUILD_SIZE_END_KEY = ". Total uncompressed size ";

			string result = string.Empty;

			string line = DldUtil.BigFileReader.SeekText(_lastEditorLogPath, COMPRESSED_BUILD_SIZE_STA_KEY);

			if (!string.IsNullOrEmpty(line))
			{
				int compressedBuildSizeIdx = line.LastIndexOf(COMPRESSED_BUILD_SIZE_STA_KEY, StringComparison.Ordinal);
				if (compressedBuildSizeIdx != -1)
				{
					// this data in the editor log only shows in web builds so far
					// meaning we do not get a compressed result in other builds (except android, where we can check the file size of the .apk itself)
					//
					int compressedBuildSizeEndIdx = line.IndexOf(COMPRESSED_BUILD_SIZE_END_KEY, compressedBuildSizeIdx,
						StringComparison.Ordinal);

					result = line.Substring(compressedBuildSizeIdx + COMPRESSED_BUILD_SIZE_STA_KEY.Length,
						compressedBuildSizeEndIdx - compressedBuildSizeIdx - COMPRESSED_BUILD_SIZE_STA_KEY.Length);
				}
			}

			//Debug.Log("compressed size from log: " + result);

			return result;
		}


		/// <summary>
		/// Used for Windows and Linux builds to get build size.
		/// </summary>
		/// <param name="buildFilePath">Path to build as given by <see cref="EditorUserBuildSettings.GetBuildLocation"/></param>
		/// <param name="unityVersion"></param>
		/// <returns>Size of build in bytes</returns>
		static double GetStandaloneBuildSize(string buildFilePath, string unityVersion)
		{
			if (string.IsNullOrEmpty(buildFilePath))
			{
				return 0;
			}

			if (Directory.Exists(buildFilePath))
			{
				//Debug.LogFormat("{0} is a folder", buildFilePath);

				// build location is a folder. normally it would be a file instead (the executable file for the build)
				// in the latest versions of Unity, it's a folder

				// For Windows, attempt to find the .exe file within this folder and use that.
				// What if there are multiple unity builds in this folder??? Unfortunately,
				// we have no way of figuring out which .exe file is the one we want.
				string[] potentialBuildExeFiles =
					Directory.GetFiles(buildFilePath, "*.exe", SearchOption.TopDirectoryOnly);

				if (potentialBuildExeFiles.Length > 0)
				{
					for (int n = 0, len = potentialBuildExeFiles.Length; n < len; ++n)
					{
						if (IsUnityExecutableFile(potentialBuildExeFiles[n]))
						{
							//Debug.LogFormat("found unity .exe file: {0}", potentialBuildExeFiles[n]);
							return GetStandaloneBuildWithDataFolderSize(potentialBuildExeFiles[n], unityVersion);
						}
					}
				}

				// --------------------------

				string[] potentialBuildLinux32BitFiles =
					Directory.GetFiles(buildFilePath, "*.x86", SearchOption.TopDirectoryOnly);

				if (potentialBuildLinux32BitFiles.Length > 0)
				{
					for (int n = 0, len = potentialBuildLinux32BitFiles.Length; n < len; ++n)
					{
						if (IsUnityExecutableFile(potentialBuildLinux32BitFiles[n]))
						{
							//Debug.Log("found unity .x86 file: " + potentialBuildLinux32BitFiles[n]);
							return GetStandaloneBuildWithDataFolderSize(potentialBuildLinux32BitFiles[n], unityVersion);
						}
					}
				}

				// --------------------------

				string[] potentialBuildLinux64BitFiles =
					Directory.GetFiles(buildFilePath, "*.x86_64", SearchOption.TopDirectoryOnly);

				if (potentialBuildLinux64BitFiles.Length > 0)
				{
					for (int n = 0, len = potentialBuildLinux64BitFiles.Length; n < len; ++n)
					{
						if (IsUnityExecutableFile(potentialBuildLinux64BitFiles[n]))
						{
							//Debug.Log("found unity .x86_64 file: " + potentialBuildLinux64BitFiles[n]);
							return GetStandaloneBuildWithDataFolderSize(potentialBuildLinux64BitFiles[n], unityVersion);
						}
					}
				}

				// just return size of whole folder.
				//Debug.LogFormat("Getting size of whole folder: {0}", buildFilePath);
				return CustomBuildReport.Util.GetPathSizeInBytes(buildFilePath);
			}

			//Debug.LogFormat("{0} is a file", buildFilePath);

			// build location is a file
			return GetStandaloneBuildWithDataFolderSize(buildFilePath, unityVersion);
		}

		/// <summary>
		/// Used for Windows and Linux builds to get build size.
		/// </summary>
		/// <param name="buildFilePath">Path to build as given by <see cref="EditorUserBuildSettings.GetBuildLocation"/></param>
		/// <param name="unityVersion"></param>
		/// <returns>Size of build in bytes</returns>
		static double GetStandaloneBuildWithDataFolderSize(string buildFilePath, string unityVersion)
		{
			if (string.IsNullOrEmpty(buildFilePath))
			{
				return 0;
			}

			var folderOfBuildFile = Directory.Exists(buildFilePath) ? buildFilePath : Path.GetDirectoryName(buildFilePath);

			const string DoNotShipSuffix1 = "_BurstDebugInformation_DoNotShip";
			const string DoNotShipSuffix2 = "_BackUpThisFolder_ButDontShipItWithYourGame";

			if (IsSingleStandaloneBuildInPath(folderOfBuildFile))
			{
				//Debug.LogFormat("GetStandaloneBuildWithDataFolderSize: Getting size of whole folder {0}", folderOfBuildFile);

				double parentFolderByteSize = CustomBuildReport.Util.GetPathSizeInBytes(folderOfBuildFile, DoNotShipSuffix1, DoNotShipSuffix2);

				return parentFolderByteSize;
			}
			// else: there's multiple unity builds in the path,
			// so we should only get the size of the build we're interested in

			if (IsUnityExecutableFile(buildFilePath))
			{
				//Debug.LogFormat("GetStandaloneBuildWithDataFolderSize: Getting size of executable and its _Data folder {0}", buildFilePath);

				double exeFileByteSize = CustomBuildReport.Util.GetPathSizeInBytes(buildFilePath);

				// get the exe file but remove the file type and add _Data. that's the folder name
				string dataFolderPath = CustomBuildReport.Util.ReplaceFileType(buildFilePath, "_Data");
				//Debug.Log("dataFolderPath: " + dataFolderPath);

				double dataFolderByteSize = CustomBuildReport.Util.GetPathSizeInBytes(dataFolderPath);

				if (buildFilePath.EndsWith(".x86", StringComparison.OrdinalIgnoreCase))
				{
					// check if accompanying 64-bit executable is also there (i.e. if it's a universal build)
					// and include that in file size too

					// get the .x86 file file but change the file type to ".x86_64"
					string exe64Path = CustomBuildReport.Util.ReplaceFileType(buildFilePath, ".x86_64");

					if (File.Exists(exe64Path))
					{
						// gets the size of 64-bit executable
						double exe64SizeBytes = CustomBuildReport.Util.GetPathSizeInBytes(exe64Path);

						return (exeFileByteSize + exe64SizeBytes + dataFolderByteSize);
					}
				}

				return (exeFileByteSize + dataFolderByteSize);
			}

			// buildFilePath doesn't have a path we can use to determine the build size
			return 0;
		}

		/// <summary>
		/// Does the path contain only one Unity standalone build?
		/// </summary>
		/// <returns></returns>
		static bool IsSingleStandaloneBuildInPath(string buildFilePath)
		{
			// check if there are multiple .exe or .x86 or .x86_64 files in the folder
			if (!Directory.Exists(buildFilePath))
			{
				// not a folder
				return false;
			}

			string parentFolderPath = Path.GetDirectoryName(buildFilePath);
			if (string.IsNullOrEmpty(parentFolderPath))
			{
				return false;
			}

			//Debug.LogFormat("IsSingleStandaloneBuildInPath: Checking {0}", parentFolderPath);

			if (Directory.Exists(parentFolderPath))
			{
				var exeFilesInFolder = Directory.GetFiles(parentFolderPath, "*.exe", SearchOption.TopDirectoryOnly);
				var manyExeFiles = exeFilesInFolder.Length >= 2;
				if (manyExeFiles)
				{
					var foundUnityBuildExeFiles = 0;
					for (int n = 0, len = exeFilesInFolder.Length; n < len; ++n)
					{
						// new in Unity 2017 and above
						// even though these are .exe files, they're not a build's executable
						if (exeFilesInFolder[n].Contains("UnityCrashHandler64.exe") ||
						    exeFilesInFolder[n].Contains("UnityCrashHandler32.exe"))
						{
							continue;
						}

						if (IsUnityExecutableFile(exeFilesInFolder[n]))
						{
							++foundUnityBuildExeFiles;
						}
					}

					//Debug.LogFormat("IsSingleStandaloneBuildInPath: .exe files found in {0}: {1}",
					//	parentFolderPath, foundUnityBuildExeFiles.ToString());

					if (foundUnityBuildExeFiles > 1)
					{
						return false;
					}

					// note: Even if there's only 1 unity build exe file in this folder,
					// one of the subfolders in here may have an .exe file and build folder too
					// But it's tricky to check for this.
					// Newer versions of Unity add new files into the build like UnityCrashHandler64.exe,
					// and WinPixEventRuntime.dll (these are beside the game's exe file),
					// so an explicit approach (get size only of particular files and folders)
					// can potentially miss out on newly added files/folders of builds from newer versions of Unity.
					//
					// So the current approach of just getting the entire folder's size is preferable.
					// It's just that the user has to be mindful to always set the build location to a
					// folder where that build is the only thing in that folder.
				}

				// -------------------

				var linuxExeFilesInFolder = Directory.GetFiles(parentFolderPath, "*.x86", SearchOption.TopDirectoryOnly);
				var manyLinuxExeFiles = linuxExeFilesInFolder.Length >= 2;
				if (manyLinuxExeFiles)
				{
					//Debug.LogFormat("IsSingleStandaloneBuildInPath: Many .x86 files found in {0}", parentFolderPath);

					var foundUnityBuildExeFiles = 0;
					for (int n = 0, len = linuxExeFilesInFolder.Length; n < len; ++n)
					{
						if (IsUnityExecutableFile(linuxExeFilesInFolder[n]))
						{
							++foundUnityBuildExeFiles;
						}
					}

					if (foundUnityBuildExeFiles > 1)
					{
						return false;
					}
				}

				// -------------------

				var linuxExe64FilesInFolder =
					Directory.GetFiles(parentFolderPath, "*.x86_64", SearchOption.TopDirectoryOnly);
				var manyLinuxExe64Files = linuxExe64FilesInFolder.Length >= 2;
				if (manyLinuxExe64Files)
				{
					//Debug.LogFormat("IsSingleStandaloneBuildInPath: Many .x86_64 files found in {0}", parentFolderPath);

					var foundUnityBuildExeFiles = 0;
					for (int n = 0, len = linuxExe64FilesInFolder.Length; n < len; ++n)
					{
						if (IsUnityExecutableFile(linuxExe64FilesInFolder[n]))
						{
							++foundUnityBuildExeFiles;
						}
					}

					if (foundUnityBuildExeFiles > 1)
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Does the specified executable file also have an accompanying "_Data" folder in the same path?
		/// </summary>
		/// <param name="filepath"></param>
		/// <returns></returns>
		static bool IsUnityExecutableFile(string filepath)
		{
			if (File.Exists(filepath))
			{
				string dataFolderPath;

				if (CustomBuildReport.Util.IsFileOfType(filepath, ".exe") ||
				    CustomBuildReport.Util.IsFileOfType(filepath, ".x86") ||
				    CustomBuildReport.Util.IsFileOfType(filepath, ".x86_64"))
				{
					dataFolderPath = CustomBuildReport.Util.ReplaceFileType(filepath, "_Data");
				}
				else
				{
					// file doesn't have .exe or .x86 or .x86_64.
					// this happens in linux build where executable has no file type extension
					// just append "_Data" to it then
					dataFolderPath = filepath + "_Data";
				}

				if (Directory.Exists(dataFolderPath))
				{
					return true;
				}
			}

			return false;
		}

		public static bool CheckIfUnityHasNoLogArgument()
		{
			if (!_gotCommandLineArguments)
			{
				string[] args = System.Environment.GetCommandLineArgs();

				_unityHasNoLogArgument = false;
				for (int i = 0; i < args.Length; i++)
				{
					//Debug.Log(args[i]);
					if (args[i] == "-nolog")
					{
						_unityHasNoLogArgument = true;
						break;
					}
				}

				_gotCommandLineArguments = true;
			}

			return _unityHasNoLogArgument;
		}

		// ==================================================================================================================================================================================================================
		// main function for generating a report

		public static void GetValues(BuildInfo buildInfo, string buildFilePath, string projectAssetsPath,
			string editorAppContentsPath, bool calculateBuildSize)
		{
			BRT_BuildReportWindow.GetValueMessage = "Getting values...";

			if (!DoesEditorLogHaveBuildInfo(_lastEditorLogPath))
			{
				string lastSuccessfulBuildEditorLog = CustomBuildReport.Util.LastSuccessfulBuildFilePath(projectAssetsPath);
				if (DoesEditorLogHaveBuildInfo(lastSuccessfulBuildEditorLog))
				{
					_lastEditorLogPath = lastSuccessfulBuildEditorLog;
					Debug.Log($"Reusing last successful build editor log file from: {lastSuccessfulBuildEditorLog}");
				}
				else
				{
					if (CustomBuildReport.Util.IsDefaultEditorLogPathOverridden)
					{
						Debug.LogWarning(string.Format(NO_BUILD_INFO_OVERRIDDEN_LOG_WARNING, _lastEditorLogPath,
							CustomBuildReport.Options.FoundPathForSavedOptions));
					}
					else if (CheckIfUnityHasNoLogArgument())
					{
						Debug.LogWarning(NO_BUILD_INFO_NO_LOG_WARNING);
					}
					else
					{
						Debug.LogWarning(NO_BUILD_INFO_WARNING);
					}

					return;
				}
			}

			if (CustomBuildReport.Options.KeepCopyOfLogOfLastSuccessfulBuild &&
			    !CustomBuildReport.Util.IsDefaultEditorLogPathOverridden &&
			    !CustomBuildReport.Util.IsUsingLastSuccessfulEditorLog(_lastEditorLogPath))
			{
				// make a copy of the Editor log file first
				// save it in the Library as Editor-LastSuccessfulBuild.log
				string lastSuccessfulBuildEditorLog = CustomBuildReport.Util.LastSuccessfulBuildFilePath(projectAssetsPath);
				File.Copy(_lastEditorLogPath, lastSuccessfulBuildEditorLog, true);
				Debug.Log($"Copied: {_lastEditorLogPath} to: {lastSuccessfulBuildEditorLog}");

				// also save other pertinent information like build time
				var buildExtraData = new CustomBuildReport.Util.LastBuildExtraData();
				buildExtraData.SetBuildTimeStarted(buildInfo.BuildTimeGot);
				buildExtraData.SetBuildDuration(buildInfo.BuildDurationTime);

				CustomBuildReport.Util.Serialize(buildExtraData, CustomBuildReport.Util.LastSuccessfulBuildExtraDataFilePath(projectAssetsPath));
			}

			// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// Determine some build data (like Unity Version and Build Platform) based on editor log.
			// Much more reliable, especially when using an override log.

			string gotUnityVersion = GetUnityVersionFromEditorLog(_lastEditorLogPath);
			if (!string.IsNullOrEmpty(gotUnityVersion))
			{
				buildInfo.UnityVersion = gotUnityVersion;
			}

			string gotUnityEditorPath = GetUnityEditorPathFromEditorLog(_lastEditorLogPath);
			if (!string.IsNullOrEmpty(gotUnityEditorPath))
			{
				buildInfo.EditorAppContentsPath = gotUnityEditorPath;
			}

			string gotProjectAssetsPath = GetProjectAssetsPathFromEditorLog(_lastEditorLogPath);
			if (!string.IsNullOrEmpty(gotProjectAssetsPath))
			{
				buildInfo.ProjectAssetsPath = gotProjectAssetsPath;
			}

			// If no build platform is found from the Editor Log, it will just use `buildInfo.BuildTargetUsed`,
			// which gets its value from `EditorUserBuildSettings.activeBuildTarget`
			string gotBuildType = GetBuildTypeFromEditorLog(_lastEditorLogPath);
			BuildPlatform buildPlatform = GetBuildPlatformFromString(gotBuildType, buildInfo.BuildTargetUsed);

			//Debug.LogFormat("Build Type found in Editor Log: \"{0}\"\nDetermined build platform: {1}",
			//	gotBuildType, buildPlatform);

			if (string.IsNullOrEmpty(gotBuildType))
			{
				buildInfo.BuildType = buildPlatform.ToString();
			}
			else
			{
				buildInfo.BuildType = gotBuildType;
			}


			buildInfo.ProjectName = CustomBuildReport.Util.GetProjectName(buildInfo.ProjectAssetsPath);

			// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------

			string unityBuildReportFilePath = string.Format("{0}/{1}", _lastSavePath,
				CustomBuildReport.Util.GetUnityBuildReportDefaultFilename(buildInfo.ProjectName, buildInfo.BuildType, buildInfo.BuildTimeGot));
			_lastKnownUnityBuildReport = CustomBuildReport.Util.OpenSerialized<CustomBuildReport.UnityBuildReport>(unityBuildReportFilePath);
#if BRT_UNITY_BUILD_REPORT_DEBUG
			CustomBuildReport.Util.DebugLogBuildReport(_lastKnownUnityBuildReport);
#endif

			// Need to check if the last build made was for AssetBundles or not
			//
			// If AssetBundle:
			// 1. Need to create separate SizeParts and UsedAssets list per bundle that was built
			// 2. Do not include assets in Resources folder
			// 3. Skip DLLs and StreamingAssets
			// 3. Get file size per bundle
			//
			// If not AssetBundle (regular build):
			// 1. Only one SizeParts and UsedAssets list
			// 2. Include assets in Resources folder
			// 3. Get DLLs and StreamingAssets
			// 4. Get Total build size
			//

			// get last "Uncompressed usage by category (Percentages based on user generated assets only):"
			// go up one line, check if it starts with "Compressed Size:" or "Build Report"
			const string BUILD_LOG_KEY =
				"Uncompressed usage by category (Percentages based on user generated assets only):";
			const string BUNDLE_SIZE_KEY = "Compressed Size:";
			const string BUNDLE_NAME_KEY = "Bundle Name: ";
			const string BUILD_NAME_KEY = "Build Report";
			const string BUILD_DIV_KEY = "-------------------------------------------------------------";
			(long lastAssetBundleBuildLine, long lastProjectBuildLine) = DldUtil.BigFileReader.GetLineNumberTextWithPrevLines(_lastEditorLogPath,
				BUILD_LOG_KEY, BUNDLE_SIZE_KEY, BUNDLE_NAME_KEY,
				BUILD_LOG_KEY, BUILD_NAME_KEY, BUILD_DIV_KEY);

			//Debug.Log($"lastAssetBundleBuildLine: {lastAssetBundleBuildLine}\nlastProjectBuildLine: {lastProjectBuildLine}");

			// either there's no project build, or the asset bundle build is later than the project build
			bool isAssetBundle = lastAssetBundleBuildLine > -1 && (lastProjectBuildLine == -1 || lastAssetBundleBuildLine > lastProjectBuildLine);
			List<CustomBuildReport.BundleEntry> bundles;
			if (isAssetBundle)
			{
				bundles = new List<CustomBuildReport.BundleEntry>();
			}
			else
			{
				bundles = null;
				buildInfo.AssetBundles = null;
			}

			// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// DLLs

			if (!isAssetBundle)
			{
				BRT_BuildReportWindow.GetValueMessage = "Getting list of DLLs...";

				bool wasWebBuild = buildInfo.BuildType == "WebPlayer";
				bool wasWebGLBuild = buildInfo.BuildType == "WebGLSupport" || buildInfo.BuildType == "WebGL";

				//Debug.Log("going to call parseDLLs");
				ParseDLLs(_lastEditorLogPath, wasWebBuild, wasWebGLBuild, buildFilePath, projectAssetsPath,
					editorAppContentsPath, buildInfo.MonoLevel, buildInfo.CodeStrippingLevel,
					out buildInfo.MonoDLLs, out buildInfo.UnityEngineDLLs, out buildInfo.ScriptDLLs);

				//Debug.Log("ParseDLLs done");
			}


			// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// build sizes per category

			BRT_BuildReportWindow.GetValueMessage = "Getting build sizes...";

			if (isAssetBundle)
			{
				buildInfo.BuildSizes = null;

				bool gettingSizeParts = false;
				List<CustomBuildReport.SizePart> buildSizes = new List<CustomBuildReport.SizePart>();
				bool gotDateTimePrefix = false;
				int dateTimePrefixLen = 0;

				bool gettingUsedAssets = false;
				List<CustomBuildReport.SizePart> assetSizes = new List<CustomBuildReport.SizePart>();

				buildInfo.FileFilters = CustomBuildReport.FiltersUsed.GetProperFileFilterGroupToUse(_lastSavePath);

				foreach (string line in DldUtil.BigFileReader.ReadFile(_lastEditorLogPath, lastAssetBundleBuildLine-2))
				{
					// blank line signifies end of list
					if (string.IsNullOrWhiteSpace(line) || line == "\n" || line == "\r\n")
					{
						break;
					}

					//Debug.LogFormat("Bundle line: {0}", line);

					// --------------------------------------------
					// remove timestamp
					string gotLine = line;
					if (!gotDateTimePrefix)
					{
						Match dateTime = Regex.Match(line, DATE_TIME_PREFIX, RegexOptions.IgnoreCase);
						if (dateTime.Success)
						{
							dateTimePrefixLen = dateTime.Groups[0].Value.Length;
							gotDateTimePrefix = true;
							gotLine = line.Substring(dateTimePrefixLen);
						}
					}
					else if (dateTimePrefixLen > 0 && line.Length > dateTimePrefixLen && char.IsDigit(line[0]))
					{
						gotLine = line.Substring(dateTimePrefixLen);
					}

					// --------------------------------------------

					if (gotLine.StartsWith(BUNDLE_NAME_KEY))
					{
						// new bundle
						CustomBuildReport.BundleEntry newBundle = new CustomBuildReport.BundleEntry();
						newBundle.Name = gotLine.Substring(BUNDLE_NAME_KEY.Length);
						bundles.Add(newBundle);
					}
					else if (gotLine.StartsWith(BUNDLE_SIZE_KEY))
					{
						string sizeLine = gotLine.Substring(BUNDLE_SIZE_KEY.Length);
						Match dateTime = Regex.Match(sizeLine, DATE_TIME_PREFIX, RegexOptions.IgnoreCase);
						if (dateTime.Success)
						{
							sizeLine = sizeLine.Replace(dateTime.Groups[0].Value, "");
						}

						Debug.Assert(bundles.Count > 0, "bundles list count should be at least 1 at this point");
						bundles[bundles.Count - 1].TotalOutputSize = sizeLine.ToUpper();
					}
					else if (gettingSizeParts || gotLine.StartsWith(SIZE_PARTS_KEY))
					{
						if (!gettingSizeParts)
						{
							gettingSizeParts = true;
							buildSizes.Clear();
						}

						CustomBuildReport.SizePart inPart = CreateSizePartFromLine(gotLine, false, out bool stop);

						Debug.Assert(bundles.Count > 0, "bundles list count should be at least 1 at this point");
						if (stop)
						{
							gettingSizeParts = false;
							bundles[bundles.Count - 1].BuildSizes = buildSizes.ToArray();
						}
						else
						{
							if (inPart.Name == TOTAL_USER_ASSETS_SIZE_KEY)
							{
								bundles[bundles.Count - 1].TotalUserAssetsSize = inPart.Size.ToUpper();
								gettingSizeParts = false;
								bundles[bundles.Count - 1].BuildSizes = buildSizes.ToArray();
							}
							else
							{
								buildSizes.Add(inPart);
							}
						}
					}
					else if (gettingUsedAssets)
					{
						Debug.Assert(bundles.Count > 0, "bundles list count should be at least 1 at this point");
						CustomBuildReport.SizePart assetSizePart = CreateAssetSizeFromLine(gotLine, null, out bool stop);
						if (stop)
						{
							gettingUsedAssets = false;

							CustomBuildReport.SizePart[] newAllUsedArray = assetSizes.ToArray();

							CustomBuildReport.SizePart[][] newPerCategoryUsed =
								SegregateAssetSizesPerCategory(newAllUsedArray, buildInfo.FileFilters);
							bundles[bundles.Count - 1].UsedAssets = new AssetList();
							bundles[bundles.Count - 1].UsedAssets.Init(newAllUsedArray, newPerCategoryUsed,
								CustomBuildReport.Options.NumberOfTopLargestUsedAssetsToShow, buildInfo.FileFilters);
						}
						else if (assetSizePart != null)
						{
							assetSizes.Add(assetSizePart);
						}
					}
					else if (gotLine.StartsWith(ASSET_SIZES_KEY_2))
					{
						gettingUsedAssets = true;
						assetSizes.Clear();
					}
				}
			}
			else
			{
				//Debug.Log("ParseSizePartsFromString sta");

				buildInfo.BuildSizes = ParseSizePartsFromString(_lastEditorLogPath);

				//Debug.Log("ParseSizePartsFromString end");
			}


			// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// getting total asset size (uncompressed)

			buildInfo.UsedTotalSize = "";

			if (!isAssetBundle)
			{
				foreach (CustomBuildReport.SizePart b in buildInfo.BuildSizes)
				{
					if (b.IsTotal)
					{
						buildInfo.UsedTotalSize = b.Size;
						break;
					}
				}
			}

			// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// getting streaming assets size (uncompressed)

			if (!isAssetBundle)
			{
				BRT_BuildReportWindow.GetValueMessage = "Getting Streaming Assets size...";

				var streamingAssetsPath = projectAssetsPath + "/StreamingAssets";

				if (calculateBuildSize) // CustomBuildReport.Options.IncludeBuildSizeInReportCreation
				{
					buildInfo.StreamingAssetsSize = CustomBuildReport.Util.GetFolderSizeReadable(streamingAssetsPath);
				}

				foreach (CustomBuildReport.SizePart b in buildInfo.BuildSizes)
				{
					if (b.IsStreamingAssets)
					{
						b.DerivedSize = CustomBuildReport.Util.GetFolderSizeInBytes(streamingAssetsPath);
						b.Size = CustomBuildReport.Util.GetBytesReadable(b.DerivedSize);
						break;
					}
				}
			}

			// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// getting compressed total build size

			buildInfo.TotalBuildSize = "";

			if (calculateBuildSize)
			{
				BRT_BuildReportWindow.GetValueMessage = "Getting final build size...";
				//Debug.LogFormat("trying to get size for {0} of {1} ({2})",
				//	buildPlatform, buildFilePath, buildInfo.UnityVersion);

				// note: buildFilePath is the path to the build, as given by EditorUserBuildSettings.GetBuildLocation()

				if (isAssetBundle)
				{
					buildInfo.TotalBuildSize = "";
				}
				else if (buildPlatform == BuildPlatform.Flash)
				{
					// in Flash builds, `buildFilePath` is the .swf file

					buildInfo.TotalBuildSize = CustomBuildReport.Util.GetPathSizeReadable(buildFilePath);
				}
				else if (buildPlatform == BuildPlatform.Android)
				{
					//Debug.Log("trying to get size of: " + buildFilePath);

					// in Unity 4, Android can generate an Eclipse project if set so in the build settings
					// or an .apk with an accompanying .obb file, which we should take into account

					// check if an .obb file was generated and get its file size


					if (!buildInfo.AndroidCreateProject && !buildInfo.AndroidUseAPKExpansionFiles)
					{
						// .apk without an .obb

						buildInfo.TotalBuildSize = CustomBuildReport.Util.GetPathSizeReadable(buildFilePath);
					}
					else if (!buildInfo.AndroidCreateProject && buildInfo.AndroidUseAPKExpansionFiles)
					{
						// .apk with .obb

						// get the .apk file but remove the file type
						string obbPath = CustomBuildReport.Util.ReplaceFileType(buildFilePath, ".main.obb");

						double obbSize = CustomBuildReport.Util.GetPathSizeInBytes(obbPath);
						double apkSize = CustomBuildReport.Util.GetPathSizeInBytes(buildFilePath);

						buildInfo.TotalBuildSize = CustomBuildReport.Util.GetBytesReadable(apkSize + obbSize);
						buildInfo.AndroidApkFileBuildSize = CustomBuildReport.Util.GetBytesReadable(apkSize);
						buildInfo.AndroidObbFileBuildSize = CustomBuildReport.Util.GetBytesReadable(obbSize);
					}
					else if (buildInfo.AndroidCreateProject)
					{
						// total build size is size of the eclipse project folder
						buildInfo.TotalBuildSize = CustomBuildReport.Util.GetPathSizeReadable(buildFilePath);

						// if there is .obb, find it
						if (buildInfo.AndroidUseAPKExpansionFiles)
						{
							// the .obb is inside this folder
							buildInfo.AndroidObbFileBuildSize =
								CustomBuildReport.Util.GetObbSizeInEclipseProjectReadable(buildFilePath);
						}
					}
					else
					{
						// ???
						buildInfo.TotalBuildSize = CustomBuildReport.Util.GetPathSizeReadable(buildFilePath);
					}
				}
				else if (buildPlatform == BuildPlatform.Web)
				{
					// in web builds, `buildFilePath` is the folder
					buildInfo.TotalBuildSize = CustomBuildReport.Util.GetPathSizeReadable(buildFilePath);

					if (Directory.Exists(buildFilePath))
					{
						// find a .unity3d file inside the build folder and get its file size
						foreach (
							var file in TraverseDirectory
							            .Do(buildFilePath).Where(file => CustomBuildReport.Util.IsFileOfType(file, ".unity3d"))
						)
						{
							buildInfo.WebFileBuildSize = CustomBuildReport.Util.GetPathSizeReadable(file);
							break;
						}
					}
				}
				else if (
					buildPlatform == BuildPlatform.Windows32 ||
					buildPlatform == BuildPlatform.Windows64 ||
					buildPlatform == BuildPlatform.Linux32 ||
					buildPlatform == BuildPlatform.Linux64 ||
					buildPlatform == BuildPlatform.LinuxUniversal ||
					(buildPlatform == BuildPlatform.None &&
					 (buildFilePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
					  buildFilePath.EndsWith(".x86", StringComparison.OrdinalIgnoreCase) ||
					  buildFilePath.EndsWith(".x86_64", StringComparison.OrdinalIgnoreCase))))
				{
					//Debug.LogFormat(
					//	"CustomBuildReport.ReportGenerator: Getting Total Build Size: Detected Windows/Linux buildFilePath: {0}",
					//	buildFilePath);

					// in Windows/Linux builds, `buildFilePath` is only the executable file (.exe, .x86, or .x86_64 file).
					// we still need to get the size of the Data folder

					buildInfo.TotalBuildSize =
						CustomBuildReport.Util.GetBytesReadable(GetStandaloneBuildSize(buildFilePath, buildInfo.UnityVersion));
				}
				else if (
					buildPlatform == BuildPlatform.MacOSX32 ||
					buildPlatform == BuildPlatform.MacOSX64 ||
					buildPlatform == BuildPlatform.MacOSXUniversal)
				{
					//Debug.LogFormat(
					//	"CustomBuildReport.ReportGenerator: Getting Total Build Size: Detected Mac OS X buildFilePath: {0}",
					//	buildFilePath);

					// in Mac builds, `buildFilePath` is the .app file (which is really just a folder)
					buildInfo.TotalBuildSize = CustomBuildReport.Util.GetPathSizeReadable(buildFilePath);
				}
				else if (buildPlatform == BuildPlatform.iOS)
				{
					// in iOS builds, `buildFilePath` is the Xcode project folder
					buildInfo.TotalBuildSize = CustomBuildReport.Util.GetPathSizeReadable(buildFilePath);
				}
				else
				{
					//Debug.LogFormat(
					//	"CustomBuildReport.ReportGenerator: Getting Total Build Size: Unknown build platform: {0}",
					//	buildFilePath);

					// in console builds, `buildFilePath` is ???
					// last resort for unknown build platforms
					buildInfo.TotalBuildSize = CustomBuildReport.Util.GetPathSizeReadable(buildFilePath);
				}
			}


			// for debug
			//GetCompressedSizeReadingFromLog();

			// ensure this is not used anymore on new reports
			// (it's still there for old build report XML files)
			//buildInfo.CompressedBuildSize = "";


			buildInfo.UnusedTotalSize = "";


			// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// assets list

			if (buildInfo.UsedAssetsIncludedInCreation)
			{
				BRT_BuildReportWindow.GetValueMessage = "Getting list of used assets...";

				// asset list

				if (!isAssetBundle)
				{
					buildInfo.FileFilters = CustomBuildReport.FiltersUsed.GetProperFileFilterGroupToUse(_lastSavePath);
				}

				if (isAssetBundle)
				{
					buildInfo.UsedAssets = null;

					if (buildInfo.UnusedAssetsIncludedInCreation)
					{
						BRT_BuildReportWindow.GetValueMessage = "Getting list of unused assets...";

						AssignAllUnusedAssets(buildInfo,
							projectAssetsPath,
							buildPlatform,
							null);
					}
				}
				else
				{
					var allUsed = ParseAssetSizesFromEditorLog(_lastEditorLogPath, PrefabsUsedInScenesList);

					var scenes = buildInfo.ScenesInBuild;
					if (scenes != null)
					{
						// add Unity scene files into the Used Assets list even though technically they do not show up there

						string projectPath = CustomBuildReport.Util.GetProjectPath(buildInfo.ProjectAssetsPath);

						string buildDataFolderPath = CustomBuildReport.Util.GetBuildDataFolder(buildFilePath);

						int enabledSceneIdx = 0;
						for (int n = 0, len = scenes.Length; n < len; ++n)
						{
							if (!scenes[n].Enabled)
							{
								// disabled scene means it was not included in the build
								continue;
							}

							//Debug.Log("Scene " + n + ": " + projectPath + scenes[n].path + " enabled: " + scenes[n].enabled + " level" + enabledSceneIdx);

							var sceneSizePart =
								CustomBuildReport.Util.CreateSizePartFromFile(scenes[n].Path, projectPath + scenes[n].Path, false);

							if (!string.IsNullOrEmpty(buildDataFolderPath))
							{
								// in standalone builds, a unity scene file is found in the _Data folder as files with filename level0 ... leveln
								// the number index there being the scene's index in the build

								var fileInBuild = string.Format("{0}/level{1}", buildDataFolderPath, enabledSceneIdx);

								if (File.Exists(fileInBuild))
								{
									long fileSizeBytes = CustomBuildReport.Util.GetFileSizeInBytes(fileInBuild);
									sceneSizePart.RawSizeBytes = fileSizeBytes;
									sceneSizePart.RawSize = CustomBuildReport.Util.GetBytesReadable(fileSizeBytes);
								}
							}

							allUsed.Add(sceneSizePart);
							++enabledSceneIdx;
						}
					}

					// remove scripts that aren't included in the build
					// by checking if the assembly they belong to is not in the build
					if (_lastAssemblies != null)
					{
						foreach (Assembly assembly in _lastAssemblies)
						{
							string assemblyFilename = Path.GetFileName(assembly.outputPath);

							if (buildInfo.ScriptDLLs.Exists(assemblyFilename) ||
							    buildInfo.UnityEngineDLLs.Exists(assemblyFilename))
							{
								continue;
							}

							// this is an assembly that isn't included in the build
							// any source file of this assembly shouldn't be in the used list
							foreach (string sourceFile in assembly.sourceFiles)
							{
								int sourceFileIdxInAllUsed = allUsed.FindIdx(sourceFile);
								if (sourceFileIdxInAllUsed == -1)
								{
									continue;
								}

								allUsed.RemoveAt(sourceFileIdxInAllUsed);
							}
						}
					}

					//Debug.Log("buildInfo.UsedAssets.All: " + buildInfo.UsedAssets.All.Length);

					if (buildInfo.UnusedAssetsIncludedInCreation)
					{
						BRT_BuildReportWindow.GetValueMessage = "Getting list of unused assets...";

						AssignAllUnusedAssets(buildInfo,
							projectAssetsPath,
							buildPlatform,
							allUsed);
					}

					CustomBuildReport.SizePart[] allUsedArray = allUsed.ToArray();

					CustomBuildReport.SizePart[][] perCategoryUsed =
						SegregateAssetSizesPerCategory(allUsedArray, buildInfo.FileFilters);
					buildInfo.UsedAssets = new AssetList();
					buildInfo.UsedAssets.Init(allUsedArray, perCategoryUsed,
						CustomBuildReport.Options.NumberOfTopLargestUsedAssetsToShow, buildInfo.FileFilters);
				}
			}


			buildInfo.SortSizes();

			if (!isAssetBundle)
			{
				Array.Sort(buildInfo.MonoDLLs, delegate(CustomBuildReport.SizePart b1, CustomBuildReport.SizePart b2)
				{
					if (b1.SizeBytes > b2.SizeBytes) return -1;
					if (b1.SizeBytes < b2.SizeBytes) return 1;
					return string.Compare(b1.Name, b2.Name, StringComparison.Ordinal);
				});
				Array.Sort(buildInfo.UnityEngineDLLs, delegate(CustomBuildReport.SizePart b1, CustomBuildReport.SizePart b2)
				{
					if (b1.SizeBytes > b2.SizeBytes) return -1;
					if (b1.SizeBytes < b2.SizeBytes) return 1;
					return string.Compare(b1.Name, b2.Name, StringComparison.Ordinal);
				});
				Array.Sort(buildInfo.ScriptDLLs, delegate(CustomBuildReport.SizePart b1, CustomBuildReport.SizePart b2)
				{
					if (b1.SizeBytes > b2.SizeBytes) return -1;
					if (b1.SizeBytes < b2.SizeBytes) return 1;
					return string.Compare(b1.Name, b2.Name, StringComparison.Ordinal);
				});
			}

			//foreach (string d in EditorUserBuildSettings.activeScriptCompilationDefines)
			//{
			//	Debug.Log("define: " + d);
			//}

			if (isAssetBundle)
			{
				buildInfo.AssetBundles = bundles.ToArray();
			}

			// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// duration of build generation

			System.TimeSpan timeNow = new System.TimeSpan(System.DateTime.Now.Ticks);
			buildInfo.ReportGenerationTime = timeNow - new System.TimeSpan(buildInfo.TimeGot.Ticks);

			BRT_BuildReportWindow.GetValueMessage = "";

			buildInfo.FlagOkToRefresh();
		}

		//public static void ChangeSavePathToUserPersonalFolder()
		//{
		//CustomBuildReport.Options.BuildReportSavePath = CustomBuildReport.Util.GetUserHomeFolder();
		//}

		public static string GetSavePathToProjectFolder()
		{
			string projectParent;
			if (_lastKnownBuildInfo != null)
			{
				projectParent = _lastKnownBuildInfo.ProjectAssetsPath;
			}
			else
			{
				projectParent = Application.dataPath;
			}

			const string SUFFIX_STRING_TO_REMOVE = "/Assets";
			projectParent = CustomBuildReport.Util.RemoveSuffix(SUFFIX_STRING_TO_REMOVE, projectParent);

			int lastSlashIdx = projectParent.LastIndexOf("/", StringComparison.Ordinal);
			projectParent = projectParent.Substring(0, lastSlashIdx);
			return projectParent;
			//CustomBuildReport.Options.BuildReportSavePath = projectParent;
			//Debug.Log("projectParent: " + projectParent);
		}


		/// <summary>
		/// Called by <see cref="BRT_BuildReportWindow.Refresh"/> to start creating a build report.
		/// </summary>
		///
		/// Called when the "Get Log" button is pressed by the user in the BRT_BuildReportWindow.
		///
		/// Can also be called due to BRT_BuildReportWindow's <see cref="BRT_BuildReportWindow.OnInspectorUpdate"/>,
		/// when it has detected that a build has completed, and a Build Report creation was scheduled
		/// (<see cref="CustomBuildReport.Util.ShouldGetBuildReportNow"/>). This was scheduled for us
		/// when <see cref="OnPostprocessBuild"/> was called, which gets called automatically by Unity
		/// when a build has finished.
		///
		/// <param name="buildInfo"></param>
		/// <param name="assetDependencies"></param>
		/// <param name="textureData"></param>
		/// <returns></returns>
		public static bool RefreshData(bool fromBuild, CustomBuildReport.AssetBundleSession assetBundleSession, ref CustomBuildReport.BuildInfo buildInfo,
			ref CustomBuildReport.AssetDependencies assetDependencies, ref CustomBuildReport.TextureData textureData,
			ref CustomBuildReport.MeshData meshData, ref CustomBuildReport.PrefabData prefabData)
		{
			// this would have been set to true in CustomBuildReport.ReportGenerator.OnPostprocessBuild
			// which allowed BRT_BuildReportWindow.OnInspectorUpdate() to get here
			CustomBuildReport.Util.ClearShouldGetBuildReportNow();

			if (!DoesEditorLogHaveBuildInfo(CustomBuildReport.Util.UsedEditorLogPath))
			{
				string lastSuccessfulBuildEditorLog = CustomBuildReport.Util.LastSuccessfulBuildFilePath(Application.dataPath);
				if (DoesEditorLogHaveBuildInfo(lastSuccessfulBuildEditorLog))
				{
					_lastEditorLogPath = lastSuccessfulBuildEditorLog;
				}
				else
				{
					if (DoesEditorLogHaveNoBuildInfoDueToNoPlayerRebuilt(CustomBuildReport.Util.UsedEditorLogPath))
					{
						Debug.LogWarning("No new player data was created since no changes were detected. Do a clean build to force creation of a new player data build.");
					}
					else if (CustomBuildReport.Util.IsDefaultEditorLogPathOverridden)
					{
						Debug.LogWarning(string.Format(NO_BUILD_INFO_OVERRIDDEN_LOG_WARNING,
							CustomBuildReport.Util.UsedEditorLogPath, CustomBuildReport.Options.FoundPathForSavedOptions));
					}
					else if (CheckIfUnityHasNoLogArgument())
					{
						Debug.LogWarning(NO_BUILD_INFO_NO_LOG_WARNING);
					}
					else
					{
						Debug.LogWarning(NO_BUILD_INFO_WARNING);
					}

					return false;
				}
			}

			// --------------------

			var timeNow = System.DateTime.Now;

			// --------------------

			// get important values from the Unity API
			// (which can only be retrieved in the main thread)
			Init(fromBuild, ref buildInfo, null);
			buildInfo.TimeGot = timeNow;
			buildInfo.TimeGotReadable = timeNow.ToString(TIME_OF_BUILD_FORMAT);

			System.DateTime timeBuildStarted;
			if (fromBuild && CustomBuildReport.Util.HasBuildTime())
			{
				timeBuildStarted = CustomBuildReport.Util.LoadBuildTime();
			}
			else
			{
				timeBuildStarted = timeNow;
			}

			if (CustomBuildReport.Options.CalculateAssetDependencies)
			{
				if (assetDependencies == null)
				{
					assetDependencies = new CustomBuildReport.AssetDependencies();
				}

				assetDependencies.TimeGot = timeBuildStarted;
			}

			if (CustomBuildReport.Options.CollectTextureImportSettings)
			{
				if (textureData == null)
				{
					textureData = new CustomBuildReport.TextureData();
				}

				textureData.TimeGot = timeBuildStarted;
			}
			else
			{
				if (textureData != null)
				{
					textureData.Clear();
				}
			}

			if (CustomBuildReport.Options.CollectMeshData)
			{
				if (meshData == null)
				{
					meshData = new CustomBuildReport.MeshData();
				}

				meshData.TimeGot = timeBuildStarted;
			}
			else
			{
				if (meshData != null)
				{
					meshData.Clear();
				}
			}

			if (CustomBuildReport.Options.CollectPrefabData)
			{
				if (prefabData == null)
				{
					prefabData = new CustomBuildReport.PrefabData();
				}

				prefabData.TimeGot = timeBuildStarted;
			}
			else
			{
				if (prefabData != null)
				{
					prefabData.Clear();
				}
			}

			// --------------------

			// getting prefabs has to be done in the main thread
			// since it uses the Unity API (AssetDatabase.GetDependencies)
			if (CustomBuildReport.Options.IncludeUnusedPrefabsInReportCreation)
			{
				RefreshListOfAllPrefabsUsedInAllScenesIncludedInBuild();
			}
			else
			{
				ClearListOfAllPrefabsUsedInAllScenes();
			}

			CommitAdditionalInfoToCache();

			// --------------------

			CreateBuildReportInBackgroundIfNeeded(buildInfo, assetBundleSession, assetDependencies);

			return true;
		}

		/// <summary>
		/// Called by <see cref="RefreshData"/> to create the Build Report.
		/// </summary>
		///
		/// <para>It will be done either on the main thread or on a new one, depending
		/// on the value of <see cref="CustomBuildReport.Options.UseThreadedReportGeneration"/>.</para>
		///
		/// <para>Once it's done, <see cref="_gettingValuesCurrentState"/> will be set to
		/// <see cref="GettingValues.Finished"/>, to signal the rest of the code to continue.
		/// Specifically, <see cref="BRT_BuildReportWindow.OnInspectorUpdate"/> keeps checking
		/// that state and when it does, it calls
		/// <see cref="BRT_BuildReportWindow.OnFinishGeneratingBuildReport"/> as the next step.</para>
		///
		static void CreateBuildReportInBackgroundIfNeeded(CustomBuildReport.BuildInfo buildInfo, CustomBuildReport.AssetBundleSession assetBundleSession,
			CustomBuildReport.AssetDependencies assetDependencies)
		{
			//Debug.Log("starting thread");
			_shouldCalculateBuildSize = CustomBuildReport.Options.IncludeBuildSizeInReportCreation;

			_gettingValuesCurrentState = GettingValues.Yes;

			if (CustomBuildReport.Options.UseThreadedReportGeneration)
			{
				// the only things we do is get values from the Editor.log txt file
				// so it's safe to do it in a separate thread, nothing in the Unity API
				// is used.

				Thread thread = new Thread(delegate()
				{
					CreateBuildReport(buildInfo);
					assetBundleSession.Refresh(buildInfo);
				});
				thread.Start();
			}
			else
			{
				CreateBuildReport(buildInfo);
				assetBundleSession.Refresh(buildInfo);
			}
		}

		/// <summary>
		/// Finally go and create the contents of the Build Report, based on
		/// the values given in the Editor.log text file, and other info prepared
		/// beforehand.
		/// </summary>
		///
		/// Once it's done, <see cref="_gettingValuesCurrentState"/> will be set to
		/// <see cref="GettingValues.Finished"/>, to signal the rest of the code to continue.
		/// Specifically, <see cref="BRT_BuildReportWindow.OnInspectorUpdate"/> keeps checking
		/// that state and when it does, it calls
		/// <see cref="BRT_BuildReportWindow.OnFinishGeneratingBuildReport"/> as the next step.
		///
		/// <param name="buildInfo">The BuildInfo to populate.</param>
		static void CreateBuildReport(CustomBuildReport.BuildInfo buildInfo)
		{
			//Debug.Log("in thread");

			GetValues(buildInfo, buildInfo.BuildFilePath, buildInfo.ProjectAssetsPath, buildInfo.EditorAppContentsPath,
				_shouldCalculateBuildSize);

			//Debug.Log("done thread");
			_gettingValuesCurrentState = GettingValues.Finished;

			// the next part of the code that gets executed is BRT_BuildReportWindow.OnFinishGeneratingBuildReport()
		}

		public static string OnFinishedGetValues(CustomBuildReport.BuildInfo buildInfo,
			CustomBuildReport.AssetDependencies assetDependencies, CustomBuildReport.TextureData textureData,
			CustomBuildReport.MeshData meshData, CustomBuildReport.PrefabData prefabData, string customSavePath = null)
		{
			string resultingFilePath = null;

			if (buildInfo.HasUsedAssets)
			{
				if (CustomBuildReport.Options.ShowImportedSizeForUsedAssets)
				{
					buildInfo.UsedAssets.PopulateImportedSizes();
				}

				if (CustomBuildReport.Options.GetSizeBeforeBuildForUsedAssets)
				{
					buildInfo.UsedAssets.PopulateSizeInAssetsFolder();
				}
			}

			if (buildInfo.HasUnusedAssets && CustomBuildReport.Options.GetImportedSizesForUnusedAssets)
			{
				buildInfo.UnusedAssets.PopulateImportedSizes();
			}

			buildInfo.FixReport();

			// ------------------------------

			// Asset dependency calculation has to be done *after* build report has been created,
			// but it also has to be done in the main thread, since it makes use of the Unity Editor API
			// (UnityEditor.AssetDatabase.GetDependencies()).
			if (CustomBuildReport.Options.CalculateAssetDependencies)
			{
				assetDependencies.ProjectName = buildInfo.ProjectName;
				assetDependencies.BuildType = buildInfo.BuildType;

				CustomBuildReport.AssetDependencyGenerator.Create(assetDependencies, buildInfo, CustomBuildReport.Options.CalculateAssetDependenciesOnUnusedToo,
#if BRT_ASSET_DEPENDENCY_DEBUG
					true
#else
					false
#endif
				);
			}

			// ------------------------------

			if (CustomBuildReport.Options.CollectTextureImportSettings)
			{
				textureData.ProjectName = buildInfo.ProjectName;
				textureData.BuildType = buildInfo.BuildType;

				CustomBuildReport.TextureDataGenerator.Create(textureData, buildInfo, CustomBuildReport.Options.CollectTextureImportSettingsOnUnusedToo,
#if BRT_TEXTURE_DATA_DEBUG
					true
#else
					false
#endif
				);
			}

			// ------------------------------

			if (CustomBuildReport.Options.CollectMeshData)
			{
				meshData.ProjectName = buildInfo.ProjectName;
				meshData.BuildType = buildInfo.BuildType;

				CustomBuildReport.MeshDataGenerator.Create(meshData, buildInfo, CustomBuildReport.Options.CollectMeshDataOnUnusedToo,
#if BRT_MESH_DATA_DEBUG
					true
#else
					false
#endif
				);
			}

			// ------------------------------

			if (CustomBuildReport.Options.CollectPrefabData)
			{
				prefabData.ProjectName = buildInfo.ProjectName;
				prefabData.BuildType = buildInfo.BuildType;

				CustomBuildReport.PrefabDataGenerator.CreateForUsedAssetsOnly(prefabData, buildInfo);
			}

			// ------------------------------
			// CustomBuildReport.Util.ShouldSaveGottenBuildReportNow was set to true on
			// CustomBuildReport.ReportGenerator.OnPostprocessBuild,
			// which is called automatically after a build
			// so by this time, it should be true
			if (CustomBuildReport.Util.ShouldSaveGottenBuildReportNow)
			{
				CustomBuildReport.Util.ShouldSaveGottenBuildReportNow = false;

				string savePathToUse = string.IsNullOrEmpty(customSavePath)
					? _lastSavePath
					: customSavePath;

				if (!buildInfo.HasContents)
				{
					Debug.LogError("Build Report Tool: No build data detected. Try doing a Clean Build.");

					// Since the build data is invalid, any temporarily saved build report file should be deleted.
					string buildReportFile = CustomBuildReport.Util.GetDataFilePath(buildInfo, savePathToUse);
					if (System.IO.File.Exists(buildReportFile))
					{
						System.IO.File.Delete(buildReportFile);
					}

					if (CustomBuildReport.Options.CalculateAssetDependencies)
					{
						string assetDependenciesFile = CustomBuildReport.Util.GetDataFilePath(assetDependencies, savePathToUse);
						if (System.IO.File.Exists(assetDependenciesFile))
						{
							System.IO.File.Delete(assetDependenciesFile);
						}
					}

					if (CustomBuildReport.Options.CollectTextureImportSettings)
					{
						string textureDataFile = CustomBuildReport.Util.GetDataFilePath(textureData, savePathToUse);
						if (System.IO.File.Exists(textureDataFile))
						{
							System.IO.File.Delete(textureDataFile);
						}
					}

					if (CustomBuildReport.Options.CollectMeshData)
					{
						string meshDataFile = CustomBuildReport.Util.GetDataFilePath(meshData, savePathToUse);
						if (System.IO.File.Exists(meshDataFile))
						{
							System.IO.File.Delete(meshDataFile);
						}
					}

					if (CustomBuildReport.Options.CollectPrefabData)
					{
						string prefabDataFile = CustomBuildReport.Util.GetDataFilePath(prefabData, savePathToUse);
						if (System.IO.File.Exists(prefabDataFile))
						{
							System.IO.File.Delete(prefabDataFile);
						}
					}

					return null;
				}

				resultingFilePath = CustomBuildReport.Util.SerializeAtFolder(buildInfo, savePathToUse);

				if (CustomBuildReport.Options.CalculateAssetDependencies)
				{
					CustomBuildReport.Util.SerializeAtFolder(assetDependencies, savePathToUse);
				}

				if (CustomBuildReport.Options.CollectTextureImportSettings)
				{
					CustomBuildReport.Util.SerializeAtFolder(textureData, savePathToUse);
				}

				if (CustomBuildReport.Options.CollectMeshData)
				{
					CustomBuildReport.Util.SerializeAtFolder(meshData, savePathToUse);
				}

				if (CustomBuildReport.Options.CollectPrefabData)
				{
					CustomBuildReport.Util.SerializeAtFolder(prefabData, savePathToUse);
				}
			}

			_gettingValuesCurrentState = GettingValues.No;

			FixZeroSizeUsedAssetEntries(buildInfo);

			return resultingFilePath;
		}

		static void FixZeroSizeUsedAssetEntries(BuildInfo buildInfo)
		{
			if (!buildInfo.UsedAssetsIncludedInCreation)
			{
				return;
			}

			AssetList usedAssetsList = buildInfo.UsedAssets;

			if (usedAssetsList == null)
			{
				return;
			}

			SizePart[] usedAssets = usedAssetsList.All;

			if (usedAssets == null || usedAssets.Length == 0)
			{
				return;
			}

			bool sizeWasChangedAtLeastOnce = false;

			string projectPath = CustomBuildReport.Util.GetProjectPath(buildInfo.ProjectAssetsPath);

			for (int n = 0, len = usedAssets.Length; n < len; ++n)
			{
				// files in StreamingAssets folder do not get imported in the 1st place
				// so skip them
				if (Util.IsFileStreamingAsset(usedAssets[n].Name))
				{
					continue;
				}

				if (usedAssets[n].Size == "N/A")
				{
					continue;
				}

				if (usedAssets[n].DerivedSize <= 0.0 && usedAssets[n].SizeBytes <= 0)
				{
					// got size from log was 0?
					// likely the asset was so small, Unity rounded off the value to 0
					// then we forcibly get the imported size

					long realSize = CustomBuildReport.Util.GetFileSizeInBytes(projectPath + usedAssets[n].Name);

					// but check first if real file size really is 0, then we need to indicate to the user that "hey, this file actually is empty"
					if (realSize <= 0)
					{
						continue;
					}

					sizeWasChangedAtLeastOnce = true;

					// here's the weird thing:
					// when the asset is text, Unity reports the file size based on the .txt file's real size on disk
					// when it's a texture image, Unity reports the file size based on the imported size
					// when it's a material, seems Unity does some extra compressing because it ends up smaller than either raw size or imported size
					//
					// seems it's really different per asset type
					//
					// so we'll make our own rules:
					// just use whichever value is smaller: raw or imported size.

					long importedSize = BRT_LibCacheUtil.GetImportedFileSize(usedAssets[n].Name);

					long sizeToUse = Math.Min(realSize, importedSize);

					usedAssets[n].SizeBytes = sizeToUse;
					usedAssets[n].Size = CustomBuildReport.Util.GetBytesReadable(usedAssets[n].SizeBytes);

					//Debug.Log("asset \"" + usedAssets[n].Name + "\" size from log is " + usedAssets[n].DerivedSize + " so we calculated imported size and got: " + usedAssets[n].SizeBytes + " real size is " + CustomBuildReport.Util.GetBytesReadable(realSize));
				}
			}

			if (sizeWasChangedAtLeastOnce)
			{
				// resort asset list since sizes were changed
				usedAssetsList.ResortDefault(CustomBuildReport.Options.NumberOfTopLargestUsedAssetsToShow);
			}
		}


		enum GettingValues
		{
			/// <summary>
			/// Initial state, not doing anything.
			/// </summary>
			No,

			/// <summary>
			/// Currently in the middle of creating a report.
			/// </summary>
			Yes,

			/// <summary>
			/// Just finished generating a build report and is ready to be saved.
			/// </summary>
			Finished
		}

		static GettingValues _gettingValuesCurrentState = GettingValues.No;

		public static bool IsStillGettingValues
		{
			get { return _gettingValuesCurrentState == GettingValues.Yes; }
		}

		public static bool IsFinishedGettingValues
		{
			get { return _gettingValuesCurrentState == GettingValues.Finished; }
		}


		public static void RecategorizeAssetList(BuildInfo buildInfo)
		{
			buildInfo.RecategorizeAssetLists();
			buildInfo.FlagOkToRefresh();
		}

		const string EDITOR_WINDOW_TITLE = "Build Report";

		[MenuItem("Window/Show Build Report")]
		public static void ShowBuildReport()
		{
			//RefreshData(ref _lastKnownBuildInfo);

			// close any existing window first, in case it's stuck in an error
			BRT_BuildReportWindow brtWindow =
				(BRT_BuildReportWindow) EditorWindow.GetWindow(typeof(BRT_BuildReportWindow), false, EDITOR_WINDOW_TITLE,
					true);
			brtWindow.Close();

			ShowBuildReportWithLastValues();
		}

		// has to be called in main thread
		static void ShowBuildReportWithLastValues()
		{
			//BRT_BuildReportWindow window = ScriptableObject.CreateInstance<BRT_BuildReportWindow>();
			//window.ShowUtility();

			//Debug.Log("showing build report window...");

			//BRT_BuildReportWindow brtWindow = EditorWindow.GetWindow<BRT_BuildReportWindow>("Build Report", true, typeof(SceneView));
			BRT_BuildReportWindow brtWindow =
				(BRT_BuildReportWindow) EditorWindow.GetWindow(typeof(BRT_BuildReportWindow), false, EDITOR_WINDOW_TITLE,
					true);
			//BRT_BuildReportWindow brtWindow = EditorWindow.GetWindow(typeof(BRT_BuildReportWindow), false, "Build Report", true) as BRT_BuildReportWindow;
			brtWindow.Init(_lastKnownBuildInfo);
		}
	}
} // namespace CustomBuildReport