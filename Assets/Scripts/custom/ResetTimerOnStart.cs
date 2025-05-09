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

        // 根据当前阶段获取预设持续时间
        StateTimer.Value = StateDurations[CurrentState.Value];
        return TaskStatus.Success;
    }
}
