using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//不需要了
public class AudioManaager : MonoBehaviour
{
    public static AudioManaager Instance;
    public AudioType[] AudioTypes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach(AudioType audioType in AudioTypes)
        {
            audioType.source=gameObject.AddComponent<AudioSource>();

            audioType.source.clip = audioType.clip; 
            audioType.source.name = audioType.name;
            audioType.source.volume = audioType.volume;
            audioType.source.pitch = audioType.pitch;
            audioType.source.loop = audioType.loop;

            if(audioType.group!=null)
            {
                audioType.source.outputAudioMixerGroup = audioType.group;
            }
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(string name)
    {
        foreach(AudioType audioType in AudioTypes)
        {
            if(audioType.name.Equals(name))
            {
                audioType.source.Play();
                return;
            }
            Debug.LogWarning($"There is no audio name is '{name}'");
        }
    }
    public void Pause(string name)
    {
        foreach (AudioType audioType in AudioTypes)
        {
            if (audioType.name.Equals(name))
            {
                audioType.source.Pause();
                return;
            }
            Debug.LogWarning($"There is no audio name is '{name}'");
        }
    }
    public void Stop(string name)
    {
        foreach (AudioType audioType in AudioTypes)
        {
            if (audioType.name.Equals(name))
            {
                audioType.source.Stop();
                return;
            }
            Debug.LogWarning($"There is no audio name is '{name}'");
        }
    }
}
