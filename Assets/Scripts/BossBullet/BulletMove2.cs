using UnityEngine;

public class BulletMove2 : MonoBehaviour
{
    public float BulletSpeed;
    public float maxAmplitude = 1f;    // ��󲨶�����
    public float minAmplitude = 0.2f;  // ��С��������
    public float maxFrequency = 3f;    // ��󲨶�Ƶ��
    public float minFrequency = 1f;    // ��С����Ƶ��

    private float amplitude;
    private float frequency;
    private float timeElapsed;

    private void Start()
    {
        // ������ɲ�������
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