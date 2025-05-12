using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class ThrowBananaPeels : Action
{
    [Header("Ͷ������")]
    public GameObject bananaPrefab;  // �㽶ƤԤ����
    public GameObject areaPrefab;   // ����Ԥ����
    public float throwSpeed = 10f;  // Ͷ���ٶ�
    public float throwInterval = 0.5f; // ������ʱ��
    public int totalThrows = 5;     // �ܹ��������
    public float areaDuration = 5f; // �������ʱ��

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

        // Ԥ���ض����
        if (!ObjectPool.Instance.HasPool(bananaPrefab.name))
            ObjectPool.Instance.PrewarmPool(bananaPrefab, totalThrows + 2);
        if (!ObjectPool.Instance.HasPool(areaPrefab.name))
            ObjectPool.Instance.PrewarmPool(areaPrefab, totalThrows + 2);
    }

    public override TaskStatus OnUpdate()
    {
        if (!isThrowing) return TaskStatus.Success;

        throwTimer += Time.deltaTime;

        // �ﵽ�������һ���ʣ�෢�����
        if (throwTimer >= throwInterval && throwCount < totalThrows)
        {
            // ��ȡ��ǰ֡�����λ�ã���Ԥ�⣩
            Vector3 targetPos = player.position;
            ThrowBanana(targetPos);
            throwTimer = 0f;
            throwCount++;
        }

        // ����Ƿ�������з���
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
            Debug.LogError("��ȡ�㽶Ƥʧ�ܣ�");
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
