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


    // �������Ӷ�����
    [Header("Sword Swing Settings")]
    public Transform swordTransform; // ����Transform���
    public float swingAngle = 90f;   // �ӽ��Ƕ�
    public float swingDuration = 0.3f; // �ӽ�����ʱ��
    private Quaternion originalSwordRotation; // ���ĳ�ʼ��ת
    private bool isSwinging = false;
    private float swingTimer = 0f;
    private int swingDirection = 1; // 1Ϊ�ң�-1Ϊ��

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
        // ������ס������
        if (Input.GetButton("Fire1") && isHolding)
        {
            holdTimer += Time.deltaTime;

            // �����㹻ʱ�䴥��Զ�̹���
            if (holdTimer >= holdTimeForRanged)
            {
                ReleaseRangedSlash();
                isHolding = false;
                holdTimer = 0f;
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
            isHolding = false;
            holdTimer = 0f;
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

        // �����ӵ��Ƕȣ���Զ���ϣ�
        float baseAngle = easedProgress * swingAngle;

        // �ؼ��޸ģ�ʹ�ù̶��ֲ���ת��
        if (transform.rotation.eulerAngles.x < 0f) // �������
        {
            // ���ʱ������ת������������180�ȷ�ת��
            swordTransform.localRotation = originalSwordRotation * Quaternion.Euler(0, 0, -baseAngle);

            // ǿ��������ͼ����ת
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.flipX = true; // ���������巭ת
                renderer.flipY = false;
            }
        }
        else // �����Ҳ�
        {
            // �Ҳ�������ת
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
            // ������ת״̬
            swordTransform.localRotation = originalSwordRotation;
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var resetRenderer))
            {
                resetRenderer.flipX = false;
            }
        }
    }

    /// <summary>
    /// ���λ���������ʹ��������Ȼ
    /// </summary>
    float EaseOutQuad(float t)
    {
        return t * (2 - t);
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