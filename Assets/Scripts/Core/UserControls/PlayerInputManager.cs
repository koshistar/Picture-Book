using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public GameObject pausePanel;
    //public List<AudioSource> audioSources;
    //private int i = 1;
    private bool isAwake=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.Return))
        {
            PromptAdvance();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
    public void PromptAdvance()
    {
        Debug.Log("Get input");
        //if(i<1)
        //audioSources[i++].Play();
        DialogueSystem.instance.OnUserPrompt_Next();
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Pause()
    {
        if (isAwake)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        isAwake = !isAwake;
    }
}
