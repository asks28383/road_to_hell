using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;

    [Header("UI Elements")]
    public GameObject primaryHeatBar; // ������������������UI��
    public GameObject secondaryChargeBar; // �������������������壩

    private enum WeaponType { Primary, Secondary }
    private WeaponType currentWeapon = WeaponType.Primary;

    void Start()
    {
        // ��ʼ����ʾ״̬
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
        // �л���������
        currentWeapon = (currentWeapon == WeaponType.Primary) ? WeaponType.Secondary : WeaponType.Primary;
        SetWeaponActive(currentWeapon == WeaponType.Primary);
    }

    private void SetWeaponActive(bool isPrimary)
    {
        // ������������״̬
        primaryWeapon.SetActive(isPrimary);
        secondaryWeapon.SetActive(!isPrimary);

        // ����UI����ʾ״̬
        if (primaryHeatBar != null)
            primaryHeatBar.SetActive(isPrimary);
        if (secondaryChargeBar != null)
            secondaryChargeBar.gameObject.SetActive(!isPrimary);
    }
}