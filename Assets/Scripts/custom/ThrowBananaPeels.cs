using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class ThrowBananaPeels : Action
{
    [Header("投掷设置")]
    public GameObject bananaPrefab;  // 香蕉皮预制体
    public GameObject areaPrefab;   // 区域预制体
    public float throwSpeed = 10f;  // 投掷速度
    public float throwInterval = 0.5f; // 发射间隔时间
    public int totalThrows = 5;     // 总共发射次数
    public float areaDuration = 5f; // 区域持续时间

    private Transform player;
    private float throwTimer;
    private int throwCount;
    private bool isThrowing;

    public override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        throwTimer = 0f;
        throwCount = 0;
        isThrowing = true;

        // 预加载对象池
        if (!ObjectPool.Instance.HasPool(bananaPrefab.name))
            ObjectPool.Instance.PrewarmPool(bananaPrefab, totalThrows + 2);
        if (!ObjectPool.Instance.HasPool(areaPrefab.name))
            ObjectPool.Instance.PrewarmPool(areaPrefab, totalThrows + 2);
    }

    public override TaskStatus OnUpdate()
    {
        if (!isThrowing) return TaskStatus.Success;

        throwTimer += Time.deltaTime;

        // 达到发射间隔且还有剩余发射次数
        if (throwTimer >= throwInterval && throwCount < totalThrows)
        {
            // 获取当前帧的玩家位置（不预测）
            Vector3 targetPos = player.position;
            ThrowBanana(targetPos);
            throwTimer = 0f;
            throwCount++;
        }

        // 检查是否完成所有发射
        if (throwCount >= totalThrows)
        {
            isThrowing = false;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private void ThrowBanana(Vector3 targetPos)
    {
        GameObject banana = ObjectPool.Instance.GetObject(bananaPrefab);
        if (banana == null)
        {
            Debug.LogError("获取香蕉皮失败！");
            return;
        }

        banana.transform.position = transform.position;
        banana.SetActive(true);

        StartCoroutine(MoveBanana(banana, targetPos));
    }

    private IEnumerator MoveBanana(GameObject banana, Vector3 targetPos)
    {
        while (banana != null && banana.activeSelf &&
               Vector3.Distance(banana.transform.position, targetPos) > 0.5f)
        {
            banana.transform.position = Vector3.MoveTowards(
                banana.transform.position,
                targetPos,
                throwSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (banana != null && banana.activeSelf)
        {
            ObjectPool.Instance.PushObject(banana);

            GameObject area = ObjectPool.Instance.GetObject(areaPrefab);
            if (area != null)
            {
                area.transform.position = targetPos;
                area.SetActive(true);
                StartCoroutine(DeactivateArea(area));
            }
        }
    }

    private IEnumerator DeactivateArea(GameObject area)
    {
        yield return new WaitForSeconds(areaDuration);
        if (area != null && area.activeSelf)
        {
            ObjectPool.Instance.PushObject(area);
        }
    }
}
