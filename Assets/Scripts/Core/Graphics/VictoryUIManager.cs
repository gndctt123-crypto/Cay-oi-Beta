using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryUIManager : MonoBehaviour
{
    public static VictoryUIManager Instance;
    
    public GameObject victoryPanel;
    public Button nextLevelButton;
    public Button exitButton;

    private void Awake()
    {
        Instance = this;
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(NextLevel);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f; // Dừng game
        }
    }

    private void NextLevel()
    {
        Time.timeScale = 1f;
        // Hiện tại game chỉ có 1 màn chơi nên load lại chính màn này
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
