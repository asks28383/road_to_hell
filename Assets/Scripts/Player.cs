using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;

    [Header("UI Elements")]
    public GameObject primaryHeatBar; // 主武器过热条（独立UI）
    public GameObject secondaryChargeBar; // 副武器蓄力条（子物体）

    private enum WeaponType { Primary, Secondary }
    private WeaponType currentWeapon = WeaponType.Primary;

    void Start()
    {
        // 初始化显示状态
        SetWeaponActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
        }
    }

    private void SwitchWeapon()
    {
        // 切换武器类型
        currentWeapon = (currentWeapon == WeaponType.Primary) ? WeaponType.Secondary : WeaponType.Primary;
        SetWeaponActive(currentWeapon == WeaponType.Primary);
    }

    private void SetWeaponActive(bool isPrimary)
    {
        // 设置武器激活状态
        primaryWeapon.SetActive(isPrimary);
        secondaryWeapon.SetActive(!isPrimary);

        // 设置UI条显示状态
        if (primaryHeatBar != null)
            primaryHeatBar.SetActive(isPrimary);
        if (secondaryChargeBar != null)
            secondaryChargeBar.gameObject.SetActive(!isPrimary);
    }
}