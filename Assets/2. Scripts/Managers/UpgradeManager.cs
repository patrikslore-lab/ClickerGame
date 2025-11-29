using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private PlayerConfig playerConfig;

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

    // Future upgrade methods will go here
}