using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class RecoverHealth : Action
{
    public float recoverRate =100f; // 每秒恢复量

    public override TaskStatus OnUpdate()
    {
        // 假设BossHealth是挂载在Boss上的组件
        GetComponent<EnemyHealth>().Recover(recoverRate * Time.deltaTime);
        return TaskStatus.Running;
    }
}
