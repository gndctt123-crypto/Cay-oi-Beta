using UnityEngine;

public class ThreepeaterController : ShooterController
{
    private float[] offsets = new float[] { 1.2f, 0f, -1.2f }; // Top, Middle, Bottom (in PVZ, y is negative going down, so +1.2 is top, -1.2 is bottom)

    protected override bool CheckForZombiesInLane()
    {
        foreach (float offset in offsets)
        {
            Vector3 checkPos = transform.position + new Vector3(0, offset, 0);
            RaycastHit2D[] hits = Physics2D.RaycastAll(checkPos, Vector2.right, 15f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.GetComponent<BaseZombieController>() != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected override void Shoot()
    {
        Debug.Log($"[Kiểm định] Cây {gameObject.name} nhả 3 đạn!");
        if (projectilePrefab != null)
        {
            plantAnimator?.SetState(PlantState.Attack);
            
            Vector3 baseSpawnPos = shootPoint != null ? shootPoint.position : (transform.position + new Vector3(0.5f, 0.2f, 0));
            
            foreach (float offset in offsets)
            {
                Vector3 spawnPos = baseSpawnPos + new Vector3(0, offset, 0);
                GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                Projectile projScript = projectile.GetComponent<Projectile>();
                if (projScript != null)
                {
                    projScript.damage = plantData.damage;
                }
            }
        }
    }
}
