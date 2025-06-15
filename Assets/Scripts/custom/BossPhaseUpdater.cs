using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Range(0, 1)] public float firstSleepThreshold = 0.66f; // ��һ��˯����ֵ (2/3Ѫ��)
    [Range(0, 1)] public float secondSleepThreshold = 0.33f; // �ڶ���˯����ֵ (1/3Ѫ��)

    // �����л���ر���
    [Header("�����л�����")]
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
    private int phaseBeforeSleep; // ��¼����˯��ǰ�Ľ׶�

    public override void OnStart()
    {
        if (!isInitialized)
        {
            health = GetComponent<EnemyHealth>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerHealth = player.GetComponent<PlayerHealth>();

            // ���ݵ�ǰ������ʼ��
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
        // ��PlayerPrefs����Ѫ��������Ǵ��ξ����أ�
        if (PlayerPrefs.HasKey("PlayerHealth") && PlayerPrefs.HasKey("BossHealth"))
        {
            playerHealth.currentHealth = PlayerPrefs.GetFloat("PlayerHealth");
            health.currentHealth = PlayerPrefs.GetFloat("BossHealth");
            PlayerPrefs.DeleteKey("PlayerHealth");
            PlayerPrefs.DeleteKey("BossHealth");
        }

        // ��PlayerPrefs�����ξ�����״̬
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

        // ������״γ�ʼ��������ΪBulletAttack״̬
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
        currentPhase.Value = 0; // ǿ������ΪSleep״̬
        isSleeping.Value = true;
        phaseTimer.Value = sleepDuration;

        // ��PlayerPrefs����Ѫ��������Ǵ���������룩
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

        // ֻ���������ҷ�˯��״̬ʱ�ۼ��˺�
        if (!isInDreamWorld && !isSleeping.Value)
        {
            damageTakenBeforeDream = initialHealth - health.currentHealth;
        }

        phaseTimer.Value -= Time.deltaTime;

        // ����Ƿ���Ҫǿ��˯�ߣ�ֻ����������δ����˯��״̬ʱ��飩
        if (!isInDreamWorld && !isSleeping.Value && ShouldForceSleep())
        {
            ForceSleep();
            return TaskStatus.Running;
        }

        // ˯��״̬�������
        if (phaseTimer.Value <= 0 && isSleeping.Value)
        {
            WakeUpFromSleep();
            return TaskStatus.Running;
        }

        // �����׶�ת��
        if (phaseTimer.Value <= 0 && !isSleeping.Value)
        {
            HandleNormalPhaseTransition();
            return TaskStatus.Running;
        }

        return TaskStatus.Running;
    }

    private bool ShouldForceSleep()
    {
        // ����һ��˯��������δ��������Ѫ��������ֵ��
        if (!hasTriggeredFirstDream && healthPercentage.Value <= firstSleepThreshold)
        {
            return true;
        }

        // ���ڶ���˯��������δ��������Ѫ��������ֵ��
        if (!hasTriggeredSecondDream && healthPercentage.Value <= secondSleepThreshold)
        {
            return true;
        }

        return false;
    }

    private void ForceSleep()
    {
        phaseBeforeSleep = currentPhase.Value; // ��¼��ǰ�׶�
        currentPhase.Value = 0;
        phaseTimer.Value = sleepDuration;
        isSleeping.Value = true;

        // ��Ǵ������ξ�������״̬
        if (!hasTriggeredFirstDream && healthPercentage.Value <= firstSleepThreshold)
        {
            hasTriggeredFirstDream = true;
            PlayerPrefs.SetInt("HasTriggeredFirstDream", 1);
            Debug.Log($"��һ�ν���˯��״̬ | ֮ǰ�׶�: {GetPhaseName(phaseBeforeSleep)} | Ѫ����ֵ: {firstSleepThreshold}");
        }
        else if (!hasTriggeredSecondDream && healthPercentage.Value <= secondSleepThreshold)
        {
            hasTriggeredSecondDream = true;
            PlayerPrefs.SetInt("HasTriggeredSecondDream", 1);
            Debug.Log($"�ڶ��ν���˯��״̬ | ֮ǰ�׶�: {GetPhaseName(phaseBeforeSleep)} | Ѫ����ֵ: {secondSleepThreshold}");
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
            // ��������˯�߽������ָ�֮ǰ��״̬
            SetPhase(phaseBeforeSleep, GetPhaseDuration(phaseBeforeSleep));
            initialHealth = health.currentHealth; // �����˺������׼
            damageTakenBeforeDream = 0;

            Debug.Log($"������˯�߽������ص��׶�: {GetPhaseName(phaseBeforeSleep)}");
        }
    }

    private void HandleNormalPhaseTransition()
    {
        switch (currentPhase.Value)
        {
            case 3: // BulletAttack �� Chase/SodaCharge
                if (distanceToPlayer.Value < closeDistanceThreshold)
                {
                    SetPhase(1, chaseDuration);
                }
                else
                {
                    SetPhase(2, sodaChargeDuration);
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
        // ����ؼ�״̬
        PlayerPrefs.SetFloat("PlayerHealth", playerHealth.currentHealth);
        PlayerPrefs.SetFloat("BossHealth", health.currentHealth);
        PlayerPrefs.Save();

        // ��ʼת��
        DreamTransitionManager.Instance.TransitionToScene(dreamSceneName);

        // �ȴ�ת����ɣ�ͨ�����������жϣ�
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == dreamSceneName);

        isInDreamWorld = true;
        AchievementEvents.OnAchievementTriggered?.Invoke("EnterDreamWorld");
        Debug.Log("�ѽ����ξ�����");
    }

    private IEnumerator ReturnToMainWorldCoroutine()
    {
        // ����״̬
        PlayerPrefs.SetFloat("PlayerHealth", playerHealth.currentHealth);
        PlayerPrefs.SetFloat("BossHealth", health.currentHealth);
        PlayerPrefs.Save();

        // ��ʼת��
        DreamTransitionManager.Instance.TransitionToScene(mainSceneName);

        // �ȴ�ת�����
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == mainSceneName);

        isInDreamWorld = false;
        Debug.Log("�ѷ���������");
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