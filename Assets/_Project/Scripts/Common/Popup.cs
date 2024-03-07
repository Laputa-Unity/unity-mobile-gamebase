using UnityEngine;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    [SerializeField] private bool useAnimation;
    public GameObject background;
    public GameObject container;
    [SerializeField] private bool useShowAnimation;
    [SerializeField] private ShowAnimationType showAnimationType;
    [SerializeField] private bool useHideAnimation;
    [SerializeField] private HideAnimationType hideAnimationType;
    
    public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
    public Canvas Canvas => GetComponent<Canvas>();

    public virtual void Show()
    {
        BeforeShow();
        gameObject.SetActive(true);
        if (useShowAnimation)
        {
            switch (showAnimationType)
            {
                case ShowAnimationType.OutBack:
                    DOTween.Sequence().OnStart(() => container.transform.localScale = Vector3.one*.9f)
                        .Append(container.transform.DOScale(Vector3.one, ConfigController.Game.durationPopup).SetEase(Ease.OutBack));
                    break;
                case ShowAnimationType.Fade:
                    CanvasGroup.DOFade(1, ConfigController.Game.durationPopup);
                    break;
            }
            AfterShown();
        }
        else
        {
            AfterShown();
        }
    }

    public virtual void Hide()
    {
        BeforeHide();
        if (useHideAnimation)
        {
            switch (hideAnimationType)
            {
                case HideAnimationType.Fade:
                    CanvasGroup.DOFade(0, ConfigController.Game.durationPopup).OnComplete(() =>
                    {
                        CanvasGroup.alpha = 1;
                        gameObject.SetActive(false);
                    });
                    break;
            }
            AfterHidden();
        }
        else
        {
            gameObject.SetActive(false);
            AfterHidden();
        }
    }
    
    protected virtual void BeforeShow() { }
    protected virtual void AfterShown() { }
    protected virtual void BeforeHide() { }
    protected virtual void AfterHidden() { }
}

public enum ShowAnimationType
{
    OutBack,
    Fade
}

public enum HideAnimationType
{
    Fade,
}

