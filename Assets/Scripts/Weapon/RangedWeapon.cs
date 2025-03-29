using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("�������")]
    public GameObject bulletPrefab;
    protected Transform muzzlePos;

    [Header("UI����")]
    public chargeBar heatBar;

    [Header("��������")]
    public float heatPerShot = 0.2f; // ÿ��������ӵİٷֱ�(0-1)

    // ��UI��ȡ��״̬
    public bool IsOverheated => heatBar != null && heatBar.IsWeaponOverheated();
    public float CurrentHeatPercent => heatBar != null ? heatBar.get_percent() : 0f;

    protected override void Start()
    {
        base.Start();
        muzzlePos = transform.Find("Muzzle");
    }

    protected override void Update()
    {
        base.Update();
        HandleAttack();
        UpdateUIOrientation();
    }
    // ��������������UI����
    private void UpdateUIOrientation()
    {
        if (heatBar == null) return;

        // ��ȡ������ǰ�����ҷ���
        bool isFacingLeft = transform.localScale.y < 0;

        // ��ȡUI��RectTransform
        RectTransform uiRect = heatBar.GetComponent<RectTransform>();

        // ���������������UI
        if (isFacingLeft)
        {
            // ���ﳯ��ʱUI��Ҫ����ת
            uiRect.localScale = new Vector3(-1, 1, 1);
            // ����λ��ƫ�ƣ�������Ҫ΢����
            uiRect.anchoredPosition = new Vector2(-10, 0);
        }
        else
        {
            // ���ﳯ��ʱ�ָ�����
            uiRect.localScale = Vector3.one;
            uiRect.anchoredPosition = Vector2.zero;
        }
    }
    protected override void HandleAttack()
    {
        if (IsOverheated) return;

        if (Input.GetButton("Fire1") && timer == 0)
        {
            timer = interval;
            Fire();
        }
    }

    private void Fire()
    {
        // ����߼�
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        bullet.transform.position = muzzlePos.position;

        float angle = Random.Range(-5f, 5f);
        Vector2 shotDirection = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
        bullet.GetComponent<Bullet>().SetSpeed(shotDirection);

        TriggerAttackAnimation("Shoot");
    }

    // �ṩ��UI��ȡ��ǰ����ֵ�ķ���
    public float GetCurrentHeatForUI()
    {
        return CurrentHeatPercent * 100f; // ת��Ϊ0-100��Χ
    }
}