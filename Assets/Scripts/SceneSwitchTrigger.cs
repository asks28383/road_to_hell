using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitchTrigger : MonoBehaviour
{
    [Header("场景设置")]
    public string targetSceneName = "zx";
    public float requiredStandTime = 1f;

    [Header("视觉效果")]
    public Color activeColor = Color.yellow; // 激活时的颜色
    public float pulseSpeed = 2f;           // 脉冲速度
    public Vector3 pulseScale = new Vector3(0.2f, 0.2f, 0); // 脉冲缩放幅度
    public ParticleSystem triggerEffect;    // 粒子特效（可选）

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

            // 触发初始效果
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

            // 动态效果
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(originalColor, activeColor, pulse);
                transform.localScale = originalScale + pulseScale * pulse;
            }

            // 完成计时
            if (standTimer >= requiredStandTime)
            {
                StartCoroutine(TransitionScene());
            }
        }
    }

    IEnumerator TransitionScene()
    {
        isTransitioning = true;

        // 最终触发效果
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            transform.localScale = originalScale * 1.3f;
        }

        // 等待一帧让玩家看到最终效果
        yield return new WaitForSeconds(0.3f);

        // 加载场景
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

    // 可选：在编辑器中可视化触发区域
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