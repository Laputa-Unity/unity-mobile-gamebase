using System.Collections.Generic;
using CustomInspector;
using CustomTween;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float minBreakingSpeed = 1f;
    [SerializeField] private float increaseSpeedTime = 2.5f;
    [SerializeField] private float maxSpeedHoldingTime = 2f;
    [SerializeField] private float breakingTime = 3f;
    [SerializeField] private float findingTargetSlotAtSpeed = 3f;
    [SerializeField] private Transform wheelTransform;
    [SerializeField] private List<SlotItem> slotItems;
    
    [SerializeField] [ReadOnly] private float speed;
    [SerializeField] [ReadOnly] public SlotItem currentSlotItem;
    [SerializeField] [ReadOnly] public SlotItem targetSlotItem;
    
    private PopupSpin _popupSpin;
    private bool _isSpinning;
    private bool _isBreaking;
    private bool _isFindingTarget;
    private int _randomWeight;
    
    private readonly List<Tween> _tweenCache = new List<Tween>();

    public void Setup(PopupSpin popupSpin, List<SlotItemData> slotItemData)
    {
        _popupSpin ??= popupSpin;
        for (int i = 0; i < slotItemData.Count; i++)
        {
            slotItems[i].Setup(_popupSpin, slotItemData[i]);
        }
    }
    
    public void OnSpin()
    {
        _randomWeight = Random.Range(0, 100);
        targetSlotItem = slotItems.Find(item => item.IsTarget(_randomWeight));

        ClearTweenCache();
        _isFindingTarget = false;
        _isSpinning = true;
        _tweenCache.Add(Tween.Custom( speed, maxSpeed, increaseSpeedTime, value => { speed = value;} ).OnComplete(() =>
        {
            _tweenCache.Add(Tween.Delay(maxSpeedHoldingTime, () =>
            {
                _isBreaking = true;
            }));
        }));
    }

    private void Update()
    {
        if (_isSpinning)
        {
            wheelTransform.transform.RotateAround(wheelTransform.transform.position, Vector3.forward, Time.deltaTime * 90f * speed);
            if (_isBreaking)
            {
                _isBreaking = false;
                _tweenCache.Add(Tween.Custom(speed, minBreakingSpeed, breakingTime, value =>
                {
                    speed = value;
                    if (speed <= findingTargetSlotAtSpeed)
                    {
                        _isFindingTarget = true;
                    }
                }));
            }
        }

        if (_isFindingTarget)
        {
            if (wheelTransform.localEulerAngles.z % 45 <= 2 && currentSlotItem == targetSlotItem)
            {
                ClearTweenCache();
                speed = 0;
                _isFindingTarget = false;
                targetSlotItem = null;
                _isSpinning = false;
                _popupSpin.OnStopWheel();
            }
        }
    }

    private void ClearTweenCache()
    {
        foreach (var tween in _tweenCache)
        {
            tween.Stop();
        }
        _tweenCache.Clear();
    }

    public void OnStopWheel()
    {
        currentSlotItem.ClaimReward();
    }
}
    