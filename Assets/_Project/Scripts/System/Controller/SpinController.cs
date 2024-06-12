using System.Collections.Generic;
using UnityEngine;

public class SpinController : SingletonDontDestroy<SpinController>
{
    [SerializeField] private SpinConfig spinConfig;

    public List<SlotItemData> GetSlotItemData()
    {
        return spinConfig.slotData;
    }
}
