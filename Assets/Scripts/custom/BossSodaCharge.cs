using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class BossSodaCharge : Action
{

    [Header("�������")]
    public int maxCharges = 3;               // ����̴���
    public float chargeSpeed = 15f;          // ����ٶ�
    public float chargeDuration = 0.5f;      // ���γ�̳���ʱ��
    public float preparationTime = 0.5f;     // ���ǰ׼��ʱ��
    public float postChargeDelay = 0.8f;     // ÿ�γ�̺����Ϣʱ��

    [Header("ճҺ�ۼ�")]
    public GameObject sodaTrailPrefab;       // ճҺ�ۼ�Ԥ����
    public float trailSpawnInterval = 0.1f;  // ճҺ���ɼ��

    private int currentCharges;              // ��ǰ��̴���
    private float chargeTimer;               // ��̼�ʱ��
    private float preparationTimer;          // ׼���׶μ�ʱ��
    private float delayTimer;                // ��Ϣ��ʱ��
    private Vector3 chargeDirection;         // ��̷���
    private float lastTrailSpawnTime;        // �ϴ�����ճҺʱ��
    private Transform player;                // ��Ҳο�
    private bool isPreparing;                // �Ƿ���׼���׶�
    private bool isCharging;                 // �Ƿ����ڳ��
    private Rigidbody2D rb;                  // �������
    private Animator animator;               // ����������
    private const string IsDashingParam = "isDashing"; // ����������

    public override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Ԥ�ȶ����
        if (!ObjectPool.Instance.HasPool(sodaTrailPrefab.name))
        {
            ObjectPool.Instance.PrewarmPool(sodaTrailPrefab, 10);
        }

        currentCharges = 0;
        StartPreparation();
    }

    public override TaskStatus OnUpdate()
    {
        //if (currentCharges >= maxCharges)
        //    return TaskStatus.Success;

        // ׼���׶�
        if (isPreparing)
        {
            preparationTimer -= Time.deltaTime;
            if (preparationTimer <= 0)
            {
                DetermineChargeDirection();
                StartCharging();
            }
        }
        // ��̽׶�
        else if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            rb.velocity = chargeDirection * chargeSpeed;

            // ����ճҺ�ۼ�
            if (Time.time - lastTrailSpawnTime > trailSpawnInterval)
            {
                SpawnTrail();
                lastTrailSpawnTime = Time.time;
            }

            // ��̽�������
            if (chargeTimer >= chargeDuration)
            {
                EndCharging();
            }
        }
        // ��Ϣ�׶�
        else
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0 && currentCharges < maxCharges)
            {
                StartPreparation();
            }
        }

        return TaskStatus.Running;
    }

    private void StartPreparation()
    {
        isPreparing = true;
        isCharging = false;
        preparationTimer = preparationTime;
        rb.velocity = Vector2.zero;
        animator.SetBool(IsDashingParam, false); // ȷ����̶����ر�
    }

    private void DetermineChargeDirection()
    {
        Vector3 toPlayer = player.position - transform.position;
        chargeDirection = new Vector3(toPlayer.x, toPlayer.y, 0).normalized;
    }

    private void StartCharging()
    {
        isPreparing = false;
        isCharging = true;
        chargeTimer = 0;
        currentCharges++;
        animator.SetBool(IsDashingParam, true); // ��ʼ��̶���
    }

    private void EndCharging()
    {
        isCharging = false;
        rb.velocity = Vector2.zero;
        animator.SetBool(IsDashingParam, false); // ������̶���

        if (currentCharges < maxCharges)
        {
            delayTimer = postChargeDelay;
        }
    }

    private void SpawnTrail()
    {
        GameObject trail = ObjectPool.Instance.GetObject(sodaTrailPrefab);
        trail.transform.position = transform.position;

        Vector2 direction2D = new Vector2(chargeDirection.x, chargeDirection.y);
        if (direction2D != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction2D.y, direction2D.x) * Mathf.Rad2Deg;
            trail.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        StartCoroutine(ReturnTrailAfterDelay(trail, 10f));
    }

    private IEnumerator ReturnTrailAfterDelay(GameObject trail, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trail != null && trail.activeInHierarchy)
        {
            ObjectPool.Instance.PushObject(trail);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging && collision.gameObject.CompareTag("Wall"))
        {
            EndCharging();
        }
    }

    public override void OnEnd()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool(IsDashingParam, false); // ȷ������״̬����
    }
}