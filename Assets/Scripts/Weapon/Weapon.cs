using UnityEngine;

// 武器基类，抽象类，不能被直接实例化
// 所有具体武器类型应该继承这个类并实现抽象方法
public abstract class Weapon : MonoBehaviour
{
    // ========== 公有配置参数 ==========

    // 攻击间隔时间（秒），控制武器攻击频率
    public float interval;

    // ========== 受保护的成员变量 ==========

    // 存储鼠标的世界坐标位置
    protected Vector2 mousePos;

    // 标准化后的攻击方向向量（长度为1）
    protected Vector2 direction;

    // 攻击间隔计时器，用于控制攻击冷却
    protected float timer;

    // 记录原始Y轴缩放值，用于武器翻转计算
    protected float flipY;

    // 控制武器动画的Animator组件引用
    protected Animator animator;

    // ========== Unity生命周期方法 ==========

    /// <summary>
    /// 初始化方法，virtual表示子类可以重写
    /// </summary>
    protected virtual void Start()
    {
        // 获取附加在同一GameObject上的Animator组件
        animator = GetComponent<Animator>();
        // 记录初始的Y轴缩放值，用于后续的武器翻转计算
        flipY = transform.localScale.y;
    }

    /// <summary>
    /// 每帧更新逻辑，virtual表示子类可以重写
    /// </summary>
    protected virtual void Update()
    {
        // 将鼠标的屏幕坐标转换为世界坐标
        // Camera.main指向场景中标记为"MainCamera"的相机
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 根据鼠标位置翻转武器（左右方向）
        // 如果鼠标在武器左侧，翻转武器（通过改变Y轴缩放实现）
        if (mousePos.x < transform.position.x)
            transform.localScale = new Vector3(flipY, -flipY, 1);
        else
            // 鼠标在右侧时恢复正常朝向
            transform.localScale = new Vector3(flipY, flipY, 1);

        // 计算标准化的攻击方向（长度为1的向量）
        // 从武器位置指向鼠标位置的方向
        direction = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;

        // 将武器的右方向设置为攻击方向（使武器指向鼠标）
        transform.right = direction;

        // 攻击冷却计时逻辑
        if (timer != 0)
        {
            // 减少计时器时间（Time.deltaTime是上一帧的时间）
            timer -= Time.deltaTime;
            // 确保计时器不低于0
            if (timer <= 0) timer = 0;
        }
        // 调用攻击处理逻辑（由子类具体实现）
        HandleAttack();
    }

    // ========== 抽象方法 ==========

    /// <summary>
    /// 攻击处理抽象方法，子类必须实现具体的攻击逻辑
    /// </summary>
    protected abstract void HandleAttack();

    // ========== 受保护的通用方法 ==========

    /// <summary>
    /// 触发攻击动画的通用方法
    /// </summary>
    /// <param name="triggerName">动画触发器名称，默认为"Attack"</param>
    protected void TriggerAttackAnimation(string triggerName = "Attack")
    {
        // 设置Animator上的触发器，触发攻击动画
        animator.SetTrigger(triggerName);
    }
}
