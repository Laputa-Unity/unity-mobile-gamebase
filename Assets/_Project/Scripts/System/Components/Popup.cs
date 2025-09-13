using System;
using CustomTween;
using UnityEngine;
using UnityEngine.Serialization;

public class Popup : MonoBehaviour
{
    [SerializeField] protected RectTransform background;
    [SerializeField] protected RectTransform container;
    [FormerlySerializedAs("hideScaleMaxValue")]
    [FormerlySerializedAs("hideScaleStartValue")]
    [Header("Hide animation config")] 
    [SerializeField] private Vector3 scaleMaxValue = Vector3.one;
    [SerializeField] private Vector3 scaleMinValue = new Vector3(.2f, .2f, .2f);
    [SerializeField] private float alphaMaxValue = 1;
    [SerializeField] private float alphaMinValue = 0;
    [SerializeField] private float animationDuration = .5f;
    [SerializeField] private float delayDuration;
    [SerializeField] private Ease showEase;
    [SerializeField] private Ease hideEase;

    
    protected bool IsShowing;
    protected bool IsHiding;
        
    private bool _isFirstShow = true;
    private Action _afterHiddenAction;
    private Action _beforeHiddenAction;

    public Action AfterHiddenAction
    {
        get => _afterHiddenAction;
        set => _afterHiddenAction = value;
    }

    public Action BeforeHiddenAction
    {
        get => _beforeHiddenAction;
        set => _beforeHiddenAction = value;
    }

    public bool IsFirstShow
    {
        get => _isFirstShow;
        set => _isFirstShow = value;
    }
    
    public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
    public Canvas Canvas => GetComponent<Canvas>();

    protected virtual void OnEnable()
    {
        CanvasGroup.alpha = 1;
        transform.localScale = Vector3.one;
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

    public virtual void Show(PopupAnimation animation = PopupAnimation.None)
    {
        BeforeShow();
        switch (animation)
        {
            case PopupAnimation.None:
                gameObject.SetActive(true);
                AfterShown();
                break;
            case PopupAnimation.ScaleFade:
                IsShowing = true;
                gameObject.SetActive(true);
                Tween.Alpha(CanvasGroup, alphaMinValue, alphaMaxValue, animationDuration, useUnscaledTime: true);
                Tween.Scale(container, scaleMinValue, scaleMaxValue, animationDuration, showEase,
                    startDelay: delayDuration, useUnscaledTime: true).OnComplete(
                    () =>
                    {
                        IsShowing = false;
                        AfterShown();
                    });
                break;
            case PopupAnimation.ScaleFade2:
                IsShowing = true;
                gameObject.SetActive(true);
                Tween.Alpha(CanvasGroup, alphaMinValue, alphaMaxValue, animationDuration, useUnscaledTime: true);
                Tween.Scale(container,scaleMaxValue, scaleMinValue, animationDuration, showEase, startDelay: delayDuration, useUnscaledTime: true).OnComplete(
                    () =>
                    {
                        IsShowing = false;
                        AfterShown();
                    });
                break;
            case PopupAnimation.FadeOnly:
                IsShowing = true;
                gameObject.SetActive(true);
                Tween.Alpha(CanvasGroup, alphaMinValue, alphaMaxValue, animationDuration, useUnscaledTime: true).OnComplete(()=>
                {
                    IsShowing = false;
                    AfterShown();
                });
                break;
        }
        
    }

    public virtual void Hide(PopupAnimation animation = PopupAnimation.None)
    {
        if (IsHiding) return;
        BeforeHide();
        switch (animation)
        {
            case PopupAnimation.None:
                gameObject.SetActive(false);
                AfterHidden();
                break;
            case PopupAnimation.ScaleFade:
                IsHiding = true;
                Tween.Alpha(CanvasGroup, alphaMaxValue, alphaMinValue, animationDuration, useUnscaledTime: true);
                Tween.Scale(container,scaleMaxValue, scaleMinValue, animationDuration, hideEase, startDelay: delayDuration, useUnscaledTime: true).OnComplete(
                    () =>
                    {
                        IsHiding = false;
                        gameObject.SetActive(false);
                        AfterHidden();
                    });
                break;
            case PopupAnimation.ScaleFade2:
                IsHiding = true;
                Tween.Alpha(CanvasGroup, alphaMinValue, alphaMaxValue, animationDuration, useUnscaledTime: true);
                Tween.Scale(container,scaleMinValue, scaleMaxValue, animationDuration, showEase, startDelay: delayDuration, useUnscaledTime: true).OnComplete(
                    () =>
                    {
                        IsHiding = false;
                        gameObject.SetActive(false);
                        AfterHidden();
                    });
                break;
            case PopupAnimation.FadeOnly:
                IsHiding = true;
                Tween.Alpha(CanvasGroup, alphaMaxValue, alphaMinValue, animationDuration, useUnscaledTime: true).OnComplete(
                    () =>
                    {
                        IsHiding = false;
                        gameObject.SetActive(false);
                        AfterHidden();
                    });
                break;
        }
        
    }

    protected virtual void BeforeShow()
    {
    }

    protected virtual void AfterShown()
    {
    }

    protected virtual void BeforeHide()
    {
        BeforeHiddenAction?.Invoke();
        BeforeHiddenAction = null;
    }

    protected virtual void AfterHidden()
    {
        AfterHiddenAction?.Invoke();
        AfterHiddenAction = null;
    }
}

public enum PopupAnimation
{
    None,
    ScaleFade,
    ScaleFade2,
    FadeOnly,
}