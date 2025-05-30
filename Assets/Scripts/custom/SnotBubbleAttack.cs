using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class SnotBubbleAttack : Action
{
    [Header("发射设置")]
    public GameObject bubblePrefab;
    public Transform mouthPosition; // 嘴巴发射点
    public float spawnInterval = 0.2f; // 发射间隔
    public float attackDuration = 5f; // 攻击持续时间

    [Header("追踪参数")]
    public float playerTrackingAngle = 30f; // 偏向玩家的角度范围
    public float minSpeed = 1f;
    public float maxSpeed = 2f;

    [Header("摇晃参数")]
    public float wobbleIntensity = 0.5f;
    public float wobbleFrequency = 2f;

    private float timer;
    private float attackTimer;
    private Transform player;

    public override void OnStart()
    {
        timer = 0f;
        attackTimer = 0f;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 确保对象池初始化
        if (!ObjectPool.Instance.HasPool(bubblePrefab.name))
        {
            PrewarmPool();
        }
    }

    private void PrewarmPool()
    {
        for (int i = 0; i < 20; i++)
        {
            var bubble = GameObject.Instantiate(bubblePrefab);
            bubble.name = bubblePrefab.name;
            ObjectPool.Instance.PushObject(bubble);
        }
    }

    public override TaskStatus OnUpdate()
    {
        attackTimer += Time.deltaTime;
        timer += Time.deltaTime;

        // 攻击持续时间结束
        if (attackTimer >= attackDuration)
        {
            return TaskStatus.Running;
        }

        // 定时发射泡泡
        if (timer >= spawnInterval)
        {
            SpawnBubble();
            timer = 0f;
        }

        return TaskStatus.Running;
    }

    private void SpawnBubble()
    {
        GameObject bubbleObj = ObjectPool.Instance.GetObject(bubblePrefab);
        Bubble bubble = bubbleObj.GetComponent<Bubble>();
        if (bubble == null)
        {
            bubble = bubbleObj.AddComponent<Bubble>();
        }

        // 计算偏向玩家的方向
        Vector2 toPlayer = (player.position - mouthPosition.position).normalized;
        float angle = Random.Range(-playerTrackingAngle, playerTrackingAngle);
        Vector2 direction = Quaternion.Euler(0, 0, angle) * toPlayer;

        // 设置泡泡参数
        bubble.transform.position = mouthPosition.position;
        bubble.Setup(
            direction * Random.Range(minSpeed, maxSpeed),
            wobbleIntensity,
            wobbleFrequency
        );
    }
}