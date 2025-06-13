using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Events; // �����¼�ϵͳ

[TaskCategory("Custom")]
public class EnemyHealth : Health
{
    [Header("������Ч")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private Color flashColor = Color.red;

    [Header("Boss��������")]
    [SerializeField] private bool isBoss = false; // ����Ƿ�ΪBoss
    [SerializeField] private GameObject victoryUI; // ʤ��UIԤ����򳡾��еĶ���
    [SerializeField] private float showUIDelay = 2f; // ��������ʾUI���ӳ�ʱ��
    [SerializeField] private UnityEvent onBossDeath; // ����չ���¼�ϵͳ

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

        // ���ûָ��UI�������ڳ����в���
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

        // �����Boss������ʤ��UI
        if (isBoss)
        {
            StartCoroutine(ShowVictoryUI());
        }
    }

    private IEnumerator ShowVictoryUI()
    {
        // �ȴ�������������һ��ʱ��
        yield return new WaitForSeconds(showUIDelay);

        // �����¼�
        onBossDeath.Invoke();

        // ��ʾUI
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);

            // ��ѡ����ͣ��Ϸ
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Victory UIδָ����");
        }
    }

    public void Recover(float recovery)
    {
        currentHealth = Mathf.Min(currentHealth + recovery, maxHealth);
    }
}