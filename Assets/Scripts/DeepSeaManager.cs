using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DeepSeaManager : MonoBehaviour
{
    [SerializeField] private Transform hamsi;
    [SerializeField] private List<DeepSeaFish> allFish;
    [SerializeField] private int chasersCount = 2;

    private bool hamsiInDeepSea = false;
    private List<DeepSeaFish> currentChasers = new List<DeepSeaFish>();

    void Update()
    {
        if (!hamsiInDeepSea) return;

        float depth = -hamsi.position.y;
        chasersCount = depth > 10f ? 3 : 2;

        var nearest = allFish
            .Where(f => f != null)
            .OrderBy(f => Vector2.Distance(f.transform.position, hamsi.position))
            .Take(chasersCount)
            .ToList();

        foreach (var fish in nearest)
        {
            if (!currentChasers.Contains(fish))
            {
                fish.StartChase(hamsi);
                currentChasers.Add(fish);
            }
        }

        foreach (var fish in currentChasers.Except(nearest).ToList())
        {
            fish.StopChase();
            currentChasers.Remove(fish);
        }
    }

    public void HamsiEnteredDeepSea()
    {
        hamsiInDeepSea = true;
    }

    public void HamsiExitedDeepSea()
    {
        hamsiInDeepSea = false;
        foreach (var fish in currentChasers)
            fish.StopChase();
        currentChasers.Clear();
    }
}