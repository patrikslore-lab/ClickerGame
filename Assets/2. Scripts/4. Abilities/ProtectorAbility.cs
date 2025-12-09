// ProtectorAbility.cs
using UnityEngine;
using System.Collections;

/// <summary>
/// Protector ability - when active, June circles the screen center and adds light.
/// </summary>
public class ProtectorAbility : BaseAbility, IAbility
{
    [Header("Circle Settings")]
    [SerializeField] private float circleRadius = 2f;
    [SerializeField] private float circleSpeed = 3f;
    [SerializeField] private Vector3 circleCenter = Vector3.zero;

    private PlayerConfig playerConfig;
    private JuneCharacter june;
    private AbilityController abilityController;
    private float timer;
    private bool isActive;
    private Coroutine circleCoroutine;
    public bool IsUnlocked => playerConfig != null && playerConfig.protectorUnlocked;

    private void Awake()
    {
        abilityController = GetComponent<AbilityController>();
    }

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    private void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= playerConfig.protectorMaxActiveTime)
        {
            Deactivate();
            abilityController?.OnAbilityFinished();
        }
    }

    //===========================================
    // IAbility IMPLEMENTATION
    //===========================================

    public void Activate()
    {
        june = PlayerManager.Instance.June;
        isActive = true;
        timer = 0f;
        UIManager.Instance?.ProtectorActivate();
        circleCoroutine = StartCoroutine(CircleAndProtect());
        Debug.Log("Protector: ACTIVE - circling and adding light");
    }

    public void Deactivate()
    {
        isActive = false;
        UIManager.Instance?.ProtectorOnCooldown();

        if (circleCoroutine != null)
        {
            StopCoroutine(circleCoroutine);
            circleCoroutine = null;
        }

        if (june != null && june.IsPerformingAbility)
        {
            StartCoroutine(june.ReturnJuneHome());
        }

        Debug.Log("Protector: DEACTIVATED");
    }

    //===========================================
    // PROTECTOR LOGIC
    //===========================================

    private IEnumerator CircleAndProtect()
    {
        june.StartAbilityControl();

        Vector3 startPos = circleCenter + new Vector3(circleRadius, 0f, 0f);
        float moveToStartDuration = CalculateFlightDuration(june.transform.position, startPos);
        yield return june.MoveJuneToPosition(startPos, moveToStartDuration);

        float angle = 0f;
        while (isActive)
        {
            angle += circleSpeed * Time.deltaTime;

            float x = circleCenter.x + circleRadius * Mathf.Cos(angle);
            float y = circleCenter.y + circleRadius * Mathf.Sin(angle);
            june.transform.position = new Vector3(x, y, 0f);

            EventManager.Instance?.TriggerProtectorLightAddition();

            yield return null;
        }
    }

    private float CalculateFlightDuration(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to) / playerConfig.juneMoveSpeed;
    }
}
