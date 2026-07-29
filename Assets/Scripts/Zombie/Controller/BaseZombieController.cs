using UnityEngine;

public enum ZombieState
{
    Walk,
    Attack,
    Die,
    Freeze
}

public class BaseZombieController : MonoBehaviour
{
    [SerializeField] protected ZombieData zombieData;

    protected int currentHealth;
    protected ZombieState currentState;
    protected BaseZombieAnimator zombieAnimator;
    protected GameObject targetPlant;

    public virtual void Initialize(ZombieData data)
    {
        zombieData = data;
        currentHealth = zombieData.health;
        currentState = ZombieState.Walk;
        zombieAnimator = GetComponent<BaseZombieAnimator>();
    }

    private float currentSlowTimer = 0f;
    private bool isSlowed = false;

    protected virtual void Update()
    {
        if (currentHealth <= 0) return;

        // Xử lý đếm lùi hiệu ứng làm chậm
        if (isSlowed)
        {
            currentSlowTimer -= Time.deltaTime;
            if (currentSlowTimer <= 0f)
            {
                isSlowed = false;
                speedMultiplier = 1f;
                if (zombieAnimator != null)
                {
                    zombieAnimator.SetBaseColor(Color.white);
                }
            }
        }

        switch (currentState)
        {
            case ZombieState.Walk:
                Move();
                break;
            case ZombieState.Attack:
                Attack();
                break;
            // Freeze or Die logic can be handled here
        }
    }

    // Move moved down

    protected float attackTimer = 0f;

    protected virtual void Attack()
    {
        // Nếu cây đã bị ăn hết hoặc biến mất
        if (targetPlant == null)
        {
            currentState = ZombieState.Walk;
            zombieAnimator?.SetState(ZombieState.Walk);
            return;
        }

        attackTimer += Time.deltaTime;
        
        // Nhịp cắn sẽ bị ảnh hưởng bởi hiệu ứng làm chậm
        float currentAttackInterval = isSlowed ? (1f / speedMultiplier) : 1f; 

        if (attackTimer >= currentAttackInterval) // Cắn chậm hơn nếu bị slow
        {
            BasePlantController plant = targetPlant.GetComponent<BasePlantController>();
            if (plant != null)
            {
                plant.TakeDamage(zombieData.damage);
            }
            attackTimer = 0f;
        }
    }

    public delegate void ZombieDiedAction();
    public event ZombieDiedAction OnZombieDied;

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"[Kiểm định] Zombie bị trúng đạn! Mất {amount} HP. Máu còn: {currentHealth}");
        
        // Luôn nháy đỏ, vì HitBlinkCoroutine bây giờ đã an toàn và sẽ tự trả về baseColor (xanh nhạt) sau khi nháy xong
        zombieAnimator?.TriggerHitEffect();

        if (currentHealth <= 0 && currentState != ZombieState.Die)
        {
            Debug.Log($"[Kiểm định] Zombie đã bị tiêu diệt hoàn toàn!");
            Die();
        }
    }

    protected virtual void Die()
    {
        currentState = ZombieState.Die;
        zombieAnimator?.SetState(ZombieState.Die);
        OnZombieDied?.Invoke(); // Báo cho ZombieSpawner biết
        Destroy(gameObject, 1.5f); // Chờ animation chết chạy xong rồi xoá
    }

    public virtual void Swallow()
    {
        if (currentState == ZombieState.Die) return;
        
        currentHealth = 0;
        currentState = ZombieState.Die;
        OnZombieDied?.Invoke(); // Báo cho ZombieSpawner biết
        Destroy(gameObject); // Xoá ngay lập tức không cần chờ animation
    }

    public virtual void StartBeingEaten()
    {
        currentState = ZombieState.Freeze; // Dừng mọi hoạt động
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; // Tắt va chạm
    }

    // Xử lý va chạm với cây (Dùng OnTrigger hoặc tuỳ theo setup vật lý 2D)
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        BasePlantController plant = collision.GetComponent<BasePlantController>();
        if (plant != null)
        {
            currentState = ZombieState.Attack;
            targetPlant = collision.gameObject;
            zombieAnimator?.SetState(ZombieState.Attack);
        }
        else if (collision.name == "House")
        {
            Debug.Log("The zombies ate your brains!");
            if (GameOverUIManager.Instance != null)
            {
                GameOverUIManager.Instance.ShowGameOver();
            }
        }
    }

    private float speedMultiplier = 1f;

    protected virtual void Move()
    {
        transform.Translate(Vector3.left * zombieData.walkSpeed * speedMultiplier * Time.deltaTime);
    }

    public void ApplySlow(float slowPercent, float duration)
    {
        if (!isSlowed)
        {
            isSlowed = true;
            speedMultiplier = 1f - slowPercent;
            
            if (zombieAnimator != null)
            {
                // Màu xanh nước biển nhạt
                zombieAnimator.SetBaseColor(new Color(0.5f, 0.7f, 1f, 1f)); 
            }
        }
        
        // Refresh duration
        currentSlowTimer = duration; 
    }
}
