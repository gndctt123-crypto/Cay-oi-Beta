using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 20;
    
    // For SnowPea
    public bool isIce = false;

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            BaseZombieController zombie = collision.GetComponent<BaseZombieController>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
                // If you implement a slow effect, you can call it here:
                // if (isIce) zombie.ApplySlow(); 
            }
            // Destroy bullet after hitting a zombie
            Destroy(gameObject); 
        }
        else if (collision.CompareTag("Boundary")) // Invisible wall on the right to destroy bullets that miss
        {
            Destroy(gameObject);
        }
    }
}
