using UnityEngine;
using UnityEngine.UI;
public class RangedWeapon : Weapon
{
    [Header("Overheat Settings")]
    [Tooltip("每次射击增加的热量值")]
    public float heatPerShot = 10f;
    [Tooltip("最大热量值")]
    public float maxHeat = 100f;
    [Tooltip("停止射击时的冷却速率（单位/秒）")]
    public float cooldownRate = 20f;
    [Tooltip("过热后的惩罚时间（秒）")]
    public float overheatPenaltyTime = 2f;
    [Tooltip("校准区域范围（0-1百分比）")]
    public Vector2 calibrationZone = new Vector2(0.6f, 0.8f);

    [Header("Overheat Visuals")]
    public Renderer weaponRenderer; // 武器的Renderer组件
    public Color weapon_normalColor = Color.white; // 正常颜色
    public Color weapon_overheatColor = Color.red; // 过热颜色

    [Header("UI References")]
    [Tooltip("热量条Slider组件")]
    public Slider heatSlider;
    [Tooltip("校准区域显示图像")]
    public Image calibrationIndicator;
    [Tooltip("正常状态颜色")]
    public Color normalColor = Color.yellow;
    [Tooltip("过热状态颜色")]
    public Color overheatColor = Color.red;
    [Tooltip("校准区域颜色")]
    public Color calibrationColor = Color.green;

    // 内部状态变量
    protected float currentHeat;      // 当前热量值
    protected bool isOverheated;      // 是否处于过热状态
    protected float overheatTimer;    // 过热剩余时间
    protected bool inCalibrationZone; // 是否处于校准区域
    // 特有配置参数
    public GameObject bulletPrefab; // 子弹预制体引用
    protected Transform muzzlePos;  // 枪口位置变换组件

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
            // 设置校准区域显示范围
            calibrationIndicator.rectTransform.anchorMin = new Vector2(calibrationZone.x, 0);
            calibrationIndicator.rectTransform.anchorMax = new Vector2(calibrationZone.y, 1);
            calibrationIndicator.color = new Color(0, 1, 0, 0.3f); // 半透明绿色
        }
    }
    protected override void Update()
    {
        base.Update(); // 处理父类的攻击间隔计时器
        HandleHeatSystem();
        UpdateUI();
    }

    protected override void HandleAttack()
    {
        Debug.Log(currentHeat);
        if (isOverheated) return; // 过热时禁止射击
        // 检测开火输入
        if (Input.GetButton("Fire1") && timer == 0)
        {
            timer = interval;
            Fire();
        }
        //// 校准时按住攻击键可快速冷却
        //if (inCalibrationZone)
        //{
        //    ClearOverheat();
        //    PlayCalibrationEffect(); // 播放校准成功特效
        //}
    }
    /// <summary>
    /// 处理热量系统逻辑
    /// </summary>
    void HandleHeatSystem()
    {
        // 过热惩罚计时
        if (isOverheated)
        {
            overheatTimer -= Time.deltaTime;
            if (overheatTimer <= 0)
            {
                ResetHeatSystem();
            }
            return;
        }

        // 自然冷却
        if (!Input.GetButton("Fire1") && currentHeat > 0)
        {
            currentHeat -= cooldownRate * Time.deltaTime;
            currentHeat = Mathf.Max(0, currentHeat);
        }

        //// 检查是否进入校准区域
        //float heatPercent = currentHeat / maxHeat;
        //inCalibrationZone = heatPercent >= calibrationZone.x && heatPercent <= calibrationZone.y;

        // 过热检查
        if (currentHeat >= maxHeat)
        {
            EnterOverheat();
        }
    }

    /// <summary>
    /// 执行射击逻辑
    /// </summary>
    protected virtual void Fire()
    {
        // 热量积累
        currentHeat += heatPerShot;

        // 播放射击动画
        TriggerAttackAnimation("Shoot");

        // 从对象池获取子弹
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        bullet.transform.position = muzzlePos.position;

        // 添加随机散布
        float angle = Random.Range(-5f, 5f);
        Vector2 shotDirection = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
        bullet.GetComponent<Bullet>().SetSpeed(shotDirection);

        // 播放枪口闪光等特效
        PlayMuzzleEffect();
    }

    private void UpdateHeatVisuals()
    {
        if (weaponRenderer == null) return;

        // 根据状态切换颜色
        weaponRenderer.material.color = isOverheated ? weapon_overheatColor : weapon_normalColor;
    }
    /// <summary>
    /// 进入过热状态
    /// </summary>
    void EnterOverheat()
    {
        isOverheated = true;
        overheatTimer = overheatPenaltyTime;
        currentHeat = maxHeat; // 确保UI显示满条

        // 播放过热特效
        PlayOverheatEffect();
        UpdateHeatVisuals(); // 变红
    }

    /// <summary>
    /// 重置热量系统
    /// </summary>
    void ResetHeatSystem()
    {
        isOverheated = false;
        currentHeat = 0;
        overheatTimer = 0;
        UpdateHeatVisuals(); // 恢复原色
    }

    /// <summary>
    /// 校准成功时清除过热
    /// </summary>
    void ClearOverheat()
    {
        currentHeat = 0;
        inCalibrationZone = false;
    }

    /// <summary>
    /// 更新UI显示
    /// </summary>
    void UpdateUI()
    {
        if (heatSlider == null) return;

        // 更新热量条数值
        heatSlider.value = currentHeat;

        // 根据状态改变颜色
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

    // === 特效方法 === //
    void PlayMuzzleEffect()
    {
        // 实现枪口闪光特效
        // 例如：Instantiate(muzzleFlash, muzzlePos.position, muzzlePos.rotation);
    }

    void PlayOverheatEffect()
    {
        // 实现过热特效
        // 例如：PlaySound(overheatSound);
    }

    void PlayCalibrationEffect()
    {
        // 实现校准成功特效
        // 例如：StartCoroutine(FlashIndicator());
    }
}