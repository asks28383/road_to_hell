using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class BossSodaCharge : Action
{
    [Header("冲刺设置")]
    public int maxCharges = 3;           // 最大冲刺次数
    public float chargeSpeed = 15f;      // 冲刺速度
    public float chargeDuration = 0.5f;  // 单次冲刺持续时间
    public float postChargeDelay = 0.8f; // 每次冲刺后的停顿时间

    [Header("粘液痕迹")]
    public GameObject sodaTrailPrefab;   // 粘液痕迹预制体
    public float trailSpawnInterval = 0.1f; // 粘液生成间隔

    private int currentCharges;          // 当前冲刺次数
    private float chargeTimer;           // 冲刺计时器
    private float delayTimer;            // 停顿计时器
    private Vector3 chargeDirection;     // 当前冲刺方向
    private float lastTrailSpawnTime;    // 上次生成粘液时间
    private Transform player;            // 玩家参考
    private bool isCharging;             // 是否正在冲刺

    public override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 预热对象池
        if (!ObjectPool.Instance.HasPool(sodaTrailPrefab.name))
        {
            ObjectPool.Instance.PrewarmPool(sodaTrailPrefab, 10);
        }

        currentCharges = 0;
        isCharging = false;
        PrepareNextCharge();
    }

    private void PrepareNextCharge()
    {
        // 计算朝向玩家的方向（忽略Y轴）
        chargeDirection = (player.position - transform.position).normalized;

        GetComponent<Animator>().SetTrigger("ChargeWindup"); // 播放蓄力动画
        delayTimer = postChargeDelay;
    }

    public override TaskStatus OnUpdate()
    {
        if (currentCharges >= maxCharges)
            return TaskStatus.Running;

        // 停顿阶段
        if (!isCharging)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0)
            {
                StartCharging();
            }
        }
        // 冲刺阶段
        else
        {
            chargeTimer += Time.deltaTime;
            transform.position += chargeDirection * chargeSpeed * Time.deltaTime;

            // 生成粘液痕迹
            if (Time.time - lastTrailSpawnTime > trailSpawnInterval)
            {
                SpawnTrail();
                lastTrailSpawnTime = Time.time;
            }

            // 冲刺结束检测
            if (chargeTimer >= chargeDuration)
            {
                EndCharging();
            }
        }

        return TaskStatus.Running;
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeTimer = 0;
        currentCharges++;
        GetComponent<Animator>().SetTrigger("ChargeStart");
    }

    private void EndCharging()
    {
        isCharging = false;
        GetComponent<Animator>().SetTrigger("ChargeEnd");

        if (currentCharges < maxCharges)
        {
            PrepareNextCharge(); // 准备下一次冲刺
        }
    }

    private void SpawnTrail()
    {
        GameObject trail = ObjectPool.Instance.GetObject(sodaTrailPrefab);
        trail.transform.position = transform.position;

        // 2D方向计算（使用Z轴作为俯视角的"上方向"）
        Vector2 direction2D = new Vector2(chargeDirection.x, chargeDirection.y);
        if (direction2D != Vector2.zero)
        {
            // 计算角度并旋转（注意Unity的2D旋转是绕Z轴）
            float angle = Mathf.Atan2(direction2D.y, direction2D.x) * Mathf.Rad2Deg;
            trail.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        StartCoroutine(ReturnTrailAfterDelay(trail, 3f));
    }
    private IEnumerator ReturnTrailAfterDelay(GameObject trail, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 安全回收检查
        if (trail != null && trail.activeInHierarchy)
        {
            ObjectPool.Instance.PushObject(trail);
        }
    }
    public override void OnEnd()
    {
        // 清理状态
        GetComponent<Animator>().ResetTrigger("ChargeStart");
        GetComponent<Animator>().ResetTrigger("ChargeEnd");
    }
}