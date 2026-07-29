using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler
{
    public PlantData plantData;
    
    [Header("UI Elements")]
    public Image iconImage;
    public Image cooldownOverlay;
    public Text costText;

    private float currentCooldown = 0f;
    private bool isReady = true;

    // Delegate để báo cáo sự kiện bấm thẻ cho MenuBar hoặc PlayerManager
    public delegate void CardClickedAction(PlantData data);
    public event CardClickedAction OnCardClicked;

    public void SetupCard(PlantData data)
    {
        plantData = data;
        
        // Tự động lấy component Image gốc làm biểu tượng nếu chưa gán
        if (iconImage == null) iconImage = GetComponent<Image>();
        
        if (iconImage != null && plantData != null) 
        {
            iconImage.sprite = plantData.cardIcon;
        }
        
        if (costText != null) costText.text = plantData.sunCost.ToString();
        
        currentCooldown = 0f;
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        isReady = true;
    }

    private void Update()
    {
        bool hasEnoughSun = false;
        if (PlayerManager.Instance != null && plantData != null)
        {
            hasEnoughSun = PlayerManager.Instance.currentSun >= plantData.sunCost;
        }

        if (!isReady)
        {
            currentCooldown -= Time.deltaTime;
            
            // Cập nhật lớp mờ thời gian hồi
            if (cooldownOverlay != null && plantData != null && plantData.cooldownTime > 0)
            {
                cooldownOverlay.fillAmount = currentCooldown / plantData.cooldownTime;
            }

            if (iconImage != null)
            {
                iconImage.color = Color.gray; // Luôn tối khi đang hồi chiêu
            }

            if (currentCooldown <= 0f)
            {
                isReady = true;
                if (cooldownOverlay != null)
                {
                    cooldownOverlay.fillAmount = 0f;
                }
            }
        }
        else
        {
            if (iconImage != null)
            {
                iconImage.color = hasEnoughSun ? Color.white : Color.gray;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isReady || plantData == null) return;

        bool isChoosing = (SeedChooserManager.Instance != null && SeedChooserManager.Instance.chooserPanel.activeSelf);

        if (isChoosing || (PlayerManager.Instance != null && PlayerManager.Instance.currentSun >= plantData.sunCost))
        {
            // Phát sự kiện bấm thẻ để UIManager chuẩn bị trạng thái chờ nhấp chuột xuống bãi cỏ
            OnCardClicked?.Invoke(plantData);
        }
    }

    public void StartCooldown()
    {
        isReady = false;
        if (plantData != null)
        {
            currentCooldown = plantData.cooldownTime;
        }
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 1f;
        }
    }
}
