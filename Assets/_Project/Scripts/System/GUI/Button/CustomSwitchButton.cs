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
[AddComponentMenu("UI/Custom Switch Button", 30)]
public class CustomSwitchButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
    IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] private bool isOn = true;
    [SerializeField] private SwitchButtonChangeStyle switchButtonChangeStyle = SwitchButtonChangeStyle.Color;
    
    [SerializeField] [ShowIf("switchButtonChangeStyle", SwitchButtonChangeStyle.Color)] private bool affectSelf;
    [SerializeField] [ShowIf("switchButtonChangeStyle", SwitchButtonChangeStyle.Color)] private Color onColor = Color.white;
    [SerializeField] [ShowIf("switchButtonChangeStyle", SwitchButtonChangeStyle.Color)] private Color offColor = Color.black;

    [SerializeField] private bool useScaleMotion = true;
    [SerializeField] [ShowIf("useScaleMotion")] private float scaleDuration = .2f;
    [SerializeField] [ShowIf("useScaleMotion")] private float scalePercent = .9f;
    [SerializeField] [ShowIf("useScaleMotion")] private Ease scaleEase = Ease.Default;

    [SerializeField] [ReadOnly] private SwitchButtonPressState switchButtonPressState;
    private Image targetImage => GetComponent<Image>();

    public bool IsOn
    {
        get => isOn;
        set => isOn = value;
    }

    [Serializable]
    public class ButtonClickedEvent : UnityEvent
    {
        
    }

    // Event delegates triggered on click.
    [FormerlySerializedAs("onClick")] [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
    
    [Serializable]
    public class StateEvent : UnityEvent
    {
    }

    // Event delegates triggered on click.
    [FormerlySerializedAs("onStateChange")] [SerializeField]
    private StateEvent m_OnStateChange = new StateEvent();
    
    

    protected CustomSwitchButton()
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
        SetupSwitchButton(isOn, affectSelf);
    }
#endif
    
    protected override void Awake()
    {
        base.Awake();
        _currentScale = transform.localScale;
    }

    private void Press()
    {
        if (!IsActive())
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
        if (!IsActive())
            return;
        StartCoroutine(OnFinishSubmit());
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        switchButtonPressState = SwitchButtonPressState.Pressed;
        isOn = !isOn;
        SetupSwitchButton(isOn, affectSelf);
        if (useScaleMotion)
        {
            Tween.Scale(transform, _currentScale * scalePercent, scaleDuration, scaleEase);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        switchButtonPressState = SwitchButtonPressState.Normal;
        if (useScaleMotion)
        {
            Tween.Scale(transform, _currentScale, scaleDuration, scaleEase);
        }
    }
    
    protected virtual void SetupSwitchButton(bool state, bool affectSelf)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (state)
        {
            if (affectSelf)
            {
                targetImage.color = onColor;
            }
            else
            {
                foreach (var img in images)
                {
                    img.color = onColor;
                }
            }
        }
        else
        {
            if (affectSelf)
            {
                targetImage.color = offColor;
            }
            else
            {
                foreach (var img in images)
                {
                    img.color = offColor;
                }
            }
        }
        
        m_OnStateChange?.Invoke();
    }

    private IEnumerator OnFinishSubmit()
    {
        isOn = !isOn;
        SetupSwitchButton(isOn, affectSelf);
        yield return null;
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

public enum SwitchButtonPressState
{
    Normal,
    Pressed,
}

public enum SwitchButtonChangeStyle
{
    Color,
}
