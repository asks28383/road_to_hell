using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class BossPhaseUpdater : Action
{
    // ֱ������EnemyHealth���
    private EnemyHealth health;
    public SharedInt currentPhase; // ����ҪSharedInt����Ϊ��ʹ��

    [Header("Ѫ����ֵ")]
    [Range(0, 1)] public float chaseThreshold = 0.8f;
    [Range(0, 1)] public float sodaThreshold = 0.5f;
    [Range(0, 1)] public float bulletThreshold = 0.3f;

    public override void OnStart()
    {
        health = GetComponent<EnemyHealth>();
    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log("��ǰ����ֵ��");
        
        float healthPercent = health.currentHealth / health.maxHealth;
        Debug.Log(healthPercent);
        if (healthPercent <= bulletThreshold && currentPhase.Value < 3)
        {
            currentPhase.Value = 3;
            Debug.Log($"�׶��л���BulletAttack | ��ǰѪ��: {health.currentHealth}");
        }
        else if (healthPercent <= sodaThreshold && currentPhase.Value < 2)
        {
            currentPhase.Value = 2;
            Debug.Log($"�׶��л���SodaCharge | ��ǰѪ��: {health.currentHealth}");
        }
        else if (healthPercent <= chaseThreshold && currentPhase.Value < 1)
        {
            currentPhase.Value = 1;
            Debug.Log($"�׶��л���Chase | ��ǰѪ��: {health.currentHealth}");
        }

        return TaskStatus.Running;
    }
}