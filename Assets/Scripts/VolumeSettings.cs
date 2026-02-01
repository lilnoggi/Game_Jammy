using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("References")]
    public AudioMixer audioMixer; // MainMixer goes here
    public Slider masterSlider; // Glover son or slider daughter...
    public Slider musicSlider;
    public Slider sfxSlider;

     void Start()
    {
        // Setup sliders based on saved preferences
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            //SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        // !!!!!!IMPORTANT!!!!!! AudioMixer works in decibels, so convert 0-1 range to -80 to 0 dB
        audioMixer.SetFloat("MusicVol", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);

        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFXVol", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);

        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        SetMusicVolume();
        SetSFXVolume();
    }
}
