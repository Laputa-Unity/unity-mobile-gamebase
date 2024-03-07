using PrimeTween;
using UnityEngine;

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
                    Sequence.Create().ChainCallback(()=>container.transform.localScale = Vector3.one*.9f).Chain(Tween.Scale(container.transform, Vector3.one, ConfigController.Game.durationPopup, Ease.OutBack));
                    break;
                case ShowAnimationType.Fade:
                    Tween.Alpha(CanvasGroup, 1, .5f);
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
                    Tween.Alpha(CanvasGroup, 0, .5f).OnComplete(() =>
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

