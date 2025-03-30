using UnityEngine;
using UnityEngine.UI;

public class MeleeWeapon : Weapon
{
    // 配置参数
    public XuLiBar bar;
    public GameObject meleeHitBox;
    public GameObject rangedSlashPrefab; // 普通剑气
    public GameObject poweredSlashPrefab; // 强力剑气
    public float holdTimeForRanged = 3f;
    public float perfectChargeThreshold = 0.15f; // 完美蓄力允许的误差范围
    public float meleeRange = 1.5f;

    // 状态变量
    protected float holdTimer;
    protected bool isHolding;
    protected bool isInPerfectWindow; // 是否处于完美蓄力窗口期
    protected Transform slashPoint;

    [Header("Sword Swing Settings")]
    public Transform swordTransform;
    public float swingAngle = 90f;
    public float swingDuration = 0.3f;
    private Quaternion originalSwordRotation;
    private bool isSwinging = false;
    private float swingTimer = 0f;

    public bool get_isHolding() => isHolding;
    public float get_holdTime() => holdTimer;

    protected override void Start()
    {
        base.Start();
        slashPoint = transform.Find("SlashPoint");
        if (swordTransform != null)
        {
            originalSwordRotation = swordTransform.localRotation;
        }
    }

    protected override void Update()
    {
        base.Update();

        // 更新蓄力条显示
        bar.Active(isHolding && holdTimer >= 0.3f);
        bar.UpdateCharge(holdTimer);

        HandleAttack();
        Debug.Log(isInPerfectWindow);
        if (isSwinging)
            UpdateSwordSwing();
    }

    protected override void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && timer == 0)
        {
            isHolding = true;
            holdTimer = 0f;
            isInPerfectWindow = false;
        }

        if (Input.GetButton("Fire1") && isHolding)
        {
            holdTimer += Time.deltaTime;

            // 检查是否进入完美蓄力窗口
            if (!isInPerfectWindow &&
                holdTimer >= holdTimeForRanged - perfectChargeThreshold &&
                holdTimer <= holdTimeForRanged + perfectChargeThreshold)
            {
                isInPerfectWindow = true;
                // 可以在这里添加进入完美蓄力的视觉效果
            }
            // 检查是否已经超过完美蓄力窗口
            else if (isInPerfectWindow && holdTimer > holdTimeForRanged + perfectChargeThreshold)
            {
                isInPerfectWindow = false;
                // 可以在这里添加退出完美蓄力的视觉效果
            }
        }

        if (Input.GetButtonUp("Fire1") && isHolding)
        {
            if (holdTimer >= holdTimeForRanged - perfectChargeThreshold) // 达到蓄力要求
            {
                // 根据是否在完美窗口内决定释放哪种剑气
                ReleaseRangedSlash(isInPerfectWindow);
            }
            else // 未达到蓄力要求
            {
                PerformMeleeAttack();
            }

            // 重置状态
            isHolding = false;
            holdTimer = 0f;
            isInPerfectWindow = false;
            timer = interval;
        }
    }
    void StartSwordSwing()
    {
        if (swordTransform == null) return;
        isSwinging = true;
        swingTimer = 0f;
    }

    void UpdateSwordSwing()
    {
        if (swordTransform == null) return;

        swingTimer += Time.deltaTime;
        float swingProgress = Mathf.PingPong(swingTimer / swingDuration * 2, 1f);
        float easedProgress = EaseOutQuad(swingProgress);
        float baseAngle = easedProgress * swingAngle;

        if (transform.rotation.eulerAngles.x < 0f) // 面向左侧
        {
            swordTransform.localRotation = originalSwordRotation * Quaternion.Euler(0, 0, -baseAngle);
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.flipX = true;
                renderer.flipY = false;
            }
        }
        else // 面向右侧
        {
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
            swordTransform.localRotation = originalSwordRotation;
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var resetRenderer))
            {
                resetRenderer.flipX = false;
            }
        }
    }

    float EaseOutQuad(float t) => t * (2 - t);

    protected virtual void PerformMeleeAttack()
    {
        TriggerAttackAnimation("Melee");
        StartSwordSwing();
    }


    // 修改后的释放剑气方法，增加isPerfect参数

    protected virtual void ReleaseRangedSlash(bool isPerfect)
    {
        TriggerAttackAnimation("Melee");
        StartSwordSwing();

        GameObject slashPrefab = isPerfect ? poweredSlashPrefab : rangedSlashPrefab;
        GameObject slash = ObjectPool.Instance.GetObject(slashPrefab);

        slash.transform.position = slashPoint.position;
        slash.GetComponent<Bullet>().SetSpeed(direction);
        slash.transform.right = direction;

        if (isPerfect)
        {
            // 设置强力剑气属性
            //var bullet = slash.GetComponent<Bullet>();
            //bullet.damage *= 2f; // 双倍伤害
            //bullet.speed *= 1.2f; // 更快速度
        }
    }
}