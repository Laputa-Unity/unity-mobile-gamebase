using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CustomInspector;
using CustomTween;
using Lean.Pool;
using Lean.Touch;

public class PopupController : SingletonDontDestroy<PopupController>
{
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Camera uiCamera2;
    [SerializeField] private Transform canvasTransform2;
    [SerializeField] private BlockUI blockUI;
    [SerializeField] private NotifyTagItem notifyTagItemPrefab;
    [SerializeField] private PopupConfig popupConfig;
    [SerializeField] private GameConfig gameConfig;

    [ReadOnly] public Popup currentPopup;

    private readonly Dictionary<Type, Popup> _dictionary = new Dictionary<Type, Popup>();
    private List<Popup> _savingPopups = new List<Popup>();

    public Transform CanvasTransform
    {
        get => canvasTransform;
        set => canvasTransform = value;
    }

    public Transform CanvasTransform2
    {
        get => canvasTransform2;
        set => canvasTransform2 = value;
    }

    protected void Start()
    {
        Initialize();
        blockUI.SetBlockUIState(false);
        if (gameConfig.isTesting)
        {
            InitializeDebugConsole();
        }
        
        Observer.Notify += SpawnNotifyText;
    }

    private void OnDestroy()
    {
        Observer.Notify -= SpawnNotifyText;
    }

    private void SpawnNotifyText(string content, Vector3 position)
    {
        if (string.IsNullOrEmpty(content)) return;
        var notifyText = LeanPool.Spawn(notifyTagItemPrefab, canvasTransform2);
        notifyText.SetText(content);
        notifyText.Action(position);
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

    public void InitializeDebugConsole()
    {
        PopupDebugConsole popupDebugConsole = Instantiate(popupConfig.popupDebugConsole, canvasTransform);
        popupDebugConsole.Canvas.sortingOrder = 999;
        popupDebugConsole.Show(PopupAnimation.None);
    }

    public void Show<T>(PopupAnimation popupAnimation = PopupAnimation.None)
    {
        if (_dictionary.TryGetValue(typeof(T), out Popup popup))
        {
            if (!popup.isActiveAndEnabled)
            {
                popup.Show(popupAnimation);
                currentPopup = popup;
            }
        }
    }

    public void Hide<T>(PopupAnimation popupAnimation = PopupAnimation.None)
    {
        if (_dictionary.TryGetValue(typeof(T), out Popup popup))
        {
            if (popup.isActiveAndEnabled)
            {
                popup.Hide(popupAnimation);
            }
        }
    }

    public void HideAll()
    {
        foreach (Popup item in _dictionary.Values)
        {
            if (item.isActiveAndEnabled)
            {
                item.Hide(PopupAnimation.None);
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

    public void SavePopups()
    {
        _savingPopups.Clear();
        foreach (Popup item in _dictionary.Values)
        {
            if (item.isActiveAndEnabled)
            {
                _savingPopups.Add(item);
            }
        }
    }

    public void LoadPopups()
    {
        foreach (var popup in _savingPopups.ToList())
        {
            popup.Show(PopupAnimation.None);
        }
    }
    
    public void SetTargetUI(bool isActive, TargetUIType targetUIType = TargetUIType.Frame, RectTransform targetTransform = null, float timeDelay = 0)
    {
        blockUI.SetTargetUI(isActive, targetUIType, targetTransform, timeDelay);
    }
    
    public void SetBlockUIState(bool isActive, BlockType blockType = BlockType.Black, bool useFetching = false, float timeDelay = 0,  Action onComplete = null)
    {
        blockUI.SetBlockUIState(isActive, blockType, useFetching, timeDelay, onComplete);
    }

    public void BringToFront(GameObject go)
    {
        go.transform.SetParent(canvasTransform2);
        go.layer = LayerMask.NameToLayer("UI2");
        Utility.SetLayerRecursively(go, LayerMask.NameToLayer("UI2"));
    }

    public void BringBack(GameObject go, RectTransform rectTransform)
    {
        go.transform.SetParent(rectTransform);
        go.layer = LayerMask.NameToLayer("UI");
        Utility.SetLayerRecursively(go, LayerMask.NameToLayer("UI"));
    }
}
