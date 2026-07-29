using UnityEngine;
using UnityEngine.UI;

public class SunCounterUI : MonoBehaviour
{
    private Text sunText;

    private void Awake()
    {
        sunText = GetComponent<Text>();
        
        // Ensure it renders above the background
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 20;
        }
    }

    private void Start()
    {
        if (PlayerManager.Instance != null && sunText != null)
        {
            PlayerManager.Instance.OnSunChanged.AddListener(UpdateSunText);
            UpdateSunText(PlayerManager.Instance.currentSun);
        }
    }

    private void UpdateSunText(int amount)
    {
        if (sunText != null)
        {
            sunText.text = amount.ToString();
        }
    }
    
    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnSunChanged.RemoveListener(UpdateSunText);
        }
    }
}
