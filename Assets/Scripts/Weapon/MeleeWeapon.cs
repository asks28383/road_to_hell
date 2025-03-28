using UnityEngine;
using UnityEngine.UI;
public class MeleeWeapon : Weapon
{
    // 特有配置参数
    public GameObject meleeHitBox;      // 近战攻击判定区域
    public GameObject rangedSlashPrefab; // 远程剑气预制体
    public float holdTimeForRanged = 1f; // 长按多久触发远程攻击
    public float meleeRange = 1.5f;     // 近战攻击范围

    // 特有状态变量
    protected float holdTimer;          // 长按计时器
    protected bool isHolding;           // 是否正在长按
    protected Transform slashPoint;     // 剑气生成点

    // 新增蓄力条UI参数
    [Header("Charge UI Settings")]
    public Slider chargeSlider;          // 蓄力条Slider组件
    public Image chargeFillImage;        // 蓄力条填充图像
    public Color minChargeColor = Color.white; // 最小蓄力颜色
    public Color maxChargeColor = Color.blue;  // 最大蓄力颜色
    public GameObject chargeUI;          // 整个蓄力条UI对象

    // 新增剑挥动参数
    [Header("Sword Swing Settings")]
    public Transform swordTransform; // 剑的Transform组件
    public float swingAngle = 90f;   // 挥剑角度
    public float swingDuration = 0.3f; // 挥剑持续时间
    private Quaternion originalSwordRotation; // 剑的初始旋转
    private bool isSwinging = false;
    private float swingTimer = 0f;
    private int swingDirection = 1; // 1为右，-1为左

    protected override void Start()
    {
        base.Start();
        //meleeHitBox.SetActive(false);
        slashPoint = transform.Find("SlashPoint");
        InitializeChargeUI();
        // 记录剑的初始旋转
        if (swordTransform != null)
        {
            originalSwordRotation = swordTransform.localRotation;
        }
    }
    protected override void Update()
    {
        base.Update();

        // 根据鼠标位置确定方向
        if (direction.x < 0) // 向左
            swingDirection = -1;
        else if (direction.x > 0) // 向右
            swingDirection = 1;

        if (isSwinging)
            UpdateSwordSwing();
    }

    /// <summary>
    /// 初始化蓄力条UI
    /// </summary>
    void InitializeChargeUI()
    {
        if (chargeSlider != null)
        {
            chargeSlider.minValue = 0;
            chargeSlider.maxValue = holdTimeForRanged;
            chargeSlider.value = 0;
            chargeUI.SetActive(false); // 默认隐藏
        }
    }
    protected override void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && timer == 0)
        {
            StartCharging();

        }
        // 持续按住攻击键
        if (Input.GetButton("Fire1") && isHolding)
        {
            UpdateCharging();

            // 长按足够时间触发远程攻击
            if (holdTimer >= holdTimeForRanged)
            {
                ReleaseRangedSlash();
                ResetCharging();
                isHolding = false;
                timer = interval;
            }
        }
        // 松开攻击键
        if (Input.GetButtonUp("Fire1") && isHolding)
        {
            if (holdTimer < holdTimeForRanged)
            {
                PerformMeleeAttack();
                timer = interval;
            }
            ResetCharging();
        }
    }


    /// <summary>
    /// 开始剑的挥动动画
    /// </summary>
    void StartSwordSwing()
    {
        if (swordTransform == null) return;

        isSwinging = true;
        swingTimer = 0f;
    }

    /// <summary>
    /// 更新剑的挥动动画
    /// </summary>
    void UpdateSwordSwing()
    {
        if (swordTransform == null) return;

        swingTimer += Time.deltaTime;
        float swingProgress = Mathf.PingPong(swingTimer / swingDuration * 2, 1f);
        float easedProgress = EaseOutQuad(swingProgress);

        // 根据方向调整旋转
        float currentAngle = easedProgress * swingAngle ;
        swordTransform.localRotation = originalSwordRotation * Quaternion.Euler(0, 0, currentAngle);

        if (swingTimer >= swingDuration)
        {
            isSwinging = false;
            swordTransform.localRotation = originalSwordRotation;
        }
    }

    /// <summary>
    /// 二次缓出函数，使动作更自然
    /// </summary>
    float EaseOutQuad(float t)
    {
        return t * (2 - t);
    }

    /// <summary>
    /// 开始蓄力
    /// </summary>
    void StartCharging()
    {
        isHolding = true;
        holdTimer = 0f;
        if (chargeUI != null) chargeUI.SetActive(true);
    }

    /// <summary>
    /// 更新蓄力状态
    /// </summary>
    void UpdateCharging()
    {
        holdTimer += Time.deltaTime;

        // 更新UI
        if (chargeSlider != null)
        {
            chargeSlider.value = holdTimer;
            // 蓄力条颜色渐变
            float chargePercent = holdTimer / holdTimeForRanged;
            chargeFillImage.color = Color.Lerp(minChargeColor, maxChargeColor, chargePercent);
        }
    }

    /// <summary>
    /// 重置蓄力状态
    /// </summary>
    void ResetCharging()
    {
        isHolding = false;
        holdTimer = 0f;
        if (chargeSlider != null) chargeSlider.value = 0;
        if (chargeUI != null) chargeUI.SetActive(false);
    }

    // 执行近战攻击
    protected virtual void PerformMeleeAttack()
    {
        TriggerAttackAnimation("Melee");
        StartSwordSwing();
        //// 激活近战判定区域
        //meleeHitBox.SetActive(true);
        //// 可以根据需要在这里添加一个协程来延迟关闭判定区域
        //Invoke("DisableMeleeHitBox", 0.2f);
    }

    //protected void DisableMeleeHitBox()
    //{
    //    meleeHitBox.SetActive(false);
    //}

    // 释放远程剑气
    protected virtual void ReleaseRangedSlash()
    {
        TriggerAttackAnimation("Melee");
        // 从对象池获取剑气实例
        GameObject slash = ObjectPool.Instance.GetObject(rangedSlashPrefab);
        slash.transform.position = slashPoint.position;
        // 设置剑气方向和速度
        slash.GetComponent<Bullet>().SetSpeed(direction);
        // 设置剑气的旋转方向（朝向鼠标方向）
        slash.transform.right = direction;
    }
}