using System;
using CustomTween;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float timeLoading = 5f;

    [Header("Components")] 
    [SerializeField] private Image titleFrame;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Image logo;
    [SerializeField] private Image background;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    private AsyncOperation _sceneOperation;

    void Start()
    {
        _sceneOperation = SceneManager.LoadSceneAsync("GameplayScene");
        _sceneOperation.allowSceneActivation = false;
        
        progressBar.fillAmount = 0;
        Tween.Alpha(titleFrame, 1, timeLoading);
        Tween.Alpha(title, 1, timeLoading);
        Tween.Alpha(logo, 1, timeLoading);
        Tween.Alpha(background, 1, timeLoading);
        Tween.UIFillAmount(progressBar,1, timeLoading).OnUpdate(loadingText, (loadingText, tween) =>
        {
            loadingText.text = $"Loading {(int) (progressBar.fillAmount * 100)}%";
        }).OnComplete(()=>
        {
            _sceneOperation.allowSceneActivation = true;
        });
    }
}
