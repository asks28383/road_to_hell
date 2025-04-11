using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{

    public Image healthPointImage;
    public Image healthPointEffect;

    private EnemyHealth health;
    [SerializeField] private float hurtSpeed = 0.0003f;

    private void Awake()
    {
        health = GameObject.FindGameObjectWithTag("Boss").GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        healthPointImage.fillAmount = (float)(health.currentHealth * 1.0 / health.maxHealth);

        if (healthPointEffect.fillAmount >= healthPointImage.fillAmount)
        {
            healthPointEffect.fillAmount -= hurtSpeed;
        }
        else
        {
            healthPointEffect.fillAmount = healthPointImage.fillAmount;
        }
    }

}
