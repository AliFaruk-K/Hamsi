using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int scoreForGoldenBird = 100;

    private int currentScore = 0;
    private int lastGoldenBirdScore = -999;
    private bool goldenBirdActive = false;
    private bool goldenBirdOnMap = false;

    void Awake() => instance = this;

    void Update()
    {
        scoreText.text = "Puan: " + currentScore.ToString();

        if (!goldenBirdActive && !goldenBirdOnMap)
        {
            if (currentScore - lastGoldenBirdScore >= scoreForGoldenBird)
            {
                GoldenBirdSpawner.instance.SpawnGoldenBird();
                goldenBirdOnMap = true;
            }
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    public int GetScore()
    {
        return currentScore;
    }

    public void OnGoldenBirdEaten()
    {
        goldenBirdOnMap = false;
        goldenBirdActive = true;
    }

    public void OnPowerUpEnded()
    {
        goldenBirdActive = false;
        lastGoldenBirdScore = currentScore;
    }
}