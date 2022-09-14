#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public class GameBase : EditorWindow
{
    void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.EndHorizontal();
    }
    
    [MenuItem("GameBase/OPEN SCENE/Loading Scene %F1")]
    public static void PlayFromLoadingScene(){
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.LOADING_SCENE}.unity");
    }

    [MenuItem("GameBase/OPEN SCENE/Gameplay Scene %F2")]
    public static void PlayFromGamePlayScene(){
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.GAMEPLAY_SCENE}.unity");
    } 

    
    [MenuItem("GameBase/CLEAR DATA/CLEAR ALL %F3")]
    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
#endif
