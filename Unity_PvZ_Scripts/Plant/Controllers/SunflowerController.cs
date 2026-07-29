using UnityEngine;

public class SunflowerController : BasePlantController
{
    public GameObject sunPrefab;
    public Transform spawnPoint;
    public float produceInterval = 24f; // Produce sun every 24 seconds

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(nameof(ProduceSun), produceInterval, produceInterval);
    }

    void ProduceSun()
    {
        if (visuals != null) visuals.PlayAttackAnim(); // Maybe a specific 'Produce' animation

        if (sunPrefab != null && spawnPoint != null)
        {
            // Instantiate slightly offset from the flower
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Instantiate(sunPrefab, spawnPoint.position + offset, Quaternion.identity);
        }
    }
}
