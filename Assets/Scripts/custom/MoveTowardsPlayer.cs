using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class MoveTowardsPlayer : Action
{
    public float speed = 2f;
    private Transform player;

    public override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override TaskStatus OnUpdate()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
        return TaskStatus.Running;
    }
}