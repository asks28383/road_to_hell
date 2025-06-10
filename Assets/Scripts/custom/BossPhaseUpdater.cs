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
    [Range(0, 1)] public float sleepThreshold = 0.33f;

    // �����л���ر���
    [Header("�����л�����")]
    public bool enableDreamTransition = true;
    public float transitionDelay = 1.0f;
    public string mainSceneName = "zx";
    public string dreamSceneName = "llw";

    private EnemyHealth health;
    private PlayerHealth playerHealth;
    private Transform player;
    private float initialHealth;
    private float damageTakenBeforeDream;
    private bool isInitialized = false;
    private bool isInDreamWorld = false;
    private bool hasTriggeredDream = false;
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

        initialHealth = health.currentHealth;
        damageTakenBeforeDream = 0;
        isInDreamWorld = false;
        hasTriggeredDream = false;

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
        hasTriggeredDream = true;
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
        if (!isInDreamWorld && !isSleeping.Value && ShouldForceSleep() && !hasTriggeredDream)
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
        return damageTakenBeforeDream >= initialHealth * sleepThreshold;
    }

    private void ForceSleep()
    {
        phaseBeforeSleep = currentPhase.Value; // ��¼��ǰ�׶�
        currentPhase.Value = 0;
        phaseTimer.Value = sleepDuration;
        isSleeping.Value = true;
        hasTriggeredDream = true;

        Debug.Log($"����˯��״̬ | ֮ǰ�׶�: {GetPhaseName(phaseBeforeSleep)}");

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
            hasTriggeredDream = false;
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

    private void TransitionToDreamWorld()
    {
        if (isInDreamWorld) return;

        // ����ؼ�״̬
        PlayerPrefs.SetFloat("PlayerHealth", playerHealth.currentHealth);
        PlayerPrefs.SetFloat("BossHealth", health.currentHealth);
        PlayerPrefs.SetFloat("BossInitialHealth", initialHealth);
        PlayerPrefs.SetInt("BossPhaseBeforeSleep", phaseBeforeSleep);
        PlayerPrefs.Save();

        // �����ξ�����
        SceneManager.LoadScene(dreamSceneName);
        isInDreamWorld = true;
        Debug.Log("���ڽ����ξ�����...");
    }

    private void ReturnToMainWorld()
    {
        if (!isInDreamWorld) return;

        // �����ξ��е�Ѫ��״̬
        PlayerPrefs.SetFloat("PlayerHealth", playerHealth.currentHealth);
        PlayerPrefs.SetFloat("BossHealth", health.currentHealth);
        PlayerPrefs.Save();

        // ����������
        SceneManager.LoadScene(mainSceneName);
        isInDreamWorld = false;

        Debug.Log("���ڷ���������...");
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