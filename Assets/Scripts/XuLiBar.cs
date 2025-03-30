using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class XuLiBar : MonoBehaviour
{
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
    private float _currentCharge;      // ��ǰ����ֵ��0-1��

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

    private Color _originalColor;    // ԭʼ��ɫ��ȡ�Խ���ɫ��
    private bool _isFlashing;        // �Ƿ�������˸
    private Coroutine _flashCoroutine; // ��˸Э������



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



        // ����Ƿ��״δﵽ������
        if (Mathf.Approximately(_currentCharge, 1f) && !Mathf.Approximately(_previousCharge, 1f))
        {
            if (!_hasTriggeredFullAnimation)
            {
                PlayFullChargeAnimation();
                _hasTriggeredFullAnimation = true;
            }
        }
        // �����ǰ���Ȳ������ģ������ñ��
        else if (_currentCharge < 1f)
        {
            _hasTriggeredFullAnimation = false;
        }

        _previousCharge = _currentCharge; // ��¼��ǰ֡����ֵ


        if (_currentCharge >= 1f && !_isFlashing)
        {
            StartFlashing();
        }
        else if (_currentCharge < 1f && _isFlashing)
        {
            StopFlashing();
        }

        // �����Ӿ�����
        UpdateFillAmount();
        UpdateColor();
        updateWiderBar(_currentCharge*gap);
    }

    /// <summary>
    /// ��������״̬
    /// </summary>
    public void ResetCharge()
    {
        _currentCharge = 0f;
        _hasTriggeredFullAnimation = false;
        _previousCharge = 0f;
        UpdateFillAmount();
        UpdateColor();
        StopFlashing();
    }

    private void UpdateFillAmount()
    {
        // Ӧ�����߿���������
        float curvedProgress = fillCurve.Evaluate(_currentCharge);
        fillImage.fillAmount = curvedProgress;

    }

    private void UpdateColor()
    {
        // ������ɫ����
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

        //���ò�͸����
        if (color.a < 1)
        {
            color.a = (w / gap) * (w / gap) * (w / gap);
            widerBar.color = color;
        }
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
            fillImage.color = _originalColor; // �ָ�ԭʼ��ɫ
        }
    }
    private IEnumerator FlashRoutine()
    {
        Color baseColor = _originalColor;
        float timer = 0f;

        while (true)
        {
            // ʹ��PingPong����ʵ�����ؽ���
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(timer * flashSpeed, 1f));
            fillImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

            timer += Time.deltaTime;
            yield return null;
        }
    }
}