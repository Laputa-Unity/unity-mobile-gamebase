using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    [SerializeField] private float maxWidth = 1000f;

    [Header("Prefabs & Holders")]
    [SerializeField] private Cell  cellPrefab;
    [SerializeField] private Transform cellHolder;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private Transform blockHolder;

    // Lưu ý: 1D list, độ dài = rows * columns. Vị trí trống sẽ là null.
    [HideInInspector] public List<Cell>  cells;
    [HideInInspector] public List<Block> blocks;

    // Lưu lại kích thước grid để map (r,c) <-> idx
    public int Rows { get; private set; }
    public int Columns { get; private set; }

    const float fallbackBaseSize = 512f;

    public void GenerateBoardFromEditor(int rows, int columns, List<int> cellsLayout, BlockConfig config)
    {
        GenerateInternal(rows, columns, cellsLayout, config);
    }

    private void GenerateInternal(int rows, int columns, List<int> cellsLayout, BlockConfig config)
    {
        if (rows <= 0 || columns <= 0)
        {
            Debug.LogWarning("GenerateBoard: rows/columns chưa hợp lệ.");
            return;
        }
        if (blockHolder == null || cellHolder == null)
        {
            Debug.LogWarning("GenerateBoard: thiếu holder (blockHolder / cellHolder).");
            return;
        }
        if (cellPrefab == null || blockPrefab == null)
        {
            Debug.LogWarning("GenerateBoard: thiếu cellPrefab hoặc blockPrefab.");
            return;
        }

        Rows = rows;
        Columns = columns;

        cellHolder.Clear();
        blockHolder.Clear();

        // Chuẩn bị list 1D
        int size = rows * columns;
        if (cells == null)  cells  = new List<Cell>(size);
        if (blocks == null) blocks = new List<Block>(size);
        cells.Clear();  cells.Capacity  = size;
        blocks.Clear(); blocks.Capacity = size;

        // Kích thước 1 ô
        float cellSize = maxWidth / columns;

        // Scale cho Cell/Block
        float baseWidthCell  = GetPrefabBaseWidth(cellPrefab);
        float baseWidthBlock = GetPrefabBaseWidth(blockPrefab);
        float scaleCell  = cellSize / baseWidthCell;
        float scaleBlock = cellSize / baseWidthBlock;

        float totalHeight = rows * cellSize;
        float startX = -maxWidth * 0.5f + cellSize * 0.5f;
        float startY =  totalHeight * 0.5f - cellSize * 0.5f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int idx = ToIndex(r, c);

                Vector3 localPos = new Vector3(
                    startX + c * cellSize,
                    startY - r * cellSize,
                    0f
                );

                // 1) Spawn CELL (luôn có)
#if UNITY_EDITOR
                GameObject cellGo = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab.gameObject, cellHolder);
                Cell cell = cellGo.GetComponent<Cell>();
#else
                Cell cell = Instantiate(cellPrefab, cellHolder);
#endif
                cell.transform.localRotation = Quaternion.identity;
                cell.transform.localScale    = Vector3.one * scaleCell;
                cell.transform.localPosition = localPos;
                cell.attachBlock = null;
                cells.Add(cell); // list 1D

                // 2) Kiểm tra layout: -1 = ô trống -> không tạo Block
                bool spawnBlock = true;
                int paletteIndex = -1;
                if (cellsLayout != null && idx < cellsLayout.Count)
                {
                    paletteIndex = cellsLayout[idx];
                    if (paletteIndex < 0) spawnBlock = false;
                }

                if (!spawnBlock)
                {
                    blocks.Add(null); // giữ đúng chỉ số
                    continue;
                }

                // 3) Spawn BLOCK
#if UNITY_EDITOR
                GameObject blockGo = (GameObject)PrefabUtility.InstantiatePrefab(blockPrefab.gameObject, blockHolder);
                Block block = blockGo.GetComponent<Block>();
#else
                Block block = Instantiate(blockPrefab, blockHolder);
#endif
                block.transform.localRotation = Quaternion.identity;
                block.transform.localScale    = Vector3.one * scaleBlock;
                block.transform.localPosition = localPos;

                // 4) Set type theo palette
                if (config != null && paletteIndex >= 0 && paletteIndex < config.blockData.Count)
                {
                    var data = config.blockData[paletteIndex];
                    block.SetType(data.blockType);
                }

                // 5) Link
                cell.attachBlock = block;
                blocks.Add(block);
            }
        }
    }

    // Map helpers
    public int ToIndex(int row, int col) => row * Columns + col;
    public (int row, int col) ToRC(int index) => (index / Columns, index % Columns);

    public Block GetBlock(int row, int col)
    {
        int idx = ToIndex(row, col);
        return (idx >= 0 && idx < blocks.Count) ? blocks[idx] : null;
    }
    public Cell GetCell(int row, int col)
    {
        int idx = ToIndex(row, col);
        return (idx >= 0 && idx < cells.Count) ? cells[idx] : null;
    }

    private static float GetPrefabBaseWidth(Component prefabRoot)
    {
        var sr = prefabRoot.GetComponentInChildren<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            return sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
        }
        return fallbackBaseSize;
    }
}
