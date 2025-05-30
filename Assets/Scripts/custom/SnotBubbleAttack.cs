using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class SnotBubbleAttack : Action
{
    [Header("��������")]
    public GameObject bubblePrefab;
    public Transform mouthPosition; // ��ͷ����
    public float spawnInterval = 0.2f; // ������
    public float attackDuration = 5f; // ��������ʱ��

    [Header("׷�ٲ���")]
    public float playerTrackingAngle = 30f; // ƫ����ҵĽǶȷ�Χ
    public float minSpeed = 1f;
    public float maxSpeed = 2f;

    [Header("ҡ�β���")]
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

        // ȷ������س�ʼ��
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

        // ��������ʱ�����
        if (attackTimer >= attackDuration)
        {
            return TaskStatus.Running;
        }

        // ��ʱ��������
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

        // ����ƫ����ҵķ���
        Vector2 toPlayer = (player.position - mouthPosition.position).normalized;
        float angle = Random.Range(-playerTrackingAngle, playerTrackingAngle);
        Vector2 direction = Quaternion.Euler(0, 0, angle) * toPlayer;

        // �������ݲ���
        bubble.transform.position = mouthPosition.position;
        bubble.Setup(
            direction * Random.Range(minSpeed, maxSpeed),
            wobbleIntensity,
            wobbleFrequency
        );
    }
}