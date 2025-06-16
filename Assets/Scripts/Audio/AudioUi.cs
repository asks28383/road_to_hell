using UnityEngine;
using UnityEngine.UI;

public class AudioUi : MonoBehaviour
{
    [Header("�������ƻ���")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // ��ʼ������ֵ���ӱ�������ü��أ�
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // ���¼�����
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

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
