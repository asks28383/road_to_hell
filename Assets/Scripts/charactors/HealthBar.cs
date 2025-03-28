using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider HPStrip;
    public static HealthBar Instance;
    public int maxHp;// 角色的最大血量


    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        maxHp = CharacterHealthController.instance.MaxHealthValue;
        HPStrip.value = HPStrip.maxValue = maxHp;
    }
    /// <summary>
    /// present is the current health v
    /// </summary>
    /// <param name="present"></param>
    public void UpdateHealth(int present)
    {
        HPStrip.value = present;
    }
}
