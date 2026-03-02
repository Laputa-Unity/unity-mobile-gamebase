using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace CustomBuildReport.Window.Screen
{
	public class SizeStats : BaseScreen
	{
		public override string Name
		{
			get { return Labels.SIZE_STATS_CATEGORY_LABEL; }
		}

		public override void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies,
			TextureData textureData, MeshData meshData, PrefabData prefabData, UnityBuildReport unityBuildReport, CustomBuildReport.AssetBundleSession assetBundleSession)
		{
		}

		Vector2 _assetListScrollPos;

		bool _hasTotalBuildSize;
		bool _hasUsedAssetsTotalSize;
		bool _hasBuildSizes;
		bool _hasAssetBundles;
		bool _hasCompressedBuildSize;
		bool _hasMonoDLLsToDisplay;
		bool _hasUnityEngineDLLsToDisplay;
		bool _hasScriptDLLsToDisplay;

		public override void DrawGUI(Rect position,
			BuildInfo buildReportToDisplay, AssetDependencies assetDependencies,
			TextureData textureData, MeshData meshData, PrefabData prefabData,
			UnityBuildReport unityBuildReport, CustomBuildReport.ExtraData extraData, CustomBuildReport.AssetBundleSession assetBundleSession,
			out bool requestRepaint)
		{
			if (buildReportToDisplay == null)
			{
				requestRepaint = false;
				return;
			}

			requestRepaint = false;

			if (Event.current.type == EventType.Layout)
			{
				_hasTotalBuildSize = !string.IsNullOrEmpty(buildReportToDisplay.TotalBuildSize) &&
				                     !string.IsNullOrEmpty(buildReportToDisplay.BuildFilePath);

				_hasUsedAssetsTotalSize = !string.IsNullOrEmpty(buildReportToDisplay.UsedTotalSize);
				_hasCompressedBuildSize = !string.IsNullOrEmpty(buildReportToDisplay.CompressedBuildSize);
				_hasBuildSizes = buildReportToDisplay.BuildSizes != null;
				_hasMonoDLLsToDisplay = buildReportToDisplay.MonoDLLs != null && buildReportToDisplay.MonoDLLs.Length > 0;

				_hasUnityEngineDLLsToDisplay = buildReportToDisplay.UnityEngineDLLs != null &&
				                               buildReportToDisplay.UnityEngineDLLs.Length > 0;

				_hasScriptDLLsToDisplay =
					buildReportToDisplay.ScriptDLLs != null && buildReportToDisplay.ScriptDLLs.Length > 0;

				_hasAssetBundles = buildReportToDisplay.HasAssetBundles;
			}

			// Toolbar at top
			// ------------------------------------------------
			if (_hasAssetBundles)
			{
				DrawTopBar(position, assetBundleSession);
			}

			// ------------------------------------------------

			_assetListScrollPos = GUILayout.BeginScrollView(_assetListScrollPos);

			GUILayout.Space(10); // top padding for content

			GUILayout.BeginHorizontal();
			GUILayout.Space(10); // extra left padding

			DrawTotalSize(buildReportToDisplay, assetBundleSession);

			GUILayout.Space(CustomBuildReport.Window.Settings.CATEGORY_HORIZONTAL_SPACING);
			GUILayout.BeginVertical();

			CustomBuildReport.SizePart[] buildSizesToDisplay;
			if (_hasAssetBundles)
			{
				buildSizesToDisplay = buildReportToDisplay.AssetBundles[assetBundleSession.SelectedBundleIdx].BuildSizes;
			}
			else if (_hasBuildSizes)
			{
				buildSizesToDisplay = buildReportToDisplay.BuildSizes;
			}
			else
			{
				buildSizesToDisplay = null;
			}

			DrawBuildSizes(buildSizesToDisplay);

			GUILayout.Space(CustomBuildReport.Window.Settings.CATEGORY_VERTICAL_SPACING);

			DrawDLLList(buildReportToDisplay);

			GUILayout.EndVertical();
			GUILayout.Space(20); // extra right padding
			GUILayout.EndHorizontal();

			GUILayout.EndScrollView();
		}

		void DrawTopBar(Rect position, AssetBundleSession assetBundleSession)
		{
			var topBarBgStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.TOP_BAR_BG_STYLE_NAME);
			if (topBarBgStyle == null)
			{
				topBarBgStyle = GUI.skin.label;
			}

			var topBarLabelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.TOP_BAR_LABEL_STYLE_NAME);
			if (topBarLabelStyle == null)
			{
				topBarLabelStyle = GUI.skin.label;
			}

			var topBarPopupStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.FILE_FILTER_POPUP_STYLE_NAME);
			if (topBarPopupStyle == null)
			{
				topBarPopupStyle = GUI.skin.label;
			}

			GUILayout.Space(1); // top padding

			GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutHeight11);
			GUILayout.Label(" ", topBarBgStyle, BRT_BuildReportWindow.LayoutNone);

			GUILayout.Label("Bundle: ", topBarLabelStyle);
			assetBundleSession.SelectedBundleIdx = EditorGUILayout.Popup(
				assetBundleSession.SelectedBundleIdx, assetBundleSession.BundleNames,
				topBarPopupStyle);

			GUILayout.Space(20);

			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();

			GUILayout.Space(5);
		}

		void DrawTotalSize(CustomBuildReport.BuildInfo buildReportToDisplay, CustomBuildReport.AssetBundleSession assetBundleSession)
		{
			GUILayout.BeginVertical();

			var bigLabelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.INFO_TITLE_STYLE_NAME);
			if (bigLabelStyle == null)
			{
				bigLabelStyle = GUI.skin.label;
			}

			var descStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.TINY_HELP_STYLE_NAME);
			if (descStyle == null)
			{
				descStyle = GUI.skin.label;
			}

			var valueStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.BIG_NUMBER_STYLE_NAME);
			if (valueStyle == null)
			{
				valueStyle = GUI.skin.label;
			}

			if (buildReportToDisplay.HasOldSizeValues)
			{
				// in old sizes:
				// TotalBuildSize is really the used assets size
				// CompressedBuildSize if present is the total build size

				CustomBuildReport.Window.Utility.DrawLargeSizeDisplay(Labels.USED_TOTAL_SIZE_LABEL,
					Labels.USED_TOTAL_SIZE_DESC, buildReportToDisplay.TotalBuildSize);
				GUILayout.Space(40);
				CustomBuildReport.Window.Utility.DrawLargeSizeDisplay(Labels.BUILD_TOTAL_SIZE_LABEL,
					CustomBuildReport.Window.Utility.GetProperBuildSizeDesc(buildReportToDisplay),
					buildReportToDisplay.CompressedBuildSize);
			}
			else
			{
				// Total Build Size

				if (_hasTotalBuildSize || _hasAssetBundles)
				{
					GUILayout.BeginVertical();

					// ----------------------------------------------

					string buildSizeLabelToUse;
					if (_hasAssetBundles)
					{
						buildSizeLabelToUse = Labels.BUNDLE_TOTAL_SIZE_LABEL;
					}
					else if (_hasTotalBuildSize)
					{
						var buildPlatform =
							CustomBuildReport.ReportGenerator.GetBuildPlatformFromString(buildReportToDisplay.BuildType,
								buildReportToDisplay.BuildTargetUsed);
						if (buildPlatform == BuildPlatform.iOS)
						{
							buildSizeLabelToUse = Labels.BUILD_XCODE_SIZE_LABEL;
						}
						else
						{
							buildSizeLabelToUse = Labels.BUILD_TOTAL_SIZE_LABEL;
						}
					}
					else
					{
						buildSizeLabelToUse = null;
					}
					GUILayout.Label(buildSizeLabelToUse, bigLabelStyle);

					// ----------------------------------------------

					string buildSizeDesc;
					if (_hasAssetBundles)
					{
						buildSizeDesc = Labels.BUNDLE_TOTAL_SIZE_DESC;
					}
					else if (_hasTotalBuildSize)
					{
						buildSizeDesc = CustomBuildReport.Util.GetBuildSizePathDescription(buildReportToDisplay);
					}
					else
					{
						buildSizeDesc = null;
					}
					GUILayout.Label(buildSizeDesc, descStyle);

					// ----------------------------------------------

					if (_hasAssetBundles)
					{
						GUILayout.Label(buildReportToDisplay.AssetBundles[assetBundleSession.SelectedBundleIdx].TotalOutputSize, valueStyle);
					}
					else if (_hasTotalBuildSize)
					{
						GUILayout.Label(buildReportToDisplay.TotalBuildSize, valueStyle);
					}

					// ----------------------------------------------

					GUILayout.EndVertical();

					DrawAuxiliaryBuildSizes(buildReportToDisplay);
					GUILayout.Space(40);
				}


				// Used Assets
				if (_hasUsedAssetsTotalSize || _hasAssetBundles)
				{
					string totalAssetSizeToUse;
					if (_hasUsedAssetsTotalSize)
					{
						totalAssetSizeToUse = buildReportToDisplay.UsedTotalSize;
					}
					else
					{
						totalAssetSizeToUse = buildReportToDisplay.AssetBundles[assetBundleSession.SelectedBundleIdx].TotalUserAssetsSize;
					}

					string descToUse;
					if (_hasUsedAssetsTotalSize)
					{
						descToUse = Labels.USED_TOTAL_SIZE_DESC;
					}
					else
					{
						descToUse = Labels.BUNDLE_USED_TOTAL_SIZE_DESC;
					}
					CustomBuildReport.Window.Utility.DrawLargeSizeDisplay(Labels.USED_TOTAL_SIZE_LABEL,
						descToUse, totalAssetSizeToUse);
					GUILayout.Space(40);
				}


				// Unused Assets
				if (buildReportToDisplay.UnusedAssetsIncludedInCreation)
				{
					CustomBuildReport.Window.Utility.DrawLargeSizeDisplay(Labels.UNUSED_TOTAL_SIZE_LABEL,
						Labels.UNUSED_TOTAL_SIZE_DESC, buildReportToDisplay.UnusedTotalSize);

					if (buildReportToDisplay.ProcessUnusedAssetsInBatches)
					{
						GUILayout.Space(10);

						GUILayout.BeginHorizontal();
						var warning = GUI.skin.FindStyle("Icon-Warning");
						if (warning != null)
						{
							var warningIcon = warning.normal.background;

							var iconWidth = warning.fixedWidth;
							var iconHeight = warning.fixedHeight;

							GUI.DrawTexture(GUILayoutUtility.GetRect(iconWidth, iconHeight), warningIcon);
						}
						GUILayout.Label(string.Format(Labels.UNUSED_TOTAL_IS_FROM_BATCH, buildReportToDisplay.UnusedAssetsEntriesPerBatch), descStyle);
						GUILayout.EndHorizontal();
					}
				}
			}

			GUILayout.EndVertical();
		}


		void DrawAuxiliaryBuildSizes(CustomBuildReport.BuildInfo buildReportToDisplay)
		{
			CustomBuildReport.BuildPlatform buildPlatform =
				CustomBuildReport.ReportGenerator.GetBuildPlatformFromString(buildReportToDisplay.BuildType,
					buildReportToDisplay.BuildTargetUsed);

			var bigLabelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.INFO_TITLE_STYLE_NAME);
			if (bigLabelStyle == null)
			{
				bigLabelStyle = GUI.skin.label;
			}

			var medLabelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.INFO_SUBTITLE_BOLD_STYLE_NAME);
			if (medLabelStyle == null)
			{
				medLabelStyle = GUI.skin.label;
			}

			var valueStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.BIG_NUMBER_STYLE_NAME);
			if (valueStyle == null)
			{
				valueStyle = GUI.skin.label;
			}

			if (buildPlatform == CustomBuildReport.BuildPlatform.Web)
			{
				GUILayout.Space(20);
				GUILayout.BeginVertical();
				GUILayout.Label(Labels.WEB_UNITY3D_FILE_SIZE_LABEL, medLabelStyle);
				GUILayout.Label(buildReportToDisplay.WebFileBuildSize, valueStyle);
				GUILayout.EndVertical();
			}
			else if (buildPlatform == CustomBuildReport.BuildPlatform.Android)
			{
				if (!buildReportToDisplay.AndroidCreateProject && buildReportToDisplay.AndroidUseAPKExpansionFiles)
				{
					GUILayout.Space(20);
					GUILayout.BeginVertical();
					GUILayout.Label(Labels.ANDROID_APK_FILE_SIZE_LABEL, medLabelStyle);
					GUILayout.Label(buildReportToDisplay.AndroidApkFileBuildSize, bigLabelStyle);
					GUILayout.EndVertical();

					GUILayout.Space(20);
					GUILayout.BeginVertical();
					GUILayout.Label(Labels.ANDROID_OBB_FILE_SIZE_LABEL, medLabelStyle);
					GUILayout.Label(buildReportToDisplay.AndroidObbFileBuildSize, bigLabelStyle);
					GUILayout.EndVertical();
				}
				else if (buildReportToDisplay.AndroidCreateProject && buildReportToDisplay.AndroidUseAPKExpansionFiles)
				{
					GUILayout.Space(20);
					GUILayout.BeginVertical();
					GUILayout.Label(Labels.ANDROID_OBB_FILE_SIZE_LABEL, medLabelStyle);
					GUILayout.Label(buildReportToDisplay.AndroidObbFileBuildSize, bigLabelStyle);
					GUILayout.EndVertical();
				}
			}

			// Streaming Assets
			if (buildReportToDisplay.HasStreamingAssets)
			{
				GUILayout.Space(20);
				CustomBuildReport.Window.Utility.DrawLargeSizeDisplay(Labels.STREAMING_ASSETS_TOTAL_SIZE_LABEL,
					Labels.STREAMING_ASSETS_SIZE_DESC, buildReportToDisplay.StreamingAssetsSize);
			}
		}


		void DrawBuildSizes(CustomBuildReport.SizePart[] buildSizes)
		{
			if (_hasCompressedBuildSize)
			{
				GUILayout.BeginVertical();
			}

			var bigLabelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.INFO_TITLE_STYLE_NAME);
			if (bigLabelStyle == null)
			{
				bigLabelStyle = GUI.skin.label;
			}

			var medLabelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.INFO_SUBTITLE_BOLD_STYLE_NAME);
			if (medLabelStyle == null)
			{
				medLabelStyle = GUI.skin.label;
			}

			var labelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.INFO_SUBTITLE_STYLE_NAME);
			if (labelStyle == null)
			{
				labelStyle = GUI.skin.label;
			}

			GUILayout.Label(Labels.TOTAL_SIZE_BREAKDOWN_LABEL, bigLabelStyle);

			if (_hasCompressedBuildSize)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(Labels.TOTAL_SIZE_BREAKDOWN_MSG_PRE_BOLD, labelStyle);
				GUILayout.Label(Labels.TOTAL_SIZE_BREAKDOWN_MSG_BOLD, medLabelStyle);
				GUILayout.Label(Labels.TOTAL_SIZE_BREAKDOWN_MSG_POST_BOLD, labelStyle);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.EndVertical();
			}

			if (buildSizes != null)
			{
				GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutMaxWidth500);

				DrawNames(buildSizes);
				DrawReadableSizes(buildSizes);
				DrawPercentages(buildSizes);

				GUILayout.EndHorizontal();
			}
		}

		void DrawDLLList(CustomBuildReport.BuildInfo buildReportToDisplay)
		{
			CustomBuildReport.BuildPlatform buildPlatform =
				CustomBuildReport.ReportGenerator.GetBuildPlatformFromString(buildReportToDisplay.BuildType,
					buildReportToDisplay.BuildTargetUsed);

			var bigLabelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.INFO_TITLE_STYLE_NAME);
			if (bigLabelStyle == null)
			{
				bigLabelStyle = GUI.skin.label;
			}

			GUILayout.BeginHorizontal();

			// column 1
			GUILayout.BeginVertical();
			if (_hasMonoDLLsToDisplay)
			{
				GUILayout.Label(Labels.MONO_DLLS_LABEL, bigLabelStyle);
				{
					GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutMaxWidth500);
					DrawNames(buildReportToDisplay.MonoDLLs);
					DrawReadableSizes(buildReportToDisplay.MonoDLLs);
					GUILayout.EndHorizontal();
				}

				GUILayout.Space(20);
			}

			if (_hasUnityEngineDLLsToDisplay)
			{
				DrawScriptDLLsList(buildReportToDisplay, buildPlatform);
			}

			GUILayout.EndVertical();

			GUILayout.Space(15);

			// column 2
			GUILayout.BeginVertical();
			if (_hasUnityEngineDLLsToDisplay)
			{
				GUILayout.Label(Labels.UNITY_ENGINE_DLLS_LABEL, bigLabelStyle);
				{
					GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutMaxWidth500);
					DrawNames(buildReportToDisplay.UnityEngineDLLs);
					DrawReadableSizes(buildReportToDisplay.UnityEngineDLLs);
					GUILayout.EndHorizontal();
				}
			}
			else
			{
				DrawScriptDLLsList(buildReportToDisplay, buildPlatform);
			}

			GUILayout.Space(20);
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();
		}

		void DrawScriptDLLsList(CustomBuildReport.BuildInfo buildReportToDisplay,
			CustomBuildReport.BuildPlatform buildPlatform)
		{
			if (!_hasScriptDLLsToDisplay)
			{
				return;
			}

			var bigLabelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.INFO_TITLE_STYLE_NAME);
			if (bigLabelStyle == null)
			{
				bigLabelStyle = GUI.skin.label;
			}

			GUILayout.Label(Labels.SCRIPT_DLLS_LABEL, bigLabelStyle);
			{
				GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutMaxWidth500);
				DrawNames(buildReportToDisplay.ScriptDLLs);
				DrawReadableSizes(buildReportToDisplay.ScriptDLLs);
				GUILayout.EndHorizontal();
			}
		}


		void DrawNames(CustomBuildReport.SizePart[] list)
		{
			if (list == null)
			{
				return;
			}

			var listNormalStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_NORMAL_STYLE_NAME);
			if (listNormalStyle == null)
			{
				listNormalStyle = GUI.skin.label;
			}

			var listAltStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_NORMAL_ALT_STYLE_NAME);
			if (listAltStyle == null)
			{
				listAltStyle = GUI.skin.label;
			}

			GUILayout.BeginVertical();
			bool useAlt = false;
			foreach (CustomBuildReport.SizePart b in list)
			{
				if (b.IsTotal) continue;
				var styleToUse = useAlt ? listAltStyle : listNormalStyle;
				GUILayout.Label(b.Name, styleToUse);
				useAlt = !useAlt;
			}

			GUILayout.EndVertical();
		}

		void DrawReadableSizes(CustomBuildReport.SizePart[] list)
		{
			if (list == null)
			{
				return;
			}

			var listNormalStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_NORMAL_STYLE_NAME);
			if (listNormalStyle == null)
			{
				listNormalStyle = GUI.skin.label;
			}

			var listAltStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_NORMAL_ALT_STYLE_NAME);
			if (listAltStyle == null)
			{
				listAltStyle = GUI.skin.label;
			}

			GUILayout.BeginVertical();
			bool useAlt = false;
			foreach (CustomBuildReport.SizePart b in list)
			{
				if (b.IsTotal) continue;
				var styleToUse = useAlt ? listAltStyle : listNormalStyle;
				GUILayout.Label(b.Size, styleToUse);
				useAlt = !useAlt;
			}

			GUILayout.EndVertical();
		}

		void DrawPercentages(CustomBuildReport.SizePart[] list)
		{
			if (list == null)
			{
				return;
			}

			var listNormalStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_NORMAL_STYLE_NAME);
			if (listNormalStyle == null)
			{
				listNormalStyle = GUI.skin.label;
			}

			var listAltStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_NORMAL_ALT_STYLE_NAME);
			if (listAltStyle == null)
			{
				listAltStyle = GUI.skin.label;
			}

			GUILayout.BeginVertical();
			bool useAlt = false;
			foreach (CustomBuildReport.SizePart b in list)
			{
				if (b.IsTotal) continue;
				var styleToUse = useAlt ? listAltStyle : listNormalStyle;
				GUILayout.Label(string.Format("{0}%", b.Percentage.ToString(CultureInfo.InvariantCulture)), styleToUse);
				useAlt = !useAlt;
			}

			GUILayout.EndVertical();
		}
	}
}