using UnityEngine;
using System.Collections;

public class BirdSpawner : MonoBehaviour
{
    public static BirdSpawner instance;

    [SerializeField] private GameObject birdPrefab;
    [SerializeField] private int birdCount = 5;
    [SerializeField] private float minY = 2f;
    [SerializeField] private float maxY = 10f;
    [SerializeField] private float mapHalfWidth = 50f;

    void Awake() => instance = this;

    void Start()
    {
        for (int i = 0; i < birdCount; i++)
            SpawnBird();
    }

    public void OnBirdEaten(BirdScript bird)
    {
        HungerSystem.instance.OnBirdEaten();
        ScoreManager.instance.AddScore(25);
        Destroy(bird.gameObject);
        StartCoroutine(SpawnAfterDelay());
    }

    IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        SpawnBird();
    }

    void SpawnBird()
    {
        float spawnX = Random.Range(-mapHalfWidth, mapHalfWidth);
        float spawnY = Random.Range(minY, maxY);
        Instantiate(birdPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
    }
}