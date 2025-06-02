using UnityEngine;

public class Boss : MonoBehaviour
{
    private Transform player;          // 玩家参考
    private SpriteRenderer spriteRenderer; // 精灵渲染器

    void Start()
    {
        // 获取玩家引用
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 确保初始朝向正确（假设默认面朝右）
        spriteRenderer.flipX = false;
    }

    void Update()
    {
        FlipTowardsPlayer();
    }

    // 简单的左右翻转面向玩家
    private void FlipTowardsPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            // 玩家在Boss右侧 - 面朝右（不翻转）
            spriteRenderer.flipX = false;
        }
        else
        {
            // 玩家在Boss左侧 - 面朝左（翻转）
            spriteRenderer.flipX = true;
        }
    }
}