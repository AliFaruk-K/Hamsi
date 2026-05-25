using UnityEngine;
using System.Collections;

public class FishSpawner : MonoBehaviour
{
    public static FishSpawner instance;

    [SerializeField] private GameObject smallFishPrefab;
    [SerializeField] private int fishCount = 10;
    [SerializeField] private float seaSurfaceY = 0f;
    [SerializeField] private float seaBottomY = -10f;
    [SerializeField] private float mapHalfWidth = 50f; // CameraFollow ile ayný deðer olsun

    private int currentFishCount = 0;

    void Awake() => instance = this;

    void Start()
    {
        for (int i = 0; i < fishCount; i++)
            SpawnFish();
    }

    public void OnFishEaten(SmallFish fish)
    {
        HungerSystem.instance.OnFishEaten();
        ScoreManager.instance.AddScore(10);
        currentFishCount--;
        Destroy(fish.gameObject);
        StartCoroutine(SpawnAfterDelay());
    }

    IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        SpawnFish();
    }

    void SpawnFish()
    {
        // Haritanýn herhangi bir yerine rastgele spawn et
        float spawnX = Random.Range(-mapHalfWidth, mapHalfWidth);
        float spawnY = Random.Range(seaBottomY, seaSurfaceY - 0.5f);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
        Instantiate(smallFishPrefab, spawnPos, Quaternion.identity);
        currentFishCount++;
    }
}