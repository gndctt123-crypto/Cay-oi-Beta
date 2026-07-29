using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundBar;
    public Image fillBar;
    public RectTransform zombieHead;
    
    [Header("Progress Settings")]
    public int totalZombies = 10;
    private int currentZombies = 0;
    public float maxBarWidth = 540f; // Độ dài tối đa của thanh
    public float padding = 10f; // Viền bù thêm cho background
    
    private void Start()
    {
        if (fillBar != null)
        {
            fillBar.type = Image.Type.Tiled;
        }
        
        UpdateProgressUI();
    }
    
    public void OnZombieSpawned()
    {
        if (currentZombies < totalZombies)
        {
            currentZombies++;
            UpdateProgressUI();
        }
    }
    
    public void SetProgress(int current, int total)
    {
        currentZombies = current;
        totalZombies = total;
        UpdateProgressUI();
    }

    private void UpdateProgressUI()
    {
        if (totalZombies <= 0) return;
        
        float totalWidth = maxBarWidth;
        float currentWidth = ((float)currentZombies / totalZombies) * maxBarWidth;
        
        if (backgroundBar != null)
        {
            backgroundBar.rectTransform.sizeDelta = new Vector2(totalWidth, backgroundBar.rectTransform.sizeDelta.y);
        }
        
        if (fillBar != null)
        {
            RectTransform fillRt = fillBar.GetComponent<RectTransform>();
            fillRt.sizeDelta = new Vector2(currentWidth, fillRt.sizeDelta.y);
        }
        
        if (zombieHead != null)
        {
            zombieHead.anchoredPosition = new Vector2(-currentWidth, zombieHead.anchoredPosition.y);
        }
    }
}
