using System;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
    public Canvas Canvas => GetComponent<Canvas>();

    private bool _isFirstShow = true;

    protected virtual void OnEnable()
    {
        if (_isFirstShow)
        {
            OnInstantiate();
        }
    }

    protected virtual void OnDisable()
    {
        _isFirstShow = false;
    }

    protected virtual void OnInstantiate()
    {
        
    }

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

    protected virtual void BeforeShow()
    {
    }

    protected virtual void AfterShown()
    {
    }

    protected virtual void BeforeHide()
    {
    }

    protected virtual void AfterHidden()
    {
    }
}