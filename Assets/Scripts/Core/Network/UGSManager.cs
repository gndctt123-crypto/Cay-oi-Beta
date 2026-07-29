using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public static class UGSManager
{
    public static bool IsInitialized { get; private set; }
    public static bool IsSignedIn { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static async void InitUGSAutomatically()
    {
        Debug.Log("[UGSManager] Bắt đầu khởi tạo Unity Gaming Services...");
        try
        {
            await UnityServices.InitializeAsync();
            IsInitialized = true;
            Debug.Log("[UGSManager] Khởi tạo UGS thành công!");

            await SignInAnonymously();
        }
        catch (Exception e)
        {
            Debug.LogError($"[UGSManager] Lỗi khởi tạo UGS: {e.Message}");
        }
    }

    private static async Task SignInAnonymously()
    {
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("[UGSManager] Đã đăng nhập trước đó.");
                IsSignedIn = true;
                return;
            }

            AuthenticationService.Instance.SignedIn += async () =>
            {
                IsSignedIn = true;
                Debug.Log($"[UGSManager] Đăng nhập ẩn danh thành công! Player ID: {AuthenticationService.Instance.PlayerId}");
                GameAnalyticsManager.Initialize();
                await CloudSaveManager.LoadGameData();
            };

            AuthenticationService.Instance.SignInFailed += (err) =>
            {
                Debug.LogError($"[UGSManager] Lỗi đăng nhập: {err.ErrorCode} - {err.Message}");
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"[UGSManager] Ngoại lệ xác thực: {ex}");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"[UGSManager] Lỗi kết nối mạng: {ex}");
        }
    }
}
