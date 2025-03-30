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


    // 新增剑挥动参数
    [Header("Sword Swing Settings")]
    public Transform swordTransform; // 剑的Transform组件
    public float swingAngle = 90f;   // 挥剑角度
    public float swingDuration = 0.3f; // 挥剑持续时间
    private Quaternion originalSwordRotation; // 剑的初始旋转
    private bool isSwinging = false;
    private float swingTimer = 0f;
    private int swingDirection = 1; // 1为右，-1为左

    public bool get_isHolding()
    {
        return isHolding;
    }
    public float get_holdTime()
    {
        return holdTimer;
    }
    protected override void Start()
    {
        base.Start();
        //meleeHitBox.SetActive(false);
        slashPoint = transform.Find("SlashPoint");
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
        HandleAttack();
        Debug.Log(holdTimer);
        if (isSwinging)
            UpdateSwordSwing();
    }

    protected override void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && timer == 0)
        {
            isHolding = true;
            holdTimer = 0f;
        }
        // 持续按住攻击键
        if (Input.GetButton("Fire1") && isHolding)
        {
            holdTimer += Time.deltaTime;

            // 长按足够时间触发远程攻击
            if (holdTimer >= holdTimeForRanged)
            {
                ReleaseRangedSlash();
                isHolding = false;
                holdTimer = 0f;
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
            isHolding = false;
            holdTimer = 0f;
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

        // 基础挥刀角度（永远向上）
        float baseAngle = easedProgress * swingAngle;

        // 关键修改：使用固定局部旋转轴
        if (transform.rotation.eulerAngles.x < 0f) // 面向左侧
        {
            // 左侧时反向旋转（补偿父物体180度翻转）
            swordTransform.localRotation = originalSwordRotation * Quaternion.Euler(0, 0, -baseAngle);

            // 强制武器贴图不翻转
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.flipX = true; // 补偿父物体翻转
                renderer.flipY = false;
            }
        }
        else // 面向右侧
        {
            // 右侧正常旋转
            swordTransform.localRotation = originalSwordRotation * Quaternion.Euler(0, 0, baseAngle);

            if (swordTransform.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.flipX = false;
                renderer.flipY = false;
            }
        }

        if (swingTimer >= swingDuration)
        {
            isSwinging = false;
            // 重置旋转状态
            swordTransform.localRotation = originalSwordRotation;
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var resetRenderer))
            {
                resetRenderer.flipX = false;
            }
        }
    }

    /// <summary>
    /// 二次缓出函数，使动作更自然
    /// </summary>
    float EaseOutQuad(float t)
    {
        return t * (2 - t);
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