using UnityEngine;
using System.Collections;

public class PlantVisuals : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayAttackAnim()
    {
        if (animator != null) animator.SetTrigger("Attack");
    }

    public void FlashHurt()
    {
        StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1, 0.5f, 0.5f, 0.8f); // Reddish flash
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
}
