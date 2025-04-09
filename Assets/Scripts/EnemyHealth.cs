using UnityEngine;

public class EnemyHealth : Health
{
    protected override void PlayHurtAnimation()
    {
        animator.SetTrigger("Hurt");
    }

    protected override void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }
}