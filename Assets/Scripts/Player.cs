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
    public KeyCode sprintKey = KeyCode.Q;           // ���ܰ���
    public float sprintSpeedMultiplier = 2f;      // �����ٶȱ���
    public float sprintDuration = 3f;               // ���ܳ���ʱ��
    public float sprintCooldown = 5f;              // ������ȴʱ��
    public Image sprintCooldownMask;               // ������ȴ����
    public Text sprintCooldownText;                // ������ȴ�ı�
    public ParticleSystem sprintParticles;         // ��������Ч��

    [Header("CoolDown UI")]
    public Image blinkCooldownMask;
    public Text blinkCooldownText;
    public Image healCooldownMask;
    public Text healCooldownText;

    // ˽�б���
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;
    private MovementController playerMovement;         // ��������PlayerMovement�����ƶ�
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
    private float originalSpeed;                   // �洢ԭʼ�ƶ��ٶ�

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<MovementController>(); // ��ȡ�ƶ����
        healPerSecond = healAmount / healDuration;

        // �洢ԭʼ�ƶ��ٶ�
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
        if (Input.GetKeyDown(sprintKey)) TrySprint(); // ���Լ���

        UpdateHealing();
        UpdateSprinting(); // ���¼���״̬
        UpdateCooldowns();
    }

    private void TrySprint()
    {
        if (!canSprint || isSprinting) return;

        // ��ʼ����
        isSprinting = true;
        canSprint = false;
        sprintTimer = 0f;

        // �����ƶ��ٶ�
        if (playerMovement != null)
        {
            playerMovement.movementSpeed *= sprintSpeedMultiplier;
        }

        // ��������Ч��
        if (sprintParticles != null)
        {
            sprintParticles.Play();
        }
    }

    private void UpdateSprinting()
    {
        if (!isSprinting) return;

        sprintTimer += Time.deltaTime;

        // ���ܽ���
        if (sprintTimer >= sprintDuration)
        {
            EndSprint();
        }
    }

    private void EndSprint()
    {
        isSprinting = false;
        sprintCooldownTimer = sprintCooldown;

        // �ָ�ԭʼ�ƶ��ٶ�
        if (playerMovement != null)
        {
            playerMovement.movementSpeed = originalSpeed;
        }

        // ֹͣ����Ч��
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
        // ������ȴUI
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

        // ������ȴUI
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

        // ������ȴUI
        if (sprintCooldownMask != null)
        {
            if (isSprinting)
            {
                float sprintRatio = 1 - (sprintTimer / sprintDuration);
                sprintCooldownMask.fillAmount = sprintRatio;
                sprintCooldownText.text = Mathf.Ceil(sprintDuration - sprintTimer).ToString();
                sprintCooldownText.color = new Color(1, 0.8f, 0.5f, 0.8f); // ��ɫ
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