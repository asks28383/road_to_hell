using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class chargeBar : MonoBehaviour
{
    private Slider slider;
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
        if (Input.GetMouseButtonDown(0))
        {
            IncreaseHeat(0.2f);
        }
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

    /// <summary>
    /// 增加热量
    /// </summary>
    public void IncreaseHeat(float amount)
    {
        if (isOverheated && canbeBonus)
        {
            // 过热时，检查是否触发奖励，两者进度相近时触发奖励
            if (Mathf.Abs(percent - bonuspercent) < 0.1f)
            {
                percent = 0;

            }
            canbeBonus = false;
            slide.fillRect.GetComponent<Image>().sprite = punishimage;
            BonusClock.gameObject.SetActive(false);
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
    private void CheckOverheat()
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
        bonuspercent = Random.Range(0.25f, 0.75f);
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

}
