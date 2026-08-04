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
    public float spawnX = 7.5f; // Ranh giới đường phố bên phải (Dịch sang trái một chút)

    private int currentWaveIndex = 0;
    private int zombiesSpawnedInWave = 0;
    private int activeZombies = 0;
    
    public delegate void LevelProgressAction(float progress);
    public event LevelProgressAction OnProgressChanged;

    private LevelProgressBar levelProgressBar;

    void Start()
    {
        // Khởi tạo 5 hàng ngang nếu chưa gán
        if (rowPositions == null || rowPositions.Length != GameConstants.GridRows)
        {
            rowPositions = new float[GameConstants.GridRows];
            MapGrid grid = FindAnyObjectByType<MapGrid>();
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

            waves = new Wave[] {
                // Đợt 1: 10 con (7 Thường, 3 Nón) - Tăng dần: 1 -> 2 -> 3 -> 4 con
                new Wave { normalZombieCount = 7, coneheadZombieCount = 3, bucketheadZombieCount = 0, spawnRate = 24f, isHugeWave = false },
                // Đợt 2: 15 con (10 Thường, 4 Nón, 1 Xô) - Tăng dần: 1 -> 2 -> 3 -> 4 -> 5 con
                new Wave { normalZombieCount = 10, coneheadZombieCount = 4, bucketheadZombieCount = 1, spawnRate = 28f, isHugeWave = true },
                // Đợt 3: 21 con (12 Thường, 6 Nón, 3 Xô) - Tăng dần: 1 -> 2 -> 3 -> 4 -> 5 -> 6 con
                new Wave { normalZombieCount = 12, coneheadZombieCount = 6, bucketheadZombieCount = 3, spawnRate = 28f, isHugeWave = true }
            };
        
        levelProgressBar = FindAnyObjectByType<LevelProgressBar>();
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
        yield return new WaitForSeconds(20f); // Thời gian chuẩn bị ban đầu (Tăng lên 20s để người chơi kịp trồng cây)
        
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

            zombiesSpawnedInWave = 0;

            // Tạo danh sách zombie cho wave này
            List<GameObject> zombiesToSpawn = new List<GameObject>();
            for(int i = 0; i < currentWave.normalZombieCount; i++) 
                if (normalZombiePrefab != null) zombiesToSpawn.Add(normalZombiePrefab);
            for(int i = 0; i < currentWave.coneheadZombieCount; i++) 
                if (coneheadZombiePrefab != null) zombiesToSpawn.Add(coneheadZombiePrefab);
            for(int i = 0; i < currentWave.bucketheadZombieCount; i++) 
                if (bucketheadZombiePrefab != null) zombiesToSpawn.Add(bucketheadZombiePrefab);
            
            // Xóa phần xáo trộn ngẫu nhiên để giữ nguyên thứ tự chất lượng tăng dần (Thường -> Nón -> Xô)

            // Bắt đầu spawn theo nhóm tăng dần (1 con -> 2 con -> 3 con...)
            int groupSize = 1;
            int currentIndex = 0;
            
            while (currentIndex < zombiesToSpawn.Count)
            {
                // Spawn 1 đợt nhỏ gồm 'groupSize' zombie
                for (int i = 0; i < groupSize; i++)
                {
                    if (currentIndex < zombiesToSpawn.Count)
                    {
                        SpawnZombie(zombiesToSpawn[currentIndex]);
                        zombiesSpawnedInWave++;
                        currentIndex++;
                        
                        // Đợi một chút (2s) giữa các con trong cùng 1 nhóm để chúng không bị đè lên nhau
                        yield return new WaitForSeconds(2f);
                    }
                }
                
                groupSize++; // Lần sau sẽ xuất hiện nhiều zombie hơn
                // Đợi thời gian dài giữa các nhóm (thời gian của đợt)
                yield return new WaitForSeconds(currentWave.spawnRate);
            }

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
