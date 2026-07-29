using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantData", menuName = "Plants vs Zombies/Plant Data")]
public class PlantData : ScriptableObject
{
    [Header("Basic Info")]
    public string plantName;
    public int sunCost;
    public float cooldownTime; // Thời gian hồi thẻ bài

    [Header("Combat Stats")]
    public int health = 5;
    public int damage = 1;
    public float attackInterval = 1.0f; // Nhịp tấn công (giây)

    [Header("Visuals")]
    public Sprite cardIcon;
    public GameObject plantPrefab; // Prefab gốc (có chứa Animator + Controller)
}
