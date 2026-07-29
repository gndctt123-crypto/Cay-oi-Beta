using UnityEngine;

public class ShooterController : BasePlantController
{
    [Header("Shooter Settings")]
    public GameObject projectilePrefab; // Prefab viên đạn (hạt đậu)
    public Transform shootPoint; // Vị trí nòng súng để nhả đạn

    private float attackTimer;

    public override void Initialize(PlantData data)
    {
        base.Initialize(data);
        attackTimer = 0f;
    }

    protected override void Update()
    {
        base.Update();
        
        // Trạng thái Attack hoặc tự động bắn khi có zombie trên làn
        // Tạm thời giả định lúc nào cũng bắn nếu đã hồi đủ thời gian
        attackTimer += Time.deltaTime;
        
        if (attackTimer >= plantData.attackInterval)
        {
            // Chỗ này sau sẽ tối ưu: Chỉ bắn khi phát hiện có Zombie trước mặt bằng Raycast
            if (CheckForZombiesInLane())
            {
                Shoot();
                attackTimer = 0f;
            }
        }
    }

    protected virtual void Shoot()
    {
        Debug.Log($"[Kiểm định] Cây {gameObject.name} phát hiện Zombie và nhả đạn!");
        if (projectilePrefab != null)
        {
            // Kích hoạt animation bắn 
            plantAnimator?.SetState(PlantState.Attack);
            
            // Lấy vị trí nòng súng hoặc dùng tâm của cây nếu chưa gắn nòng
            Vector3 spawnPos = shootPoint != null ? shootPoint.position : (transform.position + new Vector3(0.5f, 0.2f, 0));
            
            // Tạo viên đạn
            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            
            // Truyền sát thương của cây vào đạn
            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null)
            {
                projScript.damage = plantData.damage;
            }
        }
    }

    protected virtual bool CheckForZombiesInLane()
    {
        // Bắn 1 tia Raycast về phía bên phải để quét Zombie
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right, 15f);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.GetComponent<BaseZombieController>() != null)
            {
                return true; // Thấy zombie là bắn ngay
            }
        }
        return false;
    }
}
