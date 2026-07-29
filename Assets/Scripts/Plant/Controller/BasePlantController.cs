using UnityEngine;

public enum PlantState
{
    Idle,
    Attack,
    Digest, // Cho Chomper
    Sleep   // Cho Mushroom ban ngày
}

public class BasePlantController : MonoBehaviour
{
    [SerializeField] protected PlantData plantData;
    
    protected int currentHealth;
    protected PlantState currentState;
    protected BasePlantAnimator plantAnimator;

    public virtual void Initialize(PlantData data)
    {
        plantData = data;
        currentHealth = plantData.health;
        currentState = PlantState.Idle;
        plantAnimator = GetComponent<BasePlantAnimator>();
    }

    protected virtual void Update()
    {
        // Core logic cho mỗi loại cây được override ở đây
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        plantAnimator?.TriggerHitEffect();
    }

    protected virtual void Die()
    {
        // Thực hiện logic dọn dẹp khi chết (trả lại ô trồng cây thành trống)
        Destroy(gameObject);
    }

    public PlantState GetState() => currentState;
}
