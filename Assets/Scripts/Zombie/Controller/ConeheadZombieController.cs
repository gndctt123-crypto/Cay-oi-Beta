using UnityEngine;

public class ConeheadZombieController : BaseZombieController
{
    [Header("Cone Settings")]
    public int coneHealth = 370;
    private bool hasCone = true;

    [Header("Normal Zombie Frames (After losing cone)")]
    public Sprite[] normalWalkFrames;
    public Sprite[] normalAttackFrames;

    public override void Initialize(ZombieData data)
    {
        base.Initialize(data);
        // Đặt máu của mũ bằng 1/2 máu gốc của Zombie
        coneHealth = currentHealth / 2;
    }

    public override void TakeDamage(int amount)
    {
        if (hasCone)
        {
            coneHealth -= amount;
            
            // Kích hoạt hiệu ứng nhấp nháy đỏ khi bị bắn trúng
            if (zombieAnimator != null)
            {
                zombieAnimator.TriggerHitEffect();
            }

            Debug.Log($"[Kiểm định] Conehead bị trúng đạn! Mũ Cone còn: {coneHealth} HP");

            if (coneHealth <= 0)
            {
                hasCone = false;
                Debug.Log($"[Kiểm định] Conehead đã rớt mũ!");

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
                int overflowDamage = Mathf.Abs(coneHealth);
                if (overflowDamage > 0)
                {
                    base.TakeDamage(overflowDamage);
                }
            }
        }
        else
        {
            // Nếu đã rớt mũ, nhận sát thương như zombie thường
            base.TakeDamage(amount);
        }
    }
}
