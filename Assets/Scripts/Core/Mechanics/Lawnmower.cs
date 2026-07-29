using UnityEngine;

public class Lawnmower : MonoBehaviour
{
    public float speed = 5.0f;
    private bool isActivated = false;

    void Update()
    {
        if (isActivated)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            
            // Tự hủy khi chạy ra khỏi màn hình
            if (transform.position.x > 15f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseZombieController zombie = collision.GetComponent<BaseZombieController>();
        if (zombie != null)
        {
            // Kích hoạt máy cắt cỏ
            isActivated = true;
            
            // Tiêu diệt zombie ngay lập tức
            zombie.TakeDamage(9999);
        }
    }
}
