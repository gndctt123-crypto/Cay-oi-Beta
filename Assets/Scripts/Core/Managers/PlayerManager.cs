using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Player Economy")]
    public int currentSun = 50; // Khởi đầu thường có 50 mặt trời

    // Sự kiện để UI tự động cập nhật chữ hiển thị số mặt trời mỗi khi tiền thay đổi
    public UnityEvent<int> OnSunChanged; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentSun = 50;
        OnSunChanged?.Invoke(currentSun);
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        currentSun = 50;
        OnSunChanged?.Invoke(currentSun);
    }

    public void AddSun(int amount)
    {
        currentSun += amount;
        OnSunChanged?.Invoke(currentSun);
    }

    public bool SpendSun(int amount)
    {
        if (currentSun >= amount)
        {
            currentSun -= amount;
            OnSunChanged?.Invoke(currentSun);
            return true;
        }
        return false;
    }
}
