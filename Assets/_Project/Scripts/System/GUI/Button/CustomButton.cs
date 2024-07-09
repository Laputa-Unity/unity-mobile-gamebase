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
public class CustomButton : Selectable, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] private bool useMotion = true;
    [SerializeField] [ShowIf("useMotion")] private float scaleDuration = .2f;
    [SerializeField] [ShowIf("useMotion")] private float scalePercent = .9f;
    [SerializeField] [ShowIf("useMotion")] private Ease scaleEase = Ease.Linear;


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

    private List<Image> _images;
    private Vector3 _currentScale;

    protected override void Awake()
    {
        base.Awake();
        _images = GetComponentsInChildren<Image>().ToList();
        _currentScale = transform.localScale;
    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();
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

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (useMotion)
        {
            Tween.Scale(transform, _currentScale * scalePercent, scaleDuration, scaleEase);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (useMotion)
        {
            Tween.Scale(transform, _currentScale, scaleDuration, scaleEase);
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        if (!gameObject.activeInHierarchy)
            return;

        if (transition == Transition.ColorTint && state == SelectionState.Disabled)
        {
            foreach (var img in _images)
            {
                img.CrossFadeColor(colors.disabledColor * colors.colorMultiplier, instant ? 0f : colors.fadeDuration,
                    true, true);
            }
        }
        else if (transition == Transition.ColorTint && state == SelectionState.Normal)
        {
            foreach (var img in _images)
            {
                img.CrossFadeColor(colors.normalColor * colors.colorMultiplier, instant ? 0f : colors.fadeDuration,
                    true, true);
            }
        }
    }

    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }
}