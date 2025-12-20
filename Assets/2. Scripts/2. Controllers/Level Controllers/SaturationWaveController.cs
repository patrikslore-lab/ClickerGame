using UnityEngine;

/// <summary>
/// Controller for spawning saturation wave effects on S-rank enemy kills.
/// Managed by LevelManager - called directly from Enemy.OnEnemyClicked().
/// </summary>
public class SaturationWaveController : MonoBehaviour
{
    [Header("Wave Prefab")]
    [SerializeField] private SaturationWave wavePrefab;

    [Header("Wave Settings")]
    [SerializeField] private float expansionSpeed = 8f;
    [SerializeField] private float waveThickness = 2f;
    [SerializeField] private float maxRadius = 20f;

    [Header("Testing")]
    [SerializeField] private bool testSpawnAtOrigin = false;

    /// <summary>
    /// Spawns a saturation wave at the specified position.
    /// Called by Enemy.OnEnemyClicked() when an S-rank kill occurs.
    /// </summary>
    public void SpawnWave(Vector3 position)
    {
        if (wavePrefab == null)
        {
            Debug.LogWarning("SaturationWave prefab not assigned to SaturationWaveController!");
            return;
        }

        // For testing: override position to origin if enabled
        if (testSpawnAtOrigin)
        {
            position = Vector3.zero;
        }

        SaturationWave wave = Instantiate(wavePrefab, position, Quaternion.identity);
        wave.Initialize(position, expansionSpeed, waveThickness, maxRadius);

        Debug.Log($"✓ SaturationWave spawned at {position} (Speed: {expansionSpeed}, Thickness: {waveThickness}, MaxRadius: {maxRadius})");
    }
}
