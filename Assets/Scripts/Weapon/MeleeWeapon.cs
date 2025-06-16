using System.Collections;
using Unity.VisualScripting;
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

    [Header("近战武器武器攻击音效设置")]
    public AudioClip AttackClip;//挥刀音效
    private AudioSource audioSource;

    [Header("蓄力攻击音效设置")]
    public AudioClip chargingClip;          // 蓄力过程循环音效
    public AudioClip perfectChargeClip;     // 完美蓄力提示音
    public AudioClip chargeReleaseClip;     // 蓄力释放音效
    public AudioClip normalSlashClip;       // 普通剑气音效
    public AudioClip poweredSlashClip;      // 强力剑气音效

    private AudioSource chargingAudioSource; // 专门用于蓄力音效的AudioSource

    public bool get_isHolding() => isHolding;
    public float get_holdTime() => holdTimer;

    protected override void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // 专门为蓄力音效创建独立的AudioSource
        chargingAudioSource = gameObject.AddComponent<AudioSource>();
        chargingAudioSource.loop = true; // 蓄力音效需要循环播放
        chargingAudioSource.volume = 0.5f;
        base.Start();
        slashPoint = transform.Find("slashpoint");
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
        UpdateChargingSound(); // 更新蓄力音效
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
        //if (isSwinging)
        //    UpdateSwordSwing();
    }

    protected override void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && canAttack && timer == 0)
        {
            isHolding = true;
            holdTimer = 0f;
            isInPerfectWindow = false;
            // 开始播放蓄力音效
            chargingAudioSource.clip = chargingClip;
            chargingAudioSource.Play();

            // 渐入效果
            StartCoroutine(FadeAudio(chargingAudioSource, 0.5f, 0.1f));
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
                audioSource.PlayOneShot(perfectChargeClip); // 播放完美蓄力提示音
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
            // 停止蓄力音效（带渐出效果）
            StartCoroutine(FadeAudio(chargingAudioSource, 0f, 0.2f, true));
            if (holdTimer >= holdTimeForRanged) // 达到蓄力要求
            {
                Debug.Log("达到蓄力要求");
                // 根据是否在完美窗口内决定释放哪种剑气
                ReleaseRangedSlash(isInPerfectWindow);
            }
            else // 未达到蓄力要求
            {
                Debug.Log("未达到蓄力要求");
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
    // 音频淡入淡出效果
    private IEnumerator FadeAudio(AudioSource source, float targetVolume, float duration, bool stopAfterFade = false)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        if (stopAfterFade && targetVolume <= 0f)
        {
            source.Stop();
        }
    }

    // 动态调整蓄力音效音调（随蓄力时间变化）
    private void UpdateChargingSound()
    {
        if (isHolding)
        {
            // 随着蓄力时间增加音调
            float pitch = Mathf.Lerp(0.8f, 1.2f, holdTimer / (holdTimeForRanged + perfectChargeThreshold));
            chargingAudioSource.pitch = pitch;

            // 随着蓄力时间增加音量
            float volume = Mathf.Lerp(0.3f, 0.8f, holdTimer / (holdTimeForRanged + perfectChargeThreshold));
            chargingAudioSource.volume = volume;
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
        audioSource.PlayOneShot(AttackClip);
        StartSwordSwing();
    }

    protected virtual void ReleaseRangedSlash(bool isPerfect)
    {
        // 停止蓄力音效
        chargingAudioSource.Stop();

        // 播放蓄力释放音效
        audioSource.PlayOneShot(chargeReleaseClip);

        TriggerAttackAnimation("Melee");
        StartSwordSwing();

        GameObject slashPrefab = isPerfect ? poweredSlashPrefab : rangedSlashPrefab;
        AudioClip slashClip = isPerfect ? poweredSlashClip : normalSlashClip;

        GameObject slash = ObjectPool.Instance.GetObject(slashPrefab);
        if (slash == null)
        {
            Debug.Log("无预制体");
        }
        slash.transform.position = slashPoint.position;
        slash.GetComponent<Bullet>().SetSpeed(direction);
        slash.transform.right = direction;

        // 播放剑气音效
        AudioSource.PlayClipAtPoint(slashClip, slashPoint.position);
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