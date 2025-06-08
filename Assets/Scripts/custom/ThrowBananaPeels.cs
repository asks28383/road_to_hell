using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class ThrowBananaPeels : Action
{
    [Header("Ͷ������")]
    public GameObject bananaPrefab;
    public GameObject areaPrefab;
    public float throwSpeed = 10f;
    public float throwInterval = 1.5f;
    public int totalThrows = 5;
    public float areaDuration = 5f;
    public Transform throwPoint; // �������㽶�׳���

    [Header("��������")]
    public string throwAnimTrigger = "Throw";
    public float animationLength = 1f;

    private Transform player;
    private float throwTimer;
    private int throwCount;
    private Animator animator;
    private bool isThrowing;
    private SpriteRenderer bossSprite; // ��������ȡBoss��SpriteRenderer

    public override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        bossSprite = GetComponent<SpriteRenderer>(); // ��ȡSpriteRenderer
        throwTimer = 0f;
        throwCount = 0;
        isThrowing = false;

        // ���û��ָ���׳��㣬Ĭ��ʹ������λ��
        if (throwPoint == null)
        {
            throwPoint = transform;
            Debug.LogWarning("û��ָ��ThrowPoint����ʹ�ý�ɫ����λ����Ϊ�׳���");
        }

        // Ԥ���ض����
        if (!ObjectPool.Instance.HasPool(bananaPrefab.name))
            ObjectPool.Instance.PrewarmPool(bananaPrefab, totalThrows + 2);
        if (!ObjectPool.Instance.HasPool(areaPrefab.name))
            ObjectPool.Instance.PrewarmPool(areaPrefab, totalThrows + 2);
    }


    private Vector3 GetAdjustedThrowPosition()
    {
        // ����Boss�Ƿ�ת�������׳���λ��
        if (bossSprite.flipX)
        {
            // ��Boss�泯��ʱ�������׳���λ��
            return new Vector3(
                transform.position.x - Mathf.Abs(throwPoint.localPosition.x),
                throwPoint.position.y,
                throwPoint.position.z
            );
        }
        else
        {
            // ����λ��
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
            Debug.LogError("��ȡ�㽶Ƥʧ�ܣ�");
            return;
        }

        // ʹ�õ�������׳���λ��
        banana.transform.position = GetAdjustedThrowPosition();
        banana.SetActive(true);

        StartCoroutine(MoveBanana(banana, targetPos));
    
}

    private IEnumerator MoveBanana(GameObject banana, Vector3 targetPos)
    {
        // �����ת�ٶȱ���
        float rotationSpeed = 360f; // ÿ����ת�������ɸ�����Ҫ����

        // �����ʼ����
        Vector3 throwDirection = (targetPos - banana.transform.position).normalized;

        while (banana != null && banana.activeSelf &&
               Vector3.Distance(banana.transform.position, targetPos) > 0.5f)
        {
            // �ƶ��㽶
            banana.transform.position = Vector3.MoveTowards(
                banana.transform.position,
                targetPos,
                throwSpeed * Time.deltaTime
            );

            // �����תЧ�� - ��Z����ת��2DЧ����
            banana.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // ����ʹ�����·������㽶ʼ�ճ����ƶ���������ֽ����תЧ����
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