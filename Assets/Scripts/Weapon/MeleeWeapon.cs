using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MeleeWeapon : Weapon
{
    // ���ò��������ֲ��䣩
    [SerializeField] private int damages = 10;
    [SerializeField] private float attackCooldown = 0.5f;
    public Collider2D weaponCollider;
    public XuLiBar bar;
    public GameObject meleeHitBox;
    public GameObject rangedSlashPrefab;
    public GameObject poweredSlashPrefab;
    public float holdTimeForRanged = 2f;
    public float perfectChargeThreshold = 0.8f;
    public float meleeRange = 1.5f;
    public string bulletTag = "BossBullet";

    // ״̬���������ֲ��䣩
    protected float holdTimer;
    protected bool isHolding;
    protected bool isInPerfectWindow;
    protected Transform slashPoint;
    protected float cooldownTimer = 0f;
    protected bool canAttack = true;
    protected bool isAttacking = false;

    [Header("Sword Swing Settings")]
    public Transform swordTransform;
    public float swingAngle = 90f;
    public float swingDuration = 0.3f;
    private Quaternion originalSwordRotation;
    private bool isSwinging = false;
    private float swingTimer = 0f;

    [Header("��Чϵͳ")]
    public AudioMixerGroup sfxMixerGroup;  // ����Mixer Group����
    public AudioClip swingSound;          // �ӵ���Ч
    public AudioClip chargingStartSound;  // ��ʼ������Ч
    public AudioClip chargingLoopSound;   // ����ѭ����Ч
    public AudioClip perfectChargeSound;  // ����������ʾ��
    public AudioClip normalSlashSound;    // ��ͨ������Ч
    public AudioClip poweredSlashSound;   // ǿ��������Ч
    public AudioClip chargeReleaseSound;  // �����ͷ���Ч
    [Header("������Ч����")]
    public float perfectSoundDuration = 0.5f; // �ɵ���ʱ��
    [Range(0.1f, 3f)] public float maxVolume = 1.5f;
    [Range(0.1f, 2f)] public float pitchRange = 0.8f;

    private AudioSource swingSource;
    private AudioSource chargeSource;
    private AudioSource specialSource;

    public bool get_isHolding() => isHolding;
    public float get_holdTime() => holdTimer;

    protected override void Start()
    {
        // ��ʼ����ƵԴ������Mixer Group
        swingSource = gameObject.AddComponent<AudioSource>();
        //swingSource.outputAudioMixerGroup = sfxMixerGroup;

        chargeSource = gameObject.AddComponent<AudioSource>();
        //chargeSource.outputAudioMixerGroup = sfxMixerGroup;

        specialSource = gameObject.AddComponent<AudioSource>();
        //specialSource.outputAudioMixerGroup = sfxMixerGroup;

        // ������ƵԴ
        chargeSource.loop = true;
        chargeSource.volume = 0.8f;

        // ����3DЧ��
        swingSource.spatialBlend = 0;
        chargeSource.spatialBlend = 0;
        specialSource.spatialBlend = 0;

        base.Start();
        slashPoint = transform.Find("slashpoint");
        if (swordTransform != null)
        {
            originalSwordRotation = swordTransform.localRotation;
        }

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }


    // �ڹ���������ʼʱ���ã�ͨ�������¼���
    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
        isAttacking = true;
    }

    // �ڹ�����������ʱ���ã�ͨ�������¼���
    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
        isAttacking = false;
    }

    protected override void Update()
    {
        base.Update();
        UpdateChargingSound();

        if (!canAttack)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f) canAttack = true;
        }

        bar.Active(isHolding && holdTimer >= 0.3f);
        bar.UpdateCharge(holdTimer);

        HandleAttack();
    }

    protected override void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && canAttack && timer == 0)
        {
            StartAttackSequence();
        }

        if (Input.GetButton("Fire1") && isHolding)
        {
            UpdateHoldState();
        }

        if (Input.GetButtonUp("Fire1") && isHolding)
        {
            EndAttackSequence();
        }
    }
    private void StartAttackSequence()
    {
        isHolding = true;
        holdTimer = 0f;
        isInPerfectWindow = false;

        // ���Ż����ӵ���Ч
        swingSource.PlayOneShot(swingSound);
        // ���ſ�ʼ������Ч
        specialSource.PlayOneShot(chargingStartSound);
        // �ӳ���������ѭ����Ч
        StartCoroutine(StartChargingLoop(0.15f));
    }
    private IEnumerator StartChargingLoop(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isHolding)
        {
            chargeSource.clip = chargingLoopSound;
            chargeSource.Play();
        }
    }
    private IEnumerator PlayPerfectSoundWithDuration()
    {
        specialSource.PlayOneShot(perfectChargeSound);
        yield return new WaitForSeconds(perfectSoundDuration);
        specialSource.Stop(); // ֹͣ����
    }
    private void UpdateHoldState()
    {
        holdTimer += Time.deltaTime;

        // ���������������
        if (!isInPerfectWindow &&
            holdTimer >= holdTimeForRanged - perfectChargeThreshold &&
            holdTimer <= holdTimeForRanged + perfectChargeThreshold)
        {
            isInPerfectWindow = true;
            StartCoroutine(PlayPerfectSoundWithDuration());
        }
        else if (isInPerfectWindow &&
                 holdTimer > holdTimeForRanged + perfectChargeThreshold)
        {
            isInPerfectWindow = false;
        }
    }

    private void EndAttackSequence()
    {
        chargeSource.Stop();
        specialSource.PlayOneShot(chargeReleaseSound);

        if (holdTimer >= holdTimeForRanged)
        {
            ReleaseRangedSlash(isInPerfectWindow);
        }
        else
        {
            PerformMeleeAttack();
        }

        ResetAttackState();
    }

    private void UpdateChargingSound()
    {
        if (isHolding && chargeSource.isPlaying)
        {
            float progress = Mathf.Clamp01(holdTimer / holdTimeForRanged);
            chargeSource.pitch = 1.0f + (progress * pitchRange);
            chargeSource.volume = 0.7f + (progress * 0.3f) * maxVolume;

            if (isInPerfectWindow)
            {
                chargeSource.pitch += 0.2f;
                chargeSource.volume += 0.15f;
            }
        }
    }
    // ��Ƶ���뵭��Ч��
    private IEnumerator FadeAudio(AudioSource source, float targetVolume, float duration, bool stopAfterFade = false)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        if (stopAfterFade && targetVolume <= 0f)
        {
            source.Stop();
        }
    }

    // ��̬����������Ч������������ʱ��仯��
    //private void UpdateChargingSound()
    //{
    //    if (isHolding)
    //    {
    //        // ��������ʱ����������
    //        float pitch = Mathf.Lerp(0.8f, 1.2f, holdTimer / (holdTimeForRanged + perfectChargeThreshold));
    //        chargingAudioSource.pitch = pitch;

    //        // ��������ʱ����������
    //        float volume = Mathf.Lerp(2f, 3.5f, holdTimer / (holdTimeForRanged + perfectChargeThreshold));
    //        chargingAudioSource.volume = volume;
    //    }
    //}

    void StartSwordSwing()
    {
        if (swordTransform == null) return;
        isSwinging = true;
        swingTimer = 0f;
    }

    void UpdateSwordSwing()
    {
        if (swordTransform == null) return;

        swingTimer += Time.deltaTime;
        float swingProgress = Mathf.PingPong(swingTimer / swingDuration * 2, 1f);
        float easedProgress = EaseOutQuad(swingProgress);
        float baseAngle = easedProgress * swingAngle;

        if (transform.rotation.eulerAngles.x < 0f) // �������
        {
            swordTransform.localRotation = originalSwordRotation * Quaternion.Euler(0, 0, -baseAngle);
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.flipX = true;
                renderer.flipY = false;
            }
        }
        else // �����Ҳ�
        {
            swordTransform.localRotation = originalSwordRotation * Quaternion.Euler(0, 0, baseAngle);
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.flipX = false;
                renderer.flipY = false;
            }
        }

        if (swingTimer >= swingDuration)
        {
            isSwinging = false;
            swordTransform.localRotation = originalSwordRotation;
            if (swordTransform.TryGetComponent<SpriteRenderer>(out var resetRenderer))
            {
                resetRenderer.flipX = false;
            }
        }
    }

    float EaseOutQuad(float t) => t * (2 - t);

    protected virtual void PerformMeleeAttack()
    {
        TriggerAttackAnimation("Melee");
        //swingSource.PlayOneShot(swingSound);
        StartSwordSwing();
        EnableWeaponCollider();
        StartCoroutine(DisableColliderAfterDelay(swingDuration));
    }

    IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableWeaponCollider();
    }

    protected virtual void ReleaseRangedSlash(bool isPerfect)
    {
        TriggerAttackAnimation("Melee");
        StartSwordSwing();
        EnableWeaponCollider();
        StartCoroutine(DisableColliderAfterDelay(swingDuration));

        // ���Ŷ�Ӧ�Ľ�����Ч
        AudioClip slashSound = isPerfect ? poweredSlashSound : normalSlashSound;
        specialSource.PlayOneShot(slashSound);

        GameObject slashPrefab = isPerfect ? poweredSlashPrefab : rangedSlashPrefab;
        GameObject slash = ObjectPool.Instance.GetObject(slashPrefab);
        if (slash != null)
        {
            slash.transform.position = slashPoint.position;
            slash.GetComponent<Bullet>().SetSpeed(direction);
            slash.transform.right = direction;
        }
    }
    private void ResetAttackState()
    {
        isHolding = false;
        holdTimer = 0f;
        isInPerfectWindow = false;
        timer = interval;
        canAttack = false;
        cooldownTimer = attackCooldown;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking) return;

        // �����еĶ����Ƿ���Health���
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damages);
        }

        // ����Ƿ���пɴݻٵĵ�Ļ��ʹ�ñ�ǩ��⣩
        if (other.CompareTag(bulletTag))
        {
            // �ݻٵ�Ļ
            Destroy(other.gameObject);

            // ����ʹ�ö���ػ���
            // ObjectPool.Instance.ReturnObject(other.gameObject);
        }
    }
}