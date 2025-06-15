using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("音频混合器")]
    public AudioMixer audioMixer; // 关联你的AudioMixer

    private const string MASTER_VOL_KEY = "MasterVolume";
    private const string MUSIC_VOL_KEY = "MusicVolume";
    private const string SFX_VOL_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); // 场景加载时读取保存的设置
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 保存设置到PlayerPrefs
    public void SaveVolumeSettings(float masterVol, float musicVol, float sfxVol)
    {
        PlayerPrefs.SetFloat(MASTER_VOL_KEY, masterVol);
        PlayerPrefs.SetFloat(MUSIC_VOL_KEY, musicVol);
        PlayerPrefs.SetFloat(SFX_VOL_KEY, sfxVol);
        PlayerPrefs.Save();

        ApplyVolume(masterVol, musicVol, sfxVol);
    }

    // 应用音量设置
    private void ApplyVolume(float masterVol, float musicVol, float sfxVol)
    {
        if (audioMixer != null)
        {
            // 将线性0-1值转换为对数dB值（AudioMixer使用对数刻度）
            audioMixer.SetFloat("MasterVol", Mathf.Log10(masterVol) * 20);
            audioMixer.SetFloat("MusicVol", Mathf.Log10(musicVol) * 20);
            audioMixer.SetFloat("SFXVol", Mathf.Log10(sfxVol) * 20);
        }
    }

    // 加载保存的设置
    private void LoadSettings()
    {
        float masterVol = PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1f);
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);

        ApplyVolume(masterVol, musicVol, sfxVol);
    }
}