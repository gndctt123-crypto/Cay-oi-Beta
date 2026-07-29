using System;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using UnityEngine;

public static class LeaderboardManager
{
    // Tên ID của Leaderboard trên Unity Dashboard (cần trùng khớp)
    private const string LeaderboardId = "top_zombie_killers";

    public static async Task AddScoreToLeaderboard(int score)
    {
        if (!UGSManager.IsSignedIn)
        {
            Debug.LogWarning("[Leaderboard] Cần đăng nhập để nộp điểm.");
            return;
        }

        try
        {
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
            Debug.Log($"[Leaderboard] Nộp điểm thành công! Điểm: {score}. Xếp hạng mới: {response.Rank}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Leaderboard] Lỗi khi nộp điểm: {e.Message}");
        }
    }

    public static async Task FetchTopScores()
    {
        if (!UGSManager.IsSignedIn)
        {
            Debug.LogWarning("[Leaderboard] Cần đăng nhập để xem điểm.");
            return;
        }

        try
        {
            // Tải 10 người đứng đầu
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Limit = 10 });
            
            Debug.Log($"[Leaderboard] BẢNG XẾP HẠNG TOP 10:");
            foreach (var entry in scoresResponse.Results)
            {
                Debug.Log($"Hạng #{entry.Rank + 1} | Player: {entry.PlayerId} | Score: {entry.Score}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Leaderboard] Lỗi khi tải bảng xếp hạng: {e.Message}");
        }
    }
}
