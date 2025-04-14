using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("�������")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float scanInterval = 0.5f;
    [SerializeField] private string playerTag = "Player";

    [Header("��������")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int bulletsPerAttack = 5;
    [SerializeField] private float attackAngleRange = 120f;

    [Header("�ƶ���������")]
    [SerializeField] private Rect worldMoveArea = new Rect(-10, -5, 20, 10); // x,y,width,height

    [Header("�ƶ�����")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float minMoveTime = 1f;
    [SerializeField] private float maxMoveTime = 3f;
    [SerializeField] private float idleTime = 1f;

    // ״̬����
    private Transform playerTarget;
    private bool isAttacking;
    private float nextAttackTime;
    private float nextScanTime;

    // �ƶ�����
    private Vector2 targetPosition;
    private bool isMoving;
    private float currentMoveTime;
    private float moveDuration;
    private float idleTimer;

    private void Start()
    {
        ClampPosition(); // ��ʼλ��Լ��
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

    #region �ƶ����ƣ�������棩
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
        // ����������ȫ�������Ŀ���
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
            // ��ֱ���ƶ�
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

    #region ��������
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

    #region �����߼������ֲ��䣩
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

    #region ���Ի���
    private void OnDrawGizmosSelected()
    {
        // �����ƶ����򣨺�ɫ�߿�
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Vector3 center = new Vector3(worldMoveArea.center.x, worldMoveArea.center.y, 0);
        Vector3 size = new Vector3(worldMoveArea.width, worldMoveArea.height, 0.1f);
        Gizmos.DrawWireCube(center, size);

        // ���Ƽ�ⷶΧ����ɫԲȦ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    #endregion

    #region ��������
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