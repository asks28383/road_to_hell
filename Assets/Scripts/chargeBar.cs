using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class chargeBar : MonoBehaviour
{
    // �����������
    public RangedWeapon weapon;
    public Sprite cdimage;
    public Sprite punishimage;
    private Sprite oldimage;
    public float CdTime = 3.0f; // ������ȴʱ��
    public float CoolDownSpeed = 0.5f; // ������ȴ�ٶ�
    public float OverheatDecaySpeed = 0.2f; // ������ȴ�ٶ�
    private float percent = 0f; // ��ǰ���� (0 - 1)
    private bool isOverheated = false; // �Ƿ����
    private bool canbeBonus = true; // �Ƿ񴥷�����
    private float bonuspercent;

    private Slider slide;
    private Image SurroundingBox;
    private Image BonusClock;

    private float width;  // chargebar�Ŀ�ȣ��������ţ�
    private float barOriginalWidth; // �������ĳ�ʼ���

    void Start()
    {
        slide = GetComponent<Slider>();
        BonusClock = transform.Find("BonusClock").GetComponent<Image>();
        width = transform.GetComponent<RectTransform>().sizeDelta.x;
        oldimage = transform.Find("Fill Area").Find("Fill").GetComponent<Image>().sprite;
        BonusClock.gameObject.SetActive(false); // ��ʼʱ BonusClock ���ɼ�

        UpdateBar();
    }

    void Update()
    {
        if(percent<=0.01f)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            //transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
            //transform.GetChild(3).gameObject.SetActive(true);
        }

        //if (weapon.get_flag())
        //{
        //    IncreaseHeat(0.15f);
        //}
        //Debug.Log(isOverheated);
        if (isOverheated)
        {
            // ����ʱ��OverHeat ��������
            percent -= OverheatDecaySpeed * Time.deltaTime;
            if (percent <= 0)
            {
                percent = 0;
                isOverheated = false; // �������
                BonusClock.gameObject.SetActive(false); // ���� BonusClock
                canbeBonus = true;
                //����������
                slide.fillRect.GetComponent<Image>().sprite = oldimage;
            }
            // ������ֻ�ڹ���ʱ���ո��У׼
            if (canbeBonus)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TryCalibrate();
                }
            }

        }
        else
        {
            // ����״̬������ȴ
            if (percent > 0)
            {
                percent -= CoolDownSpeed * Time.deltaTime;
                if (percent < 0)
                {
                    percent = 0;
                }
            }
        }

        // ���� UI
        UpdateBar();
    }

    private void TryCalibrate()
    {
        // ����Ƿ���У׼������
        if (Mathf.Abs(percent - bonuspercent) < 0.1f)
        {
            // �ɹ�У׼
            percent = 0;
            isOverheated = false;
            BonusClock.gameObject.SetActive(false);
            canbeBonus = true;
            slide.fillRect.GetComponent<Image>().sprite = oldimage;
        }
        else
        {
            // У׼ʧ�ܳͷ�
            canbeBonus = false;
            slide.fillRect.GetComponent<Image>().sprite = punishimage;
            BonusClock.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void IncreaseHeat(float amount)
    {
        if (isOverheated && canbeBonus)
        {
            ;
        }
        if (isOverheated)
            return; // ����ʱ������

        percent += amount;
        if (percent >= 1f)
        {
            percent = 1f;
            CheckOverheat(); // ����Ƿ����
        }

        UpdateBar();
    }

    /// <summary>
    /// ����Ƿ�������״̬
    /// </summary>
    public void CheckOverheat()
    {
        if (percent >= 1f)
        {
            isOverheated = true;
            SetBonusClock();
            //���������
            slide.fillRect.GetComponent<Image>().sprite = cdimage;
        }
    }

    private void SetBonusClock()
    {
        bonuspercent = Random.Range(0.45f, 0.8f);
        BonusClock.gameObject.transform.localPosition = new Vector3((bonuspercent - 0.5f) * width, 0, 0);
        BonusClock.gameObject.SetActive(true);
    }

    /// <summary>
    /// ���������� UI
    /// </summary>
    private void UpdateBar()
    {
        slide.value = percent;
    }
    public bool IsWeaponOverheated()
    {
        return isOverheated;
    }
    public float get_percent()
    {
        return percent;
    }

}
