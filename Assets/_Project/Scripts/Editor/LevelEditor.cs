using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelEditorWindow : EditorWindow
{
    private List<string> whiteListFolders = new List<string>();
    private List<string> blackListFolders = new List<string>();
    private List<GameObject> availablePrefabs = new List<GameObject>();
    private GameObject selectedPrefab;
    private Vector2 scrollPosition;
    private Texture2D greenOverlayTexture;
    private float previewObjectAngle = 0f;
    private float rotationSpeed = 10f;
    private Vector3? startLinePosition = null;
    private int previewObjectCount = 1;
    private List<GameObject> previewObjects = new List<GameObject>();
    private float maxDistance = 1000f;
    private float lastScrollValue = 0f;
    private float scrollThreshold = 0.1f;
    private string prefabFilter = "";

    private Dictionary<GameObject, Texture2D> prefabThumbnails = new Dictionary<GameObject, Texture2D>();
    private List<GameObject> levelPrefabs = new List<GameObject>();

    private bool enableHint = true; // Default value to show hints
    private LevelEditMode editMode = LevelEditMode.MouseZ2D; // Enum for mode selection
    private float zAxisPosition = 0f; // Default Z position for 2D Mode

    private Vector2 levelScrollPosition; // Scroll position for the level list
    private List<string> levelPrefabPaths = new List<string>(); // Store level prefab paths
    private string levelDirectory = "Assets/_Project/Resources/Levels"; // Level prefab directory
    private SerializedObject serializedLevelObject; // Serialized object for levels
    private List<SerializedProperty> levelProperties = new List<SerializedProperty>(); // Serialized properties list
    private string selectedLevelPath = ""; // Track the selected level

    public enum LevelEditMode
    {
        MouseZ2D,
        Raycast3D,
    }

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
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
        DrawTitleEditorUI();

        EditorGUILayout.BeginHorizontal(); // Split into two vertical sections (Left and Right)

        // Left: Vertical Scroll View for Level Prefabs
        DrawLevelPrefabsScrollView();

        // Right: Existing Level Editor UI
        EditorGUILayout.BeginVertical();
        DrawMainEditorUI(); // Move existing UI to its own method for better separation
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    
    private void DrawLevelPrefabsScrollView()
    {
        // Scroll view for level prefabs
        levelScrollPosition = EditorGUILayout.BeginScrollView(levelScrollPosition, GUILayout.Width(200));

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
        };

        GUILayout.Label("Level Prefabs", style);

        foreach (string prefabPath in levelPrefabPaths)
        {
            string prefabName = Path.GetFileNameWithoutExtension(prefabPath); // Extract prefab name

            // Highlight the currently selected level with a green background
            GUIStyle itemStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white },
            };

            if (prefabPath == selectedLevelPath)
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = Color.white;
            }

            // Toggle for selecting a level
            bool isSelected = prefabPath == selectedLevelPath; // Set initial toggle state based on the current selection
            bool newSelectedState = GUILayout.Toggle(isSelected, prefabName, itemStyle, GUILayout.ExpandWidth(true));

            // Check if the toggle state has changed
            if (newSelectedState && prefabPath != selectedLevelPath)
            {
                selectedLevelPath = prefabPath; // Update the selected level path
                OpenLevelPrefab(selectedLevelPath); // Open the prefab in Prefab Mode
            }

            GUI.backgroundColor = Color.white; // Reset background color
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
                normal = { textColor = Color.green }
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
        enableHint = EditorGUILayout.Toggle("Show Hints", enableHint);

        // Enum dropdown for 2D/3D Mode
        editMode = (LevelEditMode)EditorGUILayout.EnumPopup("Edit Mode", editMode);

        // Check if Scene view is in 2D mode when editMode is MouseZ2D
        if (editMode == LevelEditMode.MouseZ2D)
        {
            if (!IsSceneViewIn2DMode())
            {
                // Display a warning message if Scene view is not in 2D mode
                EditorGUILayout.HelpBox(
                    "Warning: The Scene view is not in 2D mode. Please switch to 2D view to use MouseZ2D mode effectively.",
                    MessageType.Warning);
            }

            // Additional setting for Z-axis in 2D mode
            zAxisPosition = EditorGUILayout.FloatField("Z-Axis Position", zAxisPosition);
        }

        EditorGUILayout.BeginHorizontal();
        DrawFoldersList(ref whiteListFolders, "White List Folders");
        DrawFoldersList(ref blackListFolders, "Black List Folders");
        EditorGUILayout.EndHorizontal();

        // Search field to filter prefabs by name
        GUILayout.Label("Filter Prefabs", EditorStyles.boldLabel);
        prefabFilter = EditorGUILayout.TextField("Search:", prefabFilter);

        DrawPrefabsGrid();
        DrawSelectedPrefabThumbnail();

        // Add a Refresh button to redraw available prefabs
        if (GUILayout.Button("Refresh"))
        {
            RefreshAvailablePrefabs();
        }
    }

    private void LoadLevelPrefabs()
    {
        levelPrefabPaths.Clear();

        if (Directory.Exists(levelDirectory))
        {
            string[] prefabFiles = Directory.GetFiles(levelDirectory, "*.prefab");

            // List to store level index and paths
            List<(int index, string path)> indexedLevels = new List<(int, string)>();

            foreach (string prefabFile in prefabFiles)
            {
                string assetPath = prefabFile.Replace("\\", "/");
                string prefabName = Path.GetFileNameWithoutExtension(assetPath);

                // Extract numeric index from the prefab name
                if (TryExtractLevelIndex(prefabName, out int levelIndex))
                {
                    indexedLevels.Add((levelIndex, assetPath));
                }
                else
                {
                    Debug.LogWarning($"Failed to extract level index from prefab: {prefabName}");
                }
            }

            // Sort levels by their numeric index
            indexedLevels.Sort((a, b) => a.index.CompareTo(b.index));

            // Add the sorted paths to the levelPrefabPaths list
            foreach (var (_, path) in indexedLevels)
            {
                levelPrefabPaths.Add(path);
            }
        }
        else
        {
            Debug.LogWarning($"Level directory not found: {levelDirectory}");
        }
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
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            AssetDatabase.OpenAsset(prefab); // Open the prefab in Prefab Mode
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
        if (selectedPrefab != null)
        {
            // Capture the prefab's view with a temporary camera
            Texture2D preview = GetCachedThumbnail(selectedPrefab, 100, 100);
            // Center both the thumbnail and the text together in a vertical layout
            if (preview != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace(); // Adds flexible space to center the thumbnail
                EditorGUILayout.BeginVertical();
                GUIStyle centeredLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter, // Center the text inside the label
                    normal = { textColor = Color.green }
                };
                GUILayout.Label(preview, centeredLabelStyle, GUILayout.Width(300), GUILayout.Height(120));

                GUILayout.Label($"Selected: {selectedPrefab.name}", centeredLabelStyle, GUILayout.Width(300));
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }

    private Texture2D GetCachedThumbnail(GameObject prefab, int width, int height)
    {
        if (prefabThumbnails.ContainsKey(prefab))
            return prefabThumbnails[prefab];

        Texture2D thumbnail = CapturePrefabThumbnail(prefab, width, height);
        prefabThumbnails[prefab] = thumbnail;
        return thumbnail;
    }

    private Texture2D CapturePrefabThumbnail(GameObject prefab, int width, int height)
    {
        GameObject cameraObject = new GameObject("ThumbnailCamera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(1, 1, 1, 0.1f);
        camera.orthographic = true;

        RenderTexture renderTexture = new RenderTexture(width, height, 16);
        camera.targetTexture = renderTexture;

        GameObject tempPrefabInstance = Instantiate(prefab);
        tempPrefabInstance.transform.position = Vector3.zero;

        Bounds bounds = CalculateBounds(tempPrefabInstance);
        camera.orthographicSize = bounds.extents.y > bounds.extents.x ? bounds.extents.y : bounds.extents.x;
        camera.transform.position = bounds.center + new Vector3(0, 0, -10);
        camera.transform.LookAt(bounds.center);

        camera.Render();

        RenderTexture.active = renderTexture;
        Texture2D thumbnail = new Texture2D(width, height, TextureFormat.ARGB32, false);
        thumbnail.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        thumbnail.Apply();

        RenderTexture.active = null;
        camera.targetTexture = null;
        DestroyImmediate(cameraObject);
        DestroyImmediate(renderTexture);
        DestroyImmediate(tempPrefabInstance);

        return thumbnail;
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }

    private void DrawFoldersList(ref List<string> foldersList, string label)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f - 10));
        GUILayout.Label(label, EditorStyles.boldLabel);

        if (GUILayout.Button("Add Folder to " + label, GUILayout.Height(40)))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Folder for " + label, "Assets", "");
            if (!string.IsNullOrEmpty(folderPath) && !foldersList.Contains(folderPath))
            {
                foldersList.Add(folderPath);
                RefreshAvailablePrefabs();
            }
        }

        int indexToRemove = -1;
        for (int i = 0; i < foldersList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(foldersList[i], GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
            {
                indexToRemove = i;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (indexToRemove != -1)
        {
            foldersList.RemoveAt(indexToRemove);
            RefreshAvailablePrefabs();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPrefabsGrid()
    {
        GUILayout.Label("Available Prefabs", EditorStyles.boldLabel);

        int columnWidth = 110;
        float availableWidth = position.width - columnWidth;
        int columns = Mathf.FloorToInt(availableWidth / columnWidth);
        columns = Mathf.Max(columns, 1);

        // Add scrolling to the prefab grid
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        int rowCount = 0;
        EditorGUILayout.BeginHorizontal();

        GUIStyle greenTextStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
        {
            normal = { textColor = Color.green }
        };

        GUIStyle selectedButtonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = { background = MakeTex(2, 2, new Color(0, 1, 0, 0.5f)) },
            active = { background = MakeTex(2, 2, new Color(0, 1, 0, 0.7f)) },
            focused = { background = MakeTex(2, 2, new Color(0, 1, 0, 0.5f)) }
        };

        // Filter prefabs based on the search string
        var filteredPrefabs = string.IsNullOrEmpty(prefabFilter)
            ? availablePrefabs
            : availablePrefabs.FindAll(prefab => prefab.name.ToLower().Contains(prefabFilter.ToLower()));

        for (int i = 0; i < filteredPrefabs.Count; i++)
        {
            GameObject prefab = filteredPrefabs[i];
            Texture2D preview = GetCachedThumbnail(prefab, 80, 80);

            EditorGUILayout.BeginVertical(GUILayout.Width(columnWidth));

            bool isSelected = selectedPrefab == prefab;
            GUIStyle buttonStyle = isSelected ? selectedButtonStyle : GUI.skin.button;

            Rect buttonRect = GUILayoutUtility.GetRect(columnWidth, columnWidth);
            if (GUI.Button(buttonRect, "", buttonStyle))
            {
                if (isSelected)
                {
                    selectedPrefab = null;
                    ClearPreviewObjects();
                }
                else
                {
                    selectedPrefab = prefab;
                }
            }

            if (preview != null)
            {
                float textureWidth = preview.width;
                float textureHeight = preview.height;
                float maxDimension = Mathf.Max(textureWidth, textureHeight);
                float scale = (columnWidth - 10) / maxDimension;
                float textureX = buttonRect.x + (buttonRect.width - textureWidth * scale) / 2;
                float textureY = buttonRect.y + (buttonRect.height - textureHeight * scale) / 2;

                Rect textureRect = new Rect(textureX, textureY, textureWidth * scale, textureHeight * scale);
                GUI.DrawTexture(textureRect, preview, ScaleMode.ScaleToFit);
            }

            GUILayout.Label(prefab.name, greenTextStyle);
            EditorGUILayout.EndVertical();

            rowCount++;
            if (rowCount >= columns)
            {
                rowCount = 0;
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
            }
            else
            {
                GUILayout.Space(10);
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView(); // End the scroll view
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (selectedPrefab == null)
            return;

        Event e = Event.current;
        Vector2 mousePosition = e.mousePosition;

        // Determine the world position based on the edit mode
        Vector3 worldPosition = GetWorldPosition(mousePosition);

        if (e.shift && startLinePosition == null)
        {
            startLinePosition = worldPosition;
        }

        if (e.shift && startLinePosition.HasValue)
        {
            Handles.color = Color.green;
            Handles.DrawLine(startLinePosition.Value, worldPosition);

            float scrollValue = Mouse.current.scroll.ReadValue().y;

            if (e.alt)
            {
                if (scrollValue > 0)
                {
                    previewObjectAngle -= rotationSpeed;
                }
                else if (scrollValue < 0)
                {
                    previewObjectAngle += rotationSpeed;
                }
            }
            else
            {
                if (Mathf.Abs(scrollValue - lastScrollValue) > scrollThreshold)
                {
                    if (scrollValue > 0f)
                    {
                        previewObjectCount++;
                    }
                    else if (scrollValue < 0f && previewObjectCount > 1)
                    {
                        previewObjectCount--;
                    }

                    lastScrollValue = scrollValue;
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
        if (enableHint)
        {
            Handles.Label(worldPosition + Vector3.up * 0.5f,
                "Shift + Scroll to adjust count, Shift + Alt + Scroll to rotate, Shift + Click to spawn", new GUIStyle
                {
                    fontSize = 12,
                    normal = new GUIStyleState { textColor = Color.green },
                    alignment = TextAnchor.MiddleCenter
                });
        }

        if (!e.shift)
        {
            startLinePosition = null;
            previewObjectCount = 1;
            previewObjectAngle = 0;
            ClearPreviewObjects();
        }

        sceneView.Repaint();
    }

    // Method to get world position based on the edit mode
    private Vector3 GetWorldPosition(Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        if (editMode == LevelEditMode.Raycast3D)
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
            worldPosition.z = zAxisPosition;
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

        if (!startLinePosition.HasValue || previewObjectCount < 1)
            return;

        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        Scene activeScene = SceneManager.GetActiveScene();

        // Determine the context for previewing objects
        Transform parentTransform = prefabStage != null ? prefabStage.prefabContentsRoot.transform : null;

        Vector3 start = startLinePosition.Value;
        Vector3 end = endPosition;

        Vector3 direction = (end - start).normalized;
        float distance = Mathf.Min(Vector3.Distance(start, end), maxDistance);

        float spacing = previewObjectCount > 1 ? distance / (previewObjectCount - 1) : 0;

        for (int i = 0; i < previewObjectCount; i++)
        {
            Vector3 position = start + direction * spacing * i;
            GameObject preview = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab,
                prefabStage != null ? prefabStage.scene : activeScene);
            preview.transform.position = position;
            preview.transform.rotation = Quaternion.Euler(0, 0, previewObjectAngle);
            preview.transform.SetParent(parentTransform); // Set parent only in Prefab Mode
            preview.hideFlags = HideFlags.HideAndDontSave;
            DisableColliders(preview);
            EnableRenderers(preview);

            ApplyOutlineEffect(preview);

            previewObjects.Add(preview);
        }
    }


    private void SpawnAllPreviewPrefabs()
    {
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        Scene activeScene = SceneManager.GetActiveScene();

        foreach (var preview in previewObjects)
        {
            if (preview != null)
            {
                // Instantiate prefabs based on the current mode
                GameObject spawnedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab,
                    prefabStage != null ? prefabStage.scene : activeScene);
                spawnedPrefab.transform.position = preview.transform.position;
                spawnedPrefab.transform.rotation = preview.transform.rotation;

                if (prefabStage != null)
                {
                    // Set the parent to the root of the prefab stage in Prefab Mode
                    spawnedPrefab.transform.SetParent(prefabStage.prefabContentsRoot.transform);
                }

                Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn Prefab");
            }
        }

        ClearPreviewObjects();
    }

    private void ClearPreviewObjects()
    {
        foreach (var obj in previewObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

        previewObjects.Clear();
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

        outlineMaterial.SetFloat("_OutlineWidth", 0.001f);
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

    private void RefreshAvailablePrefabs()
    {
        availablePrefabs.Clear();
        prefabThumbnails.Clear();

        foreach (string folder in whiteListFolders)
        {
            string[] prefabPaths = Directory.GetFiles(folder, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (!IsInBlackList(prefabPath))
                {
                    string assetPath = "Assets" + prefabPath.Replace(Application.dataPath, "").Replace("\\", "/");
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (prefab != null)
                    {
                        availablePrefabs.Add(prefab);
                    }
                }
            }
        }

        Repaint();
    }

    private bool IsInBlackList(string prefabPath)
    {
        prefabPath = Path.GetFullPath(prefabPath).Replace("\\", "/");
        foreach (string blackListFolder in blackListFolders)
        {
            string normalizedBlackListPath = Path.GetFullPath(blackListFolder).Replace("\\", "/");
            if (prefabPath.StartsWith(normalizedBlackListPath))
            {
                return true;
            }
        }

        return false;
    }

    private void CreateGreenOverlayTexture()
    {
        greenOverlayTexture = new Texture2D(1, 1);
        greenOverlayTexture.SetPixel(0, 0, new Color(0, 1, 0, 0.3f));
        greenOverlayTexture.Apply();
    }
}