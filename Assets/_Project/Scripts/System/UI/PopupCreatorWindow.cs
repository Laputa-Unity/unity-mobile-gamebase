#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PopupCreatorWindow : EditorWindow
{
    private PopupCreator _creator;
    private string _popupName = "Popup";

    public static void Open(PopupCreator creator)
    {
        var window = GetWindow<PopupCreatorWindow>(true, "Create New Popup");
        window._creator = creator;
        window.minSize = new Vector2(420, 140);
        window.maxSize = new Vector2(420, 140);
        window.ShowUtility();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.HelpBox($"Enter the popup name. For example: PopupShop\nThe script will be created in: {_creator.ScriptSavingDirectory} \nThe prefab will be created in: {_creator.PopupSavingDirectory}", MessageType.Info);

        EditorGUILayout.Space(5);
        _popupName = EditorGUILayout.TextField("Popup Name", _popupName);

        EditorGUILayout.Space(10);

        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Cancel", GUILayout.Height(30)))
            {
                Close();
            }

            if (GUILayout.Button("OK", GUILayout.Height(30)))
            {
                PopupCreationUtility.CreatePopup(_creator, _popupName);
                Close();
            }
        }
    }
}

[InitializeOnLoad]
public static class PopupCreationUtility
{
    private const string PendingKey = "POPUP_CREATOR_PENDING";
    private const string PopupNameKey = "POPUP_CREATOR_NAME";
    private const string ScriptPathKey = "POPUP_CREATOR_SCRIPT_PATH";
    private const string PrefabPathKey = "POPUP_CREATOR_PREFAB_PATH";
    private const string TemplatePrefabPathKey = "POPUP_CREATOR_TEMPLATE_PREFAB_PATH";
    private const string PopupConfigPathKey = "POPUP_CREATOR_CONFIG_PATH";

    static PopupCreationUtility()
    {
        EditorApplication.delayCall += TryFinishPendingPopupCreation;
    }

    public static void CreatePopup(PopupCreator controller, string rawPopupName)
    {
        if (controller == null)
        {
            EditorUtility.DisplayDialog("Error", "PopupCreator is null.", "OK");
            return;
        }

        if (controller.PopupPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "popupPrefab chưa được gán.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(controller.ScriptSavingDirectory))
        {
            EditorUtility.DisplayDialog("Error", "scriptSavingDirectory đang rỗng.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(controller.PopupSavingDirectory))
        {
            EditorUtility.DisplayDialog("Error", "popupSavingDirectory đang rỗng.", "OK");
            return;
        }

        string popupName = SanitizePopupName(rawPopupName);

        if (string.IsNullOrWhiteSpace(popupName))
        {
            EditorUtility.DisplayDialog("Error", "Tên popup không hợp lệ.", "OK");
            return;
        }

        string scriptPath = $"{controller.ScriptSavingDirectory}/{popupName}.cs".Replace("\\", "/");
        string prefabPath = $"{controller.PopupSavingDirectory}/{popupName}.prefab".Replace("\\", "/");

        if (File.Exists(scriptPath))
        {
            EditorUtility.DisplayDialog("Error", $"Script đã tồn tại:\n{scriptPath}", "OK");
            return;
        }

        if (File.Exists(prefabPath))
        {
            EditorUtility.DisplayDialog("Error", $"Prefab đã tồn tại:\n{prefabPath}", "OK");
            return;
        }

        string templatePrefabPath = AssetDatabase.GetAssetPath(controller.PopupPrefab.gameObject);
        if (string.IsNullOrEmpty(templatePrefabPath))
        {
            EditorUtility.DisplayDialog("Error", "Không lấy được path của popupPrefab.", "OK");
            return;
        }

        CreateFolderIfNeeded(controller.ScriptSavingDirectory);
        CreateFolderIfNeeded(controller.PopupSavingDirectory);

        string scriptContent =
$@"using UnityEngine;

public class {popupName} : Popup
{{
    
}}";

        File.WriteAllText(scriptPath, scriptContent);

        SessionState.SetBool(PendingKey, true);
        SessionState.SetString(PopupNameKey, popupName);
        SessionState.SetString(ScriptPathKey, scriptPath);
        SessionState.SetString(PrefabPathKey, prefabPath);
        SessionState.SetString(TemplatePrefabPathKey, templatePrefabPath);
        SessionState.SetString(PopupConfigPathKey, AssetDatabase.GetAssetPath(controller.PopupConfig));

        AssetDatabase.Refresh();

        Debug.Log($"Đã tạo script {popupName}.cs. Đang chờ Unity compile để tạo prefab...");
    }

    private static void TryFinishPendingPopupCreation()
    {
        if (!SessionState.GetBool(PendingKey, false))
            return;

        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;

        string popupName = SessionState.GetString(PopupNameKey, string.Empty);
        string prefabPath = SessionState.GetString(PrefabPathKey, string.Empty);
        string templatePrefabPath = SessionState.GetString(TemplatePrefabPathKey, string.Empty);
        string popupConfigPath = SessionState.GetString(PopupConfigPathKey, string.Empty);

        if (string.IsNullOrEmpty(popupName) ||
            string.IsNullOrEmpty(prefabPath) ||
            string.IsNullOrEmpty(templatePrefabPath))
        {
            ClearPendingState();
            return;
        }

        Type popupType = TypeCache.GetTypesDerivedFrom<Popup>()
            .FirstOrDefault(t => t.Name == popupName);

        if (popupType == null)
        {
            Debug.LogWarning($"Chưa tìm thấy type {popupName}. Có thể Unity chưa compile xong hoàn toàn.");
            EditorApplication.delayCall += TryFinishPendingPopupCreation;
            return;
        }

        GameObject templatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(templatePrefabPath);
        if (templatePrefab == null)
        {
            Debug.LogError($"Không load được popupPrefab tại path: {templatePrefabPath}");
            ClearPendingState();
            return;
        }

        GameObject instance = null;
        try
        {
            instance = (GameObject)PrefabUtility.InstantiatePrefab(templatePrefab);
            instance.name = popupName;

            PrefabUtility.UnpackPrefabInstance(
                instance,
                PrefabUnpackMode.Completely,
                InteractionMode.AutomatedAction
            );

            Popup oldPopup = instance.GetComponent<Popup>();
            Component newComponent = instance.GetComponent(popupType);
            if (newComponent == null)
            {
                newComponent = instance.AddComponent(popupType);
            }

            Popup newPopup = newComponent as Popup;

            if (oldPopup != null && newPopup != null && oldPopup != newPopup)
            {
                EditorUtility.CopySerialized(oldPopup, newPopup);
                UnityEngine.Object.DestroyImmediate(oldPopup, true);
            }

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (savedPrefab != null)
            {
                Popup popupAsset = savedPrefab.GetComponent(popupType) as Popup;
                TryAddPopupToConfig(popupConfigPath, popupAsset);

                EditorGUIUtility.PingObject(savedPrefab);
                Selection.activeObject = savedPrefab;
                Debug.Log($"Tạo popup thành công: {prefabPath}");
            }
            else
            {
                Debug.LogError("SaveAsPrefabAsset trả về null.");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (instance != null)
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }

            ClearPendingState();
        }
    }

    private static void TryAddPopupToConfig(string popupConfigPath, Popup popupAsset)
    {
        if (string.IsNullOrEmpty(popupConfigPath) || popupAsset == null)
            return;

        var popupConfig = AssetDatabase.LoadAssetAtPath<ScriptableObject>(popupConfigPath);
        if (popupConfig == null)
            return;

        SerializedObject so = new SerializedObject(popupConfig);
        SerializedProperty popupsProp = so.FindProperty("popups");
        if (popupsProp == null || !popupsProp.isArray)
            return;

        bool existed = false;
        for (int i = 0; i < popupsProp.arraySize; i++)
        {
            if (popupsProp.GetArrayElementAtIndex(i).objectReferenceValue == popupAsset)
            {
                existed = true;
                break;
            }
        }

        if (!existed)
        {
            popupsProp.arraySize++;
            popupsProp.GetArrayElementAtIndex(popupsProp.arraySize - 1).objectReferenceValue = popupAsset;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(popupConfig);
            AssetDatabase.SaveAssets();
        }
    }

    private static string SanitizePopupName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return string.Empty;

        string name = rawName.Trim();

        name = name.Replace(" ", "");
        name = name.Replace("-", "");
        name = name.Replace(".", "");

        if (!name.StartsWith("Popup", StringComparison.Ordinal))
        {
            name = "Popup" + name;
        }

        return name;
    }

    private static void CreateFolderIfNeeded(string assetFolder)
    {
        string fullPath = Path.GetFullPath(assetFolder);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
    }

    private static void ClearPendingState()
    {
        SessionState.EraseBool(PendingKey);
        SessionState.EraseString(PopupNameKey);
        SessionState.EraseString(ScriptPathKey);
        SessionState.EraseString(PrefabPathKey);
        SessionState.EraseString(TemplatePrefabPathKey);
        SessionState.EraseString(PopupConfigPathKey);
    }
}
#endif