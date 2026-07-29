using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

public static class CloudSaveManager
{
    // Cấu trúc dữ liệu JSON để lưu trữ tiến độ
    [Serializable]
    public class PlayerData
    {
        public int currentLevel;
        public int totalCoins;
        public int totalZombiesKilled;
    }

    public static PlayerData CurrentData { get; private set; } = new PlayerData();

    // Gọi hàm này để lưu dữ liệu lên Cloud
    public static async Task SaveGameData(int level, int coins, int zombiesKilled)
    {
        if (!UGSManager.IsSignedIn)
        {
            Debug.LogWarning("[CloudSave] Cần đăng nhập để lưu game.");
            return;
        }

        try
        {
            CurrentData.currentLevel = level;
            CurrentData.totalCoins = coins;
            CurrentData.totalZombiesKilled = zombiesKilled;

            var data = new Dictionary<string, object>{
                { "PlayerData", CurrentData }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log("[CloudSave] Lưu game thành công lên mây!");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CloudSave] Lỗi khi lưu: {e.Message}");
        }
    }

    // Gọi hàm này để tải dữ liệu từ Cloud về máy
    public static async Task LoadGameData()
    {
        if (!UGSManager.IsSignedIn)
        {
            Debug.LogWarning("[CloudSave] Cần đăng nhập để tải game.");
            return;
        }

        try
        {
            var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "PlayerData" });

            if (savedData.TryGetValue("PlayerData", out var item))
            {
                CurrentData = item.Value.GetAs<PlayerData>();
                Debug.Log($"[CloudSave] Tải thành công! Level: {CurrentData.currentLevel}, Coins: {CurrentData.totalCoins}, Kills: {CurrentData.totalZombiesKilled}");
            }
            else
            {
                Debug.Log("[CloudSave] Người chơi mới, chưa có dữ liệu nào trên mây.");
                CurrentData = new PlayerData(); // Khởi tạo dữ liệu gốc
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[CloudSave] Lỗi khi tải: {e.Message}");
        }
    }
}
