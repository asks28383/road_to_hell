using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : Health
{
    [Header("Damage Settings")]
    [SerializeField] private float damageCooldown = 0.4f;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private int flashCount = 2;
    [SerializeField] private Color flashColor = Color.red;

    [Header("Death Settings")]
    [SerializeField] private GameObject gameOverUI; // 失败UI对象
    [SerializeField] private float showUIDelay = 1.5f; // 显示UI的延迟时间
    [SerializeField] private UnityEvent onPlayerDeath; // 玩家死亡事件

    private bool canTakeDamage = true;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public CameraShake cameraShake;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // 初始化时隐藏UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        //else
        //{
        //    // 尝试自动查找
        //    gameOverUI = GameObject.FindGameObjectWithTag("GameOverUI");
        //}
    }

    public override void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        PlayHurtAnimation();
        StartCoroutine(FlashEffect());
        StartCoroutine(DamageCooldown());
        // 例如在 PlayerHealth.cs 中
        cameraShake.Shake();

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

    public void Recover(float recovery)
    {
        currentHealth = Mathf.Min(currentHealth + recovery, maxHealth);
    }

    protected override void Die()
    {
        Debug.Log("die");
        base.Die();
        StartCoroutine(ShowGameOverUI());

        //// 禁用玩家控制
        //GetComponent<PlayerMovement>().enabled = false;
        //GetComponent<PlayerAttack>().enabled = false;
    }

    private IEnumerator ShowGameOverUI()
    {
        // 等待一段时间让死亡动画播放
        yield return new WaitForSeconds(showUIDelay);

        // 触发死亡事件
        onPlayerDeath.Invoke();

        // 显示UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);

            Destroy(gameObject);
            // 暂停游戏（可选）
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Game Over UI未指定！");
        }
    }
    void OnDestroy()
    {
        Time.timeScale = 1f; // 确保切换场景后时间恢复正常
    }
}