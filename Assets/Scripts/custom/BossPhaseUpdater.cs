using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Boss")]
public class BossPhaseController : Action
{
    // �������
    public SharedInt currentPhase;
    public SharedFloat phaseTimer;
    public SharedFloat distanceToPlayer;
    public SharedFloat healthPercentage;
    public SharedBool isSleeping;

    // �׶β���
    [Header("�׶γ���ʱ��")]
    public float bulletAttackDuration = 10f;
    public float chaseDuration = 8f;
    public float sodaChargeDuration = 6f;
    public float sleepDuration = 8f;

    [Header("������ֵ")]
    public float closeDistanceThreshold = 5f;

    [Header("Ѫ����ֵ")]
    [Range(0, 1)] public float sleepThreshold = 0.33f;

    private EnemyHealth health;
    private Transform player;
    private float initialHealth;
    private float damageTaken;
    private bool isInitialized = false;

    public override void OnStart()
    {
        if (!isInitialized)
        {
            health = GetComponent<EnemyHealth>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            initialHealth = health.maxHealth;
            ResetDamageTracking();

            // ��ʼΪBulletAttack״̬
            currentPhase.Value = 3;
            phaseTimer.Value = bulletAttackDuration;
            isSleeping.Value = false;

            isInitialized = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        // ���¹������
        healthPercentage.Value = health.currentHealth / health.maxHealth;
        distanceToPlayer.Value = Vector3.Distance(transform.position, player.position);
        damageTaken = initialHealth - health.currentHealth;

        // �׶μ�ʱ��
        phaseTimer.Value -= Time.deltaTime;

        // ����Ƿ���Ҫǿ��˯��
        if (ShouldForceSleep())
        {
            ForceSleep();
            return TaskStatus.Running;
        }

        // �׶�ת���߼�
        if (phaseTimer.Value <= 0)
        {
            if (isSleeping.Value)
            {
                WakeUpFromSleep();
            }
            else
            {
                HandleNormalPhaseTransition();
            }
            return TaskStatus.Running;
        }

        return TaskStatus.Running;
    }

    private bool ShouldForceSleep()
    {
        return !isSleeping.Value && damageTaken >= initialHealth * sleepThreshold;
    }

    private void HandleNormalPhaseTransition()
    {
        switch (currentPhase.Value)
        {
            case 3: // BulletAttack �� Chase/SodaCharge
                if (distanceToPlayer.Value < closeDistanceThreshold)
                {
                    SetPhase(1, chaseDuration); // ����Chase
                }
                else
                {
                    SetPhase(2, sodaChargeDuration); // ����SodaCharge
                }
                break;

            case 1: // Chase �� SodaCharge
                SetPhase(2, sodaChargeDuration);
                break;

            case 2: // SodaCharge �� BulletAttack
                SetPhase(3, bulletAttackDuration);
                break;
        }
    }

    private void SetPhase(int phase, float duration)
    {
        currentPhase.Value = phase;
        phaseTimer.Value = duration;
        Debug.Log($"�л����׶�: {GetPhaseName(phase)} | ʣ��Ѫ��: {health.currentHealth}");
    }

    private void ForceSleep()
    {
        currentPhase.Value = 0;
        phaseTimer.Value = sleepDuration;
        isSleeping.Value = true;
        Debug.Log($"Ѫ���ۼƽ��ͳ�����ֵ��ǿ�ƽ���˯�� | �ۼ��˺�: {damageTaken}");
    }

    private void WakeUpFromSleep()
    {
        ResetDamageTracking();
        SetPhase(3, bulletAttackDuration);
        isSleeping.Value = false;
        Debug.Log("˯�߽������ص�BulletAttack״̬");
    }

    private void ResetDamageTracking()
    {
        initialHealth = health.currentHealth; // ���û�׼Ѫ��
        damageTaken = 0;
    }

    private string GetPhaseName(int phase)
    {
        return phase switch
        {
            0 => "SleepState",
            1 => "ChaseState",
            2 => "SodaChargeState",
            3 => "BulletAttackState",
            _ => "Unknown"
        };
    }
}