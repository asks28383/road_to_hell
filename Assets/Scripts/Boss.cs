using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("侦察设置")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float scanInterval = 0.5f;
    [SerializeField] private string playerTag = "Player";

    [Header("攻击设置")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int bulletsPerAttack = 5;
    [SerializeField] private float attackAngleRange = 120f;

    [Header("移动区域设置")]
    [SerializeField] private Rect worldMoveArea = new Rect(-10, -5, 20, 10); // x,y,width,height

    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float minMoveTime = 1f;
    [SerializeField] private float maxMoveTime = 3f;
    [SerializeField] private float idleTime = 1f;

    // 状态变量
    private Transform playerTarget;
    private bool isAttacking;
    private float nextAttackTime;
    private float nextScanTime;

    // 移动控制
    private Vector2 targetPosition;
    private bool isMoving;
    private float currentMoveTime;
    private float moveDuration;
    private float idleTimer;

    private void Start()
    {
        ClampPosition(); // 初始位置约束
        SetNewRandomTarget();
    }

    private void Update()
    {
        if (Time.time >= nextScanTime)
        {
            ScanForPlayerByTag();
            nextScanTime = Time.time + scanInterval;
        }

        if (playerTarget != null && Time.time >= nextAttackTime && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        HandleMovement();
    }

    #region 移动控制（纯随机版）
    private void HandleMovement()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                SetNewRandomTarget();
            }
        }
    }

    private void SetNewRandomTarget()
    {
        // 在区域内完全随机生成目标点
        targetPosition = new Vector2(
            Random.Range(worldMoveArea.xMin, worldMoveArea.xMax),
            Random.Range(worldMoveArea.yMin, worldMoveArea.yMax)
        );

        isMoving = true;
        moveDuration = Random.Range(minMoveTime, maxMoveTime);
        currentMoveTime = 0f;
        idleTimer = 0f;
    }

    private void MoveToTarget()
    {
        currentMoveTime += Time.deltaTime;

        if (currentMoveTime < moveDuration)
        {
            // 简单直线移动
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            isMoving = false;
        }
    }
    #endregion

    #region 区域限制
    private Vector2 ClampToArea(Vector2 position)
    {
        return new Vector2(
            Mathf.Clamp(position.x, worldMoveArea.xMin, worldMoveArea.xMax),
            Mathf.Clamp(position.y, worldMoveArea.yMin, worldMoveArea.yMax)
        );
    }

    private void ClampPosition()
    {
        transform.position = ClampToArea(transform.position);
    }
    #endregion

    #region 攻击逻辑（保持不变）
    private void ScanForPlayerByTag()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float closestDist = Mathf.Infinity;
        Transform closestPlayer = null;

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

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Vector2 baseDir = (playerTarget.position - transform.position).normalized;
        float spawnRadius = 2.5f;

        for (int i = 0; i < bulletsPerAttack; i++)
        {
            float angle = Random.Range(-attackAngleRange / 2f, attackAngleRange / 2f);
            Vector2 shootDir = Quaternion.Euler(0, 0, angle) * baseDir;

            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.position = (Vector2)transform.position + shootDir * spawnRadius;
            bullet.transform.up = shootDir;

            Bullet bulletComp = bullet.GetComponent<Bullet>();
            bulletComp.SetSpeed(shootDir);

            yield return new WaitForSeconds(0.1f);
        }

        nextAttackTime = Time.time + attackCooldown;
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
    public void SetMoveArea(Rect newArea)
    {
        worldMoveArea = newArea;
        ClampPosition();
    }

    public void SetMoveArea(Vector2 center, Vector2 size)
    {
        worldMoveArea = new Rect(center.x - size.x / 2, center.y - size.y / 2, size.x, size.y);
        ClampPosition();
    }
    #endregion
}