using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class CheckShouldSleep : Conditional
{
    public SharedInt currentPhase;
    public int requiredPhase;

    public override TaskStatus OnUpdate()
    {
        return (currentPhase.Value == requiredPhase) ?
            TaskStatus.Success :
            TaskStatus.Failure;
    }

}
