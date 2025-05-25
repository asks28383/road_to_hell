using UnityEngine;

public class SlipperyArea : MonoBehaviour
{
    public float slowFactor = 0.5f; // 减速比例 (0.5 = 50%速度)
    public float duration = 5f;

    [Header("Damage Settings")]
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private int damagePerTick = 4;

    private float timer;
    private static bool isPlayerInSoda;
    private static float lastDamageTime;
    private static MovementController affectedMovement; // 静态记录受影响的移动控制器

    void Start()
    {
        timer = duration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // 如果自己是当前影响玩家的区域，则清除状态
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

    // 安全的检查并重置移动状态
    private void CheckAndResetMovement()
    {
        // 确认玩家真的不在任何粘液区域了
        if (!IsPlayerInAnySodaArea(affectedMovement.transform.position))
        {
            isPlayerInSoda = false;
            if (affectedMovement != null)
            {
                affectedMovement.RemoveSpeedModifier(slowFactor);
                affectedMovement = null; // 清除引用
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
        // 防止对象销毁时状态未清除
        if (isPlayerInSoda && affectedMovement != null)
        {
            CheckAndResetMovement();
        }
    }
}