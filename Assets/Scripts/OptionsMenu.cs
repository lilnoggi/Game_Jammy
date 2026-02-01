using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider volume_slider;

    public void ChangeVolume()
    {
        AudioListener.volume = volume_slider.value;
    }
}
