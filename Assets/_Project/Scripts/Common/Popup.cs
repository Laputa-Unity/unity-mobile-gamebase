using UnityEngine;

public class Popup : MonoBehaviour
{
    public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
    public Canvas Canvas => GetComponent<Canvas>();

    public virtual void Show()
    {
        BeforeShow();
        gameObject.SetActive(true);
        AfterShown();
    }

    public virtual void Hide()
    {
        BeforeHide();
        gameObject.SetActive(false);
        AfterHidden();
    }
    
    protected virtual void BeforeShow() { }
    protected virtual void AfterShown() { }
    protected virtual void BeforeHide() { }
    protected virtual void AfterHidden() { }
}
