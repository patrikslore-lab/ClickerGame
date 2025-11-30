using UnityEngine;
using System.Collections;

public class JuneCharacter : MonoBehaviour
{
    [Header("June Prefab")]
    [SerializeField] protected GameObject junePrefab;

    protected GameObject juneInstance;
    protected SpriteRenderer juneSpriteRenderer;
    protected bool isPerformingAbility = false;
    protected PlayerConfig playerConfig;

    public bool IsPerformingAbility => isPerformingAbility;
    public GameObject JuneInstance => juneInstance;

    // Position and movement settings (loaded from PlayerConfig)
    protected Vector3 homePosition;
    protected float idleMovementRadius;
    protected float idleMovementSpeed;

    // Perlin noise idle movement
    protected float noiseOffsetX;
    protected float noiseOffsetY;

    protected virtual void Start()
    {
        // Load settings from PlayerConfig
        playerConfig = GameManager.Instance.GetPlayerConfig();
        homePosition = playerConfig.juneHomePosition;
        idleMovementRadius = playerConfig.juneIdleMovementRadius;
        idleMovementSpeed = playerConfig.juneIdleMovementSpeed;

        SpawnJune();

        noiseOffsetX = Random.Range(0f, 1f);
        noiseOffsetY = Random.Range(0f, 1f);
    }

    protected virtual void Update()
    {
        if (juneInstance == null) return;

        if (!isPerformingAbility)
        {
            IdleMovement();
        }
    }

    protected void IdleMovement()
    {
        // Sample Perlin noise for smooth, organic movement
        float noiseX = Mathf.PerlinNoise((Time.time * idleMovementSpeed) + noiseOffsetX, 0f);
        float noiseY = Mathf.PerlinNoise(0f, (Time.time * idleMovementSpeed) + noiseOffsetY);

        // Remap from [0,1] to [-1,1] for symmetric movement
        noiseX = (noiseX - 0.5f) * 2f;
        noiseY = (noiseY - 0.5f) * 2f;

        // Apply offset from home position
        Vector3 offset = new Vector3(noiseX, noiseY, 0f) * idleMovementRadius;
        juneInstance.transform.position = homePosition + offset;
    }

    protected IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = juneInstance.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            juneInstance.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        juneInstance.transform.position = targetPosition;
    }

    protected IEnumerator ReturnHome()
    {
        yield return MoveToPosition(homePosition, 0.5f);
        isPerformingAbility = false;
    }

    // Public API for abilities to control June
    public void StartAbilityControl()
    {
        isPerformingAbility = true;
    }

    public void EndAbilityControl()
    {
        isPerformingAbility = false;
    }

    public IEnumerator MoveJuneToPosition(Vector3 targetPosition, float duration)
    {
        return MoveToPosition(targetPosition, duration);
    }

    public IEnumerator ReturnJuneHome()
    {
        return ReturnHome();
    }

    private void SpawnJune()
    {
        if (junePrefab == null)
        {
            Debug.LogError("JuneCharacter: June prefab not assigned!");
            return;
        }

        juneInstance = Instantiate(junePrefab, homePosition, Quaternion.identity);
        DontDestroyOnLoad(juneInstance);
        juneSpriteRenderer = juneInstance.GetComponent<SpriteRenderer>();

        if (juneSpriteRenderer == null)
        {
            Debug.LogWarning("JuneCharacter: June prefab missing SpriteRenderer component");
        }
    }

    protected virtual void OnDestroy()
    {
        if (juneInstance != null)
        {
            Destroy(juneInstance);
        }
    }
}
