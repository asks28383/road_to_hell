using UnityEngine;

public class Bullet : MonoBehaviour
{
    // ���б���������Unity�༭�������ã�
    [SerializeField] private int damage = 10; // �����˺�ֵ
    public float speed;              // �ӵ������ٶ�
    public GameObject explosionPrefab; // ��ըЧ��Ԥ����
    public float lifetime = 4f;      // �ӵ����ʱ�䣨�룩
    public BulletOwner owner;        // �ӵ�����������

    private float _timer;            // �������ڼ�ʱ��
    private Rigidbody2D rigidbody;   // �ӵ��ĸ������

    // �ӵ�������ö��
    public enum BulletOwner
    {
        Player,
        Boss
    }

    // ��ʼ��ʱ��ȡ���
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// �����ӵ����з���
    /// </summary>
    /// <param name="direction">��׼����ķ��з�������</param>
    public void SetSpeed(Vector2 direction)
    {
        rigidbody.velocity = direction * speed;
    }

    void OnEnable()
    {
        _timer = 0f;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= lifetime)
        {
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    /// <summary>
    /// ��������ײ���
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ������ͬ��Ӫ�ӵ�����ײ
        if (other.CompareTag("Bullet")|| other.CompareTag("Bullet2")) return;

        // �����ӵ������߾�����ײ�߼�
        switch (owner)
        {
            case BulletOwner.Player:
                HandlePlayerBulletCollision(other);
                break;

            case BulletOwner.Boss:
                HandleBossBulletCollision(other);
                break;
        }
    }

    /// <summary>
    /// ��������ӵ�����ײ
    /// </summary>
    private void HandlePlayerBulletCollision(Collider2D other)
    {
        // ֻ��Boss����˺�
        if (!other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Debug.Log("hello");
            SpawnExplosion();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    /// <summary>
    /// ����Boss�ӵ�����ײ
    /// </summary>
    private void HandleBossBulletCollision(Collider2D other)
    {
        // ֻ���������˺�
        if (!other.CompareTag("Boss"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            SpawnExplosion();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    /// <summary>
    /// ���ɱ�ըЧ��
    /// </summary>
    private void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
            exp.transform.position = transform.position;
            exp.transform.rotation = Quaternion.identity;
        }
    }
}