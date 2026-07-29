using UnityEngine;

[CreateAssetMenu(fileName = "NewZombieData", menuName = "PvZ/Zombie Data")]
public class ZombieData : ScriptableObject
{
    public string zombieName;
    public int maxHealth = 100;
    public int damage = 10;
    public float moveSpeed = 1f;
    public float attackSpeed = 1f; // How often damage is dealt in seconds
}
