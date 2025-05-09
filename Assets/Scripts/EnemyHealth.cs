using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
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
    public void Recover(float recovery)
    {
        currentHealth = Mathf.Min(currentHealth + recovery, maxHealth);
    }
}