using UnityEngine;

public class DoorAnimationHelper : MonoBehaviour
{
    private void DoorAnimationFinished()
    {
        LevelManager.Instance.DoorAnimationFinished();
    }
}
