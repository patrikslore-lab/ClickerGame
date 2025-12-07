using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private GameObject doorPrefab;
    private GameObject doorInstance;
    private Animator doorAnimator;

    void OnEnable()
    {
        EventManager.Instance.doorBreak1 += DoorBreak1;
        EventManager.Instance.doorBreak2 += DoorBreak2;
        EventManager.Instance.doorBreak3 += DoorBreak3;
    }

    void OnDisable()
    {
        EventManager.Instance.doorBreak1 -= DoorBreak1;
        EventManager.Instance.doorBreak2 -= DoorBreak2;
        EventManager.Instance.doorBreak3 -= DoorBreak3;
    }

    public void InstantiateDoor()
    {
        doorPrefab = LevelManager.Instance.CurrentRoomConfig.door;
        doorInstance = Instantiate(doorPrefab, new Vector2(0, 8.2f), Quaternion.identity);
        doorAnimator = doorInstance.GetComponent<Animator>();
    }

    public void DoorBreak1()
    {
        doorAnimator.SetBool("break1", true);
    }
    public void DoorBreak2()
    {
        doorAnimator.SetBool("break2", true);
    }
    public void DoorBreak3()
    {
        doorAnimator.SetBool("break3", true);
    }

    // Called by Animation Event at the end of door break animation
    public void OnDoorBreakAnimationComplete()
    {
        Debug.Log("Door break animation complete - triggering level completion");

        // Transition to level complete state
        LevelManager.Instance.TransitionToLevelComplete();
    }
}
