using System.Collections;
using UnityEngine;

public class PlayerHealth : Health
{
    [Header("Damage Settings")]
    [SerializeField] private float damageCooldown = 0.8f; // Time between allowed damage
    [SerializeField] private float flashDuration = 0.2f; // Duration of each flash
    [SerializeField] private int flashCount = 2; // How many times to flash
    [SerializeField] private Color flashColor = Color.red; // Color to flash when hit

    private bool canTakeDamage = true;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public override void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log(currentHealth);
        PlayHurtAnimation();
        StartCoroutine(FlashEffect());
        StartCoroutine(DamageCooldown());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    private IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    protected override void Die()
    {
        base.Die();
        // Additional player death logic if needed
    }
}