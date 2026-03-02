using System;

namespace CustomBuildReport
{
	public static partial class AssetListUtility
	{
		public static void SortAssetList(CustomBuildReport.SizePart[] assetList, CustomBuildReport.TextureData textureData, TextureData.DataId textureSortType, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			switch (textureSortType)
			{
				case CustomBuildReport.TextureData.DataId.TextureType:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.TextureType);
					break;
				case CustomBuildReport.TextureData.DataId.IsSRGB:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.IsSRGB);
					break;
				case CustomBuildReport.TextureData.DataId.AlphaSource:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.AlphaSource);
					break;
				case CustomBuildReport.TextureData.DataId.AlphaIsTransparency:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.AlphaIsTransparency);
					break;
				case CustomBuildReport.TextureData.DataId.IgnorePngGamma:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.IgnorePngGamma);
					break;
				case CustomBuildReport.TextureData.DataId.NPotScale:
					SortNPotScale(assetList, textureData, sortOrder);
					break;
				case CustomBuildReport.TextureData.DataId.IsReadable:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.IsReadable);
					break;
				case CustomBuildReport.TextureData.DataId.MipMapGenerated:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.MipMapGenerated);
					break;
				case CustomBuildReport.TextureData.DataId.MipMapFilter:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.MipMapFilter);
					break;
				case CustomBuildReport.TextureData.DataId.StreamingMipMaps:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.StreamingMipMaps);
					break;
				case CustomBuildReport.TextureData.DataId.BorderMipMaps:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.BorderMipMaps);
					break;
				case CustomBuildReport.TextureData.DataId.PreserveCoverageMipMaps:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.PreserveCoverageMipMaps);
					break;
				case CustomBuildReport.TextureData.DataId.FadeOutMipMaps:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.FadeOutMipMaps);
					break;
				case CustomBuildReport.TextureData.DataId.SpriteImportMode:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.SpriteImportMode);
					break;
				case CustomBuildReport.TextureData.DataId.SpritePackingTag:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.SpritePackingTag);
					break;
				case CustomBuildReport.TextureData.DataId.SpritePixelsPerUnit:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.SpritePixelsPerUnit);
					break;
				case CustomBuildReport.TextureData.DataId.QualifiesForSpritePacking:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.QualifiesForSpritePacking);
					break;
				case CustomBuildReport.TextureData.DataId.WrapMode:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapMode);
					break;
				case CustomBuildReport.TextureData.DataId.WrapModeU:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapModeU);
					break;
				case CustomBuildReport.TextureData.DataId.WrapModeV:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapModeV);
					break;
				case CustomBuildReport.TextureData.DataId.WrapModeW:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapModeW);
					break;
				case CustomBuildReport.TextureData.DataId.FilterMode:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.FilterMode);
					break;
				case CustomBuildReport.TextureData.DataId.AnisoLevel:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.AnisoLevel);
					break;
				case CustomBuildReport.TextureData.DataId.MaxTextureSize:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownMaxTextureSize());
					break;
				case CustomBuildReport.TextureData.DataId.TextureResizeAlgorithm:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownTextureResizeAlgorithm());
					break;
				case CustomBuildReport.TextureData.DataId.TextureFormat:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownTextureFormat());
					break;
				case CustomBuildReport.TextureData.DataId.CompressionType:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownCompressionType());
					break;
				case CustomBuildReport.TextureData.DataId.CompressionIsCrunched:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownCompressionIsCrunched());
					break;
				case CustomBuildReport.TextureData.DataId.CompressionQuality:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownCompressionQuality());
					break;
				case CustomBuildReport.TextureData.DataId.ImportedWidthAndHeight:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.GetImportedPixelCount());
					break;
				case CustomBuildReport.TextureData.DataId.RealWidthAndHeight:
					SortTextureData(assetList, textureData, sortOrder, entry => entry.GetRealPixelCount());
					break;
			}
		}

		static int CompareNPotScale(string nPotScale1, string nPotScale2)
		{
			var nPotScale1IsNoneNot = nPotScale1 == CustomBuildReport.TextureData.NPOT_SCALE_NONE_NOT_POT;
			var nPotScale2IsNoneNot = nPotScale1 == CustomBuildReport.TextureData.NPOT_SCALE_NONE_NOT_POT;

			if (nPotScale1IsNoneNot && !nPotScale2IsNoneNot)
			{
				return -1;
			}
			if (!nPotScale1IsNoneNot && nPotScale2IsNoneNot)
			{
				return 1;
			}

			return string.Compare(nPotScale1, nPotScale2, StringComparison.Ordinal);
		}

		// =============================================================================================================

		static void SortByInt(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					int sortResult = entry2.GetIntAuxData().CompareTo(entry1.GetIntAuxData());
					if (sortResult != 0)
					{
						return sortResult;
					}

					// same texture data
					// sort by asset size for assets with texture data
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
					int sortResult = entry1.GetIntAuxData().CompareTo(entry2.GetIntAuxData());
					if (sortResult != 0)
					{
						return sortResult;
					}

					// same texture data
					// sort by asset size for assets with same texture data
					if (entry1.UsableSize > entry2.UsableSize) return 1;
					if (entry1.UsableSize < entry2.UsableSize) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameAscending(entry1, entry2);
				});
			}
		}

		static void SortByFloat(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					int sortResult = entry1.GetFloatAuxData().CompareTo(entry2.GetFloatAuxData());
					if (sortResult != 0)
					{
						return sortResult;
					}

					// same texture data
					// sort by asset size for assets with texture data
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
					int sortResult = entry2.GetFloatAuxData().CompareTo(entry1.GetFloatAuxData());
					if (sortResult != 0)
					{
						return sortResult;
					}

					// same texture data
					// sort by asset size for assets with same texture data
					if (entry1.UsableSize > entry2.UsableSize) return 1;
					if (entry1.UsableSize < entry2.UsableSize) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameAscending(entry1, entry2);
				});
			}
		}

		static void SortByText(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					int sortTextureTypeResult = string.Compare(entry1.GetTextAuxData(), entry2.GetTextAuxData(), StringComparison.OrdinalIgnoreCase);
					if (sortTextureTypeResult != 0)
					{
						return sortTextureTypeResult;
					}

					// same texture type
					// sort by asset size for assets with same texture types
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
					int sortTextureTypeResult = string.Compare(entry2.GetTextAuxData(), entry1.GetTextAuxData(), StringComparison.OrdinalIgnoreCase);
					if (sortTextureTypeResult != 0)
					{
						return sortTextureTypeResult;
					}

					// same texture type
					// sort by asset size for assets with same texture types
					if (entry1.UsableSize > entry2.UsableSize) return 1;
					if (entry1.UsableSize < entry2.UsableSize) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameAscending(entry1, entry2);
				});
			}
		}

		static void SortByText(CustomBuildReport.SizePart[] assetList, CustomBuildReport.AssetList.SortOrder sortOrder, Func<string, string, int> compare)
		{
			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					int sortTextureTypeResult = compare(entry1.GetTextAuxData(), entry2.GetTextAuxData());
					if (sortTextureTypeResult != 0)
					{
						return sortTextureTypeResult;
					}

					// same texture type
					// sort by asset size for assets with same texture types
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
					int sortTextureTypeResult = compare(entry2.GetTextAuxData(), entry1.GetTextAuxData());
					if (sortTextureTypeResult != 0)
					{
						return sortTextureTypeResult;
					}

					// same texture type
					// sort by asset size for assets with same texture types
					if (entry1.UsableSize > entry2.UsableSize) return 1;
					if (entry1.UsableSize < entry2.UsableSize) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameAscending(entry1, entry2);
				});
			}
		}

		// =============================================================================================================

		static void SortTextureData(CustomBuildReport.SizePart[] assetList, CustomBuildReport.TextureData textureData, CustomBuildReport.AssetList.SortOrder sortOrder, Func<CustomBuildReport.TextureData.Entry, bool> func)
		{
			var textureEntries = textureData.GetTextureData();

			for (int n = 0; n < assetList.Length; ++n)
			{
				int boolValue = 0;
				if (textureEntries.ContainsKey(assetList[n].Name))
				{
					boolValue = func(textureEntries[assetList[n].Name]) ? 1 : 0;
				}

				assetList[n].SetIntAuxData(boolValue);
			}

			SortByInt(assetList, sortOrder);
		}

		static void SortTextureData(CustomBuildReport.SizePart[] assetList, CustomBuildReport.TextureData textureData, CustomBuildReport.AssetList.SortOrder sortOrder, Func<CustomBuildReport.TextureData.Entry, string> func)
		{
			var textureEntries = textureData.GetTextureData();

			for (int n = 0; n < assetList.Length; ++n)
			{
				string textData = null;
				if (textureEntries.ContainsKey(assetList[n].Name))
				{
					textData = func(textureEntries[assetList[n].Name]);
				}

				assetList[n].SetTextAuxData(textData);
			}

			SortByText(assetList, sortOrder);
		}

		static void SortTextureData(CustomBuildReport.SizePart[] assetList, CustomBuildReport.TextureData textureData, CustomBuildReport.AssetList.SortOrder sortOrder, Func<CustomBuildReport.TextureData.Entry, float> func)
		{
			var textureEntries = textureData.GetTextureData();

			for (int n = 0; n < assetList.Length; ++n)
			{
				float floatValue = 0;
				if (textureEntries.ContainsKey(assetList[n].Name))
				{
					floatValue = func(textureEntries[assetList[n].Name]);
				}

				assetList[n].SetFloatAuxData(floatValue);
			}

			SortByFloat(assetList, sortOrder);
		}

		static void SortTextureData(CustomBuildReport.SizePart[] assetList, CustomBuildReport.TextureData textureData, CustomBuildReport.AssetList.SortOrder sortOrder, Func<CustomBuildReport.TextureData.Entry, int> func)
		{
			var textureEntries = textureData.GetTextureData();

			for (int n = 0; n < assetList.Length; ++n)
			{
				int intValue = 0;
				if (textureEntries.ContainsKey(assetList[n].Name))
				{
					intValue = func(textureEntries[assetList[n].Name]);
				}

				assetList[n].SetIntAuxData(intValue);
			}

			SortByInt(assetList, sortOrder);
		}

		// NPotScale sort is special: we want the "None (Not Power of 2)" values to go at top, ignoring alphabetical order for that special value
		static void SortNPotScale(CustomBuildReport.SizePart[] assetList, CustomBuildReport.TextureData textureData, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			var textureEntries = textureData.GetTextureData();

			for (int n = 0; n < assetList.Length; ++n)
			{
				string textData = null;
				if (textureEntries.ContainsKey(assetList[n].Name))
				{
					textData = textureEntries[assetList[n].Name].NPotScale;
				}

				assetList[n].SetTextAuxData(textData);
			}

			if (sortOrder == CustomBuildReport.AssetList.SortOrder.Descending)
			{
				Array.Sort(assetList, delegate(CustomBuildReport.SizePart entry1, CustomBuildReport.SizePart entry2)
				{
					string nPotScale1 = entry1.GetTextAuxData();
					string nPotScale2 = entry2.GetTextAuxData();

					// put non-power-of-2 at top
					bool nPotScale1IsNoneNot = nPotScale1 == CustomBuildReport.TextureData.NPOT_SCALE_NONE_NOT_POT;
					bool nPotScale2IsNoneNot = nPotScale2 == CustomBuildReport.TextureData.NPOT_SCALE_NONE_NOT_POT;
					if (nPotScale1IsNoneNot && !nPotScale2IsNoneNot)
					{
						return -1;
					}
					if (!nPotScale1IsNoneNot && nPotScale2IsNoneNot)
					{
						return 1;
					}

					// at this point, entry1 and entry 2 are not non-power-of-2 (or both of them are), so compare them as usual
					int sortTextureTypeResult = string.Compare(nPotScale1, nPotScale2, StringComparison.OrdinalIgnoreCase);
					if (sortTextureTypeResult != 0)
					{
						return sortTextureTypeResult;
					}

					// same nPotScale type
					// sort by asset size for assets with same nPotScale types
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
					string nPotScale1 = entry1.GetTextAuxData();
					string nPotScale2 = entry2.GetTextAuxData();

					// put non-power-of-2 at bottom
					bool nPotScale1IsNoneNot = nPotScale1 == CustomBuildReport.TextureData.NPOT_SCALE_NONE_NOT_POT;
					bool nPotScale2IsNoneNot = nPotScale2 == CustomBuildReport.TextureData.NPOT_SCALE_NONE_NOT_POT;
					if (nPotScale1IsNoneNot && !nPotScale2IsNoneNot)
					{
						return 1;
					}
					if (!nPotScale1IsNoneNot && nPotScale2IsNoneNot)
					{
						return -1;
					}

					// at this point, entry1 and entry 2 are not non-power-of-2 (or both of them are), so compare them as usual
					int sortTextureTypeResult = string.Compare(nPotScale2, nPotScale1, StringComparison.OrdinalIgnoreCase);
					if (sortTextureTypeResult != 0)
					{
						return sortTextureTypeResult;
					}

					// same nPotScale type
					// sort by asset size for assets with same nPotScale types
					if (entry1.UsableSize > entry2.UsableSize) return 1;
					if (entry1.UsableSize < entry2.UsableSize) return -1;

					// same size
					// sort by asset name for assets with same sizes
					return SortByAssetNameAscending(entry1, entry2);
				});
			}
		}
	}
}