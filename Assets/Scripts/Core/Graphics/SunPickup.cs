using UnityEngine;

public class SunPickup : MonoBehaviour
{
    [Header("Settings")]
    public int sunValue = GameConstants.SunValue;
    public float lifeTime = GameConstants.SunLiveTime;
    
    // Dùng cho hiệu ứng rớt xuống
    private Vector2 targetPosition;
    private bool isFalling = false;
    private float fallSpeed = 1f;

    private float timer = 0f;

    public void SetupDrop(Vector2 startPos, Vector2 targetPos)
    {
        transform.position = startPos;
        targetPosition = targetPos;
        isFalling = true;
    }

    private bool isCollected = false;

    private void Collect()
    {
        if (isCollected) return; // Tránh bấm nhiều lần
        isCollected = true;
        
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.AddSun(sunValue);
        }
        else
        {
            Debug.Log($"+{sunValue} Mặt trời!");
        }
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit != null && hit.gameObject == gameObject)
            {
                Collect();
            }
        }

        if (isCollected)
        {
            // Điểm đích (Góc trên cùng bên trái màn hình)
            Vector3 uiTarget = Camera.main.ScreenToWorldPoint(new Vector3(50, Screen.height - 50, Camera.main.nearClipPlane));
            uiTarget.z = 0;
            
            transform.position = Vector3.MoveTowards(transform.position, uiTarget, 15f * Time.deltaTime);
            if (Vector3.Distance(transform.position, uiTarget) < 0.5f)
            {
                Destroy(gameObject);
            }
            return;
        }

        if (isFalling)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isFalling = false;
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
