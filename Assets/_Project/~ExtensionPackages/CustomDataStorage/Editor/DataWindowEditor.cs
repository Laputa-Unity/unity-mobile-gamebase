using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Custom.DataStorage
{
#if UNITY_EDITOR
    public class DataWindowEditor : EditorWindow
    {
        [MenuItem("GameBase/~Custom Data/Clear Data")]
        public static void ClearData()
        {
            GameData.DeleteAll();
            GameData.DeleteFileData();
            PlayerPrefs.DeleteAll();
            Debug.Log($"<color=Green>Clear all data succeed</color>");
        }

        [MenuItem("GameBase/~Custom Data/Save Data")]
        public static void SaveData()
        {
            GameData.Save();
            Debug.Log($"<color=Green>Save data succeed</color>");
        }

        [MenuItem("GameBase/~Custom Data/Open Data Path")]
        public static void OpenPathData()
        {
            string path = GameData.GetPersistentDataPath();
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                    if (Directory.Exists(path))
                    {
                        Process.Start(path);
                    }
                    else
                    {
                        Debug.LogError("The directory does not exist: " + path);
                    }

                    break;
                case OperatingSystemFamily.MacOSX:
                    if (Directory.Exists(path))
                    {
                        Process.Start("open", path);
                    }
                    else
                    {
                        Debug.LogError("The directory does not exist: " + path);
                    }

                    break;
            }
        }
    }
#endif
}