using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
[TaskDescription("等待指定时间后返回成功")]
public class HasTimePassed : Conditional
{
    public float waitTime = 1f;
    private float startTime;

    public override void OnStart()
    {
        startTime = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
        return Time.time - startTime >= waitTime ?
            TaskStatus.Success : TaskStatus.Failure;
    }
}