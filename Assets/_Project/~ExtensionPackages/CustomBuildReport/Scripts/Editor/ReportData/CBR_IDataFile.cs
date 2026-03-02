
namespace CustomBuildReport
{
	public interface IDataFile
	{
		void OnBeforeSave();
		void OnAfterLoad();
		void SetSavedPath(string savedPath);
		string SavedPath { get; }
		string GetDefaultFilename();
	}
}