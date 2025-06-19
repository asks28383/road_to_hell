using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{

    public float BulletSpeed;

    public int flag=1;
    public float lateralSpeed = 100f; // 横向位移强度

    private float timeElapsed;
    private void FixedUpdate()
    {
        if(flag==1)
        {
            transform.position = transform.position + transform.up * Time.fixedDeltaTime * BulletSpeed;
        }
        else
        {
            timeElapsed += Time.fixedDeltaTime;

            // 基础前进运动（沿当前up方向）
            Vector3 forwardMovement = transform.up * BulletSpeed * Time.fixedDeltaTime;

            // 精确的横向位移：π/2 + sin(t)
            float lateralOffset = Mathf.PI / 2 + Mathf.Sin(timeElapsed);
            Vector3 sideMovement = transform.right * lateralOffset * lateralSpeed * Time.fixedDeltaTime;

            // 组合运动
            transform.position += forwardMovement + sideMovement;
        }
    }
}
