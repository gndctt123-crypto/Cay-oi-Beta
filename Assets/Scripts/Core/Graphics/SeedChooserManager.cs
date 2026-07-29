using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SeedChooserManager : MonoBehaviour
{
    public static SeedChooserManager Instance;

    public GameObject chooserPanel;
    public Transform gridLayout; // Nơi chứa các card cây (Grid)
    public Button letsRockButton;
    public GameObject cardButtonPrefab; // Prefab cho button thẻ cây
    public MenuBar menuBar;

    public List<PlantData> allPlants = new List<PlantData>();
    private Dictionary<PlantData, Button> plantButtons = new Dictionary<PlantData, Button>();
    private List<PlantData> selectedPlants = new List<PlantData>();

    private void Awake()
    {
        Instance = this;
        // Dừng game khi đang chọn bài
        Time.timeScale = 0f;
    }

    private void Start()
    {
        InitializeChooser();
    }

    private void InitializeChooser()
    {
        // Xoá list cũ của menuBar
        menuBar.availablePlants.Clear();
        menuBar.UpdateUI();

        // Xóa các card nháp (dummy) tạo trong Editor
        for (int i = gridLayout.childCount - 1; i >= 0; i--)
        {
            Destroy(gridLayout.GetChild(i).gameObject);
        }

        // Tạo button cho mỗi cây trong grid
        foreach (PlantData plant in allPlants)
        {
            GameObject btnObj = new GameObject("ChooserCard_" + plant.plantName);
            btnObj.transform.SetParent(gridLayout, false);
            
            Image img = btnObj.AddComponent<Image>();
            img.sprite = plant.cardIcon;

            Button btn = btnObj.AddComponent<Button>();
            PlantData currentPlant = plant; // Local copy for closure
            btn.onClick.AddListener(() => OnPlantSelected(currentPlant));
            
            plantButtons[plant] = btn;
        }

        UpdateLetsRockButton();
    }

    private void OnPlantSelected(PlantData plant)
    {
        if (selectedPlants.Contains(plant))
        {
            OnPlantDeselected(plant);
            return;
        }

        if (selectedPlants.Count < 8)
        {
            selectedPlants.Add(plant);
            menuBar.availablePlants.Add(plant);
            menuBar.UpdateUI();

            // Làm mờ card ở Panel nhưng vẫn cho phép bấm để huỷ
            Image img = plantButtons[plant].GetComponent<Image>();
            img.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Xám đi

            UpdateLetsRockButton();
        }
    }

    public void OnPlantDeselected(PlantData plant)
    {
        if (selectedPlants.Contains(plant))
        {
            selectedPlants.Remove(plant);
            menuBar.availablePlants.Remove(plant);
            menuBar.UpdateUI();

            // Sáng lại card ở Panel
            Image img = plantButtons[plant].GetComponent<Image>();
            img.color = Color.white;

            UpdateLetsRockButton();
        }
    }

    private void UpdateLetsRockButton()
    {
        letsRockButton.interactable = (selectedPlants.Count == 8);
    }

    public void StartGame()
    {
        chooserPanel.SetActive(false);
        if (menuBar != null)
        {
            menuBar.HideDummySlots();
        }
        
        GameAnalyticsManager.TrackLevelStarted(CloudSaveManager.CurrentData.currentLevel);
        
        Time.timeScale = 1f;
    }
}
