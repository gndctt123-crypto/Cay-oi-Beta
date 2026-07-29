using UnityEngine;

public class ZombieFactory : MonoBehaviour
{
    public static GameObject CreateZombie(ZombieData data, Vector3 position)
    {
        if (data == null || data.zombiePrefab == null)
        {
            Debug.LogError("Thiếu dữ liệu ZombieData hoặc chưa gán Prefab!");
            return null;
        }

        GameObject newZombie = Instantiate(data.zombiePrefab, position, Quaternion.identity);
        newZombie.name = data.zombieName;

        BaseZombieController controller = newZombie.GetComponent<BaseZombieController>();
        if (controller != null)
        {
            controller.Initialize(data);
        }
        else
        {
            Debug.LogWarning($"Zombie {data.zombieName} thiếu BaseZombieController!");
        }

        return newZombie;
    }
}
