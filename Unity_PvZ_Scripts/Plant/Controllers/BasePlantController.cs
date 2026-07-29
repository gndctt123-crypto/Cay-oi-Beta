using UnityEngine;

public abstract class BasePlantController : MonoBehaviour
{
    public PlantData plantData;
    protected int currentHealth;
    protected PlantVisuals visuals;

    protected virtual void Awake()
    {
        visuals = GetComponentInChildren<PlantVisuals>();
    }

    protected virtual void Start()
    {
        if (plantData != null)
        {
            currentHealth = plantData.maxHealth;
        }
    }

    public virtual void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (visuals != null) visuals.FlashHurt();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
