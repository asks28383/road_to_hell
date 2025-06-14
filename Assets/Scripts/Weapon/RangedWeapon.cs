using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("射击设置")]
    public GameObject bulletPrefab;
    protected Transform muzzlePos;
    public float shotSpeed;

    [Header("UI引用")]
    public chargeBar heatBar;

    [Header("武器参数")]
    public float heatPerShot = 0.2f; // 每次射击增加的百分比(0-1)

    [Header("过热视觉效果")]
    public Renderer weaponRenderer;
    public Color normalColor = Color.white;
    public Color overheatColor = Color.red;
    // 私有状态变量
    private bool isOverheated = false;

    protected override void Start()
    {
        base.Start();
        muzzlePos = transform.Find("Muzzle");

        // 初始化武器颜色
        if (weaponRenderer != null)
        {
            weaponRenderer.material.color = normalColor;
        }
    }

    protected override void Update()
    {
        base.Update();

        // 从UI同步过热状态
        if (heatBar != null)
        {
            isOverheated = heatBar.IsWeaponOverheated();
            
            // 更新武器颜色
            if (weaponRenderer != null)
            {
                weaponRenderer.material.color = isOverheated ? overheatColor : normalColor;
            }
        }
        float percent = heatBar.get_percent();
        //只在非过热状态下处理攻击
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
        // 射击逻辑
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        bullet.transform.position = muzzlePos.position;

        float angle = Random.Range(-5f, 5f);
        Vector2 shotDirection = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
        bullet.GetComponent<Bullet>().SetSpeed(shotDirection*shotSpeed);

        TriggerAttackAnimation("Shoot");
    }
}