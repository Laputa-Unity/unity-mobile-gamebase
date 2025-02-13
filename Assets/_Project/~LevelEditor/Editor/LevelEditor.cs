using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelEditorWindow : EditorWindow
{
    private GameObject _selectedPrefab;
    private Vector2 _scrollPosition;
    private Texture2D _greenOverlayTexture;
    private float _previewObjectAngle = 0f;
    private Vector3? _startLinePosition = null;
    private int _previewObjectCount = 1;
    private float _previewRotationSpeed = 10f;
    private List<GameObject> _previewObjects = new List<GameObject>();
    private float _maxDistance = 1000f;
    private float _lastScrollValue = 0f;
    private float _scrollThreshold = 0.1f;
    private string _prefabFilter = "";
    private Dictionary<GameObject, Texture2D> _prefabThumbnails = new Dictionary<GameObject, Texture2D>();
    private List<GameObject> _levelPrefabs = new List<GameObject>();
    private bool _enableHint = true;
    private LevelEditMode _editMode = LevelEditMode.Mode2D;
    private float _zAxisPosition = 0f;
    private Vector2 _levelScrollPosition;
    private List<string> _levelPrefabPaths = new List<string>();
    private SerializedObject _serializedLevelObject;
    private List<SerializedProperty> _levelProperties = new List<SerializedProperty>();
    private string _selectedLevelPath = "";
    private GameObject _selectedLevelPrefab;

    private LevelEditorSetting _setting;

    public enum LevelEditMode
    {
        Mode2D,
        Mode3D,
    }

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
        _setting = Resources.Load("LevelEditorSetting") as LevelEditorSetting;

        SetupSetting();
        CreateGreenOverlayTexture();
        SceneView.duringSceneGui += OnSceneGUI;
        LoadLevelPrefabs(); // Load level prefabs at initialization
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        ClearPreviewObjects();
    }

    private void OnGUI()
    {
        if (!IsInPrefabMode() && !string.IsNullOrEmpty(_selectedLevelPath))
        {
            _selectedLevelPath = ""; // Clear the selected level path
            _selectedPrefab = null;
            _selectedLevelPrefab = null;
            Repaint(); // Force the window to repaint
        }

        DrawTitleEditorUI();

        if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
        {
            LoadLevelPrefabs();
        }

        EditorGUILayout.BeginHorizontal(); // Split into two vertical sections (Left and Right)

        // Left: Vertical Scroll View for Level Prefabs
        DrawLevelPrefabsScrollView();

        // Right: Existing Level Editor UI
        EditorGUILayout.BeginVertical();
        DrawMainEditorUI(); // Move existing UI to its own method for better separation
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private bool IsInPrefabMode()
    {
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        return prefabStage != null;
    }

    private void SetupSetting()
    {
        minSize = _setting.windowMinSize;
    }

    private void DrawLevelPrefabsScrollView()
    {
        // Scroll view for level prefabs
        _levelScrollPosition = EditorGUILayout.BeginScrollView(_levelScrollPosition, GUILayout.Width(200));

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
        };

        GUILayout.Label("Level Prefabs", style);

        foreach (string prefabPath in _levelPrefabPaths)
        {
            string prefabName = Path.GetFileNameWithoutExtension(prefabPath); // Extract prefab name

            // Highlight the currently selected level with a green background
            GUIStyle itemStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                normal = {textColor = Color.white},
            };

            if (prefabPath == _selectedLevelPath)
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = Color.white;
            }

            // Toggle for selecting a level
            bool isSelected =
                prefabPath == _selectedLevelPath; // Set initial toggle state based on the current selection
            bool newSelectedState = GUILayout.Toggle(isSelected, prefabName, itemStyle, GUILayout.ExpandWidth(true));

            // Check if the toggle state has changed
            if (newSelectedState && prefabPath != _selectedLevelPath)
            {
                _selectedLevelPath = prefabPath; // Update the selected level path
                OpenLevelPrefab(_selectedLevelPath); // Open the prefab in Prefab Mode
            }

            GUI.backgroundColor = Color.white; // Reset background color
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Play This Level", GUILayout.ExpandWidth(true), GUILayout.Height(40)))
        {
            var level = _selectedLevelPrefab.GetComponent<Level>();
            level.PlayThisLevel();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawTitleEditorUI()
    {
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            GUIStyle centeredHelpBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = {textColor = Color.green}
            };

            GUILayout.Label($"EDITING LEVEL: {prefabStage.prefabContentsRoot.name.ToUpper()}", centeredHelpBoxStyle);
        }
        else
        {
            GUILayout.Label("No Level Prefab Opened", EditorStyles.helpBox);
        }
    }

    private void DrawMainEditorUI()
    {
        // Settings section
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
        };

        GUILayout.Label("Settings", style);
        _enableHint = EditorGUILayout.Toggle("Show Hints", _enableHint);

        // Enum dropdown for 2D/3D Mode
        _editMode = (LevelEditMode) EditorGUILayout.EnumPopup("Edit Mode", _editMode, GUILayout.ExpandWidth(false),
            GUILayout.Width(250));

        // Check if Scene view is in 2D mode when editMode is MouseZ2D
        if (_editMode == LevelEditMode.Mode2D)
        {
            if (!IsSceneViewIn2DMode())
            {
                // Display a warning message if Scene view is not in 2D mode
                EditorGUILayout.HelpBox(
                    "Warning: The Scene view is not in 2D mode. Please switch to 2D view to use MouseZ2D mode effectively.",
                    MessageType.Warning);
            }

            // Additional setting for Z-axis in 2D mode

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Z position", GUILayout.Width(100));
            _zAxisPosition = EditorGUILayout.FloatField(_zAxisPosition, GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Rotation speed", GUILayout.Width(100));
            _previewRotationSpeed = EditorGUILayout.FloatField(_previewRotationSpeed, GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // Search field to filter prefabs by name
        GUILayout.Label("Filter Prefabs", EditorStyles.boldLabel);
        _prefabFilter = EditorGUILayout.TextField("Search:", _prefabFilter);

        DrawPrefabsGrid();
        DrawSelectedPrefabThumbnail();
    }

    private void LoadLevelPrefabs()
    {
        _levelPrefabPaths.Clear();
        _prefabThumbnails.Clear(); // Clear old thumbnails

        if (Directory.Exists(_setting.levelDirectory))
        {
            string[] prefabFiles = Directory.GetFiles(_setting.levelDirectory, "*.prefab");

            List<(int index, string path)> indexedLevels = new List<(int, string)>();

            foreach (string prefabFile in prefabFiles)
            {
                string assetPath = prefabFile.Replace("\\", "/");
                string prefabName = Path.GetFileNameWithoutExtension(assetPath);

                if (TryExtractLevelIndex(prefabName, out int levelIndex))
                {
                    indexedLevels.Add((levelIndex, assetPath));
                }
                else
                {
                    Debug.LogWarning($"Failed to extract level index from prefab: {prefabName}");
                }
            }

            indexedLevels.Sort((a, b) => a.index.CompareTo(b.index));

            foreach (var (_, path) in indexedLevels)
            {
                _levelPrefabPaths.Add(path);

                // Load prefab and generate a thumbnail
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    _prefabThumbnails[prefab] = CapturePrefabThumbnail(prefab, 128, 128);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Level directory not found: {_setting.levelDirectory}");
        }
    }

    private List<GameObject> LoadAvailablePrefabs()
    {
        List<GameObject> prefabs = new List<GameObject>();

        // Ensure the directory exists
        if (Directory.Exists(_setting.prefabDirectory))
        {
            string[] prefabFiles =
                Directory.GetFiles(_setting.prefabDirectory, "*.prefab", SearchOption.AllDirectories);

            foreach (string prefabFile in prefabFiles)
            {
                string assetPath = prefabFile.Replace("\\", "/");
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab != null)
                {
                    prefabs.Add(prefab);

                    // Capture and cache the thumbnail
                    if (!_prefabThumbnails.ContainsKey(prefab))
                    {
                        _prefabThumbnails[prefab] = CapturePrefabThumbnail(prefab, 128, 128);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"Prefab directory not found: {_setting.prefabDirectory}");
        }

        return prefabs;
    }


    private bool TryExtractLevelIndex(string prefabName, out int levelIndex)
    {
        levelIndex = 0;
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\d+");
        var match = regex.Match(prefabName);

        if (match.Success && int.TryParse(match.Value, out int index))
        {
            levelIndex = index;
            return true;
        }

        return false;
    }

    private void OpenLevelPrefab(string prefabPath)
    {
        _selectedLevelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (_selectedLevelPrefab != null)
        {
            AssetDatabase.OpenAsset(_selectedLevelPrefab); // Open the prefab in Prefab Mode
        }
        else
        {
            Debug.LogError($"Failed to load prefab at path: {prefabPath}");
        }
    }


// Method to check if the Scene view is currently in 2D mode
    private bool IsSceneViewIn2DMode()
    {
        // Check the last active Scene view and return its 2D mode state
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            return sceneView.in2DMode;
        }

        // Default to false if no Scene view is active
        return false;
    }

    private void DrawSelectedPrefabThumbnail()
    {
        if (_selectedPrefab != null)
        {
            // Get the AssetPreview for the selected prefab
            Texture2D preview = AssetPreview.GetAssetPreview(_selectedPrefab);
            if (preview == null)
            {
                // If preview is null, try to load a mini thumbnail
                AssetPreview.SetPreviewTextureCacheSize(1);
                preview = AssetPreview.GetMiniThumbnail(_selectedPrefab);
            }

            if (preview != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace(); // Center the thumbnail horizontally
                EditorGUILayout.BeginVertical();

                // Style for the preview and the label
                GUIStyle selectedPrefabIconStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = {textColor = Color.white},
                };

                // Display the preview texture
                GUILayout.Label(preview, selectedPrefabIconStyle, GUILayout.Width(100), GUILayout.Height(100));

                GUIStyle centeredLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = {textColor = Color.green}
                };

                // Display the selected prefab's name
                GUILayout.Label($"Selected: {_selectedPrefab.name}", centeredLabelStyle, GUILayout.ExpandWidth(true));

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace(); // Center the thumbnail horizontally
                GUILayout.EndHorizontal();
            }
            else
            {
                // Display a fallback message if no preview is available
                GUILayout.Label("No preview available", EditorStyles.centeredGreyMiniLabel);
            }
        }
        else
        {
            // Display a message if no prefab is selected
            GUILayout.Label("No prefab selected", EditorStyles.centeredGreyMiniLabel);
        }
    }

    private Texture2D CapturePrefabThumbnail(GameObject prefab, int width, int height)
    {
        GameObject cameraObject = new GameObject("ThumbnailCamera", typeof(Camera));
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0, 0, 0, 0); // Transparent background
        camera.orthographic = true;
        camera.orthographicSize = 1;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 10f;
        camera.targetTexture = new RenderTexture(width, height, 16);

        GameObject tempPrefabInstance = Instantiate(prefab);
        tempPrefabInstance.transform.position = Vector3.zero;
        tempPrefabInstance.transform.rotation = Quaternion.identity;
        tempPrefabInstance.transform.localScale = Vector3.one;

        Bounds bounds = CalculateBounds(tempPrefabInstance);
        camera.orthographicSize = Mathf.Max(bounds.extents.y, bounds.extents.x) * 1.2f;
        camera.transform.position = bounds.center + new Vector3(0, 0, -5);
        camera.transform.LookAt(bounds.center);

        camera.Render();

        RenderTexture.active = camera.targetTexture;
        Texture2D thumbnail = new Texture2D(width, height, TextureFormat.ARGB32, false);
        thumbnail.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        thumbnail.Apply();

        RenderTexture.active = null;
        camera.targetTexture = null;
        DestroyImmediate(cameraObject);
        DestroyImmediate(tempPrefabInstance);

        return thumbnail;
    }


    private Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one * 0.5f);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }


    private void DrawPrefabsGrid()
    {
        GUILayout.Label("Available Prefabs", EditorStyles.boldLabel);

        int columnWidth = 80; // Width of each grid item
        float availableWidth = position.width - 220;
        int spacing = 10;
        int maxItemsPerRow = Mathf.FloorToInt((availableWidth + spacing) / (columnWidth + spacing));
        maxItemsPerRow = Mathf.Max(maxItemsPerRow, 1);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));

        string[] prefabPaths = Directory.GetFiles(_setting.prefabDirectory, "*.prefab", SearchOption.AllDirectories);
        List<GameObject> levelDesignPrefabs = new List<GameObject>();

        foreach (string path in prefabPaths)
        {
            string assetPath = path.Replace("\\", "/");
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null)
            {
                levelDesignPrefabs.Add(prefab);
            }
        }

        var filteredPrefabs = string.IsNullOrEmpty(_prefabFilter)
            ? levelDesignPrefabs
            : levelDesignPrefabs.FindAll(prefab => prefab.name.ToLower().Contains(_prefabFilter.ToLower()));

        int currentRowItemCount = 0;
        EditorGUILayout.BeginHorizontal();

        foreach (var prefab in filteredPrefabs)
        {
            if (currentRowItemCount >= maxItemsPerRow)
            {
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(spacing);
                EditorGUILayout.BeginHorizontal();
                currentRowItemCount = 0;
            }

            EditorGUILayout.BeginVertical(GUILayout.Width(columnWidth));

            Texture2D previewTexture = null;

            if (_editMode == LevelEditMode.Mode3D)
            {
                // Use Unity's Asset Preview for 3D mode
                previewTexture = AssetPreview.GetAssetPreview(prefab);
                if (previewTexture == null)
                {
                    AssetPreview.SetPreviewTextureCacheSize(filteredPrefabs.Count * 3);
                    AssetPreview.GetAssetPreview(prefab);
                    EditorApplication.delayCall += Repaint;
                    previewTexture = AssetPreview.GetMiniThumbnail(prefab);
                }
            }
            else
            {
                // Use custom 2D-generated thumbnails
                if (!_prefabThumbnails.ContainsKey(prefab))
                {
                    _prefabThumbnails[prefab] = CapturePrefabThumbnail(prefab, 128, 128);
                }

                previewTexture = _prefabThumbnails[prefab];
            }

            bool isSelected = _selectedPrefab == prefab;
            bool newSelectedState = GUILayout.Toggle(
                isSelected,
                new GUIContent(previewTexture ?? Texture2D.grayTexture), // Fallback in case of null
                new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = columnWidth,
                    fixedWidth = columnWidth
                });

            if (newSelectedState && !isSelected)
            {
                _selectedPrefab = prefab;
            }
            else if (!newSelectedState && isSelected)
            {
                _selectedPrefab = null;
            }

            GUIStyle prefabNameStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip
            };

            GUILayout.Label(prefab.name, prefabNameStyle, GUILayout.Width(columnWidth));
            EditorGUILayout.EndVertical();

            currentRowItemCount++;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }


    private void OnSceneGUI(SceneView sceneView)
    {
        if (_selectedPrefab == null)
            return;

        Event e = Event.current;
        Vector2 mousePosition = e.mousePosition;

        // Determine the world position based on the edit mode
        Vector3 worldPosition = GetWorldPosition(mousePosition);

        if (e.shift && _startLinePosition == null)
        {
            _startLinePosition = worldPosition;
        }

        if (e.shift && _startLinePosition.HasValue)
        {
            Handles.color = Color.green;
            Handles.DrawLine(_startLinePosition.Value, worldPosition);

            float scrollValue = Mouse.current.scroll.ReadValue().y;

            if (e.alt)
            {
                if (scrollValue > 0)
                {
                    _previewObjectAngle -= _previewRotationSpeed;
                }
                else if (scrollValue < 0)
                {
                    _previewObjectAngle += _previewRotationSpeed;
                }
            }
            else
            {
                if (Mathf.Abs(scrollValue - _lastScrollValue) > _scrollThreshold)
                {
                    if (scrollValue > 0f)
                    {
                        _previewObjectCount++;
                    }
                    else if (scrollValue < 0f && _previewObjectCount > 1)
                    {
                        _previewObjectCount--;
                    }

                    _lastScrollValue = scrollValue;
                }
            }

            UpdatePreviewObjects(worldPosition);

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                SpawnAllPreviewPrefabs();
                e.Use();
            }
        }

        // Display the hint only if enableHint is true
        if (_enableHint)
        {
            Handles.Label(worldPosition + Vector3.up * 0.5f,
                "Shift + Scroll to adjust count, Shift + Alt + Scroll to rotate, Shift + Click to spawn", new GUIStyle
                {
                    fontSize = 12,
                    normal = new GUIStyleState {textColor = Color.green},
                    alignment = TextAnchor.MiddleCenter
                });
        }

        if (!e.shift)
        {
            _startLinePosition = null;
            _previewObjectCount = 1;
            _previewObjectAngle = 0;
            ClearPreviewObjects();
        }

        sceneView.Repaint();
    }

    // Method to get world position based on the edit mode
    private Vector3 GetWorldPosition(Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        if (_editMode == LevelEditMode.Mode3D)
        {
            // Check if we're in Prefab Mode
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                // Use the correct scene for raycasting within the prefab environment
                if (RaycastInPrefabMode(ray, prefabStage.scene, out RaycastHit hit))
                {
                    return hit.point; // Return the hit point in Prefab Mode
                }
            }
            else
            {
                // Perform a 3D raycast to find the target point in the regular scene
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    return hit.point; // Return the hit point in the regular scene
                }
            }

            // Default position if nothing is hit
            return ray.origin + ray.direction * 10f; // Arbitrary point forward
        }
        else
        {
            // For MouseZ2D, calculate world position with Z fixed at zAxisPosition
            Vector3 worldPosition = ray.origin;
            worldPosition.z = _zAxisPosition;
            return worldPosition;
        }
    }

    private bool RaycastInPrefabMode(Ray ray, Scene prefabScene, out RaycastHit hit)
    {
        // Create a list to store all hits
        List<RaycastHit> hits = new List<RaycastHit>();

        // Get all root game objects in the Prefab Mode scene
        GameObject[] rootObjects = prefabScene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            // Get all colliders under each root object
            Collider[] colliders = root.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders)
            {
                // Perform the raycast against each collider in the Prefab Mode
                if (collider.Raycast(ray, out RaycastHit tempHit, Mathf.Infinity))
                {
                    hits.Add(tempHit);
                }
            }
        }

        // Find the closest hit
        if (hits.Count > 0)
        {
            hit = hits[0];
            float closestDistance = hit.distance;

            foreach (RaycastHit tempHit in hits)
            {
                if (tempHit.distance < closestDistance)
                {
                    hit = tempHit;
                    closestDistance = tempHit.distance;
                }
            }

            return true;
        }

        // Return false if no hits were found
        hit = new RaycastHit();
        return false;
    }

    private void UpdatePreviewObjects(Vector3 endPosition)
    {
        ClearPreviewObjects();

        if (!_startLinePosition.HasValue || _previewObjectCount < 1)
            return;

        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        Scene activeScene = SceneManager.GetActiveScene();

        // Determine the context for previewing objects
        Transform parentTransform = prefabStage != null ? prefabStage.prefabContentsRoot.transform : null;

        Vector3 start = _startLinePosition.Value;
        Vector3 end = endPosition;

        Vector3 direction = (end - start).normalized;
        float distance = Mathf.Min(Vector3.Distance(start, end), _maxDistance);

        float spacing = _previewObjectCount > 1 ? distance / (_previewObjectCount - 1) : 0;

        for (int i = 0; i < _previewObjectCount; i++)
        {
            Vector3 position = start + direction * spacing * i;
            GameObject preview = (GameObject) PrefabUtility.InstantiatePrefab(_selectedPrefab,
                prefabStage != null ? prefabStage.scene : activeScene);
            preview.transform.position = position;
            preview.transform.rotation = Quaternion.Euler(0, 0, _previewObjectAngle);
            preview.transform.SetParent(parentTransform); // Set parent only in Prefab Mode
            preview.hideFlags = HideFlags.HideAndDontSave;
            DisableColliders(preview);
            EnableRenderers(preview);

            ApplyOutlineEffect(preview);

            _previewObjects.Add(preview);
        }
    }


    private void SpawnAllPreviewPrefabs()
    {
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        Scene activeScene = SceneManager.GetActiveScene();

        GameObject lastSpawnedPrefab = null;

        foreach (var preview in _previewObjects)
        {
            if (preview != null)
            {
                // Instantiate prefabs based on the current mode
                GameObject spawnedPrefab = (GameObject) PrefabUtility.InstantiatePrefab(_selectedPrefab,
                    prefabStage != null ? prefabStage.scene : activeScene);
                spawnedPrefab.transform.position = preview.transform.position;
                spawnedPrefab.transform.rotation = preview.transform.rotation;

                if (prefabStage != null)
                {
                    // Set the parent to the root of the prefab stage in Prefab Mode
                    spawnedPrefab.transform.SetParent(prefabStage.prefabContentsRoot.transform);
                }

                Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn Prefab");

                // Keep track of the last spawned prefab
                lastSpawnedPrefab = spawnedPrefab;
            }
        }

        // Select the last spawned prefab in the hierarchy
        if (lastSpawnedPrefab != null)
        {
            Selection.activeObject = lastSpawnedPrefab;
        }

        ClearPreviewObjects();
    }

    private void ClearPreviewObjects()
    {
        foreach (var obj in _previewObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

        _previewObjects.Clear();
    }

    private void DisableColliders(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void EnableRenderers(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }
    }

    private void ApplyOutlineEffect(GameObject obj)
    {
        Shader outlineSpriteShader = Shader.Find("Custom/SpritesOutline");
        Shader outlineShader = Shader.Find("Custom/Outline");

        if (outlineSpriteShader == null || outlineShader == null)
        {
            Debug.LogError(
                "Outline shader not found! Make sure the shader is named correctly and located in your project.");
            return;
        }

        Material outlineSpriteMaterial = new Material(outlineSpriteShader)
        {
            color = Color.white
        };

        Material outlineMaterial = new Material(outlineShader)
        {
            color = Color.white
        };

        outlineSpriteMaterial.SetFloat("_OutlineSize", 5f);
        outlineSpriteMaterial.SetColor("_OutlineColor", Color.green);

        outlineMaterial.SetFloat("_OutlineWidth", 0.00001f);
        outlineMaterial.SetColor("_OutlineColor", Color.green);

        ApplyMaterialToRenderers<SpriteRenderer>(obj, outlineSpriteMaterial);
        ApplyMaterialToRenderers<MeshRenderer>(obj, outlineMaterial);
    }

    private void ApplyMaterialToRenderers<T>(GameObject obj, Material outlineMaterial) where T : Renderer
    {
        T[] renderers = obj.GetComponentsInChildren<T>(true);

        foreach (Renderer renderer in renderers)
        {
            Material[] originalMaterials = renderer.sharedMaterials;
            Material[] combinedMaterials = new Material[originalMaterials.Length + 1];
            originalMaterials.CopyTo(combinedMaterials, 0);

            combinedMaterials[combinedMaterials.Length - 1] = outlineMaterial;

            renderer.sharedMaterials = combinedMaterials;
        }

        MeshRenderer[] meshRenderers = obj.GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer renderer in meshRenderers)
        {
            Material[] originalMaterials = renderer.sharedMaterials;
            Material[] combinedMaterials = new Material[originalMaterials.Length + 1];
            originalMaterials.CopyTo(combinedMaterials, 0);

            combinedMaterials[combinedMaterials.Length - 1] = outlineMaterial;

            renderer.sharedMaterials = combinedMaterials;
        }
    }

    private void CreateGreenOverlayTexture()
    {
        _greenOverlayTexture = new Texture2D(1, 1);
        _greenOverlayTexture.SetPixel(0, 0, new Color(0, 1, 0, 0.3f));
        _greenOverlayTexture.Apply();
    }
}
