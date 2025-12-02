using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator doorAnimator;
    private Sprite doorSprite;

    [SerializeField] Sprite openDoor;
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

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
        // First set the game state
        GameManager.Instance.SetGameState(GameManager.GameState.LevelComplete);
        // Then show the panel (this needs to happen after state change)
        UIManager.Instance.LevelCompletionRoutine();

        doorSprite = openDoor;
    }
}
