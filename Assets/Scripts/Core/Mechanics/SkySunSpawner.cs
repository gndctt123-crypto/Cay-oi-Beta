using UnityEngine;
using System.Collections;

public class SkySunSpawner : MonoBehaviour
{
    public GameObject sunPrefab;
    public float spawnInterval = 10f;
    
    private float timer = 0f;
    private MapGrid grid;

    void Start()
    {
        grid = FindObjectOfType<MapGrid>();
        if (sunPrefab == null)
        {
#if UNITY_EDITOR
            sunPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Core/Prefabs/Sun.prefab");
#endif
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnSun();
            timer = 0f;
        }
    }

    void SpawnSun()
    {
        if (sunPrefab != null && grid != null)
        {
            float targetX = Random.Range(grid.startPosition.x, grid.startPosition.x + grid.cellWidth * GameConstants.GridColumns);
            float targetY = Random.Range(grid.startPosition.y - grid.cellHeight * GameConstants.GridRows, grid.startPosition.y);
            
            Vector2 startPos = new Vector2(targetX, 6f);
            Vector2 dropPos = new Vector2(targetX, targetY);
            
            GameObject sun = Instantiate(sunPrefab, startPos, Quaternion.identity);
            
            SunPickup pickup = sun.GetComponent<SunPickup>();
            if (pickup != null)
            {
                pickup.sunValue = 50;
                pickup.SetupDrop(startPos, dropPos);
            }
        }
    }
}
