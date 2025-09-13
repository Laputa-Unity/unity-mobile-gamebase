using CustomTween;
using Lean.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotifyTagItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notifyText;
    [SerializeField] private float offsetY = 200;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content)
    {
        notifyText.text = $"{content}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    public void Action(Vector3 position)
    {
        _rectTransform.position = position;
        canvasGroup.alpha = 1;
        Tween.LocalPosition(_rectTransform, _rectTransform.localPosition + Vector3.up * offsetY, 2f, Ease.OutCubic);
        Tween.Scale(_rectTransform, Vector3.one * .6f, Vector3.one, .2f);
        Tween.Alpha(canvasGroup, 0, 1.5f, startDelay: 1f).OnComplete(()=> LeanPool.Despawn(gameObject));
    }
}
