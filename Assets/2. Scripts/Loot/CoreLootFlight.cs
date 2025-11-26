using UnityEngine;
using System.Collections;

public class CoreLootFlight : MonoBehaviour
{
    [SerializeField] private float flyDuration = 1.5f;
    [SerializeField] private float accelerationMultiplier = 2f;

    private Vector3 targetPosition = new Vector3(0, -9, 0);

    /// <summary>
    /// Start the flying animation towards the UI at bottom center
    /// </summary>
    public void FlyToUI()
    {
        Debug.Log("CoreLootFlight: FlyToUI called");
        StartCoroutine(FlyCoroutine());
    }

    private IEnumerator FlyCoroutine()
    {
        Debug.Log("CoreLootFlight: Flying from " + transform.position + " to " + targetPosition);
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < flyDuration)
        {
            elapsedTime += Time.deltaTime;

            // Use acceleration curve: easeInQuad-like behavior for speedup
            float t = elapsedTime / flyDuration;
            float acceleratedT = t * t * accelerationMultiplier;

            // Clamp to prevent overshoot
            acceleratedT = Mathf.Min(acceleratedT, 1f);

            transform.position = Vector3.Lerp(startPosition, targetPosition, acceleratedT);

            yield return null;
        }

        transform.position = targetPosition;
        Debug.Log("CoreLootFlight: Reached target position");

        // Notify that core has been collected
        Loot loot = GetComponent<Loot>();
        if (loot != null)
        {
            Debug.Log("CoreLootFlight: Found Loot component, calling LootManager.Collect");
            LootManager.Instance.Collect(loot);
        }
        else
        {
            Debug.LogError("CoreLootFlight: No Loot component found!");
            Destroy(gameObject);
        }
    }
}
