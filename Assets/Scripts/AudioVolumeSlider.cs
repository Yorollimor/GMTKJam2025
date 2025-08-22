using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeSlider : MonoBehaviour
{
    public Slider slider;
    public bool isSFX;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        if (isSFX)
        {
            if (PlayerPrefs.HasKey("GlobalSFXVolume"))
                slider.value = PlayerPrefs.GetFloat("GlobalSFXVolume");
            else
                slider.value = slider.maxValue / 2;
        }
        else
        {
            if (PlayerPrefs.HasKey("GlobalMusicVolume"))
                slider.value  = PlayerPrefs.GetFloat("GlobalMusicVolume");
            else
                slider.value = slider.maxValue / 2;
        }
    }

    public void SetVolumeOnSliderChanged(float volume)
    {
        if (isSFX)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GlobalSFXVolume", volume);
            PlayerPrefs.SetFloat("GlobalSFXVolume", volume);
        }
        else
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GlobalMusicVolume", volume);
            PlayerPrefs.SetFloat("GlobalMusicVolume", volume);
        }
    }
}