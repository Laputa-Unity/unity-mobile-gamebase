using CustomInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform skinHolder;
    
    void Start()
    {
        SetSkin(Data.PlayerData.CurrentSkin);
    
        Observer.EquipPlayerSkin += SetSkin;
    }

    private void OnDestroy()
    {
        Observer.EquipPlayerSkin -= SetSkin;
    }

    private void SetSkin(string itemIdentity)
    {
        var item = ItemController.Instance.GetItemData(itemIdentity);
        var skinPrefab = item.skinPrefab;
        skinHolder.Clear();
        Instantiate(skinPrefab, skinHolder);
    }

    [Button]
    public void RandomSkin()
    {
        var item = ItemController.Instance.GetRandomItemData();
        var skinPrefab = item.skinPrefab;
        
        skinHolder.Clear();
        Instantiate(skinPrefab, skinHolder);
        
        Data.PlayerData.CurrentSkin = item.identity;
    }
}
