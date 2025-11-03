using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TexttoSlide : MonoBehaviour
{
    [SerializeField]
    private Slider slide;
    [SerializeField]
    private TMP_InputField text;

    [SerializeField]
    private AudioMixerGroup mixer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slide.value = PlayerPrefs.GetFloat("volume", 100);
        text.text = PlayerPrefs.GetFloat("volume", 100) + "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendToSlider(string num)
    {
        if (num.Length > 0)
        {
            slide.value = float.Parse(num);
            mixer.audioMixer.SetFloat("MasterVolume", (slide.value / 100 * 80 - 80));
            PlayerPrefs.SetFloat("volume", slide.value);
        }
    }

    public void SendToText(float num)
    {
        text.text = num + "";
        mixer.audioMixer.SetFloat("MasterVolume", (num / 100 * 80 - 80));
        PlayerPrefs.SetFloat("volume", num);
    }
}
