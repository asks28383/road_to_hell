using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{

    public float BulletSpeed;
    
    private void FixedUpdate()
    {
        transform.position = transform.position + transform.up * Time.fixedDeltaTime * BulletSpeed;
    }
}
