using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantData", menuName = "PvZ/Plant Data")]
public class PlantData : ScriptableObject
{
    public string plantName;
    public int maxHealth = 100;
    public int sunCost = 50;
    public float cooldownTime = 5f;
    public GameObject plantPrefab;
}
