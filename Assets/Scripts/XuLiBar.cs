using UnityEngine;
using UnityEngine.UI;

public class XuLiBar : MonoBehaviour
{
    public Slider slider;
    public Color start = new Color(0, 1, 0, 1); // ��ʽ����Alpha
    public Color end = new Color(1, 0, 0, 1);   // RGBA(255,0,0,255)
    public Image fillImage;

    void Start()
    {
        // ȷ��Slider��Χ��ȷ
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0;

        // ��ʼ����ɫ
        fillImage.color = start;
    }

    void Update()
    {
        ContinueChangingcolor();
        DebugVisualCheck();
    }

    public void ContinueChangingcolor()
    {
        // ��ӷ�Χ����
        float clampedValue = Mathf.Clamp01(slider.value);
        fillImage.color = Color.Lerp(start, end, clampedValue);
    }

    void DebugVisualCheck()
    {
        // �ڳ�����ͼ�л��Ƶ��Է���
        Debug.DrawRay(fillImage.rectTransform.position,
                     Vector3.right * 50,
                     fillImage.color);
    }
}