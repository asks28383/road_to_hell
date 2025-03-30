using UnityEngine;
using UnityEngine.UI;

public class XuLiBar : MonoBehaviour
{
    public Transform target;         // 要跟随的角色
    public Vector3 offset = new Vector3(0, 2f, 0); // 头顶偏移量
    [Header("Charge Settings")]
    [SerializeField] private float maxChargeDuration = 2f;  // 最大蓄力时间
    [SerializeField] private Gradient colorGradient;        // 颜色渐变配置
    [SerializeField] private AnimationCurve fillCurve;      // 填充曲线控制


    [Header("References")]
    [SerializeField] private Image fillImage;               // 圆形填充图像

    private float _currentCharge;      // 当前蓄力值（0-1）

    void Update()
    {
        // 跟随角色
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Camera.main.transform.rotation; // 始终面向相机
        }
    }
    /// <summary>
    /// 外部调用更新蓄力状态
    /// </summary>
    /// <param name="chargeTime">当前蓄力时间（单位：秒）</param>
    public void UpdateCharge(float chargeTime)
    {
        // 计算标准化进度（0-1）
        _currentCharge = Mathf.Clamp01(chargeTime / maxChargeDuration);

        // 更新视觉表现
        UpdateFillAmount();
        UpdateColor();
    }

    /// <summary>
    /// 重置蓄力状态
    /// </summary>
    public void ResetCharge()
    {
        _currentCharge = 0f;
        UpdateFillAmount();
        UpdateColor();
    }

    private void UpdateFillAmount()
    {
        // 应用曲线控制填充进度
        float curvedProgress = fillCurve.Evaluate(_currentCharge);
        fillImage.fillAmount = curvedProgress;

    }

    private void UpdateColor()
    {
        // 渐变颜色控制
        Color targetColor = colorGradient.Evaluate(_currentCharge);
        fillImage.color = targetColor;
    }
    public void Active(bool active)
    {
        gameObject.SetActive(active);
    }
}