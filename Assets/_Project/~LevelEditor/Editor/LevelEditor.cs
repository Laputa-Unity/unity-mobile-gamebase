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

    // Block palette
    private BlockConfig _blockConfig;
    private Vector2 _paletteScroll;
    private int _selectedBlockIndex = -1;

    // Grid (cells)
    // -1 = empty; otherwise = index vào _blockConfig.blockData
    private List<int> _cells = new List<int>();
    private Vector2 _gridScroll;

    // Setting
    private LevelEditorSetting _setting;

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow() => GetWindow<LevelEditorWindow>("Level Editor");

    private void OnEnable()
    {
        _setting = Resources.Load("LevelEditorSetting") as LevelEditorSetting;
        if (_setting != null) minSize = _setting.windowMinSize;

        _blockConfig = Resources.Load<BlockConfig>("BlockConfig");
        if (_blockConfig == null)
            Debug.LogWarning("BlockConfig not found in Resources. Please create Resources/BlockConfig.asset");

        EnsureGridSize(_setting != null ? _setting.rowCount : 1, _setting != null ? _setting.columnCount : 1);
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

        // RIGHT: settings panel (banner + sliders + palette + grid)
        DrawSettingsPanel();

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
        
        using (new EditorGUI.DisabledScope(_blockConfig == null))
        {
            if (GUILayout.Button("Generate Level", GUILayout.Height(36)))
            {
                GenerateOpenPrefabFromGrid();
            }
        }
        
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
    
    private void GenerateOpenPrefabFromGrid()
    {
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        if (stage == null)
        {
            EditorUtility.DisplayDialog("No Prefab Opened",
                "Hãy mở Level Prefab trong Prefab Mode (double-click prefab trong Project) trước khi Generate.",
                "OK");
            return;
        }

        // Lấy root prefab đang mở
        var root = stage.prefabContentsRoot;
        if (root == null)
        {
            Debug.LogError("Prefab stage root not found.");
            return;
        }

        // Tìm Level và Board
        var level = root.GetComponent<Level>();
        var board = root.GetComponentInChildren<Board>(true);

        if (level == null || board == null)
        {
            EditorUtility.DisplayDialog("Missing Components",
                "Không tìm thấy Level và/hoặc Board trong prefab đang mở.",
                "OK");
            return;
        }

        if (_setting == null)
        {
            EditorUtility.DisplayDialog("Missing Setting",
                "LevelEditorSetting bị null.",
                "OK");
            return;
        }

        int rows = Mathf.Clamp(_setting.rowCount, 1, 14);
        int cols = Mathf.Clamp(_setting.columnCount, 1, 10);

        // Bảo vệ kích thước _cells
        EnsureGridSize(rows, cols);

        // Ghi undo để có thể Ctrl+Z
        Undo.RegisterFullObjectHierarchyUndo(root, "Generate Level Grid");

        // Gọi Board để build theo layout (-1 = Empty)
        board.GenerateBoardFromEditor(rows, cols, _cells, _blockConfig);

        // Đánh dấu dirty để lưu thay đổi prefab
        EditorUtility.SetDirty(board.gameObject);
        EditorUtility.SetDirty(root);

        // Với Prefab Mode, bạn có nút Save ở thanh trên; có thể force save nếu muốn:
        // PrefabUtility.SaveAsPrefabAsset(root, stage.assetPath);
    }

    private void DrawSettingsPanel()
    {
        EditorGUILayout.BeginVertical();

        // ===== Editing banner đặt TRÊN phần Settings =====
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            var centered = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.green }
            };
            GUILayout.Label($"EDITING LEVEL: {prefabStage.prefabContentsRoot.name.ToUpper()}", centered);
        }
        else
        {
            GUILayout.Label("No Level Prefab Opened", EditorStyles.helpBox);
        }

        // ===== Settings block: chỉ slider Column/Row =====
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Column (1..10)
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Column", GUILayout.Width(80));
        int col = _setting != null ? _setting.columnCount : 1;
        int newCol = EditorGUILayout.IntSlider(col, 1, 10);
        if (_setting != null && newCol != _setting.columnCount)
        {
            _setting.columnCount = newCol;
            EditorUtility.SetDirty(_setting);
            EnsureGridSize(_setting.rowCount, _setting.columnCount);
        }

        EditorGUILayout.EndHorizontal();

        // Row (1..14)
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Row", GUILayout.Width(80));
        int row = _setting != null ? _setting.rowCount : 1;
        int newRow = EditorGUILayout.IntSlider(row, 1, 14);
        if (_setting != null && newRow != _setting.rowCount)
        {
            _setting.rowCount = newRow;
            EditorUtility.SetDirty(_setting);
            EnsureGridSize(_setting.rowCount, _setting.columnCount);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        GUILayout.Space(8);

        // ===== Block Palette (hiển thị & chọn) =====
        DrawBlockPalette();

        GUILayout.Space(8);

        // ===== Grid theo Row x Column (click trái gắn, click phải xóa) =====
        DrawPlacementGrid();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
    }

    private void DrawBlockPalette()
    {
        if (_blockConfig == null || _blockConfig.blockData == null || _blockConfig.blockData.Count == 0)
        {
            EditorGUILayout.HelpBox("No BlockConfig or Block Data found.", MessageType.Info);
            return;
        }

        GUILayout.Label("Blocks", EditorStyles.boldLabel);

        const int tileSize = 64;
        const int spacing = 8;
        float panelWidth = position.width - 220 - 30; // trừ sidebar + padding
        int perRow = Mathf.Max(1, Mathf.FloorToInt((panelWidth + spacing) / (tileSize + spacing)));

        int col = 0;
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < _blockConfig.blockData.Count; i++)
        {
            var data = _blockConfig.blockData[i];

            if (col >= perRow)
            {
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(spacing);
                EditorGUILayout.BeginHorizontal();
                col = 0;
            }

            Rect r = GUILayoutUtility.GetRect(tileSize, tileSize, GUILayout.Width(tileSize),
                GUILayout.Height(tileSize));

            // Toggle chọn
            bool selected = i == _selectedBlockIndex;
            Color prev = GUI.color;
            GUI.color = selected ? new Color(0.6f, 1f, 0.6f, 1f) : Color.white;
            if (GUI.Button(r, GUIContent.none))
            {
                _selectedBlockIndex = selected ? -1 : i;
                Repaint();
            }

            GUI.color = prev;

            // Viền + preview
            Handles.DrawSolidRectangleWithOutline(r, new Color(0, 0, 0, 0),
                selected ? Color.green : new Color(0.2f, 0.2f, 0.2f, 1f));
            DrawBlockPreviewIntoRect(data, r);

            col++;
            GUILayout.Space(spacing);
        }

        EditorGUILayout.EndHorizontal();

        string label = _selectedBlockIndex >= 0
            ? _blockConfig.blockData[_selectedBlockIndex].blockType.ToString()
            : "None";
        EditorGUILayout.LabelField("Selected Block:", label);
    }


    private void DrawPlacementGrid()
{
    if (_setting == null) return;

    int rows = Mathf.Clamp(_setting.rowCount, 1, 14);
    int cols = Mathf.Clamp(_setting.columnCount, 1, 10);

    // đảm bảo kích thước _cells khớp grid hiện tại
    EnsureGridSize(rows, cols);

    GUILayout.Label("Color Statistics", EditorStyles.boldLabel);

    const int tileSize = 60;
    const int spacing  = 10;

    for (int r = 0; r < rows; r++)
    {
        // HÀNG CĂN GIỮA: flexible space hai bên
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        for (int c = 0; c < cols; c++)
        {
            int idx = r * cols + c;
            Rect cellRect = GUILayoutUtility.GetRect(tileSize, tileSize,
                                 GUILayout.Width(tileSize), GUILayout.Height(tileSize));

            int blockIdx   = (idx < _cells.Count ? _cells[idx] : -1);
            bool hasTexture = blockIdx >= 0;

            // Nền nhẹ (tùy chọn) + VIỀN ĐEN
            // EditorGUI.DrawRect(cellRect, new Color(0,0,0,0.08f)); // bật nếu muốn nền nhạt
            Handles.DrawSolidRectangleWithOutline(cellRect, new Color(0, 0, 0, 0), Color.black);

            // Nội dung
            if (hasTexture && _blockConfig != null && blockIdx < _blockConfig.blockData.Count)
            {
                DrawBlockPreviewIntoRect(_blockConfig.blockData[blockIdx], cellRect);
            }
            else
            {
                var centered = new GUIStyle(EditorStyles.centeredGreyMiniLabel){ alignment = TextAnchor.MiddleCenter };
                GUI.Label(cellRect, "Empty", centered);
            }

            // Click: trái gắn (nếu đang chọn block), phải xóa (nếu có)
            var e = Event.current;
            if (e.type == EventType.MouseDown && cellRect.Contains(e.mousePosition))
            {
                if (e.button == 0 && _selectedBlockIndex != -1)
                {
                    SetCell(idx, _selectedBlockIndex);
                    e.Use();
                }
                else if (e.button == 1 && hasTexture)
                {
                    SetCell(idx, -1);
                    e.Use();
                }
            }

            // Chỉ chèn spacing giữa các ô, KHÔNG thêm sau ô cuối
            if (c < cols - 1) GUILayout.Space(spacing);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        // khoảng cách dọc giữa các hàng
        GUILayout.Space(spacing);
    }
}



    private void DrawBlockPreviewIntoRect(BlockData data, Rect r)
    {
        const int pad = 4;
        var inner = new Rect(r.x + pad, r.y + pad, r.width - pad * 2, r.height - pad * 2);

        if (data == null) return;

        if (data.sprite != null && data.sprite.texture != null)
        {
            var s = data.sprite;
            Rect uv = new Rect(
                s.textureRect.x / s.texture.width,
                s.textureRect.y / s.texture.height,
                s.textureRect.width / s.texture.width,
                s.textureRect.height / s.texture.height
            );
            GUI.DrawTextureWithTexCoords(inner, s.texture, uv, true);
        }
        else if (data.texture != null)
        {
            GUI.DrawTexture(inner, data.texture, ScaleMode.ScaleToFit, true);
        }
        else
        {
            var centered = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleCenter };
            GUI.Label(inner, "No\nTexture", centered);
        }
    }

    private void SetCell(int index, int value)
    {
        if (index < 0) return;
        if (index >= _cells.Count) return;
        _cells[index] = value;
        Repaint();
    }

    private void EnsureGridSize(int newRows, int newCols)
    {
        newRows = Mathf.Clamp(newRows, 1, 14);
        newCols = Mathf.Clamp(newCols, 1, 10);

        int newSize = newRows * newCols;
        if (_cells.Count == 0)
        {
            _cells = new List<int>(newSize);
            for (int i = 0; i < newSize; i++) _cells.Add(-1);
            return;
        }

        // Resize nhưng giữ dữ liệu giao nhau
        var oldCells = _cells;
        int oldRows = Mathf.Max(1, _setting != null ? _setting.rowCount : 1);
        int oldCols = Mathf.Max(1, _setting != null ? _setting.columnCount : 1);

        var resized = new List<int>(newSize);
        for (int i = 0; i < newSize; i++) resized.Add(-1);

        int copyRows = Mathf.Min(oldRows, newRows);
        int copyCols = Mathf.Min(oldCols, newCols);

        for (int r = 0; r < copyRows; r++)
        {
            for (int c = 0; c < copyCols; c++)
            {
                int oldIdx = r * oldCols + c;
                int newIdx = r * newCols + c;
                if (oldIdx >= 0 && oldIdx < oldCells.Count && newIdx < resized.Count)
                    resized[newIdx] = oldCells[oldIdx];
            }
        }

        _cells = resized;
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