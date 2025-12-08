using UnityEngine;

public class InputManager : MonoBehaviour
{
    private SpriteRenderer crosshairRenderer;
    private Transform crosshairTransform;
    private static InputManager _instance;

    public static InputManager Instance => _instance;

    [SerializeField] private LayerMask enemyLayer;
    private Camera mainCamera;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        GameObject crosshairObj = GameObject.Find("Crosshair");
        if (crosshairObj != null)
        {
            crosshairTransform = crosshairObj.transform;
            crosshairRenderer = crosshairObj.GetComponent<SpriteRenderer>();
            if (crosshairRenderer != null)
            {
                crosshairRenderer.enabled = true;
            }
        }
    }

    private void Update()
    {
        bool isMainMenu = GameManager.Instance != null && GameManager.Instance.IsInMainMenu;

        // Cursor visibility
        if (isMainMenu)
        {
            Cursor.visible = true;
            if (crosshairRenderer != null)
                crosshairRenderer.enabled = false;
        }
        else
        {
            Cursor.visible = false;
            if (crosshairRenderer != null)
                crosshairRenderer.enabled = true;
        }

        // Click handling
        if (Input.GetMouseButtonDown(0) && !isMainMenu)
        {
            HandleClick();
        }

        UpdateCrosshairPosition();

        // Pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void UpdateCrosshairPosition()
    {
        if (crosshairTransform != null)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            crosshairTransform.position = worldPos;
        }
    }

    private void HandleClick()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity, enemyLayer);

        RaycastHit2D lootHit = default;
        RaycastHit2D coreHit = default;
        RaycastHit2D enemyHit = default;

        // Categorize hits by priority: Loot > Core > Enemy
        foreach (RaycastHit2D hit in hits)
        {
            Loot loot = hit.collider.GetComponent<Loot>();

            if (loot != null && lootHit.collider == null)
            {
                lootHit = hit;
            }
            else if (hit.collider.CompareTag("Core") && coreHit.collider == null)
            {
                coreHit = hit;
            }
            else if (enemyHit.collider == null)
            {
                enemyHit = hit;
            }
        }

        // Handle in priority order: Loot > Core > Enemy
        if (lootHit.collider != null)
        {
            HandleLootClick(lootHit);
        }
        else if (coreHit.collider != null)
        {
            HandleCoreClick(coreHit);
        }
        else if (enemyHit.collider != null)
        {
            HandleEnemyClick(enemyHit);
        }
    }

    private void HandleLootClick(RaycastHit2D hit)
    {
        Loot loot = hit.collider.GetComponent<Loot>();
        if (loot != null && !loot.IsCollected)
        {
            Debug.Log("LOOT");
            loot.OnLootClicked();
        }
    }

    private void HandleCoreClick(RaycastHit2D hit)
    {
        Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
        if (enemy == null || enemy.IsDead()) return;

        Debug.Log("COREHIT");

        // Get reaction time before killing enemy
        float timeTaken = enemy.EnemyDeathTimeTaken();

        // Kill the enemy
        enemy.OnEnemyClicked();

        // Notify systems
        EventManager.Instance.TriggerEnemyHit(enemy);

        // Delegate core hit handling to LevelManager (loot decision + light reward)
        LevelManager.Instance?.HandleCoreHit(enemy, timeTaken);

        // Route to PlayerManager for ability handling
        PlayerManager.Instance?.OnEnemyHit(enemy);
    }

    private void HandleEnemyClick(RaycastHit2D hit)
    {
        Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
        if (enemy == null || enemy.IsDead()) return;

        enemy.OnEnemyClicked();
        EventManager.Instance.TriggerEnemyHit(enemy);

        // Route to PlayerManager for ability handling
        PlayerManager.Instance?.OnEnemyHit(enemy);
    }

    private void TogglePause()
    {
        if (GameManager.Instance.IsInMainMenu ||
            GameManager.Instance.IsInLevelComplete ||
            GameManager.Instance.IsInGameOver)
            return;

        if (GameManager.Instance.IsInPaused)
        {
            GameManager.Instance.ResumeFromPause();

            if (GameManager.Instance.IsInLevelGameplay)
            {
                LevelManager.Instance?.ResumeCombat();
            }
        }
        else if (GameManager.Instance.IsInBase || GameManager.Instance.IsInLevelGameplay)
        {
            if (GameManager.Instance.IsInLevelGameplay)
            {
                LevelManager.Instance?.PauseCombat();
            }

            GameManager.Instance.TransitionToPaused();
        }
    }
}
