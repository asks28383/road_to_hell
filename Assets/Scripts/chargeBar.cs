using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class chargeBar : MonoBehaviour
{
    public Slider slider;
    public GameObject prefab;      // Ҫ���ɵ�����Ԥ����
    public Camera targetCamera;    // ����ת���õ����������Canvas��Ⱦģʽ���ã�
    public RectTransform fillarea;
    private void Start()
    {
        RectTransform fillArea = slider.fillRect.parent as RectTransform;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // ���ո����������
        {
            setjzBar();
        }
    }

    public void setjzBar()
    {
        int x = Random.Range(-1, 1);

        
    }
   
}
