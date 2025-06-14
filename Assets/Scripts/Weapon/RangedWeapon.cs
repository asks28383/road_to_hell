using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("�������")]
    public GameObject bulletPrefab;
    protected Transform muzzlePos;
    public float shotSpeed;

    [Header("UI����")]
    public chargeBar heatBar;

    [Header("��������")]
    public float heatPerShot = 0.2f; // ÿ��������ӵİٷֱ�(0-1)

    [Header("�����Ӿ�Ч��")]
    public Renderer weaponRenderer;
    public Color normalColor = Color.white;
    public Color overheatColor = Color.red;
    // ˽��״̬����
    private bool isOverheated = false;

    protected override void Start()
    {
        base.Start();
        muzzlePos = transform.Find("Muzzle");

        // ��ʼ��������ɫ
        if (weaponRenderer != null)
        {
            weaponRenderer.material.color = normalColor;
        }
    }

    protected override void Update()
    {
        base.Update();

        // ��UIͬ������״̬
        if (heatBar != null)
        {
            isOverheated = heatBar.IsWeaponOverheated();
            
            // ����������ɫ
            if (weaponRenderer != null)
            {
                weaponRenderer.material.color = isOverheated ? overheatColor : normalColor;
            }
        }
        float percent = heatBar.get_percent();
        //ֻ�ڷǹ���״̬�´�����
        //Debug.Log("1:" + isOverheated);
        if (!isOverheated)
        {

            HandleAttack();
        }
    }

    protected override void HandleAttack()
    {
        if (Input.GetButton("Fire1") && timer == 0)
        {
            heatBar.IncreaseHeat(0.05f);
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
        bullet.GetComponent<Bullet>().SetSpeed(shotDirection*shotSpeed);

        TriggerAttackAnimation("Shoot");
    }
}