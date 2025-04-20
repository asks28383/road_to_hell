using UnityEngine;

public class EnemyHealth : Health
{
    protected override void PlayHurtAnimation()
    {
        animator.SetTrigger("Hurt");
    }
    public override void TakeDamage(int damage)
    {

        currentHealth -= damage;

        PlayHurtAnimation();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    protected override void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }
}