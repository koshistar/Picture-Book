using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VideoManager : MonoBehaviour,IPointerMoveHandler
{
    public VideoPlayer vp;
    //public Text playButtonText;
    public Text videoTimeText;
    public Slider videoSlider;
    public Slider volumeSlider;
    [FormerlySerializedAs("volumnShowingTime")] public int volumeShowingTime = 3;
    public RectTransform videoBar;
    public GameObject StartCanvas;
    
    private string videoLengthString;
    private bool videoChanged = false;
    private int volumeTime = 0;

    private float barTargetY = -85;
    //public GameObject StartMenu;
    // Start is called before the first frame update
    void Start()
    {
        // vp = GetComponentInChildren<VideoPlayer>();
        int minutes = Mathf.FloorToInt((float)vp.clip.length / 60f);
        int seconds = (int)vp.clip.length - minutes * 60;
        videoLengthString = minutes.ToString("00") + ":" + seconds.ToString("00");
        videoSlider.onValueChanged.AddListener(OnVideoSliderChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumnSliderChanged);
        //这里会卡住，不知道为什么
        StartCoroutine("DelayForTimeText");
        StartCoroutine("VideoBarAnimation");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        barTargetY = 0;
    }

    private IEnumerator VideoBarAnimation()
    {
        WaitForSeconds waitTime = new WaitForSeconds(1f/30);
        float offsetY = 85 / 30f;
        int ShowFrame = 0;
        while (true)
        {
            yield return waitTime;
            float currentY = videoBar.anchoredPosition.y;
            if (barTargetY == 0 && barTargetY > currentY)
            {
                videoBar.anchoredPosition+=new Vector2(0f,offsetY);
            }
            else if(barTargetY == -85&&barTargetY<currentY)
            {
                videoBar.anchoredPosition-=new Vector2(0, offsetY);
            }

            if (videoBar.anchoredPosition.y == barTargetY && videoBar.anchoredPosition.y >= 0)
            {
                ShowFrame++;
                if (ShowFrame >= 90)
                {
                    barTargetY -= 85;
                    ShowFrame = 0;
                }
            }
        }
    }
    // Update is called once per frame
    // void FixedUpdate()
    // {
    //     StartCanvas.SetActive(!vp.isPlaying);
    // }

    public void OnPlayButtonDown()
    {
        if (vp.isPlaying)
        {
            vp.Pause();
            //playButtonText.text = "播放";
        }
        else
        {
            vp.Play();
            //playButtonText.text = "暂停";
        }
    }

    private IEnumerator DelayForTimeText()
    {
        WaitForSeconds waitTime = new WaitForSeconds(1);
        while (true)
        {
            if (videoChanged)
            {
                videoChanged = false;
            }
            else
            {
                int currentVideoTime=(int)vp.time;
                int minutes = Mathf.FloorToInt(currentVideoTime / 60f);
                int seconds = currentVideoTime - minutes * 60;
                videoTimeText.text = string.Format("{0:00}:{1:00} / ", minutes, seconds) + videoLengthString;
                videoSlider.SetValueWithoutNotify(currentVideoTime / (float)vp.clip.length);
                if (currentVideoTime == (int)vp.clip.length)
                {
                    LoadNextScene();
                }
                if (volumeSlider.gameObject.activeSelf)
                {
                    volumeTime++;
                    if (volumeTime >= volumeShowingTime)
                    {
                        volumeSlider.gameObject.SetActive(false);
                        volumeShowingTime = 0;
                    }
                }

                yield return waitTime;
            }
        }
    }

    private void OnVideoSliderChanged(float value)
    {
        vp.time = value * vp.clip.length;
        int currentVideoTime=(int)vp.time;
        int minutes = Mathf.FloorToInt(currentVideoTime / 60f);
        int seconds = currentVideoTime - minutes * 60;
        videoTimeText.text = string.Format("{0:00}:{1:00} / ", minutes, seconds) + videoLengthString;
        videoSlider.SetValueWithoutNotify(currentVideoTime / (float)vp.clip.length);
        videoChanged = true;
    }

    private void OnVolumnSliderChanged(float value)
    {
        vp.SetDirectAudioVolume(0, value);
        volumeTime = 0;
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
