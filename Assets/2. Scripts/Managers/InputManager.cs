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
        DontDestroyOnLoad(gameObject);

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
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
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
                    SpawnManagerScript.Instance.SpawnCoreLoot(enemy);
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
        GameManager.GameState currentState = GameManager.Instance.CurrentGameState;
        GameManager.GameMode currentMode = GameManager.Instance.CurrentGameMode;

        // Don't allow pausing in MainMenu
        if (currentMode == GameManager.GameMode.MainMenu)
            return;

        if (currentState == GameManager.GameState.Playing)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Paused);

            // Only pause waves if we're in Combat mode
            if (currentMode == GameManager.GameMode.Combat)
            {
                RoomManager roomManager = FindAnyObjectByType<RoomManager>();
                if (roomManager != null)
                {
                    roomManager.PauseWaves();
                }
            }
        }
        else if (currentState == GameManager.GameState.Paused)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Playing);

            // Only resume waves if we're in Combat mode
            if (currentMode == GameManager.GameMode.Combat)
            {
                RoomManager roomManager = FindAnyObjectByType<RoomManager>();
                if (roomManager != null)
                {
                    roomManager.ResumeWaves();
                }
            }
        }
    }
}