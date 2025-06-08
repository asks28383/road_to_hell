using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class ThrowBananaPeels : Action
{
    [Header("投掷设置")]
    public GameObject bananaPrefab;
    public GameObject areaPrefab;
    public float throwSpeed = 10f;
    public float throwInterval = 1.5f;
    public int totalThrows = 5;
    public float areaDuration = 5f;
    public Transform throwPoint; // 新增：香蕉抛出点

    [Header("动画参数")]
    public string throwAnimTrigger = "Throw";
    public float animationLength = 1f;

    private Transform player;
    private float throwTimer;
    private int throwCount;
    private Animator animator;
    private bool isThrowing;
    private SpriteRenderer bossSprite; // 新增：获取Boss的SpriteRenderer

    public override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        bossSprite = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer
        throwTimer = 0f;
        throwCount = 0;
        isThrowing = false;

        // 如果没有指定抛出点，默认使用自身位置
        if (throwPoint == null)
        {
            throwPoint = transform;
            Debug.LogWarning("没有指定ThrowPoint，将使用角色自身位置作为抛出点");
        }

        // 预加载对象池
        if (!ObjectPool.Instance.HasPool(bananaPrefab.name))
            ObjectPool.Instance.PrewarmPool(bananaPrefab, totalThrows + 2);
        if (!ObjectPool.Instance.HasPool(areaPrefab.name))
            ObjectPool.Instance.PrewarmPool(areaPrefab, totalThrows + 2);
    }


    private Vector3 GetAdjustedThrowPosition()
    {
        // 根据Boss是否翻转来调整抛出点位置
        if (bossSprite.flipX)
        {
            // 当Boss面朝左时，调整抛出点位置
            return new Vector3(
                transform.position.x - Mathf.Abs(throwPoint.localPosition.x),
                throwPoint.position.y,
                throwPoint.position.z
            );
        }
        else
        {
            // 正常位置
            return throwPoint.position;
        }
    }
    public override TaskStatus OnUpdate()
    {
        throwTimer += Time.deltaTime;

        if (isThrowing && throwTimer < animationLength)
        {
            return TaskStatus.Running;
        }

        if (throwTimer >= throwInterval && throwCount < totalThrows)
        {
            StartThrow();
            throwTimer = 0f;
            throwCount++;
        }

        if (throwCount >= totalThrows && !isThrowing)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private void StartThrow()
    {
        isThrowing = true;

        if (animator != null)
        {
            animator.SetTrigger(throwAnimTrigger);
        }

        StartCoroutine(ExecuteThrowAfterDelay(animationLength * 1f));
    }

    private IEnumerator ExecuteThrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 targetPos = player.position;
        ThrowBanana(targetPos);

        isThrowing = false;
    }

    private void ThrowBanana(Vector3 targetPos)
    {
        GameObject banana = ObjectPool.Instance.GetObject(bananaPrefab);
        if (banana == null)
        {
            Debug.LogError("获取香蕉皮失败！");
            return;
        }

        // 使用调整后的抛出点位置
        banana.transform.position = GetAdjustedThrowPosition();
        banana.SetActive(true);

        StartCoroutine(MoveBanana(banana, targetPos));
    
}

    private IEnumerator MoveBanana(GameObject banana, Vector3 targetPos)
    {
        // 添加旋转速度变量
        float rotationSpeed = 360f; // 每秒旋转度数，可根据需要调整

        // 计算初始方向
        Vector3 throwDirection = (targetPos - banana.transform.position).normalized;

        while (banana != null && banana.activeSelf &&
               Vector3.Distance(banana.transform.position, targetPos) > 0.5f)
        {
            // 移动香蕉
            banana.transform.position = Vector3.MoveTowards(
                banana.transform.position,
                targetPos,
                throwSpeed * Time.deltaTime
            );

            // 添加旋转效果 - 绕Z轴旋转（2D效果）
            banana.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // 或者使用以下方法让香蕉始终朝向移动方向（类似纸牌旋转效果）
            // banana.transform.right = (targetPos - banana.transform.position).normalized;

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