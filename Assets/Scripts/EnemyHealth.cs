using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class EnemyHealth : Health
{
    [SerializeField] private float flashDuration = 0.1f; // Duration of each flash
    [SerializeField] private int flashCount = 3; // Number of times to flash
    [SerializeField] private Color flashColor = Color.red; // Color to flash

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Store original color
        }
    }

    protected override void PlayHurtAnimation()
    {
        animator.SetTrigger("Hurt");
        FlashRed();
    }

    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        FlashRed(); // Flash when taking damage

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void FlashRed()
    {
        if (spriteRenderer == null || isFlashing) return;

        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        isFlashing = true;

        for (int i = 0; i < flashCount; i++)
        {
            // Turn red
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            // Revert to original color
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        isFlashing = false;
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