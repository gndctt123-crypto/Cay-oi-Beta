using UnityEngine;

public class MapGrid : MonoBehaviour
{
    public int rows = 5;
    public int cols = 9;
    public float cellSizeX = 1.2f;
    public float cellSizeY = 1.5f;

    // Array keeping track of whether a cell is occupied by a plant
    private bool[,] gridOccupied;

    void Start()
    {
        gridOccupied = new bool[cols, rows];
    }

    public bool TryPlant(GameObject plantPrefab, Vector2 worldPosition)
    {
        // Convert world position to grid coordinates (relative to this Grid's origin)
        Vector3 localPos = transform.InverseTransformPoint(worldPosition);
        int gridX = Mathf.FloorToInt(localPos.x / cellSizeX);
        int gridY = Mathf.FloorToInt(localPos.y / cellSizeY);

        if (IsValidCell(gridX, gridY) && !gridOccupied[gridX, gridY])
        {
            gridOccupied[gridX, gridY] = true;
            
            // Calculate center of the cell for instantiation
            float cellCenterX = (gridX * cellSizeX) + (cellSizeX / 2f);
            float cellCenterY = (gridY * cellSizeY) + (cellSizeY / 2f);
            Vector3 spawnPos = transform.TransformPoint(new Vector3(cellCenterX, cellCenterY, 0));

            // Instantiate plant
            Instantiate(plantPrefab, spawnPos, Quaternion.identity);
            return true; 
        }

        return false; // Cell occupied or out of bounds
    }

    private bool IsValidCell(int x, int y)
    {
        return x >= 0 && x < cols && y >= 0 && y < rows;
    }
}
