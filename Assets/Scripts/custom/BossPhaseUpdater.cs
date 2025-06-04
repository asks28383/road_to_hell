using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Boss")]
public class BossPhaseController : Action
{
    // 共享变量
    public SharedInt currentPhase;
    public SharedFloat phaseTimer;
    public SharedFloat distanceToPlayer;
    public SharedFloat healthPercentage;
    public SharedBool isSleeping;

    // 阶段参数
    [Header("阶段持续时间")]
    public float bulletAttackDuration = 10f;
    public float chaseDuration = 8f;
    public float sodaChargeDuration = 6f;
    public float sleepDuration = 8f;

    [Header("距离阈值")]
    public float closeDistanceThreshold = 5f;

    [Header("血量阈值")]
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

            // 初始为BulletAttack状态
            currentPhase.Value = 3;
            phaseTimer.Value = bulletAttackDuration;
            isSleeping.Value = false;

            isInitialized = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        // 更新共享变量
        healthPercentage.Value = health.currentHealth / health.maxHealth;
        distanceToPlayer.Value = Vector3.Distance(transform.position, player.position);
        damageTaken = initialHealth - health.currentHealth;

        // 阶段计时器
        phaseTimer.Value -= Time.deltaTime;

        // 检查是否需要强制睡眠
        if (ShouldForceSleep())
        {
            ForceSleep();
            return TaskStatus.Running;
        }

        // 阶段转换逻辑
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
            case 3: // BulletAttack → Chase/SodaCharge
                if (distanceToPlayer.Value < closeDistanceThreshold)
                {
                    SetPhase(1, chaseDuration); // 进入Chase
                }
                else
                {
                    SetPhase(2, sodaChargeDuration); // 进入SodaCharge
                }
                break;

            case 1: // Chase → SodaCharge
                SetPhase(2, sodaChargeDuration);
                break;

            case 2: // SodaCharge → BulletAttack
                SetPhase(3, bulletAttackDuration);
                break;
        }
    }

    private void SetPhase(int phase, float duration)
    {
        currentPhase.Value = phase;
        phaseTimer.Value = duration;
        Debug.Log($"切换到阶段: {GetPhaseName(phase)} | 剩余血量: {health.currentHealth}");
    }

    private void ForceSleep()
    {
        currentPhase.Value = 0;
        phaseTimer.Value = sleepDuration;
        isSleeping.Value = true;
        Debug.Log($"血量累计降低超过阈值，强制进入睡眠 | 累计伤害: {damageTaken}");
    }

    private void WakeUpFromSleep()
    {
        ResetDamageTracking();
        SetPhase(3, bulletAttackDuration);
        isSleeping.Value = false;
        Debug.Log("睡眠结束，回到BulletAttack状态");
    }

    private void ResetDamageTracking()
    {
        initialHealth = health.currentHealth; // 重置基准血量
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