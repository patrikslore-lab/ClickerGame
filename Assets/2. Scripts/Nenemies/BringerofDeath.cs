using Unity.VisualScripting;
using UnityEngine;

public class BringerOfDeathEnemy : Enemy
{
    private Animator boDAnimator; //calls animator component directly

    bool isCasting = false;
    
    protected override void Start()
    {
        boDAnimator = GetComponent<Animator>();
        
        if (boDAnimator == null)
        {
            Debug.LogWarning("BringerOfDeathAnimator not found!");
        }
        
        base.Start();
    }
    
    protected override void PlaySpawnAnimation()
    {

    }
    
    protected override void PlayDeathAnimation()
    {
        if (boDAnimator != null)
        {
            boDAnimator.SetBool("isDying", true);
        }
        // Destroy after animation plays (1 second)
        Destroy(gameObject, 1f);
    }

    // Example: Add boss-specific behavior
    public void TriggerSpecialAttack()
    {
        Debug.Log("BringerOfDeath special attack!");
        // Custom boss logic here
    }

    void Update()
    {
        // Check if the CastPrep animation is playing
        if (boDAnimator != null && !isCasting && !isDead)
        {
            AnimatorStateInfo stateInfo = boDAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("CastLoop"))
            {
                isCasting = true;
                Debug.Log("CastPrep animation detected - starting light reduction");
            }
        }

        // While casting, continuously reduce light
        if (boDAnimator != null && isCasting && !isDead)
        {
            EventManager.Instance.LightBeingDestroyed(this);
        }
    }
}