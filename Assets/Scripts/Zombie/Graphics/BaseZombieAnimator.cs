using UnityEngine;
using System.Collections;

public class BaseZombieAnimator : MonoBehaviour
{
    [Header("Animation Frames")]
    public Sprite[] walkFrames;
    public Sprite[] attackFrames;
    public Sprite[] dieFrames;
    public float frameRate = 0.15f;

    protected SpriteRenderer spriteRenderer;
    protected ZombieState currentState = ZombieState.Walk;
    
    private int currentFrame = 0;
    private float timer = 0f;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (spriteRenderer == null) return;

        Sprite[] currentArray = walkFrames;
        if (currentState == ZombieState.Attack && attackFrames != null && attackFrames.Length > 0)
            currentArray = attackFrames;
        else if (currentState == ZombieState.Die && dieFrames != null && dieFrames.Length > 0)
            currentArray = dieFrames;

        if (currentArray == null || currentArray.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame++;
            
            // Nếu đang chết, dừng ở frame cuối
            if (currentState == ZombieState.Die && currentFrame >= currentArray.Length)
            {
                currentFrame = currentArray.Length - 1;
            }
            else
            {
                currentFrame %= currentArray.Length;
            }
            
            spriteRenderer.sprite = currentArray[currentFrame];
        }
    }

    public virtual void SetState(ZombieState state)
    {
        if (currentState != state)
        {
            currentState = state;
            currentFrame = 0;
            timer = 0f;
        }
    }

    private Color baseColor = Color.white;
    private bool isBlinking = false;

    public void SetBaseColor(Color c)
    {
        baseColor = c;
        if (!isBlinking && spriteRenderer != null)
        {
            spriteRenderer.color = baseColor;
        }
    }

    public virtual void TriggerHitEffect()
    {
        if (!isBlinking)
        {
            StartCoroutine(HitBlinkCoroutine());
        }
    }

    private IEnumerator HitBlinkCoroutine()
    {
        isBlinking = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 1f); // Nháy đỏ sáng
            yield return new WaitForSeconds(0.1f);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = baseColor;
            }
        }
        isBlinking = false;
    }
}
