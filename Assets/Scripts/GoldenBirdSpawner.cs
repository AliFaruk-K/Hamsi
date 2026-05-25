using UnityEngine;

public class GoldenBirdSpawner : MonoBehaviour
{
    public static GoldenBirdSpawner instance;

    [SerializeField] private GameObject goldenBirdPrefab;
    [SerializeField] private float minY = 8f;
    [SerializeField] private float maxY = 14f;
    [SerializeField] private float mapHalfWidth = 50f;

    private GameObject currentGoldenBird;

    void Awake() => instance = this;

    public void SpawnGoldenBird()
    {
        if (currentGoldenBird != null) return;

        float spawnX = Random.Range(-mapHalfWidth, mapHalfWidth);
        float spawnY = Random.Range(minY, maxY);
        currentGoldenBird = Instantiate(goldenBirdPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
    }

    public void OnGoldenBirdEaten()
    {
        ScoreManager.instance.AddScore(100);
        currentGoldenBird = null;
    }
}