using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class BossPhaseUpdater : Action
{
    // 直接引用EnemyHealth组件
    private EnemyHealth health;
    public SharedInt currentPhase; // 仍需要SharedInt供行为树使用

    [Header("血量阈值")]
    [Range(0, 1)] public float chaseThreshold = 0.8f;
    [Range(0, 1)] public float sodaThreshold = 0.5f;
    [Range(0, 1)] public float bulletThreshold = 0.3f;

    public override void OnStart()
    {
        health = GetComponent<EnemyHealth>();
    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log("当前生命值：");
        
        float healthPercent = health.currentHealth / health.maxHealth;
        Debug.Log(healthPercent);
        if (healthPercent <= bulletThreshold && currentPhase.Value < 3)
        {
            currentPhase.Value = 3;
            Debug.Log($"阶段切换至BulletAttack | 当前血量: {health.currentHealth}");
        }
        else if (healthPercent <= sodaThreshold && currentPhase.Value < 2)
        {
            currentPhase.Value = 2;
            Debug.Log($"阶段切换至SodaCharge | 当前血量: {health.currentHealth}");
        }
        else if (healthPercent <= chaseThreshold && currentPhase.Value < 1)
        {
            currentPhase.Value = 1;
            Debug.Log($"阶段切换至Chase | 当前血量: {health.currentHealth}");
        }

        return TaskStatus.Running;
    }
}