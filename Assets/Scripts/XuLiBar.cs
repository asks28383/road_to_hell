using UnityEngine;
using UnityEngine.UI;

public class XuLiBar : MonoBehaviour
{
    public Slider slider;
    public Color start = new Color(0, 1, 0, 1); // 显式设置Alpha
    public Color end = new Color(1, 0, 0, 1);   // RGBA(255,0,0,255)
    public Image fillImage;

    void Start()
    {
        // 确保Slider范围正确
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0;

        // 初始化颜色
        fillImage.color = start;
    }

    void Update()
    {
        ContinueChangingcolor();
        DebugVisualCheck();
    }

    public void ContinueChangingcolor()
    {
        // 添加范围保护
        float clampedValue = Mathf.Clamp01(slider.value);
        fillImage.color = Color.Lerp(start, end, clampedValue);
    }

    void DebugVisualCheck()
    {
        // 在场景视图中绘制调试方块
        Debug.DrawRay(fillImage.rectTransform.position,
                     Vector3.right * 50,
                     fillImage.color);
    }
}