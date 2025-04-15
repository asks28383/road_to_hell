using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    // ========== Inspector 可调参数 ==========
    [Header("侦察设置")]
    [SerializeField] private float detectionRange = 10f; // 检测玩家的范围半径
    [SerializeField] private float scanInterval = 0.5f; // 扫描玩家的时间间隔(秒)
    [SerializeField] private string playerTag = "Player"; // 玩家对象的标签

    [Header("攻击设置")]
    [SerializeField] private GameObject bulletPrefab; // 子弹预制体
    [SerializeField] private float attackCooldown = 2f; // 攻击冷却时间
    [SerializeField] private int bulletsPerAttack = 5; // 每次攻击发射的子弹数量
    [SerializeField] private float attackAngleRange = 120f; // 子弹散射角度范围(总角度)

    [Header("移动区域设置")]
    [SerializeField] private Rect worldMoveArea = new Rect(-10, -5, 20, 10); // Boss可移动的矩形区域(x,y,width,height)

    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 3f; // 移动速度
    [SerializeField] private float minMoveTime = 1f; // 最小移动持续时间
    [SerializeField] private float maxMoveTime = 3f; // 最大移动持续时间
    [SerializeField] private float idleTime = 1f; // 空闲停留时间

    // ========== 状态变量 ==========
    private Transform playerTarget; // 当前锁定的玩家目标
    private bool isAttacking; // 是否正在攻击中
    private float nextAttackTime; // 下次可攻击时间
    private float nextScanTime; // 下次扫描时间

    // ========== 移动控制变量 ==========
    private Vector2 targetPosition; // 当前移动目标位置
    private bool isMoving; // 是否正在移动中
    private float currentMoveTime; // 当前移动已持续的时间
    private float moveDuration; // 本次移动的总持续时间
    private float idleTimer; // 空闲计时器

    private void Start()
    {
        ClampPosition(); // 确保初始位置在移动区域内
        SetNewRandomTarget(); // 设置第一个随机移动目标
    }

    private void Update()
    {
        // 定期扫描玩家
        if (Time.time >= nextScanTime)
        {
            ScanForPlayerByTag();
            nextScanTime = Time.time + scanInterval;
        }

        //// 如果发现玩家且不在冷却中，开始攻击
        //if (playerTarget != null && Time.time >= nextAttackTime && !isAttacking)
        //{
        //    StartCoroutine(AttackRoutine());
        //}

        HandleMovement(); // 处理移动逻辑
    }

    #region 移动控制（纯随机版）
    /// <summary>
    /// 处理Boss的移动状态
    /// </summary>
    private void HandleMovement()
    {
        if (isMoving)
        {
            MoveToTarget(); // 正在移动中
        }
        else
        {
            // 空闲状态计时
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                SetNewRandomTarget(); // 空闲时间结束，设置新目标
            }
        }
    }

    /// <summary>
    /// 设置一个新的随机移动目标位置
    /// </summary>
    private void SetNewRandomTarget()
    {
        // 在移动区域内随机生成一个目标点
        targetPosition = new Vector2(
            Random.Range(worldMoveArea.xMin, worldMoveArea.xMax),
            Random.Range(worldMoveArea.yMin, worldMoveArea.yMax)
        );

        isMoving = true;
        moveDuration = Random.Range(minMoveTime, maxMoveTime); // 随机移动持续时间
        currentMoveTime = 0f;
        idleTimer = 0f; // 重置空闲计时器
    }

    /// <summary>
    /// 向目标位置移动
    /// </summary>
    private void MoveToTarget()
    {
        currentMoveTime += Time.deltaTime;

        if (currentMoveTime < moveDuration)
        {
            // 使用MoveTowards进行平滑移动
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            isMoving = false; // 移动时间结束，进入空闲状态
        }
    }
    #endregion

    #region 区域限制
    /// <summary>
    /// 将位置限制在移动区域内
    /// </summary>
    private Vector2 ClampToArea(Vector2 position)
    {
        return new Vector2(
            Mathf.Clamp(position.x, worldMoveArea.xMin, worldMoveArea.xMax),
            Mathf.Clamp(position.y, worldMoveArea.yMin, worldMoveArea.yMax)
        );
    }

    /// <summary>
    /// 修正当前位置到移动区域内
    /// </summary>
    private void ClampPosition()
    {
        transform.position = ClampToArea(transform.position);
    }
    #endregion

    #region 攻击逻辑
    /// <summary>
    /// 扫描范围内的玩家目标
    /// </summary>
    private void ScanForPlayerByTag()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float closestDist = Mathf.Infinity;
        Transform closestPlayer = null;

        // 找出范围内最近的玩家
        foreach (GameObject player in players)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist <= detectionRange && dist < closestDist)
            {
                closestDist = dist;
                closestPlayer = player.transform;
            }
        }

        playerTarget = closestPlayer;
    }

    /// <summary>
    /// 攻击协程，发射散射子弹
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Vector2 baseDir = (playerTarget.position - transform.position).normalized; // 基础方向(朝向玩家)
        float spawnRadius = 2.5f; // 子弹生成半径

        // 发射指定数量的子弹
        for (int i = 0; i < bulletsPerAttack; i++)
        {
            // 计算散射角度
            float angle = Random.Range(-attackAngleRange / 2f, attackAngleRange / 2f);
            Vector2 shootDir = Quaternion.Euler(0, 0, angle) * baseDir;

            // 从对象池获取子弹并设置位置和方向
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.position = (Vector2)transform.position + shootDir * spawnRadius;
            bullet.transform.up = shootDir;

            // 设置子弹移动方向
            Bullet bulletComp = bullet.GetComponent<Bullet>();
            bulletComp.SetSpeed(shootDir);

            yield return new WaitForSeconds(0.1f); // 子弹发射间隔
        }

        nextAttackTime = Time.time + attackCooldown; // 设置下次攻击时间
        isAttacking = false;
    }
    #endregion

    #region 调试绘制
    private void OnDrawGizmosSelected()
    {
        // 绘制移动区域（红色线框）
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Vector3 center = new Vector3(worldMoveArea.center.x, worldMoveArea.center.y, 0);
        Vector3 size = new Vector3(worldMoveArea.width, worldMoveArea.height, 0.1f);
        Gizmos.DrawWireCube(center, size);

        // 绘制检测范围（黄色圆圈）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 设置Boss的移动区域
    /// </summary>
    public void SetMoveArea(Rect newArea)
    {
        worldMoveArea = newArea;
        ClampPosition(); // 确保当前位置在新区域内
    }

    /// <summary>
    /// 设置Boss的移动区域(通过中心和大小)
    /// </summary>
    public void SetMoveArea(Vector2 center, Vector2 size)
    {
        worldMoveArea = new Rect(center.x - size.x / 2, center.y - size.y / 2, size.x, size.y);
        ClampPosition();
    }
    #endregion
}