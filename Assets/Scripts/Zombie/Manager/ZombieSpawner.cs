using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int normalZombieCount;
        public int coneheadZombieCount;
        public int bucketheadZombieCount;
        public float spawnRate;
        public bool isHugeWave;
    }

    public Wave[] waves;
    public GameObject normalZombiePrefab;
    public GameObject coneheadZombiePrefab;
    public GameObject bucketheadZombiePrefab;
    
    // Tọa độ Y tương ứng với 5 hàng ngang
    public float[] rowPositions;
    public float spawnX = 10f; // Ranh giới đường phố bên phải

    private int currentWaveIndex = 0;
    private int zombiesSpawnedInWave = 0;
    private int activeZombies = 0;
    private bool isSpawning = false;
    
    public delegate void LevelProgressAction(float progress);
    public event LevelProgressAction OnProgressChanged;

    private LevelProgressBar levelProgressBar;

    void Start()
    {
        // Khởi tạo 5 hàng ngang nếu chưa gán
        if (rowPositions == null || rowPositions.Length != GameConstants.GridRows)
        {
            rowPositions = new float[GameConstants.GridRows];
            MapGrid grid = FindObjectOfType<MapGrid>();
            if (grid != null)
            {
                for (int i = 0; i < GameConstants.GridRows; i++)
                {
                    rowPositions[i] = grid.startPosition.y - i * grid.cellHeight;
                }
            }
        }
        
        if (normalZombiePrefab == null)
        {
#if UNITY_EDITOR
            normalZombiePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Zombie/Prefabs/NormalZombie.prefab");
#endif
        }

        if (coneheadZombiePrefab == null)
        {
#if UNITY_EDITOR
            coneheadZombiePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Zombie/Prefabs/ConeheadZombie.prefab");
#endif
        }

        if (bucketheadZombiePrefab == null)
        {
#if UNITY_EDITOR
            bucketheadZombiePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Zombie/Prefabs/BucketheadZombie.prefab");
#endif
        }

        if (waves == null || waves.Length == 0)
        {
            waves = new Wave[] {
                new Wave { normalZombieCount = 3, coneheadZombieCount = 1, bucketheadZombieCount = 0, spawnRate = 8f, isHugeWave = false },
                new Wave { normalZombieCount = 5, coneheadZombieCount = 3, bucketheadZombieCount = 1, spawnRate = 4f, isHugeWave = true }
            };
        }
        
        levelProgressBar = FindObjectOfType<LevelProgressBar>();
        if (levelProgressBar != null)
        {
            int totalZ = 0;
            foreach (var wave in waves)
            {
                totalZ += wave.normalZombieCount + wave.coneheadZombieCount + wave.bucketheadZombieCount;
            }
            levelProgressBar.SetProgress(0, totalZ);
        }
        
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(5f); // Thời gian chuẩn bị ban đầu
        
        while (currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];
            
            if (currentWave.isHugeWave)
            {
                // Cảnh báo wave lớn
                Debug.Log("A huge wave of zombies is approaching!");
                // TODO: Hiển thị UI text
                yield return new WaitForSeconds(3f);
            }

            isSpawning = true;
            zombiesSpawnedInWave = 0;

            // Tạo danh sách zombie cho wave này
            List<GameObject> zombiesToSpawn = new List<GameObject>();
            for(int i = 0; i < currentWave.normalZombieCount; i++) 
                if (normalZombiePrefab != null) zombiesToSpawn.Add(normalZombiePrefab);
            for(int i = 0; i < currentWave.coneheadZombieCount; i++) 
                if (coneheadZombiePrefab != null) zombiesToSpawn.Add(coneheadZombiePrefab);
            for(int i = 0; i < currentWave.bucketheadZombieCount; i++) 
                if (bucketheadZombiePrefab != null) zombiesToSpawn.Add(bucketheadZombiePrefab);
            
            // Xáo trộn ngẫu nhiên thứ tự xuất hiện
            for (int i = 0; i < zombiesToSpawn.Count; i++)
            {
                GameObject temp = zombiesToSpawn[i];
                int randomIndex = Random.Range(i, zombiesToSpawn.Count);
                zombiesToSpawn[i] = zombiesToSpawn[randomIndex];
                zombiesToSpawn[randomIndex] = temp;
            }

            // Bắt đầu spawn
            foreach(GameObject prefab in zombiesToSpawn)
            {
                SpawnZombie(prefab);
                zombiesSpawnedInWave++;
                yield return new WaitForSeconds(currentWave.spawnRate);
            }

            isSpawning = false;

            // Chờ cho đến khi tất cả zombie trong wave bị tiêu diệt
            while (activeZombies > 0)
            {
                yield return new WaitForSeconds(1f);
            }

            currentWaveIndex++;
            float progress = (float)currentWaveIndex / waves.Length;
            OnProgressChanged?.Invoke(progress);
        }

        Debug.Log("Chiến thắng!");
        // Cập nhật dữ liệu
        CloudSaveManager.CurrentData.currentLevel++;
        // Lưu dữ liệu lên Cloud
        _ = CloudSaveManager.SaveGameData(CloudSaveManager.CurrentData.currentLevel, CloudSaveManager.CurrentData.totalCoins, CloudSaveManager.CurrentData.totalZombiesKilled);
        // Nộp điểm lên Bảng xếp hạng
        _ = LeaderboardManager.AddScoreToLeaderboard(CloudSaveManager.CurrentData.totalZombiesKilled);
        
        GameAnalyticsManager.TrackLevelCompleted(CloudSaveManager.CurrentData.currentLevel, CloudSaveManager.CurrentData.totalZombiesKilled);

        // Mở UI GameVictory
        if (VictoryUIManager.Instance != null)
        {
            VictoryUIManager.Instance.ShowVictory();
        }
    }

    private void SpawnZombie(GameObject prefab)
    {
        if (prefab != null)
        {
            int randomRow = Random.Range(0, GameConstants.GridRows);
            Vector2 spawnPos = new Vector2(spawnX, rowPositions[randomRow]);
            
            GameObject zombie = Instantiate(prefab, spawnPos, Quaternion.identity);
            BaseZombieController controller = zombie.GetComponent<BaseZombieController>();
            
            // Khởi tạo ZombieData
            string dataName = "NormalZombieData";
            if (prefab.name.Contains("Conehead")) dataName = "ConeheadZombieData";
            else if (prefab.name.Contains("Buckethead")) dataName = "BucketheadZombieData";
            
            ZombieData data = Resources.Load<ZombieData>($"Data/{dataName}");
            if (data == null)
            {
#if UNITY_EDITOR
                data = UnityEditor.AssetDatabase.LoadAssetAtPath<ZombieData>($"Assets/Scripts/Zombie/Data/{dataName}.asset");
#endif
            }
            if (data != null && controller != null)
            {
                controller.Initialize(data);
            }
            
            // Xử lý sự kiện zombie chết
            controller.OnZombieDied += HandleZombieDied;
            
            activeZombies++;
            
            if (levelProgressBar != null)
            {
                levelProgressBar.OnZombieSpawned();
            }
        }
    }

    private void HandleZombieDied()
    {
        activeZombies--;
        CloudSaveManager.CurrentData.totalZombiesKilled++;
    }
}
