using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class LevelEditorWindow : EditorWindow
{
    // Sidebar (left)
    private Vector2 _levelScrollPosition;
    private readonly List<string> _levelPrefabPaths = new List<string>();
    private string _selectedLevelPath = "";
    private GameObject _selectedLevelPrefab;
    private LevelEditorSetting _setting;

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow() => GetWindow<LevelEditorWindow>("Level Editor");

    private void OnEnable()
    {
        _setting = Resources.Load("LevelEditorSetting") as LevelEditorSetting;
        if (_setting != null) minSize = _setting.windowMinSize;
        
        LoadLevelPrefabs();
    }

    private void OnGUI()
    {
        // rời Prefab Mode thì clear chọn
        if (!IsInPrefabMode() && !string.IsNullOrEmpty(_selectedLevelPath))
        {
            _selectedLevelPath = "";
            _selectedLevelPrefab = null;
            Repaint();
        }

        DrawTitleBar();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();

        // LEFT: level list + play
        DrawLevelPrefabsScrollView();

        EditorGUILayout.EndHorizontal();
    }

    private bool IsInPrefabMode() => PrefabStageUtility.GetCurrentPrefabStage() != null;

    private void DrawTitleBar()
    {
        // Top row: Refresh
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (GUILayout.Button("Refresh", GUILayout.Height(24)))
        {
            LoadLevelPrefabs();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawLevelPrefabsScrollView()
    {
        // Sidebar fixed width
        EditorGUILayout.BeginVertical(GUILayout.Width(220));

        _levelScrollPosition = EditorGUILayout.BeginScrollView(_levelScrollPosition);

        GUILayout.Label("Level Prefabs", new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter
        });

        foreach (string prefabPath in _levelPrefabPaths)
        {
            string prefabName = Path.GetFileNameWithoutExtension(prefabPath);

            GUI.backgroundColor = prefabPath == _selectedLevelPath ? Color.green : Color.white;

            bool isSelected = prefabPath == _selectedLevelPath;
            bool newSelected = GUILayout.Toggle(
                isSelected,
                prefabName,
                new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft });

            if (newSelected && prefabPath != _selectedLevelPath)
            {
                _selectedLevelPath = prefabPath;
                OpenLevelPrefab(_selectedLevelPath);
            }

            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(8);

        using (new EditorGUI.DisabledScope(_selectedLevelPrefab == null))
        {
            if (GUILayout.Button("Play This Level", GUILayout.Height(36)))
            {
                var level = _selectedLevelPrefab.GetComponent<Level>();
                if (level != null) level.PlayThisLevel();
                else Debug.LogWarning("Selected prefab does not have a Level component.");
            }
        }

        EditorGUILayout.EndVertical();
    }
    
    private void LoadLevelPrefabs()
    {
        _levelPrefabPaths.Clear();

        if (_setting == null || string.IsNullOrEmpty(_setting.levelDirectory))
        {
            Debug.LogWarning("Level directory not configured in LevelEditorSetting.");
            return;
        }

        if (!Directory.Exists(_setting.levelDirectory))
        {
            Debug.LogWarning($"Level directory not found: {_setting.levelDirectory}");
            return;
        }

        string[] prefabFiles = Directory.GetFiles(_setting.levelDirectory, "*.prefab");
        var indexed = new List<(int index, string path)>();

        foreach (string file in prefabFiles)
        {
            string assetPath = file.Replace("\\", "/");
            string prefabName = Path.GetFileNameWithoutExtension(assetPath);

            if (TryExtractLevelIndex(prefabName, out int levelIndex))
                indexed.Add((levelIndex, assetPath));
            else
                indexed.Add((int.MaxValue, assetPath)); // không có số -> đẩy cuối
        }

        indexed.Sort((a, b) =>
        {
            int idx = a.index.CompareTo(b.index);
            if (idx != 0) return idx;
            return string.Compare(Path.GetFileNameWithoutExtension(a.path), Path.GetFileNameWithoutExtension(b.path));
        });

        foreach (var entry in indexed) _levelPrefabPaths.Add(entry.path);
    }

    private bool TryExtractLevelIndex(string prefabName, out int levelIndex)
    {
        levelIndex = 0;
        var regex = new System.Text.RegularExpressions.Regex(@"\d+");
        var match = regex.Match(prefabName);
        if (match.Success && int.TryParse(match.Value, out int idx))
        {
            levelIndex = idx;
            return true;
        }

        return false;
    }

    private void OpenLevelPrefab(string prefabPath)
    {
        _selectedLevelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (_selectedLevelPrefab != null)
            AssetDatabase.OpenAsset(_selectedLevelPrefab); // mở Prefab Mode
        else
            Debug.LogError($"Failed to load prefab at path: {prefabPath}");
    }
}