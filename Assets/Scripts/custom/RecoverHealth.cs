using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class RecoverHealth : Action
{
    public float recoverRate =100f; // ÿ��ָ���

    public override TaskStatus OnUpdate()
    {
        // ����BossHealth�ǹ�����Boss�ϵ����
        GetComponent<EnemyHealth>().Recover(recoverRate * Time.deltaTime);
        return TaskStatus.Running;
    }
}
