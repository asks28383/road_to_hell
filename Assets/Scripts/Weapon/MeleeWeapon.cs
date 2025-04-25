using UnityEngine;
using UnityEngine.UI;

public class MeleeWeapon : Weapon
{
    // ���ò���
    [SerializeField] private int damages = 10; // �����˺�ֵ
    [SerializeField] private float attackCooldown = 0.5f; // ������ȴʱ��
    public Collider2D weaponCollider;
    public XuLiBar bar;
    public GameObject meleeHitBox;
    public GameObject rangedSlashPrefab; // ��ͨ����
    public GameObject poweredSlashPrefab; // ǿ������
    public float holdTimeForRanged = 2f;
    public float perfectChargeThreshold = 0.8f; // ���������������Χ
    public float meleeRange = 1.5f;
    public string bulletTag = "EnemyBullet"; // ���Դݻٵĵ�Ļ��ǩ

    // ״̬����
    protected float holdTimer;
    protected bool isHolding;
    protected bool isInPerfectWindow; // �Ƿ�����������������
    protected Transform slashPoint;
    protected float cooldownTimer = 0f; // ������ȴ��ʱ��
    protected bool canAttack = true; // �Ƿ���Թ���

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

    // �ڹ���������ʼʱ���ã�ͨ�������¼���
    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    // �ڹ�����������ʱ���ã�ͨ�������¼���
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }

    protected override void Update()
    {
        base.Update();

        // ���¹�����ȴ
        if (!canAttack)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canAttack = true;
            }
        }

        // ������������ʾ
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

            // ����Ƿ����������������
            if (!isInPerfectWindow &&
                holdTimer >= holdTimeForRanged + perfectChargeThreshold / 2 &&
                holdTimer <= holdTimeForRanged + perfectChargeThreshold)
            {
                isInPerfectWindow = true;
                // ������������ӽ��������������Ӿ�Ч��
            }
            // ����Ƿ��Ѿ�����������������
            else if (isInPerfectWindow && holdTimer > holdTimeForRanged + perfectChargeThreshold)
            {
                isInPerfectWindow = false;
                // ��������������˳������������Ӿ�Ч��
            }
        }

        if (Input.GetButtonUp("Fire1") && isHolding)
        {
            if (holdTimer >= holdTimeForRanged) // �ﵽ����Ҫ��
            {
                // �����Ƿ������������ھ����ͷ����ֽ���
                ReleaseRangedSlash(isInPerfectWindow);
            }
            else // δ�ﵽ����Ҫ��
            {
                PerformMeleeAttack();
            }

            // ����״̬
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

        if (transform.rotation.eulerAngles.x < 0f) // �������
        {
            swordTransform.localRotation = originalSwordRotation * Quaternion.Euler(0, 0, -baseAngle);
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.flipX = true;
                renderer.flipY = false;
            }
        }
        else // �����Ҳ�
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
        // �����еĶ����Ƿ���Health���
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damages);
        }

        // ����Ƿ���пɴݻٵĵ�Ļ��ʹ�ñ�ǩ��⣩
        if (other.CompareTag(bulletTag))
        {
            // �ݻٵ�Ļ
            Destroy(other.gameObject);

            // ����ʹ�ö���ػ���
            // ObjectPool.Instance.ReturnObject(other.gameObject);
        }
    }
}