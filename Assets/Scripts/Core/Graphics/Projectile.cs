using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 5f;
    public int damage = 1;
    public bool isIce = false; // Dành cho Đậu Băng (SnowPea)

    private void Update()
    {
        // Viên đạn luôn bay thẳng về bên phải
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Huỷ viên đạn nếu bay ra khỏi màn hình (x > 10 là ví dụ ngoài rìa)
        if (transform.position.x > 15f)
        {
            Destroy(gameObject);
        }
    }

    public GameObject hitEffectPrefab;

    private void Awake()
    {
        if (hitEffectPrefab == null)
        {
#if UNITY_EDITOR
            hitEffectPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Core/Prefabs/PeaSplat.prefab");
#endif
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        BaseZombieController zombie = collision.GetComponent<BaseZombieController>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage);
            
            // Spawn hit effect (Splat)
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 0.2f); // Hiển thị 0.2s rồi tự xóa
            }
            
            Destroy(gameObject);
        }
    }
}
