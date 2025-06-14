using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class BossSodaCharge : Action
{
    [Header("Charge Settings")]
    public int maxCharges = 3;
    public float chargeSpeed = 15f;
    public float chargeDuration = 0.5f;
    public float preparationTime = 0.5f;
    public float postChargeDelay = 0.8f;

    [Header("Soda Trail")]
    public GameObject sodaTrailPrefab;
    public float trailSpawnInterval = 0.1f;

    [Header("Damage Settings")]
    public int chargeDamage = 1;
    public float damageCooldown = 0.5f;
    public float knockbackForce = 5f;

    // Private variables
    private int currentCharges;
    private float chargeTimer;
    private float preparationTimer;
    private float delayTimer;
    private Vector2 chargeDirection;
    private float lastTrailSpawnTime;
    private float lastDamageTime;
    private Transform player;
    private bool isPreparing;
    private bool isCharging;
    private Rigidbody2D rb;
    private Animator animator;
    private const string IsDashingParam = "isDashing";
    public GameObject boss;
    public override void OnStart()
    {
        boss.GetComponent<BulletConfig>().enabled = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Initialize object pool
        if (!ObjectPool.Instance.HasPool(sodaTrailPrefab.name))
        {
            ObjectPool.Instance.PrewarmPool(sodaTrailPrefab, 10);
        }

        currentCharges = 0;
        StartPreparation();
    }

    public override TaskStatus OnUpdate()
    {
        if (isPreparing)
        {
            preparationTimer -= Time.deltaTime;
            if (preparationTimer <= 0)
            {
                DetermineChargeDirection();
                StartCharging();
            }
        }
        else if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            rb.velocity = chargeDirection * chargeSpeed;

            // Spawn trail effect
            if (Time.time - lastTrailSpawnTime > trailSpawnInterval)
            {
                SpawnTrail();
                lastTrailSpawnTime = Time.time;
            }

            // End charge if duration exceeded
            if (chargeTimer >= chargeDuration)
            {
                EndCharging();
            }
        }
        else
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0 && currentCharges < maxCharges)
            {
                StartPreparation();
            }
        }

        return currentCharges >= maxCharges ? TaskStatus.Success : TaskStatus.Running;
    }

    private void StartPreparation()
    {
        isPreparing = true;
        isCharging = false;
        preparationTimer = preparationTime;
        rb.velocity = Vector2.zero;
        animator.SetBool(IsDashingParam, false);
    }

    private void DetermineChargeDirection()
    {
        chargeDirection = (player.position - transform.position).normalized;
    }

    private void StartCharging()
    {
        isPreparing = false;
        isCharging = true;
        chargeTimer = 0;
        currentCharges++;
        animator.SetBool(IsDashingParam, true);
    }

    private void EndCharging()
    {
        isCharging = false;
        rb.velocity = Vector2.zero;
        animator.SetBool(IsDashingParam, false);

        if (currentCharges < maxCharges)
        {
            delayTimer = postChargeDelay;
        }
    }

    private void SpawnTrail()
    {
        GameObject trail = ObjectPool.Instance.GetObject(sodaTrailPrefab);
        trail.transform.position = transform.position;

        if (chargeDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(chargeDirection.y, chargeDirection.x) * Mathf.Rad2Deg;
            trail.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        StartCoroutine(ReturnTrailAfterDelay(trail, 10f));
    }

    private IEnumerator ReturnTrailAfterDelay(GameObject trail, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trail != null && trail.activeInHierarchy)
        {
            ObjectPool.Instance.PushObject(trail);
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCharging) return;

        if (collision.gameObject.CompareTag("Wall"))
        {
            EndCharging();
        }
        else if (collision.gameObject.CompareTag("Player") && Time.time > lastDamageTime + damageCooldown)
        {
            DealDamageToPlayer(collision.gameObject);
        }

        // 如果需要可以调用基类方法
        // base.OnCollisionEnter2D(collision);
    }

    private void DealDamageToPlayer(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(chargeDamage);
            lastDamageTime = Time.time;

            // Apply knockback
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    public override void OnEnd()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool(IsDashingParam, false);
    }
}