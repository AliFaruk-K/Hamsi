using UnityEngine;
using UnityEngine.UI;

public class HungerSystem : MonoBehaviour
{
    public static HungerSystem instance;

    [SerializeField] private Slider hungerBar;
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float initialHungerRate = 5f;
    [SerializeField] private float hungerRateIncrease = 0.5f;
    [SerializeField] private float hungerRestoreAmount = 30f;
    [SerializeField] private HamsiScript hamsi;

    private float currentHunger;
    private float currentHungerRate;
    private float rateIncreaseTimer;

    void Awake() => instance = this;

    void Start()
    {
        currentHunger = maxHunger;
        currentHungerRate = initialHungerRate;
        hungerBar.maxValue = maxHunger;
        hungerBar.value = maxHunger;
    }

    void Update()
    {
        currentHunger -= currentHungerRate * Time.deltaTime;
        hungerBar.value = currentHunger;

        rateIncreaseTimer += Time.deltaTime;
        if (rateIncreaseTimer >= 10f)
        {
            rateIncreaseTimer = 0f;
            currentHungerRate += hungerRateIncrease;
            // Her artýþta bir öncekinin yüzde 10 fazlasý artsýn, katlanarak zorlaþsýn
            hungerRateIncrease *= 1.1f;
        }

        if (currentHunger <= 0f)
        {
            currentHunger = 0f;
            GameOver();
        }
    }

    public void OnFishEaten()
    {
        currentHunger = Mathf.Min(currentHunger + hungerRestoreAmount, maxHunger);
    }

    public void OnBirdEaten()
    {
        // Kuþ 2 kat tok tutuyor
        currentHunger = Mathf.Min(currentHunger + hungerRestoreAmount * 2f, maxHunger);
    }

    void GameOver()
    {
        Debug.Log("Açlýktan öldü!");
        hamsi.Die();
    }
}