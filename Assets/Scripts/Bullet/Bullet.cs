using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 公有变量（可在Unity编辑器中配置）
    [SerializeField] private int damage = 10; // 武器伤害值
    public float speed;              // 子弹飞行速度
    public GameObject explosionPrefab; // 爆炸效果预制体
    public float lifetime = 4f;      // 子弹存活时间（秒）
    public BulletOwner owner;        // 子弹所有者类型

    private float _timer;            // 生命周期计时器
    private Rigidbody2D rigidbody;   // 子弹的刚体组件

    // 子弹所有者枚举
    public enum BulletOwner
    {
        Player,
        Boss
    }

    // 初始化时获取组件
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 设置子弹飞行方向
    /// </summary>
    /// <param name="direction">标准化后的飞行方向向量</param>
    public void SetSpeed(Vector2 direction)
    {
        rigidbody.velocity = direction * speed;
    }

    void OnEnable()
    {
        _timer = 0f;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= lifetime)
        {
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    /// <summary>
    /// 触发器碰撞检测
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 忽略与同阵营子弹的碰撞
        if (other.CompareTag("Bullet")|| other.CompareTag("Bullet2")) return;

        // 根据子弹所有者决定碰撞逻辑
        switch (owner)
        {
            case BulletOwner.Player:
                HandlePlayerBulletCollision(other);
                break;

            case BulletOwner.Boss:
                HandleBossBulletCollision(other);
                break;
        }
    }

    /// <summary>
    /// 处理玩家子弹的碰撞
    /// </summary>
    private void HandlePlayerBulletCollision(Collider2D other)
    {
        // 只对Boss造成伤害
        if (!other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Debug.Log("hello");
            SpawnExplosion();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    /// <summary>
    /// 处理Boss子弹的碰撞
    /// </summary>
    private void HandleBossBulletCollision(Collider2D other)
    {
        // 只对玩家造成伤害
        if (!other.CompareTag("Boss"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            SpawnExplosion();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    /// <summary>
    /// 生成爆炸效果
    /// </summary>
    private void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
            exp.transform.position = transform.position;
            exp.transform.rotation = Quaternion.identity;
        }
    }
}