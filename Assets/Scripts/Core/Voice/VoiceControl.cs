using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceControl : MonoBehaviour
{
    public List<AudioSource> bgmAudio;
    //public static List<AudioSource> bgm;
    public Slider volumeSlider;
    // Start is called before the first frame update
    void Start()
    {
        //bgm = bgmAudio;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(AudioSource audioSource in bgmAudio)
        {
            audioSource.volume = volumeSlider.value;
        }
    }
}
