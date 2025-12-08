using UnityEngine;
public class BaseAbility : MonoBehaviour
{
    public bool isUnlocked;
    public bool AbilityUnlockedCheck()
    {
        if (!isUnlocked)
        {
            return true;
        }
        else return false;
    }

    public void UnlockAbility()
    {
        isUnlocked = true;
    }
}