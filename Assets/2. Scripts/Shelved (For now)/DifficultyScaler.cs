using UnityEngine;

public class DifficultyScaler : MonoBehaviour
{
    [Header("Spawn Rate Ramping")]
    [SerializeField] private float spawnRateIncrease = 0.1f;   // How much faster it gets per second
    [SerializeField] private float minSpawnRate = 0.5f;        // The fastest it can go

    private float elapsedTime = 0f;           // How many seconds have passed since game started

    void Update()
    {
        // Keep track of total time elapsed since the game started
        elapsedTime += Time.deltaTime;
    }

    /// <summary>
    /// Calculates the difficulty-adjusted spawn rate based on a base spawn rate
    /// Takes each enemy's base spawn rate and makes it faster over time
    /// </summary>
    public float GetCurrentSpawnRate(float baseSpawnRate)
    {
        // Reduce spawn rate by the difficulty multiplier
        float adjustedSpawnRate = baseSpawnRate - (elapsedTime * spawnRateIncrease);

        // Make sure it never goes faster than minSpawnRate
        return Mathf.Max(adjustedSpawnRate, minSpawnRate);
    }
}