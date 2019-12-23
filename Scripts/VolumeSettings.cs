using UnityEngine;
using UnityEngine.UI;
public class VolumeSettings : MonoBehaviour
{

    public Slider effectSlider;
    public Slider backgroundSlider;
    public Button OKButton;
    public GameObject panel;
    void Awake()
    {
        effectSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetAudioVolume(effectSlider.value, backgroundSlider.value); });
        backgroundSlider.onValueChanged.AddListener(delegate { AudioManager.instance.SetAudioVolume(effectSlider.value, backgroundSlider.value); });
        OKButton.onClick.AddListener(() => Database.SaveVolumeData(new VolumeData(effectSlider.value, backgroundSlider.value)));
    }

    public void Toggle()
    {
        panel.SetActive(!panel.activeInHierarchy);
    }

    public void SetSliders(VolumeData volumeData)
    {
        effectSlider.value = volumeData.EffectsVolume;
        backgroundSlider.value = volumeData.MusicVolume;
    }

}
