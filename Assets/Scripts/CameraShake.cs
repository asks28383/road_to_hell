using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public float shakeDuration = 0.2f;
    public float shakeAmplitude = 2f;
    public float shakeFrequency = 3f;

    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeCoroutine;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        // ��ȡ��������ϵ�����ģ��
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(DoShake());
    }

    private IEnumerator DoShake()
    {
        noise.m_AmplitudeGain = shakeAmplitude;
        noise.m_FrequencyGain = shakeFrequency;

        yield return new WaitForSeconds(shakeDuration);

        noise.m_AmplitudeGain = 0f;
        shakeCoroutine = null;
    }
}
