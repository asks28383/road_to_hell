using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Vector2 baseDirection;
    private float speed;
    private float wobbleIntensity;
    private float wobbleFrequency;
    private float spawnTime;
    private float lifetime = 6f;

    private Vector2 currentPosition;
    private float wobbleOffset;

    public void Setup(Vector2 direction, float intensity, float frequency)
    {
        this.baseDirection = direction.normalized;
        this.speed = direction.magnitude;
        this.wobbleIntensity = intensity;
        this.wobbleFrequency = frequency;
        this.spawnTime = Time.time;
        this.wobbleOffset = Random.Range(0f, 10f); // 随机相位偏移

        currentPosition = transform.position;
        gameObject.SetActive(true);
    }

    void Update()
    {
        // 计算摇晃偏移
        float wobble = Mathf.PerlinNoise(
            (Time.time + wobbleOffset) * wobbleFrequency,
            0
        ) * 2f - 1f; // 转换为-1到1范围

        Vector2 wobbleVector = Vector2.Perpendicular(baseDirection) * wobble * wobbleIntensity;

        // 组合基础方向和摇晃方向
        Vector2 combinedDirection = (baseDirection + wobbleVector).normalized;

        // 更新位置
        currentPosition += combinedDirection * speed * Time.deltaTime;
        transform.position = currentPosition;

        // 生命周期检测
        if (Time.time - spawnTime >= lifetime)
        {
            ReturnToPool();
        }
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        other.GetComponent<PlayerHealth>().TakeDamage(1);
    //        ReturnToPool();
    //    }
    //}

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        ObjectPool.Instance.PushObject(gameObject);
    }
}