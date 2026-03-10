#if UNITY_EDITOR
using CustomInspector;
using UnityEngine;

public class PopupCreator : MonoBehaviour
{
    [SerializeField] private PopupConfig popupConfig;
    
    [Header("Editor")] 
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private string popupSavingDirectory;
    [SerializeField] private string scriptSavingDirectory;
    
    public GameObject PopupPrefab => popupPrefab;
    public PopupConfig PopupConfig => popupConfig;
    public string PopupSavingDirectory => popupSavingDirectory;
    public string ScriptSavingDirectory => scriptSavingDirectory;
    
    [Button]
    public void CreateNewPopup()
    {
        PopupCreatorWindow.Open(this);
    }
}
#endif

