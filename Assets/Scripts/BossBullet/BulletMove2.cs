using UnityEngine;

public class BulletMove2 : MonoBehaviour
{
    public float BulletSpeed;
    public float maxAmplitude = 1f;    // 最大波动幅度
    public float minAmplitude = 0.2f;  // 最小波动幅度
    public float maxFrequency = 3f;    // 最大波动频率
    public float minFrequency = 1f;    // 最小波动频率

    private float amplitude;
    private float frequency;
    private float timeElapsed;

    private void Start()
    {
        // 随机生成波动参数
        amplitude = Random.Range(minAmplitude, maxAmplitude);
        frequency = Random.Range(minFrequency, maxFrequency);
    }

    private void FixedUpdate()
    {
        timeElapsed += Time.fixedDeltaTime;

        Vector3 forward = transform.up * BulletSpeed * Time.fixedDeltaTime;
        Vector3 side = transform.right * Mathf.Sin(timeElapsed * frequency) * amplitude * Time.fixedDeltaTime;

        transform.position += forward + side;
    }
}