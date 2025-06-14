using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHealth = 100;
    public float currentHealth;
    protected Animator animator;


    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damage)
    {
        
    }

    protected virtual void PlayHurtAnimation()
    {
        // ����ʵ��(����Ϊ��)
    }

    protected virtual void Die()
    {
        PlayDeathAnimation();
        //Destroy();
    }
    protected virtual void Destroy()
    {
        gameObject.SetActive(false);
    }

    protected virtual void PlayDeathAnimation()
    {
        // ����ʵ��(����Ϊ��)
    }
}
