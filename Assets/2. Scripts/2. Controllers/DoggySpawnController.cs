// DoggySpawnController.cs
using UnityEngine;

/// <summary>
/// Utility spawner for non-combat items.
/// </summary>
public class DoggySpawnController : MonoBehaviour
{
    [SerializeField] private GameObject baseDoggy;
    public void SpawnBaseDoggy()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-14f, 14f),
            Random.Range(-7f, 3.5f),
            0f
        );
        Instantiate(baseDoggy, randomPos, Quaternion.identity);
        Debug.Log("Spawned Base Doggy for 30 wood");
    }
}
