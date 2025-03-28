using UnityEngine;
using UnityEngine.UI;
public class MeleeWeapon : Weapon
{
    // �������ò���
    public GameObject meleeHitBox;      // ��ս�����ж�����
    public GameObject rangedSlashPrefab; // Զ�̽���Ԥ����
    public float holdTimeForRanged = 1f; // ������ô���Զ�̹���
    public float meleeRange = 1.5f;     // ��ս������Χ

    // ����״̬����
    protected float holdTimer;          // ������ʱ��
    protected bool isHolding;           // �Ƿ����ڳ���
    protected Transform slashPoint;     // �������ɵ�

    // ����������UI����
    [Header("Charge UI Settings")]
    public Slider chargeSlider;          // ������Slider���
    public Image chargeFillImage;        // ���������ͼ��
    public Color minChargeColor = Color.white; // ��С������ɫ
    public Color maxChargeColor = Color.blue;  // ���������ɫ
    public GameObject chargeUI;          // ����������UI����

    // �������Ӷ�����
    [Header("Sword Swing Settings")]
    public Transform swordTransform; // ����Transform���
    public float swingAngle = 90f;   // �ӽ��Ƕ�
    public float swingDuration = 0.3f; // �ӽ�����ʱ��
    private Quaternion originalSwordRotation; // ���ĳ�ʼ��ת
    private bool isSwinging = false;
    private float swingTimer = 0f;
    private int swingDirection = 1; // 1Ϊ�ң�-1Ϊ��

    protected override void Start()
    {
        base.Start();
        //meleeHitBox.SetActive(false);
        slashPoint = transform.Find("SlashPoint");
        InitializeChargeUI();
        // ��¼���ĳ�ʼ��ת
        if (swordTransform != null)
        {
            originalSwordRotation = swordTransform.localRotation;
        }
    }
    protected override void Update()
    {
        base.Update();

        // �������λ��ȷ������
        if (direction.x < 0) // ����
            swingDirection = -1;
        else if (direction.x > 0) // ����
            swingDirection = 1;

        if (isSwinging)
            UpdateSwordSwing();
    }

    /// <summary>
    /// ��ʼ��������UI
    /// </summary>
    void InitializeChargeUI()
    {
        if (chargeSlider != null)
        {
            chargeSlider.minValue = 0;
            chargeSlider.maxValue = holdTimeForRanged;
            chargeSlider.value = 0;
            chargeUI.SetActive(false); // Ĭ������
        }
    }
    protected override void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && timer == 0)
        {
            StartCharging();

        }
        // ������ס������
        if (Input.GetButton("Fire1") && isHolding)
        {
            UpdateCharging();

            // �����㹻ʱ�䴥��Զ�̹���
            if (holdTimer >= holdTimeForRanged)
            {
                ReleaseRangedSlash();
                ResetCharging();
                isHolding = false;
                timer = interval;
            }
        }
        // �ɿ�������
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
    /// ��ʼ���ĻӶ�����
    /// </summary>
    void StartSwordSwing()
    {
        if (swordTransform == null) return;

        isSwinging = true;
        swingTimer = 0f;
    }

    /// <summary>
    /// ���½��ĻӶ�����
    /// </summary>
    void UpdateSwordSwing()
    {
        if (swordTransform == null) return;

        swingTimer += Time.deltaTime;
        float swingProgress = Mathf.PingPong(swingTimer / swingDuration * 2, 1f);
        float easedProgress = EaseOutQuad(swingProgress);

        // ���ݷ��������ת
        float currentAngle = easedProgress * swingAngle ;
        swordTransform.localRotation = originalSwordRotation * Quaternion.Euler(0, 0, currentAngle);

        if (swingTimer >= swingDuration)
        {
            isSwinging = false;
            swordTransform.localRotation = originalSwordRotation;
        }
    }

    /// <summary>
    /// ���λ���������ʹ��������Ȼ
    /// </summary>
    float EaseOutQuad(float t)
    {
        return t * (2 - t);
    }

    /// <summary>
    /// ��ʼ����
    /// </summary>
    void StartCharging()
    {
        isHolding = true;
        holdTimer = 0f;
        if (chargeUI != null) chargeUI.SetActive(true);
    }

    /// <summary>
    /// ��������״̬
    /// </summary>
    void UpdateCharging()
    {
        holdTimer += Time.deltaTime;

        // ����UI
        if (chargeSlider != null)
        {
            chargeSlider.value = holdTimer;
            // ��������ɫ����
            float chargePercent = holdTimer / holdTimeForRanged;
            chargeFillImage.color = Color.Lerp(minChargeColor, maxChargeColor, chargePercent);
        }
    }

    /// <summary>
    /// ��������״̬
    /// </summary>
    void ResetCharging()
    {
        isHolding = false;
        holdTimer = 0f;
        if (chargeSlider != null) chargeSlider.value = 0;
        if (chargeUI != null) chargeUI.SetActive(false);
    }

    // ִ�н�ս����
    protected virtual void PerformMeleeAttack()
    {
        TriggerAttackAnimation("Melee");
        StartSwordSwing();
        //// �����ս�ж�����
        //meleeHitBox.SetActive(true);
        //// ���Ը�����Ҫ���������һ��Э�����ӳٹر��ж�����
        //Invoke("DisableMeleeHitBox", 0.2f);
    }

    //protected void DisableMeleeHitBox()
    //{
    //    meleeHitBox.SetActive(false);
    //}

    // �ͷ�Զ�̽���
    protected virtual void ReleaseRangedSlash()
    {
        TriggerAttackAnimation("Melee");
        // �Ӷ���ػ�ȡ����ʵ��
        GameObject slash = ObjectPool.Instance.GetObject(rangedSlashPrefab);
        slash.transform.position = slashPoint.position;
        // ���ý���������ٶ�
        slash.GetComponent<Bullet>().SetSpeed(direction);
        // ���ý�������ת���򣨳�����귽��
        slash.transform.right = direction;
    }
}