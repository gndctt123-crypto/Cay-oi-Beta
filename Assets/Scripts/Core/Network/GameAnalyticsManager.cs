using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public static class GameAnalyticsManager
{
    public static void Initialize()
    {
        if (UGSManager.IsInitialized)
        {
            try
            {
                // Yêu cầu sự cho phép thu thập dữ liệu (bắt buộc theo luật định của Unity)
                // Trong thực tế cần có nút UI cho người chơi đồng ý, ở đây tạm thời gọi luôn.
                AnalyticsService.Instance.StartDataCollection();
                Debug.Log("[Analytics] Bắt đầu thu thập dữ liệu.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Analytics] Lỗi khởi tạo: {e.Message}");
            }
        }
    }

    public static void TrackLevelStarted(int levelIndex)
    {
        if (!UGSManager.IsSignedIn) return;
        
        var ev = new CustomEvent("level_started")
        {
            { "level_index", levelIndex }
        };

        AnalyticsService.Instance.RecordEvent(ev);
        Debug.Log($"[Analytics] Đã gửi sự kiện: level_started - {levelIndex}");
    }

    public static void TrackLevelCompleted(int levelIndex, int zombiesKilled)
    {
        if (!UGSManager.IsSignedIn) return;

        var ev = new CustomEvent("level_completed")
        {
            { "level_index", levelIndex },
            { "zombies_killed", zombiesKilled }
        };

        AnalyticsService.Instance.RecordEvent(ev);
        Debug.Log($"[Analytics] Đã gửi sự kiện: level_completed - Level {levelIndex} (Kills: {zombiesKilled})");
    }

    public static void TrackLevelFailed(int levelIndex, int waveReached)
    {
        if (!UGSManager.IsSignedIn) return;

        var ev = new CustomEvent("level_failed")
        {
            { "level_index", levelIndex },
            { "wave_reached", waveReached }
        };

        AnalyticsService.Instance.RecordEvent(ev);
        Debug.Log($"[Analytics] Đã gửi sự kiện: level_failed - Level {levelIndex} (Wave: {waveReached})");
    }

    public static void TrackPlantPlaced(string plantName)
    {
        if (!UGSManager.IsSignedIn) return;

        var ev = new CustomEvent("plant_placed")
        {
            { "plant_name", plantName }
        };

        AnalyticsService.Instance.RecordEvent(ev);
        // Không in log để tránh spam Console
    }
}
