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
            // ������0-1ֵת��Ϊ����dBֵ��AudioMixerʹ�ö����̶ȣ�
            audioMixer.SetFloat("MasterVol", Mathf.Log10(masterVol) * 20);
            audioMixer.SetFloat("MusicVol", Mathf.Log10(musicVol) * 20);
            audioMixer.SetFloat("SFXVol", Mathf.Log10(sfxVol) * 20);
        }
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