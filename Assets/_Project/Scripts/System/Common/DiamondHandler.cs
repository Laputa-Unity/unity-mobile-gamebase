using TMPro;
using UnityEngine;

public class DiamondHandler : ResourceHandler
{
    [Header("Diamond Settings")]
    [SerializeField] private Resource diamondPrefab;
    [SerializeField] private GameObject diamondTarget;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private GameObject diamondBar;

    protected override Resource Prefab => diamondPrefab;
    protected override GameObject Target => diamondTarget;
    protected override TextMeshProUGUI AmountText => diamondText;
    protected override GameObject Bar => diamondBar;

    protected override int Cache { get; set; }

    protected override int CurrentValue => Data.PlayerData.CurrentDiamond;
  
    protected override void SubscribeEvents()
    {
        Observer.DiamondChanged += OnDiamondChanged;
    }

    protected override void UnsubscribeEvents()
    {
        Observer.DiamondChanged -= OnDiamondChanged;
    }

    private void OnDiamondChanged(int amount)
    {
        if (amount >= 0) Increase(amount);
        else Decrease(-amount);
    }
    
    protected override void OnCollectedEffect(GameObject target)
    {
        VFXController.Instance.SpawnEffect(
            EffectName.SparkleDiamond,
            Vector3.zero,
            target.transform,
            0.5f
        );
    }
}