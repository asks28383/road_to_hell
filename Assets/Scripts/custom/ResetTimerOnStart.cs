using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class ResetTimerOnStart : Action
{
    public SharedFloat StateTimer;
    public SharedInt CurrentState;
    public float[] StateDurations = new float[] {  };
    public override TaskStatus OnUpdate()
    {

        // ���ݵ�ǰ�׶λ�ȡԤ�����ʱ��
        StateTimer.Value = StateDurations[CurrentState.Value];
        return TaskStatus.Success;
    }
}
