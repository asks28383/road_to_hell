using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("射击设置")]
    public GameObject bulletPrefab;
    protected Transform muzzlePos;

    [Header("UI引用")]
    public chargeBar heatBar;

    [Header("武器参数")]
    public float heatPerShot = 0.2f; // 每次射击增加的百分比(0-1)

    // 从UI获取的状态
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
    // 新增方法：控制UI方向
    private void UpdateUIOrientation()
    {
        if (heatBar == null) return;

        // 获取武器当前的左右方向
        bool isFacingLeft = transform.localScale.y < 0;

        // 获取UI的RectTransform
        RectTransform uiRect = heatBar.GetComponent<RectTransform>();

        // 根据武器方向调整UI
        if (isFacingLeft)
        {
            // 人物朝左时UI需要镜像翻转
            uiRect.localScale = new Vector3(-1, 1, 1);
            // 调整位置偏移（根据需要微调）
            uiRect.anchoredPosition = new Vector2(-10, 0);
        }
        else
        {
            // 人物朝右时恢复正常
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
        // 射击逻辑
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        bullet.transform.position = muzzlePos.position;

        float angle = Random.Range(-5f, 5f);
        Vector2 shotDirection = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
        bullet.GetComponent<Bullet>().SetSpeed(shotDirection);

        TriggerAttackAnimation("Shoot");
    }

    // 提供给UI获取当前热量值的方法
    public float GetCurrentHeatForUI()
    {
        return CurrentHeatPercent * 100f; // 转换为0-100范围
    }
}