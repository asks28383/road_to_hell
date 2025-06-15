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
    [SerializeField] private GameObject gameOverUI; // ʧ��UI����
    [SerializeField] private float showUIDelay = 1.5f; // ��ʾUI���ӳ�ʱ��
    [SerializeField] private UnityEvent onPlayerDeath; // ��������¼�

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

        // ��ʼ��ʱ����UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        //else
        //{
        //    // �����Զ�����
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
        // ������ PlayerHealth.cs ��
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

        //// ������ҿ���
        //GetComponent<PlayerMovement>().enabled = false;
        //GetComponent<PlayerAttack>().enabled = false;
    }

    private IEnumerator ShowGameOverUI()
    {
        // �ȴ�һ��ʱ����������������
        yield return new WaitForSeconds(showUIDelay);

        // ���������¼�
        onPlayerDeath.Invoke();

        // ��ʾUI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);

            Destroy(gameObject);
            // ��ͣ��Ϸ����ѡ��
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Game Over UIδָ����");
        }
    }
    void OnDestroy()
    {
        Time.timeScale = 1f; // ȷ���л�������ʱ��ָ�����
    }
}