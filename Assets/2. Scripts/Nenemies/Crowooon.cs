using UnityEngine;

public class Crowooon : Enemy
{
    private Animator crowooonAnim;
    private bool isCasting = false;
    
    protected override void Start()
    {
        crowooonAnim = GetComponent<Animator>();
        
        if (crowooonAnim == null)
        {
            Debug.LogWarning("Crowooon: Animator not found!");
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

    public void TriggerSpecialAttack()
    {
        Debug.Log("Crowooon special attack!");
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
                Debug.Log("CastLoop animation detected - starting light reduction");
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