using UnityEngine;
using System.Collections;

public class Protector : BaseAbility
{
    private enum ProtectorState
    {
        Inactive,
        Active,
        OnCooldown
    }
    private ProtectorState currentState = ProtectorState.Inactive;
    private float activeTimer = 0f;

    public bool IsActive => currentState == ProtectorState.Active;

    // Circular motion variables
    private float currentAngle = 0f;
    private Vector3 screenCenter = Vector3.zero;

    // Circular motion configuration (living in script)
    private readonly float circleRadius = 2f;
    private readonly float circleSpeed = 3f; // radians per second
    private readonly KeyCode activationKey = KeyCode.W;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        // Input handling
        if (Input.GetKeyDown(activationKey))
        {
            if (!CanUseAbility())
            {
                Debug.Log("Protector ability on cooldown!");
                return;
            }

            if (currentState == ProtectorState.Inactive)
            {
                ActivateProtector();
            }
            else if (currentState == ProtectorState.Active)
            {
                DeactivateProtector();
            }
        }

        // Handle auto-deactivation after max active time
        if (currentState == ProtectorState.Active)
        {
            activeTimer += Time.deltaTime;
            if (activeTimer >= playerConfig.protectorMaxActiveTime)
            {
                DeactivateProtector();
            }
        }
    }

    private void ActivateProtector()
    {
        currentState = ProtectorState.Active;
        activeTimer = 0f;
        currentAngle = 0f; // Start at angle 0 (right side of circle)
        UIManager.Instance.ProtectorActivate();
        Debug.Log("Protector ability: ACTIVE");

        StartCoroutine(PerformProtectorSequence());
    }

    private void DeactivateProtector()
    {
        currentState = ProtectorState.Inactive;
        base.StartCooldown();
        UIManager.Instance.ProtectorOnCooldown();
        Debug.Log("Protector deactivated - COOLDOWN started");
    }

    private IEnumerator PerformProtectorSequence()
    {
        // Take control of June
        juneCharacter.StartAbilityControl();

        // Move to starting position on circle (angle 0, right side)
        Vector3 startPos = screenCenter + new Vector3(circleRadius, 0f, 0f);
        float moveToStartDuration = Vector3.Distance(
            juneCharacter.JuneInstance.transform.position,
            startPos
        ) / playerConfig.juneMoveSpeed;
        yield return juneCharacter.MoveJuneToPosition(startPos, moveToStartDuration);

        // Circle continuously while active
        while (currentState == ProtectorState.Active)
        {
            // Update circular position
            currentAngle += circleSpeed * Time.deltaTime;

            float x = screenCenter.x + circleRadius * Mathf.Cos(currentAngle);
            float y = screenCenter.y + circleRadius * Mathf.Sin(currentAngle);
            juneCharacter.JuneInstance.transform.position = new Vector3(x, y, 0f);

            // Trigger light addition event per frame
            EventManager.Instance.TriggerProtectorLightAddition();

            yield return null; // Wait one frame
        }

        // Return home (also releases control back to JuneCharacter)
        yield return juneCharacter.ReturnJuneHome();
    }
}
