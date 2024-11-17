using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoManager : MonoBehaviour
{
    private VideoPlayer vp;
    public GameObject StartCanvas;
    //public GameObject StartMenu;
    // Start is called before the first frame update
    void Start()
    {
        vp = GetComponentInChildren<VideoPlayer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StartCanvas.SetActive(!vp.isPlaying);
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
