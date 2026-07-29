using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
    public static GameOverUIManager Instance;
    
    public GameObject gameOverPanel; // Panel chứa thông báo và các nút
    public Button retryButton;
    public Button exitButton;

    private void Awake()
    {
        Instance = this;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryGame);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            int currentWave = 0; // Tạm để 0 vì chưa thể lấy wave từ Spawner dễ dàng
            GameAnalyticsManager.TrackLevelFailed(CloudSaveManager.CurrentData.currentLevel, currentWave);
            
            Time.timeScale = 0f; // Dừng game
        }
    }

    private void RetryGame()
    {
        Time.timeScale = 1f; // Khôi phục thời gian
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ExitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
