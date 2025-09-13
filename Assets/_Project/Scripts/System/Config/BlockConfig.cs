using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockConfig", menuName = "ScriptableObject/BlockConfig")]
public class BlockConfig : ScriptableObject
{
    public List<BlockData> blockData;

    public BlockData GetBlockData(BlockType type)
    {
        return blockData.Find(item => item.blockType == type);
    }
}

[Serializable]
public class BlockData
{
    public BlockType blockType;
    public Texture texture;
    public Sprite sprite;
}
