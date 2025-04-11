using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("侦察设置")]
    [SerializeField] private float detectionRange = 10f; // 玩家检测范围
    [SerializeField] private float scanInterval = 0.5f; // 扫描间隔(秒)
    [SerializeField] private string playerTag = "Player"; // 玩家标签名称

    [Header("攻击设置")]
    [SerializeField] private GameObject bulletPrefab;  // 子弹预制体
    [SerializeField] private float attackCooldown = 2f; // 攻击冷却时间
    [SerializeField] private int bulletsPerAttack = 5; // 每次攻击发射子弹数
    [SerializeField] private float attackAngleRange = 120f; // 攻击角度随机范围(度)
    [SerializeField] private float bulletSpeed = 8f;   // 子弹速度

    private Transform playerTarget;    // 玩家目标引用
    private bool isAttacking;         // 是否正在攻击
    private float nextAttackTime;     // 下次可攻击时间
    private float nextScanTime;       // 下次扫描时间

    private void Update()
    {
        // 定期扫描玩家
        if (Time.time >= nextScanTime)
        {
            ScanForPlayerByTag();
            nextScanTime = Time.time + scanInterval;
        }

        // 如果发现玩家且不在冷却期，执行攻击
        if (playerTarget != null && Time.time >= nextAttackTime && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    /// <summary>
    /// 通过标签在检测范围内扫描玩家
    /// </summary>
    private void ScanForPlayerByTag()
    {
        // 方法1: 使用GameObject.FindWithTag (适合场景中只有一个玩家)
        // playerTarget = GameObject.FindWithTag(playerTag)?.transform;
        // if(playerTarget != null && Vector2.Distance(transform.position, playerTarget.position) > detectionRange)
        //     playerTarget = null;

        // 方法2: 查找所有带标签的对象并筛选距离(推荐)
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= detectionRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
            }
        }

        playerTarget = closestPlayer;
    }

    /// <summary>
    /// 攻击协程
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 计算基础方向(指向玩家)
        Vector2 baseDirection = (playerTarget.position - transform.position).normalized;

        // 子弹生成半径(大于Boss碰撞体半径)
        float spawnRadius = 2.5f; // 根据Boss实际大小调整

        // 发射所有子弹
        for (int i = 0; i < bulletsPerAttack; i++)
        {
            // 计算随机角度偏移(-angleRange/2 到 angleRange/2)
            float randomAngle = Random.Range(-attackAngleRange / 2f, attackAngleRange / 2f);

            // 旋转基础方向得到最终发射方向
            Vector2 shootDirection = Quaternion.Euler(0, 0, randomAngle) * baseDirection;

            // 从对象池获取子弹
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);

            // 设置子弹位置(在Boss周围一定距离生成)
            Vector2 spawnPosition = (Vector2)transform.position + shootDirection * spawnRadius;
            bullet.transform.position = spawnPosition;

            // 设置子弹旋转方向
            bullet.transform.up = shootDirection;

            // 获取子弹组件并设置速度
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.speed = bulletSpeed;
            bulletComponent.SetSpeed(shootDirection);

            // 短暂间隔后再发射下一发(创建扇形效果)
            yield return new WaitForSeconds(0.1f);
        }

        // 设置下次攻击时间
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = false;
    }

    /// <summary>
    /// 在场景视图中绘制检测范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}