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

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb.position += (mousePos - (Vector2)transform.position).normalized * blinkDistance;

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
        if (blinkCooldownUI)
            blinkCooldownUI.fillAmount = canBlink ? 0 : blinkCooldownTimer / blinkCooldown;

        if (healCooldownUI)
            healCooldownUI.fillAmount = canHeal ? 0 : healCooldownTimer / healCooldown;
    }
}