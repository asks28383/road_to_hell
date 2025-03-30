using UnityEngine;
using UnityEngine.UI;

public class XuLiBar : MonoBehaviour
{
    [Header("Charge Settings")]
    [SerializeField] private float maxChargeDuration = 2f;  // �������ʱ��
    [SerializeField] private Gradient colorGradient;        // ��ɫ��������
    [SerializeField] private AnimationCurve fillCurve;      // ������߿���


    [Header("References")]
    [SerializeField] private Image fillImage;               // Բ�����ͼ��


    private float _currentCharge;      // ��ǰ����ֵ��0-1��


    /// <summary>
    /// �ⲿ���ø�������״̬
    /// </summary>
    /// <param name="chargeTime">��ǰ����ʱ�䣨��λ���룩</param>
    public void UpdateCharge(float chargeTime)
    {
        // �����׼�����ȣ�0-1��
        _currentCharge = Mathf.Clamp01(chargeTime / maxChargeDuration);

        // �����Ӿ�����
        UpdateFillAmount();
        UpdateColor();
    }

    /// <summary>
    /// ��������״̬
    /// </summary>
    public void ResetCharge()
    {
        _currentCharge = 0f;
        UpdateFillAmount();
        UpdateColor();
    }

    private void UpdateFillAmount()
    {
        // Ӧ�����߿���������
        float curvedProgress = fillCurve.Evaluate(_currentCharge);
        fillImage.fillAmount = curvedProgress;

    }

    private void UpdateColor()
    {
        // ������ɫ����
        Color targetColor = colorGradient.Evaluate(_currentCharge);
        fillImage.color = targetColor;
    }
    public void Active(bool active)
    {
        gameObject.SetActive(active);
    }
}