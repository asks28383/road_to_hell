using UnityEngine;
using UnityEngine.UI;

public class MeleeWeapon : Weapon
{
    // 配置参数
    [SerializeField] private int damages = 10; // 武器伤害值
    [SerializeField] private float attackCooldown = 0.5f; // 攻击冷却时间
    public Collider2D weaponCollider;
    public XuLiBar bar;
    public GameObject meleeHitBox;
    public GameObject rangedSlashPrefab; // 普通剑气
    public GameObject poweredSlashPrefab; // 强力剑气
    public float holdTimeForRanged = 2f;
    public float perfectChargeThreshold = 0.8f; // 完美蓄力允许的误差范围
    public float meleeRange = 1.5f;
    public string bulletTag = "EnemyBullet"; // 可以摧毁的弹幕标签

    // 状态变量
    protected float holdTimer;
    protected bool isHolding;
    protected bool isInPerfectWindow; // 是否处于完美蓄力窗口期
    protected Transform slashPoint;
    protected float cooldownTimer = 0f; // 攻击冷却计时器
    protected bool canAttack = true; // 是否可以攻击

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

    // 在攻击动画开始时调用（通过动画事件）
    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    // 在攻击动画结束时调用（通过动画事件）
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }

    protected override void Update()
    {
        base.Update();

        // 更新攻击冷却
        if (!canAttack)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canAttack = true;
            }
        }

        // 更新蓄力条显示
        bar.Active(isHolding && holdTimer >= 0.3f);
        bar.UpdateCharge(holdTimer);

        HandleAttack();
        if (isSwinging)
            UpdateSwordSwing();
    }

    protected override void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && canAttack && timer == 0)
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
                holdTimer >= holdTimeForRanged + perfectChargeThreshold / 2 &&
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
            if (holdTimer >= holdTimeForRanged) // 达到蓄力要求
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
            canAttack = false;
            cooldownTimer = attackCooldown;
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

    protected virtual void ReleaseRangedSlash(bool isPerfect)
    {
        TriggerAttackAnimation("Melee");
        StartSwordSwing();

        GameObject slashPrefab = isPerfect ? poweredSlashPrefab : rangedSlashPrefab;
        GameObject slash = ObjectPool.Instance.GetObject(slashPrefab);

        slash.transform.position = slashPoint.position;
        slash.GetComponent<Bullet>().SetSpeed(direction);
        slash.transform.right = direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查击中的对象是否有Health组件
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damages);
        }

        // 检查是否击中可摧毁的弹幕（使用标签检测）
        if (other.CompareTag(bulletTag))
        {
            // 摧毁弹幕
            Destroy(other.gameObject);

            // 或者使用对象池回收
            // ObjectPool.Instance.ReturnObject(other.gameObject);
        }
    }
}