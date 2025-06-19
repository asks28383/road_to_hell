using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class chargeBar : MonoBehaviour
{
    // 添加武器引用
    public RangedWeapon weapon;
    public Sprite cdimage;
    public Sprite punishimage;
    private Sprite oldimage;
    public float CdTime = 3.0f; // 过热冷却时间
    public float CoolDownSpeed = 0.5f; // 正常冷却速度
    public float OverheatDecaySpeed = 0.2f; // 过热冷却速度
    private float percent = 0f; // 当前热量 (0 - 1)
    private bool isOverheated = false; // 是否过热
    private bool canbeBonus = true; // 是否触发奖励
    private float bonuspercent;

    private Slider slide;
    private Image SurroundingBox;
    private Image BonusClock;

    private float width;  // chargebar的宽度（经过缩放）
    private float barOriginalWidth; // 热量条的初始宽度

    void Start()
    {
        slide = GetComponent<Slider>();
        BonusClock = transform.Find("BonusClock").GetComponent<Image>();
        width = transform.GetComponent<RectTransform>().sizeDelta.x;
        oldimage = transform.Find("Fill Area").Find("Fill").GetComponent<Image>().sprite;
        BonusClock.gameObject.SetActive(false); // 初始时 BonusClock 不可见

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
            // 过热时，OverHeat 缓慢减少
            percent -= OverheatDecaySpeed * Time.deltaTime;
            if (percent <= 0)
            {
                percent = 0;
                isOverheated = false; // 解除过热
                BonusClock.gameObject.SetActive(false); // 隐藏 BonusClock
                canbeBonus = true;
                //滑动条变绿
                slide.fillRect.GetComponent<Image>().sprite = oldimage;
            }
            // 新增：只在过热时检测空格键校准
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
            // 正常状态下逐渐冷却
            if (percent > 0)
            {
                percent -= CoolDownSpeed * Time.deltaTime;
                if (percent < 0)
                {
                    percent = 0;
                }
            }
        }

        // 更新 UI
        UpdateBar();
    }

    private void TryCalibrate()
    {
        // 检查是否在校准区域内
        if (Mathf.Abs(percent - bonuspercent) < 0.1f)
        {
            // 成功校准
            percent = 0;
            isOverheated = false;
            BonusClock.gameObject.SetActive(false);
            canbeBonus = true;
            slide.fillRect.GetComponent<Image>().sprite = oldimage;
        }
        else
        {
            // 校准失败惩罚
            canbeBonus = false;
            slide.fillRect.GetComponent<Image>().sprite = punishimage;
            BonusClock.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 增加热量
    /// </summary>
    public void IncreaseHeat(float amount)
    {
        if (isOverheated && canbeBonus)
        {
            ;
        }
        if (isOverheated)
            return; // 过热时不增加

        percent += amount;
        if (percent >= 1f)
        {
            percent = 1f;
            CheckOverheat(); // 检查是否过热
        }

        UpdateBar();
    }

    /// <summary>
    /// 检查是否进入过热状态
    /// </summary>
    public void CheckOverheat()
    {
        if (percent >= 1f)
        {
            isOverheated = true;
            SetBonusClock();
            //滑动条变红
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
    /// 更新热量条 UI
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
