using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
[TaskCategory("Custom")]
public class RotateStates : Action
{
    public float[] stateDurations = new float[] { 10f, 8f, 7f, 5f };
    private string[] stateNames = new string[] { "Sleep", "Chase", "SodaCharge", "BulletAttack" };
    private int currentIndex;
    private float timer;


    public override void OnStart()
    {
        currentIndex = Random.Range(0, stateNames.Length);
        Owner.SetVariable("CurrentState", (SharedString)stateNames[currentIndex]);
        timer = 0f;
    }

    public override TaskStatus OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= stateDurations[currentIndex])
        {
            currentIndex = (currentIndex + 1) % stateNames.Length;
            Owner.SetVariable("CurrentState", (SharedString)stateNames[currentIndex]);
            timer = 0f;
        }

        return TaskStatus.Running;
    }
}
