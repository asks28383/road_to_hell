using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    // ========== Inspector �ɵ����� ==========
    [Header("�������")]
    [SerializeField] private float detectionRange = 10f; // �����ҵķ�Χ�뾶
    [SerializeField] private float scanInterval = 0.5f; // ɨ����ҵ�ʱ����(��)
    [SerializeField] private string playerTag = "Player"; // ��Ҷ���ı�ǩ

    [Header("��������")]
    [SerializeField] private GameObject bulletPrefab; // �ӵ�Ԥ����
    [SerializeField] private float attackCooldown = 2f; // ������ȴʱ��
    [SerializeField] private int bulletsPerAttack = 5; // ÿ�ι���������ӵ�����
    [SerializeField] private float attackAngleRange = 120f; // �ӵ�ɢ��Ƕȷ�Χ(�ܽǶ�)

    [Header("�ƶ���������")]
    [SerializeField] private Rect worldMoveArea = new Rect(-10, -5, 20, 10); // Boss���ƶ��ľ�������(x,y,width,height)

    [Header("�ƶ�����")]
    [SerializeField] private float moveSpeed = 3f; // �ƶ��ٶ�
    [SerializeField] private float minMoveTime = 1f; // ��С�ƶ�����ʱ��
    [SerializeField] private float maxMoveTime = 3f; // ����ƶ�����ʱ��
    [SerializeField] private float idleTime = 1f; // ����ͣ��ʱ��

    // ========== ״̬���� ==========
    private Transform playerTarget; // ��ǰ���������Ŀ��
    private bool isAttacking; // �Ƿ����ڹ�����
    private float nextAttackTime; // �´οɹ���ʱ��
    private float nextScanTime; // �´�ɨ��ʱ��

    // ========== �ƶ����Ʊ��� ==========
    private Vector2 targetPosition; // ��ǰ�ƶ�Ŀ��λ��
    private bool isMoving; // �Ƿ������ƶ���
    private float currentMoveTime; // ��ǰ�ƶ��ѳ�����ʱ��
    private float moveDuration; // �����ƶ����ܳ���ʱ��
    private float idleTimer; // ���м�ʱ��

    private void Start()
    {
        ClampPosition(); // ȷ����ʼλ�����ƶ�������
        SetNewRandomTarget(); // ���õ�һ������ƶ�Ŀ��
    }

    private void Update()
    {
        // ����ɨ�����
        if (Time.time >= nextScanTime)
        {
            ScanForPlayerByTag();
            nextScanTime = Time.time + scanInterval;
        }

        //// �����������Ҳ�����ȴ�У���ʼ����
        //if (playerTarget != null && Time.time >= nextAttackTime && !isAttacking)
        //{
        //    StartCoroutine(AttackRoutine());
        //}

        HandleMovement(); // �����ƶ��߼�
    }

    #region �ƶ����ƣ�������棩
    /// <summary>
    /// ����Boss���ƶ�״̬
    /// </summary>
    private void HandleMovement()
    {
        if (isMoving)
        {
            MoveToTarget(); // �����ƶ���
        }
        else
        {
            // ����״̬��ʱ
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                SetNewRandomTarget(); // ����ʱ�������������Ŀ��
            }
        }
    }

    /// <summary>
    /// ����һ���µ�����ƶ�Ŀ��λ��
    /// </summary>
    private void SetNewRandomTarget()
    {
        // ���ƶ��������������һ��Ŀ���
        targetPosition = new Vector2(
            Random.Range(worldMoveArea.xMin, worldMoveArea.xMax),
            Random.Range(worldMoveArea.yMin, worldMoveArea.yMax)
        );

        isMoving = true;
        moveDuration = Random.Range(minMoveTime, maxMoveTime); // ����ƶ�����ʱ��
        currentMoveTime = 0f;
        idleTimer = 0f; // ���ÿ��м�ʱ��
    }

    /// <summary>
    /// ��Ŀ��λ���ƶ�
    /// </summary>
    private void MoveToTarget()
    {
        currentMoveTime += Time.deltaTime;

        if (currentMoveTime < moveDuration)
        {
            // ʹ��MoveTowards����ƽ���ƶ�
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            isMoving = false; // �ƶ�ʱ��������������״̬
        }
    }
    #endregion

    #region ��������
    /// <summary>
    /// ��λ���������ƶ�������
    /// </summary>
    private Vector2 ClampToArea(Vector2 position)
    {
        return new Vector2(
            Mathf.Clamp(position.x, worldMoveArea.xMin, worldMoveArea.xMax),
            Mathf.Clamp(position.y, worldMoveArea.yMin, worldMoveArea.yMax)
        );
    }

    /// <summary>
    /// ������ǰλ�õ��ƶ�������
    /// </summary>
    private void ClampPosition()
    {
        transform.position = ClampToArea(transform.position);
    }
    #endregion

    #region �����߼�
    /// <summary>
    /// ɨ�跶Χ�ڵ����Ŀ��
    /// </summary>
    private void ScanForPlayerByTag()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float closestDist = Mathf.Infinity;
        Transform closestPlayer = null;

        // �ҳ���Χ����������
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
    /// ����Э�̣�����ɢ���ӵ�
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Vector2 baseDir = (playerTarget.position - transform.position).normalized; // ��������(�������)
        float spawnRadius = 2.5f; // �ӵ����ɰ뾶

        // ����ָ���������ӵ�
        for (int i = 0; i < bulletsPerAttack; i++)
        {
            // ����ɢ��Ƕ�
            float angle = Random.Range(-attackAngleRange / 2f, attackAngleRange / 2f);
            Vector2 shootDir = Quaternion.Euler(0, 0, angle) * baseDir;

            // �Ӷ���ػ�ȡ�ӵ�������λ�úͷ���
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.position = (Vector2)transform.position + shootDir * spawnRadius;
            bullet.transform.up = shootDir;

            // �����ӵ��ƶ�����
            Bullet bulletComp = bullet.GetComponent<Bullet>();
            bulletComp.SetSpeed(shootDir);

            yield return new WaitForSeconds(0.1f); // �ӵ�������
        }

        nextAttackTime = Time.time + attackCooldown; // �����´ι���ʱ��
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
    /// <summary>
    /// ����Boss���ƶ�����
    /// </summary>
    public void SetMoveArea(Rect newArea)
    {
        worldMoveArea = newArea;
        ClampPosition(); // ȷ����ǰλ������������
    }

    /// <summary>
    /// ����Boss���ƶ�����(ͨ�����ĺʹ�С)
    /// </summary>
    public void SetMoveArea(Vector2 center, Vector2 size)
    {
        worldMoveArea = new Rect(center.x - size.x / 2, center.y - size.y / 2, size.x, size.y);
        ClampPosition();
    }
    #endregion
}