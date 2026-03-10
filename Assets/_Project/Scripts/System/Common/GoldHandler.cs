using TMPro;
using UnityEngine;

public class GoldHandler : ResourceHandler
{
    [Header("Gold Settings")]
    [SerializeField] private Resource goldPrefab;
    [SerializeField] private GameObject goldTarget;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GameObject goldBar;

    protected override Resource Prefab => goldPrefab;
    protected override GameObject Target => goldTarget;
    protected override TextMeshProUGUI AmountText => goldText;
    protected override GameObject Bar => goldBar;

    protected override int Cache { get; set; }

    protected override int CurrentValue => Data.PlayerData.CurrentGold;

    protected override void SubscribeEvents()
    {
        Observer.GoldChanged += OnGoldChanged;
    }

    protected override void UnsubscribeEvents()
    {
        Observer.GoldChanged -= OnGoldChanged;
    }

    private void OnGoldChanged(int amount)
    {
        if (amount > 0) Increase(amount);
        else Decrease(-amount);
    }
    
    protected override void OnCollectedEffect(GameObject target)
    {
        VFXController.Instance.SpawnEffect(
            EffectName.SparkleGold,
            Vector3.zero,
            target.transform,
            0.5f
        );
    }
}