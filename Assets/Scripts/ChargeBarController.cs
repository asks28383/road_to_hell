using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBarController : MonoBehaviour
{
    public Transform target;         // Ҫ����Ľ�ɫ
    public Vector3 offset = new Vector3(0, 2f, 0); // ͷ��ƫ����
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �����ɫ
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Camera.main.transform.rotation; // ʼ���������
        }
    }
}
