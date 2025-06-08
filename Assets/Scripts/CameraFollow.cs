using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TopDownCameraFollow : MonoBehaviour
{
    [Header("������������")]
    [Tooltip("Ҫ�����Ŀ��(��ҽ�ɫ)")]
    public Transform target;

    [Tooltip("����ƽ����(ֵԽС����Խ����)")]
    [Range(0.01f, 1f)]
    public float smoothness = 0.2f;

    [Tooltip("��ͷ�߶�(���ӽǾ���)")]
    [Min(5f)]
    public float cameraHeight = 10f;

    [Header("�߽����")]
    [Tooltip("�Ƿ����ñ߽�����")]
    public bool useBounds = false;

    [Tooltip("��С�߽�����")]
    public Vector2 minBounds;

    [Tooltip("���߽�����")]
    public Vector2 maxBounds;

    [Header("��̬����")]
    [Tooltip("�Ƿ�����ٶ��Զ�����")]
    public bool autoZoom = false;

    [Tooltip("��С���ż���")]
    [Min(5f)]
    public float minZoom = 5f;

    [Tooltip("������ż���")]
    [Min(10f)]
    public float maxZoom = 15f;

    [Tooltip("������Ӧ�ٶ�")]
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
            // ��ʼλ��ֱ�Ӷ�׼Ŀ��
            transform.position = CalculateTargetPosition();
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ����Ŀ��λ��
        Vector3 targetPosition = CalculateTargetPosition();

        // ƽ���ƶ�
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothness
        );

        // ��̬���Ŵ���
        HandleAutoZoom();
    }

    Vector3 CalculateTargetPosition()
    {
        Vector3 basePosition = target.position;
        basePosition.z = -cameraHeight; // ����2D���ӽ�

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
            // �����ٶȼ���Ŀ�����ż���
            float speedFactor = Mathf.Clamp01(targetRigidbody.velocity.magnitude / 10f);
            float desiredZoom = Mathf.Lerp(minZoom, maxZoom, speedFactor);
            currentZoom = Mathf.Lerp(currentZoom, desiredZoom, zoomSpeed * Time.deltaTime);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, zoomSpeed * Time.deltaTime);
    }

    // ���Ʊ߽�Gizmos
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

    // �ⲿ���ã������µĸ���Ŀ��
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (newTarget != null)
        {
            targetRigidbody = newTarget.GetComponent<Rigidbody2D>();
        }
    }

    // �ⲿ���ã����ñ߽�
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }
}