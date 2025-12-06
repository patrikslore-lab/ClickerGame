// DoggySpawnController.cs
using UnityEngine;

/// <summary>
/// Utility spawner for non-combat items.
/// </summary>
public class DoggySpawnController : MonoBehaviour
{
    public static DoggySpawnController Instance { get; private set; }

    [SerializeField] private GameObject baseDoggy;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnBaseDoggy()
    {
        PlayerConfig playerConfig = GameManager.Instance.GetPlayerConfig();

        if (playerConfig.wood >= 30)
        {
            playerConfig.wood -= 30;
            UIManager.Instance.UpdateWoodCountUI(playerConfig.wood);

            Vector3 randomPos = new Vector3(
                Random.Range(-14f, 14f),
                Random.Range(-7f, 3.5f),
                0f
            );
            Instantiate(baseDoggy, randomPos, Quaternion.identity);
            Debug.Log("Spawned Base Doggy for 30 wood");
        }
        else
        {
            Debug.Log($"Not enough wood! Need 30, have {playerConfig.wood}");
        }
    }
}
