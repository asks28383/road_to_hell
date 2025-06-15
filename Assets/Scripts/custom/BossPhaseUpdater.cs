using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Range(0, 1)] public float firstSleepThreshold = 0.66f; // 第一次睡眠阈值 (2/3血量)
    [Range(0, 1)] public float secondSleepThreshold = 0.33f; // 第二次睡眠阈值 (1/3血量)

    // 场景切换相关变量
    [Header("场景切换设置")]
    public bool enableDreamTransition = true;
    public float transitionDelay = 1.0f;
    public string mainSceneName = "zx";
    public string dreamSceneName = "DreamScene";

    private EnemyHealth health;
    private PlayerHealth playerHealth;
    private Transform player;
    private float initialHealth;
    private float damageTakenBeforeDream;
    private bool isInitialized = false;
    private bool isInDreamWorld = false;
    private bool hasTriggeredFirstDream = false;
    private bool hasTriggeredSecondDream = false;
    private int phaseBeforeSleep; // 记录进入睡眠前的阶段

    public override void OnStart()
    {
        if (!isInitialized)
        {
            health = GetComponent<EnemyHealth>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerHealth = player.GetComponent<PlayerHealth>();

            // 根据当前场景初始化
            if (SceneManager.GetActiveScene().name == mainSceneName)
            {
                InitializeMainWorld();
            }
            else
            {
                InitializeDreamWorld();
            }

            isInitialized = true;
        }
    }

    private void InitializeMainWorld()
    {
        // 从PlayerPrefs加载血量（如果是从梦境返回）
        if (PlayerPrefs.HasKey("PlayerHealth") && PlayerPrefs.HasKey("BossHealth"))
        {
            playerHealth.currentHealth = PlayerPrefs.GetFloat("PlayerHealth");
            health.currentHealth = PlayerPrefs.GetFloat("BossHealth");
            PlayerPrefs.DeleteKey("PlayerHealth");
            PlayerPrefs.DeleteKey("BossHealth");
        }

        // 从PlayerPrefs加载梦境触发状态
        if (PlayerPrefs.HasKey("HasTriggeredFirstDream"))
        {
            hasTriggeredFirstDream = PlayerPrefs.GetInt("HasTriggeredFirstDream") == 1;
        }
        if (PlayerPrefs.HasKey("HasTriggeredSecondDream"))
        {
            hasTriggeredSecondDream = PlayerPrefs.GetInt("HasTriggeredSecondDream") == 1;
        }

        initialHealth = health.currentHealth;
        damageTakenBeforeDream = 0;
        isInDreamWorld = false;

        // 如果是首次初始化，设置为BulletAttack状态
        if (currentPhase.Value == 0)
        {
            currentPhase.Value = 3;
            phaseTimer.Value = bulletAttackDuration;
            isSleeping.Value = false;
        }
    }

    private void InitializeDreamWorld()
    {
        isInDreamWorld = true;
        currentPhase.Value = 0; // 强制设置为Sleep状态
        isSleeping.Value = true;
        phaseTimer.Value = sleepDuration;

        // 从PlayerPrefs加载血量（如果是从主世界进入）
        if (PlayerPrefs.HasKey("PlayerHealth") && PlayerPrefs.HasKey("BossHealth"))
        {
            playerHealth.currentHealth = PlayerPrefs.GetFloat("PlayerHealth");
            health.currentHealth = PlayerPrefs.GetFloat("BossHealth");
        }
    }

    public override TaskStatus OnUpdate()
    {
        healthPercentage.Value = health.currentHealth / health.maxHealth;
        distanceToPlayer.Value = Vector3.Distance(transform.position, player.position);

        // 只在主场景且非睡眠状态时累计伤害
        if (!isInDreamWorld && !isSleeping.Value)
        {
            damageTakenBeforeDream = initialHealth - health.currentHealth;
        }

        phaseTimer.Value -= Time.deltaTime;

        // 检查是否需要强制睡眠（只在主场景且未处于睡眠状态时检查）
        if (!isInDreamWorld && !isSleeping.Value && ShouldForceSleep())
        {
            ForceSleep();
            return TaskStatus.Running;
        }

        // 睡眠状态结束检查
        if (phaseTimer.Value <= 0 && isSleeping.Value)
        {
            WakeUpFromSleep();
            return TaskStatus.Running;
        }

        // 正常阶段转换
        if (phaseTimer.Value <= 0 && !isSleeping.Value)
        {
            HandleNormalPhaseTransition();
            return TaskStatus.Running;
        }

        return TaskStatus.Running;
    }

    private bool ShouldForceSleep()
    {
        // 检查第一次睡眠条件（未触发过且血量降到阈值）
        if (!hasTriggeredFirstDream && healthPercentage.Value <= firstSleepThreshold)
        {
            return true;
        }

        // 检查第二次睡眠条件（未触发过且血量降到阈值）
        if (!hasTriggeredSecondDream && healthPercentage.Value <= secondSleepThreshold)
        {
            return true;
        }

        return false;
    }

    private void ForceSleep()
    {
        phaseBeforeSleep = currentPhase.Value; // 记录当前阶段
        currentPhase.Value = 0;
        phaseTimer.Value = sleepDuration;
        isSleeping.Value = true;

        // 标记触发的梦境并保存状态
        if (!hasTriggeredFirstDream && healthPercentage.Value <= firstSleepThreshold)
        {
            hasTriggeredFirstDream = true;
            PlayerPrefs.SetInt("HasTriggeredFirstDream", 1);
            Debug.Log($"第一次进入睡眠状态 | 之前阶段: {GetPhaseName(phaseBeforeSleep)} | 血量阈值: {firstSleepThreshold}");
        }
        else if (!hasTriggeredSecondDream && healthPercentage.Value <= secondSleepThreshold)
        {
            hasTriggeredSecondDream = true;
            PlayerPrefs.SetInt("HasTriggeredSecondDream", 1);
            Debug.Log($"第二次进入睡眠状态 | 之前阶段: {GetPhaseName(phaseBeforeSleep)} | 血量阈值: {secondSleepThreshold}");
        }

        PlayerPrefs.Save();

        if (enableDreamTransition)
        {
            TransitionToDreamWorld();
        }
    }

    private void WakeUpFromSleep()
    {
        isSleeping.Value = false;

        if (isInDreamWorld)
        {
            ReturnToMainWorld();
        }
        else
        {
            // 主场景中睡眠结束，恢复之前的状态
            SetPhase(phaseBeforeSleep, GetPhaseDuration(phaseBeforeSleep));
            initialHealth = health.currentHealth; // 重置伤害计算基准
            damageTakenBeforeDream = 0;

            Debug.Log($"主场景睡眠结束，回到阶段: {GetPhaseName(phaseBeforeSleep)}");
        }
    }

    private void HandleNormalPhaseTransition()
    {
        switch (currentPhase.Value)
        {
            case 3: // BulletAttack → Chase/SodaCharge
                if (distanceToPlayer.Value < closeDistanceThreshold)
                {
                    SetPhase(1, chaseDuration);
                }
                else
                {
                    SetPhase(2, sodaChargeDuration);
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
    }

    private float GetPhaseDuration(int phase)
    {
        return phase switch
        {
            1 => chaseDuration,
            2 => sodaChargeDuration,
            3 => bulletAttackDuration,
            _ => bulletAttackDuration
        };
    }

    private IEnumerator TransitionToDreamWorldCoroutine()
    {
        // 保存关键状态
        PlayerPrefs.SetFloat("PlayerHealth", playerHealth.currentHealth);
        PlayerPrefs.SetFloat("BossHealth", health.currentHealth);
        PlayerPrefs.Save();

        // 开始转场
        DreamTransitionManager.Instance.TransitionToScene(dreamSceneName);

        // 等待转场完成（通过场景名称判断）
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == dreamSceneName);

        isInDreamWorld = true;
        AchievementEvents.OnAchievementTriggered?.Invoke("EnterDreamWorld");
        Debug.Log("已进入梦境世界");
    }

    private IEnumerator ReturnToMainWorldCoroutine()
    {
        // 保存状态
        PlayerPrefs.SetFloat("PlayerHealth", playerHealth.currentHealth);
        PlayerPrefs.SetFloat("BossHealth", health.currentHealth);
        PlayerPrefs.Save();

        // 开始转场
        DreamTransitionManager.Instance.TransitionToScene(mainSceneName);

        // 等待转场完成
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == mainSceneName);

        isInDreamWorld = false;
        Debug.Log("已返回主世界");
    }
    private void TransitionToDreamWorld()
    {
        if (isInDreamWorld) return;
        StartCoroutine(TransitionToDreamWorldCoroutine());
    }

    private void ReturnToMainWorld()
    {
        if (!isInDreamWorld) return;
        StartCoroutine(ReturnToMainWorldCoroutine());
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