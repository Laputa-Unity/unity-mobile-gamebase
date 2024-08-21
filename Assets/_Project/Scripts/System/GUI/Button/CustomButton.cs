using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomInspector;
using CustomTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Custom Button", 30)]
public class CustomButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
    IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] [FormerlySerializedAs("Interactable")] private bool interactable = true;
    [SerializeField] private Color normalColor = Color.white;

    [SerializeField] private ButtonDisableType disableType = ButtonDisableType.None;
    [SerializeField] [ShowIf("disableType", ButtonDisableType.Color)] private Color disableColor = new Color(200/255f, 200/255f, 200/255f, 128/255f);
    [SerializeField] [ShowIf("disableType", ButtonDisableType.Sprite)] private Sprite normalSprite;
    [SerializeField] [ShowIf("disableType", ButtonDisableType.Sprite)] private Sprite disableSprite;
    
    [SerializeField] private bool useFadeColorMotion = true; 
    [SerializeField] [ShowIf("useFadeColorMotion")] private bool affectSelf;
    [SerializeField] [ShowIf("useFadeColorMotion")] [Min(0)] private float fadeDuration = .1f;
    [SerializeField] [ShowIf("useFadeColorMotion")] private Color pressColor = new Color(200/255f, 200/255f, 200/255f, 255/255f);
    
    [SerializeField] private bool useScaleMotion = true;
    [SerializeField] [ShowIf("useScaleMotion")] private float scaleDuration = .2f;
    [SerializeField] [ShowIf("useScaleMotion")] private float scalePercent = .9f;
    [SerializeField] [ShowIf("useScaleMotion")] private Ease scaleEase = Ease.Default;
    
    [SerializeField] [ReadOnly] private ButtonPressState buttonPressState;
    private Image targetImage => GetComponent<Image>();
    
    public bool Interactable
    {
        get => interactable;
        set
        {
            interactable = value;
            SetupButtonInteractable(interactable, false);
        }
    }

    [Serializable]
    public class ButtonClickedEvent : UnityEvent
    {
    }

    // Event delegates triggered on click.
    [FormerlySerializedAs("onClick")] [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    protected CustomButton()
    {
        
    }


    public ButtonClickedEvent onClick
    {
        get => m_OnClick;
        set => m_OnClick = value;
    }

    private List<Image> images => GetComponentsInChildren<Image>().ToList();
    private Vector3 _currentScale;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetupButtonInteractable(interactable, true);
    }
#endif
    
    protected override void Awake()
    {
        base.Awake();
        _currentScale = transform.localScale;
    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;
        
        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();
    }

    private bool IsInteractable()
    {
        return interactable;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        Press();
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        Press();
        // if we get set disabled during the press
        // don't run the coroutine.
        if (!IsActive() || !IsInteractable())
            return;
        
        StartCoroutine(OnFinishSubmit());
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;
        buttonPressState = ButtonPressState.Pressed;
        if (useFadeColorMotion)
        {
            DoStateTransition(ButtonPressState.Pressed, false, affectSelf);
        }
        if (useScaleMotion)
        {
            Tween.Scale(transform, _currentScale * scalePercent, scaleDuration, scaleEase);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        buttonPressState = ButtonPressState.Normal;
        if (useFadeColorMotion)
        {
            DoStateTransition(ButtonPressState.Normal, false, affectSelf);
        }
        if (useScaleMotion)
        {
            Tween.Scale(transform, _currentScale, scaleDuration, scaleEase);
        }
    }

    protected virtual void DoStateTransition(ButtonPressState state, bool instant, bool affectSelf)
    {
        if (!gameObject.activeInHierarchy)
            return;
        
        switch (state)
        {
            case ButtonPressState.Normal:
                if (affectSelf)
                {
                    Tween.Color(targetImage, normalColor, fadeDuration);
                }
                else
                {
                    foreach (var img in images)
                    {
                        Tween.Color(img, normalColor, fadeDuration);
                    }
                }
                break;
            case ButtonPressState.Pressed:
                if (affectSelf)
                {
                    Tween.Color(targetImage, pressColor, fadeDuration);
                }
                else
                {
                    foreach (var img in images)
                    {
                        Tween.Color(img, pressColor, fadeDuration);
                    }
                }
                break;
        }
    }

    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = fadeDuration;
        var elapsedTime = 0f;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
        yield return null;
        }
        
        DoStateTransition(ButtonPressState.Pressed, true, affectSelf);
    }

    private void SetupButtonInteractable(bool isInteracted, bool instant)
    {
        if (isInteracted)
        {
            switch (disableType)
            {
                case ButtonDisableType.None:
                    break;
                case ButtonDisableType.Color:
                    if (instant)
                    {
                        foreach (var img in images)
                        {
                            img.color = normalColor;
                        }
                    }
                    else
                    {
                        foreach (var img in images)
                        {
                            img.CrossFadeColor(normalColor, 0, true, true);
                        }
                    }
                    break;
                case ButtonDisableType.Sprite:
                    targetImage.sprite = normalSprite;
                    break;
            }
        }
        else
        {
            switch (disableType)
            {
                case ButtonDisableType.None:
                    break;
                case ButtonDisableType.Color:
                    if (instant)
                    {
                        foreach (var img in images)
                        {
                            img.color = disableColor;
                        }
                    }
                    else
                    {
                        foreach (var img in images)
                        {
                            img.CrossFadeColor(disableColor, 0, true, true);
                        }
                    }
                    break;
                case ButtonDisableType.Sprite:
                    targetImage.sprite = disableSprite;
                    break;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        
    }

    public void OnDeselect(BaseEventData eventData)
    {
        
    }
}

public enum ButtonPressState
{
    Normal,
    Pressed,
}

public enum ButtonDisableType
{
    None,
    Color,
    Sprite,
}