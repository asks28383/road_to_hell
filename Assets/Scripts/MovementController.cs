using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Vector2 mousePos;    // �洢������������λ��
    public float movementSpeed = 3.0f;
    Vector2 movement = new Vector2();

    Animator animator;

    Rigidbody2D rb2D;



    // Start is called before the first frame update
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        UpdateRotation();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    // ���½�ɫ���򣨸������λ�ã�
    void UpdateRotation()
    {
        // �������Ļ����ת��Ϊ��������
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // �������Xλ�þ�����ɫ����
        if (mousePos.x > transform.position.x)
        {
            // ������Ҳࣺ����Ĭ����ת��0�ȣ�
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            // �������ࣺ��Y����ת180�ȣ�����ת��
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    private void MoveCharacter()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();
        rb2D.velocity = movement * movementSpeed;
    }

    private void UpdateState()
    {
        if (Mathf.Approximately(movement.x, 0) && Mathf.Approximately(movement.y, 0))
        {
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }
    }
}