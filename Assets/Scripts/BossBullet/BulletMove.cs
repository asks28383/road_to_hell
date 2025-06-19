using System;
using System.Collections;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float BulletSpeed = 5f;

    public enum MoveMode
    {
        straight,
        wound,
        rightward
    }

    public MoveMode movemode = MoveMode.straight;

    public float lateralSpeed = 1f; // 横向位移强度
    public float frequency = 5f;    // 蜿蜒频率
    private float timeElapsed;

    private void FixedUpdate()
    {
        timeElapsed += Time.fixedDeltaTime;

        if (movemode == MoveMode.straight)
        {
            //Debug.Log("straight bullet orbit");
            transform.position += transform.up * Time.fixedDeltaTime * BulletSpeed;
        }
        else if (movemode == MoveMode.wound)
        {
            //Debug.Log("wound bullet orbit");

            // 计算向前位移
            Vector3 forwardMovement = transform.up * BulletSpeed * Time.fixedDeltaTime;

            // 计算横向蜿蜒偏移
            float lateralOffset = Mathf.Sin(timeElapsed * frequency) * lateralSpeed * Time.fixedDeltaTime;
            Vector3 lateralMovement = transform.right * lateralOffset;

            // 综合移动
            transform.position += forwardMovement + lateralMovement;
        }
        else if (movemode == MoveMode.rightward)
        {
            Debug.Log("rightward drifting bullet");
            // 计算向前位移
            Vector3 forwardMovement = transform.up * BulletSpeed * Time.fixedDeltaTime;
            // 右偏移动量，cos 从 1 开始，向右偏移（可以换成 timeElapsed * timeElapsed 来增强偏移）
            float rightOffset =  lateralSpeed * Time.fixedDeltaTime;
            transform.position += forwardMovement + transform.right * rightOffset;
            
        }
    }
}
