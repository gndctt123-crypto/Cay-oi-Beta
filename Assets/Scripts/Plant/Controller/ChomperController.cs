using UnityEngine;
using System.Collections;

public class ChomperController : BasePlantController
{
    private bool isEating = false;

    public Sprite[] digestFrames;
    public Sprite[] idleFrames;

    public override void Initialize(PlantData data)
    {
        base.Initialize(data);
        if (plantAnimator != null)
        {
            idleFrames = plantAnimator.idleFrames; // Lưu lại idle gốc
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isEating || currentHealth <= 0) return;

        // Quét Zombie phía trước mặt trong khoảng cách ngắn (1 ô)
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right, 1f);
        foreach (RaycastHit2D hit in hits)
        {
            BaseZombieController zombie = hit.collider.GetComponent<BaseZombieController>();
            if (zombie != null)
            {
                EatZombie(zombie);
                break; // Cắn con đầu tiên thấy được
            }
        }
    }

    private void EatZombie(BaseZombieController zombie)
    {
        isEating = true;
        plantAnimator?.SetState(PlantState.Attack);
        
        // Đóng băng zombie ngay lập tức (không cho di chuyển/tấn công)
        zombie.StartBeingEaten();

        // Bắt đầu quá trình nhai (nuốt xác sau 0.8s khi kết thúc animation cắn)
        StartCoroutine(DigestCoroutine(zombie));
    }

    private IEnumerator DigestCoroutine(BaseZombieController zombie)
    {
        // Chờ animation cắn xong (tuỳ frameRate) -> chuyển về idle trạng thái đang nhai
        yield return new WaitForSeconds(0.8f);
        
        // Nuốt chửng zombie vào bụng (biến mất khỏi màn hình)
        if (zombie != null)
        {
            zombie.Swallow();
        }
        
        if (plantAnimator != null)
        {
            plantAnimator.idleFrames = digestFrames;
            plantAnimator.SetState(PlantState.Idle);
        }
        
        // Quá trình nhai 40 giây
        yield return new WaitForSeconds(40f);
        
        isEating = false;
        if (plantAnimator != null)
        {
            plantAnimator.idleFrames = idleFrames;
            plantAnimator.SetState(PlantState.Idle);
        }
    }
}
