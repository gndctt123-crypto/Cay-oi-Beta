using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [System.Serializable]
    public class GridCell
    {
        public int x;
        public int y;
        public Vector2 worldPosition;
        public bool hasPlant;
        public GameObject currentPlant;
    }

    public GridCell[,] grid;

    [Header("Grid Setup")]
    public float cellWidth = 1.0f;
    public float cellHeight = 1.2f;
    public Vector2 startPosition;

    private SpriteRenderer ghostRenderer;

    void Awake()
    {
        InitializeGrid();
        GameObject ghostObj = new GameObject("GhostPlant");
        ghostObj.transform.SetParent(this.transform); // Làm con để hủy cùng lúc
        ghostRenderer = ghostObj.AddComponent<SpriteRenderer>();
        ghostRenderer.color = new Color(1f, 1f, 1f, 0.5f); // Bán trong suốt
        ghostRenderer.sortingOrder = 50; // Luôn hiển thị trên cùng
        ghostRenderer.enabled = false;
    }

    private void InitializeGrid()
    {
        grid = new GridCell[GameConstants.GridColumns, GameConstants.GridRows];
        for (int x = 0; x < GameConstants.GridColumns; x++)
        {
            for (int y = 0; y < GameConstants.GridRows; y++)
            {
                grid[x, y] = new GridCell
                {
                    x = x,
                    y = y,
                    worldPosition = new Vector2(startPosition.x + x * cellWidth, startPosition.y - y * cellHeight),
                    hasPlant = false
                };
            }
        }
    }

    // Lấy toạ độ thế giới của ô gần với vị trí chuột nhất
    public GridCell GetCellFromWorldPosition(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - startPosition.x) / cellWidth);
        int y = Mathf.RoundToInt((startPosition.y - worldPos.y) / cellHeight);

        if (x >= 0 && x < GameConstants.GridColumns && y >= 0 && y < GameConstants.GridRows)
        {
            return grid[x, y];
        }
        return null;
    }

    public bool isDiggingMode = false;

    private void Update()
    {
        if (UnityEngine.InputSystem.Mouse.current == null) return;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        GridCell targetCell = GetCellFromWorldPosition(mouseWorldPos);

        if (targetCell != null && targetCell.hasPlant && targetCell.currentPlant == null)
        {
            // Plant was eaten by zombie or destroyed otherwise
            targetCell.hasPlant = false;
        }

        // Hiệu ứng Ghost Plant
        if (ghostRenderer != null)
        {
            if (MenuBar.Instance != null && MenuBar.Instance.currentSelectedPlant != null && targetCell != null && !targetCell.hasPlant && !isDiggingMode)
            {
                ghostRenderer.enabled = true;
                ghostRenderer.transform.position = targetCell.worldPosition;
                SpriteRenderer prefabRenderer = MenuBar.Instance.currentSelectedPlant.plantPrefab.GetComponent<SpriteRenderer>();
                if (prefabRenderer != null) ghostRenderer.sprite = prefabRenderer.sprite;
            }
            else
            {
                ghostRenderer.enabled = false;
            }
        }

        if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (targetCell != null)
            {
                if (isDiggingMode)
                {
                    if (targetCell.hasPlant && targetCell.currentPlant != null)
                    {
                        Destroy(targetCell.currentPlant);
                        targetCell.hasPlant = false;
                        targetCell.currentPlant = null;
                        isDiggingMode = false;
                        Debug.Log("Đã đào cây bằng xẻng!");
                    }
                }
                else if (MenuBar.Instance != null && MenuBar.Instance.currentSelectedPlant != null && !targetCell.hasPlant)
                {
                    PlantSeed(targetCell, MenuBar.Instance.currentSelectedPlant);
                    MenuBar.Instance.ConfirmPlanting();
                }
            }
        }
        
        // Hủy chọn cây hoặc xẻng khi bấm chuột phải
        if (UnityEngine.InputSystem.Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (isDiggingMode)
            {
                isDiggingMode = false;
            }
            else if (MenuBar.Instance != null && MenuBar.Instance.currentSelectedPlant != null)
            {
                MenuBar.Instance.CancelSelection();
            }
        }
    }

    public bool PlantSeed(GridCell cell, PlantData plantData)
    {
        if (cell != null)
        {
            if (cell.hasPlant && cell.currentPlant == null)
            {
                cell.hasPlant = false;
            }

            if (!cell.hasPlant)
            {
                GameObject plant = PlantFactory.CreatePlant(plantData, cell.worldPosition);
                if (plant != null)
                {
                    cell.hasPlant = true;
                    cell.currentPlant = plant;
                    return true;
                }
            }
        }
        return false;
    }
}
