using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TopDownCameraFollow : MonoBehaviour
{
    [Header("基础跟随设置")]
    [Tooltip("要跟随的目标(玩家角色)")]
    public Transform target;

    [Tooltip("跟随平滑度(值越小跟随越紧密)")]
    [Range(0.01f, 1f)]
    public float smoothness = 0.2f;

    [Tooltip("镜头高度(俯视角距离)")]
    [Min(5f)]
    public float cameraHeight = 10f;

    [Header("边界控制")]
    [Tooltip("是否启用边界限制")]
    public bool useBounds = false;

    [Tooltip("最小边界坐标")]
    public Vector2 minBounds;

    [Tooltip("最大边界坐标")]
    public Vector2 maxBounds;

    [Header("动态缩放")]
    [Tooltip("是否根据速度自动缩放")]
    public bool autoZoom = false;

    [Tooltip("最小缩放级别")]
    [Min(5f)]
    public float minZoom = 5f;

    [Tooltip("最大缩放级别")]
    [Min(10f)]
    public float maxZoom = 15f;

    [Tooltip("缩放响应速度")]
    [Range(0.1f, 5f)]
    public float zoomSpeed = 2f;

    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private Rigidbody2D targetRigidbody;
    private float currentZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
        currentZoom = cameraHeight;

        if (target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody2D>();
            // 初始位置直接对准目标
            transform.position = CalculateTargetPosition();
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 计算目标位置
        Vector3 targetPosition = CalculateTargetPosition();

        // 平滑移动
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothness
        );

        // 动态缩放处理
        HandleAutoZoom();
    }

    Vector3 CalculateTargetPosition()
    {
        Vector3 basePosition = target.position;
        basePosition.z = -cameraHeight; // 保持2D俯视角

        if (useBounds)
        {
            basePosition.x = Mathf.Clamp(basePosition.x, minBounds.x, maxBounds.x);
            basePosition.y = Mathf.Clamp(basePosition.y, minBounds.y, maxBounds.y);
        }

        return basePosition;
    }

    void HandleAutoZoom()
    {
        if (!autoZoom)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, zoomSpeed * Time.deltaTime);
            return;
        }

        if (targetRigidbody != null)
        {
            // 根据速度计算目标缩放级别
            float speedFactor = Mathf.Clamp01(targetRigidbody.velocity.magnitude / 10f);
            float desiredZoom = Mathf.Lerp(minZoom, maxZoom, speedFactor);
            currentZoom = Mathf.Lerp(currentZoom, desiredZoom, zoomSpeed * Time.deltaTime);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, zoomSpeed * Time.deltaTime);
    }

    // 绘制边界Gizmos
    void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) * 0.5f,
            (minBounds.y + maxBounds.y) * 0.5f,
            0
        );
        Vector3 size = new Vector3(
            maxBounds.x - minBounds.x,
            maxBounds.y - minBounds.y,
            0.1f
        );
        Gizmos.DrawWireCube(center, size);
    }

    // 外部调用：设置新的跟随目标
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (newTarget != null)
        {
            targetRigidbody = newTarget.GetComponent<Rigidbody2D>();
        }
    }

    // 外部调用：设置边界
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }
}