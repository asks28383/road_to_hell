using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Boss")]
public class BossSodaCharge : Action
{
    [Header("�������")]
    public int maxCharges = 3;           // ����̴���
    public float chargeSpeed = 15f;      // ����ٶ�
    public float chargeDuration = 0.5f;  // ���γ�̳���ʱ��
    public float postChargeDelay = 0.8f; // ÿ�γ�̺��ͣ��ʱ��

    [Header("ճҺ�ۼ�")]
    public GameObject sodaTrailPrefab;   // ճҺ�ۼ�Ԥ����
    public float trailSpawnInterval = 0.1f; // ճҺ���ɼ��

    private int currentCharges;          // ��ǰ��̴���
    private float chargeTimer;           // ��̼�ʱ��
    private float delayTimer;            // ͣ�ټ�ʱ��
    private Vector3 chargeDirection;     // ��ǰ��̷���
    private float lastTrailSpawnTime;    // �ϴ�����ճҺʱ��
    private Transform player;            // ��Ҳο�
    private bool isCharging;             // �Ƿ����ڳ��

    public override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Ԥ�ȶ����
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
        // ���㳯����ҵķ��򣨺���Y�ᣩ
        chargeDirection = (player.position - transform.position).normalized;

        GetComponent<Animator>().SetTrigger("ChargeWindup"); // ������������
        delayTimer = postChargeDelay;
    }

    public override TaskStatus OnUpdate()
    {
        if (currentCharges >= maxCharges)
            return TaskStatus.Running;

        // ͣ�ٽ׶�
        if (!isCharging)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0)
            {
                StartCharging();
            }
        }
        // ��̽׶�
        else
        {
            chargeTimer += Time.deltaTime;
            transform.position += chargeDirection * chargeSpeed * Time.deltaTime;

            // ����ճҺ�ۼ�
            if (Time.time - lastTrailSpawnTime > trailSpawnInterval)
            {
                SpawnTrail();
                lastTrailSpawnTime = Time.time;
            }

            // ��̽������
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
            PrepareNextCharge(); // ׼����һ�γ��
        }
    }

    private void SpawnTrail()
    {
        GameObject trail = ObjectPool.Instance.GetObject(sodaTrailPrefab);
        trail.transform.position = transform.position - Vector3.up * 0.2f;
        trail.transform.rotation = Quaternion.identity;
    }

    public override void OnEnd()
    {
        // ����״̬
        GetComponent<Animator>().ResetTrigger("ChargeStart");
        GetComponent<Animator>().ResetTrigger("ChargeEnd");
    }
}