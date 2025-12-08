// JuneCharacter.cs
using UnityEngine;
using System.Collections;

/// <summary>
/// The June character component - attach this to June's GameObject in the scene.
/// Handles idle movement and provides movement API for abilities.
/// </summary>
public class JuneCharacter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isPerformingAbility = false;
    private PlayerConfig playerConfig;

    // Position and movement settings
    private Vector3 homePosition;
    private float idleMovementRadius;
    private float idleMovementSpeed;

    // Perlin noise for idle movement
    private float noiseOffsetX;
    private float noiseOffsetY;

    // Public accessors
    public bool IsPerformingAbility => isPerformingAbility;
    public GameObject JuneInstance => gameObject;  // This IS June

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();

        // Store initial position as home
        homePosition = playerConfig.juneHomePosition;
        idleMovementRadius = playerConfig.juneIdleMovementRadius;
        idleMovementSpeed = playerConfig.juneIdleMovementSpeed;

        // Set initial position
        transform.position = homePosition;

        spriteRenderer = GetComponent<SpriteRenderer>();

        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetY = Random.Range(0f, 100f);

        Debug.Log($"JuneCharacter initialized at {homePosition}");
    }

    private void Update()
    {
        if (!isPerformingAbility)
        {
            IdleMovement();
        }
    }

    //===========================================
    // IDLE MOVEMENT
    //===========================================

    private void IdleMovement()
    {
        float noiseX = Mathf.PerlinNoise((Time.time * idleMovementSpeed) + noiseOffsetX, 0f);
        float noiseY = Mathf.PerlinNoise(0f, (Time.time * idleMovementSpeed) + noiseOffsetY);

        // Remap from [0,1] to [-1,1]
        noiseX = (noiseX - 0.5f) * 2f;
        noiseY = (noiseY - 0.5f) * 2f;

        Vector3 offset = new Vector3(noiseX, noiseY, 0f) * idleMovementRadius;
        transform.position = homePosition + offset;
    }

    //===========================================
    // PUBLIC API (for abilities)
    //===========================================

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
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }

    public IEnumerator ReturnJuneHome()
    {
        yield return MoveJuneToPosition(homePosition, 0.5f);
        isPerformingAbility = false;
    }
}
