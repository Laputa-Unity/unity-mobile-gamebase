using System;

namespace CustomBuildReport
{
	public static partial class AssetListUtility
	{
		public static void SortAssetList(CustomBuildReport.SizePart[] assetList, CustomBuildReport.MeshData meshData, MeshData.DataId meshSortType, CustomBuildReport.AssetList.SortOrder sortOrder)
		{
			switch (meshSortType)
			{
				case CustomBuildReport.MeshData.DataId.MeshFilterCount:
					SortMeshData(assetList, meshData, sortOrder, entry => entry.MeshFilterCount);
					break;
				case CustomBuildReport.MeshData.DataId.SkinnedMeshRendererCount:
					SortMeshData(assetList, meshData, sortOrder, entry => entry.SkinnedMeshRendererCount);
					break;
				case CustomBuildReport.MeshData.DataId.SubMeshCount:
					SortMeshData(assetList, meshData, sortOrder, entry => entry.SubMeshCount);
					break;
				case CustomBuildReport.MeshData.DataId.VertexCount:
					SortMeshData(assetList, meshData, sortOrder, entry => entry.VertexCount);
					break;
				case CustomBuildReport.MeshData.DataId.TriangleCount:
					SortMeshData(assetList, meshData, sortOrder, entry => entry.TriangleCount);
					break;
				case CustomBuildReport.MeshData.DataId.AnimationType:
					SortMeshData(assetList, meshData, sortOrder, entry => entry.AnimationType);
					break;
				case CustomBuildReport.MeshData.DataId.AnimationClipCount:
					SortMeshData(assetList, meshData, sortOrder, entry => entry.AnimationClipCount);
					break;
			}
		}

		static void SortMeshData(CustomBuildReport.SizePart[] assetList, CustomBuildReport.MeshData meshData, CustomBuildReport.AssetList.SortOrder sortOrder, Func<CustomBuildReport.MeshData.Entry, int> func)
		{
			var meshEntries = meshData.GetMeshData();

			for (int n = 0; n < assetList.Length; ++n)
			{
				int intValue = 0;
				if (meshEntries.ContainsKey(assetList[n].Name))
				{
					intValue = func(meshEntries[assetList[n].Name]);
				}

				assetList[n].SetIntAuxData(intValue);
			}

			SortByInt(assetList, sortOrder);
		}

		static void SortMeshData(CustomBuildReport.SizePart[] assetList, CustomBuildReport.MeshData meshData, CustomBuildReport.AssetList.SortOrder sortOrder, Func<CustomBuildReport.MeshData.Entry, string> func)
		{
			var meshEntries = meshData.GetMeshData();

			for (int n = 0; n < assetList.Length; ++n)
			{
				string textData = null;
				if (meshEntries.ContainsKey(assetList[n].Name))
				{
					textData = func(meshEntries[assetList[n].Name]);
				}

				assetList[n].SetTextAuxData(textData);
			}

			SortByText(assetList, sortOrder);
		}
	}
}