using System.Collections;
using UnityEngine;

/// <summary>
/// 武器调用increaseheat方法时，增加热量，当热量达到100%时，进入过热状态，此时过热条会缓慢减少，直到降到0%。
/// 如果有需要让我直接封装为attack方法，可以告诉我。
public class BulletBarController : MonoBehaviour
{
    public float CdTime = 3.0f; // 过热冷却时间
    public float CoolDownSpeed = 0.5f; // 正常冷却速度
    public float OverheatDecaySpeed = 0.2f; // 过热冷却速度
    private float percent = 0f; // 当前热量 (0 - 1)
    private bool isOverheated = false; // 是否过热
    private bool canbeBonus = true; // 是否触发奖励
    private float bonuspercent;

    private GameObject OverHeat;
    private GameObject UnderHeat;
    private GameObject SurroundingBox;
    private GameObject BonusClock;
    private SpriteRenderer overHeatSprite;
    private SpriteRenderer underHeatSprite;
    private SpriteRenderer BonusClockSprite;

    private float width;  // SurroundingBox的宽度（经过缩放）
    private float barOriginalWidth; // 热量条的初始宽度

    void Start()
    {
        // 获取子对象
        OverHeat = transform.Find("OverHeat").gameObject;
        UnderHeat = transform.Find("UnderHeat").gameObject;
        SurroundingBox = transform.Find("SurroundingBox").gameObject;
        BonusClock = transform.Find("BonusClock").gameObject;

        // 获取 SpriteRenderer
        overHeatSprite = OverHeat.GetComponent<SpriteRenderer>();
        underHeatSprite = UnderHeat.GetComponent<SpriteRenderer>();
        BonusClockSprite = BonusClock.GetComponent<SpriteRenderer>();

        // 获取 SurroundingBox 的宽度，并考虑到它的缩放
        width = SurroundingBox.GetComponent<SpriteRenderer>().bounds.size.x * SurroundingBox.transform.localScale.x;
        barOriginalWidth = underHeatSprite.bounds.size.x;  // 记录初始宽度

        // 初始化状态
        OverHeat.SetActive(false); // 初始时 OverHeat 不可见
        BonusClock.SetActive(false); // 初始时 BonusClock 不可见
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
                OverHeat.SetActive(false);
                UnderHeat.SetActive(true);
                BonusClock.SetActive(false);
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
            BonusClock.SetActive(false);
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
            OverHeat.SetActive(true);
            UnderHeat.SetActive(false);
        }
    }

    private void SetBonusClock()
    {
        bonuspercent = Random.Range(0.25f, 0.75f);
        BonusClock.SetActive(true);
        float minX = (SurroundingBox.transform.localPosition.x - (width / 2))/2; // SurroundingBox 左边界
        float maxX = (SurroundingBox.transform.localPosition.x + (width / 2))/2; // SurroundingBox 右边界
        float bonusX = Mathf.Lerp(minX, maxX, bonuspercent); // 在 25%~75% 之间取值
        BonusClockSprite.transform.localPosition = new Vector3(bonusX, 0, 0);
    }

    /// <summary>
    /// 更新热量条 UI
    /// </summary>
    private void UpdateBar()
    {
        float scaledWidth = width * percent;  // 计算当前热量条目标宽度
        float scaleFactor = scaledWidth / barOriginalWidth;  // 计算缩放比例

        // 计算对齐位置 (基于 SurroundingBox)
        float baseX = SurroundingBox.transform.localPosition.x - width / 2;
        print(baseX);
        float newPositionX = (baseX + scaledWidth /2);

        // 更新 UnderHeat（正常加热状态）
        underHeatSprite.transform.localScale = new Vector3(scaleFactor, 1, 1);
        underHeatSprite.transform.localPosition = new Vector3(newPositionX, 0, 0);

        // 更新 OverHeat（过热状态）
        if (isOverheated)
        {
            overHeatSprite.transform.localScale = new Vector3(scaleFactor, 1, 1);
            overHeatSprite.transform.localPosition = new Vector3(newPositionX, 0, 0);
        }
    }
}
