using UnityEngine;
using Lean.Touch;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DebugConsoleShortcut : MonoBehaviour
{
    [SerializeField] private UnityEvent action; // Exposed in Inspector

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _fingerDownPos;
    private bool _isDrag;
    private bool _isOverThisObject;
    private const float TapThreshold = 10f; // Distance threshold to detect a tap

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>(); // Ensure we have the correct canvas
    }

    // **Check if Finger is Over THIS GameObject**
    private bool IsFingerOverThisObject(LeanFinger finger)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = finger.ScreenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var hit in results)
        {
            if (hit.gameObject == gameObject) // Check if hit is THIS object
            {
                return true;
            }
        }
        return false;
    }

    // Called when a finger begins touching
    public void OnFingerDown(LeanFinger finger)
    {
        if (IsFingerOverThisObject(finger)) // Check if tap started on this object
        {
            _isOverThisObject = true;
            _fingerDownPos = finger.ScreenPosition;
        }
        else
        {
            _isOverThisObject = false; // Ignore if tap started elsewhere
        }
    }

    // Called when a finger is released
    public void OnFingerUp(LeanFinger finger)
    {
        if (!_isOverThisObject) return; // Ignore if tap started elsewhere

        float distance = Vector2.Distance(_fingerDownPos, finger.ScreenPosition);

        if (distance < TapThreshold) // Small movement = Tap
        {
            Debug.Log("✅ Tap on DebugConsoleShortcut - Invoking Action!");
            action?.Invoke();
        }
        else
        {
            Debug.Log("❌ Drag detected, no action triggered.");
        }

        _isOverThisObject = false;
    }

    // Called when a finger is dragged
    public void OnFingerDrag(LeanFinger finger)
    {
        _isDrag = true;

        // Move object when dragging
        if (_isOverThisObject)
        {
            Vector2 delta = finger.ScreenDelta / _canvas.scaleFactor;
            if (finger.Index == LeanTouch.HOVER_FINGER_INDEX)
            {
                delta *= 0.6f;
            }
            _rectTransform.anchoredPosition += delta;
        }
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUp += OnFingerUp;
        LeanTouch.OnFingerUpdate += OnFingerDrag;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUp -= OnFingerUp;
        LeanTouch.OnFingerUpdate -= OnFingerDrag;
    }
}
