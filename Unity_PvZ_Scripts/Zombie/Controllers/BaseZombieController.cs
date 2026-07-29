using UnityEngine;

public class BaseZombieController : MonoBehaviour
{
    public ZombieData zombieData;
    protected int currentHealth;
    protected bool isAttacking = false;
    protected BasePlantController targetPlant;
    
    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        if (zombieData != null)
        {
            currentHealth = zombieData.maxHealth;
        }
    }

    protected virtual void Update()
    {
        if (!isAttacking)
        {
            Move();
        }
    }

    protected virtual void Move()
    {
        transform.Translate(Vector3.left * zombieData.moveSpeed * Time.deltaTime);
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (animator != null) animator.SetTrigger("Die");
        // Disable collider so plants don't keep attacking it
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        Destroy(gameObject, 1.5f); // wait for animation
    }

    // Called via Unity's Physics2D
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Plant"))
        {
            targetPlant = collision.GetComponent<BasePlantController>();
            if (targetPlant != null)
            {
                StartAttacking();
            }
        }
    }

    protected virtual void StartAttacking()
    {
        isAttacking = true;
        if (animator != null) animator.SetBool("IsAttacking", true);
        InvokeRepeating(nameof(DealDamageToPlant), 0f, zombieData.attackSpeed);
    }

    protected virtual void DealDamageToPlant()
    {
        if (targetPlant != null)
        {
            targetPlant.TakeDamage(zombieData.damage);
        }
        else
        {
            // Plant died, resume moving
            isAttacking = false;
            if (animator != null) animator.SetBool("IsAttacking", false);
            CancelInvoke(nameof(DealDamageToPlant));
        }
    }
}
