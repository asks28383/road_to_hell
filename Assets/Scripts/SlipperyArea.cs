using UnityEngine;

public class SlipperyArea : MonoBehaviour
{
    public float slowFactor = 0.5f; // ���ٱ��� (0.5 = 50%�ٶ�)
    public float duration = 5f;

    [Header("Damage Settings")]
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private int damagePerTick = 4;

    private float timer;
    private static bool isPlayerInSoda;
    private static float lastDamageTime;
    private static MovementController affectedMovement; // ��̬��¼��Ӱ����ƶ�������

    void Start()
    {
        timer = duration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // ����Լ��ǵ�ǰӰ����ҵ����������״̬
            if (affectedMovement != null && isPlayerInSoda)
            {
                CheckAndResetMovement();
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPlayerInSoda)
        {
            isPlayerInSoda = true;
            affectedMovement = other.GetComponent<MovementController>();
            if (affectedMovement != null)
            {
                affectedMovement.ApplySpeedModifier(slowFactor);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlayerInSoda)
        {
            if (Time.time - lastDamageTime >= damageInterval)
            {
                var health = other.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damagePerTick);
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlayerInSoda)
        {
            CheckAndResetMovement();
        }
    }

    // ��ȫ�ļ�鲢�����ƶ�״̬
    private void CheckAndResetMovement()
    {
        // ȷ�������Ĳ����κ�ճҺ������
        if (!IsPlayerInAnySodaArea(affectedMovement.transform.position))
        {
            isPlayerInSoda = false;
            if (affectedMovement != null)
            {
                affectedMovement.RemoveSpeedModifier(slowFactor);
                affectedMovement = null; // �������
            }
        }
    }

    private bool IsPlayerInAnySodaArea(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.2f);
        foreach (var col in colliders)
        {
            if (col != this.GetComponent<Collider2D>() &&
                col.GetComponent<SlipperyArea>() != null)
            {
                return true;
            }
        }
        return false;
    }

    void OnDestroy()
    {
        // ��ֹ��������ʱ״̬δ���
        if (isPlayerInSoda && affectedMovement != null)
        {
            CheckAndResetMovement();
        }
    }
}