using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class BossSodaCharge : Action
{
    [Header("冲刺设置")]
    public int maxCharges = 3;               // 最大冲刺次数
    public float chargeSpeed = 15f;          // 冲刺速度
    public float chargeDuration = 0.5f;      // 单次冲刺持续时间
    public float preparationTime = 0.5f;     // 冲刺前准备时间(可播放准备动画)
    public float postChargeDelay = 0.8f;     // 每次冲刺后的休息时间

    [Header("粘液痕迹")]
    public GameObject sodaTrailPrefab;       // 粘液痕迹预制体
    public float trailSpawnInterval = 0.1f;  // 粘液生成间隔

    private int currentCharges;              // 当前冲刺次数
    private float chargeTimer;               // 冲刺计时器
    private float preparationTimer;          // 准备阶段计时器
    private float delayTimer;                // 休息计时器
    private Vector3 chargeDirection;         // 冲刺方向
    private float lastTrailSpawnTime;        // 上次生成粘液时间
    private Transform player;                // 玩家参考
    private bool isPreparing;                // 是否在准备阶段
    private bool isCharging;                 // 是否正在冲刺
    private Rigidbody2D rb;                  // 刚体组件

    public override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        // 预热对象池
        if (!ObjectPool.Instance.HasPool(sodaTrailPrefab.name))
        {
            ObjectPool.Instance.PrewarmPool(sodaTrailPrefab, 10);
        }

        currentCharges = 0;
        StartPreparation(); // 开始第一次准备
    }

    public override TaskStatus OnUpdate()
    {
        if (currentCharges >= maxCharges)
            return TaskStatus.Success;

        // 准备阶段
        if (isPreparing)
        {
            preparationTimer -= Time.deltaTime;
            if (preparationTimer <= 0)
            {
                // 在准备阶段结束时才确定冲刺方向
                DetermineChargeDirection();
                StartCharging();
            }
        }
        // 冲刺阶段
        else if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            // 使用刚体移动(便于碰撞检测)
            rb.velocity = chargeDirection * chargeSpeed;

            // 生成粘液痕迹
            if (Time.time - lastTrailSpawnTime > trailSpawnInterval)
            {
                SpawnTrail();
                lastTrailSpawnTime = Time.time;
            }

            // 冲刺时间结束或碰到墙壁都会停止冲刺
            if (chargeTimer >= chargeDuration)
            {
                EndCharging();
            }
        }
        // 休息阶段
        else
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0 && currentCharges < maxCharges)
            {
                StartPreparation(); // 开始下一次准备
            }
        }

        return TaskStatus.Running;
    }

    // 开始准备阶段
    private void StartPreparation()
    {
        isPreparing = true;
        isCharging = false;
        preparationTimer = preparationTime;
        rb.velocity = Vector2.zero;

        // 播放准备动画
        GetComponent<Animator>().SetTrigger("ChargeWindup");
    }

    // 在准备阶段结束时确定冲刺方向
    private void DetermineChargeDirection()
    {
        // 计算当前时刻朝向玩家的方向(忽略Y轴)
        Vector3 toPlayer = player.position - transform.position;
        chargeDirection = new Vector3(toPlayer.x, toPlayer.y, 0).normalized;
    }

    // 开始冲刺
    private void StartCharging()
    {
        isPreparing = false;
        isCharging = true;
        chargeTimer = 0;
        currentCharges++;

        // 播放冲刺动画
        GetComponent<Animator>().SetTrigger("ChargeStart");
    }

    // 结束冲刺
    private void EndCharging()
    {
        isCharging = false;
        rb.velocity = Vector2.zero;

        // 播放结束动画
        GetComponent<Animator>().SetTrigger("ChargeEnd");

        // 如果不是最后一次冲刺，则开始休息计时
        if (currentCharges < maxCharges)
        {
            delayTimer = postChargeDelay;
        }
    }

    // 生成粘液痕迹
    private void SpawnTrail()
    {
        GameObject trail = ObjectPool.Instance.GetObject(sodaTrailPrefab);
        trail.transform.position = transform.position;

        // 2D方向计算(使用Z轴作为俯视角的"上方向")
        Vector2 direction2D = new Vector2(chargeDirection.x, chargeDirection.y);
        if (direction2D != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction2D.y, direction2D.x) * Mathf.Rad2Deg;
            trail.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        StartCoroutine(ReturnTrailAfterDelay(trail, 10f));
    }

    // 延迟回收粘液痕迹
    private IEnumerator ReturnTrailAfterDelay(GameObject trail, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trail != null && trail.activeInHierarchy)
        {
            ObjectPool.Instance.PushObject(trail);
        }
    }

    // 碰撞检测(碰到墙壁停止冲刺)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging && collision.gameObject.CompareTag("Wall"))
        {
            EndCharging();
        }
    }

    public override void OnEnd()
    {
        // 重置状态
        rb.velocity = Vector2.zero;
        GetComponent<Animator>().ResetTrigger("ChargeStart");
        GetComponent<Animator>().ResetTrigger("ChargeEnd");
        GetComponent<Animator>().ResetTrigger("ChargeWindup");
    }
}