using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitchTrigger : MonoBehaviour
{
    [Header("��������")]
    public string targetSceneName = "zx";
    public float requiredStandTime = 1f;

    [Header("�Ӿ�Ч��")]
    public Color activeColor = Color.yellow; // ����ʱ����ɫ
    public float pulseSpeed = 2f;           // �����ٶ�
    public Vector3 pulseScale = new Vector3(0.2f, 0.2f, 0); // �������ŷ���
    public ParticleSystem triggerEffect;    // ������Ч����ѡ��

    private float standTimer = 0f;
    private bool isPlayerStanding = false;
    private Color originalColor;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private bool isTransitioning = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            originalMaterial = spriteRenderer.material;
        }
        originalScale = transform.localScale;

        if (triggerEffect != null)
            triggerEffect.Stop();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isPlayerStanding = true;
            standTimer = 0f;

            // ������ʼЧ��
            if (triggerEffect != null)
                triggerEffect.Play();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ResetEffects();
            isPlayerStanding = false;
            standTimer = 0f;
        }
    }

    void Update()
    {
        if (isPlayerStanding && !isTransitioning)
        {
            standTimer += Time.deltaTime;

            // ��̬Ч��
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(originalColor, activeColor, pulse);
                transform.localScale = originalScale + pulseScale * pulse;
            }

            // ��ɼ�ʱ
            if (standTimer >= requiredStandTime)
            {
                StartCoroutine(TransitionScene());
            }
        }
    }

    IEnumerator TransitionScene()
    {
        isTransitioning = true;

        // ���մ���Ч��
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            transform.localScale = originalScale * 1.3f;
        }

        // �ȴ�һ֡����ҿ�������Ч��
        yield return new WaitForSeconds(0.3f);

        // ���س���
        SceneManager.LoadScene(targetSceneName);
    }

    void ResetEffects()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
            transform.localScale = originalScale;
        }
        if (triggerEffect != null)
            triggerEffect.Stop();
    }

    // ��ѡ���ڱ༭���п��ӻ���������
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            if (collider is BoxCollider2D)
            {
                Gizmos.DrawCube(transform.position + (Vector3)((BoxCollider2D)collider).offset,
                               ((BoxCollider2D)collider).size);
            }
            else if (collider is CircleCollider2D)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)((CircleCollider2D)collider).offset,
                                  ((CircleCollider2D)collider).radius);
            }
        }
    }
}