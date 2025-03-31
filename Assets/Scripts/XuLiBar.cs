using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.ComponentModel;
public class XuLiBar : MonoBehaviour
{

    public static XuLiBar instance;
    public bool isfull;
    public Transform target;         // 要跟随的角色
    public Vector3 offset = new Vector3(0, 2f, 0); // 头顶偏移量
    [Header("Charge Settings")]
    [SerializeField] private float maxChargeDuration = 2f;  // 最大蓄力时间
    [SerializeField] private float minChargeDuration = .3f;  // 最小蓄力时间
    [SerializeField] private Gradient colorGradient;        // 颜色渐变配置
    [SerializeField] private AnimationCurve fillCurve;      // 填充曲线控制


    [Header("References")]
    [SerializeField] private Image fillImage;               // 圆形填充图像
    [SerializeField] private Image widerBar;
    public float _currentCharge;      // 当前蓄力值（0-1）

    private RectTransform inner;
    private RectTransform outer;
    private float gap;
    private float cx;
    private Color color;


    [Header("Animation")]
    [SerializeField] private Animator chargeAnimator; // 关联动画控制器
    private bool _hasTriggeredFullAnimation; // 标记是否已触发过满蓄力动画
    private float _previousCharge; // 记录上一帧的蓄力值



    [Header("Flash Settings")]
    [SerializeField] private float flashSpeed = 2f; // 闪烁频率
    [SerializeField] private float minAlpha = 0.3f; // 最小透明度
    [SerializeField] private float maxAlpha = 1f;   // 最大透明度
    [SerializeField] private Color flashColor1 = Color.yellow; // 新增：闪烁颜色1
    [SerializeField] private Color flashColor2 = Color.red;    // 新增：闪烁颜色2


    private Color _originalColor;    // 原始颜色（取自渐变色）
    public bool _isFlashing;        // 是否正在闪烁
    private Coroutine _flashCoroutine; // 闪烁协程引用


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {

        inner = fillImage.rectTransform;
        outer = widerBar.rectTransform;

        float x = inner.rect.width;
        cx = outer.rect.width;
        gap = cx - x;

        color = widerBar.color;

        _hasTriggeredFullAnimation = false;
        _previousCharge = 0f;

        _originalColor = colorGradient.Evaluate(1f); // 获取满蓄力时的颜色
    }
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
        if(chargeTime >= minChargeDuration) { 
            chargeTime -= minChargeDuration; 
        }
        else
        {
            return;
        }
        _currentCharge = Mathf.Clamp01(chargeTime / maxChargeDuration);




        _previousCharge = _currentCharge; // 记录当前帧蓄力值


        if (_currentCharge >= 1f && !_isFlashing)
        {
            StartFlashing();
            isfull = true;
        }
        else if (_currentCharge < 1f && _isFlashing)
        {
            StopFlashing();
            isfull = false;
        }

        // 更新视觉表现
        UpdateFillAmount();
        UpdateColor();
        updateWiderBar(_currentCharge*gap);
    }



    private void UpdateFillAmount()
    {
        // 应用曲线控制填充进度
        float curvedProgress = fillCurve.Evaluate(_currentCharge);
        fillImage.fillAmount = curvedProgress;

    }

    private void UpdateColor()
    {
        if (_isFlashing) return; // 正在闪烁时不更新颜色

        Color targetColor = colorGradient.Evaluate(_currentCharge);
        fillImage.color = targetColor;
    }
    public void Active(bool active)
    {
        gameObject.SetActive(active);
    }

    public void updateWiderBar(float w)
    {

        float current_width = cx - w;
        // 设置宽度
        outer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, current_width);

        // 设置高度
        outer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, current_width);

       
        if (Mathf.Approximately(w, gap))
        {
            color.a = 0f;
        }
        else
        {
            float progress = w / gap;
            color.a = Mathf.Pow(progress, 3) * 0.5f;
        }
        widerBar.color = color;

    }
    public void PlayFullChargeAnimation()
    {
        if (chargeAnimator != null)
        {
            chargeAnimator.Play("FullCharge", 0, 0f); // 播放名为"FullCharge"的动画
        }
    }

    public void StartFlashing()
    {

        _isFlashing = true;
        if (_flashCoroutine == null)
        {
            _flashCoroutine = StartCoroutine(FlashRoutine());
        }
    }
    public void StopFlashing()
    {
        _isFlashing = false;
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
            fillImage.color = _originalColor; // 恢复到满蓄力时的颜色
        }
    }
    private IEnumerator FlashRoutine()
    {
        float timer = 0f;

        while (true)
        {
            // 在两种颜色之间来回渐变
            float t = Mathf.PingPong(timer * flashSpeed, 1f);
            fillImage.color = Color.Lerp(flashColor1, flashColor2, t);

            timer += Time.deltaTime;
            yield return null;
        }
    }





}