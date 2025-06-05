using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;
    public GameObject primaryHeatBar;
    public GameObject secondaryChargeBar;

    [Header("Blink Settings")]
    public float blinkDistance = 5f;
    public float blinkCooldown = 10f;
    public Image blinkCooldownUI;
    public KeyCode blinkKey = KeyCode.Mouse1;

    [Header("Heal Settings")]
    public KeyCode healKey = KeyCode.E;
    public Image healCooldownUI;
    public float healAmount = 15f;
    public float healDuration = 5f;
    public float healCooldown = 10f;

    [Header("Sprint Settings")]
    public KeyCode sprintKey = KeyCode.Q;           // 疾跑按键
    public float sprintSpeedMultiplier = 2f;      // 疾跑速度倍率
    public float sprintDuration = 3f;               // 疾跑持续时间
    public float sprintCooldown = 5f;              // 疾跑冷却时间
    public Image sprintCooldownMask;               // 疾跑冷却遮罩
    public Text sprintCooldownText;                // 疾跑冷却文本
    public ParticleSystem sprintParticles;         // 疾跑粒子效果

    [Header("CoolDown UI")]
    public Image blinkCooldownMask;
    public Text blinkCooldownText;
    public Image healCooldownMask;
    public Text healCooldownText;

    // 私有变量
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;
    private MovementController playerMovement;         // 假设你有PlayerMovement控制移动
    private bool canBlink = true;
    private bool canHeal = true;
    private bool canSprint = true;
    private float blinkCooldownTimer;
    private float healCooldownTimer;
    private float sprintCooldownTimer;
    private bool isHealing = false;
    private bool isSprinting = false;
    private float healTimer;
    private float sprintTimer;
    private float healPerSecond;
    private float originalSpeed;                   // 存储原始移动速度

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<MovementController>(); // 获取移动组件
        healPerSecond = healAmount / healDuration;

        // 存储原始移动速度
        if (playerMovement != null)
        {
            originalSpeed = playerMovement.movementSpeed;
        }

        UpdateCooldownUI();
        SetWeaponActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) SwitchWeapon();
        if (Input.GetKeyDown(blinkKey)) TryBlink();
        if (Input.GetKeyDown(healKey) && canHeal && !isHealing) StartHealing();
        if (Input.GetKeyDown(sprintKey)) TrySprint(); // 尝试疾跑

        UpdateHealing();
        UpdateSprinting(); // 更新疾跑状态
        UpdateCooldowns();
    }

    private void TrySprint()
    {
        if (!canSprint || isSprinting) return;

        // 开始疾跑
        isSprinting = true;
        canSprint = false;
        sprintTimer = 0f;

        // 增加移动速度
        if (playerMovement != null)
        {
            playerMovement.movementSpeed *= sprintSpeedMultiplier;
        }

        // 开启粒子效果
        if (sprintParticles != null)
        {
            sprintParticles.Play();
        }
    }

    private void UpdateSprinting()
    {
        if (!isSprinting) return;

        sprintTimer += Time.deltaTime;

        // 疾跑结束
        if (sprintTimer >= sprintDuration)
        {
            EndSprint();
        }
    }

    private void EndSprint()
    {
        isSprinting = false;
        sprintCooldownTimer = sprintCooldown;

        // 恢复原始移动速度
        if (playerMovement != null)
        {
            playerMovement.movementSpeed = originalSpeed;
        }

        // 停止粒子效果
        if (sprintParticles != null)
        {
            sprintParticles.Stop();
        }
    }

    private void StartHealing()
    {
        canHeal = false;
        isHealing = true;
        healTimer = 0f;
        healCooldownTimer = healCooldown;
    }

    private void UpdateHealing()
    {
        if (!isHealing) return;

        healTimer += Time.deltaTime;
        if (playerHealth != null)
        {
            playerHealth.Recover(healPerSecond * Time.deltaTime);
        }

        if (healTimer >= healDuration)
        {
            isHealing = false;
        }
    }

    private void SwitchWeapon()
    {
        bool isPrimary = !primaryWeapon.activeSelf;
        SetWeaponActive(isPrimary);
    }

    private void SetWeaponActive(bool isPrimary)
    {
        primaryWeapon.SetActive(isPrimary);
        secondaryWeapon.SetActive(!isPrimary);
        if (primaryHeatBar) primaryHeatBar.SetActive(isPrimary);
        if (secondaryChargeBar) secondaryChargeBar.SetActive(!isPrimary);
    }

    private void TryBlink()
    {
        if (!canBlink) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            moveHorizontal = transform.localScale.x > 0 ? 1 : -1;
        }

        Vector2 moveDirection = new Vector2(moveHorizontal, moveVertical).normalized;
        if (moveDirection == Vector2.zero)
        {
            moveDirection = Vector2.right;
        }

        rb.position += moveDirection * blinkDistance;
        canBlink = false;
        blinkCooldownTimer = blinkCooldown;
        UpdateCooldownUI();
    }

    private void UpdateCooldowns()
    {
        if (!canBlink)
        {
            blinkCooldownTimer -= Time.deltaTime;
            if (blinkCooldownTimer <= 0f) canBlink = true;
        }

        if (!canHeal && !isHealing)
        {
            healCooldownTimer -= Time.deltaTime;
            if (healCooldownTimer <= 0f) canHeal = true;
        }

        if (!canSprint && !isSprinting)
        {
            sprintCooldownTimer -= Time.deltaTime;
            if (sprintCooldownTimer <= 0f) canSprint = true;
        }

        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        // 闪现冷却UI
        if (blinkCooldownMask != null)
        {
            if (canBlink)
            {
                blinkCooldownMask.fillAmount = 0;
                blinkCooldownText.text = "";
            }
            else
            {
                float blinkRatio = blinkCooldownTimer / blinkCooldown;
                blinkCooldownMask.fillAmount = blinkRatio;
                blinkCooldownText.text = Mathf.Ceil(blinkCooldownTimer).ToString();

                if (blinkRatio < 0.2f)
                {
                    float alpha = Mathf.PingPong(Time.time * 5, 1);
                    blinkCooldownText.color = new Color(1, 1, 1, alpha);
                }
                else
                {
                    blinkCooldownText.color = new Color(1, 1, 1, 0.8f);
                }
            }
        }

        // 治疗冷却UI
        if (healCooldownMask != null)
        {
            if (isHealing)
            {
                float healRatio = 1 - (healTimer / healDuration);
                healCooldownMask.fillAmount = healRatio;
                healCooldownText.text = Mathf.Ceil(healDuration - healTimer).ToString();
                healCooldownText.color = new Color(0.5f, 1, 0.5f, 0.8f);
            }
            else if (canHeal)
            {
                healCooldownMask.fillAmount = 0;
                healCooldownText.text = "";
            }
            else
            {
                float healRatio = healCooldownTimer / healCooldown;
                healCooldownMask.fillAmount = healRatio;
                healCooldownText.text = Mathf.Ceil(healCooldownTimer).ToString();
                healCooldownText.color = new Color(1, 1, 1, 0.8f);
            }
        }

        // 疾跑冷却UI
        if (sprintCooldownMask != null)
        {
            if (isSprinting)
            {
                float sprintRatio = 1 - (sprintTimer / sprintDuration);
                sprintCooldownMask.fillAmount = sprintRatio;
                sprintCooldownText.text = Mathf.Ceil(sprintDuration - sprintTimer).ToString();
                sprintCooldownText.color = new Color(1, 0.8f, 0.5f, 0.8f); // 橙色
            }
            else if (canSprint)
            {
                sprintCooldownMask.fillAmount = 0;
                sprintCooldownText.text = "";
            }
            else
            {
                float sprintRatio = sprintCooldownTimer / sprintCooldown;
                sprintCooldownMask.fillAmount = sprintRatio;
                sprintCooldownText.text = Mathf.Ceil(sprintCooldownTimer).ToString();
                sprintCooldownText.color = new Color(1, 1, 1, 0.8f);
            }
        }
    }
}