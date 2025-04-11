using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("�������")]
    [SerializeField] private float detectionRange = 10f; // ��Ҽ�ⷶΧ
    [SerializeField] private float scanInterval = 0.5f; // ɨ����(��)
    [SerializeField] private string playerTag = "Player"; // ��ұ�ǩ����

    [Header("��������")]
    [SerializeField] private GameObject bulletPrefab;  // �ӵ�Ԥ����
    [SerializeField] private float attackCooldown = 2f; // ������ȴʱ��
    [SerializeField] private int bulletsPerAttack = 5; // ÿ�ι��������ӵ���
    [SerializeField] private float attackAngleRange = 120f; // �����Ƕ������Χ(��)
    [SerializeField] private float bulletSpeed = 8f;   // �ӵ��ٶ�

    private Transform playerTarget;    // ���Ŀ������
    private bool isAttacking;         // �Ƿ����ڹ���
    private float nextAttackTime;     // �´οɹ���ʱ��
    private float nextScanTime;       // �´�ɨ��ʱ��

    private void Update()
    {
        // ����ɨ�����
        if (Time.time >= nextScanTime)
        {
            ScanForPlayerByTag();
            nextScanTime = Time.time + scanInterval;
        }

        // �����������Ҳ�����ȴ�ڣ�ִ�й���
        if (playerTarget != null && Time.time >= nextAttackTime && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    /// <summary>
    /// ͨ����ǩ�ڼ�ⷶΧ��ɨ�����
    /// </summary>
    private void ScanForPlayerByTag()
    {
        // ����1: ʹ��GameObject.FindWithTag (�ʺϳ�����ֻ��һ�����)
        // playerTarget = GameObject.FindWithTag(playerTag)?.transform;
        // if(playerTarget != null && Vector2.Distance(transform.position, playerTarget.position) > detectionRange)
        //     playerTarget = null;

        // ����2: �������д���ǩ�Ķ���ɸѡ����(�Ƽ�)
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
    /// ����Э��
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // �����������(ָ�����)
        Vector2 baseDirection = (playerTarget.position - transform.position).normalized;

        // �ӵ����ɰ뾶(����Boss��ײ��뾶)
        float spawnRadius = 2.5f; // ����Bossʵ�ʴ�С����

        // ���������ӵ�
        for (int i = 0; i < bulletsPerAttack; i++)
        {
            // ��������Ƕ�ƫ��(-angleRange/2 �� angleRange/2)
            float randomAngle = Random.Range(-attackAngleRange / 2f, attackAngleRange / 2f);

            // ��ת��������õ����շ��䷽��
            Vector2 shootDirection = Quaternion.Euler(0, 0, randomAngle) * baseDirection;

            // �Ӷ���ػ�ȡ�ӵ�
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);

            // �����ӵ�λ��(��Boss��Χһ����������)
            Vector2 spawnPosition = (Vector2)transform.position + shootDirection * spawnRadius;
            bullet.transform.position = spawnPosition;

            // �����ӵ���ת����
            bullet.transform.up = shootDirection;

            // ��ȡ�ӵ�����������ٶ�
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.speed = bulletSpeed;
            bulletComponent.SetSpeed(shootDirection);

            // ���ݼ�����ٷ�����һ��(��������Ч��)
            yield return new WaitForSeconds(0.1f);
        }

        // �����´ι���ʱ��
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = false;
    }

    /// <summary>
    /// �ڳ�����ͼ�л��Ƽ�ⷶΧ
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}