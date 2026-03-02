namespace CustomBuildReport
{
	[System.Serializable]
	public class BundleEntry
	{
		public string Name;
		public string TotalOutputSize = "";
		public string TotalUserAssetsSize = "";
		public CustomBuildReport.SizePart[] BuildSizes;
		public AssetList UsedAssets;
	}
}
