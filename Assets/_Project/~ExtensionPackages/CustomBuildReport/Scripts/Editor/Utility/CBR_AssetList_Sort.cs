using System;

namespace CustomBuildReport
{
	public static partial class AssetListUtility
	{
		public static void SortAssetList(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortType sortType, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			switch (sortType)
			{
				case CustomBuildReport.AssetList.SortType.RawSize:
					SortRawSize(assetList, sortOrder);
					break;
				case CustomBuildReport.AssetList.SortType.ImportedSize:
					SortImportedSize(assetList, sortOrder);
					break;
				case CustomBuildReport.AssetList.SortType.ImportedSizeOrRawSize:
					SortImportedSizeOrRawSize(assetList, sortOrder);
					break;
				case CustomBuildReport.AssetList.SortType.SizeBeforeBuild:
					SortSizeBeforeBuild(assetList, sortOrder);
					break;
				case CustomBuildReport.AssetList.SortType.PercentSize:
					SortPercentSize(assetList, sortOrder);
					break;
				case CustomBuildReport.AssetList.SortType.AssetFullPath:
					SortAssetFullPath(assetList, sortOrder);
					break;
				case CustomBuildReport.AssetList.SortType.AssetFilename:
					SortAssetName(assetList, sortOrder);
					break;
			}
		}

		static void SortRawSize(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.UsableSize > entry2.UsableSize) return -1;
					if (entry1.UsableSize < entry2.UsableSize) return 1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameDescending(entry1, entry2);
				});
			}
			else
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.UsableSize > entry2.UsableSize) return 1;
					if (entry1.UsableSize < entry2.UsableSize) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameAscending(entry1, entry2);
				});
			}
		}

		static void SortImportedSizeOrRawSize(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.ImportedSizeOrRawSize > entry2.ImportedSizeOrRawSize) return -1;
					if (entry1.ImportedSizeOrRawSize < entry2.ImportedSizeOrRawSize) return 1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameDescending(entry1, entry2);
				});
			}
			else
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.ImportedSizeOrRawSize > entry2.ImportedSizeOrRawSize) return 1;
					if (entry1.ImportedSizeOrRawSize < entry2.ImportedSizeOrRawSize) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameAscending(entry1, entry2);
				});
			}
		}

		static void SortImportedSize(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.ImportedSizeBytes > entry2.ImportedSizeBytes) return -1;
					if (entry1.ImportedSizeBytes < entry2.ImportedSizeBytes) return 1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameDescending(entry1, entry2);
				});
			}
			else
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.ImportedSizeBytes > entry2.ImportedSizeBytes) return 1;
					if (entry1.ImportedSizeBytes < entry2.ImportedSizeBytes) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameAscending(entry1, entry2);
				});
			}
		}

		static void SortSizeBeforeBuild(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.SizeInAssetsFolderBytes > entry2.SizeInAssetsFolderBytes) return -1;
					if (entry1.SizeInAssetsFolderBytes < entry2.SizeInAssetsFolderBytes) return 1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameDescending(entry1, entry2);
				});
			}
			else
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.SizeInAssetsFolderBytes > entry2.SizeInAssetsFolderBytes) return 1;
					if (entry1.SizeInAssetsFolderBytes < entry2.SizeInAssetsFolderBytes) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameAscending(entry1, entry2);
				});
			}
		}

		static void SortPercentSize(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.Percentage > entry2.Percentage) return -1;
					if (entry1.Percentage < entry2.Percentage) return 1;

					// same percent
					// sort by asset name for assets with same percent
					return SortByAssetFullPathDescending(entry1, entry2);
				});
			}
			else
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					if (entry1.Percentage > entry2.Percentage) return 1;
					if (entry1.Percentage < entry2.Percentage) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetFullPathAscending(entry1, entry2);
				});
			}
		}

		static void SortAssetFullPath(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, SortByAssetFullPathDescending);
			}
			else
			{
				Array.Sort(assetList, SortByAssetFullPathAscending);
			}
		}

		static int SortByAssetFullPathDescending(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
		{
			int result = string.Compare(entry1.Name, entry2.Name, StringComparison.OrdinalIgnoreCase);

			return result;
		}

		static int SortByAssetFullPathAscending(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
		{
			int result = string.Compare(entry1.Name, entry2.Name, StringComparison.OrdinalIgnoreCase);

			// invert the result
			if (result == 1) return -1;
			if (result == -1) return 1;
			return 0;
		}

		static void SortAssetName(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, SortByAssetNameDescending);
			}
			else
			{
				Array.Sort(assetList, SortByAssetNameAscending);
			}
		}

		static int SortByAssetNameDescending(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
		{
			int result = string.Compare(entry1.Name.GetFileNameOnly(), entry2.Name.GetFileNameOnly(),
				StringComparison.OrdinalIgnoreCase);

			return result;
		}

		static int SortByAssetNameAscending(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
		{
			int result = string.Compare(entry1.Name.GetFileNameOnly(), entry2.Name.GetFileNameOnly(),
				StringComparison.OrdinalIgnoreCase);

			// invert the result
			if (result == 1) return -1;
			if (result == -1) return 1;

			return 0;
		}
	}
}