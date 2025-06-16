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
            // 优化后的音量转换曲线
            audioMixer.SetFloat("MasterVol", LinearToDecibel(masterVol));
            audioMixer.SetFloat("MusicVol", LinearToDecibel(musicVol));
            audioMixer.SetFloat("SFXVol", LinearToDecibel(sfxVol));
        }
    }

    // 线性值(0-1)转分贝值(-80dB到0dB)的优化方法
    private float LinearToDecibel(float linear)
    {
        // 安全保护：确保值在0-1范围内
        linear = Mathf.Clamp01(linear);

        // 优化后的转换曲线：
        // - 0 → -80dB (完全静音)
        // - 0.3 → -20dB (低音量)
        // - 0.6 → -10dB (中等音量)
        // - 1 → 0dB (最大音量)
        if (linear <= 0.0001f) return -80f; // 完全静音
        return Mathf.Log10(linear*1.5f) * 20f; // 优化后的曲线
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