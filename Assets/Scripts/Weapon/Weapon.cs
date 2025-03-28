using UnityEngine;

// �������࣬�����࣬���ܱ�ֱ��ʵ����
// ���о�����������Ӧ�ü̳�����ಢʵ�ֳ��󷽷�
public abstract class Weapon : MonoBehaviour
{
    // ========== �������ò��� ==========

    // �������ʱ�䣨�룩��������������Ƶ��
    public float interval;

    // ========== �ܱ����ĳ�Ա���� ==========

    // �洢������������λ��
    protected Vector2 mousePos;

    // ��׼����Ĺ�����������������Ϊ1��
    protected Vector2 direction;

    // ���������ʱ�������ڿ��ƹ�����ȴ
    protected float timer;

    // ��¼ԭʼY������ֵ������������ת����
    protected float flipY;

    // ��������������Animator�������
    protected Animator animator;

    // ========== Unity�������ڷ��� ==========

    /// <summary>
    /// ��ʼ��������virtual��ʾ���������д
    /// </summary>
    protected virtual void Start()
    {
        // ��ȡ������ͬһGameObject�ϵ�Animator���
        animator = GetComponent<Animator>();
        // ��¼��ʼ��Y������ֵ�����ں�����������ת����
        flipY = transform.localScale.y;
    }

    /// <summary>
    /// ÿ֡�����߼���virtual��ʾ���������д
    /// </summary>
    protected virtual void Update()
    {
        // ��������Ļ����ת��Ϊ��������
        // Camera.mainָ�򳡾��б��Ϊ"MainCamera"�����
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // �������λ�÷�ת���������ҷ���
        // ��������������࣬��ת������ͨ���ı�Y������ʵ�֣�
        if (mousePos.x < transform.position.x)
            transform.localScale = new Vector3(flipY, -flipY, 1);
        else
            // ������Ҳ�ʱ�ָ���������
            transform.localScale = new Vector3(flipY, flipY, 1);

        // �����׼���Ĺ������򣨳���Ϊ1��������
        // ������λ��ָ�����λ�õķ���
        direction = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;

        // ���������ҷ�������Ϊ��������ʹ����ָ����꣩
        transform.right = direction;

        // ������ȴ��ʱ�߼�
        if (timer != 0)
        {
            // ���ټ�ʱ��ʱ�䣨Time.deltaTime����һ֡��ʱ�䣩
            timer -= Time.deltaTime;
            // ȷ����ʱ��������0
            if (timer <= 0) timer = 0;
        }
        // ���ù��������߼������������ʵ�֣�
        HandleAttack();
    }

    // ========== ���󷽷� ==========

    /// <summary>
    /// ����������󷽷����������ʵ�־���Ĺ����߼�
    /// </summary>
    protected abstract void HandleAttack();

    // ========== �ܱ�����ͨ�÷��� ==========

    /// <summary>
    /// ��������������ͨ�÷���
    /// </summary>
    /// <param name="triggerName">�������������ƣ�Ĭ��Ϊ"Attack"</param>
    protected void TriggerAttackAnimation(string triggerName = "Attack")
    {
        // ����Animator�ϵĴ�������������������
        animator.SetTrigger(triggerName);
    }
}
