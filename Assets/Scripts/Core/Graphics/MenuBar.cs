using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuBar : MonoBehaviour
{
    public static MenuBar Instance { get; private set; }

    [Header("UI Settings")]
    public Text sunTotalText;
    public Transform cardContainer; // Đối tượng cha chứa danh sách các thẻ bài
    public GameObject cardPrefab;

    [Header("Available Plants")]
    public List<PlantData> availablePlants;
    private List<Card> activeCards = new List<Card>();

    [Header("Planting State")]
    public PlantData currentSelectedPlant; // Cây đang được giữ trên tay
    public Image plantGhostImage; // Hình mờ của cây dính theo chuột

    private void Awake()
    {
        Instance = this;
        
        // Đảm bảo MenuBar luôn nổi lên trên SeedChooserCanvas để nhận được click chuột
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 20; // Cao hơn SeedChooserCanvas (10)
        }
        
        UnityEngine.UI.GraphicRaycaster raycaster = GetComponent<UnityEngine.UI.GraphicRaycaster>();
        if (raycaster == null)
        {
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        if (sunTotalText != null)
        {
            sunTotalText.transform.SetParent(this.transform, true);
        }
    }

    private void Start()
    {
        UpdateUI();

        // Lắng nghe sự kiện đổi tiền từ PlayerManager để cập nhật UI Text
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnSunChanged.AddListener(UpdateSunText);
            UpdateSunText(PlayerManager.Instance.currentSun);
        }

        if (plantGhostImage != null)
        {
            plantGhostImage.gameObject.SetActive(false);
        }
    }

    private void UpdateSunText(int currentSun)
    {
        if (sunTotalText != null)
        {
            sunTotalText.text = currentSun.ToString();
        }
    }

    public void UpdateUI()
    {
        Transform container = cardContainer != null ? cardContainer : transform;
        
        // Dọn dẹp các thẻ bài cũ và ẩn toàn bộ DummySlot
        foreach (Transform dummySlot in container)
        {
            Image dummyImg = dummySlot.GetComponent<Image>();
            if (dummyImg != null) 
            {
                dummyImg.enabled = false;
                dummyImg.raycastTarget = false;
            }

            foreach (Transform card in dummySlot)
            {
                Destroy(card.gameObject);
            }
        }
        activeCards.Clear();

        // Tạo lại card mới và đặt vào trong các DummySlot_Menu
        for (int i = 0; i < availablePlants.Count; i++)
        {
            if (i < container.childCount)
            {
                Transform dummySlot = container.GetChild(i);


                PlantData data = availablePlants[i];
                
                GameObject newCardObj = Instantiate(cardPrefab, dummySlot);
                // Reset vị trí để nằm ngay chính giữa Dummy Slot
                newCardObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                
                Card cardScript = newCardObj.GetComponent<Card>();
                if (cardScript != null)
                {
                    cardScript.SetupCard(data);
                    cardScript.OnCardClicked += HandleCardClicked;
                    activeCards.Add(cardScript);
                }
            }
        }
    }

    private void HandleCardClicked(PlantData data)
    {
        // Nếu đang ở màn hình chọn bài
        if (SeedChooserManager.Instance != null && SeedChooserManager.Instance.chooserPanel.activeSelf)
        {
            SeedChooserManager.Instance.OnPlantDeselected(data);
            return;
        }
        // Khi người chơi ấn vào một thẻ bài có đủ tiền
        currentSelectedPlant = data;
        
        // Bật hình ảnh bóng mờ (Ghost) đi theo con trỏ chuột
        if (plantGhostImage != null)
        {
            plantGhostImage.sprite = data.cardIcon; // Tạm dùng icon, nếu có sprite gốc thì dùng
            plantGhostImage.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        // Nếu đang giữ một cây trên tay, cho cái bóng (Ghost) chạy theo vị trí chuột
        if (currentSelectedPlant != null && plantGhostImage != null && plantGhostImage.gameObject.activeSelf)
        {
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                plantGhostImage.transform.position = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

                // Nếu người dùng bấm chuột trái hoặc phải để hủy / trồng
                if (UnityEngine.InputSystem.Mouse.current.rightButton.wasPressedThisFrame) // Bấm chuột phải để huỷ chọn
                {
                    CancelSelection();
                }
            }
            // Việc bấm chuột trái để trồng thực tế sẽ được xử lý ở MapGrid (Bắn tia raycast) 
            // MapGrid sẽ gọi hàm ConfirmPlanting bên dưới
        }
    }

    public void ConfirmPlanting()
    {
        // Trừ tiền
        PlayerManager.Instance.SpendSun(currentSelectedPlant.sunCost);
        
        GameAnalyticsManager.TrackPlantPlaced(currentSelectedPlant.plantName);

        // Tìm thẻ bài tương ứng và kích hoạt thời gian hồi chiêu
        foreach (Card card in activeCards)
        {
            if (card.plantData == currentSelectedPlant)
            {
                card.StartCooldown();
                break;
            }
        }

        CancelSelection();
    }

    public void CancelSelection()
    {
        currentSelectedPlant = null;
        if (plantGhostImage != null)
        {
            plantGhostImage.gameObject.SetActive(false);
        }
    }

    public void HideDummySlots()
    {
        Transform container = cardContainer != null ? cardContainer : transform;
        foreach (Transform dummySlot in container)
        {
            Image img = dummySlot.GetComponent<Image>();
            if (img != null)
            {
                img.enabled = false;
            }
        }
    }
}
