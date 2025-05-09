using System.Diagnostics;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class CheckShouldBulletAttack : Conditional
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

