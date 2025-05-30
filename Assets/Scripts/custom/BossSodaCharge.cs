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
    public float preparationTime = 0.5f;     // ���ǰ׼��ʱ��(�ɲ���׼������)
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

    public override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        // Ԥ�ȶ����
        if (!ObjectPool.Instance.HasPool(sodaTrailPrefab.name))
        {
            ObjectPool.Instance.PrewarmPool(sodaTrailPrefab, 10);
        }

        currentCharges = 0;
        StartPreparation(); // ��ʼ��һ��׼��
    }

    public override TaskStatus OnUpdate()
    {
        if (currentCharges >= maxCharges)
            return TaskStatus.Success;

        // ׼���׶�
        if (isPreparing)
        {
            preparationTimer -= Time.deltaTime;
            if (preparationTimer <= 0)
            {
                // ��׼���׶ν���ʱ��ȷ����̷���
                DetermineChargeDirection();
                StartCharging();
            }
        }
        // ��̽׶�
        else if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            // ʹ�ø����ƶ�(������ײ���)
            rb.velocity = chargeDirection * chargeSpeed;

            // ����ճҺ�ۼ�
            if (Time.time - lastTrailSpawnTime > trailSpawnInterval)
            {
                SpawnTrail();
                lastTrailSpawnTime = Time.time;
            }

            // ���ʱ�����������ǽ�ڶ���ֹͣ���
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
                StartPreparation(); // ��ʼ��һ��׼��
            }
        }

        return TaskStatus.Running;
    }

    // ��ʼ׼���׶�
    private void StartPreparation()
    {
        isPreparing = true;
        isCharging = false;
        preparationTimer = preparationTime;
        rb.velocity = Vector2.zero;

        // ����׼������
        GetComponent<Animator>().SetTrigger("ChargeWindup");
    }

    // ��׼���׶ν���ʱȷ����̷���
    private void DetermineChargeDirection()
    {
        // ���㵱ǰʱ�̳�����ҵķ���(����Y��)
        Vector3 toPlayer = player.position - transform.position;
        chargeDirection = new Vector3(toPlayer.x, toPlayer.y, 0).normalized;
    }

    // ��ʼ���
    private void StartCharging()
    {
        isPreparing = false;
        isCharging = true;
        chargeTimer = 0;
        currentCharges++;

        // ���ų�̶���
        GetComponent<Animator>().SetTrigger("ChargeStart");
    }

    // �������
    private void EndCharging()
    {
        isCharging = false;
        rb.velocity = Vector2.zero;

        // ���Ž�������
        GetComponent<Animator>().SetTrigger("ChargeEnd");

        // ����������һ�γ�̣���ʼ��Ϣ��ʱ
        if (currentCharges < maxCharges)
        {
            delayTimer = postChargeDelay;
        }
    }

    // ����ճҺ�ۼ�
    private void SpawnTrail()
    {
        GameObject trail = ObjectPool.Instance.GetObject(sodaTrailPrefab);
        trail.transform.position = transform.position;

        // 2D�������(ʹ��Z����Ϊ���ӽǵ�"�Ϸ���")
        Vector2 direction2D = new Vector2(chargeDirection.x, chargeDirection.y);
        if (direction2D != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction2D.y, direction2D.x) * Mathf.Rad2Deg;
            trail.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        StartCoroutine(ReturnTrailAfterDelay(trail, 10f));
    }

    // �ӳٻ���ճҺ�ۼ�
    private IEnumerator ReturnTrailAfterDelay(GameObject trail, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trail != null && trail.activeInHierarchy)
        {
            ObjectPool.Instance.PushObject(trail);
        }
    }

    // ��ײ���(����ǽ��ֹͣ���)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging && collision.gameObject.CompareTag("Wall"))
        {
            EndCharging();
        }
    }

    public override void OnEnd()
    {
        // ����״̬
        rb.velocity = Vector2.zero;
        GetComponent<Animator>().ResetTrigger("ChargeStart");
        GetComponent<Animator>().ResetTrigger("ChargeEnd");
        GetComponent<Animator>().ResetTrigger("ChargeWindup");
    }
}