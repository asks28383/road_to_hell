using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    //需要控制的声音是什么
    private AudioSource menuAudio;
    //获得滑动条
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
    //控制声音音效
    public void VolumeControll()
    {
        //控制
        menuAudio.volume = audioSlider.value;
        //同时控制多个声音
        //获取到需要控制的声音，把声音的音量和滑动条挂钩
    }
    //关闭游戏设置界面
    public void CloseGameSettingUI()
    {

    }
}
