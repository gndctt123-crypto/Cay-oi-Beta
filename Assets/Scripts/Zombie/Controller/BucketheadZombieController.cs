using UnityEngine;

public class BucketheadZombieController : BaseZombieController
{
    [Header("Bucket Settings")]
    public int bucketHealth = 200;
    private bool hasBucket = true;

    [Header("Normal Zombie Frames (After losing bucket)")]
    public Sprite[] normalWalkFrames;
    public Sprite[] normalAttackFrames;

    public override void Initialize(ZombieData data)
    {
        base.Initialize(data);
        // Đặt máu của xô bằng đúng máu gốc của 1 con Zombie
        bucketHealth = currentHealth;
    }

    public override void TakeDamage(int amount)
    {
        if (hasBucket)
        {
            bucketHealth -= amount;
            
            // Kích hoạt hiệu ứng nhấp nháy đỏ khi bị bắn trúng
            if (zombieAnimator != null)
            {
                zombieAnimator.TriggerHitEffect();
            }

            Debug.Log($"[Kiểm định] Buckethead bị trúng đạn! Xô còn: {bucketHealth} HP");

            if (bucketHealth <= 0)
            {
                hasBucket = false;
                Debug.Log($"[Kiểm định] Buckethead đã rớt xô!");

                // Đổi animation về zombie thường
                if (zombieAnimator != null)
                {
                    zombieAnimator.walkFrames = normalWalkFrames;
                    zombieAnimator.attackFrames = normalAttackFrames;
                    
                    // Reset lại state để load frame mới ngay lập tức mà không bị kẹt
                    ZombieState tempState = currentState;
                    zombieAnimator.SetState(ZombieState.Freeze); // Đổi tạm để trigger reset
                    zombieAnimator.SetState(tempState);
                }

                // Sát thương dư thừa sẽ trừ vào máu thực của Zombie
                int overflowDamage = Mathf.Abs(bucketHealth);
                if (overflowDamage > 0)
                {
                    base.TakeDamage(overflowDamage);
                }
            }
        }
        else
        {
            // Nếu đã rớt xô, nhận sát thương như zombie thường
            base.TakeDamage(amount);
        }
    }
}
