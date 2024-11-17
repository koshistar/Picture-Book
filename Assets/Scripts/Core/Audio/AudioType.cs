using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


//不需要了
[System.Serializable]
public class AudioType
{
    public AudioSource source;
    public AudioClip clip;
    public AudioMixerGroup group;

    public string name;

    [Range(0f, 1f)] public float volume;
    [Range(0.1f,5f)] public float pitch;
    public bool loop;
}
