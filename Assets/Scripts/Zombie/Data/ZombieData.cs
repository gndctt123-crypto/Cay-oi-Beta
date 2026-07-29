using UnityEngine;

[CreateAssetMenu(fileName = "NewZombieData", menuName = "Plants vs Zombies/Zombie Data")]
public class ZombieData : ScriptableObject
{
    [Header("Basic Info")]
    public string zombieName;

    [Header("Combat Stats")]
    public int health = 10;
    public int damage = 1; // Lượng máu cắn mỗi nhịp
    public float attackInterval = 1.0f; // Tốc độ cắn (giây/lần)
    
    [Header("Movement")]
    public float walkSpeed = 0.5f;

    [Header("Visuals")]
    public GameObject zombiePrefab;
}
