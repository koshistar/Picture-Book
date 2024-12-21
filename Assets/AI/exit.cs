using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exit : MonoBehaviour
{
    public GameObject gb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void BackScene()
    {
        SceneManager.LoadScene(12);
    }
    public void Set()
    {
        gb.SetActive(true);
    }

    public void ContinueStory()
    {
        if (!GameData.hasGenerateText || !GameData.hasGenerateImage || !GameData.hasGenerateAudio)
        {
            SceneManager.LoadScene(12);
        }
        else
        {
            GameData.hasGenerateAudio = false;
            GameData.hasGenerateText = false;
            GameData.hasGenerateImage = false;
        
            SceneManager.LoadScene(CurrentScene.currentScene);   
        }
    }
}
