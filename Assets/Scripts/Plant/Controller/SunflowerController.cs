using UnityEngine;

public class SunflowerController : BasePlantController
{
    [Header("Sunflower Settings")]
    public GameObject sunPrefab; // Chứa Prefab của Mặt Trời
    public Transform dropPoint; // Vị trí giọt mặt trời rơi ra (thường là dưới chân cây)

    private float produceTimer;

    public override void Initialize(PlantData data)
    {
        base.Initialize(data);
        produceTimer = 0f;
    }

    protected override void Update()
    {
        base.Update();
        
        // Theo nguyên tác, Sunflower rớt mặt trời lần đầu sau thời gian ngắn, 
        // nhưng ở đây cứ dùng attackInterval (khoảng 24s trong game gốc) để đếm
        produceTimer += Time.deltaTime;
        if (produceTimer >= plantData.attackInterval)
        {
            ProduceSun();
            produceTimer = 0f;
        }
    }

    private void ProduceSun()
    {
        if (sunPrefab != null)
        {
            // Bật hiệu ứng giật giật vươn lên của Hướng dương
            plantAnimator?.SetState(PlantState.Attack);
            
            // Tính toán vị trí nảy ra của mặt trời
            Vector2 spawnPos = transform.position;
            Vector2 dropPos = dropPoint != null ? dropPoint.position : spawnPos + new Vector2(0.5f, -0.5f);

            GameObject sunObj = Instantiate(sunPrefab, spawnPos, Quaternion.identity);
            SunPickup sunPickup = sunObj.GetComponent<SunPickup>();
            if (sunPickup != null)
            {
                sunPickup.sunValue = 50;
                // Rơi từ vị trí cây đến vị trí dropPos bên dưới
                sunPickup.SetupDrop(spawnPos, dropPos);
            }
        }
    }
}
