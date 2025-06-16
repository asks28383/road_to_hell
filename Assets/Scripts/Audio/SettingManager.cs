using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("��Ƶ�����")]
    public AudioMixer audioMixer; // �������AudioMixer

    private const string MASTER_VOL_KEY = "MasterVolume";
    private const string MUSIC_VOL_KEY = "MusicVolume";
    private const string SFX_VOL_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); // ��������ʱ��ȡ���������
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �������õ�PlayerPrefs
    public void SaveVolumeSettings(float masterVol, float musicVol, float sfxVol)
    {
        PlayerPrefs.SetFloat(MASTER_VOL_KEY, masterVol);
        PlayerPrefs.SetFloat(MUSIC_VOL_KEY, musicVol);
        PlayerPrefs.SetFloat(SFX_VOL_KEY, sfxVol);
        PlayerPrefs.Save();

        ApplyVolume(masterVol, musicVol, sfxVol);
    }

    // Ӧ����������
    private void ApplyVolume(float masterVol, float musicVol, float sfxVol)
    {
        if (audioMixer != null)
        {
            // �Ż��������ת������
            audioMixer.SetFloat("MasterVol", LinearToDecibel(masterVol));
            audioMixer.SetFloat("MusicVol", LinearToDecibel(musicVol));
            audioMixer.SetFloat("SFXVol", LinearToDecibel(sfxVol));
        }
    }

    // ����ֵ(0-1)ת�ֱ�ֵ(-80dB��0dB)���Ż�����
    private float LinearToDecibel(float linear)
    {
        // ��ȫ������ȷ��ֵ��0-1��Χ��
        linear = Mathf.Clamp01(linear);

        // �Ż����ת�����ߣ�
        // - 0 �� -80dB (��ȫ����)
        // - 0.3 �� -20dB (������)
        // - 0.6 �� -10dB (�е�����)
        // - 1 �� 0dB (�������)
        if (linear <= 0.0001f) return -80f; // ��ȫ����
        return Mathf.Log10(linear*1.5f) * 20f; // �Ż��������
    }

    // ���ر��������
    private void LoadSettings()
    {
        float masterVol = PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1f);
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);

        ApplyVolume(masterVol, musicVol, sfxVol);
    }
}