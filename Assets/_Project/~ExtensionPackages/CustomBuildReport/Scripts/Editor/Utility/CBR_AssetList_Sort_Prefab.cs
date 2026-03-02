using System;

namespace CustomBuildReport
{
	public static partial class AssetListUtility
	{
		public static void SortAssetList(CustomBuildReport.SizePart[] assetList, CustomBuildReport.PrefabData prefabData, PrefabData.DataId prefabSortType, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			switch (prefabSortType)
			{
				case CustomBuildReport.PrefabData.DataId.ContributeGI:
					SortPrefabData(assetList, prefabData, sortOrder, entry => entry.ContributeGIValue);
					break;
				case CustomBuildReport.PrefabData.DataId.BatchingStatic:
					SortPrefabData(assetList, prefabData, sortOrder, entry => entry.BatchingStaticValue);
					break;
				case CustomBuildReport.PrefabData.DataId.ReflectionProbeStatic:
					SortPrefabData(assetList, prefabData, sortOrder, entry => entry.ReflectionProbeStaticValue);
					break;
				case CustomBuildReport.PrefabData.DataId.OccluderStatic:
					SortPrefabData(assetList, prefabData, sortOrder, entry => entry.OccluderStaticValue);
					break;
				case CustomBuildReport.PrefabData.DataId.OccludeeStatic:
					SortPrefabData(assetList, prefabData, sortOrder, entry => entry.OccludeeStaticValue);
					break;
				case CustomBuildReport.PrefabData.DataId.NavigationStatic:
					SortPrefabData(assetList, prefabData, sortOrder, entry => entry.NavigationStaticValue);
					break;
				case CustomBuildReport.PrefabData.DataId.OffMeshLinkGeneration:
					SortPrefabData(assetList, prefabData, sortOrder, entry => entry.OffMeshLinkGenerationValue);
					break;
			}
		}

		static void SortPrefabData(CustomBuildReport.SizePart[] assetList, CustomBuildReport.PrefabData prefabData, CustomBuildReport.AssetList.SortOrder sortOrder, Func<CustomBuildReport.PrefabData.Entry, int> func)
		{
			var prefabEntries = prefabData.GetPrefabData();

			for (int n = 0; n < assetList.Length; ++n)
			{
				int intValue = 0;
				if (prefabEntries.ContainsKey(assetList[n].Name))
				{
					intValue = func(prefabEntries[assetList[n].Name]);
				}

				assetList[n].SetIntAuxData(intValue);
			}

			SortByInt(assetList, sortOrder);
		}
	}
}