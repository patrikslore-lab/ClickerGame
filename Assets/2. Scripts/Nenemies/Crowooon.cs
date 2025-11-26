using UnityEngine;

public class Crowooon : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator crowooonAnim; //calls animator component directly

    bool isCasting = false;
    
    protected override void Start()
    {
        crowooonAnim = GetComponent<Animator>();
        
        if (crowooonAnim == null)
        {
            Debug.LogWarning("BringerOfDeathAnimator not found!");
        }
        
        base.Start();
    }
    
    private void PlayIdleAnimation()
    {
        if (crowooonAnim != null)
        {
            crowooonAnim.SetBool("isSpawning", false);
        }
    }
    
    protected override void PlayDeathAnimation()
    {
        if (crowooonAnim != null)
        {
            crowooonAnim.SetBool("isDying", true);
        }
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
        if (crowooonAnim != null && !isCasting && !isDead)
        {
            AnimatorStateInfo stateInfo = crowooonAnim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("CastLoop"))
            {
                isCasting = true;
                Debug.Log("CastPrep animation detected - starting light reduction");
            }
        }

        // While casting, continuously reduce light
        if (crowooonAnim != null && isCasting && !isDead)
        {
            EventManager.Instance.LightBeingDestroyed(this);
        }
    }

    protected override void Destroy()
    {
        Destroy(gameObject);
    }

}
