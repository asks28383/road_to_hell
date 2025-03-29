using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBarController : MonoBehaviour
{
    public Transform target;         // 要跟随的角色
    public Vector3 offset = new Vector3(0, 2f, 0); // 头顶偏移量
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 跟随角色
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Camera.main.transform.rotation; // 始终面向相机
        }
    }
}
