using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [Header("Attributes")] 
    [SerializeField] private float timeLoading = 5f;
    
    [Header("Components")]
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    private AsyncOperation _sceneOperation;

    void Start()
    {
        _sceneOperation = SceneManager.LoadSceneAsync(Constant.GameplayScene);
        _sceneOperation.allowSceneActivation = false;
        
        progressBar.fillAmount = 0;
        Tween.UIFillAmount(progressBar,1, timeLoading).OnUpdate(loadingText, (loadingText, tween) => loadingText.text = $"Loading {(int) (progressBar.fillAmount * 100)}%").OnComplete(()=>
        {
            _sceneOperation.allowSceneActivation = true;
        });
    }
}
