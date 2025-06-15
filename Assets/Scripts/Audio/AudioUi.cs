using UnityEngine;
using UnityEngine.UI;

public class AudioUi : MonoBehaviour
{
    [Header("绑定Slider控件")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 初始化Slider值（读取保存的设置）
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 添加实时监听
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    // Slider值变化时的回调
    private void OnMasterVolumeChanged(float value)
    {
        SettingsManager.Instance.SaveVolumeSettings(
            value,
            musicSlider.value,
            sfxSlider.value
        );
    }

    private void OnMusicVolumeChanged(float value)
    {
        SettingsManager.Instance.SaveVolumeSettings(
            masterSlider.value,
            value,
            sfxSlider.value
        );
    }

    private void OnSFXVolumeChanged(float value)
    {
        SettingsManager.Instance.SaveVolumeSettings(
            masterSlider.value,
            musicSlider.value,
            value
        );
    }
}
