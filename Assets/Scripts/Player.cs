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
    public float healAmount = 15f;      // 总治疗量
    public float healDuration = 5f;     // 治疗持续时间
    public float healCooldown = 10f;    // 冷却时间

    [Header("CoolDown UI")]
    public Image blinkCooldownMask;  // 闪现冷却遮罩
    public Text blinkCooldownText;   // 闪现冷却文本
    public Image healCooldownMask;   // 治疗冷却遮罩
    public Text healCooldownText;    // 治疗冷却文本

    // 私有变量
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;
    private bool canBlink = true;
    private bool canHeal = true;
    private float blinkCooldownTimer;
    private float healCooldownTimer;
    private bool isHealing = false;
    private float healTimer;
    private float healPerSecond;  // 每秒治疗量

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        healPerSecond = healAmount / healDuration; // 计算每秒治疗量
        UpdateCooldownUI();
        SetWeaponActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) SwitchWeapon();
        if (Input.GetKeyDown(blinkKey)) TryBlink();
        if (Input.GetKeyDown(healKey) && canHeal && !isHealing) StartHealing();

        UpdateHealing();  // 更新治疗过程
        UpdateCooldowns(); // 更新冷却
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

        // 每帧恢复血量（总量/持续时间 * Time.deltaTime）
        if (playerHealth != null)
        {
            playerHealth.Recover(healPerSecond * Time.deltaTime);
        }

        // 治疗结束
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

        // 获取角色当前的移动输入
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // 如果没有输入，则使用角色当前面朝方向（例如最后移动的方向）
        if (moveHorizontal == 0 && moveVertical == 0)
        {
            // 可以获取角色当前的SpriteRenderer.flipX或transform.localScale.x来判断面朝方向
            // 这里假设向右为正方向
            moveHorizontal = transform.localScale.x > 0 ? 1 : -1;
        }

        // 创建移动方向向量并归一化
        Vector2 moveDirection = new Vector2(moveHorizontal, moveVertical).normalized;

        // 如果完全没有方向（理论上不会发生），则默认向右闪现
        if (moveDirection == Vector2.zero)
        {
            moveDirection = Vector2.right;
        }

        // 应用闪现
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

        if (!canHeal && !isHealing) // 只有治疗完全结束后才开始冷却计时
        {
            healCooldownTimer -= Time.deltaTime;
            if (healCooldownTimer <= 0f) canHeal = true;
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

                // 冷却快结束时闪烁效果
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
                // 治疗中显示进度
                float healRatio = 1 - (healTimer / healDuration);
                healCooldownMask.fillAmount = healRatio;
                healCooldownText.text = Mathf.Ceil(healDuration - healTimer).ToString();
                healCooldownText.color = new Color(0.5f, 1, 0.5f, 0.8f); // 浅绿色
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
    }
}