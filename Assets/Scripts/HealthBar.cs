using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider HPStrip;
    public static HealthBar Instance;
    public int maxHp;// 角色的最大血量
    private Health playerHealth;
    public Image fillImage;


    [Header("Color Settings")]
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    [Header("Thresholds (0-1)")]
    [Range(0, 1)] public float mediumHealthThreshold = 0.5f;
    [Range(0, 1)] public float lowHealthThreshold = 0.2f;

    private void Awake()
    {
        Instance = this;
        playerHealth= GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }
    void Start()
    {
        maxHp = playerHealth.maxHealth;
        HPStrip.value = HPStrip.maxValue = maxHp;
    }
    private void Update()
    {
        UpdateHealth(playerHealth.currentHealth);
    }
    /// <summary>
    /// present is the current health v
    /// </summary>
    /// <param name="present"></param>
    public void UpdateHealth(int present)
    {
        HPStrip.value = present;
        UpdateHealthBarColor();
    }
    private void UpdateHealthBarColor()
    {
        if (fillImage == null) return;

        float healthPercentage = HPStrip.value / HPStrip.maxValue;

        if (healthPercentage <= lowHealthThreshold)
        {
            fillImage.color = lowHealthColor;
        }
        else if (healthPercentage <= mediumHealthThreshold)
        {
            fillImage.color = mediumHealthColor;
        }
        else
        {
            fillImage.color = highHealthColor;
        }
    }
}
