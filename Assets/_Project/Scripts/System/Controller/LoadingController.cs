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
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI loadingText;
    private AsyncOperation _sceneOperation;
    
    void Start()
    {
        _sceneOperation = SceneManager.LoadSceneAsync("GameplayScene");
        _sceneOperation.allowSceneActivation = false;

        slider.value = 0;
        Tween.UISliderValue(slider, 100, timeLoading).OnComplete(() =>
        {
            _sceneOperation.allowSceneActivation = true;
        });
    }

    public void OnSliderValueChanged()
    {
        loadingText.text = $"Loading {(int) (slider.value)}%";
    }
}