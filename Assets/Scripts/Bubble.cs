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
        this.wobbleOffset = Random.Range(0f, 10f); // �����λƫ��

        currentPosition = transform.position;
        gameObject.SetActive(true);
    }

    void Update()
    {
        // ����ҡ��ƫ��
        float wobble = Mathf.PerlinNoise(
            (Time.time + wobbleOffset) * wobbleFrequency,
            0
        ) * 2f - 1f; // ת��Ϊ-1��1��Χ

        Vector2 wobbleVector = Vector2.Perpendicular(baseDirection) * wobble * wobbleIntensity;

        // ��ϻ��������ҡ�η���
        Vector2 combinedDirection = (baseDirection + wobbleVector).normalized;

        // ����λ��
        currentPosition += combinedDirection * speed * Time.deltaTime;
        transform.position = currentPosition;

        // �������ڼ��
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