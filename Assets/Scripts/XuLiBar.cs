using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.ComponentModel;
public class XuLiBar : MonoBehaviour
{

    public static XuLiBar instance;
    public bool isfull;
    public Transform target;         // Ҫ����Ľ�ɫ
    public Vector3 offset = new Vector3(0, 2f, 0); // ͷ��ƫ����
    [Header("Charge Settings")]
    [SerializeField] private float maxChargeDuration = 2f;  // �������ʱ��
    [SerializeField] private float minChargeDuration = .3f;  // ��С����ʱ��
    [SerializeField] private Gradient colorGradient;        // ��ɫ��������
    [SerializeField] private AnimationCurve fillCurve;      // ������߿���


    [Header("References")]
    [SerializeField] private Image fillImage;               // Բ�����ͼ��
    [SerializeField] private Image widerBar;
    public float _currentCharge;      // ��ǰ����ֵ��0-1��

    private RectTransform inner;
    private RectTransform outer;
    private float gap;
    private float cx;
    private Color color;


    [Header("Animation")]
    [SerializeField] private Animator chargeAnimator; // ��������������
    private bool _hasTriggeredFullAnimation; // ����Ƿ��Ѵ���������������
    private float _previousCharge; // ��¼��һ֡������ֵ



    [Header("Flash Settings")]
    [SerializeField] private float flashSpeed = 2f; // ��˸Ƶ��
    [SerializeField] private float minAlpha = 0.3f; // ��С͸����
    [SerializeField] private float maxAlpha = 1f;   // ���͸����
    [SerializeField] private Color flashColor1 = Color.yellow; // ��������˸��ɫ1
    [SerializeField] private Color flashColor2 = Color.red;    // ��������˸��ɫ2


    private Color _originalColor;    // ԭʼ��ɫ��ȡ�Խ���ɫ��
    public bool _isFlashing;        // �Ƿ�������˸
    private Coroutine _flashCoroutine; // ��˸Э������


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {

        inner = fillImage.rectTransform;
        outer = widerBar.rectTransform;

        float x = inner.rect.width;
        cx = outer.rect.width;
        gap = cx - x;

        color = widerBar.color;

        _hasTriggeredFullAnimation = false;
        _previousCharge = 0f;

        _originalColor = colorGradient.Evaluate(1f); // ��ȡ������ʱ����ɫ
    }
    void Update()
    {
        // �����ɫ
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Camera.main.transform.rotation; // ʼ���������
        }
    }
    /// <summary>
    /// �ⲿ���ø�������״̬
    /// </summary>
    /// <param name="chargeTime">��ǰ����ʱ�䣨��λ���룩</param>
    public void UpdateCharge(float chargeTime)
    {
        // �����׼�����ȣ�0-1��
        if(chargeTime >= minChargeDuration) { 
            chargeTime -= minChargeDuration; 
        }
        else
        {
            return;
        }
        _currentCharge = Mathf.Clamp01(chargeTime / maxChargeDuration);




        _previousCharge = _currentCharge; // ��¼��ǰ֡����ֵ


        if (_currentCharge >= 1f && !_isFlashing)
        {
            StartFlashing();
            isfull = true;
        }
        else if (_currentCharge < 1f && _isFlashing)
        {
            StopFlashing();
            isfull = false;
        }

        // �����Ӿ�����
        UpdateFillAmount();
        UpdateColor();
        updateWiderBar(_currentCharge*gap);
    }



    private void UpdateFillAmount()
    {
        // Ӧ�����߿���������
        float curvedProgress = fillCurve.Evaluate(_currentCharge);
        fillImage.fillAmount = curvedProgress;

    }

    private void UpdateColor()
    {
        if (_isFlashing) return; // ������˸ʱ��������ɫ

        Color targetColor = colorGradient.Evaluate(_currentCharge);
        fillImage.color = targetColor;
    }
    public void Active(bool active)
    {
        gameObject.SetActive(active);
    }

    public void updateWiderBar(float w)
    {

        float current_width = cx - w;
        // ���ÿ��
        outer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, current_width);

        // ���ø߶�
        outer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, current_width);

       
        if (Mathf.Approximately(w, gap))
        {
            color.a = 0f;
        }
        else
        {
            float progress = w / gap;
            color.a = Mathf.Pow(progress, 3) * 0.5f;
        }
        widerBar.color = color;

    }
    public void PlayFullChargeAnimation()
    {
        if (chargeAnimator != null)
        {
            chargeAnimator.Play("FullCharge", 0, 0f); // ������Ϊ"FullCharge"�Ķ���
        }
    }

    public void StartFlashing()
    {

        _isFlashing = true;
        if (_flashCoroutine == null)
        {
            _flashCoroutine = StartCoroutine(FlashRoutine());
        }
    }
    public void StopFlashing()
    {
        _isFlashing = false;
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
            fillImage.color = _originalColor; // �ָ���������ʱ����ɫ
        }
    }
    private IEnumerator FlashRoutine()
    {
        float timer = 0f;

        while (true)
        {
            // ��������ɫ֮�����ؽ���
            float t = Mathf.PingPong(timer * flashSpeed, 1f);
            fillImage.color = Color.Lerp(flashColor1, flashColor2, t);

            timer += Time.deltaTime;
            yield return null;
        }
    }





}