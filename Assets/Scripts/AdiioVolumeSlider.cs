using UnityEngine;
using UnityEngine.UI;

public class AdiioVolumeSlider : MonoBehaviour
{

    public Slider slider;
    public bool isSFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        if (isSFX)
        {
            FMODUnity.RuntimeManager.StudioSystem.getParameterByName("GlobalSFXVolume", out float vol);
            slider.value = vol;
        }
        else
        {
            FMODUnity.RuntimeManager.StudioSystem.getParameterByName("GlobalMusicVolume", out float vol);
            slider.value = vol;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolumeOnSliderChanged(float volume)
    {
        if (isSFX)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GlobalSFXVolume", volume);
        }
        else
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GlobalMusicVolume", volume);
        }
    }
}
