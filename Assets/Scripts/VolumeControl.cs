using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    //��Ҫ���Ƶ�������ʲô
    private AudioSource menuAudio;
    //��û�����
    private Slider audioSlider;
    void Start()//
    {
        menuAudio = GameObject.FindGameObjectWithTag("mainmenu").transform.GetComponent<AudioSource>();
        audioSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        VolumeControll();
    }
    //����������Ч
    public void VolumeControll()
    {
        //����
        menuAudio.volume = audioSlider.value;
        //ͬʱ���ƶ������
        //��ȡ����Ҫ���Ƶ��������������������ͻ������ҹ�
    }
    //�ر���Ϸ���ý���
    public void CloseGameSettingUI()
    {

    }
}
