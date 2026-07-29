using UnityEngine;
using System.Collections;

public class RepeaterController : ShooterController
{
    protected override void Shoot()
    {
        // Bắn viên đạn đầu tiên
        base.Shoot();
        // Bắn viên đạn thứ hai sau 0.15 giây
        StartCoroutine(ShootSecondPea());
    }

    private IEnumerator ShootSecondPea()
    {
        yield return new WaitForSeconds(0.15f);
        base.Shoot();
    }
}
