using UnityEngine;
using GameStateMachine;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] UpgradeController upgradeController;
    [SerializeField] DoggySpawnController doggySpawnController;
    private PlayerConfig playerConfig;
    [SerializeField] float doggyCost;
    public static PlayerManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Safe to access GameManager here - Start() runs after all Awake() methods
        if (GameManager.Instance != null)
        {
            playerConfig = GameManager.Instance.GetPlayerConfig();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null in UpgradeManager.Start()!");
        }
    }

    private void Update()
    {

    }

    //Buy Doggy - accessed by UI Upgrade button directly
    public void BuyDoggy()
    {
        if (playerConfig.wood >= doggyCost)
        {
            upgradeController.PayDoggy(doggyCost);
            doggySpawnController.SpawnBaseDoggy();
            UIManager.Instance.UpdateWoodCountUI(playerConfig.wood);
        }
        else 
        {
         Debug.Log("Not enough wood for doggy!");
         return;   
        }
    }
}
