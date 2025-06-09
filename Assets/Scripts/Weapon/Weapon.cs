using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // ���ò���
    public float interval;
    public float damage;

    // �������
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    // �������
    protected Vector2 mousePos;
    protected Vector2 direction;
    protected float timer;
    protected bool facingRight = true;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ����������Ӷ���ȷ������û������
        if (transform.parent != null)
        {
            transform.parent.localScale = Vector3.one;
        }
    }

    protected virtual void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        UpdateWeaponDirection();
        UpdateAttackCooldown();
        HandleAttack();
    }

    protected void UpdateWeaponDirection()
    {
        // ʹ��SpriteRenderer.flipXʵ�ַ�ת�������޸�scale
        bool shouldFaceRight = mousePos.x >= transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            spriteRenderer.flipX = !facingRight;
        }

        // ���㷽�򣨲��ܷ�תӰ�죩
        direction = (mousePos - (Vector2)transform.position).normalized;
        transform.right = facingRight ? direction : -direction;
    }

    protected void UpdateAttackCooldown()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) timer = 0;
        }
    }

    protected abstract void HandleAttack();

    protected void TriggerAttackAnimation(string triggerName = "Attack")
    {
        animator.SetTrigger(triggerName);
    }
}