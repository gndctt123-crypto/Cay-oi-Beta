using UnityEngine;
using System.Collections;

public class BasePlantAnimator : MonoBehaviour
{
    [Header("Animation Frames")]
    public Sprite[] idleFrames;
    public Sprite[] attackFrames;
    public float frameRate = 0.1f;
    public bool loopAnimation = true; // Cho phép tắt lặp lại đối với các hoạt họa nổ 1 lần (như Boom)

    protected SpriteRenderer spriteRenderer;
    protected PlantState currentState = PlantState.Idle;
    
    private int currentFrame = 0;
    private float timer = 0f;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (spriteRenderer == null) return;

        Sprite[] currentArray = (currentState == PlantState.Attack && attackFrames != null && attackFrames.Length > 0) 
                                ? attackFrames 
                                : idleFrames;

        if (currentArray == null || currentArray.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame++;

            if (!loopAnimation && currentFrame >= currentArray.Length)
            {
                currentFrame = currentArray.Length - 1; // Giữ ở frame cuối cùng
            }
            else
            {
                currentFrame %= currentArray.Length; // Lặp lại bình thường
            }

            spriteRenderer.sprite = currentArray[currentFrame];
        }
    }

    public virtual void SetState(PlantState state)
    {
        if (currentState != state)
        {
            currentState = state;
            currentFrame = 0;
            timer = 0f;
        }
    }

    public bool enableHitFlash = true;
    private bool isBlinking = false;

    public virtual void TriggerHitEffect()
    {
        if (enableHitFlash && !isBlinking && spriteRenderer != null)
        {
            StartCoroutine(HitBlinkCoroutine());
        }
    }

    private IEnumerator HitBlinkCoroutine()
    {
        isBlinking = true;
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(1f, 0.5f, 0.5f, 1f); // Nháy đỏ nhạt
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        isBlinking = false;
    }
}
