using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public void ApplyMovement(EnemyConfig enemyConfig, RoomConfig roomConfig)
    {
        switch (enemyConfig.movementType)
        {
            case EnemyMovementType.Static:
                // Do nothing - enemy stays in place
                break;

            case EnemyMovementType.FloatUpDown:
                StartCoroutine(MoveUpDown(enemyConfig, roomConfig));
                break;

            case EnemyMovementType.CentreResponsive:
                StartCoroutine(CentreResponsive(enemyConfig, roomConfig));
                break;
        }
    }

    private IEnumerator MoveUpDown(EnemyConfig enemyConfig, RoomConfig roomConfig)
    {
        Vector3 startPosition = transform.position;
        while (true)
        {
            float t = Mathf.PingPong(Time.time / enemyConfig.floatDuration, 1);
            float newY = Mathf.Lerp(-enemyConfig.floatHeight, enemyConfig.floatHeight, t);

            // Create the new position
            Vector3 newPosition = startPosition + new Vector3(0, newY, 0);

            // Clamp it to stay within bounds
            newPosition.y = Mathf.Clamp(newPosition.y, roomConfig.MinY, roomConfig.MaxY);

            transform.position = newPosition;
            yield return null;
        }
    }
    private IEnumerator CentreResponsive(EnemyConfig enemyConfig, RoomConfig roomConfig)
    {
        while (true)
        {

        }
    }
}