using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class ThrowBananaPeels : Action
{
    [Header("References")]
    public GameObject bananaPrefab;
    public Transform throwPoint;

    [Header("Settings")]
    public float throwForce = 10f;
    public float throwInterval = 3f;
    public int prewarmCount = 5;

    private float lastThrowTime;

    public override void OnStart()
    {
        // Ԥ�ȶ����
        if (!ObjectPool.Instance.HasPool(bananaPrefab.name))
        {
            ObjectPool.Instance.PrewarmPool(bananaPrefab, prewarmCount);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (Time.time - lastThrowTime >= throwInterval)
        {
            ThrowBanana();
            lastThrowTime = Time.time;
        }
        return TaskStatus.Running;
    }

    private void ThrowBanana()
    {
        // �Ӷ���ػ�ȡ�㽶Ƥ
        GameObject banana = ObjectPool.Instance.GetObject(bananaPrefab);

        // ����λ��/��ת
        banana.transform.position = throwPoint.position;
        banana.transform.rotation = Random.rotation;

        // ʩ��Ͷ����
        Rigidbody rb = banana.GetComponent<Rigidbody>();
        Vector3 randomDir = new Vector3(
            Random.Range(-0.3f, 0.3f),
            1f,
            Random.Range(-0.3f, 0.3f)
        ).normalized;
    }
}
