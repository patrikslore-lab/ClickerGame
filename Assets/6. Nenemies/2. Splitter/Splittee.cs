using UnityEngine;

public class Splittee : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }
    //override below to not include any delay after dying = can remove later 
    // (if we're having 1s death anims as standard)
    protected override void PlayDeathAnimation()
    {
        Destroy(gameObject);
    }
}
