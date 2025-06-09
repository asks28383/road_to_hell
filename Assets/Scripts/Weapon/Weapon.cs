using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // 配置参数
    public float interval;
    public float damage;

    // 组件引用
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    // 方向控制
    protected Vector2 mousePos;
    protected Vector2 direction;
    protected float timer;
    protected bool facingRight = true;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 如果武器是子对象，确保父级没有缩放
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
        // 使用SpriteRenderer.flipX实现翻转，避免修改scale
        bool shouldFaceRight = mousePos.x >= transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            spriteRenderer.flipX = !facingRight;
        }

        // 计算方向（不受翻转影响）
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