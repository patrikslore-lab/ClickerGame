using UnityEngine;

public class InputManager : MonoBehaviour
{
    // References to the Sprite Renderer for crosshair
    private SpriteRenderer crosshairRenderer;
    private Transform crosshairTransform;
    private static InputManager _instance;
    
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                
            }
            return _instance;
        }
    }
    
    [SerializeField] private LayerMask enemyLayer;
    private Camera mainCamera;

    private void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // InputManager setup
        mainCamera = Camera.main;
    }
    
    private void Start()
    {
        //Crosshair Logic
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
        // Manage cursor and crosshair visibility based on game state
        bool isMainMenu = GameManager.Instance != null && GameManager.Instance.IsInMainMenu;

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

        if (Input.GetMouseButtonDown(0))
        {
            if (!isMainMenu)
            {
                HandleClick();
            }
        }

        UpdateCrosshairPosition();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void UpdateCrosshairPosition()
    {
        if (crosshairTransform != null)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0; // Keep at z=0 for 2D rendering
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

        // Categorize hits by component type (priority: Loot > Core > Enemy)
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
            Loot loot = lootHit.collider.GetComponent<Loot>();
            if (loot != null)
            {
                Debug.Log("LOOT");
                EventManager.Instance.TriggerLootHit(loot);
            }
        }
        else if (coreHit.collider != null)
        {
            Enemy enemy = coreHit.collider.GetComponentInParent<Enemy>();

            if (enemy != null && !enemy.IsDead())
            {
                Debug.Log("COREHIT");
                EventManager.Instance.TriggerCoreHit(enemy);
                enemy.OnEnemyClicked();
                EventManager.Instance.TriggerEnemyHit(enemy);

                if (enemy.EnemyDeathTimeTaken() <= 2000) // Max click time on core to award core loot
                {
                    if (LevelManager.Instance != null)
                    {
                        LevelManager.Instance.SpawnCoreLoot(enemy.transform.position);
                    }
                }
                // TODO: Add conditional core loot logic here if needed
                // Example: if (someCondition) { EventManager.Instance.TriggerLootHit(coreLoot); }
            }
        }
        else if (enemyHit.collider != null)
        {
            Enemy enemy = enemyHit.collider.GetComponentInParent<Enemy>();

            if (enemy != null && !enemy.IsDead())
            {
                enemy.OnEnemyClicked();
                EventManager.Instance.TriggerEnemyHit(enemy);
            }
        }
    }
    private void TogglePause()
    {
        // Don't allow pausing in MainMenu, LevelComplete, or GameOver states
        if (GameManager.Instance.IsInMainMenu ||
            GameManager.Instance.IsInLevelComplete ||
            GameManager.Instance.IsInGameOver)
            return;

        if (GameManager.Instance.IsInPaused)
        {
            // Resume from pause - GameManager will restore previous state
            GameManager.Instance.ResumeFromPause();

            // Only resume combat if we're returning to LevelGameplay
            if (GameManager.Instance.IsInLevelGameplay)
            {
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.ResumeCombat();
                }
            }
        }
        else if (GameManager.Instance.IsInBase || GameManager.Instance.IsInLevelGameplay)
        {
            // Pause combat if we're currently in LevelGameplay
            if (GameManager.Instance.IsInLevelGameplay)
            {
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.PauseCombat();
                }
            }

            GameManager.Instance.TransitionToPaused();
        }
    }
}