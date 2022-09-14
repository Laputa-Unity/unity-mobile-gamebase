using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [Header("Components")]
    public Image ProgressBar;
    public TextMeshProUGUI LoadingText;

    [Header("Attributes")] 
    [Range(0.1f, 10f)] public float TimeLoading = 5f;

    private bool flagDoneProgress;
    private AsyncOperation _operation;

    public void Awake()
    {
        AdsManager.Initialize();
        FirebaseManager.Initialize();
    }
    
    void Start()
    {
        _operation = SceneManager.LoadSceneAsync(Constant.GAMEPLAY_SCENE);
        _operation.allowSceneActivation = false;

        ProgressBar.fillAmount = 0;
        ProgressBar.DOFillAmount(5, TimeLoading).OnUpdate(()=>LoadingText.text = $"Loading... {(int) (ProgressBar.fillAmount * 100)}%").OnComplete(()=> flagDoneProgress = true);
        WaitProcess();
    }
    
    private async void WaitProcess()
    {
        await UniTask.WaitUntil(() => AdsManager.IsInitialized && FirebaseManager.IsInitialized && flagDoneProgress);

        _operation.allowSceneActivation = true;
    }
}
