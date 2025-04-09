using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bullet : MonoBehaviour
{
    // ���б���������Unity�༭�������ã�
    [SerializeField] private int damage = 10; // �����˺�ֵ
    public float speed;              // �ӵ������ٶ�
    public GameObject explosionPrefab; // ��ըЧ��Ԥ����
    public float lifetime = 4f;      // �ӵ����ʱ�䣨�룩

    private float _timer;            // �������ڼ�ʱ��
    new private Rigidbody2D rigidbody; // �ӵ��ĸ��������new�ؼ����������ظ���ͬ����Ա��

    // ��ʼ��ʱ��ȡ���
    void Awake()
    {
        // ��ȡ��ǰ��Ϸ�����ϵ�Rigidbody2D���
        rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// �����ӵ����з���
    /// </summary>
    /// <param name="direction">��׼����ķ��з�������</param>
    public void SetSpeed(Vector2 direction)
    {
        // ͨ�����������ӵ��ٶȣ����� * �ٶ�ֵ��
        rigidbody.velocity = direction * speed;
    }

    void OnEnable()
    {
        // ÿ�δӶ����ȡ��ʱ���ü�ʱ��
        _timer = 0f;
    }
    void Update()
    {
        // ���ڴ˴�����ӵ��������ڼ�ʱ�����߼�
        _timer += Time.deltaTime;
        // ����Ƿ񳬹���������
        if (_timer >= lifetime)
        {
            // ʹ�ö���ػ����ӵ�����
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    /// <summary>
    /// ��������ײ��⣨���ӵ�����������ײ��ʱִ�У�
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �����еĶ����Ƿ���Health���
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        // ʹ�ö���ػ�ȡ��ըЧ��ʵ�������Instantiate��
        GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
        // ���ñ�ըЧ��λ��Ϊ��ǰ�ӵ�λ��
        exp.transform.position = transform.position;
        // ��Ҫȷ����ըЧ�������ڳ�ʼ��ʱ�Զ�����

        // ʹ�ö���ػ����ӵ��������Destroy��
        ObjectPool.Instance.PushObject(gameObject);
    }
}
