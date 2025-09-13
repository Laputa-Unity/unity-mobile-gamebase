using CustomInspector;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] [OnValueChanged(nameof(OnBlockTypeChanged))] private BlockType blockType;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetType(BlockType newType)
    {
        blockType = newType;
        OnBlockTypeChanged();
    }

    private void OnBlockTypeChanged()
    {
        var blockConfig = Resources.Load<BlockConfig>("BlockConfig");
        var blockData = blockConfig.GetBlockData(blockType);
        if (spriteRenderer != null && blockData != null)
        {
            spriteRenderer.sprite = blockData.sprite;
        }
    }
}


public enum BlockType
{
    Type1,
    Type2,
    Type3,
    Type4,
    Type5,
    Type6,
    Type7,
    Type8,
    Type9,
    Type10,
    Type11,
    Type12,
    Type13,
    Type14,
    Type15,
    Type16,
    Type17,
    Type18,
    Type19,
    Type20
}