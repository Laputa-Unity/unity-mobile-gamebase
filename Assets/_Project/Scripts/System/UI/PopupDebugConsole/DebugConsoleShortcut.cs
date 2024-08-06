using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DebugConsoleShortcut : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private UnityEvent onPressAction;
    
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private bool _isDrag;

    private void Start()
    {
        _rectTransform = GetComponentInParent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDrag)
        {
            SoundController.Instance.PlayFX(SoundName.ClickButton);
            onPressAction.Invoke();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isDrag = true;
        
        if (Input.touchCount == 1)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDrag = false;
    }
}
