using UnityEngine;
using System.Collections;

public class PotatoMineController : BasePlantController
{
    private bool isArmed = false;
    private bool hasExploded = false;
    
    public Sprite[] armedFrames;
    public Sprite[] potatoExplodeFrames;
    
    // In PVZ, Potato Mine takes exactly 15 seconds to arm
    public float armTime = 15f; 

    public override void Initialize(PlantData data)
    {
        base.Initialize(data);
        StartCoroutine(ArmingCoroutine());
    }

    private IEnumerator ArmingCoroutine()
    {
        // 1. Trạng thái Unarmed: Nằm dưới đất, lúc này BasePlantAnimator sẽ dùng idleFrames (PotatoMineInit_0)
        yield return new WaitForSeconds(armTime);
        
        // 2. Trạng thái Armed: Trồi lên
        isArmed = true;
        
        if (plantAnimator != null)
        {
            // Tráo đổi mảng idleFrames sang bộ ảnh lúc trồi lên (PotatoMine_0 -> PotatoMine_7)
            plantAnimator.idleFrames = armedFrames;
            
            // "Mẹo" để bắt BasePlantAnimator reset lại currentFrame về 0 và load ngay frame mới
            plantAnimator.SetState(PlantState.Attack); 
            plantAnimator.SetState(PlantState.Idle);
        }
    }

    protected override void Update()
    {
        base.Update();
        
        // Nếu đã chết, chưa arm, hoặc đã nổ rồi thì không quét sát thương
        if (currentHealth <= 0 || !isArmed || hasExploded) return;

        // Quét Zombie dẫm lên (Radius nhỏ, vì chỉ khi dẫm lên mới nổ)
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f);
        if (hit != null)
        {
            BaseZombieController zombie = hit.GetComponent<BaseZombieController>();
            // Kiểm tra zombie hợp lệ
            if (zombie != null)
            {
                Explode();
            }
        }
    }

    public override void TakeDamage(int amount)
    {
        if (hasExploded) return;
        
        if (isArmed)
        {
            // Đã ngoi lên mà bị zombie cắn -> Lập tức phát nổ
            Explode();
        }
        else
        {
            // Đang nằm dưới đất -> Bị cắn mất máu bình thường như hoa hướng dương
            base.TakeDamage(amount);
        }
    }

    private void Explode()
    {
        hasExploded = true;
        
        if (plantAnimator != null)
        {
            // Tráo bộ ảnh attack thành ảnh SPUDOW nổ
            plantAnimator.attackFrames = potatoExplodeFrames;
            plantAnimator.SetState(PlantState.Attack);
        }
        
        // Gây sát thương diện rộng cực lớn trong 1 ô (1800 damage)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.8f);
        foreach (var col in hits)
        {
            BaseZombieController z = col.GetComponent<BaseZombieController>();
            if (z != null)
            {
                z.TakeDamage(1800);
            }
        }
        
        // Đợi animation nổ (SPUDOW) chạy trong 0.5s rồi biến mất
        StartCoroutine(DieAfterExplosion());
    }

    private IEnumerator DieAfterExplosion()
    {
        // Khi đang nổ, nó trở nên bất tử để không bị zombie khác cắn chết mất animation
        currentHealth = 99999; 
        
        // Tắt va chạm để zombie khác đi qua không bị vướng
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(0.5f);
        
        // Chết thật
        currentHealth = 0; 
        Die();
    }
}
