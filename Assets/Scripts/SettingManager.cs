using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// …Ë÷√π‹¿Ì
/// </summary>
public class SettingManager : MonoBehaviour
{
    [Header("“Ù∆µªÏœÏ∆˜")]
    public AudioMixer mixer;

    public void SetBGMVolume(float value)
    {
        mixer.SetFloat("BGM", value);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFX", value);
    }
}
