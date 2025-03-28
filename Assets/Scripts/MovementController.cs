using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Vector2 mousePos;    // 存储鼠标的世界坐标位置
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

    // 更新角色朝向（根据鼠标位置）
    void UpdateRotation()
    {
        // 将鼠标屏幕坐标转换为世界坐标
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 根据鼠标X位置决定角色朝向
        if (mousePos.x > transform.position.x)
        {
            // 鼠标在右侧：保持默认旋转（0度）
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            // 鼠标在左侧：绕Y轴旋转180度（镜像翻转）
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