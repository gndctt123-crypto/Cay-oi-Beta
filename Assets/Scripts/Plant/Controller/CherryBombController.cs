using UnityEngine;
using System.Collections;

public class CherryBombController : BasePlantController
{
    private bool hasExploded = false;

    public override void Initialize(PlantData data)
    {
        base.Initialize(data);
        StartCoroutine(ExplosionCoroutine());
    }

    private IEnumerator ExplosionCoroutine()
    {
        // 1s to puff up and explode (Frame-by-Frame animation sẽ chạy hết 1 vòng)
        yield return new WaitForSeconds(1.0f);
        
        hasExploded = true;
        
        // Tráo sang ảnh Boom.png
        if (plantAnimator != null)
        {
            plantAnimator.SetState(PlantState.Attack);
        }
        
        // Gây sát thương diện rộng 3x3 (bán kính khoảng 2.5f)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2.5f);
        foreach (var col in hits)
        {
            BaseZombieController z = col.GetComponent<BaseZombieController>();
            if (z != null)
            {
                z.TakeDamage(1800);
            }
        }
        
        // Khi đang hiển thị ảnh Boom, tàng hình với zombie để không bị cắn mất ảnh
        currentHealth = 99999;
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null) myCollider.enabled = false;

        // Đợi animation nổ 0.5s rồi chết
        yield return new WaitForSeconds(0.5f);
        
        currentHealth = 0;
        Die();
    }
}
