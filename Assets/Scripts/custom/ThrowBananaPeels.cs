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
        // 预热对象池
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
        // 从对象池获取香蕉皮
        GameObject banana = ObjectPool.Instance.GetObject(bananaPrefab);

        // 设置位置/旋转
        banana.transform.position = throwPoint.position;
        banana.transform.rotation = Random.rotation;

        // 施加投掷力
        Rigidbody rb = banana.GetComponent<Rigidbody>();
        Vector3 randomDir = new Vector3(
            Random.Range(-0.3f, 0.3f),
            1f,
            Random.Range(-0.3f, 0.3f)
        ).normalized;
    }
}
