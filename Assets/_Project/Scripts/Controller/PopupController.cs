using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PopupController : SingletonDontDestroy<PopupController>
{
    public Camera uiCamera;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private PopupConfig popupConfig;

    private readonly Dictionary<Type, Popup> _dictionary = new Dictionary<Type, Popup>();

    protected void Start()
    {
        Initialize();
        Debug.Assert(Camera.main != null, "Camera.main != null");
        canvasScaler.matchWidthOrHeight = Camera.main.aspect > .7f ? 1 : 0;
    }

    public void Initialize()
    {
        int index = 0;
        popupConfig.popups.ForEach(popup =>
        {
            Popup popupInstance = Instantiate(popup, canvasTransform);
            popupInstance.gameObject.SetActive(false);
            popupInstance.Canvas.sortingOrder = index++;
            _dictionary.Add(popupInstance.GetType(), popupInstance);
        });
    }

    public void Show<T>()
    {
        if (_dictionary.TryGetValue(typeof(T), out Popup popup))
        {
            if (!popup.isActiveAndEnabled)
            {
                popup.Show();
            }
        }
    }

    public void Hide<T>()
    {
        if (_dictionary.TryGetValue(typeof(T), out Popup popup))
        {
            if (popup.isActiveAndEnabled)
            {
                popup.Hide();
            }
        }
    }

    public void HideAll()
    {
        foreach (Popup item in _dictionary.Values)
        {
            if (item.isActiveAndEnabled)
            {
                item.Hide();
            }
        }
    }

    public Popup Get<T>()
    {
        if (_dictionary.TryGetValue(typeof(T), out Popup popup))
        {
            return popup;
        }
        return null;
    }
}
