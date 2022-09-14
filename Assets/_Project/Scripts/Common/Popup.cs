using UnityEngine;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    public CanvasGroup CanvasGroup { get; set; }
    public Canvas Canvas { get; set; }

    private void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        Canvas = GetComponent<Canvas>();
    }

    public void Show()
    {
        BeforeShow();
        gameObject.SetActive(true);
        CanvasGroup.DOFade(1, ConfigController.Game.DurationPopup).OnComplete(() =>
        {
            CanvasGroup.interactable = true;
            AfterShown();
        });
    }

    public void Hide()
    {
        BeforeHide();
        CanvasGroup.interactable = false;
        gameObject.SetActive(false);
        AfterHidden();
    }

    protected virtual void AfterInstantiate() { }
    protected virtual void BeforeShow() { }
    protected virtual void AfterShown() { }
    protected virtual void BeforeHide() { }
    protected virtual void AfterHidden() { }
}
