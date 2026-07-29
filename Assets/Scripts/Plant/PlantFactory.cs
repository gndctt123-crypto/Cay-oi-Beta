using UnityEngine;

public class PlantFactory : MonoBehaviour
{
    // Cấu trúc Factory Pattern để sản xuất Cây theo Data
    public static GameObject CreatePlant(PlantData data, Vector3 position)
    {
        if (data == null || data.plantPrefab == null)
        {
            Debug.LogError("Thiếu dữ liệu PlantData hoặc chưa gán Prefab!");
            return null;
        }

        // Tạo (Instantiate) cây mới dựa trên prefab trong PlantData
        GameObject newPlant = Instantiate(data.plantPrefab, position, Quaternion.identity);
        newPlant.name = data.plantName;

        // Gắn controller phù hợp và khởi tạo chỉ số
        BasePlantController controller = newPlant.GetComponent<BasePlantController>();
        if (controller != null)
        {
            controller.Initialize(data);
        }
        else
        {
            Debug.LogWarning($"Cây {data.plantName} thiếu BasePlantController!");
        }

        return newPlant;
    }
}
