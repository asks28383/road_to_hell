using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bullet : MonoBehaviour
{
    // 公有变量（可在Unity编辑器中配置）
    [SerializeField] private int damage = 10; // 武器伤害值
    public float speed;              // 子弹飞行速度
    public GameObject explosionPrefab; // 爆炸效果预制体
    public float lifetime = 4f;      // 子弹存活时间（秒）

    private float _timer;            // 生命周期计时器
    new private Rigidbody2D rigidbody; // 子弹的刚体组件（new关键字用于隐藏父级同名成员）

    // 初始化时获取组件
    void Awake()
    {
        // 获取当前游戏对象上的Rigidbody2D组件
        rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 设置子弹飞行方向
    /// </summary>
    /// <param name="direction">标准化后的飞行方向向量</param>
    public void SetSpeed(Vector2 direction)
    {
        // 通过刚体设置子弹速度（方向 * 速度值）
        rigidbody.velocity = direction * speed;
    }

    void OnEnable()
    {
        // 每次从对象池取出时重置计时器
        _timer = 0f;
    }
    void Update()
    {
        // 可在此处添加子弹生命周期计时器等逻辑
        _timer += Time.deltaTime;
        // 检查是否超过生命周期
        if (_timer >= lifetime)
        {
            // 使用对象池回收子弹对象
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    /// <summary>
    /// 触发器碰撞检测（当子弹碰到其他碰撞体时执行）
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查击中的对象是否有Health组件
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        // 使用对象池获取爆炸效果实例（替代Instantiate）
        GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
        // 设置爆炸效果位置为当前子弹位置
        exp.transform.position = transform.position;
        // 需要确保爆炸效果对象在初始化时自动激活

        // 使用对象池回收子弹对象（替代Destroy）
        ObjectPool.Instance.PushObject(gameObject);
    }
}
