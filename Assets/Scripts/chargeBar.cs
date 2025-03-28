using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class chargeBar : MonoBehaviour
{
    public Slider slider;
    public GameObject prefab;      // 要生成的物体预制体
    public Camera targetCamera;    // 坐标转换用的相机（根据Canvas渲染模式设置）
    public RectTransform fillarea;
    private void Start()
    {
        RectTransform fillArea = slider.fillRect.parent as RectTransform;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 按空格键测试生成
        {
            setjzBar();
        }
    }

    public void setjzBar()
    {
        int x = Random.Range(-1, 1);

        
    }
   
}
