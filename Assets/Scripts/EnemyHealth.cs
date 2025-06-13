using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Events; // 用于事件系统

[TaskCategory("Custom")]
public class EnemyHealth : Health
{
    [Header("受伤特效")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private Color flashColor = Color.red;

    [Header("Boss死亡设置")]
    [SerializeField] private bool isBoss = false; // 标记是否为Boss
    [SerializeField] private GameObject victoryUI; // 胜利UI预制体或场景中的对象
    [SerializeField] private float showUIDelay = 2f; // 死亡后显示UI的延迟时间
    [SerializeField] private UnityEvent onBossDeath; // 可扩展的事件系统

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // 如果没指定UI，尝试在场景中查找
        if (victoryUI == null && isBoss)
        {
            victoryUI = GameObject.FindGameObjectWithTag("VictoryUI");
        }
    }

    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        FlashRed();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected override void PlayHurtAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        FlashRed();
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
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        isFlashing = false;
    }

    protected override void PlayDeathAnimation()
    {
        //if (animator != null)
        //{
        //    animator.SetTrigger("Death");
        //}

        // 如果是Boss，触发胜利UI
        if (isBoss)
        {
            StartCoroutine(ShowVictoryUI());
        }
    }

    private IEnumerator ShowVictoryUI()
    {
        // 等待死亡动画播放一段时间
        yield return new WaitForSeconds(showUIDelay);

        // 触发事件
        onBossDeath.Invoke();

        // 显示UI
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);

            // 可选：暂停游戏
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Victory UI未指定！");
        }
    }

    public void Recover(float recovery)
    {
        currentHealth = Mathf.Min(currentHealth + recovery, maxHealth);
    }
}