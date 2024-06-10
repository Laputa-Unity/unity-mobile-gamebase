using UnityEngine;

public class CanvasScaleHandler : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private UnityEngine.UI.CanvasScaler canvasScaler;
    private void Awake()
    {
        float currentRatio = 1080f / 1920;
        float newRatio = (float) Screen.width / Screen.height;
        SetupCanvasScaler(newRatio);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        if (camera != null && canvasScaler != null)
        {
            float currentRatio = 1080f / 1920;
            float newRatio = (float) camera.pixelWidth / camera.pixelHeight;
            SetupCanvasScaler(newRatio);
        }
    }

    private void SetupCanvasScaler(float ratio)
    {
        canvasScaler.matchWidthOrHeight = ratio > .65f ? 1 : 0;
    }
}
