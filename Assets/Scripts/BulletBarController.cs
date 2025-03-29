using System.Collections;
using UnityEngine;

/// <summary>
/// ��������increaseheat����ʱ�������������������ﵽ100%ʱ���������״̬����ʱ�������Ỻ�����٣�ֱ������0%��
/// �������Ҫ����ֱ�ӷ�װΪattack���������Ը����ҡ�
public class BulletBarController : MonoBehaviour
{
    public float CdTime = 3.0f; // ������ȴʱ��
    public float CoolDownSpeed = 0.5f; // ������ȴ�ٶ�
    public float OverheatDecaySpeed = 0.2f; // ������ȴ�ٶ�
    private float percent = 0f; // ��ǰ���� (0 - 1)
    private bool isOverheated = false; // �Ƿ����
    private bool canbeBonus = true; // �Ƿ񴥷�����
    private float bonuspercent;

    private GameObject OverHeat;
    private GameObject UnderHeat;
    private GameObject SurroundingBox;
    private GameObject BonusClock;
    private SpriteRenderer overHeatSprite;
    private SpriteRenderer underHeatSprite;
    private SpriteRenderer BonusClockSprite;

    private float width;  // SurroundingBox�Ŀ�ȣ��������ţ�
    private float barOriginalWidth; // �������ĳ�ʼ���

    void Start()
    {
        // ��ȡ�Ӷ���
        OverHeat = transform.Find("OverHeat").gameObject;
        UnderHeat = transform.Find("UnderHeat").gameObject;
        SurroundingBox = transform.Find("SurroundingBox").gameObject;
        BonusClock = transform.Find("BonusClock").gameObject;

        // ��ȡ SpriteRenderer
        overHeatSprite = OverHeat.GetComponent<SpriteRenderer>();
        underHeatSprite = UnderHeat.GetComponent<SpriteRenderer>();
        BonusClockSprite = BonusClock.GetComponent<SpriteRenderer>();

        // ��ȡ SurroundingBox �Ŀ�ȣ������ǵ���������
        width = SurroundingBox.GetComponent<SpriteRenderer>().bounds.size.x * SurroundingBox.transform.localScale.x;
        barOriginalWidth = underHeatSprite.bounds.size.x;  // ��¼��ʼ���

        // ��ʼ��״̬
        OverHeat.SetActive(false); // ��ʼʱ OverHeat ���ɼ�
        BonusClock.SetActive(false); // ��ʼʱ BonusClock ���ɼ�
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
            // ����ʱ��OverHeat ��������
            percent -= OverheatDecaySpeed * Time.deltaTime;
            if (percent <= 0)
            {
                percent = 0;
                isOverheated = false; // �������
                OverHeat.SetActive(false);
                UnderHeat.SetActive(true);
                BonusClock.SetActive(false);
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

    /// <summary>
    /// ��������
    /// </summary>
    public void IncreaseHeat(float amount)
    {
        if (isOverheated && canbeBonus)
        {
            // ����ʱ������Ƿ񴥷����������߽������ʱ��������
            if (Mathf.Abs(percent - bonuspercent) < 0.1f)
            {
                percent = 0;

            }
            canbeBonus = false;
            BonusClock.SetActive(false);
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
        float minX = (SurroundingBox.transform.localPosition.x - (width / 2))/2; // SurroundingBox ��߽�
        float maxX = (SurroundingBox.transform.localPosition.x + (width / 2))/2; // SurroundingBox �ұ߽�
        float bonusX = Mathf.Lerp(minX, maxX, bonuspercent); // �� 25%~75% ֮��ȡֵ
        BonusClockSprite.transform.localPosition = new Vector3(bonusX, 0, 0);
    }

    /// <summary>
    /// ���������� UI
    /// </summary>
    private void UpdateBar()
    {
        float scaledWidth = width * percent;  // ���㵱ǰ������Ŀ����
        float scaleFactor = scaledWidth / barOriginalWidth;  // �������ű���

        // �������λ�� (���� SurroundingBox)
        float baseX = SurroundingBox.transform.localPosition.x - width / 2;
        print(baseX);
        float newPositionX = (baseX + scaledWidth /2);

        // ���� UnderHeat����������״̬��
        underHeatSprite.transform.localScale = new Vector3(scaleFactor, 1, 1);
        underHeatSprite.transform.localPosition = new Vector3(newPositionX, 0, 0);

        // ���� OverHeat������״̬��
        if (isOverheated)
        {
            overHeatSprite.transform.localScale = new Vector3(scaleFactor, 1, 1);
            overHeatSprite.transform.localPosition = new Vector3(newPositionX, 0, 0);
        }
    }
}
