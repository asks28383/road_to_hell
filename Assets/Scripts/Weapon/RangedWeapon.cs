using UnityEngine;
using UnityEngine.UI;
public class RangedWeapon : Weapon
{
    [Header("Overheat Settings")]
    [Tooltip("ÿ��������ӵ�����ֵ")]
    public float heatPerShot = 10f;
    [Tooltip("�������ֵ")]
    public float maxHeat = 100f;
    [Tooltip("ֹͣ���ʱ����ȴ���ʣ���λ/�룩")]
    public float cooldownRate = 20f;
    [Tooltip("���Ⱥ�ĳͷ�ʱ�䣨�룩")]
    public float overheatPenaltyTime = 2f;
    [Tooltip("У׼����Χ��0-1�ٷֱȣ�")]
    public Vector2 calibrationZone = new Vector2(0.6f, 0.8f);

    [Header("Overheat Visuals")]
    public Renderer weaponRenderer; // ������Renderer���
    public Color weapon_normalColor = Color.white; // ������ɫ
    public Color weapon_overheatColor = Color.red; // ������ɫ

    [Header("UI References")]
    [Tooltip("������Slider���")]
    public Slider heatSlider;
    [Tooltip("У׼������ʾͼ��")]
    public Image calibrationIndicator;
    [Tooltip("����״̬��ɫ")]
    public Color normalColor = Color.yellow;
    [Tooltip("����״̬��ɫ")]
    public Color overheatColor = Color.red;
    [Tooltip("У׼������ɫ")]
    public Color calibrationColor = Color.green;

    // �ڲ�״̬����
    protected float currentHeat;      // ��ǰ����ֵ
    protected bool isOverheated;      // �Ƿ��ڹ���״̬
    protected float overheatTimer;    // ����ʣ��ʱ��
    protected bool inCalibrationZone; // �Ƿ���У׼����
    // �������ò���
    public GameObject bulletPrefab; // �ӵ�Ԥ��������
    protected Transform muzzlePos;  // ǹ��λ�ñ任���

    protected override void Start()
    {
        base.Start();
        muzzlePos = transform.Find("Muzzle");
        InitializeHeatSystem();
    }

    void InitializeHeatSystem()
    {
        if (heatSlider != null)
        {
            heatSlider.maxValue = maxHeat;
            heatSlider.value = 0;
            // ����У׼������ʾ��Χ
            calibrationIndicator.rectTransform.anchorMin = new Vector2(calibrationZone.x, 0);
            calibrationIndicator.rectTransform.anchorMax = new Vector2(calibrationZone.y, 1);
            calibrationIndicator.color = new Color(0, 1, 0, 0.3f); // ��͸����ɫ
        }
    }
    protected override void Update()
    {
        base.Update(); // ������Ĺ��������ʱ��
        HandleHeatSystem();
        UpdateUI();
    }

    protected override void HandleAttack()
    {
        Debug.Log(currentHeat);
        if (isOverheated) return; // ����ʱ��ֹ���
        // ��⿪������
        if (Input.GetButton("Fire1") && timer == 0)
        {
            timer = interval;
            Fire();
        }
        //// У׼ʱ��ס�������ɿ�����ȴ
        //if (inCalibrationZone)
        //{
        //    ClearOverheat();
        //    PlayCalibrationEffect(); // ����У׼�ɹ���Ч
        //}
    }
    /// <summary>
    /// ��������ϵͳ�߼�
    /// </summary>
    void HandleHeatSystem()
    {
        // ���ȳͷ���ʱ
        if (isOverheated)
        {
            overheatTimer -= Time.deltaTime;
            if (overheatTimer <= 0)
            {
                ResetHeatSystem();
            }
            return;
        }

        // ��Ȼ��ȴ
        if (!Input.GetButton("Fire1") && currentHeat > 0)
        {
            currentHeat -= cooldownRate * Time.deltaTime;
            currentHeat = Mathf.Max(0, currentHeat);
        }

        //// ����Ƿ����У׼����
        //float heatPercent = currentHeat / maxHeat;
        //inCalibrationZone = heatPercent >= calibrationZone.x && heatPercent <= calibrationZone.y;

        // ���ȼ��
        if (currentHeat >= maxHeat)
        {
            EnterOverheat();
        }
    }

    /// <summary>
    /// ִ������߼�
    /// </summary>
    protected virtual void Fire()
    {
        // ��������
        currentHeat += heatPerShot;

        // �����������
        TriggerAttackAnimation("Shoot");

        // �Ӷ���ػ�ȡ�ӵ�
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        bullet.transform.position = muzzlePos.position;

        // ������ɢ��
        float angle = Random.Range(-5f, 5f);
        Vector2 shotDirection = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
        bullet.GetComponent<Bullet>().SetSpeed(shotDirection);

        // ����ǹ���������Ч
        PlayMuzzleEffect();
    }

    private void UpdateHeatVisuals()
    {
        if (weaponRenderer == null) return;

        // ����״̬�л���ɫ
        weaponRenderer.material.color = isOverheated ? weapon_overheatColor : weapon_normalColor;
    }
    /// <summary>
    /// �������״̬
    /// </summary>
    void EnterOverheat()
    {
        isOverheated = true;
        overheatTimer = overheatPenaltyTime;
        currentHeat = maxHeat; // ȷ��UI��ʾ����

        // ���Ź�����Ч
        PlayOverheatEffect();
        UpdateHeatVisuals(); // ���
    }

    /// <summary>
    /// ��������ϵͳ
    /// </summary>
    void ResetHeatSystem()
    {
        isOverheated = false;
        currentHeat = 0;
        overheatTimer = 0;
        UpdateHeatVisuals(); // �ָ�ԭɫ
    }

    /// <summary>
    /// У׼�ɹ�ʱ�������
    /// </summary>
    void ClearOverheat()
    {
        currentHeat = 0;
        inCalibrationZone = false;
    }

    /// <summary>
    /// ����UI��ʾ
    /// </summary>
    void UpdateUI()
    {
        if (heatSlider == null) return;

        // ������������ֵ
        heatSlider.value = currentHeat;

        // ����״̬�ı���ɫ
        Image fillImage = heatSlider.fillRect.GetComponent<Image>();
        if (isOverheated)
        {
            fillImage.color = overheatColor;
        }
        else if (inCalibrationZone)
        {
            fillImage.color = calibrationColor;
        }
        else
        {
            fillImage.color = normalColor;
        }
    }

    // === ��Ч���� === //
    void PlayMuzzleEffect()
    {
        // ʵ��ǹ��������Ч
        // ���磺Instantiate(muzzleFlash, muzzlePos.position, muzzlePos.rotation);
    }

    void PlayOverheatEffect()
    {
        // ʵ�ֹ�����Ч
        // ���磺PlaySound(overheatSound);
    }

    void PlayCalibrationEffect()
    {
        // ʵ��У׼�ɹ���Ч
        // ���磺StartCoroutine(FlashIndicator());
    }
}