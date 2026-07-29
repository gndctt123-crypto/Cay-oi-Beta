using UnityEngine;

public class ShooterController : BasePlantController
{
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float fireRate = 1.5f;

    private float fireTimer = 0f;

    protected override void Start()
    {
        base.Start();
        fireTimer = fireRate;
    }

    void Update()
    {
        // Simple logic: constantly shoot. A real game would use a Raycast or Physics2D to check for zombies in the lane first.
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        if (visuals != null) visuals.PlayAttackAnim();
        
        if (bulletPrefab != null && shootPoint != null)
        {
            Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        }
    }
}
