using UnityEngine;

public class SlipperyArea : MonoBehaviour
{
    public float slowFactor = 0.5f; // 减速比例 (0.5 = 50%速度)
    public float duration = 5f;

    private float timer;

    void Start()
    {
        timer = duration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // 2D游戏必须用 Collider2D 参数！
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Enter: " + other.name);
        if (other.CompareTag("Player"))
        {
            var movement = other.GetComponent<MovementController>();
            if (movement != null)
            {
                movement.ApplySpeedModifier(slowFactor);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var movement = other.GetComponent<MovementController>();
            if (movement != null)
            {
                movement.RemoveSpeedModifier(slowFactor);
            }
        }
    }
}
