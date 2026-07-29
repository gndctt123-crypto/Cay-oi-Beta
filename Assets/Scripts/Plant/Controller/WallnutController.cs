using UnityEngine;

public class WallnutController : BasePlantController
{
    [Header("Wall-nut States")]
    public Sprite[] fullHealthFrames;
    public Sprite[] cracked1Frames;
    public Sprite[] cracked2Frames;

    private int maxHealth;

    public override void Initialize(PlantData data)
    {
        base.Initialize(data);
        maxHealth = plantData.health;
        UpdateVisuals();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (plantAnimator != null)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            if (healthPercent > 0.66f)
            {
                plantAnimator.idleFrames = fullHealthFrames;
            }
            else if (healthPercent > 0.33f)
            {
                plantAnimator.idleFrames = cracked1Frames;
            }
            else
            {
                plantAnimator.idleFrames = cracked2Frames;
            }
        }
    }
}
