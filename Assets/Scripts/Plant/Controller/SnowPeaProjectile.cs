using UnityEngine;

public class SnowPeaProjectile : Projectile
{
    public float slowPercent = 0.5f; // Giảm 50% tốc độ
    public float slowDuration = 10f; // Thời gian làm chậm (10 giây)

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        BaseZombieController zombie = collision.GetComponent<BaseZombieController>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage);
            zombie.ApplySlow(slowPercent, slowDuration);
            Destroy(gameObject);
        }
    }
}
