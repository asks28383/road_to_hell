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
    public float healAmount = 15f;      // ��������
    public float healDuration = 5f;     // ���Ƴ���ʱ��
    public float healCooldown = 10f;    // ��ȴʱ��

    [Header("CoolDown UI")]
    public Image blinkCooldownMask;  // ������ȴ����
    public Text blinkCooldownText;   // ������ȴ�ı�
    public Image healCooldownMask;   // ������ȴ����
    public Text healCooldownText;    // ������ȴ�ı�

    // ˽�б���
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;
    private bool canBlink = true;
    private bool canHeal = true;
    private float blinkCooldownTimer;
    private float healCooldownTimer;
    private bool isHealing = false;
    private float healTimer;
    private float healPerSecond;  // ÿ��������

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        healPerSecond = healAmount / healDuration; // ����ÿ��������
        UpdateCooldownUI();
        SetWeaponActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) SwitchWeapon();
        if (Input.GetKeyDown(blinkKey)) TryBlink();
        if (Input.GetKeyDown(healKey) && canHeal && !isHealing) StartHealing();

        UpdateHealing();  // �������ƹ���
        UpdateCooldowns(); // ������ȴ
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

        // ÿ֡�ָ�Ѫ��������/����ʱ�� * Time.deltaTime��
        if (playerHealth != null)
        {
            playerHealth.Recover(healPerSecond * Time.deltaTime);
        }

        // ���ƽ���
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

        // ��ȡ��ɫ��ǰ���ƶ�����
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // ���û�����룬��ʹ�ý�ɫ��ǰ�泯������������ƶ��ķ���
        if (moveHorizontal == 0 && moveVertical == 0)
        {
            // ���Ի�ȡ��ɫ��ǰ��SpriteRenderer.flipX��transform.localScale.x���ж��泯����
            // �����������Ϊ������
            moveHorizontal = transform.localScale.x > 0 ? 1 : -1;
        }

        // �����ƶ�������������һ��
        Vector2 moveDirection = new Vector2(moveHorizontal, moveVertical).normalized;

        // �����ȫû�з��������ϲ��ᷢ��������Ĭ����������
        if (moveDirection == Vector2.zero)
        {
            moveDirection = Vector2.right;
        }

        // Ӧ������
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

        if (!canHeal && !isHealing) // ֻ��������ȫ������ſ�ʼ��ȴ��ʱ
        {
            healCooldownTimer -= Time.deltaTime;
            if (healCooldownTimer <= 0f) canHeal = true;
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

                // ��ȴ�����ʱ��˸Ч��
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
                // ��������ʾ����
                float healRatio = 1 - (healTimer / healDuration);
                healCooldownMask.fillAmount = healRatio;
                healCooldownText.text = Mathf.Ceil(healDuration - healTimer).ToString();
                healCooldownText.color = new Color(0.5f, 1, 0.5f, 0.8f); // ǳ��ɫ
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