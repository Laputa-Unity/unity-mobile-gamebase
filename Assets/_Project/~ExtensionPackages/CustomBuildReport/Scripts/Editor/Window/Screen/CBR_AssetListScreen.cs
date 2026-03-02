//#define BRT_ASSET_LIST_SCREEN_DEBUG

using System.Globalization;
using UnityEngine;
using UnityEditor;


namespace CustomBuildReport.Window.Screen
{
	public partial class AssetList : BaseScreen
	{
#if BRT_ASSET_LIST_SCREEN_DEBUG
		bool _showDebugText;
		readonly GUIContent _debugLabel = new GUIContent();
		System.Text.StringBuilder _debugText;
#endif

		const int SCROLLBAR_BOTTOM_PADDING = 5;
		const int BOTTOM_STATUS_BAR_HEIGHT = 20;
		const int DOUBLE_CLICK_THRESHOLD = 2;

		CustomBuildReport.FileFilterGroup _configuredFileFilterGroup;

		/// <summary>
		/// Which field of the asset we are sorting the list in.
		/// </summary>
		CustomBuildReport.AssetList.SortType _currentSortType = CustomBuildReport.AssetList.SortType.RawSize;

		CustomBuildReport.TextureData.DataId _currentTextureDataSortType = CustomBuildReport.TextureData.DataId.None;

		CustomBuildReport.TextureData.DataId _hoveredTextureDataId = CustomBuildReport.TextureData.DataId.None;
		CustomBuildReport.TextureData.DataId _clickedTextureDataId = CustomBuildReport.TextureData.DataId.None;

		string _overridenTextureDataTooltipText;

		CustomBuildReport.MeshData.DataId _currentMeshDataSortType = CustomBuildReport.MeshData.DataId.None;

		CustomBuildReport.MeshData.DataId _hoveredMeshDataId = CustomBuildReport.MeshData.DataId.None;
		CustomBuildReport.MeshData.DataId _clickedMeshDataId = CustomBuildReport.MeshData.DataId.None;

		CustomBuildReport.PrefabData.DataId _currentPrefabDataSortType = CustomBuildReport.PrefabData.DataId.None;

		CustomBuildReport.PrefabData.DataId _hoveredPrefabDataId = CustomBuildReport.PrefabData.DataId.None;
		CustomBuildReport.PrefabData.DataId _clickedPrefabDataId = CustomBuildReport.PrefabData.DataId.None;

		/// <summary>
		/// Whether we are sorting ascending or descending.
		/// </summary>
		CustomBuildReport.AssetList.SortOrder _currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending;

		/// <summary>
		/// Last clicked entry in the main asset list.
		/// This is used to check if user is clicking on the same asset that's already selected, or a new one.
		/// </summary>
		int _assetListEntryLastClickedIdx = -1;

		/// <summary>
		/// -1 is already used to signify the "All" list (i.e. no filter)
		/// so we need something else to signify no value.
		/// </summary>
		const int NO_FILTER_VALUE = -2;

		int _filterIdxOfLastClickedAssetListEntry = NO_FILTER_VALUE;

		/// <summary>
		/// Hovered entry in the main asset list
		/// </summary>
		int _assetListEntryHoveredIdx = -1;

		/// <summary>
		/// Hovered entry in the asset usage ancestry list
		/// </summary>
		int _assetUsageAncestryHoveredIdx = -1;

		/// <summary>
		/// Hovered entry in the flattened asset users list
		/// </summary>
		int _assetUserEntryHoveredIdx = -1;

		bool _shouldShowThumbnailOnHoveredAsset;


		/// <summary>
		/// We record the time an asset has been clicked
		/// to check for a double-click.
		/// We use this instead of Event.current.clickCount
		/// because this way is easier.
		/// </summary>
		double _assetListEntryLastClickedTime;

		Vector2 _assetListScrollPos;
		Rect _assetPathColumnHeaderRect;
		readonly GUIContent _assetPathCheckboxLabel = new GUIContent("", "Display Asset's Path or just the filename.");

		readonly GUIContent _textureDataTooltipLabel = new GUIContent();

		/// <summary>
		/// Re-used entry in the main asset list.
		/// </summary>
		readonly GUIContent _assetListEntry = new GUIContent();


		public enum ListToDisplay
		{
			Invalid,
			UsedAssets,
			UnusedAssets,
		}

		ListToDisplay _currentListDisplayed = ListToDisplay.Invalid;

		bool _mouseIsPreviouslyInWindow;

		bool _mouseIsOnOverlayControl;

		Rect _showColumnOptionButtonRect;
		bool _showColumnOptions;
		readonly GUIContent _showColumnLabel = new GUIContent("Columns");
		readonly GUIContent _columnLabel = new GUIContent();

		bool _showPageNumbers = true;

		// =================================================================================

		public override string Name
		{
			get { return ""; }
		}

		public override void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies,
			TextureData textureData, MeshData meshData, PrefabData prefabData, UnityBuildReport unityBuildReport, CustomBuildReport.AssetBundleSession assetBundleSession)
		{
			RefreshConfiguredFileFilters();

			if (CustomBuildReport.Options.ShouldUseConfiguredFileFilters())
			{
				CustomBuildReport.AssetList listToDisplay = GetAssetListToDisplay(buildReport, assetBundleSession);
				if (listToDisplay != null)
				{
					listToDisplay.SortIfNeeded(_configuredFileFilterGroup);
				}
			}

			_currentSortType = CustomBuildReport.AssetList.SortType.RawSize;
			_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending;
		}

		public override void Update(double timeNow, double deltaTime, BuildInfo buildReportToDisplay, CustomBuildReport.AssetBundleSession assetBundleSession,
			AssetDependencies assetDependencies)
		{
			UpdateSearch(timeNow, buildReportToDisplay, assetBundleSession);
		}

		public void SetListToDisplay(ListToDisplay t)
		{
			_currentListDisplayed = t;
		}

		bool IsShowingUnusedAssets
		{
			get { return _currentListDisplayed == ListToDisplay.UnusedAssets; }
		}

		bool IsShowingUsedAssets
		{
			get { return _currentListDisplayed == ListToDisplay.UsedAssets; }
		}

		CustomBuildReport.AssetList GetAssetListToDisplay(BuildInfo buildReportToDisplay, CustomBuildReport.AssetBundleSession assetBundleSession)
		{
			if (buildReportToDisplay == null)
			{
				return null;
			}

			if (_currentListDisplayed == ListToDisplay.UsedAssets)
			{
				if (buildReportToDisplay.HasAssetBundles)
				{
					return buildReportToDisplay.AssetBundles[assetBundleSession.SelectedBundleIdx].UsedAssets;
				}

				return buildReportToDisplay.UsedAssets;
			}
			else if (_currentListDisplayed == ListToDisplay.UnusedAssets)
			{
				return buildReportToDisplay.UnusedAssets;
			}

			Debug.LogError("Invalid list to display type");
			return null;
		}

		void DrawUnderlay(CustomBuildReport.BuildInfo buildReportToDisplay, CustomBuildReport.AssetBundleSession assetBundleSession)
		{
			DrawOverlay(buildReportToDisplay, assetBundleSession, false);
		}

		Vector2 _columnOptionsScrollPos;

		void DrawOverlay(CustomBuildReport.BuildInfo buildReportToDisplay, CustomBuildReport.AssetBundleSession assetBundleSession, bool isOverlay = true)
		{
			const int TOGGLE_EXTRA_WIDTH = 9;
			const int TOGGLE_SPACING = 2;

			var prevEnabled = GUI.enabled;

			if (_showColumnOptions)
			{
				Rect columnOptionsBg = new Rect();
				columnOptionsBg.x = _showColumnOptionButtonRect.x;
				columnOptionsBg.y = _showColumnOptionButtonRect.yMax;
				columnOptionsBg.width = UnityEngine.Screen.width - _showColumnOptionButtonRect.x - 19;
				columnOptionsBg.height = 550;

				columnOptionsBg.x = UnityEngine.Screen.width - columnOptionsBg.width - 19;

				float availableHeight = UnityEngine.Screen.height - columnOptionsBg.y - 20;
				bool heightRestricted = availableHeight < columnOptionsBg.height;

				var bgStyle = GUI.skin.FindStyle(isOverlay ? "PopupPanel" : "HiddenScrollbar");
				if (bgStyle == null)
				{
					bgStyle = GUI.skin.box;
				}

				if (heightRestricted)
				{
					Rect scrollViewRect = new Rect(columnOptionsBg);
					scrollViewRect.height = availableHeight - 7;

					var hiddenHorizontalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
					if (hiddenHorizontalScrollbarStyle == null)
					{
						hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;
					}

					Rect bgRect = new Rect(scrollViewRect);
					bgRect.height = availableHeight - 5;
					GUI.Box(bgRect, GUIContent.none, bgStyle);
					_columnOptionsScrollPos = GUI.BeginScrollView(scrollViewRect, _columnOptionsScrollPos, columnOptionsBg, hiddenHorizontalScrollbarStyle, GUI.skin.verticalScrollbar);
				}
				else
				{
					GUI.Box(columnOptionsBg, GUIContent.none, bgStyle);
				}

				_mouseIsOnOverlayControl = columnOptionsBg.Contains(Event.current.mousePosition);
				if (_mouseIsOnOverlayControl)
				{
					_hoveredTextureDataId = TextureData.DataId.None;
					_hoveredMeshDataId = MeshData.DataId.None;
					_hoveredPrefabDataId = PrefabData.DataId.None;
					_overridenTextureDataTooltipText = null;
				}

				Rect rect = new Rect();
				rect.x = columnOptionsBg.x + 10;
				rect.y = columnOptionsBg.y + 5;

				float startX = rect.x;

				//rect.width = 300;
				//rect.height = 28;
				//GUI.Label(rect, string.Format("Screen.width: {0} Screen.height: {1}", UnityEngine.Screen.width.ToString(), UnityEngine.Screen.height.ToString()));

				const int COLUMN_SPACING = 15;
				const int GROUP_SPACING = 5;

				float normalColumnsHeight = 0;

				float textureDataLowestY = 0;
				float textureDataColumn1Width = 0;
				float textureDataColumn2Width = 0;

				var toggleHeaderStyle = GUI.skin.FindStyle("Header3");
				if (toggleHeaderStyle == null)
				{
					toggleHeaderStyle = GUI.skin.label;
				}

				_columnLabel.text = "Columns:";
				rect.size = toggleHeaderStyle.CalcSize(_columnLabel);
				GUI.Label(rect, _columnLabel, toggleHeaderStyle);
				rect.y += rect.height - 2;

				_columnLabel.text = "Asset Path";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowColumnAssetPath = GUI.Toggle(rect, CustomBuildReport.Options.ShowColumnAssetPath, _columnLabel);
				rect.x += rect.width + COLUMN_SPACING;
				normalColumnsHeight = Mathf.Max(normalColumnsHeight, rect.height);

				_columnLabel.text = "Size Before Build";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowColumnSizeBeforeBuild = GUI.Toggle(rect, CustomBuildReport.Options.ShowColumnSizeBeforeBuild, _columnLabel);
				rect.x += rect.width + COLUMN_SPACING;
				normalColumnsHeight = Mathf.Max(normalColumnsHeight, rect.height);

				_columnLabel.text = "Size In Build";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowColumnSizeInBuild = GUI.Toggle(rect, CustomBuildReport.Options.ShowColumnSizeInBuild, _columnLabel);
				rect.x += rect.width + COLUMN_SPACING;
				normalColumnsHeight = Mathf.Max(normalColumnsHeight, rect.height);

				rect.x = startX;
				rect.y += normalColumnsHeight + TOGGLE_SPACING;

				normalColumnsHeight = 0;
				_columnLabel.text = "Raw Size (for Unused Assets)";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowColumnUnusedRawSize = GUI.Toggle(rect, CustomBuildReport.Options.ShowColumnUnusedRawSize, _columnLabel);
				rect.x += rect.width + COLUMN_SPACING;
				normalColumnsHeight = Mathf.Max(normalColumnsHeight, rect.height);

				_columnLabel.text = "Imported Size (for Unused Assets)";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowColumnUnusedImportedSize = GUI.Toggle(rect, CustomBuildReport.Options.ShowColumnUnusedImportedSize, _columnLabel);
				rect.x += rect.width + COLUMN_SPACING;
				normalColumnsHeight = Mathf.Max(normalColumnsHeight, rect.height);

				rect.y += normalColumnsHeight + 8;

				// -----------------------------------------------------------------------

				// column 1
				rect.x = startX;

				_columnLabel.text = "Texture Data Columns:";
				rect.size = toggleHeaderStyle.CalcSize(_columnLabel);
				GUI.Label(rect, _columnLabel, toggleHeaderStyle);
				rect.y += rect.height - 2;
				float toggleYStart = rect.y;

				#region Texture Column 1

				_columnLabel.text = "Texture Type";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnTextureType = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnTextureType, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.TextureType;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Is sRGB (Color Texture)";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnIsSRGB = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnIsSRGB, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.IsSRGB;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Alpha Source";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnAlphaSource = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnAlphaSource, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.AlphaSource;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Alpha Is Transparency";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnAlphaIsTransparency = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnAlphaIsTransparency, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.AlphaIsTransparency;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Ignore PNG Gamma";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnIgnorePngGamma = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnIgnorePngGamma, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.IgnorePngGamma;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Read/Write Enabled";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnIsReadable = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnIsReadable, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.IsReadable;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				// -------------------------------------------
				rect.y += GROUP_SPACING;

				_columnLabel.text = "Mip Map Generated";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnMipMapGenerated = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnMipMapGenerated, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.MipMapGenerated;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Mip Map Filter";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnMipMapFilter = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnMipMapFilter, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.MipMapFilter;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Streaming Mip Maps";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnStreamingMipMaps = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnStreamingMipMaps, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.StreamingMipMaps;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Border Mip Maps";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnBorderMipMaps = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnBorderMipMaps, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.BorderMipMaps;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Preserve Coverage Mip Maps";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnPreserveCoverageMipMaps = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnPreserveCoverageMipMaps, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.PreserveCoverageMipMaps;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);

				_columnLabel.text = "Fade Mip Maps";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnFadeOutMipMaps = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnFadeOutMipMaps, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.FadeOutMipMaps;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn1Width = Mathf.Max(textureDataColumn1Width, rect.width);
				textureDataLowestY = Mathf.Max(textureDataLowestY, rect.y);

				#endregion

				// -----------------------------------------------------------------------
				// column 2
				rect.x += textureDataColumn1Width + COLUMN_SPACING;
				rect.y = toggleYStart;

				#region Texture Column 2

				_columnLabel.text = "Imported Width & Height";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnImportedWidthAndHeight = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnImportedWidthAndHeight, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.ImportedWidthAndHeight;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn2Width = Mathf.Max(textureDataColumn2Width, rect.width);

				_columnLabel.text = "Source Width & Height";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnRealWidthAndHeight = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnRealWidthAndHeight, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.RealWidthAndHeight;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn2Width = Mathf.Max(textureDataColumn2Width, rect.width);

				_columnLabel.text = "Non-Power of 2 Scale";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnNPotScale = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnNPotScale, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.NPotScale;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn2Width = Mathf.Max(textureDataColumn2Width, rect.width);

				_columnLabel.text = "Max Texture Size";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnMaxTextureSize = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnMaxTextureSize, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.MaxTextureSize;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn2Width = Mathf.Max(textureDataColumn2Width, rect.width);

				_columnLabel.text = "Resize Algorithm";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnResizeAlgorithm = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnResizeAlgorithm, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.TextureResizeAlgorithm;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn2Width = Mathf.Max(textureDataColumn2Width, rect.width);

				// -----------------------------------------------------------------------
				rect.y += GROUP_SPACING;

				_columnLabel.text = "Sprite Mode";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnSpriteImportMode = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnSpriteImportMode, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.SpriteImportMode;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn2Width = Mathf.Max(textureDataColumn2Width, rect.width);

				_columnLabel.text = "Sprite Packing Tag";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnSpritePackingTag = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnSpritePackingTag, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.SpritePackingTag;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn2Width = Mathf.Max(textureDataColumn2Width, rect.width);

				_columnLabel.text = "Sprite Pixels Per Unit";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnSpritePixelsPerUnit = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnSpritePixelsPerUnit, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.SpritePixelsPerUnit;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn2Width = Mathf.Max(textureDataColumn2Width, rect.width);

				_columnLabel.text = "Qualifies for Sprite Packing";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnQualifiesForSpritePacking = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnQualifiesForSpritePacking, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.QualifiesForSpritePacking;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				textureDataColumn2Width = Mathf.Max(textureDataColumn2Width, rect.width);
				textureDataLowestY = Mathf.Max(textureDataLowestY, rect.y);

				#endregion

				// -----------------------------------------------------------------------
				// column 3
				rect.x += textureDataColumn2Width + COLUMN_SPACING;
				rect.y = toggleYStart;

				#region Texture Column 3

				_columnLabel.text = "Texture Format";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnTextureFormat = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnTextureFormat, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.TextureFormat;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				_columnLabel.text = "Compression Type";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnCompressionType = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnCompressionType, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.CompressionType;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				_columnLabel.text = "Compression Crunched";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnCompressionIsCrunched = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnCompressionIsCrunched, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.CompressionIsCrunched;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				_columnLabel.text = "Compression Quality";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnCompressionQuality = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnCompressionQuality, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.CompressionQuality;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				// -----------------------------------------------------------------------
				rect.y += GROUP_SPACING;

				_columnLabel.text = "Wrap Mode";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnWrapMode = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnWrapMode, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.WrapMode;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				_columnLabel.text = "Wrap Mode U";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnWrapModeU = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnWrapModeU, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.WrapModeU;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				_columnLabel.text = "Wrap Mode V";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnWrapModeV = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnWrapModeV, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.WrapModeV;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				_columnLabel.text = "Wrap Mode W";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnWrapModeW = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnWrapModeW, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.WrapModeW;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				_columnLabel.text = "Filter Mode";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnFilterMode = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnFilterMode, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.FilterMode;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				_columnLabel.text = "Anisotropic Filtering Level";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowTextureColumnAnisoLevel = GUI.Toggle(rect, CustomBuildReport.Options.ShowTextureColumnAnisoLevel, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = TextureData.DataId.AnisoLevel;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				textureDataLowestY = Mathf.Max(textureDataLowestY, rect.y);

				#endregion

				// -----------------------------------------------------------------------

				float meshColumn1Width = 0;
				float meshColumn2Width = 0;
				float meshDataLowestY = 0;

				// column 1
				rect.x = startX;
				rect.y = textureDataLowestY + 8;

				_columnLabel.text = "Mesh Data Columns:";
				rect.size = toggleHeaderStyle.CalcSize(_columnLabel);
				GUI.Label(rect, _columnLabel, toggleHeaderStyle);
				rect.y += rect.height - 2;
				toggleYStart = rect.y;

				#region Mesh Column 1

				_columnLabel.text = "Non-Skinned Mesh Count";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowMeshColumnMeshFilterCount = GUI.Toggle(rect, CustomBuildReport.Options.ShowMeshColumnMeshFilterCount, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredMeshDataId = MeshData.DataId.MeshFilterCount;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				meshColumn1Width = Mathf.Max(meshColumn1Width, rect.width);

				_columnLabel.text = "Skinned Mesh Count";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowMeshColumnSkinnedMeshRendererCount = GUI.Toggle(rect, CustomBuildReport.Options.ShowMeshColumnSkinnedMeshRendererCount, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredMeshDataId = MeshData.DataId.SkinnedMeshRendererCount;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				meshColumn1Width = Mathf.Max(meshColumn1Width, rect.width);

				meshDataLowestY = Mathf.Max(meshDataLowestY, rect.y);

				#endregion

				// -----------------------------------------------------------------------
				// column 2

				rect.x += meshColumn1Width + COLUMN_SPACING;
				rect.y = toggleYStart;

				#region Mesh Column 2

				_columnLabel.text = "Sub-Mesh Count";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowMeshColumnSubMeshCount = GUI.Toggle(rect, CustomBuildReport.Options.ShowMeshColumnSubMeshCount, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredMeshDataId = MeshData.DataId.SubMeshCount;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				meshColumn2Width = Mathf.Max(meshColumn2Width, rect.width);

				_columnLabel.text = "Vertex Count";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowMeshColumnVertexCount = GUI.Toggle(rect, CustomBuildReport.Options.ShowMeshColumnVertexCount, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredMeshDataId = MeshData.DataId.VertexCount;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				meshColumn2Width = Mathf.Max(meshColumn2Width, rect.width);

				_columnLabel.text = "Face Count";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowMeshColumnTriangleCount = GUI.Toggle(rect, CustomBuildReport.Options.ShowMeshColumnTriangleCount, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredMeshDataId = MeshData.DataId.TriangleCount;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				meshColumn2Width = Mathf.Max(meshColumn2Width, rect.width);

				meshDataLowestY = Mathf.Max(meshDataLowestY, rect.y);

				#endregion

				// -----------------------------------------------------------------------
				// column 3

				rect.x += meshColumn2Width + COLUMN_SPACING;
				rect.y = toggleYStart;

				#region Mesh Column 3

				_columnLabel.text = "Animation Type";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowMeshColumnAnimationType = GUI.Toggle(rect, CustomBuildReport.Options.ShowMeshColumnAnimationType, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredMeshDataId = MeshData.DataId.AnimationType;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				_columnLabel.text = "Animation Clip Count";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowMeshColumnAnimationClipCount = GUI.Toggle(rect, CustomBuildReport.Options.ShowMeshColumnAnimationClipCount, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredMeshDataId = MeshData.DataId.AnimationClipCount;
				}
				rect.y += rect.height + TOGGLE_SPACING;

				meshDataLowestY = Mathf.Max(meshDataLowestY, rect.y);

				#endregion

				// -----------------------------------------------------------------------

				float prefabColumn1Width = 0;
				float prefabColumn2Width = 0;
				float prefabLowestY = 0;

				// column 1
				rect.x = startX;
				rect.y = meshDataLowestY + 8;

				_columnLabel.text = "Prefab Data Columns:";
				rect.size = toggleHeaderStyle.CalcSize(_columnLabel);
				GUI.Label(rect, _columnLabel, toggleHeaderStyle);
				rect.y += rect.height - 2;
				toggleYStart = rect.y;

				#region Prefab Column 1

				_columnLabel.text = "Contribute Global Illumination";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowPrefabColumnContributeGI = GUI.Toggle(rect, CustomBuildReport.Options.ShowPrefabColumnContributeGI, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredPrefabDataId = PrefabData.DataId.ContributeGI;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				prefabColumn1Width = Mathf.Max(prefabColumn1Width, rect.width);

				_columnLabel.text = "Static Batching";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowPrefabColumnBatchingStatic = GUI.Toggle(rect, CustomBuildReport.Options.ShowPrefabColumnBatchingStatic, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredPrefabDataId = PrefabData.DataId.BatchingStatic;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				prefabColumn1Width = Mathf.Max(prefabColumn1Width, rect.width);

				_columnLabel.text = "Reflection Probe Static";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowPrefabColumnReflectionProbeStatic = GUI.Toggle(rect, CustomBuildReport.Options.ShowPrefabColumnReflectionProbeStatic, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredPrefabDataId = PrefabData.DataId.ReflectionProbeStatic;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				prefabColumn1Width = Mathf.Max(prefabColumn1Width, rect.width);

				prefabLowestY = Mathf.Max(prefabLowestY, rect.y);

				#endregion

				// -----------------------------------------------------------------------
				// column 2

				rect.x += prefabColumn1Width + COLUMN_SPACING;
				rect.y = toggleYStart;

				#region Prefab Column 2

				_columnLabel.text = "Occluder Static";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowPrefabColumnOccluderStatic = GUI.Toggle(rect, CustomBuildReport.Options.ShowPrefabColumnOccluderStatic, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredPrefabDataId = PrefabData.DataId.OccluderStatic;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				prefabColumn2Width = Mathf.Max(prefabColumn2Width, rect.width);

				_columnLabel.text = "Occludee Static";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowPrefabColumnOccludeeStatic = GUI.Toggle(rect, CustomBuildReport.Options.ShowPrefabColumnOccludeeStatic, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredPrefabDataId = PrefabData.DataId.OccludeeStatic;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				prefabColumn2Width = Mathf.Max(prefabColumn2Width, rect.width);

				prefabLowestY = Mathf.Max(prefabLowestY, rect.y);

				#endregion

				// -----------------------------------------------------------------------
				// column 3

				rect.x += prefabColumn2Width + COLUMN_SPACING;
				rect.y = toggleYStart;

				#region Prefab Column 3

				_columnLabel.text = "Navigation Static";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowPrefabColumnNavigationStatic = GUI.Toggle(rect, CustomBuildReport.Options.ShowPrefabColumnNavigationStatic, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredPrefabDataId = PrefabData.DataId.NavigationStatic;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				prefabColumn2Width = Mathf.Max(prefabColumn2Width, rect.width);

				_columnLabel.text = "Off-Mesh Link Generation";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				CustomBuildReport.Options.ShowPrefabColumnOffMeshLinkGeneration = GUI.Toggle(rect, CustomBuildReport.Options.ShowPrefabColumnOffMeshLinkGeneration, _columnLabel);
				if (rect.Contains(Event.current.mousePosition))
				{
					_hoveredPrefabDataId = PrefabData.DataId.OffMeshLinkGeneration;
				}
				rect.y += rect.height + TOGGLE_SPACING;
				prefabColumn2Width = Mathf.Max(prefabColumn2Width, rect.width);

				prefabLowestY = Mathf.Max(prefabLowestY, rect.y);

				#endregion

				// -----------------------------------------------------------------------

				// input catcher
				GUI.Button(columnOptionsBg, GUIContent.none, "HiddenScrollbar");

				if (heightRestricted)
				{
					GUI.EndScrollView();
				}
			}

			if (_showSearchOptions)
			{
				Rect searchOptionsBg = new Rect();
				searchOptionsBg.x = _searchTextfieldRect.x + 8;
				searchOptionsBg.y = _searchTextfieldRect.yMax;
				searchOptionsBg.width = _searchTextfieldRect.width - 11;
				searchOptionsBg.height = 130;

				_mouseIsOnOverlayControl = searchOptionsBg.Contains(Event.current.mousePosition);
				if (_mouseIsOnOverlayControl)
				{
					_hoveredTextureDataId = TextureData.DataId.None;
					_hoveredMeshDataId = MeshData.DataId.None;
					_hoveredPrefabDataId = PrefabData.DataId.None;
					_overridenTextureDataTooltipText = null;
				}

				var bgStyle = GUI.skin.FindStyle(isOverlay ? "PopupPanel" : "HiddenScrollbar");
				if (bgStyle == null)
				{
					bgStyle = GUI.skin.box;
				}
				GUI.Box(searchOptionsBg, GUIContent.none, bgStyle);


				Rect rect = new Rect();
				rect.x = searchOptionsBg.x + 10;
				rect.y = searchOptionsBg.y + 5;

				float startX = rect.x;

				var toggleHeaderStyle = GUI.skin.FindStyle("Header3");
				var radioLeftStyle = GUI.skin.FindStyle("RadioLeft");
				var radioMidStyle = GUI.skin.FindStyle("RadioMiddle");
				var radioRightStyle = GUI.skin.FindStyle("RadioRight");

				if (toggleHeaderStyle == null)
				{
					toggleHeaderStyle = GUI.skin.label;
				}
				if (radioLeftStyle == null)
				{
					radioLeftStyle = GUI.skin.toggle;
				}
				if (radioMidStyle == null)
				{
					radioMidStyle = GUI.skin.toggle;
				}
				if (radioRightStyle == null)
				{
					radioRightStyle = GUI.skin.toggle;
				}

				float searchTypeHeight = 0;

				_columnLabel.text = "Search Method:";
				rect.size = toggleHeaderStyle.CalcSize(_columnLabel);
				GUI.Label(rect, _columnLabel, toggleHeaderStyle);
				rect.y += rect.height + 5;

				_columnLabel.text = "Basic";
				rect.size = radioLeftStyle.CalcSize(_columnLabel);
				var pressedBasic = GUI.Toggle(rect, CustomBuildReport.Options.SearchTypeIsBasic, _columnLabel, radioLeftStyle);
				if (rect.Contains(Event.current.mousePosition))
				{
					_overridenTextureDataTooltipText = "<b><color=white>Basic</color></b>\n\nUse * for wildcard.";
				}
				rect.x += rect.width;
				searchTypeHeight = Mathf.Max(searchTypeHeight, rect.height);

				if (pressedBasic && !CustomBuildReport.Options.SearchTypeIsBasic)
				{
					CustomBuildReport.Options.SearchType = SearchType.Basic;
					UpdateSearchNow(buildReportToDisplay, assetBundleSession);
				}

				_columnLabel.text = "Regex";
				rect.size = radioMidStyle.CalcSize(_columnLabel);
				var pressedRegex = GUI.Toggle(rect, CustomBuildReport.Options.SearchTypeIsRegex, _columnLabel, radioMidStyle);
				if (rect.Contains(Event.current.mousePosition))
				{
					_overridenTextureDataTooltipText = "<b><color=white>Regular Expression</color></b>";
				}
				rect.x += rect.width;
				searchTypeHeight = Mathf.Max(searchTypeHeight, rect.height);

				if (pressedRegex && !CustomBuildReport.Options.SearchTypeIsRegex)
				{
					CustomBuildReport.Options.SearchType = SearchType.Regex;
					UpdateSearchNow(buildReportToDisplay, assetBundleSession);
				}


				_columnLabel.text = "Fuzzy";
				rect.size = radioRightStyle.CalcSize(_columnLabel);
				var pressedFuzzy = GUI.Toggle(rect, CustomBuildReport.Options.SearchTypeIsFuzzy, _columnLabel, radioRightStyle);
				if (rect.Contains(Event.current.mousePosition))
				{
					_overridenTextureDataTooltipText = "<b><color=white>Fuzzy Search</color></b>\n\nApproximate string matching.";
				}
				searchTypeHeight = Mathf.Max(searchTypeHeight, rect.height);

				if (pressedFuzzy && !CustomBuildReport.Options.SearchTypeIsFuzzy)
				{
					CustomBuildReport.Options.SearchType = SearchType.Fuzzy;
					UpdateSearchNow(buildReportToDisplay, assetBundleSession);
				}

				rect.x = startX;
				rect.y += searchTypeHeight + 20;

				_columnLabel.text = "Search through filenames only\n(ignore path when searching)";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				rect.height += 3;
				var newSearchFilenameOnly = GUI.Toggle(rect, CustomBuildReport.Options.SearchFilenameOnly, _columnLabel);
				if (newSearchFilenameOnly != CustomBuildReport.Options.SearchFilenameOnly)
				{
					CustomBuildReport.Options.SearchFilenameOnly = newSearchFilenameOnly;
					UpdateSearchNow(buildReportToDisplay, assetBundleSession);
				}
				rect.y += rect.height + TOGGLE_SPACING;

				var usingFuzzy = CustomBuildReport.Options.SearchTypeIsFuzzy;
				GUI.enabled = prevEnabled && !usingFuzzy;
				_columnLabel.text = "Case Sensitive Search";
				rect.size = GUI.skin.toggle.CalcSize(_columnLabel);
				rect.width += TOGGLE_EXTRA_WIDTH;
				var newSearchCaseSensitive = GUI.Toggle(rect, CustomBuildReport.Options.SearchCaseSensitive, _columnLabel);
				if (newSearchCaseSensitive != CustomBuildReport.Options.SearchCaseSensitive)
				{
					CustomBuildReport.Options.SearchCaseSensitive = newSearchCaseSensitive;
					UpdateSearchNow(buildReportToDisplay, assetBundleSession);
				}
				if (usingFuzzy && rect.Contains(Event.current.mousePosition))
				{
					_overridenTextureDataTooltipText = "Not applicable to Fuzzy Search. Fuzzy Search is always Case Insensitive.";
				}
				rect.y += rect.height + TOGGLE_SPACING;
				GUI.enabled = prevEnabled;

				// input catcher
				GUI.Button(searchOptionsBg, GUIContent.none, "HiddenScrollbar");
			}
		}

		public override void DrawGUI(Rect position,
			BuildInfo buildReportToDisplay, AssetDependencies assetDependencies,
			TextureData textureData, MeshData meshData, PrefabData prefabData,
			UnityBuildReport unityBuildReport, CustomBuildReport.ExtraData extraData, CustomBuildReport.AssetBundleSession assetBundleSession,
			out bool requestRepaint)
		{
			if (buildReportToDisplay == null || (!buildReportToDisplay.HasUsedAssets && !buildReportToDisplay.HasAssetBundles))
			{
				requestRepaint = false;
				return;
			}

#if BRT_ASSET_LIST_SCREEN_DEBUG
			if (_debugText == null)
			{
				_debugText = new System.Text.StringBuilder();
			}
			else
			{
				_debugText.Length = 0;
			}
#endif


			// init variables to use
			// --------------------------------------------------------------------------

			CustomBuildReport.FileFilterGroup fileFilterGroupToUse = buildReportToDisplay.FileFilters;

			if (CustomBuildReport.Options.ShouldUseConfiguredFileFilters())
			{
				fileFilterGroupToUse = _configuredFileFilterGroup;
			}

			CustomBuildReport.AssetList listToDisplay = GetAssetListToDisplay(buildReportToDisplay, assetBundleSession);

			if (listToDisplay == null)
			{
				if (IsShowingUsedAssets)
				{
					Utility.DrawCentralMessage(position, "No \"Used Assets\" included in this build report.");
				}
				else if (IsShowingUnusedAssets)
				{
					Utility.DrawCentralMessage(position, "No \"Unused Assets\" included in this build report.");
				}

				requestRepaint = false;
				return;
			}


			// continually request repaint, since we need to check the rects
			// generated by the GUILayout (using GUILayoutUtility.GetLastRect())
			// to make the hover checks work. GetLastRect() only works during
			// repaint event.
			//
			// later checks below can set requestRepaint to false if there's no
			// need to repaint, to help lessen cpu usage
			requestRepaint = true;

			// GUI
			// --------------------------------------------------------------------------

			DrawUnderlay(buildReportToDisplay, assetBundleSession);

			GUILayout.Space(1);

			// Toolbar at top
			// ------------------------------------------------

			DrawTopBar(position, buildReportToDisplay, assetBundleSession, fileFilterGroupToUse);


			// Actual Asset List
			// ------------------------------------------------

			if (buildReportToDisplay.HasUsedAssets || buildReportToDisplay.HasAssetBundles)
			{
				DrawAssetList(position, buildReportToDisplay, assetDependencies, textureData, meshData, prefabData,
					listToDisplay, fileFilterGroupToUse, CustomBuildReport.Options.AssetListPaginationLength);
				GUILayout.FlexibleSpace();
			}


			// Asset Usage Panel for selected
			// ------------------------------------------------

			DrawAssetUsage(position, listToDisplay, buildReportToDisplay, assetDependencies);

			var selectedName = listToDisplay.GetSelectedCount() == 1 ? listToDisplay.GetLastSelected().Name : null;

			// .unity files (scenes) are users themselves, but no asset uses them, because they are the ones directly
			// included in builds
			bool selectedAssetHasUsers = listToDisplay.GetSelectedCount() == 1 && !string.IsNullOrEmpty(selectedName) &&
			                             (assetDependencies != null && assetDependencies.GetAssetDependencies().ContainsKey(selectedName));

			bool isAssetUsagePanelShown = selectedAssetHasUsers || _selectedIsAResourcesAsset || buildReportToDisplay.HasAssetBundles;


			// Status bar at bottom
			// ------------------------------------------------

			// reserve space for the bottom bar
			// later we draw the bottom bar using GUILayout.BeginArea/EndArea
			GUILayout.Space(BOTTOM_STATUS_BAR_HEIGHT);

			var bottomBarRect = new Rect(0, 0, position.width, BOTTOM_STATUS_BAR_HEIGHT);

			if (isAssetUsagePanelShown)
			{
				// bottom bar is anchored to the bottom of the window
				// but move it up a bit, since the bottom-most portion is now occupied by the Asset Usage Panel
				bottomBarRect.y = position.height - BOTTOM_STATUS_BAR_HEIGHT - _assetUsageRect.height;
			}
			else
			{
				// bottom bar is anchored to the bottom of the window
				bottomBarRect.y = position.height - BOTTOM_STATUS_BAR_HEIGHT;
			}


			// --------------------------

			if (Event.current.mousePosition.y >= position.height ||
			    Event.current.mousePosition.y <= _assetPathColumnHeaderRect.yMin - 5 ||
			    Event.current.mousePosition.x <= 0 ||
			    Event.current.mousePosition.x >= position.width)
			{
				if (!_mouseIsPreviouslyInWindow)
				{
					// mouse is outside the area that shows tooltips
					// set requestRepaint to false to help lessen cpu usage
					requestRepaint = false;
				}
				_mouseIsPreviouslyInWindow = false;
			}
			else
			{
				_mouseIsPreviouslyInWindow = true;
			}

			// if mouse is too far below, above, or to the right for showing tooltip
			// in main asset list, then prevent tooltip from showing in that situation
			// note: this doesn't prevent tooltips in the asset usage panel
			if (Event.current.type == EventType.Repaint && (Event.current.mousePosition.y >= bottomBarRect.y ||
			                                                Event.current.mousePosition.y <=
			                                                _assetPathColumnHeaderRect.yMax ||
			                                                Event.current.mousePosition.x >= position.width))
			{
				_assetListEntryHoveredIdx = -1;
			}

			var shouldShowThumbnailTooltipNow = CustomBuildReport.Options.ShowTooltipThumbnail &&
			                                    _shouldShowThumbnailOnHoveredAsset &&
			                                    (_assetListEntryHoveredIdx != -1 ||
			                                     _assetUsageAncestryHoveredIdx != -1 ||
			                                     _assetUserEntryHoveredIdx != -1);

			var zoomInChanged = false;
			if (shouldShowThumbnailTooltipNow)
			{
				var prevZoomedIn = BRT_BuildReportWindow.ZoomedInThumbnails;

				// if thumbnail is currently showing, we process the inputs
				// (ctrl zooms in on thumbnail, alt toggles alpha blend)
				BRT_BuildReportWindow.ProcessThumbnailControls();

				if (prevZoomedIn != BRT_BuildReportWindow.ZoomedInThumbnails)
				{
					zoomInChanged = true;
				}
			}
			else
			{
				// no thumbnail currently shown. ensure the controls that
				// need to be reset to initial state are reset
				BRT_BuildReportWindow.ResetThumbnailControls();
			}

			if (!zoomInChanged && !Event.current.alt &&
			    !BRT_BuildReportWindow.MouseMovedNow && !BRT_BuildReportWindow.LastMouseMoved)
			{
				// mouse hasn't moved, and no request to zoom-in thumbnail or toggle thumbnail alpha
				// no need to repaint because nothing has changed
				// set requestRepaint to false to help lessen cpu usage
				requestRepaint = false;
			}

			// --------------------------
			// actual contents of Bottom Bar

			var statusBarBgStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.STATUS_BAR_BG_STYLE_NAME);
			if (statusBarBgStyle == null)
			{
				statusBarBgStyle = GUI.skin.box;
			}

			var statusBarLabelStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.STATUS_BAR_LABEL_STYLE_NAME);
			if (statusBarLabelStyle == null)
			{
				statusBarLabelStyle = GUI.skin.label;
			}

			string selectedInfoLabel = string.Format(
				"{0}{1}. {2}{3} ({4}%)",
				Labels.SELECTED_QTY_LABEL,
				listToDisplay.GetSelectedCount().ToString("N0"),
				Labels.SELECTED_SIZE_LABEL, listToDisplay.GetReadableSizeOfSumSelection(),
				listToDisplay.GetPercentageOfSumSelection().ToString("N"));

			GUILayout.BeginArea(bottomBarRect);

			GUILayout.BeginHorizontal(statusBarBgStyle,
				BRT_BuildReportWindow.LayoutNone);
			GUILayout.Label(selectedInfoLabel, statusBarLabelStyle,
				BRT_BuildReportWindow.LayoutNone);
			GUILayout.FlexibleSpace();

			if (shouldShowThumbnailTooltipNow)
			{
				GUILayout.Label(
					"Hold Ctrl to zoom-in on the thumbnail. Press Alt to show/hide alpha transparency.",
					statusBarLabelStyle,
					BRT_BuildReportWindow.LayoutNone);
			}
			else
			{
				GUILayout.Label(
					"Click on an asset's name to include it in size calculations or batch deletions. Shift-click to select many. Ctrl-click to toggle selection.",
					statusBarLabelStyle,
					BRT_BuildReportWindow.LayoutNone);
			}

			GUILayout.EndHorizontal();

			GUILayout.EndArea();

			// --------------------------

#if BRT_ASSET_LIST_SCREEN_DEBUG
		_debugText.AppendFormat("Event.current.mousePosition.x: {0}\nposition.width: {1}\nshouldShowThumbnailTooltipNow: {2}\n",
			Event.current.mousePosition.x.ToString(CultureInfo.InvariantCulture),
			position.width.ToString(CultureInfo.InvariantCulture),
			shouldShowThumbnailTooltipNow);
#endif

			// ------------------------------------------------

			DrawOverlay(buildReportToDisplay, assetBundleSession);

			// ------------------------------------------------
			// Tooltip

			var shouldShowAssetEndUsersTooltipNow = CustomBuildReport.Options.ShowAssetPrimaryUsersInTooltipIfAvailable &&
			                                        BRT_BuildReportWindow.ShouldHoveredAssetShowEndUserTooltip(assetDependencies) &&
			                                        (_assetListEntryHoveredIdx != -1 ||
			                                         _assetUsageAncestryHoveredIdx != -1 ||
			                                         _assetUserEntryHoveredIdx != -1);

#if BRT_ASSET_LIST_SCREEN_DEBUG
			_debugText.AppendFormat("shouldShowAssetEndUsersTooltipNow: {0}\n",
				shouldShowAssetEndUsersTooltipNow);
			_debugText.AppendFormat("_mouseIsOnOverlayControl: {0}\n",
				_mouseIsOnOverlayControl);
			_debugText.AppendFormat("_overridenTextureDataTooltipText: {0}\n",
				_overridenTextureDataTooltipText);
			_debugText.AppendFormat("HoveredAssetEntryPath: {0}\n",
				BRT_BuildReportWindow.HoveredAssetEntryPath);
#endif

			if (Event.current.type == EventType.Repaint)
			{
				if (!string.IsNullOrEmpty(_overridenTextureDataTooltipText))
				{
					_textureDataTooltipLabel.text = _overridenTextureDataTooltipText;
					var tooltipTextStyle = GUI.skin.FindStyle("TooltipText");
					if (tooltipTextStyle == null)
					{
						tooltipTextStyle = GUI.skin.box;
					}

					const int MAX_TOOLTIP_WIDTH = 240;
					var tooltipSize = tooltipTextStyle.CalcSize(_textureDataTooltipLabel);
					if (tooltipSize.x > MAX_TOOLTIP_WIDTH)
					{
						tooltipSize.x = MAX_TOOLTIP_WIDTH;
						tooltipSize.y = tooltipTextStyle.CalcHeight(_textureDataTooltipLabel, tooltipSize.x);
					}
#if UNITY_6000_0_OR_NEWER
					tooltipSize.x += 5;
#endif

					var tooltipRect = BRT_BuildReportWindow.DrawTooltip(position, tooltipSize.x, tooltipSize.y, 5);
					GUI.Label(tooltipRect, _textureDataTooltipLabel, tooltipTextStyle);
				}
				else if (shouldShowThumbnailTooltipNow && shouldShowAssetEndUsersTooltipNow && !_mouseIsOnOverlayControl)
				{
					// draw thumbnail and end users below it
					BRT_BuildReportWindow.DrawThumbnailEndUsersTooltip(position, assetDependencies, textureData);
				}
				else if (shouldShowAssetEndUsersTooltipNow && !_mouseIsOnOverlayControl)
				{
					// draw only end users in tooltip
					BRT_BuildReportWindow.DrawEndUsersTooltip(position, assetDependencies);
				}
				else if (shouldShowThumbnailTooltipNow && !_mouseIsOnOverlayControl)
				{
					// draw only thumbnail in tooltip
					BRT_BuildReportWindow.DrawThumbnailTooltip(position, textureData);
				}
				else if (_hoveredTextureDataId != CustomBuildReport.TextureData.DataId.None ||
				         _hoveredMeshDataId != CustomBuildReport.MeshData.DataId.None ||
				         _hoveredPrefabDataId != CustomBuildReport.PrefabData.DataId.None)
				{
					if (_hoveredTextureDataId != CustomBuildReport.TextureData.DataId.None)
					{
						_textureDataTooltipLabel.text =
							CustomBuildReport.TextureData.GetTooltipTextFromId(_hoveredTextureDataId);
					}
					else if (_hoveredMeshDataId != CustomBuildReport.MeshData.DataId.None)
					{
						_textureDataTooltipLabel.text = CustomBuildReport.MeshData.GetTooltipTextFromId(_hoveredMeshDataId);
					}
					else if (_hoveredPrefabDataId != CustomBuildReport.PrefabData.DataId.None)
					{
						_textureDataTooltipLabel.text = CustomBuildReport.PrefabData.GetTooltipTextFromId(_hoveredPrefabDataId);
					}

					if (!string.IsNullOrEmpty(_textureDataTooltipLabel.text))
					{
						var tooltipTextStyle = GUI.skin.FindStyle("TooltipText");
						if (tooltipTextStyle == null)
						{
							tooltipTextStyle = GUI.skin.box;
						}

						const int MAX_TOOLTIP_WIDTH = 400;
						var tooltipSize = tooltipTextStyle.CalcSize(_textureDataTooltipLabel);
						if (tooltipSize.x > MAX_TOOLTIP_WIDTH)
						{
							tooltipSize.x = MAX_TOOLTIP_WIDTH;
							tooltipSize.y = tooltipTextStyle.CalcHeight(_textureDataTooltipLabel, tooltipSize.x);
						}
#if UNITY_6000_0_OR_NEWER
						tooltipSize.x += 5;
						tooltipSize.y += 5;
#endif

						var tooltipRect = BRT_BuildReportWindow.DrawTooltip(position, tooltipSize.x, tooltipSize.y, 5);
						GUI.Label(tooltipRect, _textureDataTooltipLabel, tooltipTextStyle);
					}
				}
			}

#if BRT_ASSET_LIST_SCREEN_DEBUG
		// Debug text
		// ------------------------------------------------

		_showDebugText = GUI.Toggle(new Rect(position.width - 90, 20, 90, 20),
			_showDebugText, "Show Debug", "Button");

		if (_showDebugText)
		{
			_debugLabel.text = _debugText.ToString();
			var debugStyle = GUI.skin.FindStyle("DebugOverlay");
			if (debugStyle == null)
			{
				debugStyle = GUI.skin.box;
			}
			var debugLabelSize = debugStyle.CalcSize(_debugLabel);

			var prevBgColor = GUI.backgroundColor;
			GUI.backgroundColor = new Color(1, 1, 1, 0.85f);
			GUI.Label(new Rect(position.width - debugLabelSize.x, 0, debugLabelSize.x, debugLabelSize.y), _debugLabel, debugStyle);
			GUI.backgroundColor = prevBgColor;
		}
#endif
		}


		public void ToggleSort(CustomBuildReport.AssetList assetList, CustomBuildReport.AssetList.SortType newSortType, CustomBuildReport.FileFilterGroup fileFilters)
		{
			_currentTextureDataSortType = CustomBuildReport.TextureData.DataId.None;

			if (_currentSortType != newSortType)
			{
				_currentSortType = newSortType;
				_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending; // descending by default
			}
			else
			{
				// already in this sort type
				// now toggle the sort order
				if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
				{
					_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Ascending;
				}
				else if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Ascending)
				{
					if (_searchResults != null)
					{
						// clicked again while sort order is in ascending
						// now disable it (which means sorting goes back to sort by search rank)
						_currentSortType = CustomBuildReport.AssetList.SortType.None;
						_currentSortOrder = CustomBuildReport.AssetList.SortOrder.None;
					}
					else
					{
						_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending;
					}
				}
			}

			if (_searchResults != null)
			{
				if (_currentSortType == CustomBuildReport.AssetList.SortType.None)
				{
					// no column used as sort
					// revert to sorting by search rank
					SortBySearchRank(_searchResults, _lastSearchText);
				}
				else
				{
					CustomBuildReport.AssetListUtility.SortAssetList(_searchResults, _currentSortType, _currentSortOrder);
				}
			}
			else
			{
				assetList.Sort(_currentSortType, _currentSortOrder, fileFilters);
			}
		}

		void ToggleSort(CustomBuildReport.AssetList assetList, CustomBuildReport.TextureData textureData,
			CustomBuildReport.TextureData.DataId newSortType, CustomBuildReport.FileFilterGroup fileFilters)
		{
			if (_currentSortType != CustomBuildReport.AssetList.SortType.TextureData ||
			    _currentTextureDataSortType != newSortType)
			{
				_currentSortType = CustomBuildReport.AssetList.SortType.TextureData;
				_currentTextureDataSortType = newSortType;
				_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending; // descending by default
			}
			else
			{
				// already in this sort type
				// now toggle the sort order
				if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
				{
					_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Ascending;
				}
				else if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Ascending)
				{
					if (_searchResults != null)
					{
						// clicked again while sort order is in ascending
						// now disable it (which means sorting goes back to sort by search rank)
						_currentSortType = CustomBuildReport.AssetList.SortType.None;
						_currentSortOrder = CustomBuildReport.AssetList.SortOrder.None;
						_currentTextureDataSortType = CustomBuildReport.TextureData.DataId.None;
					}
					else
					{
						_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending;
					}
				}
			}

			if (_searchResults != null)
			{
				if (_currentSortType == CustomBuildReport.AssetList.SortType.None &&
				    _currentTextureDataSortType == CustomBuildReport.TextureData.DataId.None)
				{
					// no column used as sort
					// revert to sorting by search rank
					SortBySearchRank(_searchResults, _lastSearchText);
				}
				else
				{
					CustomBuildReport.AssetListUtility.SortAssetList(_searchResults, textureData, _currentTextureDataSortType, _currentSortOrder);
				}
			}
			else
			{
				assetList.Sort(textureData, _currentTextureDataSortType, _currentSortOrder, fileFilters);
			}
		}

		void ToggleSort(CustomBuildReport.AssetList assetList, CustomBuildReport.MeshData meshData,
			CustomBuildReport.MeshData.DataId newSortType, CustomBuildReport.FileFilterGroup fileFilters)
		{
			if (_currentSortType != CustomBuildReport.AssetList.SortType.MeshData ||
			    _currentMeshDataSortType != newSortType)
			{
				_currentSortType = CustomBuildReport.AssetList.SortType.MeshData;
				_currentMeshDataSortType = newSortType;
				_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending; // descending by default
			}
			else
			{
				// already in this sort type
				// now toggle the sort order
				if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
				{
					_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Ascending;
				}
				else if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Ascending)
				{
					if (_searchResults != null)
					{
						// clicked again while sort order is in ascending
						// now disable it (which means sorting goes back to sort by search rank)
						_currentSortType = CustomBuildReport.AssetList.SortType.None;
						_currentSortOrder = CustomBuildReport.AssetList.SortOrder.None;
						_currentMeshDataSortType = CustomBuildReport.MeshData.DataId.None;
					}
					else
					{
						_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending;
					}
				}
			}

			if (_searchResults != null)
			{
				if (_currentSortType == CustomBuildReport.AssetList.SortType.None &&
				    _currentMeshDataSortType == CustomBuildReport.MeshData.DataId.None)
				{
					// no column used as sort
					// revert to sorting by search rank
					SortBySearchRank(_searchResults, _lastSearchText);
				}
				else
				{
					CustomBuildReport.AssetListUtility.SortAssetList(_searchResults, meshData, _currentMeshDataSortType, _currentSortOrder);
				}
			}
			else
			{
				assetList.Sort(meshData, _currentMeshDataSortType, _currentSortOrder, fileFilters);
			}
		}

		void ToggleSort(CustomBuildReport.AssetList assetList, CustomBuildReport.PrefabData prefabData,
			CustomBuildReport.PrefabData.DataId newSortType, CustomBuildReport.FileFilterGroup fileFilters)
		{
			if (_currentSortType != CustomBuildReport.AssetList.SortType.PrefabData ||
			    _currentPrefabDataSortType != newSortType)
			{
				_currentSortType = CustomBuildReport.AssetList.SortType.PrefabData;
				_currentPrefabDataSortType = newSortType;
				_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending; // descending by default
			}
			else
			{
				// already in this sort type
				// now toggle the sort order
				if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
				{
					_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Ascending;
				}
				else if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Ascending)
				{
					if (_searchResults != null)
					{
						// clicked again while sort order is in ascending
						// now disable it (which means sorting goes back to sort by search rank)
						_currentSortType = CustomBuildReport.AssetList.SortType.None;
						_currentSortOrder = CustomBuildReport.AssetList.SortOrder.None;
						_currentPrefabDataSortType = CustomBuildReport.PrefabData.DataId.None;
					}
					else
					{
						_currentSortOrder = CustomBuildReport.AssetList.SortOrder.Descending;
					}
				}
			}

			if (_searchResults != null)
			{
				if (_currentSortType == CustomBuildReport.AssetList.SortType.None &&
				    _currentPrefabDataSortType == CustomBuildReport.PrefabData.DataId.None)
				{
					// no column used as sort
					// revert to sorting by search rank
					SortBySearchRank(_searchResults, _lastSearchText);
				}
				else
				{
					CustomBuildReport.AssetListUtility.SortAssetList(_searchResults, prefabData, _currentPrefabDataSortType, _currentSortOrder);
				}
			}
			else
			{
				assetList.Sort(prefabData, _currentPrefabDataSortType, _currentSortOrder, fileFilters);
			}
		}

		void RefreshConfiguredFileFilters()
		{
			// reload used FileFilterGroup but save current used filter idx
			// to be re-set afterwards

			int tempIdx = 0;

			if (_configuredFileFilterGroup != null)
			{
				tempIdx = _configuredFileFilterGroup.GetSelectedFilterIdx();
			}

			_configuredFileFilterGroup = CustomBuildReport.FiltersUsed.GetProperFileFilterGroupToUse();

			_configuredFileFilterGroup.ForceSetSelectedFilterIdx(tempIdx);
		}


		void DrawTopBar(Rect position, BuildInfo buildReportToDisplay, CustomBuildReport.AssetBundleSession assetBundleSession,
			CustomBuildReport.FileFilterGroup fileFilterGroupToUse)
		{
			CustomBuildReport.AssetList assetListUsed = GetAssetListToDisplay(buildReportToDisplay, assetBundleSession);

			if (assetListUsed == null)
			{
				return;
			}

			Texture2D prevArrow;
			var prevArrowStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.BIG_LEFT_ARROW_ICON_STYLE_NAME);
			if (prevArrowStyle != null)
			{
				prevArrow = prevArrowStyle.normal.background;
			}
			else
			{
				prevArrow = null;
			}

			Texture2D nextArrow;
			var  nextArrowStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.BIG_RIGHT_ARROW_ICON_STYLE_NAME);
			if (nextArrowStyle != null)
			{
				nextArrow = nextArrowStyle.normal.background;
			}
			else
			{
				nextArrow = null;
			}

			var columnStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.COLUMN_ICON_STYLE_NAME);
			if (columnStyle != null)
			{
				_showColumnLabel.image = columnStyle.normal.background;
			}

			var topBarBgStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.TOP_BAR_BG_STYLE_NAME);
			if (topBarBgStyle == null)
			{
				topBarBgStyle = GUI.skin.label;
			}

			var topBarButtonStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.TOP_BAR_BTN_STYLE_NAME);
			if (topBarButtonStyle == null)
			{
				topBarButtonStyle = GUI.skin.button;
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

			var searchDropdownStyle = GUI.skin.FindStyle("TextField-Search-DropDown");
			if (searchDropdownStyle == null)
			{
				searchDropdownStyle = GUI.skin.label;
			}

			var searchTextStyle = GUI.skin.FindStyle("TextField-Search-Text");
			if (searchTextStyle == null)
			{
				searchTextStyle = GUI.skin.textField;
			}

			var searchClearStyle = GUI.skin.FindStyle("TextField-Search-ClearButton");
			if (searchClearStyle == null)
			{
				searchClearStyle = GUI.skin.button;
			}

			GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutHeight11);

			GUILayout.Label(" ", topBarBgStyle, BRT_BuildReportWindow.LayoutNone);

			// ------------------------------------------------------------------------------------------------------
			// Bundles

			if (IsShowingUsedAssets && buildReportToDisplay.HasAssetBundles)
			{
				GUILayout.Label("Bundle: ", topBarLabelStyle);
				assetBundleSession.SelectedBundleIdx = EditorGUILayout.Popup(assetBundleSession.SelectedBundleIdx,
					assetBundleSession.BundleNames,
					topBarPopupStyle);
				GUILayout.Space(20);
			}

			// ------------------------------------------------------------------------------------------------------

			// ------------------------------------------------------------------------------------------------------
			// File Filters

			var selectedFilterChanged = fileFilterGroupToUse.Draw(assetListUsed, position.width - 100);

			if (selectedFilterChanged)
			{
				_assetListEntryLastClickedIdx = -1;
				_filterIdxOfLastClickedAssetListEntry = NO_FILTER_VALUE;
			}
			GUILayout.Space(20);

			// ------------------------------------------------------------------------------------------------------

			// ------------------------------------------------------------------------------------------------------
			// Unused Assets Batch

			if (IsShowingUnusedAssets && buildReportToDisplay.ProcessUnusedAssetsInBatches)
			{
				int batchNumber = buildReportToDisplay.UnusedAssetsBatchIdx + 1;

				bool prevButton = prevArrow != null
					? GUILayout.Button(prevArrow, topBarButtonStyle)
					: GUILayout.Button("Previous", topBarButtonStyle);
				if (prevButton && (batchNumber - 1 >= 1))
				{
					// move to previous batch
					CustomBuildReport.ReportGenerator.MoveUnusedAssetsBatchToPrev(buildReportToDisplay, fileFilterGroupToUse);
				}

				string batchLabel = string.Format("Batch {0}", batchNumber.ToString());
				GUILayout.Label(batchLabel, topBarLabelStyle);

				bool nextButton = nextArrow != null
					? GUILayout.Button(nextArrow, topBarButtonStyle)
					: GUILayout.Button("Next", topBarButtonStyle);
				if (nextButton)
				{
					// move to next batch
					// (possible to have no new batch anymore. if so, it will just fail silently)
					CustomBuildReport.ReportGenerator.MoveUnusedAssetsBatchToNext(buildReportToDisplay, fileFilterGroupToUse);
				}
				GUILayout.Space(8);
			}

			// ------------------------------------------------------------------------------------------------------

			// ------------------------------------------------------------------------------------------------------
			// Paginate Buttons

			CustomBuildReport.SizePart[] assetListToUse = assetListUsed.GetListToDisplay(fileFilterGroupToUse);

			// how many assets overall in this entire list
			int assetListLength = 0;
			if (_searchResults != null && _searchResults.Length > 0)
			{
				assetListLength = _searchResults.Length;
			}
			else if (assetListToUse != null)
			{
				assetListLength = assetListToUse.Length;
			}

			// index of first asset to show, for current page.
			// This is an offset from the entire asset list.
			int viewOffset = _searchResults != null
				                 ? _searchViewOffset
				                 : assetListUsed.GetViewOffsetForDisplayedList(fileFilterGroupToUse);

			if (GUILayout.Button(prevArrow, topBarButtonStyle,
				    BRT_BuildReportWindow.LayoutNone) &&
			    (viewOffset - CustomBuildReport.Options.AssetListPaginationLength >= 0))
			{
				if (_searchResults != null)
				{
					_searchViewOffset -= CustomBuildReport.Options.AssetListPaginationLength;
				}
				else
				{
					assetListUsed.SetViewOffsetForDisplayedList(fileFilterGroupToUse,
						viewOffset - CustomBuildReport.Options.AssetListPaginationLength);
				}

				_assetListScrollPos.y = 0;
			}

			string paginateLabel;

			if (_showPageNumbers)
			{
				int totalPageNumbers = assetListLength/CustomBuildReport.Options.AssetListPaginationLength;
				if (assetListLength % CustomBuildReport.Options.AssetListPaginationLength > 0)
				{
					++totalPageNumbers;
				}

				// the max number of digits for the displayed offset counters
				string assetCountDigitNumFormat = string.Format("D{0}", totalPageNumbers.ToString().Length.ToString());

				paginateLabel = string.Format("Page {0} of {1}",
					((viewOffset/CustomBuildReport.Options.AssetListPaginationLength)+1).ToString(assetCountDigitNumFormat),
					totalPageNumbers.ToString());
			}
			else
			{
				// number of assets in current page
				int pageLength = Mathf.Min(viewOffset + CustomBuildReport.Options.AssetListPaginationLength, assetListLength);

				// the max number of digits for the displayed offset counters
				string assetCountDigitNumFormat = string.Format("D{0}", assetListLength.ToString().Length.ToString());

				int offsetNonZeroBased = viewOffset + (pageLength > 0 ? 1 : 0);

				paginateLabel = string.Format("Page {0} - {1} of {2}",
					offsetNonZeroBased.ToString(assetCountDigitNumFormat),
					pageLength.ToString(assetCountDigitNumFormat),
					assetListLength.ToString(assetCountDigitNumFormat));
			}

			if (GUILayout.Button(paginateLabel, topBarLabelStyle,
				BRT_BuildReportWindow.LayoutNone))
			{
				_showPageNumbers = !_showPageNumbers;
			}

			if (GUILayout.Button(nextArrow, topBarButtonStyle,
				    BRT_BuildReportWindow.LayoutNone) &&
			    (viewOffset + CustomBuildReport.Options.AssetListPaginationLength < assetListLength))
			{
				if (_searchResults != null)
				{
					_searchViewOffset += CustomBuildReport.Options.AssetListPaginationLength;
				}
				else
				{
					assetListUsed.SetViewOffsetForDisplayedList(fileFilterGroupToUse,
						viewOffset + CustomBuildReport.Options.AssetListPaginationLength);
				}

				_assetListScrollPos.y = 0;
			}

			// ------------------------------------------------------------------------------------------------------

			GUILayout.FlexibleSpace();

			var newShowColumnOptions = GUILayout.Toggle(_showColumnOptions, _showColumnLabel,
				topBarButtonStyle, BRT_BuildReportWindow.LayoutNone);
			if (newShowColumnOptions != _showColumnOptions)
			{
				_showColumnOptions = newShowColumnOptions;
				if (_showColumnOptions)
				{
					_showSearchOptions = false;
				}
			}
			if (Event.current.type == EventType.Repaint)
			{
				_showColumnOptionButtonRect = GUILayoutUtility.GetLastRect();
			}

			var newSearchOptions = GUILayout.Toggle(_showSearchOptions, GUIContent.none, searchDropdownStyle,
				BRT_BuildReportWindow.LayoutNone);
			if (newSearchOptions != _showSearchOptions)
			{
				_showSearchOptions = newSearchOptions;
				if (_showSearchOptions)
				{
					_showColumnOptions = false;
				}
			}
			if (Event.current.type == EventType.Repaint)
			{
				_searchTextfieldRect = GUILayoutUtility.GetLastRect();
			}
			_searchTextInput = GUILayout.TextField(_searchTextInput, searchTextStyle,
				BRT_BuildReportWindow.LayoutMinWidth200);
			if (GUILayout.Button(GUIContent.none, searchClearStyle, BRT_BuildReportWindow.LayoutNone))
			{
				ClearSearch();
			}
			if (Event.current.type == EventType.Repaint)
			{
				_searchTextfieldRect.xMax = GUILayoutUtility.GetLastRect().xMax;
			}

			// ------------------------------------------------------------------------------------------------------
			// Recalculate Imported sizes
			// (makes sense only for unused assets)

			if ((_currentListDisplayed != ListToDisplay.UsedAssets) &&
			    GUILayout.Button(Labels.RECALC_IMPORTED_SIZES,
				    topBarButtonStyle, BRT_BuildReportWindow.LayoutNone))
			{
				assetListUsed.PopulateImportedSizes();
			}

			if (!CustomBuildReport.Options.AutoResortAssetsWhenUnityEditorRegainsFocus &&
			    CustomBuildReport.Options.GetSizeBeforeBuildForUsedAssets &&
			    (_currentListDisplayed == ListToDisplay.UsedAssets) &&
			    GUILayout.Button(Labels.RECALC_SIZE_BEFORE_BUILD,
				    topBarButtonStyle, BRT_BuildReportWindow.LayoutNone))
			{
				assetListUsed.PopulateSizeInAssetsFolder();
			}

			// ------------------------------------------------------------------------------------------------------


			// ------------------------------------------------------------------------------------------------------
			// Selection buttons

			if (GUILayout.Button(Labels.SELECT_ALL_LABEL,
				    topBarButtonStyle, BRT_BuildReportWindow.LayoutNone))
			{
				assetListUsed.AddAllDisplayedToSumSelection(fileFilterGroupToUse);
			}

			if (GUILayout.Button(Labels.SELECT_NONE_LABEL,
				    topBarButtonStyle, BRT_BuildReportWindow.LayoutNone))
			{
				assetListUsed.ClearSelection();
				_assetListEntryLastClickedIdx = -1;
				_filterIdxOfLastClickedAssetListEntry = NO_FILTER_VALUE;
			}

			// ------------------------------------------------------------------------------------------------------


			// ------------------------------------------------------------------------------------------------------
			// Delete buttons

			if (ShouldShowDeleteButtons(buildReportToDisplay))
			{
				GUI.backgroundColor = Color.red;
				const string DEL_SELECTED_LABEL = "Delete selected";
				if (GUILayout.Button(DEL_SELECTED_LABEL,
					    topBarButtonStyle, BRT_BuildReportWindow.LayoutNone))
				{
					InitiateDeleteSelectedUsed(buildReportToDisplay, assetBundleSession);
				}

				const string DELETE_ALL_LABEL = "Delete all";
				if (GUILayout.Button(DELETE_ALL_LABEL,
					    topBarButtonStyle, BRT_BuildReportWindow.LayoutNone))
				{
					InitiateDeleteAllUnused(buildReportToDisplay);
				}

				GUI.backgroundColor = Color.white;
			}

			// ------------------------------------------------------------------------------------------------------

			GUILayout.EndHorizontal();


			GUILayout.Space(5);
		}


		GUIStyle GetColumnHeaderStyle(CustomBuildReport.AssetList.SortType sortTypeNeeded)
		{
			string styleResult = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_STYLE_NAME;

			if (_currentSortType == sortTypeNeeded)
			{
				if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
				{
					styleResult = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_DESC_STYLE_NAME;
				}
				else
				{
					styleResult = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_ASC_STYLE_NAME;
				}
			}

			var style = GUI.skin.FindStyle(styleResult);
			if (style == null)
			{
				style = GUI.skin.label;
			}

			return style;
		}

		void DrawAssetList(Rect position,
			CustomBuildReport.BuildInfo buildReportToDisplay, CustomBuildReport.AssetDependencies assetDependencies,
			CustomBuildReport.TextureData textureData, CustomBuildReport.MeshData meshData, CustomBuildReport.PrefabData prefabData,
			CustomBuildReport.AssetList list, CustomBuildReport.FileFilterGroup filter, int length)
		{
			if (list == null)
			{
				return;
			}

			if (_searchResults != null && _searchResults.Length == 0)
			{
				DrawCentralMessage(position, "No search results");
				return;
			}

			var listEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_STYLE_NAME);
			if (listEntryStyle == null)
			{
				listEntryStyle = GUI.skin.label;
			}

			var listAltEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_ALT_STYLE_NAME);
			if (listAltEntryStyle == null)
			{
				listAltEntryStyle = GUI.skin.label;
			}

			var listSelectedEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_SELECTED_NAME);
			if (listSelectedEntryStyle == null)
			{
				listSelectedEntryStyle = GUI.skin.label;
			}

			CustomBuildReport.SizePart[] assetListToUse;

			var hasSearchResults = _searchResults != null;

			if (hasSearchResults && _searchResults.Length > 0)
			{
				assetListToUse = _searchResults;
			}
			else
			{
				assetListToUse = list.GetListToDisplay(filter);
			}

			if (assetListToUse == null)
			{
				return;
			}

			var messageStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.INFO_TITLE_STYLE_NAME);
			if (messageStyle == null)
			{
				messageStyle = GUI.skin.label;
			}

			if (assetListToUse.Length == 0)
			{
				GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
				GUILayout.Space(10);
				GUILayout.Label(Labels.NO_FILES_FOR_THIS_CATEGORY_LABEL,
					messageStyle, BRT_BuildReportWindow.LayoutNone);
				GUILayout.EndHorizontal();

				return;
			}

			var hiddenHorizontalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenHorizontalScrollbarStyle == null)
			{
				hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;
			}

			var hiddenVerticalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenVerticalScrollbarStyle == null)
			{
				hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;
			}

			var textureStyle = GUI.skin.FindStyle("DrawTexture");
			if (textureStyle == null)
			{
				textureStyle = GUI.skin.label;
			}

			var listButtonStyle = GUI.skin.FindStyle("ListButton");
			if (listButtonStyle == null)
			{
				listButtonStyle = GUI.skin.button;
			}

			EditorGUIUtility.SetIconSize(BRT_BuildReportWindow.IconSize);

			int viewOffset = hasSearchResults ? _searchViewOffset : list.GetViewOffsetForDisplayedList(filter);

			// if somehow view offset was out of bounds of the SizePart[] array
			// reset it to zero
			if (viewOffset >= assetListToUse.Length)
			{
				list.SetViewOffsetForDisplayedList(filter, 0);
				viewOffset = 0;
			}

			int len = Mathf.Min(viewOffset + length, assetListToUse.Length);

			// --------------------------------------------------------------------------------------------------------

			var showTextureColumns = filter.SelectedFilterIdx >= 0 &&
			                         filter.GetSelectedFilterLabel() == CustomBuildReport.Options.FileFilterNameForTextureData &&
			                         textureData != null &&
			                         textureData.HasContents;

			var showMeshColumns = filter.SelectedFilterIdx >= 0 &&
			                      filter.GetSelectedFilterLabel() == CustomBuildReport.Options.FileFilterNameForMeshData &&
			                      meshData != null &&
			                      meshData.HasContents;

			var showPrefabColumns = filter.SelectedFilterIdx >= 0 &&
			                        filter.GetSelectedFilterLabel() == CustomBuildReport.Options.FileFilterNameForPrefabData &&
			                        prefabData != null &&
			                        prefabData.HasContents;

			// --------------------------------------------------------------------------------------------------------

			GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);

			// --------------------------------------------------------------------------------------------------------

			#region Column: Asset Path and Name
			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
			var useAlt = false;

			#region Header
			GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);

			Rect assetPathCheckboxRect = _assetPathColumnHeaderRect;
			assetPathCheckboxRect.x += 5;
			assetPathCheckboxRect.y += 1;
			assetPathCheckboxRect.width = 20;
			assetPathCheckboxRect.height -= 1;

			CustomBuildReport.Options.ShowColumnAssetPath = GUI.Toggle(assetPathCheckboxRect, CustomBuildReport.Options.ShowColumnAssetPath, _assetPathCheckboxLabel);

			var sortTypeAssetFullPathStyleName = GetColumnHeaderStyle(CustomBuildReport.AssetList.SortType.AssetFullPath);
			if (GUILayout.Button("      Asset Path", sortTypeAssetFullPathStyleName,
				BRT_BuildReportWindow.LayoutListHeight))
			{
				ToggleSort(list, CustomBuildReport.AssetList.SortType.AssetFullPath, filter);
			}

			if (Event.current.type == EventType.Repaint)
			{
				_assetPathColumnHeaderRect = GUILayoutUtility.GetLastRect();
#if BRT_ASSET_LIST_SCREEN_DEBUG
			_debugText.AppendFormat("_assetPathColumnHeaderRect: {0}\nyMax: {1}\n", _assetPathColumnHeaderRect.ToString(),
				_assetPathColumnHeaderRect.yMax.ToString(CultureInfo.InvariantCulture));
#endif
			}

			GUI.Toggle(assetPathCheckboxRect, CustomBuildReport.Options.ShowColumnAssetPath, _assetPathCheckboxLabel);

			// -----------------------------------------------------------------

			var sortTypeAssetFilenameStyleName = GetColumnHeaderStyle(CustomBuildReport.AssetList.SortType.AssetFilename);
			if (GUILayout.Button("Asset Filename", sortTypeAssetFilenameStyleName, BRT_BuildReportWindow.LayoutListHeight))
			{
				ToggleSort(list, CustomBuildReport.AssetList.SortType.AssetFilename, filter);
			}

			GUILayout.EndHorizontal();
			#endregion

			// --------------------------------------------------------------------------------------------------------

			var newEntryHoveredIdx = -1;

			_assetListScrollPos = GUILayout.BeginScrollView(_assetListScrollPos,
				hiddenHorizontalScrollbarStyle,
				hiddenVerticalScrollbarStyle, BRT_BuildReportWindow.LayoutNone);

			var mousePos = Event.current.mousePosition;

			for (int n = viewOffset; n < len; ++n)
			{
				CustomBuildReport.SizePart b = assetListToUse[n];

				var styleToUse = useAlt
					                    ? listAltEntryStyle
					                    : listEntryStyle;
				bool inSumSelect = list.InSumSelection(b);
				if (inSumSelect)
				{
					styleToUse = listSelectedEntryStyle;
				}

				#region Entry Bg
				GUILayout.BeginHorizontal(styleToUse, BRT_BuildReportWindow.LayoutNone);


				if (!CustomBuildReport.Options.DoubleClickOnAssetWillPing)
				{
					// only asset entries inside the top-level Assets or Packages folder can be pinged
					if (b.Name.IsInAssetsFolder() || b.Name.IsInPackagesFolder())
					{
						if (GUILayout.Button("Ping", listButtonStyle, BRT_BuildReportWindow.LayoutPingButton))
						{
							if (list.GetSelectedCount() > 1 && list.InSumSelection(b) && Event.current.alt)
							{
								// ping multiple
								Utility.PingSelectedAssets(list);
							}
							else
							{
								Utility.PingAssetInProject(b.Name);
							}
						}
					}
					else
					{
						// add spacing where the ping button would be,
						// so that this entry aligns with the rest
						GUILayout.Space(38);
					}
				}


				_assetListEntry.image = AssetDatabase.GetCachedIcon(b.Name);
				var hasIcon = _assetListEntry.image != null;

				var mouseIsOnEmptySpaceForIcon = false;

				if (!hasIcon)
				{
					// entry has no icon, just add space so it aligns with the other entries
					GUILayout.Label(string.Empty, textureStyle, BRT_BuildReportWindow.LayoutIconWidth);

					if (Event.current.type == EventType.Repaint)
					{
						var emptySpaceForIconRect = GUILayoutUtility.GetLastRect();
						if (emptySpaceForIconRect.Contains(mousePos))
						{
							mouseIsOnEmptySpaceForIcon = true;
						}
					}
				}

				_assetListEntry.text = GetPrettyAssetPath(b.Name, n, CustomBuildReport.Options.ShowColumnAssetPath, inSumSelect);

				Color temp = styleToUse.normal.textColor;
				int origLeft = styleToUse.padding.left;
				int origRight = styleToUse.padding.right;

				styleToUse.normal.textColor = styleToUse.onNormal.textColor;
				styleToUse.padding.right = 0;

				styleToUse.normal.textColor = temp;
				styleToUse.padding.left = 2;

				// the asset icon and name

				if (GUILayout.Button(_assetListEntry, styleToUse, BRT_BuildReportWindow.LayoutListHeight))
				{
					if (Event.current.control)
					{
						if (!inSumSelect)
						{
							list.AddToSumSelection(b);
							_assetListEntryLastClickedIdx = n;
						}
						else
						{
							list.ToggleSumSelection(b);
							_assetListEntryLastClickedIdx = -1;
						}
					}
					else if (Event.current.shift)
					{
						if (_assetListEntryLastClickedIdx != -1)
						{
							// select all from last clicked to here
							if (_assetListEntryLastClickedIdx < n)
							{
								for (int addToSelectIdx = _assetListEntryLastClickedIdx; addToSelectIdx <= n; ++addToSelectIdx)
								{
									list.AddToSumSelection(assetListToUse[addToSelectIdx]);
								}
							}
							else if (_assetListEntryLastClickedIdx > n)
							{
								for (int addToSelectIdx = n; addToSelectIdx <= _assetListEntryLastClickedIdx; ++addToSelectIdx)
								{
									list.AddToSumSelection(assetListToUse[addToSelectIdx]);
								}
							}
						}
						else
						{
							list.AddToSumSelection(b);
						}

						_assetListEntryLastClickedIdx = n;
					}
					else
					{
						// single select
						// -----------------------------

						// double-click detection for pinging
						if (CustomBuildReport.Options.DoubleClickOnAssetWillPing &&
						    (EditorApplication.timeSinceStartup - _assetListEntryLastClickedTime) < DOUBLE_CLICK_THRESHOLD &&
						    (b.Name.IsInAssetsFolder() || b.Name.IsInPackagesFolder()))
						{
							if (list.GetSelectedCount() > 1 && list.InSumSelection(b) && Event.current.alt)
							{
								// double-clicking on one of the selected assets while holding alt
								// ping multiple
								Utility.PingSelectedAssets(list);
							}
							else if (_assetListEntryLastClickedIdx == n)
							{
								// 2nd click on the same asset (i.e. double-click)
								Utility.PingAssetInProject(b.Name);
							}
						}


						// --------------------
						// selecting a different asset
						// click with no ctrl, alt, or shift
						if ((_assetListEntryLastClickedIdx != n ||
						     _filterIdxOfLastClickedAssetListEntry != filter.SelectedFilterIdx) &&
						    !Event.current.alt)
						{
							list.ClearSelection();
							list.AddToSumSelection(b);

							_assetListEntryLastClickedIdx = n;
							_filterIdxOfLastClickedAssetListEntry = filter.SelectedFilterIdx;

							// --------------------
							// update what's shown in the Asset Usage Panel

							if (b.Name.IsInResourcesFolder())
							{
								// this is a Resources asset
								_selectedIsAResourcesAsset = true;
							}
							else
							{
								_selectedIsAResourcesAsset = false;
							}
							_selectedAssetPath = b.Name;
							_selectedAsset.text = _selectedAssetPath.GetFileNameOnly();
							_selectedAsset.image = AssetDatabase.GetCachedIcon(_selectedAssetPath);

							if (_selectedAssetUsageDisplayIdx == ASSET_USAGE_DISPLAY_ALL)
							{
								_selectedAssetUserIdx = -1;
								_assetUsageAncestry.Clear();
								SetAssetUsageHistoryToFirstEndUser(b.Name, assetDependencies);
							}
							else if (_selectedAssetUsageDisplayIdx == ASSET_USAGE_DISPLAY_DIRECT)
							{
								ChangeAssetUserCrumbRootIfNeeded(b.Name);
							}
						}

						_assetListEntryLastClickedTime = EditorApplication.timeSinceStartup;
					}
				}

				styleToUse.padding.right = origRight;
				styleToUse.padding.left = origLeft;


#if BRT_ASSET_LIST_SCREEN_DEBUG
			//_debugText.AppendFormat("mousePos: {0}\n", mousePos.ToString());
#endif

				if (Event.current.type == EventType.Repaint)
				{
					// Have to do this during Repaint event because
					// GUILayoutUtility.GetLastRect() only works during that time.
					//
					// The problem is that our hover check should really only be done during
					// MouseMove event instead. The way it is right now, doing the hover check
					// every Repaint event, means it's doing this check over and over
					// even if the mouse is sitting still at the same position.
					//
					// However, we do mitigate this by not calling EditorWindow.Repaint() if
					// it's not needed.
					//
					// Also, getting GetLastRect() during Repaint event and then using
					// that value during MouseMove event means having to store the rect value
					// in a variable. Since we're checking for each entry in the asset list,
					// we'd have to store all the rects in a List
					// (we have one rect for each entry in the asset list).
					//
					// I don't know if that is too much processing,
					// so I'm leaving the code the way it is right now.

					var assetListEntryRect = GUILayoutUtility.GetLastRect();

					// note: Rects of the asset list entries do not overlap.
					// So actually, throughout all the iterations of this for-loop,
					// this if can only be successful once.
					if (assetListEntryRect.Contains(mousePos) || mouseIsOnEmptySpaceForIcon)
					{
						newEntryHoveredIdx = n;

						// ----------------
						// update what is considered the hovered asset, for use later on
						// when the tooltip will be drawn
						BRT_BuildReportWindow.UpdateHoveredAsset(b.Name, assetListEntryRect, IsShowingUsedAssets,
							buildReportToDisplay, assetDependencies);

						// ----------------
						// put a border on the icon to signify that it's the one being hovered
						// note: _assetListEntry.image currently has the icon of the asset we hovered
						Rect iconHoveredRect = assetListEntryRect;
						if (_assetListEntry.image != null)
						{
							iconHoveredRect.x += 1;
						}
						else
						{
							iconHoveredRect.x -= 15;
						}

						iconHoveredRect.y += 2;
						iconHoveredRect.width = 17;
						iconHoveredRect.height = 16;

						var iconHoveredStyle = GUI.skin.FindStyle("IconHovered");
						if (iconHoveredStyle == null)
						{
							iconHoveredStyle = GUI.skin.label;
						}
						GUI.Box(iconHoveredRect, _assetListEntry.image, iconHoveredStyle);

						// ----------------
						// if mouse is hovering over the correct area, we signify that
						// the tooltip thumbnail should be drawn
						if (CustomBuildReport.Options.ShowTooltipThumbnail &&
						    (CustomBuildReport.Options.ShowThumbnailOnHoverLabelToo ||
						     Mathf.Abs(mousePos.x - assetListEntryRect.x) < BRT_BuildReportWindow.ICON_WIDTH_WITH_PADDING) &&
						    BRT_BuildReportWindow.GetAssetPreview(b.Name) != null)
						{
							_shouldShowThumbnailOnHoveredAsset = true;
						}
						else
						{
							_shouldShowThumbnailOnHoveredAsset = false;
						}
					}
				}


				GUILayout.EndHorizontal();
				#endregion

				useAlt = !useAlt;
			} // end of for-loop for drawing all asset names

			if (Event.current.type == EventType.Repaint)
			{
				_assetListEntryHoveredIdx = newEntryHoveredIdx;
			}


#if BRT_ASSET_LIST_SCREEN_DEBUG
		_debugText.AppendFormat("_assetListEntryLastClickedTime: {0}\n",
			_assetListEntryLastClickedTime.ToString(CultureInfo.InvariantCulture));
		_debugText.AppendFormat("_assetListEntryHovered: {0}\n", _assetListEntryHoveredIdx.ToString());
#endif


			GUILayout.Space(SCROLLBAR_BOTTOM_PADDING);

			GUILayout.EndScrollView();

			GUILayout.EndVertical(); // end of column: asset path and name
			#endregion

			// --------------------------------------------------------------------------------------------------------

			#region Columns: Texture Data

			if (Event.current.type == EventType.Repaint)
			{
				_hoveredTextureDataId = CustomBuildReport.TextureData.DataId.None;
				_overridenTextureDataTooltipText = null;
			}

			_clickedTextureDataId = CustomBuildReport.TextureData.DataId.None;

			if (showTextureColumns)
			{
				if (CustomBuildReport.Options.ShowTextureColumnTextureType)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.TextureType, "Type",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnIsSRGB)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.IsSRGB, "Is sRGB?",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnAlphaSource)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.AlphaSource, "Alpha Source",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnAlphaIsTransparency)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.AlphaIsTransparency, "Alpha is Transparency",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnIgnorePngGamma)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.IgnorePngGamma, "Ignore PNG Gamma",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnIsReadable)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.IsReadable, "Read/Write Enabled",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				// -------------------------------
				if (CustomBuildReport.Options.ShowTextureColumnMipMapGenerated)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.MipMapGenerated, "MipMap Generated",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnMipMapFilter)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.MipMapFilter, "MipMap Filter",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnStreamingMipMaps)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.StreamingMipMaps, "Streaming MipMaps",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnBorderMipMaps)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.BorderMipMaps, "Border MipMaps",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnPreserveCoverageMipMaps)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.PreserveCoverageMipMaps, "Preserve Coverage MipMaps",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnFadeOutMipMaps)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.FadeOutMipMaps, "Fade Out MipMaps",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				// -------------------------------
				if (CustomBuildReport.Options.ShowTextureColumnSpriteImportMode)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.SpriteImportMode, "Sprite Mode",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnSpritePackingTag)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.SpritePackingTag, "Sprite Packing Tag",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnSpritePixelsPerUnit)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.SpritePixelsPerUnit, "Sprite Pixels-Per-Unit",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnQualifiesForSpritePacking)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.QualifiesForSpritePacking, "Qualifies for Sprite Packing",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnWrapMode)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.WrapMode, "Wrap Mode",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnWrapModeU)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.WrapModeU, "Wrap Mode U",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnWrapModeV)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.WrapModeV, "Wrap Mode V",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnWrapModeW)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.WrapModeW, "Wrap Mode W",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				// -------------------------------
				if (CustomBuildReport.Options.ShowTextureColumnFilterMode)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.FilterMode, "Filter Mode",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnAnisoLevel)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.AnisoLevel, "Anisotropic Level",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				// -------------------------------
				if (CustomBuildReport.Options.ShowTextureColumnTextureFormat)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.TextureFormat, "Format",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnCompressionType)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.CompressionType, "Compression Type",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnCompressionIsCrunched)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.CompressionIsCrunched, "Compression Crunched",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnCompressionQuality)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.CompressionQuality, "Compression Quality",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				// -------------------------------
				if (CustomBuildReport.Options.ShowTextureColumnResizeAlgorithm)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.TextureResizeAlgorithm, "Resize Algorithm",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnMaxTextureSize)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.MaxTextureSize, "Max Size",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnNPotScale)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.NPotScale, "Non-Power Of 2 Scale",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnImportedWidthAndHeight)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.ImportedWidthAndHeight, "Imported Width & Height",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowTextureColumnRealWidthAndHeight)
				{
					DrawTextureDataColumn(viewOffset, len,
						CustomBuildReport.TextureData.DataId.RealWidthAndHeight, "Source Width & Height",
						true, false, list, textureData,
						assetListToUse, ref _assetListScrollPos);
				}
				// -------------------------------

				if (_clickedTextureDataId != CustomBuildReport.TextureData.DataId.None)
				{
					ToggleSort(list, textureData, _clickedTextureDataId, filter);
					_clickedTextureDataId = CustomBuildReport.TextureData.DataId.None;
				}
			}
			#endregion

			// --------------------------------------------------------------------------------------------------------

			#region Columns: Mesh Data

			if (Event.current.type == EventType.Repaint)
			{
				_hoveredMeshDataId = CustomBuildReport.MeshData.DataId.None;
			}

			_clickedMeshDataId = CustomBuildReport.MeshData.DataId.None;

			if (showMeshColumns)
			{
				if (CustomBuildReport.Options.ShowMeshColumnMeshFilterCount)
				{
					DrawMeshDataColumn(viewOffset, len,
						CustomBuildReport.MeshData.DataId.MeshFilterCount, "Non-skinned",
						true, false, list, meshData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowMeshColumnSkinnedMeshRendererCount)
				{
					DrawMeshDataColumn(viewOffset, len,
						CustomBuildReport.MeshData.DataId.SkinnedMeshRendererCount, "Skinned",
						true, false, list, meshData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowMeshColumnSubMeshCount)
				{
					DrawMeshDataColumn(viewOffset, len,
						CustomBuildReport.MeshData.DataId.SubMeshCount, "SubMesh Count",
						true, false, list, meshData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowMeshColumnVertexCount)
				{
					DrawMeshDataColumn(viewOffset, len,
						CustomBuildReport.MeshData.DataId.VertexCount, "Vertices",
						true, false, list, meshData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowMeshColumnTriangleCount)
				{
					DrawMeshDataColumn(viewOffset, len,
						CustomBuildReport.MeshData.DataId.TriangleCount, "Faces",
						true, false, list, meshData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowMeshColumnAnimationType)
				{
					DrawMeshDataColumn(viewOffset, len,
						CustomBuildReport.MeshData.DataId.AnimationType, "Animation Type",
						true, false, list, meshData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowMeshColumnAnimationClipCount)
				{
					DrawMeshDataColumn(viewOffset, len,
						CustomBuildReport.MeshData.DataId.AnimationClipCount, "Animations",
						true, false, list, meshData,
						assetListToUse, ref _assetListScrollPos);
				}

				// -------------------------------

				if (_clickedMeshDataId != CustomBuildReport.MeshData.DataId.None)
				{
					ToggleSort(list, meshData, _clickedMeshDataId, filter);
					_clickedMeshDataId = CustomBuildReport.MeshData.DataId.None;
				}
			}

			#endregion

			// --------------------------------------------------------------------------------------------------------

			#region Columns: Prefab Data

			if (Event.current.type == EventType.Repaint)
			{
				_hoveredPrefabDataId = CustomBuildReport.PrefabData.DataId.None;
			}

			_clickedPrefabDataId = CustomBuildReport.PrefabData.DataId.None;

			if (showPrefabColumns)
			{
				if (CustomBuildReport.Options.ShowPrefabColumnContributeGI)
				{
					DrawPrefabDataColumn(viewOffset, len,
						CustomBuildReport.PrefabData.DataId.ContributeGI, "ContributeGI",
						true, false, list, prefabData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowPrefabColumnBatchingStatic)
				{
					DrawPrefabDataColumn(viewOffset, len,
						CustomBuildReport.PrefabData.DataId.BatchingStatic, "Static Batching",
						true, false, list, prefabData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowPrefabColumnOccluderStatic)
				{
					DrawPrefabDataColumn(viewOffset, len,
						CustomBuildReport.PrefabData.DataId.OccluderStatic, "Occluder Static",
						true, false, list, prefabData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowPrefabColumnOccludeeStatic)
				{
					DrawPrefabDataColumn(viewOffset, len,
						CustomBuildReport.PrefabData.DataId.OccludeeStatic, "Occludee Static",
						true, false, list, prefabData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowPrefabColumnReflectionProbeStatic)
				{
					DrawPrefabDataColumn(viewOffset, len,
						CustomBuildReport.PrefabData.DataId.ReflectionProbeStatic, "Reflection Probe Static",
						true, false, list, prefabData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowPrefabColumnNavigationStatic)
				{
					DrawPrefabDataColumn(viewOffset, len,
						CustomBuildReport.PrefabData.DataId.NavigationStatic, "Navigation Static",
						true, false, list, prefabData,
						assetListToUse, ref _assetListScrollPos);
				}
				if (CustomBuildReport.Options.ShowPrefabColumnOffMeshLinkGeneration)
				{
					DrawPrefabDataColumn(viewOffset, len,
						CustomBuildReport.PrefabData.DataId.OffMeshLinkGeneration, "Off-Mesh Link Generation",
						true, false, list, prefabData,
						assetListToUse, ref _assetListScrollPos);
				}

				// -------------------------------

				if (_clickedPrefabDataId != CustomBuildReport.PrefabData.DataId.None)
				{
					ToggleSort(list, prefabData, _clickedPrefabDataId, filter);
					_clickedPrefabDataId = CustomBuildReport.PrefabData.DataId.None;
				}
			}

			#endregion

			// --------------------------------------------------------------------------------------------------------

			#region Column: Raw File Size (Size Before Build)

			bool pressedRawSizeSortBtn = false;

			bool pressedSizeBeforeBuildSortBtn = false;

			if (IsShowingUsedAssets && CustomBuildReport.Options.ShowColumnSizeBeforeBuild && (assetListToUse[0].SizeInAssetsFolderBytes != -1))
			{
				pressedSizeBeforeBuildSortBtn = DrawColumn(viewOffset, len,
					CustomBuildReport.AssetList.SortType.SizeBeforeBuild, "Size Before Build   ", true, false,
					list, assetListToUse, (b) => b.SizeInAssetsFolder, ref _assetListScrollPos);
			}

			if (IsShowingUsedAssets && CustomBuildReport.Options.ShowColumnSizeInBuild && CustomBuildReport.Options.ShowImportedSizeForUsedAssets)
			{
				pressedRawSizeSortBtn = DrawColumn(viewOffset, len,
					CustomBuildReport.AssetList.SortType.ImportedSizeOrRawSize, "Size In Build", true, false,
					list, assetListToUse, (b) =>
					{
						// assets in the "StreamingAssets" folder do not have an imported size
						// in those cases, the raw size is the same as the imported size
						// so just use the raw size
						if (b.ImportedSize == "N/A")
						{
							return b.RawSize;
						}

						return b.ImportedSize;
					}, ref _assetListScrollPos);
			}

			if ((IsShowingUnusedAssets && CustomBuildReport.Options.ShowColumnUnusedRawSize) ||
			    (IsShowingUsedAssets && CustomBuildReport.Options.ShowColumnSizeInBuild && !CustomBuildReport.Options.ShowImportedSizeForUsedAssets))
			{
				pressedRawSizeSortBtn = DrawColumn(viewOffset, len, CustomBuildReport.AssetList.SortType.RawSize,
					(IsShowingUnusedAssets ? "Raw Size" : "Size In Build"), true, false,
					list, assetListToUse, (b) => b.RawSize, ref _assetListScrollPos);
			}

			#endregion

			bool showScrollbarForImportedSize = IsShowingUnusedAssets;

			// --------------------------------------------------------------------------------------------------------

			#region Column: Imported File Size

			bool pressedImpSizeSortBtn = false;

			if (IsShowingUnusedAssets && CustomBuildReport.Options.ShowColumnUnusedImportedSize)
			{
				pressedImpSizeSortBtn = DrawColumn(viewOffset, len, CustomBuildReport.AssetList.SortType.ImportedSize,
					"Imported Size   ", true, showScrollbarForImportedSize,
					list, assetListToUse, (b) => b.ImportedSize, ref _assetListScrollPos);
			}

			#endregion

			// --------------------------------------------------------------------------------------------------------

			#region Column: Percentage to Total Size

			bool pressedPercentSortBtn = false;

			if (IsShowingUsedAssets)
			{
				pressedPercentSortBtn = DrawColumn(viewOffset, len, CustomBuildReport.AssetList.SortType.PercentSize,
					"Percent   ", true, true,
					list, assetListToUse, (b) =>
					{
						string text = string.Format("{0}%", b.Percentage.ToString(CultureInfo.InvariantCulture));
						if (b.Percentage < 0)
						{
							text = Labels.NON_APPLICABLE_PERCENTAGE_LABEL;
						}

						return text;
					}, ref _assetListScrollPos);
			}

			#endregion

			// --------------------------------------------------------------------------------------------------------

			#region Handle Sort Change

			if (pressedRawSizeSortBtn)
			{
				var sortType = CustomBuildReport.AssetList.SortType.RawSize;
				if (IsShowingUsedAssets && CustomBuildReport.Options.ShowImportedSizeForUsedAssets)
				{
					sortType = CustomBuildReport.AssetList.SortType.ImportedSizeOrRawSize;
				}

				ToggleSort(list, sortType, filter);
			}
			else if (pressedSizeBeforeBuildSortBtn)
			{
				ToggleSort(list, CustomBuildReport.AssetList.SortType.SizeBeforeBuild, filter);
			}
			else if (pressedImpSizeSortBtn)
			{
				ToggleSort(list, CustomBuildReport.AssetList.SortType.ImportedSize, filter);
			}
			else if (pressedPercentSortBtn)
			{
				ToggleSort(list, CustomBuildReport.AssetList.SortType.PercentSize, filter);
			}

			#endregion

			GUILayout.EndHorizontal();
		}

		public static string GetPathColor(bool inSumSelect)
		{
			const string PATH_COLOR_UNSELECTED_WHITE_SKIN = "4f4f4fff";
			const string PATH_COLOR_SELECTED_WHITE_SKIN = "cececeff";

			const string PATH_COLOR_UNSELECTED_DARK_SKIN = "767676ff";
			const string PATH_COLOR_SELECTED_DARK_SKIN = "c2c2c2ff";

			string colorUsedForPath = PATH_COLOR_UNSELECTED_WHITE_SKIN;
			if (EditorGUIUtility.isProSkin || BRT_BuildReportWindow.FORCE_USE_DARK_SKIN)
			{
				colorUsedForPath = PATH_COLOR_UNSELECTED_DARK_SKIN;
			}

			if (inSumSelect)
			{
				if (EditorGUIUtility.isProSkin || BRT_BuildReportWindow.FORCE_USE_DARK_SKIN)
				{
					colorUsedForPath = PATH_COLOR_SELECTED_DARK_SKIN;
				}
				else
				{
					colorUsedForPath = PATH_COLOR_SELECTED_WHITE_SKIN;
				}
			}

			return colorUsedForPath;
		}


		delegate string ColumnDisplayDelegate(CustomBuildReport.SizePart b);

		bool DrawColumn(int sta, int end, CustomBuildReport.AssetList.SortType columnType, string columnName,
			bool allowSort,
			bool showScrollbar, CustomBuildReport.AssetList assetListCollection, CustomBuildReport.SizePart[] assetList,
			ColumnDisplayDelegate dataToDisplay, ref Vector2 scrollbarPos)
		{
			bool buttonPressed = false;

			var hiddenHorizontalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenHorizontalScrollbarStyle == null)
			{
				hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;
			}

			var hiddenVerticalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenVerticalScrollbarStyle == null)
			{
				hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;
			}

			var verticalScrollbarStyle = GUI.skin.verticalScrollbar;

			var listEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_STYLE_NAME);
			if (listEntryStyle == null)
			{
				listEntryStyle = GUI.skin.label;
			}

			var listAltEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_ALT_STYLE_NAME);
			if (listAltEntryStyle == null)
			{
				listAltEntryStyle = GUI.skin.label;
			}

			var listSelectedEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_SELECTED_NAME);
			if (listSelectedEntryStyle == null)
			{
				listSelectedEntryStyle = GUI.skin.label;
			}

			// ----------------------------------------------------------

			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

			// ----------------------------------------------------------
			// column header
			string sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_STYLE_NAME;
			if (allowSort && _currentSortType == columnType)
			{
				if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
				{
					sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_DESC_STYLE_NAME;
				}
				else
				{
					sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_ASC_STYLE_NAME;
				}
			}
			var sortTypeStyle = GUI.skin.FindStyle(sortTypeStyleName);
			if (sortTypeStyle == null)
			{
				sortTypeStyle = GUI.skin.label;
			}

			if (GUILayout.Button(columnName, sortTypeStyle, BRT_BuildReportWindow.LayoutListHeight) && allowSort)
			{
				buttonPressed = true;
			}


			// ----------------------------------------------------------
			// scrollbar
			if (showScrollbar)
			{
				scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
					hiddenHorizontalScrollbarStyle,
					verticalScrollbarStyle, BRT_BuildReportWindow.LayoutNone);
			}
			else
			{
				scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
					hiddenHorizontalScrollbarStyle,
					hiddenVerticalScrollbarStyle, BRT_BuildReportWindow.LayoutNone);
			}


			// ----------------------------------------------------------
			// actual contents
			bool useAlt = false;

			for (int n = sta; n < end; ++n)
			{
				var b = assetList[n];

				var styleToUse = useAlt ? listAltEntryStyle : listEntryStyle;
				if (assetListCollection.InSumSelection(b))
				{
					styleToUse = listSelectedEntryStyle;
				}

				GUILayout.Label(dataToDisplay(b), styleToUse, BRT_BuildReportWindow.LayoutListHeightMinWidth90);

				useAlt = !useAlt;
			}

			GUILayout.Space(SCROLLBAR_BOTTOM_PADDING);

			GUILayout.EndScrollView();

			GUILayout.EndVertical();

			return buttonPressed;
		}

		void DrawTextureDataColumn(int sta, int end, TextureData.DataId textureDataId, string columnName,
			bool allowSort, bool showScrollbar, CustomBuildReport.AssetList assetListCollection, TextureData textureData,
			SizePart[] assetList, ref Vector2 scrollbarPos)
		{
			var hiddenHorizontalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenHorizontalScrollbarStyle == null)
			{
				hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;
			}

			var hiddenVerticalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenVerticalScrollbarStyle == null)
			{
				hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;
			}

			var verticalScrollbarStyle = GUI.skin.verticalScrollbar;

			var listEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_STYLE_NAME);
			if (listEntryStyle == null)
			{
				listEntryStyle = GUI.skin.label;
			}

			var listAltEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_ALT_STYLE_NAME);
			if (listAltEntryStyle == null)
			{
				listAltEntryStyle = GUI.skin.label;
			}

			var listSelectedEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_SELECTED_NAME);
			if (listSelectedEntryStyle == null)
			{
				listSelectedEntryStyle = GUI.skin.label;
			}

			// ----------------------------------------------------------

			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

			// ----------------------------------------------------------
			// column header
			string sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_STYLE_NAME;
			if (allowSort && _currentSortType == CustomBuildReport.AssetList.SortType.TextureData && _currentTextureDataSortType == textureDataId)
			{
				if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
				{
					sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_DESC_STYLE_NAME;
				}
				else
				{
					sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_ASC_STYLE_NAME;
				}
			}
			var sortTypeStyle = GUI.skin.FindStyle(sortTypeStyleName);
			if (sortTypeStyle == null)
			{
				sortTypeStyle = GUI.skin.label;
			}

			if (GUILayout.Button(columnName, sortTypeStyle, BRT_BuildReportWindow.LayoutListHeight) && allowSort)
			{
				_clickedTextureDataId = textureDataId;
			}

			if (Event.current.type == EventType.Repaint)
			{
				var lastRect = GUILayoutUtility.GetLastRect();
				if (lastRect.Contains(Event.current.mousePosition))
				{
					_hoveredTextureDataId = textureDataId;
				}
			}

			// ----------------------------------------------------------
			// scrollbar
			if (showScrollbar)
			{
				scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
					hiddenHorizontalScrollbarStyle,
					verticalScrollbarStyle, BRT_BuildReportWindow.LayoutNone);
			}
			else
			{
				scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
					hiddenHorizontalScrollbarStyle,
					hiddenVerticalScrollbarStyle, BRT_BuildReportWindow.LayoutNone);
			}

			// ----------------------------------------------------------
			// actual contents
			bool useAlt = false;

			var textureDataEntries = textureData.GetTextureData();
			for (int n = sta; n < end; ++n)
			{
				var b = assetList[n];

				var styleToUse = useAlt ? listAltEntryStyle : listEntryStyle;
				if (assetListCollection.InSumSelection(b))
				{
					styleToUse = listSelectedEntryStyle;
				}

				var dataToDisplay = string.Empty;
				var assetHasTextureData = textureDataEntries.ContainsKey(b.Name);
				if (assetHasTextureData)
				{
					dataToDisplay = textureDataEntries[b.Name].ToDisplayedValue(textureDataId);
				}

				GUILayout.Label(dataToDisplay, styleToUse, BRT_BuildReportWindow.LayoutListHeightMinWidth90);
				if (assetHasTextureData && Event.current.type == EventType.Repaint && textureDataEntries[b.Name].IsOverriden(textureDataId))
				{
					var lastRect = GUILayoutUtility.GetLastRect();
					if (lastRect.Contains(Event.current.mousePosition))
					{
						_overridenTextureDataTooltipText = textureDataEntries[b.Name].GetOverridingMessage(textureDataId);
					}
				}

				useAlt = !useAlt;
			}

			GUILayout.Space(SCROLLBAR_BOTTOM_PADDING);

			GUILayout.EndScrollView();

			GUILayout.EndVertical();
		}

		void DrawMeshDataColumn(int sta, int end, MeshData.DataId meshDataId, string columnName,
			bool allowSort, bool showScrollbar, CustomBuildReport.AssetList assetListCollection, MeshData meshData,
			SizePart[] assetList, ref Vector2 scrollbarPos)
		{
			var hiddenHorizontalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenHorizontalScrollbarStyle == null)
			{
				hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;
			}

			var hiddenVerticalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenVerticalScrollbarStyle == null)
			{
				hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;
			}

			var verticalScrollbarStyle = GUI.skin.verticalScrollbar;

			var listEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_STYLE_NAME);
			if (listEntryStyle == null)
			{
				listEntryStyle = GUI.skin.label;
			}

			var listAltEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_ALT_STYLE_NAME);
			if (listAltEntryStyle == null)
			{
				listAltEntryStyle = GUI.skin.label;
			}

			var listSelectedEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_SELECTED_NAME);
			if (listSelectedEntryStyle == null)
			{
				listSelectedEntryStyle = GUI.skin.label;
			}

			// ----------------------------------------------------------

			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

			// ----------------------------------------------------------
			// column header
			string sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_STYLE_NAME;
			if (allowSort && _currentSortType == CustomBuildReport.AssetList.SortType.MeshData && _currentMeshDataSortType == meshDataId)
			{
				if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
				{
					sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_DESC_STYLE_NAME;
				}
				else
				{
					sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_ASC_STYLE_NAME;
				}
			}
			var sortTypeStyle = GUI.skin.FindStyle(sortTypeStyleName);
			if (sortTypeStyle == null)
			{
				sortTypeStyle = GUI.skin.label;
			}

			if (GUILayout.Button(columnName, sortTypeStyle, BRT_BuildReportWindow.LayoutListHeight) && allowSort)
			{
				_clickedMeshDataId = meshDataId;
			}

			if (Event.current.type == EventType.Repaint)
			{
				var lastRect = GUILayoutUtility.GetLastRect();
				if (lastRect.Contains(Event.current.mousePosition))
				{
					_hoveredMeshDataId = meshDataId;
				}
			}

			// ----------------------------------------------------------
			// scrollbar
			if (showScrollbar)
			{
				scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
					hiddenHorizontalScrollbarStyle,
					verticalScrollbarStyle, BRT_BuildReportWindow.LayoutNone);
			}
			else
			{
				scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
					hiddenHorizontalScrollbarStyle,
					hiddenVerticalScrollbarStyle, BRT_BuildReportWindow.LayoutNone);
			}

			// ----------------------------------------------------------
			// actual contents
			bool useAlt = false;

			var meshDataEntries = meshData.GetMeshData();
			for (int n = sta; n < end; ++n)
			{
				var b = assetList[n];

				var styleToUse = useAlt
					                    ? listAltEntryStyle
					                    : listEntryStyle;
				if (assetListCollection.InSumSelection(b))
				{
					styleToUse = listSelectedEntryStyle;
				}

				var dataToDisplay = string.Empty;
				var assetHasMeshData = meshDataEntries.ContainsKey(b.Name);
				if (assetHasMeshData)
				{
					dataToDisplay = meshDataEntries[b.Name].ToDisplayedValue(meshDataId);
				}

				GUILayout.Label(dataToDisplay, styleToUse, BRT_BuildReportWindow.LayoutListHeightMinWidth90);

				useAlt = !useAlt;
			}

			GUILayout.Space(SCROLLBAR_BOTTOM_PADDING);

			GUILayout.EndScrollView();

			GUILayout.EndVertical();
		}

		void DrawPrefabDataColumn(int sta, int end, PrefabData.DataId prefabDataId, string columnName,
			bool allowSort, bool showScrollbar, CustomBuildReport.AssetList assetListCollection, PrefabData prefabData,
			SizePart[] assetList, ref Vector2 scrollbarPos)
		{
			var hiddenHorizontalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenHorizontalScrollbarStyle == null)
			{
				hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;
			}

			var hiddenVerticalScrollbarStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
			if (hiddenVerticalScrollbarStyle == null)
			{
				hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;
			}

			var verticalScrollbarStyle = GUI.skin.verticalScrollbar;

			var listEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_STYLE_NAME);
			if (listEntryStyle == null)
			{
				listEntryStyle = GUI.skin.label;
			}

			var listAltEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_ALT_STYLE_NAME);
			if (listAltEntryStyle == null)
			{
				listAltEntryStyle = GUI.skin.label;
			}

			var listSelectedEntryStyle = GUI.skin.FindStyle(CustomBuildReport.Window.Settings.LIST_SMALL_SELECTED_NAME);
			if (listSelectedEntryStyle == null)
			{
				listSelectedEntryStyle = GUI.skin.label;
			}

			// ----------------------------------------------------------

			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

			// ----------------------------------------------------------
			// column header
			string sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_STYLE_NAME;
			if (allowSort && _currentSortType == CustomBuildReport.AssetList.SortType.PrefabData && _currentPrefabDataSortType == prefabDataId)
			{
				if (_currentSortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
				{
					sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_DESC_STYLE_NAME;
				}
				else
				{
					sortTypeStyleName = CustomBuildReport.Window.Settings.LIST_COLUMN_HEADER_ASC_STYLE_NAME;
				}
			}
			var sortTypeStyle = GUI.skin.FindStyle(sortTypeStyleName);
			if (sortTypeStyle == null)
			{
				sortTypeStyle = GUI.skin.label;
			}

			if (GUILayout.Button(columnName, sortTypeStyle, BRT_BuildReportWindow.LayoutListHeight) && allowSort)
			{
				_clickedPrefabDataId = prefabDataId;
			}

			if (Event.current.type == EventType.Repaint)
			{
				var lastRect = GUILayoutUtility.GetLastRect();
				if (lastRect.Contains(Event.current.mousePosition))
				{
					_hoveredPrefabDataId = prefabDataId;
				}
			}

			// ----------------------------------------------------------
			// scrollbar
			if (showScrollbar)
			{
				scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
					hiddenHorizontalScrollbarStyle,
					verticalScrollbarStyle, BRT_BuildReportWindow.LayoutNone);
			}
			else
			{
				scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
					hiddenHorizontalScrollbarStyle,
					hiddenVerticalScrollbarStyle, BRT_BuildReportWindow.LayoutNone);
			}

			// ----------------------------------------------------------
			// actual contents
			bool useAlt = false;

			var prefabDataEntries = prefabData.GetPrefabData();
			for (int n = sta; n < end; ++n)
			{
				var b = assetList[n];

				var styleToUse = useAlt
					                    ? listAltEntryStyle
					                    : listEntryStyle;
				if (assetListCollection.InSumSelection(b))
				{
					styleToUse = listSelectedEntryStyle;
				}

				var dataToDisplay = string.Empty;
				var assetHasMeshData = prefabDataEntries.ContainsKey(b.Name);
				if (assetHasMeshData)
				{
					dataToDisplay = prefabDataEntries[b.Name].HasValue(prefabDataId);
				}

				GUILayout.Label(dataToDisplay, styleToUse, BRT_BuildReportWindow.LayoutListHeightMinWidth90);

				useAlt = !useAlt;
			}

			GUILayout.Space(SCROLLBAR_BOTTOM_PADDING);

			GUILayout.EndScrollView();

			GUILayout.EndVertical();
		}
	}
}