using UnityEngine;

public class Boss : MonoBehaviour
{
    private Transform player;          // ��Ҳο�
    private SpriteRenderer spriteRenderer; // ������Ⱦ��

    void Start()
    {
        // ��ȡ�������
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ȷ����ʼ������ȷ������Ĭ���泯�ң�
        spriteRenderer.flipX = false;
    }

    void Update()
    {
        FlipTowardsPlayer();
    }

    // �򵥵����ҷ�ת�������
    private void FlipTowardsPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            // �����Boss�Ҳ� - �泯�ң�����ת��
            spriteRenderer.flipX = false;
        }
        else
        {
            // �����Boss��� - �泯�󣨷�ת��
            spriteRenderer.flipX = true;
        }
    }
}